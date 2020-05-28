// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkOrderPreCreate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  WorkOrderPreCreate Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Text;
    using CCRM.REF.TelemetryLog;

    public class WorkOrderPreCreate : Plugin
    {
        IRequestLogging requestLogging;
        Entity workOrder;
        public WorkOrderPreCreate() : base(typeof(WorkOrderPreCreate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, Constants.Create, "msdyn_workorder", new Action<LocalPluginContext>(this.PreCreateWorkOrder)));
        }
        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        protected void PreCreateWorkOrder(LocalPluginContext localContext)
        {
            try
            {
                CCRM.REF.TelemetryLog.LocalPluginContext localPluginContext = new CCRM.REF.TelemetryLog.LocalPluginContext(localContext.ServiceProvider);
                IConfigurationRetrieval configurationRetrieval = new ConfigurationRetrieval();
                requestLogging = new RequestLogging(configurationRetrieval, localPluginContext);
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    workOrder = context.InputParameters["Target"] as Entity;
                    if (workOrder.Attributes.Contains("msdyn_servicerequest"))
                    {
                        EntityReference serviceRequest = workOrder.GetAttributeValue<EntityReference>("msdyn_servicerequest");
                        Entity caserecord = service.Retrieve("incident", serviceRequest.Id, new ColumnSet("smp_requestedduedate", "smp_costcentercode",
                            "smp_iocode", "smp_problembuilding", "new_problemroomnumber", "smp_problemroom",
                            "smp_problemroomtype", "smp_problemtypedescription", "caseorigincode",
                            "smp_problemclassid", "smp_problemtypeid", "smp_cc", "smp_contact",
                            "smp_requestorid", "smp_priorityid", "smp_duedate",
                            "smp_duedatebybuildingtimezone", "smp_occureddatetimebybuildingtimezone",
                            "smp_submitteddatetimebybuildingtimezone", "smp_createddatetimebybuildingtimezone",
                            "smp_contactphone", "smp_contactemail", "smp_requestorphone", "smp_requestoremail"
                            , "msdyn_iotalert"));
                        Entity building = service.Retrieve(caserecord.GetAttributeValue<EntityReference>("smp_problembuilding").LogicalName,
                            caserecord.GetAttributeValue<EntityReference>("smp_problembuilding").Id,
                            new ColumnSet("smp_feedstoreid"));
                        EntityReference iotAlert = caserecord.GetAttributeValue<EntityReference>("msdyn_iotalert");
                        Entity iotAlertRecord = null;
                        if (iotAlert != null)
                        {
                            iotAlertRecord = service.Retrieve(caserecord.GetAttributeValue<EntityReference>("msdyn_iotalert").LogicalName, caserecord.GetAttributeValue<EntityReference>("msdyn_iotalert").Id, new ColumnSet("smp_correlationid", "msdyn_customerasset"));
                        }
                        if (building != null)
                        {
                            tracingService.Trace("mapping servicing account with Buliding Name");

                            QueryExpression query = new QueryExpression("account");
                            query.ColumnSet = new ColumnSet("smp_tieroneprovider");
                            query.Criteria.AddFilter(LogicalOperator.And);
                            query.Criteria.AddCondition(new ConditionExpression("smp_accounttype", ConditionOperator.Equal, 180620000));
                            query.Criteria.AddCondition(new ConditionExpression("accountnumber", ConditionOperator.Equal, building.GetAttributeValue<string>("smp_feedstoreid")));
                            EntityCollection entityCollection = service.RetrieveMultiple(query);

                            if (entityCollection.Entities.Count > 0)
                            {
                                workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", entityCollection.Entities[0].Id);
                                workOrder.Attributes["smp_tier1provider"] = entityCollection.Entities[0].GetAttributeValue<string>("smp_tieroneprovider");
                            }
                            else
                            {
                                tracingService.Trace("No Service Account existed with Same Building");
                                throw new InvalidPluginExecutionException("Building record does not exist in account entity, Service Request cannot dispatch. Map building record in account entity as account type is building.");
                            }
                        }

                        workOrder.Attributes["smp_contactphonenumber"] = caserecord.GetAttributeValue<string>("smp_contactphone");
                        workOrder.Attributes["smp_contactemail"] = caserecord.GetAttributeValue<string>("smp_contactemail");
                        workOrder.Attributes["smp_requestorphonenumber"] = caserecord.GetAttributeValue<string>("smp_requestorphone");
                        workOrder.Attributes["smp_requestoremail"] = caserecord.GetAttributeValue<string>("smp_requestoremail");
                        workOrder.Attributes["smp_costcentercode"] = caserecord.GetAttributeValue<string>("smp_costcentercode");
                        workOrder.Attributes["smp_iocode"] = caserecord.GetAttributeValue<string>("smp_iocode");
                        workOrder.Attributes["smp_building"] = new EntityReference(building.LogicalName, building.Id);
                        workOrder.Attributes["smp_room"] = caserecord.GetAttributeValue<EntityReference>("new_problemroomnumber");

                        if (caserecord.GetAttributeValue<EntityReference>("new_problemroomnumber") != null && caserecord.GetAttributeValue<EntityReference>("new_problemroomnumber").Name != string.Empty)
                        {
                            workOrder.Attributes["smp_problemroom"] = caserecord.GetAttributeValue<string>("smp_problemroom");
                        }

                        if (caserecord.GetAttributeValue<EntityReference>("smp_problemroomtype") != null)
                        {
                            workOrder.Attributes["smp_roomtype"] = caserecord.GetAttributeValue<EntityReference>("smp_problemroomtype");
                        }

                        if (caserecord.GetAttributeValue<string>("smp_problemtypedescription") != string.Empty)
                        {
                            workOrder.Attributes["msdyn_workordersummary"] = caserecord.GetAttributeValue<string>("smp_problemtypedescription");
                        }

                        workOrder.Attributes["smp_source"] = caserecord.GetAttributeValue<OptionSetValue>("caseorigincode");

                        if (caserecord.Attributes.Contains("smp_requestedduedate"))
                        {
                            AttributeHelper.MapDataTimeValue("smp_requestedduedate", caserecord, "msdyn_datewindowend", ref workOrder);
                        }

                        if (caserecord.Attributes.Contains("smp_duedate") && Convert.ToDateTime(caserecord["smp_duedate"]) != DateTime.MinValue)
                        {
                            workOrder.Attributes["msdyn_timetopromised"] = caserecord.GetAttributeValue<DateTime>("smp_duedate");
                        }

                        AttributeHelper.MapDataTimeValue("smp_duedate", caserecord, "smp_duedate", ref workOrder);
                        AttributeHelper.MapStringValue("smp_duedatebybuildingtimezone", caserecord, "smp_duedatebybuildingtimezone", ref workOrder);
                        AttributeHelper.MapStringValue("smp_occureddatetimebybuildingtimezone", caserecord, "smp_problemoccureddatetimebybuildingtimezone", ref workOrder);
                        AttributeHelper.MapStringValue("smp_submitteddatetimebybuildingtimezone", caserecord, "smp_submitteddatetimebybuildingtimezone", ref workOrder);
                        AttributeHelper.MapStringValue("smp_createddatetimebybuildingtimezone", caserecord, "smp_createddatetimebybuildingtimezone", ref workOrder);
                        EntityReference priority = caserecord.GetAttributeValue<EntityReference>("smp_priorityid");

                        if (priority != null)
                        {
                            QueryExpression priorityquery = new QueryExpression("msdyn_priority");
                            priorityquery.ColumnSet = new ColumnSet("msdyn_priorityid");
                            priorityquery.Criteria.AddCondition("msdyn_priority", "msdyn_name", ConditionOperator.Equal, priority.Name);
                            var prrecordcollection = service.RetrieveMultiple(priorityquery);
                            if (prrecordcollection.Entities != null && prrecordcollection.Entities.Count > 0)
                            {
                                Entity prrecord = prrecordcollection.Entities[0];
                                workOrder["msdyn_priority"] = new EntityReference(prrecord.LogicalName, prrecord.Id);
                            }

                            tracingService.Trace("Priority is mapped");
                        }

                        EntityReference problemClass = caserecord.GetAttributeValue<EntityReference>("smp_problemclassid");
                        EntityReference problemType = caserecord.GetAttributeValue<EntityReference>("smp_problemtypeid");
                        if (problemClass.Name != string.Empty)
                        {
                            QueryExpression problemquery = new QueryExpression("msdyn_workordertype");
                            problemquery.ColumnSet = new ColumnSet("msdyn_workordertypeid");
                            problemquery.Criteria.AddFilter(LogicalOperator.Or);
                            problemquery.Criteria.AddCondition("msdyn_workordertype", "msdyn_name", ConditionOperator.Equal, problemClass.Name.ToUpper());
                            problemquery.Criteria.AddCondition("msdyn_workordertype", "msdyn_name", ConditionOperator.Equal, problemClass.Name.ToLower());
                            problemquery.Criteria.AddCondition("msdyn_workordertype", "statecode", ConditionOperator.Equal, 0);
                            var problemclasscollection = service.RetrieveMultiple(problemquery);

                            if (problemclasscollection.Entities != null && problemclasscollection.Entities.Count > 0)
                            {
                                Entity problemclassrecord = problemclasscollection.Entities[0];
                                workOrder["msdyn_workordertype"] = new EntityReference(problemclassrecord.LogicalName, problemclassrecord.Id);
                            }

                            tracingService.Trace("WO Type is mapped");
                        }

                        if (problemType.Name != string.Empty)
                        {
                            QueryExpression problemtypequery = new QueryExpression("msdyn_incidenttype");
                            problemtypequery.ColumnSet = new ColumnSet("msdyn_incidenttypeid");
                            problemtypequery.Criteria.AddFilter(LogicalOperator.Or);
                            problemtypequery.Criteria.AddCondition("msdyn_incidenttype", "msdyn_name", ConditionOperator.Equal, problemType.Name.ToUpper());
                            problemtypequery.Criteria.AddCondition("msdyn_incidenttype", "msdyn_name", ConditionOperator.Equal, problemType.Name.ToLower());
                            problemtypequery.Criteria.AddCondition("msdyn_incidenttype", "statecode", ConditionOperator.Equal, 0);
                            var problemtypecollection = service.RetrieveMultiple(problemtypequery);

                            if (problemtypecollection.Entities != null && problemtypecollection.Entities.Count > 0)
                            {
                                Entity problemtyperecord = problemtypecollection.Entities[0];
                                workOrder["msdyn_primaryincidenttype"] = new EntityReference(problemtyperecord.LogicalName, problemtyperecord.Id);
                            }

                            tracingService.Trace("Incident Type is mapped");
                        }

                        EntityReference ccperson = caserecord.GetAttributeValue<EntityReference>("smp_cc");
                        if (ccperson != null)
                        {
                            workOrder["smp_ccperson"] = ccperson;
                            tracingService.Trace("CC is mapped");
                        }

                        EntityReference contactname = caserecord.GetAttributeValue<EntityReference>("smp_contact");
                        if (contactname != null)
                        {
                            workOrder["msdyn_reportedbycontact"] = contactname;
                            tracingService.Trace("contactname is mapped");
                        }

                        EntityReference requestorname = caserecord.GetAttributeValue<EntityReference>("smp_requestorid");
                        if (requestorname != null)
                        {
                            workOrder["smp_requestorname"] = requestorname;
                            tracingService.Trace("requestorname is mapped");
                        }

                        if (iotAlert != null)
                        {
                            workOrder["msdyn_iotalert"] = iotAlert;
                            tracingService.Trace("IOT Alert is mapped");
                            ////requestLogging.LogPluginTrace(workOrder, MappingConstants.WorkOrderCreatedSequenceId, MappingConstants.IOTAlertMappedSuccessEventId, MappingConstants.WorkOrderCreatedEventName, MappingConstants.IOTAlertMappedSuccessEventMessage);
                        }

                        if (iotAlertRecord != null)
                        {
                            if (iotAlertRecord.Attributes.Contains("smp_correlationid") && iotAlertRecord["smp_correlationid"] != null)
                            {
                                workOrder["smp_correlationid"] = iotAlertRecord.GetAttributeValue<string>("smp_correlationid");
                                ////requestLogging.LogPluginTrace(workOrder, MappingConstants.WorkOrderCreatedSequenceId, MappingConstants.CorrelationIDMappedSuccessEventId, MappingConstants.WorkOrderCreatedEventName, MappingConstants.CorrelationIDMappedSuccessEventMessage);
                            }

                            if (iotAlertRecord.Attributes.Contains("msdyn_customerasset") && iotAlertRecord["msdyn_customerasset"] != null)
                            {
                                EntityReference customerAsset = iotAlertRecord.GetAttributeValue<EntityReference>("msdyn_customerasset");
                                if (customerAsset != null)
                                {
                                    workOrder["msdyn_customerasset"] = customerAsset;
                                    ////requestLogging.LogPluginTrace(workOrder, MappingConstants.WorkOrderCreatedSequenceId, MappingConstants.CustomerAssetMappedSuccessEventId, MappingConstants.WorkOrderCreatedEventName, MappingConstants.CustomerAssetMappedSuccessEventMessage);
                                }
                            }
                        }

                        EntityReference agreement = workOrder.GetAttributeValue<EntityReference>("msdyn_agreement");
                        if (agreement == null)
                        {
                            Entity providermatrix = this.GetProviedmatrix(caserecord, service, tracingService);
                            if (providermatrix != null)
                            {
                                tracingService.Trace(providermatrix.GetAttributeValue<bool>("smp_billable").ToString());
                                workOrder["smp_billable"] = providermatrix.GetAttributeValue<bool>("smp_billable");
                            }
                        }

                        workOrder["smp_dynamicproblemtypenotes"] = this.GetDynamicQuestionandAnsewrs(caserecord, service, tracingService);
                    }

                    else if (workOrder.Attributes.Contains("msdyn_agreement"))
                    {
                        if (workOrder.Attributes.Contains("msdyn_serviceaccount"))
                        {
                            EntityReference account = workOrder.GetAttributeValue<EntityReference>("msdyn_serviceaccount");
                            Entity serviceaccount = service.Retrieve(account.LogicalName, account.Id, new ColumnSet("name"));
                            QueryExpression query = new QueryExpression("smp_building");
                            query.ColumnSet = new ColumnSet("smp_buildingid");
                            query.Criteria.AddFilter(LogicalOperator.Or);
                            query.Criteria.AddCondition(new ConditionExpression("smp_buildingname", ConditionOperator.Equal, serviceaccount.GetAttributeValue<string>("name").ToUpper()));
                            query.Criteria.AddCondition(new ConditionExpression("smp_buildingname", ConditionOperator.Equal, serviceaccount.GetAttributeValue<string>("name").ToLower()));
                            EntityCollection entityCollection = service.RetrieveMultiple(query);

                            if (entityCollection.Entities.Count > 0)
                            {
                                workOrder.Attributes["smp_building"] = new EntityReference("smp_building", entityCollection.Entities[0].Id);
                            }
                        }
                    }

                    ////requestLogging.LogPluginTrace(workOrder, MappingConstants.WorkOrderCreatedSequenceId, MappingConstants.WorkOrderCreatedSuccessEventId, MappingConstants.WorkOrderCreatedEventName, MappingConstants.WorkOrderCreatedSuccessEventMessage);
                }
            }
            catch (Exception ex)
            {
                ////requestLogging.LogPluginException(workOrder, ex, MappingConstants.WorkOrderCreatedSequenceId, MappingConstants.WorkOrderCreatedFailedEventId, MappingConstants.WorkOrderCreatedEventName, MappingConstants.WorkOrderCreatedFailedEventMessage);
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private Entity GetProviedmatrix(Entity caserecord, IOrganizationService service, ITracingService tracingService)
        {
            tracingService.Trace("Retrieving Provider Matrix");
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
                tracingService.Trace("Provieder Matrix find");
            }

            return proviedMatrix;
        }

        private string GetDynamicQuestionandAnsewrs(Entity caserecord, IOrganizationService service, ITracingService tracingService)
        {
            StringBuilder formatedContent = new StringBuilder();

            QueryExpression query = new QueryExpression("smp_servicerequestproblemtypedesc");
            query.ColumnSet = new ColumnSet("smp_problemtypedescriptionid", "smp_answer");
            query.Criteria.AddFilter(LogicalOperator.Or);
            query.Criteria.AddCondition("smp_servicerequestid", ConditionOperator.Equal, caserecord.Id);

            var dynamicNotes = service.RetrieveMultiple(query);
            if (dynamicNotes.Entities != null && dynamicNotes.Entities.Count > 0)
            {
                foreach (var dnotes in dynamicNotes.Entities)
                {
                    if (dnotes.GetAttributeValue<EntityReference>("smp_problemtypedescriptionid") != null)
                    {
                        string qa = dnotes.GetAttributeValue<EntityReference>("smp_problemtypedescriptionid").Name + "; " + dnotes.GetAttributeValue<string>("smp_answer") + "\n";
                        formatedContent.Append(qa);
                    }
                }
            }

            return formatedContent.ToString();
        }
    }
}