// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateUpdateDoIntegration.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostCreateUpdateDoIntegration Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Entities;
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Cmms.ServiceManagement.Services;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class PostCreateUpdateDoIntegration : IPlugin
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="serviceProvider"></param>
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
                else if (context.MessageName == "Update" && context.Depth > 1)
                {
                    return;
                }

                Entity serviceRequest = (context.PostEntityImages != null && context.PostEntityImages.Contains("PostImage")) ? (Entity)context.PostEntityImages["PostImage"] : null;
                EntityCollection configurationCollection = ServiceRequestHelper.GetConfigurationValueCollection(service);
                Provider provider = ServiceRequestHelper.GetProviderDetails(service, ((EntityReference)serviceRequest["customerid"]).Id, configurationCollection, trace);
                int caseOrigin = ((OptionSetValue)serviceRequest["caseorigincode"]).Value;
                //// Check if provider is not updated, if not updated no action is desired.
                if (!provider.IsProviderUpdated)
                {
                    ServiceRequestHelper.SetProviderCriteriaMissingFields(service, serviceRequest);
                    return;
                }

                string currentStatusCode = ServiceRequestHelper.GetStatusCodeFromValue(service, ((OptionSetValue)((Entity)context.InputParameters["Target"])["statuscode"]).Value);
                string priorityName = serviceRequest.Attributes.Contains("smp_priorityid") ? ((EntityReference)serviceRequest["smp_priorityid"]).Name : string.Empty;
                string invalidIntegrationCodes = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("NoCmmsIntegrationActionCodes")).First()["smp_value"].ToString();
                int preStatus = context.PreEntityImages.Contains("PreImage") ? ((OptionSetValue)context.PreEntityImages["PreImage"]["statuscode"]).Value : -1;
                var configurationList = from val in configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.RoutingPriorities8By8)).First()["smp_value"].ToString().Split(',')
                             select val;
                if (caseOrigin == 3 && currentStatusCode.ToLower() == "open" && configurationList.Contains(priorityName) && preStatus != 180620012)
                {
                    trace.Trace("Aborting Integration.....");
                }
                else if (preStatus == 1 && (currentStatusCode.ToLower() == "cncl" || currentStatusCode.ToLower() == "clsd" || currentStatusCode.ToLower() == "comp"))
                {
                    trace.Trace("Aborting Integration as Pre status is :{0} and Current Status :{1}", preStatus, currentStatusCode.ToLower());
                }
                else if (!string.IsNullOrWhiteSpace(currentStatusCode) && !string.IsNullOrWhiteSpace(invalidIntegrationCodes)
                    && (!invalidIntegrationCodes.ToUpperInvariant().Contains(currentStatusCode.ToUpperInvariant())))
                {
                    //// Call integration if status code doesnot falls in invalid integration codes or 
                    //// current status is "Dispatched" and previous status is "Pending CSR Dispatched"
                    DoIntegration(service, serviceRequest, provider, trace, caseOrigin, configurationCollection);
                }
                else
                {
                    trace.Trace("The Status is Invalid");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// DoIntegration
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRequest"></param>
        /// <param name="provider"></param>
        /// <param name="trace"></param>
        /// <param name="caseOrigin"></param>
        /// <param name="collection"></param>
        private static void DoIntegration(IOrganizationService service, Entity serviceRequest, Provider provider, ITracingService trace, int caseOrigin, EntityCollection collection)
        {
            bool isEmailSent = false;
            bool isDataUpdated = false;
            bool isDebug = false;
            //// Get the configured user id for sending email.
            string fromUserDomainName = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.EmailSenderDomainName)).First()["smp_value"].ToString();
            Guid senderId = ServiceRequestHelper.GetCrmUserId(service, fromUserDomainName);
            //// get WCF timeout values
            string timeOut = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.OutBoundIntegrationtimeOutInSeconds)).First()["smp_value"].ToString();
            //// get debugging information
            string enableDebug = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.DebugOutBoundIntegration)).First()["smp_value"].ToString();
            isDebug = (string.IsNullOrEmpty(enableDebug) == false && enableDebug.Equals("1")) ? true : false;
            int fromIntegration = 0;
            ServiceRequest providerServiceRequest = ServiceRequestHelper.SetServiceRequestObject(service, serviceRequest, fromIntegration);
            //// if provider integration is enabled, send details via service else send an email
            if (provider.IsIntegrationEnabled)
            {
                //// Create a task record with XML content
                Entity task = new Entity("task");
                task["subject"] = "CMMS Integration Task " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
                task["description"] = SerializeObjectToXML(providerServiceRequest);
                task["regardingobjectid"] = new EntityReference(serviceRequest.LogicalName, serviceRequest.Id);                
                Guid newTaskId = service.Create(task);
                task["statecode"] = new OptionSetValue(1);
                task["activityid"] = newTaskId;
                service.Update(task);
                //// if provider details are not given, send an email
                if (!string.IsNullOrWhiteSpace(provider.ProviderServiceAddress))
                {
                    //// Calling integration service, trying 3 times
                    isDataUpdated = ServiceRequestHelper.CallIntegrationService(provider, providerServiceRequest, string.IsNullOrEmpty(timeOut) ? 0 : Convert.ToUInt16(timeOut), service, serviceRequest.Id, isDebug, trace);
                    //// if integration fail, log in exception log entity and send an email
                    if (!isDataUpdated && providerServiceRequest != null)
                    {
                        Logger.Write("Service Request data update failed", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, string.Empty);
                        isEmailSent = EmailHelper.SendEmail(service, providerServiceRequest, serviceRequest.Id, "systemuser", senderId, "account", provider.ProviderId, collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestIntegrationFailEmailTemplateName")).First()["smp_value"].ToString());
                    }
                }
                else
                {
                    Logger.Write("Provider details not updated, please update provider's service url, username and password.", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, string.Empty);
                    isEmailSent = EmailHelper.SendEmail(service, providerServiceRequest, serviceRequest.Id, "systemuser", senderId, "account", provider.ProviderId, collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestIntegrationFailEmailTemplateName")).First()["smp_value"].ToString());
                }
            }
            else
            {
                if (providerServiceRequest != null)
                {
                    string environment = collection.Entities.Where(x => x["smp_title"].ToString().Equals("Environment")).First()["smp_value"].ToString();
                    if (environment == "Non-Prod")
                    {
                        isEmailSent = true;
                    }
                    else
                    {
                        isEmailSent = EmailHelper.SendEmail(service, providerServiceRequest, serviceRequest.Id, "systemuser", senderId, "account", provider.ProviderId, collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestIntegrationEmailTemplateName")).First()["smp_value"].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// SerializeObjectToXML
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static string SerializeObjectToXML(object item)
        {
            try
            {
                string xmlText;
                Type objectType = item.GetType();
                XmlSerializer xmlSerializer = new XmlSerializer(objectType);
                MemoryStream memoryStream = new MemoryStream();
                using (XmlTextWriter xmlTextWriter =
                    new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    xmlSerializer.Serialize(xmlTextWriter, item);
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    xmlText = new UTF8Encoding().GetString(memoryStream.ToArray());
                    memoryStream.Dispose();
                    if (xmlText.Length > 14000)
                    {
                        xmlText = xmlText.Substring(0, 14000);
                    }

                    return xmlText;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                return null;
            }
        }
    }
}
