// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetDueDateonWO.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  SetDueDateonWO Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class SetDueDateonWO : Plugin
    {
        public SetDueDateonWO() : base(typeof(SetDueDateonWO))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "msdyn_agreementbookingdate", new Action<LocalPluginContext>(this.SetDueDate)));
        }

        protected void SetDueDate(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;

                tracingService.Trace("Work Order Mapped in Agreement Booking Date");
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity agreementBooking = context.InputParameters["Target"] as Entity;
                    if (agreementBooking.GetAttributeValue<EntityReference>("msdyn_workorder") != null)
                    {
                        Entity agreementbookingrecord = service.Retrieve(agreementBooking.LogicalName, agreementBooking.Id, new ColumnSet("msdyn_bookingdate"));
                        Entity workorder = service.Retrieve(agreementBooking.GetAttributeValue<EntityReference>("msdyn_workorder").LogicalName, agreementBooking.GetAttributeValue<EntityReference>("msdyn_workorder").Id, new ColumnSet(true));

                        DateTime bookingDate = agreementbookingrecord.GetAttributeValue<DateTime>("msdyn_bookingdate");
                        //// var lastDayOfMonth = DateTime.DaysInMonth(bookingDate.Year, bookingDate.Month);
                        var month = bookingDate.Month;
                        var year = bookingDate.Year;
                        DateTime firstDayOfMonth = new DateTime(year, month, 1);
                        DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);
                        //// DateTime duedate = new DateTime(year, month, lastDayOfMonth);
                        //// workorder.Attributes.Add("smp_duedate", duedate);
                        workorder.Attributes.Add("smp_duedate", lastDayOfMonth);
                        workorder.Attributes.Add("smp_duedatebybuildingtimezone", lastDayOfMonth.ToString("MM/dd/yyyy h:mm tt"));
                        service.Update(workorder);
                        tracingService.Trace("WO Updated");
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