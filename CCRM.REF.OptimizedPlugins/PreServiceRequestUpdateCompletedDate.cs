// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestUpdateCompletedDate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreServiceRequestUpdateCompletedDate Plugin
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

    public class PreServiceRequestUpdateCompletedDate : Plugin
    {
        /// <summary>
        /// The target param
        /// </summary>
        private readonly string targetParam = "Target";
        private readonly string preImageAlias = "PreImage";

        public PreServiceRequestUpdateCompletedDate()
            : base(typeof(PreServiceRequestUpdateCompletedDate))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Update", "incident", new Action<LocalPluginContext>(this.ExecuteCompletedDatebyBuildingTimeZone)));
        }

        public object GetFieldFromTargetOrImage(Entity target, Entity image, string fieldName)
        {
            object rtnValue = null;
            if (target.Contains(fieldName))
            {
                rtnValue = target[fieldName];
            }
            else
            {
                if (image.Attributes.Contains(fieldName))
                {
                    rtnValue = image[fieldName];
                }
            }

            return rtnValue;
        }

        protected void ExecuteCompletedDatebyBuildingTimeZone(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService trace = localContext.TracingService;
            Guid buildingTimeZoneId = Guid.Empty;
            string completedDateOnBuilidingTimeZone = string.Empty;
            DateTime completedDate = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
            if (context != null && context.InputParameters[this.targetParam] != null && context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias))
            {
                trace.Trace("In Update Completion Date");
                var service = localContext.OrganizationService;
                var incident = (Entity)context.InputParameters[this.targetParam];
                Entity preImageEntity = (Entity)context.PreEntityImages[this.preImageAlias];
                try
                {
                    trace.Trace("status Code Value :" + ((OptionSetValue)incident["statuscode"]).Value);
                    if (incident.Attributes.Contains("statuscode") && ((OptionSetValue)incident["statuscode"]).Value == 180620011)
                    {
                        completedDate = DateTime.UtcNow;
                        incident["smp_completeddate"] = completedDate;
                        if (preImageEntity.Attributes.Contains("smp_duedate"))
                        {
                            if (Convert.ToDateTime(incident["modifiedon"]) <= Convert.ToDateTime(preImageEntity["smp_duedate"]))
                            {
                                incident["smp_slamet"] = true;
                            }
                        }

                        object smp_problembuilding = this.GetFieldFromTargetOrImage(incident, preImageEntity, "smp_problembuilding");
                        if (smp_problembuilding != null)
                        {
                            Guid buildingId = ((EntityReference)smp_problembuilding).Id;
                            string timeZoneName = GetTimeZoneNameFromBuilding(service, buildingId);

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
                                    string[] timeSplit = time[1].Split(':');
                                    decimal hourMinutes = Convert.ToDecimal(timeSplit[0], CultureInfo.InvariantCulture) * 60;
                                    decimal minutes = hourMinutes + Convert.ToDecimal(timeSplit[1], CultureInfo.InvariantCulture);
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
                                    completedDateOnBuilidingTimeZone = completedDate.AddMinutes(min).ToString();
                                }
                                else if (symbol == "-")
                                {
                                    string addnegative = "-" + Convert.ToString(min, CultureInfo.InvariantCulture);
                                    double negativeMinutes = Convert.ToDouble(addnegative, CultureInfo.InvariantCulture);
                                    completedDateOnBuilidingTimeZone = completedDate.AddMinutes(negativeMinutes).ToString();
                                }

                                if (!string.IsNullOrEmpty(completedDateOnBuilidingTimeZone))
                                {
                                    DateTime completeDate = Convert.ToDateTime(completedDateOnBuilidingTimeZone);
                                    string compDate = string.Format(CultureInfo.InvariantCulture, string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(completeDate, CultureInfo.CurrentCulture.DateTimeFormat)));
                                    if (incident.Contains("smp_completeddatebybuildingtimezone"))
                                    {
                                        incident["smp_completeddatebybuildingtimezone"] = compDate;
                                    }
                                    else
                                    {
                                        incident.Attributes.Add("smp_completeddatebybuildingtimezone", compDate);
                                    }

                                    object dueDate = this.GetFieldFromTargetOrImage(incident, preImageEntity, "smp_duedatebybuildingtimezone");
                                    if (dueDate != null)
                                    {
                                        if (completeDate <= Convert.ToDateTime(dueDate))
                                        {
                                            if (incident.Contains("smp_isslametbybuildingtimezone"))
                                            {
                                                incident["smp_isslametbybuildingtimezone"] = true;
                                            }
                                            else
                                            {
                                                incident.Attributes.Add("smp_isslametbybuildingtimezone", true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    trace.Trace("Completed Upadte COmpletiondate");
                }
                catch (Exception ex)
                {
                    trace.Trace("In exception :" + ex.Message);
                    CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Exception while updating Completed Date by Building Time Zone.", ex);
                    throw customEx;
                }
            }
        }

        /// <summary>
        /// Gets the Timezone from building.
        /// </summary>
        /// <param name="service"> Organization Service.</param>
        /// <param name="buildingId">Building Guid.</param>
        /// <param name="incidentId">Incident Guid.</param>
        /// <returns></returns>
        private static string GetTimeZoneNameFromBuilding(IOrganizationService service, Guid buildingId)
        {
            string rtnValue = string.Empty;
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
                OptionSetValue optionSetValue = zoneNames.Entities[0].GetAttributeValue<OptionSetValue>("smp_offset");
                rtnValue = GetOptionsSetTextOnValue(service, "smp_timezone", "smp_offset", optionSetValue.Value);
            }

            return rtnValue;
        }

        /// <summary>
        /// Gets the options set text on value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <returns>Option Set Value</returns>
        private static string GetOptionsSetTextOnValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
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
    }
}
