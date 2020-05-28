// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestUpdateSetIntegrationStatus.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreServiceRequestUpdateSetIntegrationStatus Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// PreServiceRequestUpdateSetIntegrationStatus Plugin.
    /// Fires when the following attributes are updated:
    /// Triggered on Status Code and Priority
    /// </summary>    
    public class PreServiceRequestUpdateSetIntegrationStatus : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreServiceRequestUpdateSetIntegrationStatus"/> class.
        /// </summary>
        public PreServiceRequestUpdateSetIntegrationStatus()
            : base(typeof(PreServiceRequestUpdateSetIntegrationStatus))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Update", "incident", new Action<LocalPluginContext>(this.ExecutePreServiceRequestUpdateSetIntegrationStatus)));

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
        protected void ExecutePreServiceRequestUpdateSetIntegrationStatus(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService trace = localContext.TracingService;
            if (context == null)
            {
                return;
            }

            Entity serviceRequest = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] as Entity : null;
            Entity preImage = (context.PreEntityImages != null && context.PreEntityImages.Contains("PreImage")) ? (Entity)context.PreEntityImages["PreImage"] : null;
            var service = localContext.OrganizationService;
            if (service != null && serviceRequest != null && preImage != null)
            {
                try
                {
                    int statusCode = -999;
                    statusCode = serviceRequest.Attributes.Contains("statuscode") ? ((OptionSetValue)serviceRequest["statuscode"]).Value : ((OptionSetValue)preImage["statuscode"]).Value;
                    if (statusCode != -999 && (statusCode == 5 || statusCode == 6))
                    {
                        return;
                    }

                    EntityCollection configurationCollection = ServiceRequestHelper.GetConfigurationValueCollection(service);
                    string currentStatusCode = ServiceRequestHelper.GetStatusCodeFromValue(service, statusCode);
                    string draftStatusCode = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("DraftStatusCode")).First()["smp_value"].ToString();
                    SetIntegrationStatus(serviceRequest, preImage, service, currentStatusCode, draftStatusCode, trace, configurationCollection);
                }
                catch (CustomServiceManagementPortalException)
                {
                    Logger.Write("Updating integration status field failed", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, string.Empty);
                }
            }
        }

        /// <summary>
        /// Sets the integration status.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serviceRequest">The service request.</param>
        /// <param name="service">The service.</param>
        /// <param name="currentStatusCode">The current status code.</param>
        /// <param name="draftStatusCode">The draft status code.</param>
        private static void SetIntegrationStatus(Entity serviceRequest, Entity preImage, IOrganizationService service, string currentStatusCode, string draftStatusCode, ITracingService trace, EntityCollection collection)
        {
            int statusCode = serviceRequest.Attributes.Contains("statuscode") ? ((OptionSetValue)serviceRequest["statuscode"]).Value : ((OptionSetValue)preImage["statuscode"]).Value;
            if (!string.IsNullOrWhiteSpace(currentStatusCode) && !string.IsNullOrWhiteSpace(draftStatusCode)
                 && (currentStatusCode.ToUpperInvariant() != draftStatusCode.ToUpperInvariant()) && statusCode != 3 && statusCode != 180620013)
            {
                if (serviceRequest.Attributes.Contains("smp_priorityid"))
                {
                    ServiceRequestHelper.SetIntegrationStatus(serviceRequest, true);
                }
                else
                {
                    SetIntegrationStatusWhenPriorityNotChanged(collection, serviceRequest, service, trace, preImage);
                }
            }
            else
            {
                ServiceRequestHelper.SetIntegrationStatus(serviceRequest, false);
            }
        }

        /// <summary>
        /// Sets the integration status when priority not changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serviceRequest">The service request.</param>
        /// <param name="service">The service.</param>
        private static void SetIntegrationStatusWhenPriorityNotChanged(EntityCollection collection, Entity serviceRequest, IOrganizationService service, ITracingService trace, Entity preImage)
        {
            // Get all priorities for 8/8 Mailbox
            string priorities = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.RoutingPriorities8By8)).First()["smp_value"].ToString();
            string preImagePriority = preImage.Attributes.Contains("smp_priorityid") ? ServiceRequestHelper.GetLookupName(service, ((EntityReference)preImage["smp_priorityid"]).Id, "smp_name", "smp_priority") : string.Empty;
            if (string.IsNullOrWhiteSpace(preImagePriority))
            {
                ServiceRequestHelper.SetIntegrationStatus(serviceRequest, false);
            }
            else if (!priorities.Contains(preImagePriority.Trim().ToUpperInvariant()))
            {
                ServiceRequestHelper.SetIntegrationStatus(serviceRequest, true);
            }
            else if (priorities.Contains(preImagePriority.Trim().ToUpperInvariant()) && ((OptionSetValue)preImage["statuscode"]).Value != 1)
            {
                ServiceRequestHelper.SetIntegrationStatus(serviceRequest, true);
            }
            else
            {
                ServiceRequestHelper.SetIntegrationStatus(serviceRequest, false);
            }
        }
    }
}
