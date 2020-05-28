// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkOrderPostUpdate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  WorkOrderPostUpdate.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Text;

    public class WorkOrderPostUpdate : Plugin
    {
        /// <summary>
        /// WorkOrderPostUpdate Registration Method
        /// </summary>
        public WorkOrderPostUpdate() : base(typeof(WorkOrderPostUpdate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "msdyn_workorder", new Action<LocalPluginContext>(this.CreateInspections)));
        }

        /// <summary>
        /// CreateInspections
        /// </summary>
        /// <param name="localContext"></param>
        public void CreateInspections(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                tracingService.Trace("Work Order Post Update");
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity workOrder = context.InputParameters["Target"] as Entity;
                    Entity workOrderRecord = service.Retrieve(workOrder.LogicalName, workOrder.Id, new ColumnSet("msdyn_serviceaccount", "smp_dynamicproblemtypenotes", "smp_triggerwoemail", "msdyn_servicerequest", "msdyn_agreement", "msdyn_primaryincidenttype", "msdyn_customerasset", "ownerid"));
                    if (workOrder.Attributes.Contains("msdyn_primaryincidenttype"))
                    {
                        if (workOrderRecord.GetAttributeValue<EntityReference>("msdyn_agreement") != null)
                        {
                            CreateInspections inspection = new CreateInspections();
                            inspection.CreateInspectionsforWorkOrders(service, workOrderRecord, tracingService);
                        }
                    }

                    if (workOrder.Attributes.Contains("msdyn_serviceaccount"))
                    {
                        EntityReference account = workOrder.GetAttributeValue<EntityReference>("msdyn_serviceaccount");
                        if (account != null)
                        {
                            Entity accountrecord = service.Retrieve(account.LogicalName, account.Id, new ColumnSet("smp_tieroneprovider"));
                            Entity updatedworkorder = new Entity(workOrder.LogicalName, workOrder.Id);
                            updatedworkorder.Attributes.Add("smp_tier1provider", accountrecord.GetAttributeValue<string>("smp_tieroneprovider"));
                            service.Update(updatedworkorder);
                        }
                    }

                    if (workOrder.Attributes.Contains("msdyn_timetopromised") || workOrderRecord.GetAttributeValue<bool>("smp_triggerwoemail") != true)
                    {
                        Entity updateRecord = new Entity(workOrder.LogicalName);
                        updateRecord.Id = workOrder.Id;
                        if (workOrderRecord.Attributes.Contains("msdyn_servicerequest") && workOrderRecord.GetAttributeValue<bool>("smp_triggerwoemail") != true)
                        {
                            EntityCollection serviceRequestDynamicNotes = this.GetServiceRequestDynamicNotes(workOrderRecord.GetAttributeValue<EntityReference>("msdyn_servicerequest").Id, service);

                            if (serviceRequestDynamicNotes.Entities != null && serviceRequestDynamicNotes.Entities.Count > 0)
                            {
                                updateRecord.Attributes.Add("smp_dynamicproblemtypenotes", this.UpdateDynamicQuestionandAnswers(workOrderRecord.GetAttributeValue<EntityReference>("msdyn_servicerequest").Id, workOrder, service));
                            }

                            EntityCollection workorderDynamicNotes = this.GetWorkOrderDynamicNotes(workOrderRecord.Id, service);
                            if (workorderDynamicNotes.Entities != null && workorderDynamicNotes.Entities.Count > 0)
                            {
                                this.UpdateOldDPN(service, workOrderRecord.GetAttributeValue<EntityReference>("msdyn_servicerequest").Id, workOrder.Id, workorderDynamicNotes);
                            }
                        }

                        updateRecord.Attributes.Add("smp_triggerwoemail", true);
                        service.Update(updateRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// ServiceRequestDynamicNotes
        /// </summary>
        /// <param name="caseid"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private EntityCollection GetServiceRequestDynamicNotes(Guid caseid, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("smp_servicerequestproblemtypedesc");
            query.ColumnSet = new ColumnSet("smp_servicerequestproblemtypedescid", "smp_problemtypedescriptionid", "smp_answer");
            query.Criteria.AddFilter(LogicalOperator.Or);
            query.Criteria.AddCondition("smp_servicerequestid", ConditionOperator.Equal, caseid);
            EntityCollection dynamicNotes = service.RetrieveMultiple(query);
            return dynamicNotes;
        }

        /// <summary>
        /// WorkOrderDynamicNotes
        /// </summary>
        /// <param name="workorderid"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private EntityCollection GetWorkOrderDynamicNotes(Guid workorderid, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("smp_servicerequestproblemtypedesc");
            query.ColumnSet = new ColumnSet("smp_servicerequestproblemtypedescid", "smp_problemtypedescriptionid", "smp_answer", "new_value", "smp_servicerequestid", "smp_workorder");
            query.Criteria.AddFilter(LogicalOperator.And);
            query.Criteria.AddCondition("smp_workorder", ConditionOperator.Equal, workorderid);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            EntityCollection workorderdynamicNotes = service.RetrieveMultiple(query);
            return workorderdynamicNotes;
        }

        /// <summary>
        /// UpdateDynamicQuestionandAnsewrs
        /// </summary>
        /// <param name="caseid"></param>
        /// <param name="workorder"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private string UpdateDynamicQuestionandAnswers(Guid caseid, Entity workorder, IOrganizationService service)
        {
            StringBuilder formatedContent = new StringBuilder();
            EntityCollection dynamicNotes = this.GetServiceRequestDynamicNotes(caseid, service);
            if (dynamicNotes.Entities != null && dynamicNotes.Entities.Count > 0)
            {
                EntityReference providerTeam = GetProviderTeam(service, caseid);
                foreach (var dynamicNote in dynamicNotes.Entities)
                {
                    dynamicNote.Attributes.Add("smp_workorder", new EntityReference(workorder.LogicalName, workorder.Id));
                    if (providerTeam != null)
                    {
                        dynamicNote["ownerid"] = providerTeam;
                    }

                    service.Update(dynamicNote);
                    if (dynamicNote.GetAttributeValue<EntityReference>("smp_problemtypedescriptionid") != null)
                    {
                        string questionAnswer = dynamicNote.GetAttributeValue<EntityReference>("smp_problemtypedescriptionid").Name + "; " + dynamicNote.GetAttributeValue<string>("smp_answer") + "\n";
                        formatedContent.Append(questionAnswer);
                    }
                }
            }

            return formatedContent.ToString();
        }

        /// <summary>
        /// Update the Service Request of Old DPN  to new Service request
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRequestid"></param>
        /// <param name="dynamicNotes"></param>
        private void UpdateOldDPN(IOrganizationService service, Guid serviceRequestid, Guid workOrderId, EntityCollection dynamicNotes)
        {
            Entity caseOriginCode = this.GetServiceRequestOrigin(serviceRequestid, service);
            EntityReference caseOwner = caseOriginCode.GetAttributeValue<EntityReference>("ownerid");
            int origenCodeOld = 0;

            if (caseOriginCode != null && caseOriginCode.Contains("smp_referencesr"))
            {
                Entity caseOriginCodeforRefSR = this.GetServiceRequestOrigin(caseOriginCode.GetAttributeValue<EntityReference>("smp_referencesr").Id, service);
                origenCodeOld = caseOriginCodeforRefSR.GetAttributeValue<OptionSetValue>("caseorigincode").Value;
            }

            EntityReference providerTeam = GetProviderTeam(service, serviceRequestid);
            foreach (var entity in dynamicNotes.Entities)
            {
                if (entity.GetAttributeValue<EntityReference>("smp_servicerequestid").Id != serviceRequestid)
                {
                    ////Old SR is not created through Portal
                    ////if (origenCodeOld != 3)
                    ////{
                    Entity newDPN = new Entity(entity.LogicalName);

                    if (entity.Contains("smp_problemtypedescriptionid"))
                    {
                        newDPN["smp_problemtypedescriptionid"] = new EntityReference(entity.GetAttributeValue<EntityReference>("smp_problemtypedescriptionid").LogicalName, entity.GetAttributeValue<EntityReference>("smp_problemtypedescriptionid").Id);
                    }

                    if (entity.Contains("smp_servicerequestid"))
                    {
                        newDPN["smp_servicerequestid"] = new EntityReference(entity.GetAttributeValue<EntityReference>("smp_servicerequestid").LogicalName, serviceRequestid);
                    }

                    if (entity.Contains("smp_answer"))
                    {
                        newDPN["smp_answer"] = entity["smp_answer"];
                    }

                    if (entity.Contains("new_value"))
                    {
                        newDPN["new_value"] = entity["new_value"];
                    }

                    newDPN["smp_workorder"] = new EntityReference("msdyn_workorder", workOrderId);

                    if (providerTeam != null)
                    {
                        newDPN["ownerid"] = providerTeam;
                    }

                    service.Create(newDPN);
                    //}

                    //// update Old DPN with new SR
                    Entity dynamicProblemType = new Entity(entity.LogicalName, entity.Id);
                    dynamicProblemType["smp_workorder"] = null;
                    service.Update(dynamicProblemType);

                    if (caseOwner != null && providerTeam != null && caseOwner.Id != Guid.Empty && providerTeam.Id != Guid.Empty)
                    {
                        if (caseOwner.Id != providerTeam.Id)
                        {
                            Entity updateServiceRequest = new Entity("incident");
                            updateServiceRequest.Id = serviceRequestid;
                            updateServiceRequest.Attributes["ownerid"] = providerTeam;
                            service.Update(updateServiceRequest);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This Method retrieve the old SR Origin Code
        /// </summary>
        /// <param name="serviceRequesid"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private Entity GetServiceRequestOrigin(Guid serviceRequesid, IOrganizationService service)
        {
            Entity caseOriginCode = null;
            QueryExpression query = new QueryExpression("incident");
            query.ColumnSet = new ColumnSet("caseorigincode", "smp_referencesr", "ownerid");
            query.Criteria.AddFilter(LogicalOperator.And);
            query.Criteria.AddCondition("incidentid", ConditionOperator.Equal, serviceRequesid);
            EntityCollection caseCollection = service.RetrieveMultiple(query);
            if (caseCollection.Entities != null && caseCollection.Entities.Count > 0)
            {
                caseOriginCode = caseCollection.Entities[0];
            }

            return caseOriginCode;
        }

        private EntityReference GetProviderTeam(IOrganizationService service, Guid caseid)
        {
            ////trace.Trace(caseid.ToString());
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='incident'>
                                                            <attribute name='customerid' />
                                                            <attribute name='incidentid' />
                                                            <order attribute='incidentid' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='incidentid' operator='eq' value='" + caseid + @"' />
                                                            </filter>
                                                            <link-entity name='account' from='accountid' to='customerid' link-type='inner' alias='aa'>
                                                              <attribute name='smp_providerteam' />
                                                            </link-entity>
                                                          </entity>
                                                        </fetch>";
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            if (result != null && result.Entities.Count > 0)
            {
                AliasedValue providerTeamAlias = result.Entities[0].GetAttributeValue<AliasedValue>("aa.smp_providerteam");
                return (EntityReference)providerTeamAlias.Value;
            }

            return null;
        }
    }
}