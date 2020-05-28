// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  EmailHelper Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using Cmms.ServiceManagement.Services;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Class EmailHelper
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Creates a new Email from the Email Template with all the data
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The service request.</param>
        /// <param name="serviceRequestId">The service request Id.</param>
        /// <param name="fromActivityPartyLogicalName">Name of from activity party logical.</param>
        /// <param name="fromActivityPartyId">From activity party id.</param>
        /// <param name="toActivityPartyLogicalName">Name of to activity party logical.</param>
        /// <param name="toActivityPartyId">To activity party id.</param>
        /// <param name="templateName">Name of the template.</param>
        /// <returns><c>true</c> if email is sent, <c>false</c> otherwise</returns>
        public static bool SendEmail(IOrganizationService service, ServiceRequest serviceRequest, Guid serviceRequestId, string fromActivityPartyLogicalName, Guid fromActivityPartyId, string toActivityPartyLogicalName, Guid toActivityPartyId, string templateName)
        {
            bool isEmailSent = false;
            try
            {
                if (service != null && serviceRequest != null)
                {
                    //// get the email template from the orion Email
                    Entity emailTemplate = GetTemplateByName(service, templateName);

                    Entity email = new Entity();

                    //// set the properties of the Email from the email template
                    StringBuilder subject =
                        new StringBuilder(GetDataFromXml(emailTemplate.Attributes["subject"].ToString(), "match"));
                    StringBuilder description =
                        new StringBuilder(GetDataFromXml(emailTemplate.Attributes["body"].ToString(), "match"));

                    //// replace the subject placeholder(s)
                    subject = subject.Replace("[TicketNumber]", serviceRequest.ServiceRequestNumber == null ? string.Empty : serviceRequest.ServiceRequestNumber);

                    //// check to see if the subject of the email is more than maximum allowed length of the subject
                    if (subject.Length > 195)
                    {
                        email["subject"] = subject.ToString().Substring(0, 195) + "....";
                    }
                    else
                    {
                        email["subject"] = subject.ToString();
                    }

                    description.Replace("[ProviderName]", serviceRequest.ProviderName);
                    description.Replace("[PriorityName]", serviceRequest.Priority);
                    description.Replace("[TicketNumber]", serviceRequest.ServiceRequestNumber);
                    description.Replace("[ProblemClass]", serviceRequest.ProblemClassName);
                    description.Replace("[ProblemType]", serviceRequest.ProblemTypeName);
                    description.Replace("[ServiceRequestStatus]", serviceRequest.StatusCode);
                    description.Replace("[BuildingName]", serviceRequest.ProblemBuildingName);
                    description.Replace("[RoomNo]", serviceRequest.ProblemRoomNumber);
                    description.Replace("[ProblemDescription]", serviceRequest.ProblemTypeDescription);
                    description.Replace("[Sender]", "(GFSC Portal Address)");

                    description.Replace("[RoomType]", serviceRequest.ProblemRoomTypeName);
                    description.Replace("[RequestorName]", serviceRequest.RequestorFirstName + " " + serviceRequest.RequestorLastName);
                    description.Replace("[RequestorAlias]", serviceRequest.RequestorAlias);
                    description.Replace("[RequestorPhoneNo]", serviceRequest.RequestorPhoneNumber);

                    description.Replace("[ContactName]", serviceRequest.ContactFirstName + " " + serviceRequest.ContactLastName);
                    description.Replace("[ContactAlias]", serviceRequest.ContactAlias);
                    description.Replace("[ContactPhoneNo]", serviceRequest.ContactPhoneNumber);

                    StringBuilder html = new StringBuilder();
                    if (description.ToString().ToLowerInvariant().Contains("[dynamicproblemtypenotes]"))
                    {
                        if (serviceRequest.DynamicProblemTypeNotesCollection.Count() > 0)
                        {
                            html.Append("<table style=\"margin-left:35px\">");
                            html.Append("<tr><td style=\"font-family:Calibri;font-size:18px;\" colspan='2'><b><u>Dynamic Problem Type Notes</u></b></td></tr>");

                            foreach (DynamicProblemTypeNotes dptNotes in serviceRequest.DynamicProblemTypeNotesCollection)
                            {
                                html.Append("<table style=\"margin-left:35px\">");
                                html.Append("<tr><td style=\"font-family:Calibri;font-size:16px;\"><b>");
                                html.Append(dptNotes.DynamicsProblemTypeName);
                                html.Append(" : </b></tr></td>");
                                html.Append("<tr><td  style=\"font-family:Calibri;font-size:16px;\">");
                                html.Append(dptNotes.Answer);
                                html.Append("</tr></td>");
                                html.Append("</table>");
                            }

                            html.Append("</table>");
                        }
                    }
                    
                    description.Replace("[DueDate]", serviceRequest.DueDateByBuildingTimeZone);
                    description.Replace("[DynamicProblemTypeNotes]", html.ToString());
                    email["description"] = string.Format(CultureInfo.InstalledUICulture, "<div style=\"font-family:Calibri;font-size:16px;\"> {0} </div>", description.ToString());
                    var toParty = new Entity("activityparty");
                    toParty["partyid"] = new EntityReference(toActivityPartyLogicalName, toActivityPartyId);

                    var fromParty = new Entity("activityparty");
                    fromParty["partyid"] = new EntityReference(fromActivityPartyLogicalName, fromActivityPartyId);

                    email.LogicalName = "email";
                    email["to"] = new[] { toParty };
                    email["from"] = new[] { fromParty };
                    email["regardingobjectid"] = new EntityReference("incident", serviceRequestId);

                    Guid emailId = service.Create(email);

                    //// Use the SendEmail message to send an e-mail message.
                    var sendEmailreq = new SendEmailRequest
                    {
                        EmailId = emailId,
                        TrackingToken = string.Empty,
                        IssueSend = true
                    };
                    service.Execute(sendEmailreq);
                    isEmailSent = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isEmailSent;
        }

        /// <summary>
        /// Creates a new Email from the Email Template and translates dynamic data as per given language
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request Id.</param>
        /// <param name="fromParty">From email party</param>
        /// <param name="toParty">List of recipients</param>
        /// <param name="carbonCopyParty">List of recipients who will receive carbon copy of email</param>
        /// <param name="languageCode">language for translation</param>
        /// <param name="templateName">Name of the template.</param>
        /// <returns><c>true</c> if email is sent, <c>false</c> otherwise</returns>
        public static bool SendTranslatedEmail(IOrganizationService service, Entity entity, Entity fromParty, List<Entity> toParty, List<Entity> carbonCopyParty, string languageCode, string templateName, EntityCollection collection)
        {
            bool isEmailSent = false;
            string surveyLink = string.Empty;
            try
            {
                if (service != null)
                {
                    int fromEmail = 1;
                    ServiceRequest serviceRequest = ServiceRequestHelper.SetServiceRequestObject(service, entity, fromEmail);

                    //// get the email template from the orion Email
                    Entity emailTemplate = GetTemplateByName(service, languageCode + "|" + templateName);
                    Entity email = new Entity();
                    //// set the properties of the Email from the email template
                    StringBuilder subject =
                        new StringBuilder(GetDataFromXml(emailTemplate.Attributes["subject"].ToString(), "match"));
                    StringBuilder description =
                        new StringBuilder(GetDataFromXml(emailTemplate.Attributes["body"].ToString(), "match"));
                    //// replace the subject placeholder(s)
                    subject = subject.Replace("[TicketNumber]", serviceRequest.ServiceRequestNumber == null ? string.Empty : serviceRequest.ServiceRequestNumber);
                    //// check to see if the subject of the email is more than maximum allowed length of the subject
                    if (subject.Length > 195)
                    {
                        email["subject"] = subject.ToString().Substring(0, 195) + "....";
                    }
                    else
                    {
                        email["subject"] = subject.ToString();
                    }

                    description.Replace("[ProviderName]", serviceRequest.ProviderName);
                    description.Replace("[PriorityName]", serviceRequest.Priority);
                    description.Replace("[TicketNumber]", serviceRequest.ServiceRequestNumber);
                    //// Get translated problem class
                    description.Replace("[ProblemClass]", ServiceRequestHelper.GetTranslatedProblemClass(service, serviceRequest.ProblemClassName, languageCode));
                    //// Get translated problem type
                    description.Replace("[ProblemType]", ServiceRequestHelper.GetTranslatedProblemTypes(service, serviceRequest.ProblemTypeName, languageCode));
                    ////Get translated status
                    int statusCode = ((OptionSetValue)entity["statuscode"]).Value;
                    string translatedStatus = ServiceRequestHelper.GetTranslatedStatus(service, statusCode, languageCode);
                    description.Replace("[ServiceRequestStatus]", string.IsNullOrEmpty(translatedStatus) == false ? translatedStatus : serviceRequest.StatusCode);
                    description.Replace("[BuildingName]", serviceRequest.ProblemBuildingName);
                    description.Replace("[RoomNo]", serviceRequest.ProblemRoomNumber);
                    if (!string.IsNullOrEmpty(serviceRequest.ProblemTypeDescription))
                    {
                        description.Replace("[ProblemDescription]", serviceRequest.ProblemTypeDescription);
                    }
                    else
                    {
                        description.Replace("[ProblemDescription]", string.Empty);
                    }

                    description.Replace("[Sender]", "(GFSC Portal Address)");
                    description.Replace("[ServiceRequestCompletedOn]", DateTime.Now.ToShortDateString());
                    if (string.IsNullOrEmpty(serviceRequest.ProblemFloorName))
                    {
                        description.Replace("[Floor]", "N/A");
                    }
                    else
                    {
                        description.Replace("[Floor]", serviceRequest.ProblemFloorName);
                    }

                    //// get translated room type
                    description.Replace("[RoomType]", ServiceRequestHelper.GetTranslatedRoomTypes(service, serviceRequest.ProblemRoomTypeName, languageCode));
                    description.Replace("[RequestorName]", serviceRequest.RequestorFirstName + " " + serviceRequest.RequestorLastName);
                    description.Replace("[RequestorAlias]", serviceRequest.RequestorAlias);
                    description.Replace("[RequestorPhoneNo]", serviceRequest.RequestorPhoneNumber);
                    description.Replace("[ContactName]", serviceRequest.ContactFirstName + " " + serviceRequest.ContactLastName);
                    description.Replace("[ContactAlias]", serviceRequest.ContactAlias);
                    description.Replace("[ContactPhoneNo]", serviceRequest.ContactPhoneNumber);
                    if (entity.Attributes.Contains("smp_requestcancelledreason"))
                    {
                        description.Replace("[CancellationReason]", (string)entity.Attributes["smp_requestcancelledreason"]);
                    }
                    else
                    {
                        description.Replace("[CancellationReason]", string.Empty);
                    }

                    if (entity.Attributes.Contains("smp_completeddate"))
                    {
                        description.Replace("[ServiceRequestCompletedOn]", Convert.ToDateTime(entity.Attributes["smp_completeddate"].ToString(), CultureInfo.InvariantCulture).ToShortDateString());
                    }
                    else
                    {
                        description.Replace("[ServiceRequestCompletedOn]", string.Empty);
                    }

                    if (entity.Attributes.Contains("smp_completionnotes"))
                    {
                        description.Replace("[SRCompletionNotes]", (string)entity.Attributes["smp_completionnotes"]);
                    }
                    else
                    {
                        description.Replace("[SRCompletionNotes]", string.Empty);
                    }

                    ////Test record
                    if (entity.Attributes.Contains("smp_duedate"))
                    {
                        string priorityName = serviceRequest.Priority;
                        string configurationEntityProrityValue = collection.Entities.Where(x => x["smp_title"].ToString().Equals("PrioritiesToDisplayTheDueDate")).First()["smp_value"].ToString();
                        if (configurationEntityProrityValue.Contains(priorityName))
                        {
                            description.Replace("[DueDate]", entity.Attributes["smp_duedatebybuildingtimezone"].ToString());
                        }
                        else
                        {
                            ////description.Replace("[DueDate]", string.Empty);
                            ////description.Replace("[DueDate]", ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequestConfirmation/ToBeScheduled"));
                            if (languageCode == "en-us")
                            {
                                description.Replace("[DueDate]", collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestConfirmation/ToBeScheduled")).First()["smp_value"].ToString());
                            }
                            else
                            {
                                description.Replace("[DueDate]", collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestConfirmation/ToBeScheduled (" + languageCode + ")")).First()["smp_value"].ToString());
                            }
                        }
                    }
                    else
                    {
                        ////description.Replace("[DueDate]", ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequestConfirmation/ToBeScheduled"));
                        if (languageCode == "en-us")
                        {
                            description.Replace("[DueDate]", collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestConfirmation/ToBeScheduled")).First()["smp_value"].ToString());
                        }
                        else
                        {
                            description.Replace("[DueDate]", collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestConfirmation/ToBeScheduled (" + languageCode + ")")).First()["smp_value"].ToString());
                        }
                    }

                    StringBuilder html = new StringBuilder();
                    if (description.ToString().ToLowerInvariant().Contains("[dynamicproblemtypenotes]"))
                    {
                        if (serviceRequest.DynamicProblemTypeNotesCollection.Count() > 0)
                        {
                            html.Append("<table style=\"margin-left:35px\">");
                            html.Append("<tr><td style=\"font-family:Calibri;font-size:18px;\" colspan='2'><b><u>Dynamic Problem Type Notes</u></b></td></tr>");
                            foreach (DynamicProblemTypeNotes dptNotes in serviceRequest.DynamicProblemTypeNotesCollection)
                            {
                                html.Append("<table style=\"margin-left:35px\">");
                                html.Append("<tr><td style=\"font-family:Calibri;font-size:16px;\"><b>");
                                html.Append(dptNotes.DynamicsProblemTypeName);
                                html.Append(" : </b></tr></td>");
                                html.Append("<tr><td  style=\"font-family:Calibri;font-size:16px;\">");
                                html.Append(dptNotes.Answer);
                                html.Append("</tr></td>");
                                html.Append("</table>");
                            }

                            html.Append("</table>");
                        }
                    }

                    description.Replace("[DynamicProblemTypeNotes]", html.ToString());
                    if (entity.Attributes.Contains("statuscode"))
                    {
                        OptionSetValue entityStatusCode = (OptionSetValue)entity.Attributes["statuscode"];
                        if (entityStatusCode.Value == 180620017 || entityStatusCode.Value == 180620007 || entityStatusCode.Value == 180620008 || entityStatusCode.Value == 180620009)
                        {
                            EntityCollection notesCollection = GetQueryResponse(service, "annotation", new string[] { "annotationid", "subject", "notetext", "createdon" }, "objectid", entity.Id);
                            if (notesCollection.Entities.Count > 0)
                            {
                                ////StringBuilder htm = new StringBuilder();
                                ////htm.Append("<table>");
                                ////htm.Append("<tr><td><b>");
                                ////htm.Append(ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequest/EmailTemplates/Notes"));
                                ////htm.Append("</b></td>");
                                ////htm.Append("<td  style=\"color:red\">");
                                ////htm.Append(notesCollection.Entities[0].Attributes["notetext"].ToString());
                                ////htm.Append("</td></tr></table>");
                                ////description.Replace("[NotesName]", ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequest/EmailTemplates/Notes"));
                                ////description.Replace("[DueDate]", ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequestConfirmation/ToBeScheduled"));
                                if (languageCode == "en-us")
                                {
                                    description.Replace("[NotesName]", collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequest/EmailTemplates/Notes")).First()["smp_value"].ToString());
                                }
                                else
                                {
                                    description.Replace("[NotesName]", collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequest/EmailTemplates/Notes (" + languageCode + ")")).First()["smp_value"].ToString());
                                }

                                description.Replace("[NoteProvidedWhenPlacedOnHold]", notesCollection.Entities[0].Attributes["notetext"].ToString());
                                ////description.Replace("[Notes]", string.Format("<b>{0}</b> {1}", ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequest/EmailTemplates/Notes"), notesCollection.Entities[0].Attributes["notetext"].ToString()));
                                ////string.Format("<div><b>{0}</b> <p style=\"color:red;\">{1}</p></div>", ServiceRequestHelper.GetValueFromWebsiteLanguage(service, languageCode, "ServiceRequest/EmailTemplates/Notes"), notesCollection.Entities[0].Attributes["notetext"].ToString())
                            }
                            else
                            {
                                description.Replace("[NotesName]", string.Empty);
                                description.Replace("[NoteProvidedWhenPlacedOnHold]", string.Empty);
                            }
                        }
                        else
                        {
                            description.Replace("[NotesName]", string.Empty);
                            description.Replace("[NoteProvidedWhenPlacedOnHold]", string.Empty);
                        }
                    }

                    description.Replace("[gfscLink]", "<a href=\"" + collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestPortalLink")).First()["smp_value"].ToString() + "\"style=\"color: #0563C1; font-family: Segoe UI;\">www.msfacilities.com</a>");
                    ////code change for survey snippet by mihika
                    ////if (languageCode == "en-us" || string.IsNullOrEmpty(languageCode))
                    ////{
                    ////    surveyLink = string.Format(CultureInfo.InvariantCulture, ServiceRequestHelper.GetConfigurationValue(service, "ServiceRequestSurveyLink"), string.Empty);
                    ////}
                    ////else
                    ////{
                    ////    surveyLink = string.Format(CultureInfo.InvariantCulture, ServiceRequestHelper.GetConfigurationValue(service, "ServiceRequestSurveyLink"), "/" + languageCode);
                    ////}

                    ////description.Replace("[Link]", "<a href=" + surveyLink + serviceRequestId + ">Survey Link</a>");
                    surveyLink = string.Format(CultureInfo.InvariantCulture, collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestSurveyLink")).First()["smp_value"].ToString(), string.Empty);
                    description.Replace("[Link]", surveyLink);
                    string srDetails = string.Empty;
                    if (languageCode == "en-us")
                    {
                        srDetails = collection.Entities.Where(x => x["smp_title"].ToString().Equals("Seemoreinformationaboutyourservicerequest")).First()["smp_value"].ToString();
                    }
                    else
                    {
                        srDetails = collection.Entities.Where(x => x["smp_title"].ToString().Equals("Seemoreinformationaboutyourservicerequest (" + languageCode + ")")).First()["smp_value"].ToString();
                    }

                    Guid requestId = Guid.Empty;
                    if (entity.Attributes.Contains("smp_reclassifiedsr"))
                    {
                        requestId = ((EntityReference)entity.Attributes["smp_reclassifiedsr"]).Id;
                    }
                    else
                    {
                        requestId = entity.Id;
                    }

                    if (entity.Attributes.Contains("smp_reasonforreclassifyingtherecordmemo"))
                    {
                        description.Replace("[ReclassificationReason]", (string)entity.Attributes["smp_reasonforreclassifyingtherecordmemo"]);
                    }

                    if (entity.Attributes.Contains("smp_reclassifiedsr"))
                    {
                        Entity reclassifiedSR = service.Retrieve("incident", ((EntityReference)entity.Attributes["smp_reclassifiedsr"]).Id, new ColumnSet("ticketnumber"));
                        description.Replace("[NewTicketNumber]", (string)reclassifiedSR["ticketnumber"]);
                    }

                    description.Replace("[Seemoreinformationaboutyourservicerequest]", "<a href=\"" + collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequestPortalLink")).First()["smp_value"].ToString() + languageCode + "/request/?id=" + requestId + "\"style=\"color: #0563C1; font-family: Segoe UI;\">" + srDetails + "</a>");
                    email["description"] = string.Format(CultureInfo.InstalledUICulture, "<div style=\"font-family:Tahoma;font-size:12px;\"> {0} </div>", description.ToString());
                    email.LogicalName = "email";

                    if (toParty != null)
                    {
                        email["to"] = toParty.ToArray();
                    }

                    email["from"] = new[] { fromParty };
                    email["regardingobjectid"] = new EntityReference("incident", entity.Id);
                    if (carbonCopyParty != null && carbonCopyParty.Count > 0)
                    {
                        email["cc"] = carbonCopyParty.ToArray();
                    }

                    Guid emailId = service.Create(email);

                    //// Use the SendEmail message to send an e-mail message.
                    var sendEmailreq = new SendEmailRequest
                    {
                        EmailId = emailId,
                        TrackingToken = string.Empty,
                        IssueSend = true
                    };
                    service.Execute(sendEmailreq);
                    isEmailSent = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isEmailSent;
        }

        /// <summary>
        /// Send approval email
        /// </summary>
        /// <param name="service">service object</param>
        /// <param name="incidentId">incident id</param>
        ////public static void SendApprovalEmail(IOrganizationService service, Entity entity,EntityCollection collection)
        ////{
        ////    try
        ////    {
        ////        if (service != null && entity!=null)
        ////        {
        ////            List<Entity> toEmail = new List<Entity>();
        ////            List<Entity> carbonCopyEmail = new List<Entity>();

        ////            // Get the configured user id for sending email.
        ////            string fromUserDomainName = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.EmailSenderDomainName)).First()["smp_value"].ToString();
        ////            Guid senderId = ServiceRequestHelper.GetCrmUserId(service, fromUserDomainName);

        ////            bool isApprovalRequired = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestIsApprovalRequired) ? (bool)entity[CRMAttributesResource.ServiceRequestIsApprovalRequired] : false;
        ////            OptionSetValue currentStatus = entity.Attributes.Contains(CRMAttributesResource.StatusCodeAttribute) ? (OptionSetValue)entity[CRMAttributesResource.StatusCodeAttribute] : null;

        ////            var fromParty = new Entity("activityparty");
        ////            fromParty["partyid"] = new EntityReference(CRMAttributesResource.SystemUserEntity, senderId);

        ////            if (isApprovalRequired && currentStatus.Value == Convert.ToInt32(CRMAttributesResource.PendingApporvalStatusValue, CultureInfo.InvariantCulture))
        ////            {
        ////                EntityReference contact = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestContact) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestContact] : null;
        ////                EntityReference requester = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestRequester) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestRequester] : null;
        ////                EntityReference approver = entity.Attributes.Contains(CRMAttributesResource.ServiceRequestApprovalManager) ? (EntityReference)entity[CRMAttributesResource.ServiceRequestApprovalManager] : null;

        ////                if (approver != null)
        ////                {
        ////                    var toApprover = new Entity("activityparty");
        ////                    toApprover["partyid"] = new EntityReference("contact", approver.Id);
        ////                    toEmail.Add(toApprover);
        ////                }

        ////                if (requester != null)
        ////                {
        ////                    if (requester.Id != contact.Id)
        ////                    {
        ////                        var toRequester = new Entity("activityparty");
        ////                        toRequester["partyid"] = new EntityReference("contact", requester.Id);
        ////                        carbonCopyEmail.Add(toRequester);
        ////                    }
        ////                }

        ////                if (contact != null)
        ////                {
        ////                    var toContact = new Entity("activityparty");
        ////                    toContact["partyid"] = new EntityReference("contact", contact.Id);
        ////                    carbonCopyEmail.Add(toContact);
        ////                }

        ////                //// get language code for contact
        ////                string contactLanguageCode = ServiceRequestHelper.GetContactPreferredLanguage(service, contact.Id);

        ////                // Send email
        ////                EmailHelper.SendTranslatedEmail(service, entity.Id, fromParty, toEmail, carbonCopyEmail, contactLanguageCode, CRMAttributesResource.ServiceRequestApprovalRequiredTemplate,collection);
        ////            }
        ////        }
        ////    }
        ////    catch (Exception)
        ////    {
        ////        throw;
        ////    }
        ////}

        /// <summary>
        /// Gets the CDATA element from the XmlString
        /// </summary>
        /// <param name="value">The XmlString to be parsed</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>the CDATA value of the Element with the attribute specified</returns>
        private static string GetDataFromXml(string value, string attributeName)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            XDocument document = XDocument.Parse(value);

            //// get the Element with the attribute name specified
            XElement element = document.Descendants().Where(ele => ele.Attributes().Any(attr => attr.Name == attributeName)).FirstOrDefault();

            return element == null ? string.Empty : element.Value;
        }

        /// <summary>
        /// Gets the name of the template by.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="title">The title.</param>
        /// <returns>Return the template entity.</returns>
        private static Entity GetTemplateByName(IOrganizationService service, string title)
        {
            var query = new QueryExpression();
            query.EntityName = "template";

            query.ColumnSet = new ColumnSet(new string[] { "subject", "body" });

            var filter = new FilterExpression();
            var condition = new ConditionExpression("title", ConditionOperator.Equal, new object[] { title });
            filter.AddCondition(condition);
            query.Criteria = filter;

            EntityCollection allTemplates = service.RetrieveMultiple(query);
            Entity emailTemplate = new Entity();
            if (allTemplates.Entities.Count > 0)
            {
                emailTemplate = allTemplates.Entities[0];
            }

            return emailTemplate;
        }

        private static EntityCollection GetQueryResponse(IOrganizationService service, string entityLogicalName, string[] fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = entityLogicalName;
                query.ColumnSet = new ColumnSet(fieldsToBeFetched);

                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression(criteriaField, ConditionOperator.Equal, criteriaValue));
                query.AddOrder("createdon", OrderType.Descending);
                query.Criteria = filter;

                EntityCollection entityCollection = service.RetrieveMultiple(query);

                return entityCollection;
            }

            return null;
        }
    }
}
