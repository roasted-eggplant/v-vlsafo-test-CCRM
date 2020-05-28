// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignToFieldServiceTeam.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AssignToFieldServiceTeam Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class AssignToFieldServiceTeam : Plugin
    {
        public AssignToFieldServiceTeam() : base(typeof(AssignToFieldServiceTeam))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, "msdyn_workorder", new Action<LocalPluginContext>(this.AssigntoTeam)));
        }

        protected void AssigntoTeam(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                tracingService.Trace("Assign Work Order to filed service team");
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity target = context.InputParameters["Target"] as Entity;

                    if (target.Attributes.Contains("msdyn_servicerequest"))
                    {
                        ////tracingService.Trace("Work Order Type is Changed");
                        Entity postImage = context.PostEntityImages["PostImage"] as Entity;
                        EntityReference servicerequest = postImage.GetAttributeValue<EntityReference>("msdyn_servicerequest");
                        Entity originalcaserecord = service.Retrieve("incident", servicerequest.Id, new ColumnSet("smp_problembuilding", "smp_problemclassid", "smp_problemtypeid", "customerid"));
                        Entity proviedMatrix = this.GetProviedmatrix(originalcaserecord, service, tracingService);
                        ////CreateNewServiceRequest(Originalcaserecord, postImage, service, tracingService);
                        if (proviedMatrix != null)
                        {
                            this.AssignRecord(target, proviedMatrix.GetAttributeValue<EntityReference>("smp_fieldserviceteam").Id, service);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Entity GetProviedmatrix(Entity originalcaserecord, IOrganizationService service, ITracingService tracingService)
        {
            tracingService.Trace("Retriving the fiels service team");
            Entity proviedMatrix = null;
            QueryExpression proviedMatrixQuery = new QueryExpression("smp_providermatrix");
            proviedMatrixQuery.ColumnSet = new ColumnSet("smp_fieldserviceteam");
            proviedMatrixQuery.Criteria.AddFilter(LogicalOperator.And);
            proviedMatrixQuery.Criteria.AddCondition("smp_buildingid", ConditionOperator.Equal, originalcaserecord.GetAttributeValue<EntityReference>("smp_problembuilding").Id);
            proviedMatrixQuery.Criteria.AddCondition("smp_problemclassid", ConditionOperator.Equal, originalcaserecord.GetAttributeValue<EntityReference>("smp_problemclassid").Id);
            proviedMatrixQuery.Criteria.AddCondition("smp_problemtypeid", ConditionOperator.Equal, originalcaserecord.GetAttributeValue<EntityReference>("smp_problemtypeid").Id);
            proviedMatrixQuery.Criteria.AddCondition("smp_fieldserviceteam", ConditionOperator.NotNull);
            proviedMatrixQuery.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //// proviedMatrixQuery.Criteria.AddCondition("smp_primaryproviderid" ,ConditionOperator.Equal, originalcaserecord.GetAttributeValue<EntityReference>("customerid").Id);
            var problemtypecollection = service.RetrieveMultiple(proviedMatrixQuery);
            if (problemtypecollection.Entities != null && problemtypecollection.Entities.Count > 0)
            {
                proviedMatrix = problemtypecollection.Entities[0];
                tracingService.Trace("Provider Matrix find");
            }

            return proviedMatrix;
        }

        ////Assign a record to a team
        private void AssignRecord(Entity targetEntity, Guid owningTeamID, IOrganizationService orgService)
        {
            try
            {
                //// Create the Request Object and Set the Request Object's Properties
                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference("team", owningTeamID),
                    Target = new EntityReference(targetEntity.LogicalName, targetEntity.Id)
                };

                //// Execute the Request
                orgService.Execute(assign);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while assinging Team to a record." + ex.Message);
            }
        }
    }
}