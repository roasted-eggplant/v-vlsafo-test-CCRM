// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurchaseRequisitionCreateandUpdate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PurchaseRequisitionCreateandUpdate
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class PurchaseRequisitionCreateandUpdate : Plugin
    {
        public PurchaseRequisitionCreateandUpdate() : base(typeof(PurchaseRequisitionCreateandUpdate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "msdyn_purchaseorder", new Action<LocalPluginContext>(this.SetProviderData)));
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "msdyn_purchaseorder", new Action<LocalPluginContext>(this.SetProviderData)));
        }

        ////Thid Plugin will fire on Work order post Create to create Inspection records.
        public void SetProviderData(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                tracingService.Trace("Purchase Order Create and Update");
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity purchaseOrder = context.InputParameters["Target"] as Entity;
                    if (purchaseOrder.Attributes.Contains("smp_building"))
                    {
                        EntityReference building = purchaseOrder.GetAttributeValue<EntityReference>("smp_building");
                        if (building != null)
                        {
                            Entity buildingrecord = service.Retrieve(building.LogicalName, building.Id, new ColumnSet("smp_tieroneprovider"));
                            if (context.MessageName == "Create")
                            {
                                purchaseOrder.Attributes.Add("smp_tier1provider", buildingrecord.GetAttributeValue<string>("smp_tieroneprovider"));
                            }
                            else
                            {
                                Entity newpurchaseorder = new Entity(purchaseOrder.LogicalName, purchaseOrder.Id);
                                newpurchaseorder.Attributes.Add("smp_tier1provider", buildingrecord.GetAttributeValue<string>("smp_tieroneprovider"));
                                service.Update(newpurchaseorder);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}