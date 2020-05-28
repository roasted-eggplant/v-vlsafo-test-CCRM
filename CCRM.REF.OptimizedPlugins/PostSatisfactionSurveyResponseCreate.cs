// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuestionVariable.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  QuestionVariable Plugin
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

    public static class QuestionVariable
    {
        public static string Question1 { get; set; }

        public static string Question2 { get; set; }

        public static string Question3 { get; set; }

        public static string Question4 { get; set; }

        public static string Question5 { get; set; }

        public static string[] SurveyQuestions { get; set; }

        public static string Question1Text { get; set; }

        public static string Question2Text { get; set; }

        public static string Question3Text { get; set; }

        public static string Question4Text { get; set; }

        public static string Question5Text { get; set; }

        public static string ContactLanguageCode { get; set; }

        public static string ProblemClass { get; set; }

        public static string ProblemType { get; set; }
    }

    public class PostSatisfactionSurveyResponseCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //// Obtain the execution context service from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //// Obtain the tracing service from the service provider.
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            //// Use the factory to generate the Organization Service.
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            ////throw new NotImplementedException();
            Guid incidentId = Guid.Empty;

            if (context == null || context.Depth != 4)
            {
                return;
            }

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                trace.Trace("Inside Target method");
                Entity entity = (Entity)context.InputParameters["Target"];
                Entity postImage = (Entity)context.PostEntityImages["PostImage"];
                EntityCollection configurationCollection = ServiceRequestHelper.GetConfigurationValueCollection(service);
                string providerQuestionOne = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("ProviderQuestionOne")).First()["smp_value"].ToString();
                string providerQuestionTwo = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("ProviderQuestionTwo")).First()["smp_value"].ToString();
                string callCenterQuestionOne = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("CallCenterQuestionOne")).First()["smp_value"].ToString();
                string callCenterQuestionTwo = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("CallCenterQuestionTwo")).First()["smp_value"].ToString();
                string singleResponseQuestion = configurationCollection.Entities.Where(x => x["smp_title"].ToString().Equals("SingleResponseQuestion")).First()["smp_value"].ToString();
                ////My issue was resolved to my satisfaction
                if (entity.Attributes.Contains(providerQuestionOne))
                {
                    QuestionVariable.Question1 = Convert.ToString(((Microsoft.Xrm.Sdk.OptionSetValue)entity[providerQuestionOne]).Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    QuestionVariable.Question1 = string.Empty;
                }

                ////The service provider left the working area in a clean and orderly fashion
                if (entity.Attributes.Contains(providerQuestionTwo))
                {
                    QuestionVariable.Question2 = Convert.ToString(((Microsoft.Xrm.Sdk.OptionSetValue)entity[providerQuestionTwo]).Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    QuestionVariable.Question2 = string.Empty;
                }

                ////Was it easy for you to request service?
                if (entity.Attributes.Contains(callCenterQuestionOne))
                {
                    QuestionVariable.Question3 = Convert.ToString(((Microsoft.Xrm.Sdk.OptionSetValue)entity[callCenterQuestionOne]).Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    QuestionVariable.Question3 = string.Empty;
                }

                ////How satisfied were you with the overall quality of
                if (entity.Attributes.Contains(callCenterQuestionTwo))
                {
                    QuestionVariable.Question4 = Convert.ToString(((Microsoft.Xrm.Sdk.OptionSetValue)entity[callCenterQuestionTwo]).Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    QuestionVariable.Question4 = string.Empty;
                }

                ////Please select any item which you feel could use improvement
                if (entity.Attributes.Contains(singleResponseQuestion))
                {
                    QuestionVariable.Question5 = Convert.ToString(((Microsoft.Xrm.Sdk.OptionSetValue)entity[singleResponseQuestion]).Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    QuestionVariable.Question5 = string.Empty;
                }

                if (postImage.Attributes.Contains("smp_incidentid"))
                {
                    incidentId = ((Microsoft.Xrm.Sdk.EntityReference)postImage.Attributes["smp_incidentid"]).Id;
                }

                trace.Trace("Inside Before Create");
                CreateServiceRequest(service, incidentId, QuestionVariable.Question1, QuestionVariable.Question2, QuestionVariable.Question3, QuestionVariable.Question4, QuestionVariable.Question5, trace, configurationCollection);
            }
        }

        /// <summary>
        /// Creates the service request.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <param name="question1">The question1.</param>
        /// <param name="question2">The question2.</param>
        /// <param name="question3">The question3.</param>
        /// <param name="question4">The question4.</param>
        /// <param name="question5">The question5.</param>
        /// <param name="question5Chinese">The question5 chinese.</param>
        /// <param name="question5Portuguese">The question5 portuguese.</param>
        /// <param name="question5French">The question5 french.</param>
        /// <param name="question5Spanish">The question5 spanish.</param>
        /// <param name="question5Korean">The question5 korean.</param>
        /// <param name="question5Japanese">The question5 japanese.</param>
        private static void CreateServiceRequest(IOrganizationService service, Guid serviceRequestId, string question1, string question2, string question3, string question4, string question5, ITracingService trace, EntityCollection collection)
        {
            trace.Trace("Inside CreateServiceRequest");
            Guid incidentId = Guid.Empty;
            string defaultProvider = string.Empty;
            Guid contactId = Guid.Empty;
            Entity serviceRequest = service.Retrieve("incident", serviceRequestId, new ColumnSet(true));
            if (serviceRequest.Attributes.Contains("smp_contact"))
            {
                contactId = ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_contact"]).Id;
            }

            Entity contactInfo = ServiceRequestHelper.GetContactDetails(service, contactId);
            QuestionVariable.ContactLanguageCode = contactInfo.Attributes.Contains("smp_preferredlanguage") ? contactInfo["smp_preferredlanguage"].ToString() : "en-us";
            Entity incident = new Entity();
            incident.LogicalName = "incident";
            incident.Attributes.Add("smp_problemoccureddatetime", DateTime.Now.ToUniversalTime());
            defaultProvider = collection.Entities.Where(x => x["smp_title"].ToString().Equals("DefaultProviderName")).First()["smp_value"].ToString();
            incident.Attributes.Add("customerid", new EntityReference("account", GetDefaultProviderId(service, defaultProvider)));
            incident.Attributes.Add("smp_referencesr", new EntityReference("incident", serviceRequestId));
            incident.Attributes.Add("smp_issurveyservicerequest", true);
            QuestionVariable.SurveyQuestions = collection.Entities.Where(x => x["smp_title"].ToString().Equals("SurveyQuestions")).First()["smp_value"].ToString().Split(';');
            QuestionVariable.Question1Text = QuestionVariable.SurveyQuestions[0].ToString();
            QuestionVariable.Question2Text = QuestionVariable.SurveyQuestions[1].ToString();
            QuestionVariable.Question3Text = QuestionVariable.SurveyQuestions[2].ToString();
            QuestionVariable.Question4Text = QuestionVariable.SurveyQuestions[3].ToString();
            QuestionVariable.Question5Text = QuestionVariable.SurveyQuestions[4].ToString();
            incident.Attributes.Add("smp_problemtypedescription", BuildSurveyDetailsToProblemDescription(service, QuestionVariable.Question1Text, QuestionVariable.Question2Text, QuestionVariable.Question3Text, QuestionVariable.Question4Text, QuestionVariable.Question5Text, question1, question2, question3, question4, question5, serviceRequest.Attributes["ticketnumber"].ToString(), QuestionVariable.ContactLanguageCode));
            ////requester details
            BindRequstorDetails(incident, serviceRequest, trace);
            //// Contact Details
            BindContactDetails(incident, serviceRequest, trace);
            //// Building Details
            BindBuildingDetails(incident, serviceRequest, trace);
            //// Provider Related response time, technitian
            if (question1 == "1" || question1 == "2" || question2 == "1" || question2 == "2" || question5 == "3" || question5 == "4")
            {
                QuestionVariable.ProblemClass = collection.Entities.Where(x => x["smp_title"].ToString().Equals("SurveyProviderRelatedProblemClass")).First()["smp_value"].ToString();
                QuestionVariable.ProblemType = collection.Entities.Where(x => x["smp_title"].ToString().Equals("SurveyProviderRelatedProblemType")).First()["smp_value"].ToString();
                incident.Attributes.Add("smp_problemclassid", new EntityReference("smp_problemclass", GetProblemClassId(service, QuestionVariable.ProblemClass)));
                incident.Attributes.Add("smp_problemtypeid", new EntityReference("smp_problemtype", GetProblemTypeId(service, QuestionVariable.ProblemType)));
                incident["statuscode"] = new OptionSetValue(2);
                incidentId = service.Create(incident);
                UpdateIncident(service, incidentId);
                trace.Trace("After UpdateIncidnet funciton");
            }

            //// Call Center Management Related ease of use, technology
            if (question3 == "1" || question3 == "2" || question4 == "1" || question4 == "2" || question5 == "1" || question5 == "2")
            {
                QuestionVariable.ProblemClass = collection.Entities.Where(x => x["smp_title"].ToString().Equals("SurveyCallCenterManagementRelatedProblemClass")).First()["smp_value"].ToString();
                QuestionVariable.ProblemType = collection.Entities.Where(x => x["smp_title"].ToString().Equals("SurveyCallCenterManagementRelatedProblemType")).First()["smp_value"].ToString();
                incident.Attributes.Remove("smp_problemclassid");
                incident.Attributes.Remove("smp_problemtypeid");
                incident.Attributes.Add("smp_problemclassid", new EntityReference("smp_problemclass", GetProblemClassId(service, QuestionVariable.ProblemClass)));
                incident.Attributes.Add("smp_problemtypeid", new EntityReference("smp_problemtype", GetProblemTypeId(service, QuestionVariable.ProblemType)));
                incident["statuscode"] = new OptionSetValue(2);
                incidentId = service.Create(incident);
                trace.Trace("Incident Created- " + incidentId);
                ////SetState(service, incidentId);
                ////Entity updateIncident = new Entity();
                ////updateIncident.LogicalName = "incident";
                ////updateIncident.Id = serviceRequestId;
                ////updateIncident.Attributes["statecode"] = (OptionSet)
                trace.Trace("After SetState funciton");
                UpdateIncident(service, incidentId);
                trace.Trace("After UpdateIncidnet funciton");
            }
        }

        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The incident id.</param>
        ////private static void SetState(IOrganizationService service, Guid incidentId)
        ////{
        ////    SetStateRequest state = new SetStateRequest();
        ////    //// Set the Request Object's Properties
        ////    state.State = new OptionSetValue(0); //// Active
        ////    state.Status = new OptionSetValue(2); ////Open
        ////    //// Point the Request to the case whose state is being changed
        ////    state.EntityMoniker = new EntityReference("incident", incidentId);
        ////    //// Execute the Request
        ////    service.Execute(state);
        ////}

        /// <summary>
        /// Gets the problem class id.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="problemClass">The problem class.</param>
        /// <returns>Problem Class Id</returns>
        private static Guid GetProblemClassId(IOrganizationService service, string problemClass)
        {
            try
            {
                Guid problemClassId = Guid.Empty;
                QueryExpression query = new QueryExpression();
                query.EntityName = "smp_problemclass";
                query.ColumnSet = new ColumnSet("smp_problemclassid");

                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression("smp_problemclassname", ConditionOperator.Equal, problemClass));
                query.Criteria = filter;

                EntityCollection entityCollection = service.RetrieveMultiple(query);
                if (entityCollection != null && entityCollection.Entities.Count > 0 && entityCollection.Entities[0].Attributes.Contains("smp_problemclassid"))
                {
                    problemClassId = new Guid(entityCollection.Entities[0].Attributes["smp_problemclassid"].ToString());
                }

                return problemClassId;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting problem class.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the problem type id.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="problemType">Type of the problem.</param>
        /// <returns>Problem Type Id</returns>
        private static Guid GetProblemTypeId(IOrganizationService service, string problemType)
        {
            try
            {
                Guid problemTypeId = Guid.Empty;
                QueryExpression query = new QueryExpression();
                query.EntityName = "smp_problemtype";
                query.ColumnSet = new ColumnSet("smp_problemtypeid");

                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression("smp_problemtypename", ConditionOperator.Equal, problemType));
                query.Criteria = filter;

                EntityCollection entityCollection = service.RetrieveMultiple(query);
                if (entityCollection != null && entityCollection.Entities.Count > 0 && entityCollection.Entities[0].Attributes.Contains("smp_problemtypeid"))
                {
                    problemTypeId = new Guid(entityCollection.Entities[0].Attributes["smp_problemtypeid"].ToString());
                }

                return problemTypeId;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting problem type.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the default provider id.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <returns>Provider Id</returns>
        private static Guid GetDefaultProviderId(IOrganizationService service, string accountName)
        {
            try
            {
                Guid accountId = Guid.Empty;
                QueryExpression query = new QueryExpression();
                query.EntityName = "account";
                query.ColumnSet = new ColumnSet("accountid");

                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression("name", ConditionOperator.Equal, accountName));
                query.Criteria = filter;

                EntityCollection entityCollection = service.RetrieveMultiple(query);
                if (entityCollection != null && entityCollection.Entities.Count > 0 && entityCollection.Entities[0].Attributes.Contains("accountid"))
                {
                    accountId = new Guid(entityCollection.Entities[0].Attributes["accountid"].ToString());
                }

                return accountId;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting provider details.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Updates the incident.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        private static void UpdateIncident(IOrganizationService service, Guid serviceRequestId)
        {
            Entity updateIncident = new Entity();
            updateIncident.LogicalName = "incident";
            updateIncident.Id = serviceRequestId;
            updateIncident.Attributes.Add("smp_issurveyservicerequest", false);
            service.Update(updateIncident);
        }

        /// <summary>
        /// Binds the requestor details.
        /// </summary>
        /// <param name="incident">The incident.</param>
        /// <param name="serviceRequest">The service request.</param>
        private static void BindRequstorDetails(Entity incident, Entity serviceRequest, ITracingService trace)
        {
            try
            {
                if (serviceRequest.Attributes.Contains("smp_requestorid"))
                {
                    incident.Attributes.Add("smp_requestorid", new EntityReference("contact", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_requestorid"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_buildingid"))
                {
                    incident.Attributes.Add("smp_buildingid", new EntityReference("smp_building", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_buildingid"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_requestorphone"))
                {
                    incident.Attributes.Add("smp_requestorphone", serviceRequest.Attributes["smp_requestorphone"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_requestoralias"))
                {
                    incident.Attributes.Add("smp_requestoralias", serviceRequest.Attributes["smp_requestoralias"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_requestorroomno"))
                {
                    incident.Attributes.Add("smp_requestorroomno", serviceRequest.Attributes["smp_requestorroomno"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_requestoremail"))
                {
                    incident.Attributes.Add("smp_requestoremail", serviceRequest.Attributes["smp_requestoremail"].ToString());
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while binding requester details.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Binds the contact details.
        /// </summary>
        /// <param name="incident">The incident.</param>
        /// <param name="serviceRequest">The service request.</param>
        private static void BindContactDetails(Entity incident, Entity serviceRequest, ITracingService trace)
        {
            try
            {
                if (serviceRequest.Attributes.Contains("smp_contact"))
                {
                    incident.Attributes.Add("smp_contact", new EntityReference("contact", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_contact"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_contactbuilding"))
                {
                    incident.Attributes.Add("smp_contactbuilding", new EntityReference("smp_building", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_contactbuilding"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_contactemail"))
                {
                    incident.Attributes.Add("smp_contactemail", serviceRequest.Attributes["smp_contactemail"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_contactphone"))
                {
                    incident.Attributes.Add("smp_contactphone", serviceRequest.Attributes["smp_contactphone"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_costcentercode"))
                {
                    incident.Attributes.Add("smp_costcentercode", serviceRequest.Attributes["smp_costcentercode"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_contactalias"))
                {
                    incident.Attributes.Add("smp_contactalias", serviceRequest.Attributes["smp_contactalias"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_contactroom"))
                {
                    incident.Attributes.Add("smp_contactroom", serviceRequest.Attributes["smp_contactroom"].ToString());
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while binding contact details.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Binds the building details.
        /// </summary>
        /// <param name="incident">The incident.</param>
        /// <param name="serviceRequest">The service request.</param>
        private static void BindBuildingDetails(Entity incident, Entity serviceRequest, ITracingService trace)
        {
            try
            {
                if (serviceRequest.Attributes.Contains("smp_problembuilding"))
                {
                    incident.Attributes.Add("smp_problembuilding", new EntityReference("smp_building", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_problembuilding"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingaddressline2"))
                {
                    incident.Attributes.Add("smp_problembuildingaddressline2", serviceRequest.Attributes["smp_problembuildingaddressline2"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingstate"))
                {
                    incident.Attributes.Add("smp_problembuildingstate", serviceRequest.Attributes["smp_problembuildingstate"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingzipcode"))
                {
                    incident.Attributes.Add("smp_problembuildingzipcode", serviceRequest.Attributes["smp_problembuildingzipcode"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_problemroomtype"))
                {
                    incident.Attributes.Add("smp_problemroomtype", new EntityReference("smp_roomtype", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_problemroomtype"]).Id));
                }

                if (serviceRequest.Attributes.Contains("new_problemroomnumber"))
                {
                    incident.Attributes.Add("new_problemroomnumber", new EntityReference("smp_room", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["new_problemroomnumber"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingaddressline1"))
                {
                    incident.Attributes.Add("smp_problembuildingaddressline1", serviceRequest.Attributes["smp_problembuildingaddressline1"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingcity"))
                {
                    incident.Attributes.Add("smp_problembuildingcity", serviceRequest.Attributes["smp_problembuildingcity"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingcountry"))
                {
                    incident.Attributes.Add("smp_problembuildingcountry", serviceRequest.Attributes["smp_problembuildingcountry"].ToString());
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingtimezone"))
                {
                    incident.Attributes.Add("smp_problembuildingtimezone", new EntityReference("smp_timezone", ((Microsoft.Xrm.Sdk.EntityReference)serviceRequest.Attributes["smp_problembuildingtimezone"]).Id));
                }

                if (serviceRequest.Attributes.Contains("smp_problemroom"))
                {
                    incident.Attributes.Add("smp_problemroom", serviceRequest.Attributes["smp_problemroom"].ToString());
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while binding building details.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the options set text on value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <returns>Option set Value.</returns>
        ////private static string GetOptionsSetTextOnValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
        ////{
        ////    try
        ////    {
        ////        RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
        ////        {
        ////            EntityLogicalName = entityName,
        ////            LogicalName = attributeName,
        ////            RetrieveAsIfPublished = true
        ////        };
        ////        RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);

        ////        Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata retrievedPicklistAttributeMetadata = (Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)

        ////        retrieveAttributeResponse.AttributeMetadata; //// Get the current options list for the retrieved attribute.
        ////        OptionMetadata[] optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
        ////        string selectedOptionLabel = string.Empty;
        ////        foreach (OptionMetadata oMD in optionList)
        ////        {
        ////            if (oMD.Value == selectedValue)
        ////            {
        ////                selectedOptionLabel = oMD.Label.UserLocalizedLabel.Label;
        ////            }
        ////        }

        ////        return selectedOptionLabel;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting time zone offset text.", ex);
        ////        throw customEx;
        ////    }
        ////}

        /// <summary>
        /// Builds the survey details to problem description.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="question1Text">The question1 text.</param>
        /// <param name="question2Text">The question2 text.</param>
        /// <param name="question3Text">The question3 text.</param>
        /// <param name="question4Text">The question4 text.</param>
        /// <param name="question5Text">The question5 text.</param>
        /// <param name="answer1">The answer1.</param>
        /// <param name="answer2">The answer2.</param>
        /// <param name="answer3">The answer3.</param>
        /// <param name="answer4">The answer4.</param>
        /// <param name="answer5">The answer5.</param>
        /// <param name="serviceRequestNumber">The service request number.</param>
        /// <param name="contactLanguageCode">The contact language code.</param>
        /// <returns>Problem Description</returns>
        private static string BuildSurveyDetailsToProblemDescription(IOrganizationService service, string question1Text, string question2Text, string question3Text, string question4Text, string question5Text, string answer1, string answer2, string answer3, string answer4, string answer5, string serviceRequestNumber, string contactLanguageCode)
        {
            try
            {
                return "Service Request '" + serviceRequestNumber.ToString() + "'  with negative results. " + Environment.NewLine + question1Text + Environment.NewLine + answer1 + Environment.NewLine + question2Text + Environment.NewLine + answer2 + Environment.NewLine + question3Text + Environment.NewLine + answer3 + Environment.NewLine + question4Text + Environment.NewLine + answer4 + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
