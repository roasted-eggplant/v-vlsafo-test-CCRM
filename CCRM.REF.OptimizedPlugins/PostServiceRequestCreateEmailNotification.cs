// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestCreateEmailNotification.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostServiceRequestCreateEmailNotification Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// PostServiceRequestCreateEmailNotification Plugin.
    /// </summary>    
    public class PostServiceRequestCreateEmailNotification : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostServiceRequestCreateEmailNotification"/> class.
        /// </summary>
        public PostServiceRequestCreateEmailNotification()
            : base(typeof(PostServiceRequestCreateEmailNotification))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "incident", new Action<LocalPluginContext>(this.ExecutePostServiceRequestCreateEmailNotification)));

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
        protected void ExecutePostServiceRequestCreateEmailNotification(LocalPluginContext localContext)
        {
            Guid incidentId = Guid.Empty;
            bool isProblemClassAllowForEmailNotification = false;
            bool isProblemTypeAllowForEmailNotification = false;
            if (localContext == null)
            {
                return;
            }

            // TODO: Implement your custom Plug-in business logic.
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService trace = localContext.TracingService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                bool allowEmailNotification = false;
                bool contact_AllowEmailNotification = false;
                bool problemBuilding_AllowEmailNotification = false;
                int serviceRequestOriginValue = 0;
                var service = localContext.OrganizationService;
                trace.Trace("Depth :" + context.Depth.ToString());
                if (entity.LogicalName == "incident")
                {
                    incidentId = entity.Id;
                    Entity serviceRequest = context.PostEntityImages.Contains("PostImage") ? (Entity)context.PostEntityImages["PostImage"] : null;
                    Entity contactInfo = ServiceRequestHelper.GetContactDetails(service, ((EntityReference)serviceRequest.Attributes["smp_contact"]).Id);
                    contact_AllowEmailNotification = (contactInfo != null && contactInfo.Attributes.Contains("smp_allowemailnotification")) ? (bool)contactInfo["smp_allowemailnotification"] : false;
                    trace.Trace("contact_AllowEmailNotification :" + contact_AllowEmailNotification.ToString());
                    if (serviceRequest.Attributes.Contains("smp_problembuilding"))
                    {
                        problemBuilding_AllowEmailNotification = ServiceRequestHelper.GetBuildingEmailNotificationStatus(service, ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_problembuilding"]).Id);
                    }

                    trace.Trace("problemBuilding_AllowEmailNotification :" + problemBuilding_AllowEmailNotification.ToString());
                    allowEmailNotification = serviceRequest.Attributes.Contains("smp_allowemailnotification") ? (bool)serviceRequest["smp_allowemailnotification"] : false;
                    if (serviceRequest.Attributes.Contains("caseorigincode"))
                    {
                        serviceRequestOriginValue = ((Microsoft.Xrm.Sdk.OptionSetValue)serviceRequest["caseorigincode"]).Value;
                    }

                    trace.Trace("serviceRequestOriginValue :" + serviceRequestOriginValue.ToString());
                    Entity problemClass = serviceRequest.Attributes.Contains("smp_problemclassid") ? ServiceRequestHelper.GetProblemClassDetails(service, ((EntityReference)serviceRequest.Attributes["smp_problemclassid"]).Id) : null;
                    isProblemClassAllowForEmailNotification = problemClass != null ? (bool)problemClass["smp_allowemailnotification"] : false;
                    trace.Trace("isProblemClassAllowForEmailNotification :" + isProblemClassAllowForEmailNotification.ToString());
                    Entity problemType = serviceRequest.Attributes.Contains("smp_problemtypeid") ? ServiceRequestHelper.GetProblemTypeDetails(service, ((EntityReference)serviceRequest["smp_problemtypeid"]).Id) : null;
                    isProblemTypeAllowForEmailNotification = problemType != null ? (bool)problemType["smp_allowemailnotification"] : false;
                    trace.Trace("isProblemTypeAllowForEmailNotification :" + isProblemTypeAllowForEmailNotification.ToString());
                    try
                    {
                        OptionSetValue entityStatusCode = (OptionSetValue)entity.Attributes["statuscode"];
                        trace.Trace("entityStatusCode" + entityStatusCode.Value.ToString());
                        if (entityStatusCode.Value == 2 || entityStatusCode.Value == 3)
                        {
                            if (contact_AllowEmailNotification == true)
                            {
                            }
                            else
                            {
                                if (allowEmailNotification == true && problemBuilding_AllowEmailNotification == true && isProblemTypeAllowForEmailNotification && isProblemClassAllowForEmailNotification)
                                {
                                    EntityCollection configurationCollection = ServiceRequestHelper.GetConfigurationValueCollection(service);
                                    if (serviceRequestOriginValue != 180620003)
                                    {
                                        SendStatusChangeEmail(service, serviceRequest, entityStatusCode.Value, configurationCollection, contactInfo);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Sends status change email
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="objectIncident">The object incident.</param>
        /// <param name="emailTemplate">Email template name</param>
        private static void SendStatusChangeEmail(IOrganizationService service, Entity objectIncident, int statusCode, EntityCollection configurationCollection, Entity contactInfo)
        {
            try
            {
                List<Entity> toEmail = new List<Entity>();
                List<Entity> carbonCopyEmail = new List<Entity>();
                string emailTemplate = string.Empty;
                if (statusCode != 3)
                {
                    emailTemplate = CRMAttributesResource.ServiceRequestAcknowledgementTemplate;
                }
                else
                {
                    emailTemplate = CRMAttributesResource.ServiceRequestApprovalRequiredTemplate;
                }
                //// Get the configured user id for sending email.
                string fromUserDomainName = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.EmailSenderDomainName)).First()["smp_value"].ToString();
                Guid senderId = ServiceRequestHelper.GetCrmUserId(service, fromUserDomainName);
                EntityReference contact = objectIncident.Attributes.Contains(CRMAttributesResource.ServiceRequestContact) ? (EntityReference)objectIncident[CRMAttributesResource.ServiceRequestContact] : null;
                EntityReference requester = objectIncident.Attributes.Contains(CRMAttributesResource.ServiceRequestRequester) ? (EntityReference)objectIncident[CRMAttributesResource.ServiceRequestRequester] : null;
                EntityReference cC = objectIncident.Attributes.Contains(CRMAttributesResource.ServiceRequestCC) ? (EntityReference)objectIncident[CRMAttributesResource.ServiceRequestCC] : null;
                EntityReference approver = objectIncident.Attributes.Contains(CRMAttributesResource.ServiceRequestApprovalManager) ? (EntityReference)objectIncident[CRMAttributesResource.ServiceRequestApprovalManager] : null;
                if (objectIncident.Attributes.Contains(CRMAttributesResource.ServiceRequestIsApprovalRequired) && (bool)objectIncident[CRMAttributesResource.ServiceRequestIsApprovalRequired] == true && statusCode == 3)
                {
                    if (approver != null)
                    {
                        var toApprover = new Entity("activityparty");
                        toApprover["partyid"] = new EntityReference("contact", approver.Id);
                        toEmail.Add(toApprover);
                    }

                    if (requester != null)
                    {
                        if (requester.Id != contact.Id)
                        {
                            var toRequester = new Entity("activityparty");
                            toRequester["partyid"] = new EntityReference("contact", requester.Id);
                            carbonCopyEmail.Add(toRequester);
                        }
                    }

                    if (contact != null)
                    {
                        var toContact = new Entity("activityparty");
                        toContact["partyid"] = new EntityReference("contact", contact.Id);
                        carbonCopyEmail.Add(toContact);
                    }
                }

                //// Get language code for contact
                string contactLanguageCode = contactInfo.Attributes.Contains("smp_preferredlanguage") ? contactInfo["smp_preferredlanguage"].ToString() : "en-us";
                var fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference(CRMAttributesResource.SystemUserEntity, senderId);
                if (statusCode != 3)
                {
                    if (requester != null)
                    {
                        var toRequester = new Entity("activityparty");
                        toRequester["partyid"] = new EntityReference("contact", requester.Id);
                        toEmail.Add(toRequester);
                    }

                    if (contact != null)
                    {
                        if (contact.Id != requester.Id)
                        {
                            var ccContact = new Entity("activityparty");
                            ccContact["partyid"] = new EntityReference("contact", contact.Id);
                            carbonCopyEmail.Add(ccContact);
                        }
                    }

                    if (cC != null)
                    {
                        if (cC.Id != contact.Id && cC.Id != requester.Id)
                        {
                            var toCC = new Entity("activityparty");
                            toCC["partyid"] = new EntityReference("contact", cC.Id);
                            carbonCopyEmail.Add(toCC);
                        }
                    }
                }

                //// Send email
                EmailHelper.SendTranslatedEmail(service, objectIncident, fromParty, toEmail, carbonCopyEmail, contactLanguageCode, emailTemplate, configurationCollection);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
