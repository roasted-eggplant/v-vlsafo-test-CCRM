// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestCreateSetPriority.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreServiceRequestCreateSetPriority Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;
    using CCRM.REF.TelemetryLog;

    public class PreServiceRequestCreateSetPriority : IPlugin
    {
        IRequestLogging requestLogging;

        public void Execute(IServiceProvider serviceProvider)
        {
            CCRM.REF.TelemetryLog.LocalPluginContext localPluginContext = new CCRM.REF.TelemetryLog.LocalPluginContext(serviceProvider);
            IConfigurationRetrieval configurationRetrieval = new ConfigurationRetrieval();
            this.requestLogging = new RequestLogging(configurationRetrieval, localPluginContext);
            Entity entity = new Entity();
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));
                ITracingService trace =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                Guid buildingId = Guid.Empty;
                Guid problemClassId = Guid.Empty;
                Guid problemTypeId = Guid.Empty;
                Guid problemRoomType = Guid.Empty;
                DateTime problemoccureddate = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
                DateTime duedate = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
                DateTime createdOn = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
                DateTime submitteddate = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
                DateTime emptysubmitteddate = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
                Guid priorityId = Guid.Empty;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    entity = (Entity)context.InputParameters["Target"];
                    trace.Trace("Depth From PreServiceRequestCreateSetPriority :" + context.Depth.ToString());
                    if (entity.LogicalName == "incident")
                    {
                        createdOn = Convert.ToDateTime(entity.Attributes["createdon"].ToString(), CultureInfo.CurrentCulture);
                        if (entity.Attributes.Contains("smp_problembuilding"))
                        {
                            buildingId = ((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes["smp_problembuilding"]).Id;
                        }

                        if (entity.Attributes.Contains("smp_duedate"))
                        {
                            duedate = Convert.ToDateTime(entity.Attributes["smp_duedate"], CultureInfo.CurrentCulture);
                        }

                        if (entity.Attributes.Contains("smp_problemoccureddatetime"))
                        {
                            problemoccureddate = Convert.ToDateTime(entity.Attributes["smp_problemoccureddatetime"], CultureInfo.CurrentCulture);
                        }

                        if (entity.Attributes.Contains("smp_problemclassid"))
                        {
                            problemClassId = ((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes["smp_problemclassid"]).Id;
                        }

                        if (entity.Attributes.Contains("smp_problemtypeid"))
                        {
                            problemTypeId = ((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes["smp_problemtypeid"]).Id;
                        }

                        if (entity.Attributes.Contains("smp_problemroomtype"))
                        {
                            problemRoomType = ((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes["smp_problemroomtype"]).Id;
                        }

                        if (entity.Attributes.Contains("smp_submitteddatetime"))
                        {
                            submitteddate = Convert.ToDateTime(entity.Attributes["smp_submitteddatetime"], CultureInfo.CurrentCulture);
                        }

                        if (entity.Attributes.Contains("smp_priorityid"))
                        {
                            priorityId = ((Microsoft.Xrm.Sdk.EntityReference)entity.Attributes["smp_priorityid"]).Id;
                        }

                        OptionSetValue entityStatusCode = (OptionSetValue)entity.Attributes["statuscode"];
                        OptionSetValue caseOrigin = (OptionSetValue)entity["caseorigincode"];

                        ////code change on 09-01-2018 for priority set only on portal submit true and not just on case origin 3
                        bool portalSubmit = (bool)entity["smp_portalsubmit"];
                        int statusCode = Convert.ToInt32(entityStatusCode.Value, CultureInfo.InvariantCulture);
                        trace.Trace("entityStatusCode :" + statusCode);
                        if (statusCode == (int)ServiceRequestEnum.StatusCode.Closed)
                        {
                            ////Logger.Write("Priority not set", ExceptionType.SetPriorityFailed, service, "incident", "smp_servicerequestid", entity.Id, "Service Request Status is Draft");
                        }
                        else if (statusCode == (int)ServiceRequestEnum.StatusCode.Draft || statusCode == (int)ServiceRequestEnum.StatusCode.Open)
                        {
                            if (submitteddate == Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                            {
                                emptysubmitteddate = Convert.ToDateTime(DateTime.UtcNow, CultureInfo.CurrentCulture);
                            }

                            int zone = 0;
                            if (problemRoomType != Guid.Empty)
                            {
                                zone = GetZoneByProblemRoomType(service, problemRoomType, context, entity.Id);
                            }

                            if (buildingId != Guid.Empty && problemoccureddate != null && problemClassId != Guid.Empty && problemTypeId != Guid.Empty && problemRoomType != Guid.Empty)
                            {
                                trace.Trace("Before the UpdateUserTimeZonesToServiceRequest");
                                UpdateUserTimeZonesToServiceRequest(
                                    service,
                                    entity.Id,
                                    buildingId,
                                    createdOn,
                                    duedate,
                                    problemoccureddate,
                                    context,
                                    problemClassId,
                                    problemTypeId,
                                    zone,
                                    submitteddate,
                                    emptysubmitteddate,
                                    priorityId,
                                    statusCode,
                                    trace);
                            }
                            else
                            {
                                trace.Trace("In Final Elese");
                                LogNotAllAttributesPopulated(service, entity);
                            }
                        }
                    }
                }

                ////this.requestLogging.LogPluginTrace(entity, MappingConstants.ServiceRequestCreatedSequenceId, MappingConstants.ServiceRequestCreatedSuccessEventId, MappingConstants.ServiceRequestCreatedEventName, MappingConstants.ServiceRequestCreatedSuccessEventMessage);
            }
            catch (Exception ex)
            {
                ////this.requestLogging.LogPluginException(entity, ex, MappingConstants.ServiceRequestCreatedSequenceId, MappingConstants.ServiceRequestCreatedFailedEventId, MappingConstants.ServiceRequestCreatedEventName, MappingConstants.ServiceRequestCreatedFailedEventMessage);
                throw new InvalidPluginExecutionException("Error in Pre create Set priority " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the building id.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="objIncident">The object incident.</param>
        /// <param name="buildingId">The building id.</param>
        /// <param name="createdTime">The created time.</param>
        /// <param name="dueDateTime">The due date time.</param>
        /// <param name="problemTime">The problem time.</param>
        /// <param name="context">Building Id</param>
        /// <param name="problemClass">The problem class.</param>
        /// <param name="problemType">Type of the problem.</param>
        /// <param name="zone">The zone.</param>
        /// <param name="submittedDateTime">The submitted date time.</param>
        /// <param name="emptySubmittedDate">The empty submitted date.</param>
        /// <param name="priorityId">Priority Id</param>
        private static void UpdateUserTimeZonesToServiceRequest(
            IOrganizationService service,
            Guid objIncident,
            Guid buildingId,
            DateTime createdTime,
            DateTime dueDateTime,
            DateTime problemTime,
            IExecutionContext context,
            Guid problemClass,
            Guid problemType,
            int zone,
            DateTime submittedDateTime,
            DateTime emptySubmittedDate,
            Guid priorityId,
            int statusCode,
            ITracingService trace)
        {
            Guid timeZoneId = Guid.Empty;
            string timeZoneName = string.Empty;
            string createdDateOnBuilidingTimeZone = string.Empty;
            string dueDateOnBuilidingTimeZone = string.Empty;
            string problemOccuredDateOnBuilidingTimeZone = string.Empty;
            string finalcialStateCampus = string.Empty;
            string submittedDateOnBuilidingTimeZone = string.Empty;
            double dueDateOffset = 0;
            try
            {
                if (context != null)
                {
                    if (buildingId != Guid.Empty)
                    {
                        trace.Trace("Before Getting Priority");
                        string[] timeZoneDetails = GetTimeZoneIdFromBuilding(service, trace, buildingId, context, objIncident).ToString().Split('@');
                        timeZoneId = new Guid(timeZoneDetails[0]);
                        finalcialStateCampus = timeZoneDetails[1].ToString();
                        if (timeZoneId != Guid.Empty)
                        {
                            timeZoneName = GetTimeZoneNameFromTimeZone(service, trace, timeZoneId, context, objIncident);

                            if (!string.IsNullOrEmpty(timeZoneName))
                            {
                                string[] time = new string[0];
                                string offset = timeZoneName;
                                string symbol = offset.Substring(0, 1);
                                if (symbol == "+")
                                {
                                    time = offset.Split('+');
                                }
                                else if (symbol == "-")
                                {
                                    time = offset.Split('-');
                                }

                                double min;

                                if (time[1].Contains(":"))
                                {
                                    string[] timesplit = time[1].Split(':');

                                    decimal hourMinutes = Convert.ToDecimal(timesplit[0], CultureInfo.InvariantCulture) * 60;
                                    decimal minutes = hourMinutes + Convert.ToDecimal(timesplit[1], CultureInfo.InvariantCulture);
                                    min = Convert.ToDouble(minutes);
                                }
                                else
                                {
                                    decimal hourMinutes = Convert.ToDecimal(time[1], CultureInfo.InvariantCulture) * 60;
                                    decimal minutes = hourMinutes;
                                    min = Convert.ToDouble(minutes);
                                }

                                if (symbol == "+")
                                {
                                    string addNegative = "-" + Convert.ToString(min, CultureInfo.InvariantCulture);
                                    dueDateOffset = Convert.ToDouble(addNegative, CultureInfo.InvariantCulture);
                                    createdDateOnBuilidingTimeZone = createdTime.AddMinutes(min).ToString();
                                    if (dueDateTime == Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                                    {
                                        dueDateOnBuilidingTimeZone = dueDateTime.ToString();
                                    }
                                    else
                                    {
                                        dueDateOnBuilidingTimeZone = dueDateTime.AddMinutes(min).ToString();
                                    }

                                    if (problemTime == Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                                    {
                                        problemOccuredDateOnBuilidingTimeZone = createdTime.AddMinutes(min).ToString();
                                    }
                                    else
                                    {
                                        problemOccuredDateOnBuilidingTimeZone = problemTime.AddMinutes(min).ToString();
                                    }

                                    if (submittedDateTime != Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                                    {
                                        submittedDateOnBuilidingTimeZone = submittedDateTime.AddMinutes(min).ToString();
                                    }
                                    else
                                    {
                                        submittedDateOnBuilidingTimeZone = emptySubmittedDate.AddMinutes(min).ToString();
                                    }
                                }
                                else if (symbol == "-")
                                {
                                    dueDateOffset = min;
                                    string addNegative = "-" + Convert.ToString(min, CultureInfo.InvariantCulture);
                                    double negativeMinutes = Convert.ToDouble(addNegative, CultureInfo.InvariantCulture);
                                    createdDateOnBuilidingTimeZone = createdTime.AddMinutes(negativeMinutes).ToString();
                                    if (dueDateTime == Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                                    {
                                        dueDateOnBuilidingTimeZone = dueDateTime.ToString();
                                    }
                                    else
                                    {
                                        dueDateOnBuilidingTimeZone = dueDateTime.AddMinutes(negativeMinutes).ToString();
                                    }

                                    if (problemTime == Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                                    {
                                        problemOccuredDateOnBuilidingTimeZone = createdTime.AddMinutes(negativeMinutes).ToString();
                                    }
                                    else
                                    {
                                        problemOccuredDateOnBuilidingTimeZone = problemTime.AddMinutes(negativeMinutes).ToString();
                                    }

                                    if (submittedDateTime != Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                                    {
                                        submittedDateOnBuilidingTimeZone = submittedDateTime.AddMinutes(negativeMinutes).ToString();
                                    }
                                    else
                                    {
                                        submittedDateOnBuilidingTimeZone = emptySubmittedDate.AddMinutes(negativeMinutes).ToString();
                                    }
                                }

                                Guid priority = Guid.Empty;
                                if (priorityId != Guid.Empty)
                                {
                                    priority = priorityId;
                                }
                                else
                                {
                                    priority = GetPriorityId(service, trace, context, problemClass, problemType, zone, finalcialStateCampus, objIncident);
                                }

                                int priorityHours = 0;
                                priorityHours = TimeZoneHelper.GetPriorityHours(service, trace, priority);
                                trace.Trace("Priority :" + priority.ToString());
                                UpdateServiceActivity(
                                service,
                                objIncident,
                                dueDateOnBuilidingTimeZone,
                                problemOccuredDateOnBuilidingTimeZone,
                                createdDateOnBuilidingTimeZone,
                                context,
                                priority,
                                priorityHours,
                                submittedDateOnBuilidingTimeZone,
                                submittedDateTime,
                                emptySubmittedDate,
                                buildingId,
                                trace,
                                dueDateOffset,
                                statusCode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace("UpdateUserTimeZonesToServiceRequest " + "---Error Message ---" + ex.Message + "----Stack Trace-----" + ex.StackTrace);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while updating service activity", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the time zone id from building.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="buildingId">The building id.</param>
        /// <param name="context">The context.</param>
        /// <param name="incidentId">The incident.</param>
        /// <returns>Get Time Zone Id</returns>
        private static string GetTimeZoneIdFromBuilding(IOrganizationService service, ITracingService tracingService, Guid buildingId, IExecutionContext context, Guid incidentId)
        {
            Guid timeZoneID = Guid.Empty;
            string timeZoneDetails = string.Empty;
            string finalcialStateCampus = string.Empty;
            try
            {
                if (context != null)
                {
                    if (service != null)
                    {
                        Entity building = service.Retrieve("smp_building", buildingId, new ColumnSet("smp_timezoneid", "smp_isfinancialstatecampus"));
                        if (building != null)
                        {
                            if (building.Attributes.Contains("smp_timezoneid"))
                            {
                                timeZoneID = ((Microsoft.Xrm.Sdk.EntityReference)building.Attributes["smp_timezoneid"]).Id;
                            }

                            if (building.Attributes.Contains("smp_isfinancialstatecampus"))
                            {
                                finalcialStateCampus = building.Attributes["smp_isfinancialstatecampus"].ToString();
                            }

                            timeZoneDetails = timeZoneID + "@" + finalcialStateCampus;
                        }
                    }
                }

                return timeZoneDetails;
            }
            catch (Exception ex)
            {
                tracingService.Trace("Exception while getting time zone details from building" + " ----Error Message----" + ex.Message + " -----Stack Trace---- " + ex.StackTrace);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while getting time zone details from building", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the time zone name from time zone.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="timeZoneId">The time zone id.</param>
        /// <param name="context">The context.</param>
        /// <param name="incidentId">The incident.</param>
        /// <returns>Get Offset</returns>
        private static string GetTimeZoneNameFromTimeZone(IOrganizationService service, ITracingService tracingService, Guid timeZoneId, IExecutionContext context, Guid incidentId)
        {
            int optionsetValue;
            string optionsetText = string.Empty;

            try
            {
                if (context != null)
                {
                    if (service != null)
                    {
                        Entity timeZone = service.Retrieve("smp_timezone", timeZoneId, new ColumnSet("smp_timezonename", "smp_offset"));
                        if (timeZone != null)
                        {
                            optionsetValue = ((Microsoft.Xrm.Sdk.OptionSetValue)timeZone["smp_offset"]).Value;
                            optionsetText = GetOptionsSetTextOnValue(service, tracingService, "smp_timezone", "smp_offset", optionsetValue);
                        }
                    }
                }

                return optionsetText;
            }
            catch (Exception ex)
            {
                tracingService.Trace("Exception while getting offset from Time Zone" + " ----Error Message----" + ex.Message + " -----Stack Trace---- " + ex.StackTrace);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while getting offset from Time Zone", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Updates the service activity.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="objIncident">The object incident.</param>
        /// <param name="userDueDate">The user due date.</param>
        /// <param name="userProblemDate">The user problem date.</param>
        /// <param name="userCreatedDateTime">The user created date time.</param>
        /// <param name="context">The context.</param>
        /// <param name="priorityId">The priority id.</param>
        /// <param name="priorityHours">The priority hours.</param>
        /// <param name="userSubmittedDateTime">The user submitted date time.</param>
        /// <param name="recordSubmittedDate">The record submitted date.</param>
        /// <param name="emptySubmittedDate">The empty submitted date.</param>
        /// <param name="buildingId">The building id.</param>
        private static void UpdateServiceActivity(
            IOrganizationService service,
            Guid objIncident,
            string userDueDate,
            string userProblemDate,
            string userCreatedDateTime,
            IExecutionContext context,
            Guid priorityId,
            int priorityHours,
            string userSubmittedDateTime,
            DateTime recordSubmittedDate,
            DateTime emptySubmittedDate,
            Guid buildingId,
            ITracingService trace,
            double dueDateOffset,
            int statusCode)
        {
            try
            {
                if (context != null)
                {
                    trace.Trace("Before Assigning Values");
                    Entity incident = (Entity)context.InputParameters["Target"];
                    EntityReference contact = incident.Attributes.Contains(CRMAttributesResource.ServiceRequestContact) ? (EntityReference)incident[CRMAttributesResource.ServiceRequestContact] : null;
                    int caseorigin = ((OptionSetValue)incident["caseorigincode"]).Value;
                    bool portalSubmit = (bool)incident["smp_portalsubmit"];
                    incident.Attributes.Add("smp_submitteddatetimebybuildingtimezone", string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(userSubmittedDateTime, CultureInfo.CurrentCulture.DateTimeFormat)));
                    incident.Attributes.Add("smp_createddatetimebybuildingtimezone", string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(userCreatedDateTime, CultureInfo.CurrentCulture.DateTimeFormat)));
                    incident.Attributes.Add("smp_occureddatetimebybuildingtimezone", string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(userProblemDate, CultureInfo.CurrentCulture.DateTimeFormat)));
                    int priorityMinutes = Convert.ToInt32(priorityHours, CultureInfo.CurrentCulture); ////* 60;
                    if (priorityId != Guid.Empty)
                    {
                        trace.Trace("Priority is not null");
                        DateTime due;
                        incident["smp_priorityid"] = new EntityReference("smp_priority", priorityId);
                        if (Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture) == Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                        {
                            due = Convert.ToDateTime(userSubmittedDateTime, CultureInfo.CurrentCulture).AddMinutes(Convert.ToDouble(Convert.ToString(priorityMinutes, CultureInfo.InvariantCulture), CultureInfo.CurrentCulture));
                        }
                        else
                        {
                            due = Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture).AddMinutes(Convert.ToDouble(Convert.ToString(priorityMinutes, CultureInfo.InvariantCulture), CultureInfo.CurrentCulture));
                        }

                        if (priorityHours != 0)
                        {
                            trace.Trace("Priority Hours is not 0");
                            due = TimeZoneHelper.GetDueDateByBuildingTimeZoneConsideringHolidays(service, buildingId, due, priorityMinutes);
                            if (due != Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                            {
                                incident.Attributes.Add("smp_duedatebybuildingtimezone", string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(due, CultureInfo.CurrentCulture.DateTimeFormat)));
                                due = due.AddMinutes(dueDateOffset);
                                trace.Trace("context.UserId : " + context.UserId);
                                int? runningUserTimeZoneCode = TimeZoneHelper.RetrieveUsersSettings(service, context.UserId, caseorigin, portalSubmit, statusCode, statusCode, trace);
                                trace.Trace("runningUserTimeZoneCode : " + runningUserTimeZoneCode);
                                DateTime dueUserTimeZone = TimeZoneHelper.RetrieveLocalTimeFromUTCTime(due, runningUserTimeZoneCode, service);
                                incident.Attributes.Add("smp_duedate", Convert.ToDateTime(dueUserTimeZone, CultureInfo.CurrentCulture));
                                incident.Attributes.Add("smp_duedatetimebybuildingtimezone", Convert.ToDateTime(dueUserTimeZone, CultureInfo.CurrentCulture));
                            }
                        }
                        else
                        {
                            incident.Attributes.Add("smp_duedatebybuildingtimezone", null);
                            incident.Attributes.Add("smp_duedatetimebybuildingtimezone", null);
                            incident.Attributes.Add("smp_duedate", null);
                        }
                    }
                    else
                    {
                        trace.Trace("Priority is Null");
                        if (priorityHours != 0)
                        {
                            trace.Trace("Priority Hours is not 0");
                            userDueDate = TimeZoneHelper.GetDueDateByBuildingTimeZoneConsideringHolidays(service, buildingId, Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture), priorityMinutes).ToString();
                            if (Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture) != Convert.ToDateTime(null, CultureInfo.CurrentCulture))
                            {
                                incident.Attributes.Add("smp_duedatebybuildingtimezone", string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture.DateTimeFormat)));
                                userDueDate = Convert.ToDateTime(userDueDate).AddMinutes(dueDateOffset).ToString();
                                incident.Attributes.Add("smp_duedate", Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture));
                                incident.Attributes.Add("smp_duedatetimebybuildingtimezone", Convert.ToDateTime(userDueDate, CultureInfo.CurrentCulture));
                            }
                        }
                        else
                        {
                            incident.Attributes.Add("smp_duedatebybuildingtimezone", null);
                            incident.Attributes.Add("smp_duedatetimebybuildingtimezone", null);
                            incident.Attributes.Add("smp_duedate", null);
                        }
                    }

                    trace.Trace("Before Submitted Date Time");
                    DateTime recordsubmitted = Convert.ToDateTime(emptySubmittedDate, CultureInfo.CurrentCulture);
                    incident["smp_submitteddatetime"] = Convert.ToDateTime(recordsubmitted, CultureInfo.CurrentCulture);
                    trace.Trace("Completed assigning");
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while updating priority.", ex);
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
        private static string GetOptionsSetTextOnValue(IOrganizationService service, ITracingService tracingService, string entityName, string attributeName, int selectedValue)
        {
            try
            {
                RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };
                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);

                Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata retrievedPicklistAttributeMetadata = (Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)

                retrieveAttributeResponse.AttributeMetadata; //// Get the current options list for the retrieved attribute.
                OptionMetadata[] optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
                string selectedOptionLabel = string.Empty;
                foreach (OptionMetadata oMD in optionList)
                {
                    if (oMD.Value == selectedValue)
                    {
                        selectedOptionLabel = oMD.Label.UserLocalizedLabel.Label;
                    }
                }

                return selectedOptionLabel;
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error while getting time zone offset text." + " ----Error Message----" + ex.Message + " -----Stack Trace---- " + ex.StackTrace);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting time zone offset text.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the type of the zone by problem room.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="problemRoomType">Type of the problem room.</param>
        /// <param name="context">The context.</param>
        /// <param name="incidentId">The incident.</param>
        /// <returns>Problem Room Type</returns>
        private static int GetZoneByProblemRoomType(IOrganizationService service, Guid problemRoomType, IExecutionContext context, Guid incidentId)
        {
            int zone = 0;
            try
            {
                if (context != null)
                {
                    if (service != null)
                    {
                        Entity roomType = service.Retrieve("smp_roomtype", problemRoomType, new ColumnSet("smp_zone"));
                        if (roomType != null)
                        {
                            if (roomType.Attributes.Contains("smp_zone"))
                            {
                                zone = Convert.ToInt32(((Microsoft.Xrm.Sdk.OptionSetValue)roomType.Attributes["smp_zone"]).Value);
                            }
                        }
                    }
                }

                return zone;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting zone from Room Type.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets the priority id.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="context">The context.</param>
        /// <param name="problemClass">The problem class.</param>
        /// <param name="problemType">Type of the problem.</param>
        /// <param name="zone">The zone.</param>
        /// <param name="finalcialStateCampus">The financial state campus.</param>
        /// <param name="incidentId">The incident.</param>
        /// <returns>Priority Id</returns>
        private static Guid GetPriorityId(
            IOrganizationService service,
            ITracingService tracingService,
            IExecutionContext context,
            Guid problemClass,
            Guid problemType,
            int zone,
            string finalcialStateCampus,
            Guid incidentId)
        {
            Guid priorityId = Guid.Empty;
            try
            {
                if (context != null)
                {
                    QueryExpression slaquery = new QueryExpression()
                    {
                        EntityName = "smp_slamatrix",
                        ColumnSet = new ColumnSet("smp_priorityid", "smp_starthours", "smp_endhours"),
                        Criteria =
                            {
                                Conditions =
                                {
                                    new ConditionExpression("smp_isfinancialstatecampus", ConditionOperator.Equal, Convert.ToBoolean(finalcialStateCampus, CultureInfo.InvariantCulture)),
                                    new ConditionExpression("smp_problemclassid", ConditionOperator.Equal, problemClass),
                                    new ConditionExpression("smp_problemtypeid", ConditionOperator.Equal, problemType),
                                    new ConditionExpression("smp_zone", ConditionOperator.Equal, zone),
                                    new ConditionExpression("statuscode", ConditionOperator.Equal, 1),
                                }
                            }
                    };
                    if (service != null)
                    {
                        EntityCollection priority = service.RetrieveMultiple(slaquery);
                        Entity priority_ = priority.Entities.Count > 0 ? priority.Entities[0] : null;
                        if (priority_ != null)
                        {
                            priorityId = ((EntityReference)priority_["smp_priorityid"]).Id;
                        }
                    }
                }

                return priorityId;
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error while getting priority." + " ----Error Message----" + ex.Message + " -----Stack Trace---- " + ex.StackTrace);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting priority.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Logs the not all attributes populated.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="objectIncident">The object incident.</param>
        private static void LogNotAllAttributesPopulated(IOrganizationService service, Entity objectIncident)
        {
            var message = "Priority was not set.";
            if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemBuildingIdAttribute))
            {
                message = "Building, ";
            }

            if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemRoomTypeIdAttribute))
            {
                message = message + "Problem Room, ";
            }

            if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemClassIdAttribute))
            {
                message = message + "Problem Class, ";
            }

            if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemTypeIdAttribute))
            {
                message = message + "Problem Type";
            }

            message = message.TrimEnd(',');
            message = message + " is missing.";
            Logger.Write(message, ExceptionType.SetPriorityFailed, service, "incident", "smp_servicerequestid", objectIncident.Id, string.Empty);
        }
    }
}
