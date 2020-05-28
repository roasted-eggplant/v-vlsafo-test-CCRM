// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateInspections.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  CreateInspections
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class CreateInspections
    {
        public void CreateInspectionsforWorkOrders(IOrganizationService service, Entity workOrder, ITracingService tracingService)
        {
            tracingService.Trace("CreateInspectionsforWorkOrders");

            tracingService.Trace("Contains msdyn_primaryincidenttype");
            Entity incidentType = service.Retrieve("msdyn_incidenttype", workOrder.GetAttributeValue<EntityReference>("msdyn_primaryincidenttype").Id, new ColumnSet("smp_inspectiontypeid"));

            if (incidentType.GetAttributeValue<EntityReference>("smp_inspectiontypeid") != null)
            {
                EntityReference inspectionType = incidentType.GetAttributeValue<EntityReference>("smp_inspectiontypeid");
                EntityReference building = workOrder.GetAttributeValue<EntityReference>("msdyn_serviceaccount");
                tracingService.Trace("Building" + building);
                if (workOrder.Attributes.Contains("msdyn_customerasset") && workOrder.GetAttributeValue<EntityReference>("msdyn_customerasset") != null)
                {
                    EntityCollection childAssets = this.GetChildAssets(service, workOrder.GetAttributeValue<EntityReference>("msdyn_customerasset"), tracingService);
                    if (childAssets.Entities.Count > 0)
                    {
                        foreach (var childasset in childAssets.Entities)
                        {
                            this.CreateInspection(service, tracingService, inspectionType, building, workOrder, new EntityReference(childasset.LogicalName, childasset.Id));
                        }

                        tracingService.Trace("Inspections Created for each Child Asset");
                    }
                    else
                    {
                        this.CreateInspection(service, tracingService, inspectionType, building, workOrder, workOrder.GetAttributeValue<EntityReference>("msdyn_customerasset"));
                        tracingService.Trace("Inspections Created for only Asset");
                    }
                }
                else
                {
                    this.CreateInspection(service, tracingService, inspectionType, building, workOrder, null);
                    tracingService.Trace("Inspections Created for only Work Order");
                }
            }
        }

        private void CreateInspection(IOrganizationService service, ITracingService tracing, EntityReference inspectionType, EntityReference building, Entity workOrder, EntityReference asset)
        {
            Entity inspection = new Entity("smp_inspection");
            inspection.Attributes.Add("smp_inspectiontype", inspectionType);
            inspection.Attributes.Add("smp_building", building);
            inspection.Attributes.Add("smp_workorder", new EntityReference(workOrder.LogicalName, workOrder.Id));
            inspection.Attributes.Add("ownerid", new EntityReference(workOrder.GetAttributeValue<EntityReference>("ownerid").LogicalName, workOrder.GetAttributeValue<EntityReference>("ownerid").Id));
            if (asset != null)
            {
                inspection.Attributes.Add("smp_customerassetid", new EntityReference(asset.LogicalName, asset.Id));
            }

            Guid inspectionid = service.Create(inspection);
        }

        private EntityCollection GetChildAssets(IOrganizationService service, EntityReference parentAsset, ITracingService tracing)
        {
            EntityCollection childAssets = new EntityCollection();
            QueryExpression query = new QueryExpression("msdyn_customerasset");
            query.ColumnSet = new ColumnSet("msdyn_customerassetid");
            query.Criteria.AddFilter(LogicalOperator.Or);
            query.Criteria.AddCondition("msdyn_customerasset", "msdyn_parentasset", ConditionOperator.Equal, parentAsset.Id);
            childAssets = service.RetrieveMultiple(query);
            return childAssets;
        }
    }    
}