// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestStatusChange.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostServiceRequestStatusChange Plugin
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
    /// PostServiceRequestStatusChange Plugin.
    /// Fires when the following attributes are updated:
    /// statuscode
    /// </summary>    
    public class PostServiceRequestStatusChange : IPlugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";

        /// <summary>
        /// Sends email as per status change
        /// </summary>
        /// <param name="service">crm service</param>
        /// <param name="serviceRequest">target object</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            //// Obtain the execution context service from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //// Obtain the tracing service from the service provider.
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            //// Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            //// Use the factory to generate the Organization Service.
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            bool isProblemClassAllowForEmailNotification = false;
            bool isProblemTypeAllowForEmailNotification = false;
            bool problemBuildingAllowEmailNotification = false;
            try
            {
                if (context == null || context.Depth > 2)
                {
                    return;
                }

                if (context != null)
                {
                    trace.Trace("In Status Change");
                    if (context.InputParameters != null && context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        Entity serviceRequest = context.InputParameters["Target"] as Entity;
                        var preImageIncident = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
                        var postImageIncident = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;
                        if (serviceRequest.Attributes.Contains(CRMAttributesResource.StatusCodeAttribute))
                        {
                            Entity problemClass = postImageIncident.Attributes.Contains("smp_problemclassid") ? ServiceRequestHelper.GetProblemClassDetails(service, ((EntityReference)postImageIncident["smp_problemclassid"]).Id) : null;
                            Entity problemType = postImageIncident.Attributes.Contains("smp_problemtypeid") ? ServiceRequestHelper.GetProblemTypeDetails(service, ((EntityReference)postImageIncident["smp_problemtypeid"]).Id) : null;
                            if (((OptionSetValue)preImageIncident.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value != ((OptionSetValue)postImageIncident.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value)
                            {
                                isProblemClassAllowForEmailNotification = problemClass != null ? Convert.ToBoolean(problemClass["smp_allowemailnotification"]) : false;
                                trace.Trace("isProblemClassAllowForEmailNotification " + isProblemClassAllowForEmailNotification);
                                isProblemTypeAllowForEmailNotification = problemType != null ? Convert.ToBoolean(problemType["smp_allowemailnotification"]) : false;
                                trace.Trace("isProblemTypeAllowForEmailNotification " + isProblemTypeAllowForEmailNotification);
                                if (postImageIncident.Attributes.Contains("smp_problembuilding"))
                                {
                                    problemBuildingAllowEmailNotification = GetBuildingEmailNotificationStatus(service, ((EntityReference)postImageIncident["smp_problembuilding"]).Id);
                                }

                                trace.Trace("problemBuilding_AllowEmailNotification " + problemBuildingAllowEmailNotification);
                                if (problemBuildingAllowEmailNotification && isProblemClassAllowForEmailNotification && isProblemTypeAllowForEmailNotification)
                                {
                                    PostServiceRequestStatusChange.ProcessStatusChange(service, postImageIncident, trace, problemType, problemClass, preImageIncident);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Exception :" + ex.Message);
            }
        }

        private static void ProcessStatusChange(IOrganizationService service, Entity entity, ITracingService trace, Entity problemType, Entity problemClass, Entity preEntity)
        {
            try
            {
                EntityCollection configurationCollection = ServiceRequestHelper.GetConfigurationValueCollection(service);
                List<Entity> toEmail = new List<Entity>();
                List<Entity> ccEmail = new List<Entity>();
                string fromUserDomainName = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.EmailSenderDomainName)).First()["smp_value"].ToString();
                Guid senderId = ServiceRequestHelper.GetCrmUserId(service, fromUserDomainName);
                EntityReference contact = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestContact) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestContact] : null;
                EntityReference requester = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestRequester) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestRequester] : null;
                EntityReference cC = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestCC) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestCC] : null;
                EntityReference approver = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestApprovalManager) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestApprovalManager] : null;
                OptionSetValue currentStatus = entity.Attributes.Contains(CRMAttributesResource.StatusCodeAttribute) ? (OptionSetValue)entity[CRMAttributesResource.StatusCodeAttribute] : null;
                EntityReference provider = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestProvider) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestProvider] : null;
                Entity contactDetails = null;
                Entity requesterDetails = (requester == null) ? null : ServiceRequestHelper.GetContactDetails(service, requester.Id);
                Entity ccDetails = (cC == null) ? null : ServiceRequestHelper.GetContactDetails(service, cC.Id);
                bool allowEmailNotification = false;
                bool cancelledForReclassification = false;
                bool contact_AllowEmailNotification = false;
                string contactLanguageCode = string.Empty;
                if (contact != null)
                {
                    contactDetails = ServiceRequestHelper.GetContactDetails(service, contact.Id);
                    contact_AllowEmailNotification = contactDetails.Attributes.Contains("smp_allowemailnotification") ? (bool)contactDetails["smp_allowemailnotification"] : false;
                    contactLanguageCode = contactDetails.Attributes.Contains("smp_preferredlanguage") ? contactDetails["smp_preferredlanguage"].ToString() : "en-us";
                }

                allowEmailNotification = entity.Attributes.Contains("smp_allowemailnotification") ? (bool)entity["smp_allowemailnotification"] : false;
                cancelledForReclassification = entity.Attributes.Contains("smp_cancelledforreclassification") ? (bool)entity["smp_cancelledforreclassification"] : false;
                var fromParty = new Entity("activityparty");
                fromParty["partyid"] = new EntityReference(CRMAttributesResource.SystemUserEntity, senderId);
                if (currentStatus.Value == Convert.ToInt32(CRMAttributesResource.DeclinedStatus, CultureInfo.InvariantCulture) && allowEmailNotification == true)
                {
                    trace.Trace("In Declined Status");
                    if (requester != null)
                    {
                        var toRequestor = new Entity("activityparty");
                        toRequestor["partyid"] = new EntityReference("contact", requester.Id);
                        toEmail.Add(toRequestor);
                    }

                    if (contact != null)
                    {
                        if (requester.Id != contact.Id)
                        {
                            var ccContact = new Entity("activityparty");
                            ccContact["partyid"] = new EntityReference("contact", contact.Id);
                            toEmail.Add(ccContact);
                        }
                    }

                    if (approver != null)
                    {
                        var toApprover = new Entity("activityparty");
                        toApprover["partyid"] = new EntityReference("contact", approver.Id);
                        ccEmail.Add(toApprover);
                    }

                    EmailHelper.SendTranslatedEmail(service, entity, fromParty, toEmail, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestApprovalRejectionTemplate, configurationCollection);
                }
                else if (currentStatus.Value == Convert.ToInt32(CRMAttributesResource.CancelledStatus, CultureInfo.InvariantCulture))
                {
                    trace.Trace("In cancelled Status");
                    if (cancelledForReclassification == false)
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
                                ccEmail.Add(ccContact);
                            }
                        }

                        if (cC != null)
                        {
                            if (contact.Id != cC.Id && cC.Id != requester.Id)
                            {
                                var toCC = new Entity("activityparty");
                                toCC["partyid"] = new EntityReference("contact", cC.Id);
                                ccEmail.Add(toCC);
                            }
                        }
                        ////End of Code Changes made by Mihika G on 03-12-2017
                        ////Send email
                        EmailHelper.SendTranslatedEmail(service, entity, fromParty, toEmail, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestCancelledTemplate, configurationCollection);
                    }
                    else if (cancelledForReclassification == true)
                    {
                        ////Code Commented By Mihika G
                        if (provider != null)
                        {
                            var toProvider = new Entity("activityparty");
                            toProvider["partyid"] = new EntityReference("account", provider.Id);
                            toEmail.Add(toProvider);
                        }

                        trace.Trace("in cancelled");
                        ////End OF Code Changes
                        if (requester != null)
                        {
                            if (contact.Id != requester.Id)
                            {
                                var toRequester = new Entity("activityparty");
                                toRequester["partyid"] = new EntityReference("contact", requester.Id);
                                ccEmail.Add(toRequester);
                            }
                        }

                        if (contact != null)
                        {
                            var toContact = new Entity("activityparty");
                            toContact["partyid"] = new EntityReference("contact", contact.Id);
                            toEmail.Add(toContact);
                        }

                        //// Code Changes made by Mihika G on 03-12-2017
                        if (cC != null)
                        {
                            if (contact.Id != cC.Id && cC.Id != requester.Id)
                            {
                                var toCC = new Entity("activityparty");
                                toCC["partyid"] = new EntityReference("contact", cC.Id);
                                ccEmail.Add(toCC);
                            }
                        }

                        trace.Trace("before email");
                        trace.Trace("Template : " + CRMAttributesResource.ServiceRequestReclassifiedTemplate);
                        EmailHelper.SendTranslatedEmail(service, entity, fromParty, toEmail, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestReclassifiedTemplate, configurationCollection);

                        ////end of code change for reclassification email
                    }
                }
                else if (currentStatus.Value == Convert.ToInt32(CRMAttributesResource.CompletedStatus, CultureInfo.InvariantCulture))
                {
                    trace.Trace("In Completed Status");
                    bool noSurvey = (bool)problemClass["smp_donotallowsurvey"] || (bool)problemType["smp_donotallowsurvey"];
                    bool ccSurvey = false; //Whether the CC contact receives a survey

                    if (requester != null && canReceiveEmail(requesterDetails))
                        ((canReceiveSurvey(requesterDetails) && !noSurvey) ? toEmail : ccEmail).Add(getEmailParty(requester));

                    if (contact != null && contact.Id != requester.Id && canReceiveEmail(contactDetails))
                        ((canReceiveSurvey(contactDetails) && !noSurvey) ? toEmail : ccEmail).Add(getEmailParty(contact));

                    if ((cC != null && canReceiveEmail(ccDetails)))
                    {
                        ccSurvey = canReceiveSurvey(ccDetails) && toEmail.Count > 0; //Doesn't make sense to send it just for the CC

                        if (ccSurvey)
                            toEmail.Add(getEmailParty(cC));
                        else
                            ccEmail.Add(getEmailParty(cC));
                    }

                    //Promote the first contact to primary, since we can only have one for the survey generator
                    List<Entity> primary = new List<Entity>();
                    if (toEmail.Count != 0)
                    {
                        primary.Add(toEmail[0]);
                        toEmail.RemoveAt(0);
                        EmailHelper.SendTranslatedEmail(service, entity, fromParty, primary, toEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestCompletedTemplate, configurationCollection);
                    }
                    primary.Clear();
                    if (ccEmail.Count != 0)
                    {
                        primary.Add(ccEmail[0]);
                        ccEmail.RemoveAt(0);
                        EmailHelper.SendTranslatedEmail(service, entity, fromParty, primary, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestCompletedTemplateWithoutSurvey, configurationCollection);
                    }
                }
                else if (currentStatus.Value == Convert.ToInt32(CRMAttributesResource.WaitingForApprovalStatus, CultureInfo.InvariantCulture))
                {
                    trace.Trace("In Waiting for Approval status");
                    if (contact_AllowEmailNotification == false)
                    {
                    }
                    else
                    {
                        if (allowEmailNotification == true)
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
                                    ccEmail.Add(toRequester);
                                }
                            }

                            if (contact != null)
                            {
                                var toContact = new Entity("activityparty");
                                toContact["partyid"] = new EntityReference("contact", contact.Id);
                                ccEmail.Add(toContact);
                            }

                            ////Send email
                            EmailHelper.SendTranslatedEmail(service, entity, fromParty, toEmail, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestWaitingForApprovalTemplate, configurationCollection);
                        }
                    }
                }
                else
                {
                    trace.Trace("In Final Else ");
                    if (((OptionSetValue)preEntity["statuscode"]).Value != 1 && (currentStatus.Value == 180620017 || currentStatus.Value == 180620007 || currentStatus.Value == 180620008 || currentStatus.Value == 180620009))
                    {
                        if (((OptionSetValue)preEntity["statuscode"]).Value != currentStatus.Value)
                        {
                            if (contact_AllowEmailNotification == true)
                            {
                            }
                            else
                            {
                                if (allowEmailNotification == true)
                                {
                                    if (requester != null)
                                    {
                                        var toRequestor = new Entity("activityparty");
                                        toRequestor["partyid"] = new EntityReference("contact", requester.Id);
                                        toEmail.Add(toRequestor);
                                    }

                                    if (contact != null)
                                    {
                                        if (contact.Id != requester.Id)
                                        {
                                            var ccContact = new Entity("activityparty");
                                            ccContact["partyid"] = new EntityReference("contact", contact.Id);
                                            ccEmail.Add(ccContact);
                                        }
                                    }

                                    if (cC != null)
                                    {
                                        if (cC.Id != contact.Id && cC.Id != requester.Id)
                                        {
                                            var toCC = new Entity("activityparty");
                                            toCC["partyid"] = new EntityReference("contact", cC.Id);
                                            ccEmail.Add(toCC);
                                        }
                                    }

                                    EmailHelper.SendTranslatedEmail(service, entity, fromParty, toEmail, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestStatusChangeTemplate, configurationCollection);
                                }
                            }
                        }
                    }
                    else
                    {
                        trace.Trace("In Final Else--Else");
                        if (((OptionSetValue)preEntity["statuscode"]).Value == 1 && currentStatus.Value == 2)
                        {
                            if (contact_AllowEmailNotification)
                            {
                            }
                            else
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
                                        ccEmail.Add(ccContact);
                                    }
                                }

                                if (cC != null)
                                {
                                    if (contact.Id != cC.Id && cC.Id != requester.Id)
                                    {
                                        var toCC = new Entity("activityparty");
                                        toCC["partyid"] = new EntityReference("contact", cC.Id);
                                        ccEmail.Add(toCC);
                                    }
                                }

                                EmailHelper.SendTranslatedEmail(service, entity, fromParty, toEmail, ccEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestAcknowledgementTemplate, configurationCollection);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the ActivityParty associated with a Contact as a target for EmailHelper
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        private static Entity getEmailParty(EntityReference contact)
        {
            Entity party = new Entity("activityparty");
            party["partyid"] = new EntityReference("contact", contact.Id);
            return party;
        }

        /// <summary>
        /// Determine if we are allowed to email a Contact
        /// </summary>
        /// <param name="contactDetails"></param>
        /// <returns></returns>
        private static bool canReceiveEmail(Entity contactDetails)
        {
            return !(contactDetails?.GetAttributeValue<bool?>("smp_allowemailnotification") ?? true);
        }

        /// <summary>
        /// Determine if a Contact is allowed to recieve satisfaction surveys
        /// </summary>
        /// <param name="contactDetails"></param>
        /// <returns></returns>
        private static bool canReceiveSurvey(Entity contactDetails)
        {
            OptionSetValue staffType = (OptionSetValue)contactDetails.GetAttributeValue<OptionSetValue>("smp_staffingresourcetype");
            //Default to no survey in unset or unknown values
            if (staffType == null || !Enum.IsDefined(typeof(Constants.StaffingResourceType), staffType.Value))
                return false;
            bool isProviderUser = (bool)(contactDetails.GetAttributeValue<bool?>("smp_isprovideruser") ?? false);
            return (!isProviderUser && (Constants.StaffingResourceType)(staffType.Value) == Constants.StaffingResourceType.Regular);
        }

        private static bool GetBuildingEmailNotificationStatus(IOrganizationService service, Guid problemBuildingId)
        {
            bool allowEmailNotification = false;
            if (service != null)
            {
                Entity problemBuilding = service.Retrieve("smp_building", problemBuildingId, new ColumnSet("smp_buildinglevelemailnotification"));
                if (problemBuilding != null)
                {
                    if (problemBuilding.Contains("smp_buildinglevelemailnotification"))
                    {
                        allowEmailNotification = Convert.ToBoolean(problemBuilding.Attributes["smp_buildinglevelemailnotification"]);
                    }
                }
            }

            return allowEmailNotification;
        }
    }
}
