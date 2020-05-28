// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateorUpdateSetPriorityBasedStatus.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostCreateorUpdateSetPriorityBasedStatus Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Entities;
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Linq;

    public class PostCreateorUpdateSetPriorityBasedStatus : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            try
            {
                if ((context.InputParameters == null) || (!context.InputParameters.Contains("Target")))
                {
                    return;
                }

                Entity serviceRequest = (context.PostEntityImages != null && context.PostEntityImages.Contains("PostImage")) ? (Entity)context.PostEntityImages["PostImage"] : null;
                if (context.MessageName == "Update" && context.Depth > 1 && serviceRequest.GetAttributeValue<EntityReference>("msdyn_iotalert") == null)
                {
                    return;
                }
                else if (context.MessageName == "Update" && context.Depth > 2)
                {
                    return;
                }
                ////Updated the code to return if the SR status is Cancelled to stop Integration mails and 8x8 Mails. Related Work Items: #4850727
                else if (((OptionSetValue)((Entity)context.InputParameters["Target"])["statuscode"]).Value == Constants.SRStatus_Cancelled)
                {
                    return;
                }

                if (serviceRequest != null)
                {
                    EntityCollection configurationCollection = ServiceRequestHelper.GetConfigurationValueCollection(service);
                    Provider provider = ServiceRequestHelper.GetProviderDetails(service, ((EntityReference)serviceRequest["customerid"]).Id, configurationCollection, trace);
                    //// Check if provider is not updated,
                    ////if not updated no action is desired.
                    if (!provider.IsProviderUpdated)
                    {
                        trace.Trace("Provider Not There");
                        ServiceRequestHelper.SetProviderCriteriaMissingFields(service, serviceRequest);
                        return;
                    }

                    //// Fetching current status code 
                    string currentStatusCode = ServiceRequestHelper.GetStatusCodeFromValue(service, ((OptionSetValue)((Entity)context.InputParameters["Target"])["statuscode"]).Value);
                    trace.Trace("Current Status :" + currentStatusCode.ToString());
                    int caseOrigin = ((OptionSetValue)serviceRequest["caseorigincode"]).Value;
                    //// if priority is (P1 or P2) and integration status is false - no integration will happen
                    if (!ServiceRequestHelper.IsPrioritySetForIntegration(currentStatusCode, service, serviceRequest, trace, configurationCollection, caseOrigin, provider.ProviderTeamId))
                    {
                        trace.Trace("The Service Request is of High Priority(P1/P2/PS1/Ps2)");
                    }
                    else
                    {
                        string invalidIntegrationCodes = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("NoCmmsIntegrationActionCodes")).First()["smp_value"].ToString();
                        trace.Trace("Invalid Integration Codes :" + invalidIntegrationCodes);
                        if (!string.IsNullOrWhiteSpace(currentStatusCode) && !string.IsNullOrWhiteSpace(invalidIntegrationCodes)
                            && (!invalidIntegrationCodes.ToUpperInvariant().Contains(currentStatusCode.ToUpperInvariant())))
                        {
                            trace.Trace("The Service Request is of Low Priority or Status is DRAFT");
                            ServiceRequestHelper.SetServiceRequestStatus(service, serviceRequest, currentStatusCode, "RequestDispatchStatusCode", caseOrigin, configurationCollection, provider.ProviderTeamId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Exception from PostCreateorUpdateSetPriorityBasedStatus :" + ex.Message);
                throw new InvalidPluginExecutionException("Contact CRM Technical Administrator " + ex.Message);
            }
        }
    }
}