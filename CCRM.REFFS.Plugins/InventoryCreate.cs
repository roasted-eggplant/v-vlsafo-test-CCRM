// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryCreate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// InventoryCreate Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class InventoryCreate : Plugin
    {
        public InventoryCreate() : base(typeof(InventoryCreate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, "product", new Action<LocalPluginContext>(this.OnProductCreate)));
        }

        protected void OnProductCreate(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService service = localContext.OrganizationService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity target = context.InputParameters["Target"] as Entity;
                try
                {
                    tracingService.Trace("Invertory Create");
                    if (target.Attributes.Contains("smp_tier1provider"))
                    {
                        EntityCollection wareHouses = this.GetWareHouses(service, target.GetAttributeValue<EntityReference>("smp_tier1provider"));
                        if (wareHouses.Entities.Count > 0)
                        {
                            tracingService.Trace("Warehouse count :" + wareHouses.Entities.Count);
                            this.CreateInventories(wareHouses, target, service, tracingService);
                        }
                    }
                }
                catch (InvalidPluginExecutionException ex)
                {
                    tracingService.Trace(ex.Message);
                }
            }
        }

        private EntityCollection GetWareHouses(IOrganizationService service, EntityReference tire1Provider)
        {
            QueryExpression wareHouseQuery = new QueryExpression("msdyn_warehouse");
            wareHouseQuery.ColumnSet = new ColumnSet("msdyn_name");
            wareHouseQuery.Criteria.AddFilter(LogicalOperator.And);
            wareHouseQuery.Criteria.AddCondition("ownerid", ConditionOperator.Equal, tire1Provider.Id);
            EntityCollection whereHouseCollection = service.RetrieveMultiple(wareHouseQuery);
            return whereHouseCollection;
        }

        private void CreateInventories(EntityCollection wareHouses, Entity newProduct, IOrganizationService service, ITracingService trace)
        {
            foreach (Entity wareHouse in wareHouses.Entities)
            {
                Entity newInventory = new Entity("smp_inventory");
                newInventory.Attributes["smp_name"] = newProduct.GetAttributeValue<string>("name");
                newInventory.Attributes["smp_productid"] = newProduct.GetAttributeValue<string>("productnumber");
                newInventory.Attributes["smp_quantity"] = 0.0;
                newInventory.Attributes["smp_product"] = new EntityReference(newProduct.LogicalName, newProduct.Id);
                newInventory.Attributes["smp_warehouse"] = new EntityReference(wareHouse.LogicalName, wareHouse.Id);
                newInventory.Attributes["ownerid"] = new EntityReference(newProduct.GetAttributeValue<EntityReference>("smp_tier1provider").LogicalName, newProduct.GetAttributeValue<EntityReference>("smp_tier1provider").Id);
                service.Create(newInventory);
            }
        }
    }
}
