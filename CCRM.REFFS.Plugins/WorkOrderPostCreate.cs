// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkOrderPostCreate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  WorkOrderPostCreate Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Text;

    public class WorkOrderPostCreate : Plugin
    {
        public WorkOrderPostCreate() : base(typeof(WorkOrderPostCreate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "msdyn_workorder", new Action<LocalPluginContext>(this.CreateInspections)));
        }
        ////Thid Plugin will fire on Work order post Create to create Inspection records.
        public void CreateInspections(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                tracingService.Trace("Work Order Post Create");
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity workOrder = context.InputParameters["Target"] as Entity;
                    if (workOrder.Attributes.Contains("msdyn_primaryincidenttype"))
                    {
                        Entity workorderrecord = service.Retrieve(workOrder.LogicalName, workOrder.Id, new ColumnSet("msdyn_serviceaccount", "smp_building", "msdyn_agreement", "msdyn_primaryincidenttype", "msdyn_customerasset", "ownerid"));
                        CreateInspections inspection = new CreateInspections();
                        inspection.CreateInspectionsforWorkOrders(service, workorderrecord, tracingService);
                    }

                    if (workOrder.Attributes.Contains("msdyn_servicerequest"))
                    {
                        EntityReference serviceRequest = workOrder.GetAttributeValue<EntityReference>("msdyn_servicerequest");
                        this.UpdateDynamicQuestionandAnsewrs(serviceRequest.Id, workOrder, service, tracingService);
                        Entity workorderrecord = new Entity(workOrder.LogicalName);
                        workorderrecord.Id = workOrder.Id;
                        workorderrecord.Attributes.Add("smp_dynamicproblemtypenotes", this.UpdateDynamicQuestionandAnsewrs(serviceRequest.Id, workOrder, service, tracingService));
                        service.Update(workorderrecord);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string UpdateDynamicQuestionandAnsewrs(Guid caseid, Entity workorder, IOrganizationService service, ITracingService tracingService)
        {
            StringBuilder formatedContent = new StringBuilder();
            ////formatedContent.Append("NA");
            QueryExpression query = new QueryExpression("smp_servicerequestproblemtypedesc");
            query.ColumnSet = new ColumnSet("smp_servicerequestproblemtypedescid", "smp_problemtypedescriptionid", "smp_answer");
            query.Criteria.AddFilter(LogicalOperator.Or);
            query.Criteria.AddCondition("smp_servicerequestid", ConditionOperator.Equal, caseid);
            var dynamicNotes = service.RetrieveMultiple(query);
            ////this tracing will be removed after the UAT testing.
            tracingService.Trace("Dynamic Questions" + dynamicNotes.Entities.Count);
            if (dynamicNotes.Entities != null && dynamicNotes.Entities.Count > 0)
            {
                formatedContent.Append(string.Empty);
                foreach (var dnotes in dynamicNotes.Entities)
                {
                    dnotes.Attributes.Add("smp_workorder", new EntityReference(workorder.LogicalName, workorder.Id));
                    service.Update(dnotes);
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