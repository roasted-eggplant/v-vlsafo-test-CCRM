// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreEscalationUpdateDuplicateDetection.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreEscalationUpdateDuplicateDetection Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// PreEscalationUpdateDuplicateDetection Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    public class PreEscalationUpdateDuplicateDetection : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreEscalationUpdateDuplicateDetection"/> class.
        /// </summary>
        public PreEscalationUpdateDuplicateDetection()
            : base(typeof(PreEscalationUpdateDuplicateDetection))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Update", "smp_escalation", new Action<LocalPluginContext>(this.ExecutePreEscalationUpdateDuplicateDetection)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected void ExecutePreEscalationUpdateDuplicateDetection(LocalPluginContext localContext)
        {
            ////Guid accountId = Guid.Empty;
            string escalationContactName = string.Empty;
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                var service = localContext.OrganizationService;
                if (context.Depth <= 1)
                {
                    if (entity.LogicalName == "smp_escalation")
                    {
                        try
                        {
                            if (entity.Attributes.Contains("smp_name"))
                            {
                                escalationContactName = entity.Attributes["smp_name"].ToString();
                                GetDuplicateEscalationContact(service, escalationContactName);
                            }
                        }
                        catch (CustomServiceManagementPortalException)
                        {
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the duplicate provider.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="escalationContactName">Name of the account.</param>
        /// <exception cref="Microsoft.Xrm.Sdk.InvalidPluginExecutionException">Provider with the same name already exist</exception>
        private static void GetDuplicateEscalationContact(IOrganizationService service, string escalationContactName)
        {
            try
            {
                if (!string.IsNullOrEmpty(escalationContactName))
                {
                    QueryExpression providerquery = new QueryExpression()
                    {
                        EntityName = "smp_escalation",
                        ColumnSet = new ColumnSet("smp_name"),
                        Criteria =
                        {
                            Conditions =
                                {
                                    new ConditionExpression("smp_name", ConditionOperator.Equal, escalationContactName),
                                }
                        }
                    };

                    if (service != null)
                    {
                        Entity escalationContact = service.RetrieveMultiple(providerquery).Entities.FirstOrDefault();
                        if (escalationContact != null)
                        {
                            throw new InvalidPluginExecutionException("Escalation Contact with the same name already exist.");
                        }
                    }
                }
            }
            catch (CustomServiceManagementPortalException)
            {
            }
        }
    }
}
