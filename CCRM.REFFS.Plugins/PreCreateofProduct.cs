// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreCreateofProduct.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreCreateofProduct Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class PreCreateofProduct : Plugin
    {
        public PreCreateofProduct() : base(typeof(PreCreateofProduct))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "product", new Action<LocalPluginContext>(this.SetDefaultPricelist)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void SetDefaultPricelist(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity target = context.InputParameters["Target"] as Entity;
                    if (target != null)
                    {
                        QueryExpression exp = new QueryExpression("pricelevel");
                        exp.ColumnSet = new ColumnSet("pricelevelid");
                        exp.Criteria.AddFilter(LogicalOperator.Or);
                        string uspricelist = "US";
                        exp.Criteria.AddCondition("name", ConditionOperator.Equal, uspricelist.ToLower());
                        exp.Criteria.AddCondition("name", ConditionOperator.Equal, uspricelist.ToUpper());
                        EntityCollection pricelistcollection = service.RetrieveMultiple(exp);
                        if (pricelistcollection != null && pricelistcollection.Entities.Count > 0)
                        {
                            target.Attributes.Add("pricelevelid", new EntityReference(pricelistcollection.Entities[0].LogicalName, pricelistcollection.Entities[0].Id));
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
