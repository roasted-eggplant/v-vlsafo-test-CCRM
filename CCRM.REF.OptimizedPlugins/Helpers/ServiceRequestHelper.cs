// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ServiceRequestHelper Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using CCRM.REF.OptimizedPlugins.Entities;
    using Cmms.ServiceManagement.Services;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Class ServiceRequestHelper
    /// </summary>
    public static class ServiceRequestHelper
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private const string PreImageAlias = "PreImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private const string PostImageAlias = "PostImage";

        /// <summary>
        /// Sets the service request status.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <param name="currentStatusCode">The current status code of service request.</param>
        /// <param name="dispatchStatusCode">The dispatch status code.</param>
        public static void SetServiceRequestStatus(IOrganizationService service, Entity serviceRequest, string currentStatusCode, string dispatchStatusCode, int caseorigin, EntityCollection collection, Guid providerTeamId)
        {
            try
            {
                Entity toUpdate = new Entity("incident");
                toUpdate.Id = serviceRequest.Id;
                toUpdate["ownerid"] = new EntityReference(CRMAttributesResource.TeamEntity, providerTeamId);
                if (service != null && !string.IsNullOrWhiteSpace(currentStatusCode))
                {
                    if ((caseorigin == 3 || caseorigin == 180620005 || caseorigin == 100008999) && currentStatusCode == "PCSRDISP")
                    {
                        toUpdate["statuscode"] = new OptionSetValue(GetValueFromStatusCode(service, collection.Entities.Where(x => x["smp_title"].ToString().Equals(dispatchStatusCode)).First()["smp_value"].ToString()));
                    }
                    else
                    {
                        string validStatusCodes = collection.Entities.Where(x => x["smp_title"].ToString().Equals("StatusChangeOnCodes")).First()["smp_value"].ToString();
                        if (!string.IsNullOrWhiteSpace(validStatusCodes) && validStatusCodes.ToUpperInvariant().Contains(currentStatusCode.ToUpperInvariant()))
                        {
                            toUpdate["statuscode"] = new OptionSetValue(GetValueFromStatusCode(service, collection.Entities.Where(x => x["smp_title"].ToString().Equals(dispatchStatusCode)).First()["smp_value"].ToString()));
                        }
                    }

                    service.Update(toUpdate);
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error occurred while updating service request status. Exception : " + ex.Message, ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the status code value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>Returns the status code value of the specified status code, if not found then returns -999</returns>
        public static int GetValueFromStatusCode(IOrganizationService service, string statusCode)
        {
            int value = -999;
            try
            {
                EntityCollection entityCollection = GetQueryResponse(service, "smp_servicerequeststatuscode", new string[] { "smp_servicerequeststatus" }, "smp_name", statusCode);

                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if (entity.Attributes.Contains("smp_servicerequeststatus"))
                    {
                        value = ((OptionSetValue)entity.Attributes["smp_servicerequeststatus"]).Value;
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching value for status code {0} failed", statusCode), ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the status code value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns the status code  of the specified value</returns>
        public static string GetStatusCodeFromValue(IOrganizationService service, int value)
        {
            try
            {
                string code = null;
                EntityCollection entityCollection = GetQueryResponse(service, "smp_servicerequeststatuscode", new string[] { "smp_name" }, "smp_servicerequeststatus", value);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if (entity.Attributes.Contains("smp_name"))
                    {
                        code = (string)entity.Attributes["smp_name"];
                        return code;
                    }
                }

                return code;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching status code value for {0} failed.", value), ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="title">The title.</param>
        /// <returns>Return the value of the title</returns>
        public static string GetConfigurationValue(IOrganizationService service, string title)
        {
            try
            {
                string value = string.Empty;
                EntityCollection entityCollection = GetQueryResponse(service, "smp_configuration", new string[] { "smp_value", "smp_title" }, "smp_title", title);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    foreach (Entity entity in entityCollection.Entities)
                    {
                        if (entity.Attributes["smp_title"].ToString().ToLower() == title.ToLower())
                        {
                            value = entity.Attributes["smp_value"].ToString();
                        }
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching configuration value for {0} failed.", title), ex);
                throw customEx;
            }
        }

        public static EntityCollection GetConfigurationValueCollection(IOrganizationService service)
        {
            try
            {
                QueryExpression query = new QueryExpression("smp_configuration");
                query.ColumnSet = new ColumnSet("smp_title", "smp_value");
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                EntityCollection collection = service.RetrieveMultiple(query);
                if (collection.Entities.Count > 0)
                {
                    return collection;
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Fetching configuration value for failed.", ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        /// Gets the provider details.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The service request.</param>
        /// <returns>Provider object.</returns>
        public static Provider GetProviderDetails(IOrganizationService service, Guid providerId, EntityCollection collection, ITracingService trace)
        {
            trace.Trace("In GetProviderDetails");
            try
            {
                Provider provider = new Provider { IsProviderUpdated = false };
                if (service != null && providerId != Guid.Empty)
                {
                    trace.Trace("In GetProviderDetails Contains Provider");
                    trace.Trace("Provider Guid :" + providerId);
                    ////Commenting below line as encryption and AAD is not moving to Prod now.
                    Entity entity = service.Retrieve("account", providerId, new ColumnSet(new string[] { "name", "smp_iscmmsintegrationenabled", "smp_cmmsurl", CRMAttributesResource.ProviderTeamAttribute, "smp_hostedonazure" }));

                    if (entity != null)
                    {
                        trace.Trace("In GetProviderDetails Assign Provider Details");
                        string defaultProviderName = collection.Entities.Where(x => x["smp_title"].ToString().Equals("DefaultProviderName")).First()["smp_value"].ToString();
                        trace.Trace("Default Provider name :" + defaultProviderName);
                        if (entity.Attributes.Contains("name") && defaultProviderName != entity.Attributes["name"].ToString())
                        {
                            trace.Trace("In GetProviderDetails If Not Default Provider");
                            if (entity.Attributes.Contains("smp_iscmmsintegrationenabled"))
                            {
                                trace.Trace("In GetProviderDetails smp_iscmmsintegrationenabled");
                                provider.IsIntegrationEnabled = (bool)entity.Attributes["smp_iscmmsintegrationenabled"];
                            }

                            if (entity.Attributes.Contains("smp_cmmsurl"))
                            {
                                trace.Trace("In GetProviderDetails smp_cmmsurl");
                                provider.ProviderServiceAddress = (string)entity.Attributes["smp_cmmsurl"];
                            }

                            if (entity.Attributes.Contains(CRMAttributesResource.ProviderTeamAttribute))
                            {
                                provider.ProviderTeamId = ((EntityReference)entity[CRMAttributesResource.ProviderTeamAttribute]).Id;
                            }

                            //// User Story 3344963
                            ////: AAd/ SSL Implementationfor Tier1's -Outbound
                            if (entity.Attributes.Contains("smp_hostedonazure"))
                            {
                                provider.HostedOnAzure = (bool)entity["smp_hostedonazure"];
                            }

                            trace.Trace("In GetProviderDetails Befored Provider Lookup");
                            provider.ProviderId = providerId;
                            provider.IsProviderUpdated = true;
                        }
                    }
                }

                return provider;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error fetching Provider", ex.InnerException);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the problem class details.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="problemClassId">The problem class id.</param>
        /// <returns>The Problem Class entity details.</returns>
        public static Entity GetProblemClassDetails(IOrganizationService service, Guid problemClassId)
        {
            try
            {
                if (service != null)
                {
                    return service.Retrieve("smp_problemclass", problemClassId, new ColumnSet("smp_allowemailnotification", "smp_donotallowsurvey"));
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Problem class fetch failed", ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        /// Gets the problem type details.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="problemTypeId">The problem type id.</param>
        /// <returns>The Problem Type entity details.</returns>
        public static Entity GetProblemTypeDetails(IOrganizationService service, Guid problemTypeId)
        {
            try
            {
                if (service != null)
                {
                    return service.Retrieve("smp_problemtype", problemTypeId, new ColumnSet("smp_allowemailnotification", "smp_donotallowsurvey"));
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Problem type fetch failed", ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        /// Gets the service request details.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <returns>The Service Request entity details.</returns>
        public static Entity GetServiceRequestDetails(IOrganizationService service, Guid serviceRequestId)
        {
            try
            {
                if (service != null)
                {
                    return service.Retrieve("incident", serviceRequestId, new ColumnSet(true));
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Service request fetch failed", ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        /// Logs the not all attributes populated.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The object serviceRequest.</param>
        public static void SetProviderCriteriaMissingFields(IOrganizationService service, Entity serviceRequest)
        {
            try
            {
                if (service != null && serviceRequest != null)
                {
                    string message = "Provider was not set.";
                    StringBuilder parameters = new StringBuilder();

                    if (!serviceRequest.Attributes.Contains(CRMAttributesResource.ProblemBuildingIdAttribute))
                    {
                        parameters.Append("Building, ");
                    }

                    if (!serviceRequest.Attributes.Contains(CRMAttributesResource.ProblemRoomTypeIdAttribute))
                    {
                        parameters.Append("Problem Room, ");
                    }

                    if (!serviceRequest.Attributes.Contains(CRMAttributesResource.ProblemClassIdAttribute))
                    {
                        parameters.Append("Problem Class, ");
                    }

                    if (!serviceRequest.Attributes.Contains(CRMAttributesResource.ProblemTypeIdAttribute))
                    {
                        parameters.Append("Problem Type, ");
                    }

                    if (!serviceRequest.Attributes.Contains(CRMAttributesResource.CostCenterCode))
                    {
                        parameters.Append("Cost Center Code, ");
                    }

                    if (!serviceRequest.Attributes.Contains(CRMAttributesResource.IoCode))
                    {
                        parameters.Append("Io Code, ");
                    }

                    if (string.IsNullOrWhiteSpace(parameters.ToString().Trim()))
                    {
                        Logger.Write(message, ExceptionType.SetProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, "Combination of Building, RoomType, Class and TypeId for setting provider was incorrect");
                    }
                    else
                    {
                        Logger.Write(message, ExceptionType.SetProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, parameters.ToString().TrimEnd(',') + " is missing.");
                    }
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Setting provider details failed.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Determines whether [is priority set for integration] [the specified current status code].
        /// </summary>
        /// <param name="currentStatusCode">The current status code.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The service request.</param>
        /// <returns>
        ///   <c>true</c> if [is priority set for integration] [the specified current status code]; otherwise, <c>false</c>.
        /// </returns>
        /// IsPrioritySetForIntegrationReclassify
        public static bool IsPrioritySetForIntegration(string currentStatusCode, IOrganizationService service, Entity serviceRequest, ITracingService trace, EntityCollection collection, int originCode, Guid providerTeamId)
        {
            bool isPrioritySetForIntegration = true;
            try
            {
                if (serviceRequest != null)
                {
                    if (currentStatusCode.ToUpperInvariant().Equals("DRAFT"))
                    {
                        return isPrioritySetForIntegration;
                    }

                    if (serviceRequest.Attributes.Contains("smp_createdfrom") && ((OptionSetValue)serviceRequest["smp_createdfrom"]).Value == 3)
                    {
                        return isPrioritySetForIntegration;
                    }

                    trace.Trace("is smp_priorityid ?" + serviceRequest.Attributes.Contains("smp_priorityid").ToString());
                    string priority = serviceRequest.Attributes.Contains("smp_priorityid") ? ((EntityReference)serviceRequest["smp_priorityid"]).Name : string.Empty;
                    bool isIntegrationToBeDone = serviceRequest.Attributes.Contains("smp_integrationstatus") ? (bool)serviceRequest["smp_integrationstatus"] : false;
                    bool isWebRequest = false;
                    if (serviceRequest.Attributes.Contains("caseorigincode"))
                    {
                        trace.Trace("originCode :" + originCode.ToString());
                        if (originCode == 3 || originCode == 180620005 || originCode == 100008999)
                        {
                            isWebRequest = true;
                        }
                    }

                    //// Get all priorities for 8/8 Mailbox
                    string priorities = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.RoutingPriorities8By8)).First()["smp_value"].ToString();
                    //// if priority is (P1 or P2) and isIntegration is false - no integration will happen
                    if (isWebRequest == true && !isIntegrationToBeDone && (string.IsNullOrWhiteSpace(priority) || priorities.Contains(priority.ToUpperInvariant())))
                    {
                        trace.Trace("In pending CSR");
                        Entity contact = ServiceRequestHelper.GetContactDetails(service, ((EntityReference)serviceRequest.Attributes["smp_contact"]).Id);
                        bool contactIsProvider = contact.Attributes.Contains("smp_allowemailnotification") ? (bool)contact["smp_allowemailnotification"] : false;
                        trace.Trace("Status Before PCSRDISP :" + currentStatusCode.ToString());
                        if (contactIsProvider == false)
                        {
                            trace.Trace("Contact is not a Provider");
                            ServiceRequestHelper.SetServiceRequestStatus(service, serviceRequest, currentStatusCode, "PendingDispatchStatusCode", originCode, collection, providerTeamId);
                            string fromUserDomainName = collection.Entities.Where(x => x["smp_title"].ToString().Equals(CRMAttributesResource.EmailSenderDomainName)).First()["smp_value"].ToString();
                            Guid senderId = ServiceRequestHelper.GetCrmUserId(service, fromUserDomainName);
                            SendEmailto8X8MailBox(service, serviceRequest, senderId, contact, collection);
                            isPrioritySetForIntegration = false;
                            trace.Trace("Email is sent to 8*8");
                        }
                        else
                        {
                            isPrioritySetForIntegration = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while checking priority and integration status", ex);
                throw customEx;
            }

            return isPrioritySetForIntegration;
        }

        public static void SetIntegrationStatusandProblemOccuredDateTime(Entity serviceRequest, bool status)
        {
            try
            {
                if (serviceRequest != null)
                {
                    serviceRequest.Attributes["smp_integrationstatus"] = status;
                    DateTime problemoccureddate = Convert.ToDateTime(DateTime.UtcNow, CultureInfo.CurrentCulture);
                    serviceRequest["smp_problemoccureddatetime"] = Convert.ToDateTime(problemoccureddate, CultureInfo.CurrentCulture);
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error occurred while updating integration status", ex);
                throw customEx;
            }
        }
        ////code change end

        /// <summary>
        /// Calls the integration service.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="providerServiceRequest">The provider service request.</param>
        /// <param name="timeOut">configured timeout value for WCF operation</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <returns>
        /// Returns true if call is successful
        /// </returns>
        public static bool CallIntegrationService(Provider provider, ServiceRequest providerServiceRequest, int timeOut, IOrganizationService service, Guid serviceRequestId, bool isDebug, ITracingService trace)
        {
            trace.Trace("In Call integration service Method");
            bool isDataUpdated = false;
            WSHttpBinding binding = null;
            EndpointAddress address = null;
            string iD = string.Empty;
            string key = string.Empty;
            if (provider != null && providerServiceRequest != null)
            {
                try
                {
                    DateTime endTime = DateTime.Now.AddSeconds(timeOut);
                    ClientBinding(provider, endTime, ref binding, ref address);
                    if (provider.HostedOnAzure)
                    {
                        CmmsServiceRequestManagerAADClient cmmsServiceClient = null;
                        trace.Trace("Hosted on Azure is True");
                        Entity collection = GetProviderIntegrationCollection(service);
                        if (collection != null)
                        {
                            iD = collection["smp_clientid"].ToString();
                            key = collection["smp_serviceprincipalid"].ToString();
                            trace.Trace("Client ID :{0} and Secret Key :{1}", iD, key);
                        }

                        string aadtoken = GenerateAzureToken(service, trace, iD, key);
                        trace.Trace("Token for Outbound :" + aadtoken);
                        /// CMMS Service Will be called Thrice
                        for (int callServiceCount = 0; callServiceCount < 3; callServiceCount++)
                        {
                            try
                            {
                                if (cmmsServiceClient == null)
                                {
                                    cmmsServiceClient = new CmmsServiceRequestManagerAADClient(binding, address);
                                }

                                if (!string.IsNullOrEmpty(aadtoken))
                                {
                                    var addressBuildier = new EndpointAddressBuilder(cmmsServiceClient.Endpoint.Address);
                                    addressBuildier.Headers.Add(AddressHeader.CreateAddressHeader("Authorization", cmmsServiceClient.Endpoint.Contract.Namespace, "Bearer " + aadtoken));
                                    cmmsServiceClient.Endpoint.Address = addressBuildier.ToEndpointAddress();
                                    isDataUpdated = cmmsServiceClient.UpdateServiceRequestDetails(providerServiceRequest);
                                }

                                if (isDebug == true && isDataUpdated == false)
                                {
                                    Logger.Write("Provider sent back failed response.", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequestId, "Call Service Count=" + Convert.ToString(callServiceCount, CultureInfo.InvariantCulture));
                                }
                                else if (isDataUpdated)
                                {
                                    break;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                if (isDebug == true)
                                {
                                    Logger.Write(ex, ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequestId, "Call Service Count=" + Convert.ToString(callServiceCount, CultureInfo.InvariantCulture));
                                }

                                if (callServiceCount > 2)
                                {
                                    CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while calling service", ex);
                                    throw customEx;
                                }
                            }
                        }
                    }
                    ////else
                    ////{
                    ////    trace.Trace("Not Hosted on Azure");
                    ////    CmmsServiceRequestManagerClient cmmsServiceClient = null;
                    ////    /// CMMS Service Will be called Thrice
                    ////    for (int callServiceCount = 0; callServiceCount < 3; callServiceCount++)
                    ////    {
                    ////        try
                    ////        {
                    ////            if (cmmsServiceClient == null)
                    ////            {
                    ////                cmmsServiceClient = new CmmsServiceRequestManagerClient(binding, address);
                    ////            }

                    ////            if (!string.IsNullOrEmpty(provider.UserId) && !string.IsNullOrEmpty(provider.Password))
                    ////            {
                    ////                isDataUpdated = cmmsServiceClient.UpdateServiceRequestDetails(provider.UserId, provider.Password, providerServiceRequest);
                    ////            }
                    ////            else
                    ////            {
                    ////                Logger.Write("Provider details not updated, please update provider's service url, username and password.", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequestId, string.Empty);
                    ////                break;
                    ////            }

                    ////            trace.Trace("Is Data sent :" + isDataUpdated.ToString().ToUpper());
                    ////            if (isDebug == true && isDataUpdated == false)
                    ////            {
                    ////                Logger.Write("Provider sent back failed response.", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequestId, "Call Service Count=" + Convert.ToString(callServiceCount, CultureInfo.InvariantCulture));
                    ////            }
                    ////            else if (isDataUpdated)
                    ////            {
                    ////                break;
                    ////            }
                    ////        }
                    ////        catch (Exception ex)
                    ////        {
                    ////            if (isDebug == true)
                    ////            {
                    ////                Logger.Write(ex, ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequestId, "Call Service Count=" + Convert.ToString(callServiceCount, CultureInfo.InvariantCulture));
                    ////            }

                    ////            if (callServiceCount > 2)
                    ////            {
                    ////                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while calling service", ex);
                    ////                throw customEx;
                    ////            }
                    ////        }
                    ////    }
                    ////}
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    address = null;
                    binding = null;
                }
            }

            return isDataUpdated;
        }

        /// <summary>
        /// Sets the service request object.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The service request.</param>
        /// <returns>The ServiceRequest.</returns>
        public static ServiceRequest SetServiceRequestObject(IOrganizationService service, Entity serviceRequest, int source)
        {
            try
            {
                if (service != null && serviceRequest != null)
                {
                    ServiceRequest providerServiceRequest = new ServiceRequest();

                    if (serviceRequest.Attributes.Contains("smp_requestedduedate"))
                    {
                        providerServiceRequest.RequestedDueDate = (DateTime)serviceRequest.Attributes["smp_requestedduedate"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_submitteddatetime"))
                    {
                        providerServiceRequest.SubmittedDateTime = (DateTime)serviceRequest.Attributes["smp_submitteddatetime"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_submitteddatetimebybuildingtimezone"))
                    {
                        providerServiceRequest.SubmittedDateTimeByBuildingTimeZone = (string)serviceRequest.Attributes["smp_submitteddatetimebybuildingtimezone"];
                    }

                    if (serviceRequest.Attributes.Contains("createdon"))
                    {
                        providerServiceRequest.CreatedDateTime = (DateTime)serviceRequest.Attributes["createdon"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_createddatetimebybuildingtimezone"))
                    {
                        providerServiceRequest.CreatedDateTimeByBuildingTimeZone = (string)serviceRequest.Attributes["smp_createddatetimebybuildingtimezone"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_duedate"))
                    {
                        providerServiceRequest.DueDate = (DateTime)serviceRequest.Attributes["smp_duedate"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_duedatebybuildingtimezone"))
                    {
                        providerServiceRequest.DueDateByBuildingTimeZone = (string)serviceRequest.Attributes["smp_duedatebybuildingtimezone"];
                    }

                    if (serviceRequest.Attributes.Contains("title"))
                    {
                        providerServiceRequest.Title = (string)serviceRequest.Attributes["title"];
                    }

                    if (serviceRequest.Attributes.Contains("ticketnumber"))
                    {
                        providerServiceRequest.ServiceRequestNumber = (string)serviceRequest.Attributes["ticketnumber"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_iocode"))
                    {
                        providerServiceRequest.IoCode = (string)serviceRequest.Attributes["smp_iocode"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_problembuilding"))
                    {
                        providerServiceRequest.BuildingId = GetBuildingFeedstoreId(service, ((EntityReference)serviceRequest.Attributes["smp_problembuilding"]).Id);
                    }

                    Entity contactEntity = GetContactDetails(service, ((EntityReference)serviceRequest.Attributes["smp_contact"]).Id);
                    providerServiceRequest.ContactPreferredLanguage = contactEntity != null && contactEntity.Attributes.Contains("smp_preferredlanguage") ? contactEntity["smp_preferredlanguage"].ToString() : "en-us";
                    if (serviceRequest.Attributes.Contains("smp_tier1workcompletiondatebybuildingtimezone"))
                    {
                        providerServiceRequest.ProviderWorkCompletionDateByBuildingTimeZone = (string)serviceRequest.Attributes["smp_tier1workcompletiondatebybuildingtimezone"];
                    }

                    if (serviceRequest.Attributes.Contains("smp_providerduedatebybuildingtimezone"))
                    {
                        providerServiceRequest.ProviderDueDateByBuildingTimeZone = (string)serviceRequest.Attributes["smp_providerduedatebybuildingtimezone"];
                    }

                    if (source == 0)
                    {
                        providerServiceRequest.NotesCollection = GetServiceRequestNotes(service, serviceRequest.Id);
                    }

                    providerServiceRequest.DynamicProblemTypeNotesCollection = GetDynamicProblemTypeNotesCollection(service, serviceRequest.Id);

                    SetContactFields(providerServiceRequest, serviceRequest, service, contactEntity);

                    SetProblemFields(providerServiceRequest, serviceRequest);

                    SetRequestorFields(providerServiceRequest, serviceRequest, service);

                    SetLookupNames(service, providerServiceRequest, serviceRequest);

                    SetOptionSetText(service, providerServiceRequest, serviceRequest);

                    return providerServiceRequest;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// Sets the integration status.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <param name="status">if set to <c>true</c> [status].</param>
        public static void SetIntegrationStatus(Entity serviceRequest, bool status)
        {
            try
            {
                serviceRequest.Attributes["smp_integrationstatus"] = status;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while setting integration Status", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets user id for user by domain name
        /// </summary>
        /// <param name="service">service instance</param>
        /// <param name="domainName">domain name of user</param>
        /// <returns>id of user</returns>
        public static Guid GetCrmUserId(IOrganizationService service, string domainName)
        {
            Guid userId = Guid.Empty;
            try
            {
                EntityCollection entityCollection = GetQueryResponse(service, "systemuser", new string[] { "systemuserid" }, "domainname", domainName);

                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    Entity entity = entityCollection.Entities[0];
                    if (entity.Attributes.Contains("systemuserid"))
                    {
                        userId = entity.Id;
                    }
                }

                return userId;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching guid  for user {0} failed", domainName), ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Get translated status value
        /// </summary>
        /// <param name="service">service object</param>
        /// <param name="status">status code value</param>
        /// <param name="language">language to translate</param>
        /// <returns>returns translated value</returns>
        public static string GetTranslatedStatus(IOrganizationService service, int status, string language)
        {
            string translatedStatus = string.Empty;
            try
            {
                switch (language)
                {
                    case "pt-pt":
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext_pt_pt", "smp_statusreason", status);
                        break;
                    case "es-mx":
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext_es_mx", "smp_statusreason", status);
                        break;
                    case "fr-fr":
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext_fr_fr", "smp_statusreason", status);
                        break;
                    case "zh-cn":
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext_zh_cn", "smp_statusreason", status);
                        break;
                    case "ja-jp":
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext_ja_jp", "smp_statusreason", status);
                        break;
                    case "ko-kr":
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext_ko_kr", "smp_statusreason", status);
                        break;
                    case "en-us":
                    default:
                        translatedStatus = GetFetchedValue(service, "smp_statusreasontranslation", "smp_statusreasontext", "smp_statusreason", status);
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return translatedStatus;
        }

        /// <summary>
        /// Get translated problem class
        /// </summary>
        /// <param name="service">service object</param>
        /// <param name="problemClass">Problem class</param>
        /// <param name="language">language to translate</param>
        /// <returns>returns translated value</returns>
        public static string GetTranslatedProblemClass(IOrganizationService service, string problemClass, string language)
        {
            string translatedProblemClass = problemClass;
            try
            {
                switch (language)
                {
                    case "pt-pt":
                        translatedProblemClass = GetFetchedValue(service, "smp_problemclass", "smp_problemclassname_pt_pt", "smp_problemclassname", problemClass);
                        break;
                    case "es-mx":
                        translatedProblemClass = GetFetchedValue(service, "smp_problemclass", "smp_problemclassname_es_mx", "smp_problemclassname", problemClass);
                        break;
                    case "fr-fr":
                        translatedProblemClass = GetFetchedValue(service, "smp_problemclass", "smp_problemclassname_fr_fr", "smp_problemclassname", problemClass);
                        break;
                    case "zh-cn":
                        translatedProblemClass = GetFetchedValue(service, "smp_problemclass", "smp_problemclassname_zh_cn", "smp_problemclassname", problemClass);
                        break;
                    case "ja-jp":
                        translatedProblemClass = GetFetchedValue(service, "smp_problemclass", "smp_problemclassname_ja_jp", "smp_problemclassname", problemClass);
                        break;
                    case "ko-kr":
                        translatedProblemClass = GetFetchedValue(service, "smp_problemclass", "smp_problemclassname_ko_kr", "smp_problemclassname", problemClass);
                        break;
                    case "en-us":
                    default:
                        translatedProblemClass = problemClass;
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return translatedProblemClass;
        }

        /// <summary>
        /// Get translated problem types
        /// </summary>
        /// <param name="service">service object</param>
        /// <param name="problemType">Problem type</param>
        /// <param name="language">language to translate</param>
        /// <returns>returns translated value</returns>
        public static string GetTranslatedProblemTypes(IOrganizationService service, string problemType, string language)
        {
            string translatedProblemTypes = problemType;
            try
            {
                switch (language)
                {
                    case "pt-pt":
                        translatedProblemTypes = GetFetchedValue(service, "smp_problemtype", "smp_problemtypename_pt_pt", "smp_problemtypename", problemType);
                        break;
                    case "es-mx":
                        translatedProblemTypes = GetFetchedValue(service, "smp_problemtype", "smp_problemtypename_es_mx", "smp_problemtypename", problemType);
                        break;
                    case "fr-fr":
                        translatedProblemTypes = GetFetchedValue(service, "smp_problemtype", "smp_problemtypename_fr_fr", "smp_problemtypename", problemType);
                        break;
                    case "zh-cn":
                        translatedProblemTypes = GetFetchedValue(service, "smp_problemtype", "smp_problemtypename_zh_cn", "smp_problemtypename", problemType);
                        break;
                    case "ja-jp":
                        translatedProblemTypes = GetFetchedValue(service, "smp_problemtype", "smp_problemtypename_ja_jp", "smp_problemtypename", problemType);
                        break;
                    case "ko-kr":
                        translatedProblemTypes = GetFetchedValue(service, "smp_problemtype", "smp_problemtypename_ko_kr", "smp_problemtypename", problemType);
                        break;
                    case "en-us":
                    default:
                        translatedProblemTypes = problemType;
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return translatedProblemTypes;
        }

        /// <summary>
        /// Get translated room types
        /// </summary>
        /// <param name="service">service object</param>
        /// <param name="roomType">Room type</param>
        /// <param name="language">language to translate</param>
        /// <returns>returns translated value</returns>
        public static string GetTranslatedRoomTypes(IOrganizationService service, string roomType, string language)
        {
            string translatedRoomTypes = roomType;
            try
            {
                switch (language)
                {
                    case "pt-pt":
                        translatedRoomTypes = GetFetchedValue(service, "smp_roomtype", "smp_roomtype_pt_pt", "smp_roomtype", roomType);
                        break;
                    case "es-mx":
                        translatedRoomTypes = GetFetchedValue(service, "smp_roomtype", "smp_roomtype_es_mx", "smp_roomtype", roomType);
                        break;
                    case "fr-fr":
                        translatedRoomTypes = GetFetchedValue(service, "smp_roomtype", "smp_roomtype_fr_fr", "smp_roomtype", roomType);
                        break;
                    case "zh-cn":
                        translatedRoomTypes = GetFetchedValue(service, "smp_roomtype", "smp_roomtype_zh_cn", "smp_roomtype", roomType);
                        break;
                    case "ja-jp":
                        translatedRoomTypes = GetFetchedValue(service, "smp_roomtype", "smp_roomtype_ja_jp", "smp_roomtype", roomType);
                        break;
                    case "ko-kr":
                        translatedRoomTypes = GetFetchedValue(service, "smp_roomtype", "smp_roomtype_ko_kr", "smp_roomtype", roomType);
                        break;
                    case "en-us":
                    default:
                        translatedRoomTypes = roomType;
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return translatedRoomTypes;
        }

        /// <summary>
        /// Gets requestor entity
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="requestorId">id of requestor</param>
        /// <returns>returns requestor entity</returns>
        public static Entity GetContactDetails(IOrganizationService service, Guid contactId)
        {
            try
            {
                if (service != null)
                {
                    return service.Retrieve("contact", contactId, new ColumnSet("smp_preferredlanguage", "smp_isprovideruser", "smp_costcenter", "smp_allowemailnotification", "smp_alias", "firstname", "lastname", "emailaddress1", "smp_staffingresourcetype"));
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Contact fetch failed", ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        /// Gets the building details.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="buildingId">The building id.</param>
        /// <param name="columns">Columns to be fetched</param>
        /// <returns>The building entity details.</returns>
        public static EntityCollection GetBuildingDetails(IOrganizationService service, Guid buildingId, string[] columns)
        {
            EntityCollection entities = new EntityCollection();
            try
            {
                if (service != null)
                {
                    QueryExpression query = new QueryExpression()
                    {
                        EntityName = "smp_weekdays",
                        ColumnSet = new ColumnSet(true),
                        LinkEntities =
                        {
                        new LinkEntity
                        {
                        Columns = new ColumnSet(true),
                        LinkFromEntityName = "smp_weekdays",
                        LinkFromAttributeName = "smp_weekdaysid",
                        LinkToEntityName = "smp_smp_weekdays_smp_building",
                        LinkToAttributeName = "smp_weekdaysid",
                        LinkCriteria = new FilterExpression
                        {
                        FilterOperator = LogicalOperator.And,
                        Conditions =
                        {
                                    new ConditionExpression
                                            {
                                            AttributeName = "smp_buildingid",
                                            Operator = ConditionOperator.Equal,
                                            Values = { buildingId }
                                            }
                        }
                        }
                        }
                        }
                    };
                    entities = service.RetrieveMultiple(query);
                    return entities;
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Building fetch failed", ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        ///  Gets the Problem building Allow Email Notification
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="problemBuildingId">The problem building Id</param>
        /// <returns>Allow notification value</returns>       
        public static bool GetBuildingEmailNotificationStatus(IOrganizationService service, Guid problemBuildingId)
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

        /// <summary>
        /// Gets the name field of the lookup from the parent entity based on lookup id.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="lookupId">The lookup id.</param>
        /// <param name="lookupFieldName">Name of the lookup field.</param>
        /// <param name="lookupEntityName">Name of the lookup entity.</param>
        /// <returns>Returns the lookup Name</returns>
        public static string GetLookupName(IOrganizationService service, Guid lookupId, string lookupFieldName, string lookupEntityName)
        {
            try
            {
                if (service != null)
                {
                    Entity entity = service.Retrieve(lookupEntityName, lookupId, new ColumnSet(new string[] { lookupFieldName }));
                    if (entity.Attributes.Contains(lookupFieldName))
                    {
                        return (string)entity.Attributes[lookupFieldName];
                    }
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching lookup name for fieldname {0} and entity {1} failed.", lookupFieldName, lookupEntityName), ex);
                throw customEx;
            }

            return null;
        }

        /// <summary>
        /// Gets buildings feed store id
        /// </summary>
        /// <param name="service">The service</param>
        /// <param name="buildingId">id of building</param>
        /// <returns>returns building feed store id</returns>
        private static string GetBuildingFeedstoreId(IOrganizationService service, Guid buildingId)
        {
            try
            {
                string feedstoreId = string.Empty;
                Entity result = service.Retrieve("smp_building", buildingId, new ColumnSet("smp_feedstoreid"));
                return feedstoreId = result != null ? result["smp_feedstoreid"].ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching feed store id  for building {0} failed.", buildingId.ToString()), ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Sets the contact in provider service request object
        /// </summary>
        /// <param name="providerServiceRequest">the provider service request object</param>
        /// <param name="serviceRequest">the service request entity.</param>
        /// <param name="service">the organization service.</param>
        private static void SetContactFields(ServiceRequest providerServiceRequest, Entity serviceRequest, IOrganizationService service, Entity contact)
        {
            if (serviceRequest != null && providerServiceRequest != null)
            {
                if (contact.Attributes.Contains("smp_alias"))
                {
                    providerServiceRequest.ContactAlias = contact["smp_alias"].ToString();
                }

                if (contact.Attributes.Contains("emailaddress1"))
                {
                    providerServiceRequest.ContactEmail = contact["emailaddress1"].ToString();
                }

                if (contact.Attributes.Contains("firstname"))
                {
                    providerServiceRequest.ContactFirstName = contact["firstname"].ToString();
                }

                if (contact.Attributes.Contains("lastname"))
                {
                    providerServiceRequest.ContactLastName = contact["lastname"].ToString();
                }

                if (serviceRequest.Attributes.Contains("smp_contactphone"))
                {
                    providerServiceRequest.ContactPhoneNumber = (string)serviceRequest.Attributes["smp_contactphone"];
                }

                if (serviceRequest.Attributes.Contains("smp_contactroom"))
                {
                    providerServiceRequest.ContactRoomNumber = (string)serviceRequest.Attributes["smp_contactroom"];
                }

                if (serviceRequest.Attributes.Contains("smp_contactroomtype"))
                {
                    providerServiceRequest.ContactRoomType = (string)serviceRequest.Attributes["smp_contactroomtype"];
                }

                if (serviceRequest.Attributes.Contains("smp_contact"))
                {
                    providerServiceRequest.IsProviderUser = contact != null ? (bool)contact["smp_isprovideruser"] : false;
                    providerServiceRequest.CostCenterCode = contact.Attributes.Contains("smp_costcenter") ? (string)contact["smp_costcenter"] : string.Empty;
                }
            }
        }

        /// <summary>
        /// Sets the problem in provider service request object
        /// </summary>
        /// <param name="providerServiceRequest">the provider service request object</param>
        /// <param name="serviceRequest">the service request entity.</param>
        private static void SetProblemFields(ServiceRequest providerServiceRequest, Entity serviceRequest)
        {
            if (serviceRequest != null && providerServiceRequest != null)
            {
                if (serviceRequest.Attributes.Contains("smp_problembuildingaddressline1"))
                {
                    providerServiceRequest.ProblemBuildingAddressLine1 = (string)serviceRequest.Attributes["smp_problembuildingaddressline1"];
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingaddressline2"))
                {
                    providerServiceRequest.ProblemBuildingAddressLine2 = (string)serviceRequest.Attributes["smp_problembuildingaddressline2"];
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingcity"))
                {
                    providerServiceRequest.ProblemBuildingCity = (string)serviceRequest.Attributes["smp_problembuildingcity"];
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingcountry"))
                {
                    providerServiceRequest.ProblemBuildingCountry = (string)serviceRequest.Attributes["smp_problembuildingcountry"];
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingstate"))
                {
                    providerServiceRequest.ProblemBuildingState = (string)serviceRequest.Attributes["smp_problembuildingstate"];
                }

                if (serviceRequest.Attributes.Contains("smp_problembuildingzipcode"))
                {
                    providerServiceRequest.ProblemBuildingZipCode = (string)serviceRequest.Attributes["smp_problembuildingzipcode"];
                }

                if (serviceRequest.Attributes.Contains("smp_problemcode"))
                {
                    providerServiceRequest.ProblemCode = (string)serviceRequest.Attributes["smp_problemcode"];
                }

                if (serviceRequest.Attributes.Contains("smp_problemoccureddatetime"))
                {
                    providerServiceRequest.ProblemOccurredDateTime = (DateTime)serviceRequest.Attributes["smp_problemoccureddatetime"];
                }

                if (serviceRequest.Attributes.Contains("smp_occureddatetimebybuildingtimezone"))
                {
                    providerServiceRequest.ProblemOccurredDateTimeByBuildingTimeZone = (string)serviceRequest.Attributes["smp_occureddatetimebybuildingtimezone"];
                }

                if (serviceRequest.Attributes.Contains("new_problemroomnumber"))
                {
                    ///Code Changes By Mihika G on 23-11-2017
                    string problemRoomLookupName = ((EntityReference)serviceRequest["new_problemroomnumber"]).Name;
                    if (problemRoomLookupName == "Other")
                    {
                        problemRoomLookupName = serviceRequest.Attributes.Contains("smp_problemroom") ? serviceRequest.Attributes["smp_problemroom"].ToString() : string.Empty;
                    }

                    providerServiceRequest.ProblemRoomNumber = problemRoomLookupName;
                    /// End of Code Changes By Mihika G
                }

                if (serviceRequest.Attributes.Contains("smp_problemroom"))
                {
                    providerServiceRequest.ProblemRoomNumber = serviceRequest.Attributes["smp_problemroom"].ToString();
                }

                if (serviceRequest.Attributes.Contains("smp_problemtypedescription"))
                {
                    providerServiceRequest.ProblemTypeDescription = (string)serviceRequest.Attributes["smp_problemtypedescription"];
                }

                if (serviceRequest.Attributes.Contains("smp_problemroomzone"))
                {
                    providerServiceRequest.ProblemZone = (string)serviceRequest.Attributes["smp_problemroomzone"];
                }

                if (serviceRequest.Attributes.Contains("smp_problemclassid"))
                {
                    providerServiceRequest.ProblemClassId = ((EntityReference)serviceRequest.Attributes["smp_problemclassid"]).Id.ToString();
                }

                if (serviceRequest.Attributes.Contains("smp_problemtypeid"))
                {
                    providerServiceRequest.ProblemTypeId = ((EntityReference)serviceRequest.Attributes["smp_problemtypeid"]).Id.ToString();
                }
            }
        }

        /// <summary>
        /// Sets the requestor in provider service request object
        /// </summary>
        /// <param name="providerServiceRequest">the provider service request object</param>
        /// <param name="serviceRequest">the service request entity.</param>
        /// <param name="service">the organization service.</param>
        private static void SetRequestorFields(ServiceRequest providerServiceRequest, Entity serviceRequest, IOrganizationService service)
        {
            if (serviceRequest != null && providerServiceRequest != null)
            {
                if (serviceRequest.Attributes.Contains("smp_requestoralias"))
                {
                    providerServiceRequest.RequestorAlias = (string)serviceRequest.Attributes["smp_requestoralias"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestoremail"))
                {
                    providerServiceRequest.RequestorEmail = (string)serviceRequest.Attributes["smp_requestoremail"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestorfirstname"))
                {
                    providerServiceRequest.RequestorFirstName = (string)serviceRequest.Attributes["smp_requestorfirstname"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestorlastname"))
                {
                    providerServiceRequest.RequestorLastName = (string)serviceRequest.Attributes["smp_requestorlastname"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestorphone"))
                {
                    providerServiceRequest.RequestorPhoneNumber = (string)serviceRequest.Attributes["smp_requestorphone"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestorroomno"))
                {
                    providerServiceRequest.RequestorRoom = (string)serviceRequest.Attributes["smp_requestorroomno"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestorroomtype"))
                {
                    providerServiceRequest.RequestorRoomType = (string)serviceRequest.Attributes["smp_requestorroomtype"];
                }

                if (serviceRequest.Attributes.Contains("smp_requestorid"))
                {
                    EntityReference requestor = (EntityReference)serviceRequest.Attributes["smp_requestorid"];
                    providerServiceRequest.RequestorFirstName = requestor.Name;
                }
            }
        }

        /// <summary>
        /// Sets the lookup name in provider service request object
        /// </summary>
        /// <param name="service">the service object.</param>
        /// <param name="providerServiceRequest">the provider service request object</param>
        /// <param name="serviceRequest">the service request entity.</param>
        private static void SetLookupNames(IOrganizationService service, ServiceRequest providerServiceRequest, Entity serviceRequest)
        {
            if (serviceRequest != null && providerServiceRequest != null)
            {
                if (serviceRequest.Attributes.Contains("smp_contactbuilding"))
                {
                    providerServiceRequest.ContactBuildingName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_contactbuilding"]).Id, "smp_buildingname", "smp_building");
                }

                if (serviceRequest.Attributes.Contains("smp_priorityid"))
                {
                    providerServiceRequest.Priority = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_priorityid"]).Id, "smp_name", "smp_priority");
                }

                if (serviceRequest.Attributes.Contains("smp_problembuilding"))
                {
                    providerServiceRequest.ProblemBuildingName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_problembuilding"]).Id, "smp_buildingname", "smp_building");
                }

                if (serviceRequest.Attributes.Contains("smp_problemclassid"))
                {
                    providerServiceRequest.ProblemClassName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_problemclassid"]).Id, "smp_problemclassname", "smp_problemclass");
                }

                if (serviceRequest.Attributes.Contains("smp_problemfloor"))
                {
                    providerServiceRequest.ProblemFloorName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_problemfloor"]).Id, "smp_name", "smp_floor");
                }

                if (serviceRequest.Attributes.Contains("smp_problemroomtype"))
                {
                    providerServiceRequest.ProblemRoomTypeName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_problemroomtype"]).Id, "smp_roomtype", "smp_roomtype");
                }

                if (serviceRequest.Attributes.Contains("smp_problemtypeid"))
                {
                    providerServiceRequest.ProblemTypeName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_problemtypeid"]).Id, "smp_problemtypename", "smp_problemtype");
                }

                if (serviceRequest.Attributes.Contains("customerid"))
                {
                    providerServiceRequest.ProviderName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["customerid"]).Id, "name", "account");
                }

                if (serviceRequest.Attributes.Contains("smp_referencesr"))
                {
                    providerServiceRequest.ReferenceServiceRequestNumber = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_referencesr"]).Id, "ticketnumber", "incident");
                }

                if (serviceRequest.Attributes.Contains("smp_buildingid"))
                {
                    providerServiceRequest.RequestorBuildingName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)serviceRequest.Attributes["smp_buildingid"]).Id, "smp_buildingname", "smp_building");
                }
            }
        }

        /// <summary>
        /// Sets the option set text to service request object
        /// </summary>
        /// <param name="service">the service object</param>
        /// <param name="providerServiceRequest">the provider service request object</param>
        /// <param name="serviceRequest">the service request entity</param>
        private static void SetOptionSetText(IOrganizationService service, ServiceRequest providerServiceRequest, Entity serviceRequest)
        {
            if (serviceRequest != null && providerServiceRequest != null)
            {
                if (serviceRequest.Attributes.Contains("smp_priorityoverridereason"))
                {
                    providerServiceRequest.PriorityOverrideReason = ServiceRequestHelper.GetOptionsSetTextForValue(service, "incident", "smp_priorityoverridereason", ((OptionSetValue)serviceRequest.Attributes["smp_priorityoverridereason"]).Value);
                }

                ////if (serviceRequest.Attributes.Contains("smp_contactzone"))
                ////{
                ////    providerServiceRequest.ContactZone = ServiceRequestHelper.GetOptionsSetTextForValue(service, "incident", "smp_contactzone", ((OptionSetValue)serviceRequest.Attributes["smp_contactzone"]).Value);
                ////}

                ////if (serviceRequest.Attributes.Contains("smp_requestorzone"))
                ////{
                ////    providerServiceRequest.RequestorZone = ServiceRequestHelper.GetOptionsSetTextForValue(service, "incident", "smp_requestorzone", ((OptionSetValue)serviceRequest.Attributes["smp_requestorzone"]).Value);
                ////}

                ////if (serviceRequest.Attributes.Contains("smp_buildingtimezone"))
                ////{
                ////    providerServiceRequest.BuildingTimeZone = ServiceRequestHelper.GetOptionsSetTextForValue(service, "incident", "smp_buildingtimezone", ((OptionSetValue)serviceRequest.Attributes["smp_buildingtimezone"]).Value);
                ////}

                if (serviceRequest.Attributes.Contains("smp_servicerequestdevicetype"))
                {
                    providerServiceRequest.ServiceRequestDeviceType = ServiceRequestHelper.GetOptionsSetTextForValue(service, "incident", "smp_servicerequestdevicetype", ((OptionSetValue)serviceRequest.Attributes["smp_servicerequestdevicetype"]).Value);
                }

                ////if (serviceRequest.Attributes.Contains("smp_servicerequestorigination"))
                ////{
                ////    providerServiceRequest.ServiceRequestOrigination = ServiceRequestHelper.GetOptionsSetTextForValue(service, "incident", "smp_servicerequestorigination", ((OptionSetValue)serviceRequest.Attributes["smp_servicerequestorigination"]).Value);
                ////}

                if (serviceRequest.Attributes.Contains("statuscode"))
                {
                    providerServiceRequest.StatusCode = ServiceRequestHelper.GetStatusCodeFromValue(service, ((OptionSetValue)serviceRequest.Attributes["statuscode"]).Value);
                }
            }
        }

        /// <summary>
        /// Send an email to 8x8 mailbox if priority is P1 or P2
        /// </summary>
        /// <param name="service">the service object.</param>
        /// <param name="serviceRequest">the service request object.</param>
        /// <param name="userId">The userId</param>
        private static void SendEmailto8X8MailBox(IOrganizationService service, Entity serviceRequest, Guid userId, Entity contact, EntityCollection collection)
        {
            try
            {
                string contactLanguageCode = contact.Attributes.Contains("smp_preferredlanguage") ? contact["smp_preferredlanguage"].ToString() : "en-us";
                /// 8x8 mail Box Email Address
                Guid eightbyEightId = GetLinkedQueryResponse(service, contactLanguageCode);
                if (eightbyEightId != Guid.Empty)
                {
                    int fromEmail = 1;
                    ServiceRequest providerServiceRequest = SetServiceRequestObject(service, serviceRequest, fromEmail);
                    providerServiceRequest.StatusCode = "PCSRDISP";
                    EmailHelper.SendEmail(service, providerServiceRequest, serviceRequest.Id, "systemuser", userId, "smp_8x8mailbox", eightbyEightId, collection.Entities.Where(x => x["smp_title"].ToString().Equals("ServiceRequest8x8MailboxEmailTemplateName")).First()["smp_value"].ToString());
                }
                else
                {
                    Logger.Write("No 8X8 Mailbox found.", ExceptionType.SendServiceRequestToProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, string.Empty);
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while sending an email to 8x8 mailbox.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the service request notes.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <returns>Notes Collection.</returns>
        private static Notes[] GetServiceRequestNotes(IOrganizationService service, Guid serviceRequestId)
        {
            try
            {
                string storageAccountName = string.Empty;
                string blobName = string.Empty;
                List<Notes> notesCollection = new List<Notes>();
                EntityCollection entityCollection = ServiceRequestHelper.GetQueryResponse(service, "annotation", new string[] { "annotationid", "subject", "notetext", "filename", "isdocument", "documentbody", "createdon" }, "objectid", serviceRequestId);
                EntityCollection azureSettingsColection = GetAzureAttachmentSettings(service);
                if (azureSettingsColection != null && azureSettingsColection.Entities.Count > 0)
                {
                    foreach (Entity entity in azureSettingsColection.Entities)
                    {
                        storageAccountName = entity["msdyn_name"].ToString();
                        blobName = entity["msdyn_annotationcontainer"].ToString();
                    }
                }

                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    foreach (Entity entity in entityCollection.Entities)
                    {
                        Notes notes = new Notes();

                        if (entity.Attributes.Contains("annotationid"))
                        {
                            notes.NoteId = entity["annotationid"].ToString();
                        }

                        if (entity.Attributes.Contains("subject"))
                        {
                            notes.Subject = (string)entity["subject"];
                        }

                        if (entity.Attributes.Contains("notetext"))
                        {
                            notes.Description = (string)entity["notetext"];
                        }

                        if (entity.Attributes.Contains("createdon"))
                        {
                            DateTime createdonDateTime = (DateTime)entity.Attributes["createdon"];
                            notes.NotesCreatedOn = new DateTime(createdonDateTime.Year, createdonDateTime.Month, createdonDateTime.Day, createdonDateTime.Hour, createdonDateTime.Minute, (int)createdonDateTime.Second, (int)DateTimeKind.Utc);
                        }

                        if (entity.Attributes.Contains("isdocument"))
                        {
                            if ((bool)entity.Attributes["isdocument"] == true)
                            {
                                if (entity.Attributes.Contains("filename"))
                                {
                                    ////User Story 4320040:Enable SAS token for Blob Storage and make entities configurable while creating blob URL with SAS token in attachmentURL entity for AX and 3rd Party
                                    string fileName = entity.Attributes["filename"].ToString();
                                    if (!string.IsNullOrEmpty(fileName))
                                    {
                                        fileName = Uri.EscapeDataString(fileName);
                                    }

                                    var documentBody = entity.Attributes["documentbody"].ToString();
                                    string sasToken = GetConfigurationValue(service, "SASToken");
                                    var blobHelper = new BlobHelper(storageAccountName, sasToken);
                                    bool putblob = BlobHelper.PutBlob(blobName, entity.Id.ToString() + "_" + fileName, documentBody);
                                    string bloburl = GetConfigurationValue(service, "AttachmentBlobUrl");
                                    bloburl = bloburl + entity.Id.ToString() + "_" + fileName;

                                    bloburl += sasToken;
                                    notes.AttachmentUrl = bloburl;
                                }
                            }
                        }

                        notesCollection.Add(notes);
                    }
                }

                return notesCollection.ToArray();
            }
            catch (System.Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Fetching Notes failed.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the dynamic problem type notes collection.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <returns>Returns a list of DynamicProblemTypeNotes.</returns>
        private static DynamicProblemTypeNotes[] GetDynamicProblemTypeNotesCollection(IOrganizationService service, Guid serviceRequestId)
        {
            try
            {
                List<DynamicProblemTypeNotes> dynamicProblemTypeNotesCollection = new List<DynamicProblemTypeNotes>();

                EntityCollection entityCollection = ServiceRequestHelper.GetQueryResponse(service, "smp_servicerequestproblemtypedesc", new string[] { "smp_servicerequestproblemtypedescid", "smp_problemtypedescriptionid", "smp_answer" }, "smp_servicerequestid", serviceRequestId);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    foreach (Entity entity in entityCollection.Entities)
                    {
                        DynamicProblemTypeNotes dynamicProblemTypeNotes = new DynamicProblemTypeNotes();
                        dynamicProblemTypeNotes.DynamicsProblemTypeId = entity.Id.ToString();
                        if (entity.Attributes.Contains("smp_problemtypedescriptionid"))
                        {
                            dynamicProblemTypeNotes.DynamicsProblemTypeName = ServiceRequestHelper.GetLookupName(service, ((EntityReference)entity["smp_problemtypedescriptionid"]).Id, "smp_problemtypedescriptionname", "smp_problemtypedescription");
                            if (entity.Attributes.Contains("smp_answer"))
                            {
                                dynamicProblemTypeNotes.Answer = (string)entity["smp_answer"];
                                dynamicProblemTypeNotesCollection.Add(dynamicProblemTypeNotes);
                            }
                        }
                    }
                }

                return dynamicProblemTypeNotesCollection.ToArray();
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Fetching dynamic problem type notes failed.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the query response.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <param name="fieldsToBeFetched">The fields to be fetched.</param>
        /// <param name="criteriaField">The criteria field.</param>
        /// <param name="criteriaValue">The criteria value.</param>
        /// <returns>Returns the Entity Collection.</returns>
        private static EntityCollection GetQueryResponse(IOrganizationService service, string entityLogicalName, string[] fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = entityLogicalName;
                query.ColumnSet = new ColumnSet(fieldsToBeFetched);
                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression(criteriaField, ConditionOperator.Equal, criteriaValue));
                query.Criteria = filter;
                EntityCollection entityCollection = service.RetrieveMultiple(query);
                return entityCollection;
            }

            return null;
        }

        private static Guid GetLinkedQueryResponse(IOrganizationService service, string contactLanguage)
        {
            Guid id = Guid.Empty;
            string query = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                <entity name='smp_8x8mailbox'>
                                <attribute name='smp_8x8mailboxid'/>
                                <order descending='false' attribute='smp_name'/>
                                <link-entity name='adx_portallanguage' alias='af' to='new_preferedlanguage' from='adx_portallanguageid'>
                                <filter type='and'>
                                <condition attribute='adx_languagecode' value='" + contactLanguage + @"' operator='eq'/>
                                </filter>
                                </link-entity>
                                </entity>
                                </fetch> ";
            EntityCollection collection = service.RetrieveMultiple(new FetchExpression(query));
            if (collection.Entities.Count > 0)
            {
                id = (Guid)collection.Entities[0]["smp_8x8mailboxid"];
            }

            return id;
        }

        /// <summary>
        /// This function is used to retrieve the option set label using the option set value
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <returns>Returns the Option set text</returns>
        private static string GetOptionsSetTextForValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
        {
            try
            {
                if (service != null)
                {
                    RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
                    {
                        EntityLogicalName = entityName,
                        LogicalName = attributeName,
                        RetrieveAsIfPublished = true
                    };

                    // Execute the request.
                    RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);

                    // Access the retrieved attribute.
                    PicklistAttributeMetadata retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;

                    // Get the current options list for the retrieved attribute.
                    OptionMetadata[] optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
                    string selectedOptionLabel = null;
                    foreach (OptionMetadata optionMetadata in optionList)
                    {
                        if (optionMetadata.Value == selectedValue)
                        {
                            selectedOptionLabel =
                                optionMetadata.Label.LocalizedLabels[0].Label.ToString(CultureInfo.InvariantCulture);
                            break;
                        }
                    }

                    return selectedOptionLabel;
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching option set label for fieldname {0}, value {1} and entity {2} failed.", attributeName, selectedValue, entityName), ex);
                throw customEx;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get fetched value from returned result
        /// </summary>
        /// <param name="service">organization service</param>
        /// <param name="entityLogicalName">logical name of entity to be fetched</param>
        /// <param name="fieldsToBeFetched">field to be fetched</param>
        /// <param name="criteriaField">criteria field</param>
        /// <param name="criteriaValue">criteria value</param>
        /// <returns>gets query result</returns>
        private static string GetFetchedValue(IOrganizationService service, string entityLogicalName, string fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            string fetchedValue = string.Empty;
            try
            {
                EntityCollection collection = GetQueryResponse(service, entityLogicalName, new string[] { fieldsToBeFetched }, criteriaField, criteriaValue);
                if (collection != null && collection.Entities.Count > 0 && collection.Entities[0].Attributes.Contains(fieldsToBeFetched))
                {
                    fetchedValue = collection.Entities[0].Attributes[fieldsToBeFetched].ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return fetchedValue;
        }

        /// <summary>
        /// For Binding the address and Client
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="endTime"></param>
        /// <param name="binding"></param>
        /// <param name="address"></param>
        private static void ClientBinding(Provider provider, DateTime endTime, ref WSHttpBinding binding, ref EndpointAddress address)
        {
            address = new EndpointAddress(provider.ProviderServiceAddress);
            if (provider.ProviderServiceAddress.ToUpperInvariant().StartsWith("HTTPS://", StringComparison.Ordinal))
            {
                binding = new WSHttpBinding(SecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            }
            else
            {
                binding = new WSHttpBinding(SecurityMode.None);
            }

            int responseTime = (int)endTime.Subtract(DateTime.Now).TotalSeconds;
            //// Set timeout for WCF call
            binding.SendTimeout = new TimeSpan(0, 0, responseTime > 0 ? responseTime : 0);
        }

        /// <summary>
        /// GetProviderIntegrationCollection
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private static Entity GetProviderIntegrationCollection(IOrganizationService service)
        {
            try
            {
                QueryExpression query = new QueryExpression("smp_providerintegration");
                query.ColumnSet = new ColumnSet("smp_clientid", "smp_serviceprincipalid");
                query.Criteria.AddCondition("smp_name", ConditionOperator.Equal, "Engineer Team Details");
                Entity entity = service.RetrieveMultiple(query).Entities.FirstOrDefault();
                if (entity != null)
                {
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// GenerateAzureToken
        /// </summary>
        /// <param name="timeOut"></param>
        /// <param name="trace"></param>
        /// <param name="iD"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GenerateAzureToken(IOrganizationService service, ITracingService trace, string iD, string key)
        {
            try
            {
                string tokenURL = GetConfigurationValue(service, "AzureToeknService");
                WSHttpBinding myBinding = new WSHttpBinding();
                myBinding.Name = "WSHttpBinding_ICmmsServiceRequestManager";
                myBinding.Security.Mode = SecurityMode.Transport;
                myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                EndpointAddress endPointAddress = new
                EndpointAddress(tokenURL);
                ICmmsServiceRequestManagerAAD client = new CmmsServiceRequestManagerAADClient(myBinding, endPointAddress);
                trace.Trace("Before Token generation");
                string aadtoken = client.GenerateToken(iD, key);
                return aadtoken;
                ////string tokenURL = GetConfigurationValue(service, "TokenEndpointURL");
                ////string aadtoken = string.Empty;
                ////string requestBody = "grant_type=client_credentials&client_secret=" + HttpUtility.UrlEncode(key) + "&client_id=" + HttpUtility.UrlEncode(iD);                
                ////HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenURL);
                ////request.Method = "POST";
                ////request.Accept = "application/json";
                ////request.ContentType = "application/x-www-form-urlencoded";
                ////byte[] postdata = System.Text.Encoding.UTF8.GetBytes(requestBody);
                ////request.ContentLength = postdata.Length;
                ////Stream requestWriter = request.GetRequestStream();
                ////requestWriter.Write(postdata, 0, postdata.Length);
                ////requestWriter.Close();
                ////HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
                ////using (webResponse)
                ////{
                ////    using (Stream webStream = webResponse.GetResponseStream())
                ////    {
                ////        if (webStream != null)
                ////        {
                ////            StreamReader responseReader = new StreamReader(webStream);
                ////            string response = responseReader.ReadToEnd();
                ////            Token token = new Token();
                ////            token = JsonHelper.JsonDeserialize<Token>(response);
                ////            aadtoken = token.Access_token;
                ////        }
                ////    }
                ////}

                ////return aadtoken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the azure attachment settings.
        /// </summary>
        /// <param name="service">The service.</param> 
        /// <returns>Returns the Entity Collection.</returns>
        private static EntityCollection GetAzureAttachmentSettings(IOrganizationService service)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = "msdyn_azureblobstoragesetting";
                query.ColumnSet = new ColumnSet(new string[] { "msdyn_name", "msdyn_sastoken", "msdyn_annotationcontainer" });
                EntityCollection entityCollection = service.RetrieveMultiple(query);
                return entityCollection;
            }

            return null;
        }

        public class Token
        {
            public string Access_token { get; set; }

            public double Expires_in { get; set; }
        }
    }
}