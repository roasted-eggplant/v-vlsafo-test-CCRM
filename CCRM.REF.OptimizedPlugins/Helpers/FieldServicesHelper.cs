// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldServicesHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  FieldServicesHelper Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public static class FieldServicesHelper
    {
        /// <summary>
        /// Checking the provider Flag  
        /// </summary>
        /// <param name="service"></param>
        /// <param name="proviederId"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static bool GetProviderDetails(IOrganizationService service, Guid proviederId, ITracingService trace)
        {
            bool isInD365 = false;
            try
            {
                if (service != null)
                {
                    Entity provieder = service.Retrieve("account", proviederId, new ColumnSet("smp_workorderwithind365"));
                    if (provieder.Attributes.Contains("smp_workorderwithind365"))
                    {
                        isInD365 = (bool)provieder.Attributes["smp_workorderwithind365"];
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Error " + ex.Message);
            }

            return isInD365;
        }

        /// <summary>
        /// Create the work order
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRequest"></param>
        /// <param name="trace"></param>
        public static void CreateWorkOrder(IOrganizationService service, Entity serviceRequest, ITracingService trace)
        {
            try
            {
                Entity priceList = GetPricelist(service, trace);
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", ((EntityReference)serviceRequest.Attributes["customerid"]).Id);
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference(serviceRequest.LogicalName, serviceRequest.Id);
                workOrder.Attributes["msdyn_pricelist"] = new EntityReference(priceList.LogicalName, priceList.Id);
                service.Create(workOrder);
            }
            catch (Exception ex)
            {
                trace.Trace("Error " + ex.Message);
            }
        }

        private static Entity GetPricelist(IOrganizationService service, ITracingService trace)
        {
            try
            {
                if (service != null)
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "pricelevel";
                    query.ColumnSet = new ColumnSet("name");

                    FilterExpression filter = new FilterExpression(LogicalOperator.And);
                    filter.AddCondition(new ConditionExpression("name", ConditionOperator.Equal, "US"));
                    query.Criteria = filter;
                    EntityCollection entityCollection = service.RetrieveMultiple(query);
                    if (entityCollection.Entities.Count == 1)
                    {
                        return entityCollection.Entities[0];
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Error in price list " + ex.Message);
            }

            return null;
        }
    }
}