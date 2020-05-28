// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestStatusChanged.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ServiceRequestStatusChanged Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Linq;

    public class ServiceRequestStatusChanged : Plugin
    {
        public ServiceRequestStatusChanged() : base(typeof(ServiceRequestStatusChanged))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "incident", new Action<LocalPluginContext>(this.SRStatusCancelled)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void SRStatusCancelled(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;

                tracingService.Trace("Depth:" + context.Depth);
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity target = context.InputParameters["Target"] as Entity;

                    tracingService.Trace("Status:" + target.GetAttributeValue<OptionSetValue>("statuscode").Value);
                    Entity servicerequest = service.Retrieve(target.LogicalName, target.Id, new ColumnSet("smp_cancellingfromwo"));
                    bool calcelledfromWO = servicerequest.GetAttributeValue<bool>("smp_cancellingfromwo");
                    if (target.GetAttributeValue<OptionSetValue>("statuscode").Value == 6 && (context.Depth == 1 || context.Depth == 2) && calcelledfromWO == false)
                    {
                        QueryExpression exp = new QueryExpression("msdyn_workorder");
                        exp.ColumnSet = new ColumnSet("msdyn_systemstatus");
                        exp.Criteria.AddFilter(LogicalOperator.And);
                        exp.Criteria.AddCondition("msdyn_servicerequest", ConditionOperator.Equal, target.Id);
                        exp.Criteria.AddCondition("msdyn_systemstatus", ConditionOperator.NotEqual, 690970004);
                        EntityCollection existedWorkorders = service.RetrieveMultiple(exp);
                        tracingService.Trace("existedWorkorders Count " + existedWorkorders.Entities.Count);
                        if (existedWorkorders.Entities != null && existedWorkorders.Entities.Count > 0)
                        {
                            foreach (Entity workorder in existedWorkorders.Entities)
                            {
                                tracingService.Trace("Work Order Status:" + workorder.GetAttributeValue<OptionSetValue>("msdyn_systemstatus").Value);

                                ////Cancell bookings
                                this.SetBookingStatusToCancel(workorder.Id, service, tracingService);

                                ////Setting System Status to Cancelled.
                                workorder.Attributes["msdyn_systemstatus"] = new OptionSetValue(690970005);
                                ////Setting ServiceRequest Cancelled as Reason
                                workorder.Attributes["smp_cancellationreason"] = new OptionSetValue(180620003);
                                service.Update(workorder);
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

        private void SetBookingStatusToCancel(Guid workOrderID, IOrganizationService service, ITracingService tracingService)
        {
            EntityCollection bookableResources = this.GetBookings(service, workOrderID);
            if (bookableResources != null)
            {
                tracingService.Trace("Work Order : " + workOrderID);
                tracingService.Trace("Bookings Count : " + bookableResources.Entities.Count);
                foreach (Entity bookableResource in bookableResources.Entities)
                {
                    tracingService.Trace("Booking Status :" + bookableResource.GetAttributeValue<OptionSetValue>("statecode").Value);
                    Entity bookingstatus = this.GetCancelledStatus(service);
                    bookableResource.Attributes["bookingstatus"] = new EntityReference(bookingstatus.LogicalName, bookingstatus.Id);
                    bookableResource.Attributes["statecode"] = new OptionSetValue(1);
                    bookableResource.Attributes["statuscode"] = new OptionSetValue(2);
                    service.Update(bookableResource);
                }
            }
        }

        private EntityCollection GetBookings(IOrganizationService organizationService, Guid workorderid)
        {
            EntityCollection result = null;
            QueryExpression query = new QueryExpression("bookableresourcebooking");
            query.ColumnSet = new ColumnSet("statecode", "bookableresourcebookingid");
            query.Criteria.AddCondition("msdyn_workorder", ConditionOperator.Equal, workorderid);
            result = organizationService.RetrieveMultiple(query);
            if (result != null)
            {
                if (result.Entities.Count > 0)
                {
                    return result;
                }
            }

            return null;
        }

        private Entity GetCancelledStatus(IOrganizationService organizationService)
        {
            QueryExpression query = new QueryExpression("bookingstatus");
            query.ColumnSet = new ColumnSet("bookingstatusid");
            query.Criteria.AddCondition("status", ConditionOperator.Equal, 3); ////Cancelled
            EntityCollection response = organizationService.RetrieveMultiple(query);
            if (response != null)
            {
                if (response.Entities.Count > 0)
                {
                    return response.Entities[0];
                }
            }

            return null;
        }
    }
}