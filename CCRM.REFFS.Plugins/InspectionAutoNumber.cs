// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InspectionAutoNumber.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  InspectionAutoNumber Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Linq;
    using System.Text;

    public class InspectionAutoNumber : Plugin
    {
        private static object locker = new object();

        public InspectionAutoNumber() : base(typeof(InspectionAutoNumber))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, Constants.Create, "smp_inspection", new Action<LocalPluginContext>(this.AutoNumber)));
        }

        public void AutoNumber(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService service = localContext.OrganizationService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                try
                {
                    string previousNumber = string.Empty;
                    int newNumber = 0;
                    tracingService.Trace("Auto Numbering");
                    Entity inspectionRecord = context.InputParameters["Target"] as Entity;

                    lock (locker)
                    {
                        tracingService.Trace("Locker on Update");
                        Entity previousRecord = this.GetlatestInspectionReocrd(service, tracingService);
                        tracingService.Trace("latest record" + previousRecord);
                        ////  tracingService.Trace(previousRecord.Attributes.Contains("smp_inspectionnumber").ToString());

                        if (previousRecord != null && previousRecord.Attributes.Contains("smp_inspectionnumber"))
                        {
                            previousNumber = previousRecord.Attributes["smp_inspectionnumber"].ToString();
                            tracingService.Trace("previous on Update" + newNumber);
                        }

                        if (previousNumber != string.Empty)
                        {
                            newNumber = Convert.ToInt32(previousNumber.Substring(4, 5)) + 1;
                            tracingService.Trace("previous on Update" + newNumber);
                        }

                        string newnumberstring = "INS-" + newNumber.ToString("00000") + "-" + this.RandomString(6);
                        //// Adding the inspection number to target
                        if (!inspectionRecord.Attributes.Contains("smp_name"))
                        {
                            inspectionRecord.Attributes.Add("smp_name", newnumberstring);
                        }

                        inspectionRecord.Attributes.Add("smp_inspectionnumber", newnumberstring);
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace(ex.Message);
                }
            }
        }

        private Entity GetlatestInspectionReocrd(IOrganizationService service, ITracingService tracingService)
        {
            QueryExpression query = new QueryExpression("smp_inspection");
            query.ColumnSet = new ColumnSet(true);
            query.AddOrder("createdon", OrderType.Descending);
            query.TopCount = 1;
            Entity entity = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            return entity;
        }

        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble()) + 65));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}