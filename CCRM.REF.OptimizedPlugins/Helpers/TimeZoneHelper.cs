// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeZoneHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Resthelper to connect the storage container in azure.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Class TimeZoneHelper
    /// As part of UserStory:3882364 changes, common functions are moved to Helper clas
    /// </summary>
    public static class TimeZoneHelper
    {
        /// <summary>
        /// Gets the time zone id from building.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="buildingId">The building id.</param>
        /// <param name="incidentId">The incident.</param>
        /// <returns>Time Zone Id.</returns>
        public static string GetTimeZoneIdFromBuilding(IOrganizationService service, Guid buildingId, Guid incidentId)
        {
            Guid timeZoneID = Guid.Empty;
            string timeZoneDetails = string.Empty;
            string finalcialStateCampus = string.Empty;
            try
            {
                if (service != null)
                {
                    Entity buildiing = service.Retrieve("smp_building", buildingId, new ColumnSet("smp_timezoneid", "smp_isfinancialstatecampus"));
                    if (buildiing != null)
                    {
                        if (buildiing.Attributes.Contains("smp_timezoneid"))
                        {
                            timeZoneID = ((Microsoft.Xrm.Sdk.EntityReference)buildiing.Attributes["smp_timezoneid"]).Id;
                        }

                        if (buildiing.Attributes.Contains("smp_isfinancialstatecampus"))
                        {
                            finalcialStateCampus = buildiing.Attributes["smp_isfinancialstatecampus"].ToString();
                        }

                        timeZoneDetails = timeZoneID + "@" + finalcialStateCampus;
                    }
                }

                return timeZoneDetails;
            }
            catch (Exception ex)
            {
                PopulateExceptionLog(ex, service, incidentId);
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
        public static string GetTimeZoneNameFromTimeZone(IOrganizationService service, Guid buildingId)
        {
            int optionsetValue;
            string optionsetText = string.Empty;
            try
            {
                if (service != null)
                {
                    QueryExpression timezonequery = new QueryExpression()
                    {
                        EntityName = "smp_timezone",
                        ColumnSet = new ColumnSet("smp_offset")
                    };
                    timezonequery.AddLink("smp_building", "smp_timezoneid", "smp_timezoneid", JoinOperator.Inner)
                        .LinkCriteria.AddCondition("smp_buildingid", ConditionOperator.Equal, buildingId);
                    EntityCollection zoneNames = service.RetrieveMultiple(timezonequery);
                    if (zoneNames.Entities.Count > 0)
                    {
                        optionsetValue = ((OptionSetValue)zoneNames.Entities[0]["smp_offset"]).Value;
                        optionsetText = GetOptionsSetTextOnValue(service, "smp_timezone", "smp_offset", optionsetValue);
                    }
                }

                return optionsetText;
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while getting offset from Time Zone", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Populates the exception log.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The incident id.</param>
        public static void PopulateExceptionLog(Exception ex, IOrganizationService service, Guid incidentId)
        {
            try
            {
                Logger.Write(ex, ExceptionType.SetPriorityFailed, service, "incident", "smp_servicerequestid", incidentId, string.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the options set text on value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <returns>Option Set Value</returns>
        public static string GetOptionsSetTextOnValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
        {
            try
            {
                RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };
                //// Execute the request.
                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);
                //// Access the retrieved attribute.

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
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting time zone offset text.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Gets due date.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="buildingId">The building Id.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns>Returns due date.</returns>
        public static DateTime GetDueDateByBuildingTimeZoneConsideringHolidays(IOrganizationService service, Guid buildingId, DateTime dueDate, int priorityMinutes)
        {
            bool isHoliday = false;
            string startHours = string.Empty;
            string endHours = string.Empty;
            int count = 0;
            int diffMinutes = 0;
            int calculatedMinutes = 0;

            DateTime finalDate = dueDate;
            DateTime submitteddate = dueDate.AddMinutes(-1 * priorityMinutes);

            EntityCollection buildingWorkHoursCollection = GetQueryResponse(service, "smp_buildingworkhours", new string[] { "smp_starthours", "smp_endhours" }, "smp_buildingid", buildingId);
            if (buildingWorkHoursCollection != null && buildingWorkHoursCollection.Entities.Count > 0)
            {
                Entity buildingWorkHours = buildingWorkHoursCollection.Entities[0];
                if (buildingWorkHours != null)
                {
                    if (buildingWorkHours.Attributes.Contains("smp_starthours"))
                    {
                        startHours = buildingWorkHours.FormattedValues["smp_starthours"].ToString();
                    }

                    if (buildingWorkHours.Attributes.Contains("smp_endhours"))
                    {
                        endHours = buildingWorkHours.FormattedValues["smp_endhours"].ToString();
                    }

                    DateTime startdatetime = Convert.ToDateTime(submitteddate.ToShortDateString() + " " + startHours, CultureInfo.CurrentCulture);
                    DateTime enddatetime = Convert.ToDateTime(submitteddate.ToShortDateString() + " " + endHours, CultureInfo.CurrentCulture);

                    EntityCollection buildingCollection = ServiceRequestHelper.GetBuildingDetails(service, buildingId, new string[] { "smp_issaturdayholiday", "smp_issundayholiday" });

                    for (int iterationCount = 1; iterationCount <= priorityMinutes; iterationCount++)
                    {
                        diffMinutes = iterationCount - count;
                        if (startdatetime <= submitteddate.AddMinutes(diffMinutes) && submitteddate.AddMinutes(diffMinutes) <= enddatetime)
                        {
                            if (buildingCollection.Entities.ToList().Where(e => e.Attributes["smp_name"].ToString().ToLower() == submitteddate.DayOfWeek.ToString().ToLower()).FirstOrDefault() != null)
                            {
                                isHoliday = true;
                            }

                            if (IsPublicHoliday(service, buildingId, submitteddate) == true)
                            {
                                isHoliday = true;
                            }

                            if (isHoliday == true)
                            {
                                enddatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + endHours);
                                startdatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + startHours);
                                submitteddate = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + startHours);
                            }
                            else
                            {
                                int balanceMinutes = Convert.ToInt16(Math.Ceiling((enddatetime - submitteddate).TotalMinutes));
                                if ((priorityMinutes - calculatedMinutes) > balanceMinutes)
                                {
                                    startdatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + startHours);
                                    enddatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + endHours);
                                    submitteddate = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + startHours);
                                }
                                else
                                {
                                    submitteddate = submitteddate.AddMinutes(priorityMinutes - calculatedMinutes);
                                }

                                iterationCount = iterationCount + balanceMinutes;
                                calculatedMinutes = calculatedMinutes + balanceMinutes;
                            }

                            count = iterationCount;
                            isHoliday = false;
                            finalDate = submitteddate;
                        }
                        else if (submitteddate.AddMinutes(diffMinutes) > enddatetime)
                        {
                            count = iterationCount;
                            enddatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + endHours);
                            startdatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + startHours);
                            submitteddate = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + startHours);

                            finalDate = submitteddate;
                        }
                        else if (submitteddate.AddMinutes(diffMinutes) < startdatetime)
                        {
                            count = iterationCount;
                            startdatetime = Convert.ToDateTime(submitteddate.ToShortDateString() + " " + startHours);
                            enddatetime = Convert.ToDateTime(submitteddate.ToShortDateString() + " " + endHours);
                            submitteddate = Convert.ToDateTime(submitteddate.ToShortDateString() + " " + startHours);
                            finalDate = submitteddate;
                        }
                    }
                }
            }
            else
            {
                endHours = "00:00";
                EntityCollection buildingCollection = ServiceRequestHelper.GetBuildingDetails(service, buildingId, new string[] { "smp_issaturdayholiday", "smp_issundayholiday" });
                for (int i = 0; i <= 365; i++)
                {
                    if (buildingCollection.Entities.ToList().Where(e => e.Attributes["smp_name"].ToString().ToLower() == submitteddate.DayOfWeek.ToString().ToLower()).FirstOrDefault() != null)
                    {
                        isHoliday = true;
                    }

                    if (IsPublicHoliday(service, buildingId, submitteddate) == true)
                    {
                        isHoliday = true;
                    }

                    if (isHoliday == true)
                    {
                        submitteddate = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + endHours);
                    }
                    else
                    {
                        DateTime enddatetime = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + endHours);
                        int balanceMinutes = Convert.ToInt16(Math.Ceiling((enddatetime - submitteddate).TotalMinutes));
                        if ((priorityMinutes - calculatedMinutes) > balanceMinutes)
                        {
                            submitteddate = Convert.ToDateTime(submitteddate.AddDays(1).ToShortDateString() + " " + endHours);
                        }
                        else
                        {
                            submitteddate = submitteddate.AddMinutes(priorityMinutes - calculatedMinutes);
                            break;
                        }

                        calculatedMinutes = calculatedMinutes + balanceMinutes;
                    }

                    isHoliday = false;
                }

                finalDate = submitteddate;
            }

            return finalDate;
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
        public static EntityCollection GetQueryResponse(IOrganizationService service, string entityLogicalName, string[] fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = entityLogicalName;
                query.ColumnSet = new ColumnSet(fieldsToBeFetched);

                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression(criteriaField, ConditionOperator.Equal, criteriaValue));
                filter.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
                query.Criteria = filter;

                EntityCollection entityCollection = service.RetrieveMultiple(query);

                return entityCollection;
            }

            return null;
        }

        /// <summary>
        /// Gets the holidays.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="buildingId">The building Id.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns>Returns if it is holiday.</returns>
        public static bool IsPublicHoliday(IOrganizationService service, Guid buildingId, DateTime dueDate)
        {
            bool isHoliday = false;
            try
            {
                if (service != null)
                {
                    QueryExpression holidayquery = new QueryExpression()
                    {
                        EntityName = "smp_holiday",
                        ColumnSet = new ColumnSet("smp_date"),
                        Criteria =
                        {
                            Conditions =
                                {
                                    new ConditionExpression("smp_buildingid", ConditionOperator.Equal, buildingId),
                                    new ConditionExpression("smp_date", ConditionOperator.On, dueDate.ToShortDateString()),
                                    new ConditionExpression("statuscode", ConditionOperator.Equal, 1)
                                }
                        }
                    };
                    EntityCollection entities = service.RetrieveMultiple(holidayquery);
                    if (entities.Entities.Count > 0)
                    {
                        isHoliday = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isHoliday;
        }

        /// <summary>
        /// Retrieves the UTC time from local time.
        /// </summary>
        /// <param name="localTime">The local time.</param>
        /// <param name="timeZoneCode">The time zone code.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static DateTime RetrieveUTCTimeFromLocalTime(DateTime localTime, int? timeZoneCode, IOrganizationService service)
        {
            if (!timeZoneCode.HasValue)
            {
                return DateTime.Now;
            }

            var request = new Microsoft.Crm.Sdk.Messages.UtcTimeFromLocalTimeRequest
            {
                TimeZoneCode = timeZoneCode.Value,
                LocalTime = localTime
            };
            var response = (Microsoft.Crm.Sdk.Messages.UtcTimeFromLocalTimeResponse)service.Execute(request);
            return response.UtcTime;
        }

        /// <summary>
        /// Retrieves the local time from UTC time.
        /// </summary>
        /// <param name="utcTime">The UTC time.</param>
        /// <param name="timeZoneCode">The time zone code.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static DateTime RetrieveLocalTimeFromUTCTime(DateTime utcTime, int? timeZoneCode, IOrganizationService service)
        {
            if (!timeZoneCode.HasValue)
            {
                return DateTime.Now;
            }

            var request = new Microsoft.Crm.Sdk.Messages.LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode.Value,
                UtcTime = utcTime.ToUniversalTime()
            };
            var response = (Microsoft.Crm.Sdk.Messages.LocalTimeFromUtcTimeResponse)service.Execute(request);
            return response.LocalTime;
        }

        /// <summary>
        /// Gets the priority hours.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="priorityId">The priority id.</param>
        /// <param name="context">The context.</param>
        /// <param name="incidentId">The incident.</param>
        /// <returns>Get Priority hours.</returns>
        public static int GetPriorityHours(IOrganizationService service, ITracingService tracingService, Guid priorityId)
        {
            int hours = 0;
            try
            {
                if (service != null)
                {
                    Entity priority = service.Retrieve("smp_priority", priorityId, new ColumnSet("smp_noofminutes"));
                    if (priority != null)
                    {
                        hours = priority.Attributes.Contains("smp_noofminutes") ? Convert.ToInt32(priority.Attributes["smp_noofminutes"]) : 0;
                    }
                }

                return hours;
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error while getting priority minutes." + " ----Error Message----" + ex.Message + " -----Stack Trace---- " + ex.StackTrace);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error while getting priority minutes.", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Retrieves the users settings.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static int? RetrieveUsersSettings(IOrganizationService service, Guid userId, int caseorigin, bool portalSubmit, int prevStatusCode, int statusCode, ITracingService trace)
        {
            Entity currentUserSettings = null;
            try
            {
                trace.Trace("caseorigin : " + caseorigin);
                trace.Trace("portalSubmit : " + portalSubmit);
                trace.Trace("statusCode : " + statusCode);
                trace.Trace("prevStatusCode : " + prevStatusCode);
                if ((caseorigin != (int)ServiceRequestEnum.ServiceRequestOrigin.Web && portalSubmit == false) || (caseorigin == (int)ServiceRequestEnum.ServiceRequestOrigin.Web && portalSubmit == false && statusCode != (int)ServiceRequestEnum.StatusCode.Draft) || (portalSubmit == true && prevStatusCode == (int)ServiceRequestEnum.StatusCode.PendingCSRDispatch))
                {
                    currentUserSettings = service.RetrieveMultiple(
                        new QueryExpression("usersettings")
                        {
                            ColumnSet = new ColumnSet("timezonecode"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression("systemuserid", ConditionOperator.Equal, userId)
                                }
                            }
                        }).Entities[0].ToEntity<Entity>();
                }
                else
                {
                    string fromUserDomainName = ServiceRequestHelper.GetConfigurationValue(service, "EmailSenderDomainName");
                    Guid systemId = ServiceRequestHelper.GetCrmUserId(service, fromUserDomainName);
                    currentUserSettings = service.RetrieveMultiple(
                 new QueryExpression("usersettings")
                 {
                     ColumnSet = new ColumnSet("timezonecode"),
                     Criteria = new FilterExpression
                     {
                         Conditions =
                         {
                                new ConditionExpression("systemuserid", ConditionOperator.Equal, systemId)
                         }
                     }
                 }).Entities[0].ToEntity<Entity>();
                }
            }
            catch (Exception e)
            {
                trace.Trace("error");
                trace.Trace("In Exception :" + e.Message);
                throw e;
            }

            return (int?)currentUserSettings.Attributes["timezonecode"];
        }
    }
}