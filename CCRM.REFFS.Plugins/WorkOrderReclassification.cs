// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkOrderReclassification.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  WorkOrderReclassification
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using CCRM.REFFS.Plugins.Helpers;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;

    public class WorkOrderReclassification : Plugin
    {
        public WorkOrderReclassification() : base(typeof(WorkOrderReclassification))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Update, "msdyn_workorder", new Action<LocalPluginContext>(this.WorkOrderReclassificationinSR)));
        }

        /// <summary>
        /// Work Order Reclassification
        /// </summary>
        /// <param name="localContext"></param>
        public void WorkOrderReclassificationinSR(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity target = context.InputParameters["Target"] as Entity;
                    if (target.Attributes.Contains("msdyn_workordertype") || target.Attributes.Contains("msdyn_primaryincidenttype"))
                    {
                        Entity postImage = context.PostEntityImages["PostImage"] as Entity;

                        tracingService.Trace("Post" + postImage.GetAttributeValue<EntityReference>("msdyn_workordertype").Name);
                        EntityReference servicerequest = postImage.GetAttributeValue<EntityReference>("msdyn_servicerequest");
                        if (servicerequest != null)
                        {
                            ////we are cloning this record so required all the attributes [new ColumnSet(true)].
                            Entity originalcaserecord = service.Retrieve("incident", servicerequest.Id, new ColumnSet(true));
                            this.CreateNewServiceRequest(originalcaserecord, postImage, service, tracingService, target);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// GetQueryResponse
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="fieldsToBeFetched"></param>
        /// <param name="criteriaField"></param>
        /// <param name="criteriaValue"></param>
        /// <returns></returns>
        private static EntityCollection GetQueryResponse(IOrganizationService service, string entityLogicalName, string[] fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = entityLogicalName;
                query.ColumnSet = new ColumnSet(fieldsToBeFetched);
                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression(criteriaField, ConditionOperator.Equal, criteriaValue));
                query.Criteria = filter;
                EntityCollection entityCollection = service.RetrieveMultiple(query);
                return entityCollection;
            }

            return null;
        }

        /// <summary>
        /// Create Reclassified Service Request
        /// </summary>
        /// <param name="originalcaserecord"></param>
        /// <param name="postImage"></param>
        /// <param name="service"></param>
        /// <param name="tracingService"></param>
        /// <param name="workorder"></param>
        private void CreateNewServiceRequest(Entity originalcaserecord, Entity postImage, IOrganizationService service, ITracingService tracingService, Entity workorder)
        {
            try
            {
                Entity newSR = new Entity("incident");
                string[] strAttributesServiceRequestToRemove = new string[] { "ticketnumber", "title", "statuscode", "customerid", "smp_referencesr", "incidentid", "createdon", "createdby", "modifiedon", "modifiedby", "smp_issurveyservicerequest", "smp_cancelledforreclassification", "smp_slamet", "smp_priorityid", "smp_submitteddatetime", "smp_problemoccureddatetime", "smp_duedate", "smp_tier1workcompletiondatebybuildingtimezone", "smp_completeddate", "smp_completeddatebybuildingtimezone", "smp_requestedduedate", "smp_createddatetimebybuildingtimezone", "smp_submitteddatetimebybuildingtimezone", "smp_occureddatetimebybuildingtimezone", "smp_duedatebybuildingtimezone", "smp_providerduedatebybuildingtimezone", "ownerid", "smp_duedatetimebybuildingtimezone", "smp_statusonhold","smp_problemtypedescriptionid", "smp_answer" };
                newSR = this.CloneRecordForEntity("incident", originalcaserecord, strAttributesServiceRequestToRemove);
                newSR.Attributes["customerid"] = new EntityReference("account", this.GetProviderId(service, "Not Assigned"));
                newSR.Attributes["smp_referencesr"] = new EntityReference("incident", originalcaserecord.Id);
                newSR.Attributes["caseorigincode"] = new OptionSetValue((int)ServiceRequestEnum.ServiceRequestOrigin.Phone);
                newSR.Attributes["smp_createdfrom"] = new OptionSetValue(1);
                newSR.Attributes["smp_problemoccureddatetime"] = Convert.ToDateTime(DateTime.UtcNow, CultureInfo.CurrentCulture);
                newSR.Attributes["smp_flag"] = true;
                newSR.Attributes["smp_portalsubmit"] = false;
                newSR.Attributes["statuscode"] = new OptionSetValue((int)ServiceRequestEnum.StatusCode.Draft);
                newSR.Attributes["smp_reclassifiedfromworkorder"] = true;
                Entity problemclassrecord = this.GetProblemClass(service, postImage);
                newSR.Attributes["smp_problemclassid"] = new EntityReference(problemclassrecord.LogicalName, problemclassrecord.Id);
                Entity problemtyperecord = this.GetProblemType(service, postImage);
                newSR.Attributes["smp_problemtypeid"] = new EntityReference(problemtyperecord.LogicalName, problemtyperecord.Id);

                Guid newincident = service.Create(newSR);
                this.SetServiceRequestTitle(service, newincident, tracingService);
                this.UpdateOriginalSR(originalcaserecord, service, newincident, postImage);
                this.AssociateWorkOrder(service, newincident, workorder.Id, tracingService);
                this.SetServiceRequestStatus(service, 2, (int)ServiceRequestEnum.StatusCode.Cancelled, originalcaserecord.Id, tracingService); ////Cancel Orginal SR
                this.SetServiceRequestStatus(service, 0, (int)ServiceRequestEnum.StatusCode.Open, newincident, tracingService); //setting Open Status
                this.SetServiceRequestStatus(service, 0, (int)ServiceRequestEnum.StatusCode.Dispatched, newincident, tracingService); ///setting Dispacted Status
                this.UpdatePriorityinWO(newincident, service, tracingService, workorder);
            }
            catch (Exception ex)
            {
                throw new Exception("error in Reclassified SR Creation" + ex.Message);
            }
        }

        /// <summary>
        /// UpdatePriority in Work Order
        /// </summary>
        /// <param name="newsrid"></param>
        /// <param name="service"></param>
        /// <param name="tracingService"></param>
        /// <param name="workorder"></param>
        private void UpdatePriorityinWO(Guid newsrid, IOrganizationService service, ITracingService tracingService, Entity workorder)
        {
            try
            {
                Entity newSRRecord = service.Retrieve("incident", newsrid, new ColumnSet("smp_priorityid", "smp_createddatetimebybuildingtimezone", "smp_occureddatetimebybuildingtimezone", "smp_submitteddatetimebybuildingtimezone", "smp_duedatebybuildingtimezone", "smp_duedate", "smp_problembuilding", "smp_problemclassid", "smp_problemtypeid"));
                Entity providermatrix = this.GetProviedMatrix(newSRRecord, service, tracingService);
                Entity updateWorkOrder = new Entity(workorder.LogicalName);
                updateWorkOrder.Id = workorder.Id;
                if (providermatrix != null)
                {
                    workorder["smp_billable"] = providermatrix.GetAttributeValue<bool>("smp_billable");
                }

                if (newSRRecord.GetAttributeValue<EntityReference>("smp_priorityid") != null)
                {
                    Entity customPriority = service.Retrieve("smp_priority", newSRRecord.GetAttributeValue<EntityReference>("smp_priorityid").Id, new ColumnSet("smp_name"));
                    tracingService.Trace("Priority:" + customPriority.Id);
                    QueryExpression priorityQuery = new QueryExpression("msdyn_priority");
                    priorityQuery.ColumnSet = new ColumnSet("msdyn_priorityid");
                    priorityQuery.Criteria.AddFilter(LogicalOperator.Or);
                    priorityQuery.Criteria.AddCondition("msdyn_name", ConditionOperator.Equal, customPriority.GetAttributeValue<string>("smp_name").ToUpper());
                    priorityQuery.Criteria.AddCondition("msdyn_name", ConditionOperator.Equal, customPriority.GetAttributeValue<string>("smp_name").ToLower());
                    EntityCollection fspriorities = service.RetrieveMultiple(priorityQuery);

                    if (fspriorities.Entities != null && fspriorities.Entities.Count > 0)
                    {
                        tracingService.Trace("Updating WO with PR:" + fspriorities.Entities.Count);

                        updateWorkOrder.Attributes.Add("msdyn_priority", new EntityReference("msdyn_priority", fspriorities.Entities[0].Id));
                        ////SLA Dates mappings to WO
                        if (newSRRecord.Attributes.Contains("smp_createddatetimebybuildingtimezone") && newSRRecord.GetAttributeValue<string>("smp_createddatetimebybuildingtimezone") != string.Empty)
                        {
                            updateWorkOrder.Attributes.Add("smp_createddatetimebybuildingtimezone", newSRRecord.GetAttributeValue<string>("smp_createddatetimebybuildingtimezone"));
                        }

                        if (newSRRecord.Attributes.Contains("smp_occureddatetimebybuildingtimezone") && newSRRecord.GetAttributeValue<string>("smp_occureddatetimebybuildingtimezone") != string.Empty)
                        {
                            updateWorkOrder.Attributes.Add("smp_problemoccureddatetimebybuildingtimezone", newSRRecord.GetAttributeValue<string>("smp_occureddatetimebybuildingtimezone"));
                        }

                        if (newSRRecord.Attributes.Contains("smp_submitteddatetimebybuildingtimezone") && newSRRecord.GetAttributeValue<string>("smp_submitteddatetimebybuildingtimezone") != string.Empty)
                        {
                            updateWorkOrder.Attributes.Add("smp_submitteddatetimebybuildingtimezone", newSRRecord.GetAttributeValue<string>("smp_submitteddatetimebybuildingtimezone"));
                        }

                        if (newSRRecord.Attributes.Contains("smp_duedatebybuildingtimezone"))
                        {
                            if (newSRRecord.GetAttributeValue<string>("smp_duedatebybuildingtimezone") != string.Empty)
                            {
                                updateWorkOrder.Attributes.Add("smp_duedatebybuildingtimezone", newSRRecord.GetAttributeValue<string>("smp_duedatebybuildingtimezone"));
                            }
                        }
                        else
                        {
                            updateWorkOrder.Attributes.Add("smp_duedatebybuildingtimezone", string.Empty);
                        }

                        if (newSRRecord.Attributes.Contains("smp_duedate"))
                        {
                            if (newSRRecord.GetAttributeValue<DateTime>("smp_duedate") != null)
                            {
                                updateWorkOrder.Attributes.Add("smp_duedate", Convert.ToDateTime(newSRRecord.GetAttributeValue<DateTime>("smp_duedate")));
                            }
                        }
                        else
                        {
                            updateWorkOrder.Attributes.Add("smp_duedate", null);
                        }
                    }

                    //// for copy new DPN notes 
                    updateWorkOrder.Attributes.Add("smp_triggerwoemail", false);
                    service.Update(updateWorkOrder);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// GetProviedMatrix
        /// </summary>
        /// <param name="caserecord"></param>
        /// <param name="service"></param>
        /// <param name="tracingService"></param>
        /// <returns></returns>
        private Entity GetProviedMatrix(Entity caserecord, IOrganizationService service, ITracingService tracingService)
        {
            Entity proviedMatrix = null;
            QueryExpression proviedMatrixQuery = new QueryExpression("smp_providermatrix");
            proviedMatrixQuery.ColumnSet = new ColumnSet("smp_billable");
            proviedMatrixQuery.Criteria.AddFilter(LogicalOperator.And);
            proviedMatrixQuery.Criteria.AddCondition("smp_buildingid", ConditionOperator.Equal, caserecord.GetAttributeValue<EntityReference>("smp_problembuilding").Id);
            proviedMatrixQuery.Criteria.AddCondition("smp_problemclassid", ConditionOperator.Equal, caserecord.GetAttributeValue<EntityReference>("smp_problemclassid").Id);
            proviedMatrixQuery.Criteria.AddCondition("smp_problemtypeid", ConditionOperator.Equal, caserecord.GetAttributeValue<EntityReference>("smp_problemtypeid").Id);
            var problemtypecollection = service.RetrieveMultiple(proviedMatrixQuery);
            if (problemtypecollection.Entities != null && problemtypecollection.Entities.Count > 0)
            {
                proviedMatrix = problemtypecollection.Entities[0];
            }

            return proviedMatrix;
        }

        /// <summary>
        /// AssociateWorkOrder
        /// </summary>
        /// <param name="service"></param>
        /// <param name="targetid"></param>
        /// <param name="relatedrecord"></param>
        /// <param name="tracing"></param>
        private void AssociateWorkOrder(IOrganizationService service, Guid targetid, Guid relatedrecord, ITracingService tracing)
        {
            try
            {
                AssociateRequest mapworkorder = new AssociateRequest
                {
                    Target = new EntityReference("incident", targetid),
                    RelatedEntities = new EntityReferenceCollection { new EntityReference("msdyn_workorder", relatedrecord) },
                    Relationship = new Relationship("msdyn_incident_msdyn_workorder_ServiceRequest")
                };

                service.Execute(mapworkorder);
            }
            catch (Exception ex)
            {
                throw new Exception("error in WO association with Reclassified SR" + ex.Message);
            }
        }

        /// <summary>
        /// UpdateOriginalSR
        /// </summary>
        /// <param name="originalcase"></param>
        /// <param name="service"></param>
        /// <param name="newsrid"></param>
        /// <param name="workorder"></param>
        private void UpdateOriginalSR(Entity originalcase, IOrganizationService service, Guid newsrid, Entity workorder)
        {
            try
            {
                Entity newsr = service.Retrieve("incident", newsrid, new ColumnSet("ticketnumber"));
                Entity worder = service.Retrieve(workorder.LogicalName, workorder.Id, new ColumnSet("smp_reclassificationreason", "smp_comment"));
                string comments = string.Empty;
                string overallReason = string.Empty;
                if (workorder.Attributes.Contains("smp_reclassificationreason") && workorder.GetAttributeValue<OptionSetValue>("smp_reclassificationreason") != null)
                {
                    string selectedreason = AttributeHelper.GetOptionSetValueLabel(workorder.LogicalName, "smp_reclassificationreason", workorder.GetAttributeValue<OptionSetValue>("smp_reclassificationreason").Value, service);
                    if (workorder.GetAttributeValue<OptionSetValue>("smp_reclassificationreason").Value == 180620002)
                    {
                        comments = workorder.GetAttributeValue<string>("smp_comment");
                        overallReason = selectedreason + " : <" + comments + "> " + newsr.GetAttributeValue<string>("ticketnumber");
                    }
                    else
                    {
                        overallReason = selectedreason + " " + newsr.GetAttributeValue<string>("ticketnumber");
                    }

                    originalcase.Attributes["smp_requestcancelledreason"] = overallReason;
                    originalcase.Attributes["smp_reasonforreclassifyingtherecordmemo"] = overallReason;
                    originalcase.Attributes["smp_cancelledforreclassification"] = true;
                    originalcase.Attributes["smp_cancellingfromwo"] = true;
                    originalcase.Attributes["smp_reclassifiedsr"] = new EntityReference("incident", newsrid);
                    service.Update(originalcase);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("error in Reclassified SR update " + ex.Message);
            }
        }

        /// <summary>
        /// SetServiceRequestStatus
        /// </summary>
        /// <param name="service"></param>
        /// <param name="state"></param>
        /// <param name="status"></param>
        /// <param name="caseid"></param>
        /// <param name="tracing"></param>
        private void SetServiceRequestStatus(IOrganizationService service, int state, int status, Guid caseid, ITracingService tracing)
        {
            try
            {
                SetStateRequest request = new SetStateRequest();
                request.EntityMoniker = new EntityReference("incident", caseid);
                request.State = new OptionSetValue(state);
                request.Status = new OptionSetValue(status);
                service.Execute(request);
            }
            catch (Exception ex)
            {
                throw new Exception("error in Reclassified SR Status Update " + ex.Message);
            }
        }

        private void SetServiceRequestTitle(IOrganizationService service, Guid newsrid, ITracingService tracing)
        {
            try
            {
                Entity newsr = service.Retrieve("incident", newsrid, new ColumnSet("ticketnumber"));
                Entity updateServiceRequest = new Entity("incident");
                updateServiceRequest.Id = newsrid;
                updateServiceRequest.Attributes["title"] = newsr.GetAttributeValue<string>("ticketnumber");
                service.Update(updateServiceRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("error in Reclassified SR Status Update " + ex.Message);
            }
        }

        /// <summary>
        /// CloneRecordForEntity
        /// </summary>
        /// <param name="targetEntityName"></param>
        /// <param name="sourceEntity"></param>
        /// <param name="strAttributestoRemove"></param>
        /// <returns></returns>
        private Entity CloneRecordForEntity(string targetEntityName, Entity sourceEntity, string[] strAttributestoRemove)
        {
            Entity clonedEntity = new Entity(targetEntityName);
            AttributeCollection attributeKeys = sourceEntity.Attributes;
            foreach (string key in attributeKeys.Keys)
            {
                //// Check if key is not there in the list of removed keys
                if (Array.IndexOf(strAttributestoRemove, key) == -1)
                {
                    clonedEntity[key] = sourceEntity[key];
                }
            }

            return clonedEntity;
        }

        /// <summary>
        /// GetProblemClass
        /// </summary>
        /// <param name="service"></param>
        /// <param name="postImage"></param>
        /// <returns></returns>
        private Entity GetProblemClass(IOrganizationService service, Entity postImage)
        {
            Entity problemclassrecord = new Entity();
            QueryExpression problemquery = new QueryExpression("smp_problemclass");
            problemquery.ColumnSet = new ColumnSet("smp_problemclassid");
            problemquery.Criteria.AddFilter(LogicalOperator.Or);
            problemquery.Criteria.AddCondition("smp_problemclass", "smp_problemclassname", ConditionOperator.Equal, postImage.GetAttributeValue<EntityReference>("msdyn_workordertype").Name.ToUpper());
            problemquery.Criteria.AddCondition("smp_problemclass", "smp_problemclassname", ConditionOperator.Equal, postImage.GetAttributeValue<EntityReference>("msdyn_workordertype").Name.ToLower());
            var problemclasscollection = service.RetrieveMultiple(problemquery);
            if (problemclasscollection.Entities != null && problemclasscollection.Entities.Count > 0)
            {
                problemclassrecord = problemclasscollection.Entities[0];
            }

            return problemclassrecord;
        }

        /// <summary>
        /// GetProblemType
        /// </summary>
        /// <param name="service"></param>
        /// <param name="postImage"></param>
        /// <returns></returns>
        private Entity GetProblemType(IOrganizationService service, Entity postImage)
        {
            Entity problemtyperecord = new Entity();
            QueryExpression problemtype = new QueryExpression("smp_problemtype");
            problemtype.ColumnSet = new ColumnSet("smp_problemtypeid");
            problemtype.Criteria.AddFilter(LogicalOperator.Or);
            problemtype.Criteria.AddCondition("smp_problemtype", "smp_problemtypename", ConditionOperator.Equal, postImage.GetAttributeValue<EntityReference>("msdyn_primaryincidenttype").Name.ToUpper());
            problemtype.Criteria.AddCondition("smp_problemtype", "smp_problemtypename", ConditionOperator.Equal, postImage.GetAttributeValue<EntityReference>("msdyn_primaryincidenttype").Name.ToLower());
            var problemTypeclasscollection = service.RetrieveMultiple(problemtype);
            if (problemTypeclasscollection.Entities != null && problemTypeclasscollection.Entities.Count > 0)
            {
                problemtyperecord = problemTypeclasscollection.Entities[0];
            }

            return problemtyperecord;
        }

        /// <summary>
        /// GetProviderId
        /// </summary>
        /// <param name="service"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        private Guid GetProviderId(IOrganizationService service, string providerName)
        {
            Guid providerId = Guid.Empty;
            EntityCollection entityCollection = GetQueryResponse(service, "account", new string[] { "accountid" }, "name", providerName);

            if (entityCollection != null && entityCollection.Entities.Count > 0)
            {
                Entity entity = entityCollection.Entities[0];
                if (entity.Attributes.Contains("accountid"))
                {
                    providerId = entity.Id;
                }
            }

            return providerId;
        }
    }
}