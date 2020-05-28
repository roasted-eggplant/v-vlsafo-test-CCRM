// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestUpdate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreServiceRequestUpdate Plugin
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

    /// <summary>
    /// PreServiceRequestUpdate Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PreServiceRequestUpdate : Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PreServiceRequestUpdate"/> class.
        /// </summary>
        public PreServiceRequestUpdate()
            : base(typeof(PreServiceRequestUpdate))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Update", "incident", new Action<LocalPluginContext>(this.ExecutePreServiceRequestUpdate)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        public Entity GetBuildingDetails(IOrganizationService service, Guid buildingId, ITracingService trace)
        {
            try
            {
                trace.Trace("in Retrieve Building Details");
                Entity result = service.Retrieve("smp_building", buildingId, new ColumnSet("smp_addressline1", "smp_addressline2", "smp_state", "smp_country", "smp_city", "smp_zipcode", "smp_timezoneid"));
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
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
                if (image != null)
                {
                    if (image.Contains(fieldName))
                    {
                        rtnValue = image[fieldName];
                    }
                }
            }

            return rtnValue;
        }

        public Entity GetRequestorDetails(IOrganizationService service, Guid requestorId, ITracingService trace)
        {
            Entity entity = null;
            try
            {
                trace.Trace("in Retrieve Requestor Details");
                return entity = service.Retrieve("contact", requestorId, new ColumnSet("emailaddress1", "smp_alias", "telephone1", "smp_buildingid", "smp_roomnumber"));
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
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
        protected void ExecutePreServiceRequestUpdate(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService trace = localContext.TracingService;
            var service = localContext.OrganizationService;
            Entity serviceRequest = null;

            if (context != null)
            {
                if (context.InputParameters != null && context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    serviceRequest = context.InputParameters["Target"] as Entity;
                    if (serviceRequest.Attributes.Contains("statuscode"))
                    {
                        int statusCode = ((OptionSetValue)serviceRequest.Attributes["statuscode"]).Value;
                        trace.Trace("Current Status Code is :" + statusCode);
                        if (statusCode != (int)ServiceRequestEnum.StatusCode.Open && statusCode != (int)ServiceRequestEnum.StatusCode.Draft)
                        {
                            trace.Trace("Current Status Code is :" + statusCode + " hence aborting!!!!!!");
                            return;
                        }
                    }
                }

                var preImageIncident = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
                if (service != null && preImageIncident != null)
                {
                    serviceRequest["title"] = preImageIncident.Attributes.Contains("ticketnumber") ? preImageIncident["ticketnumber"].ToString() : string.Empty;
                    object statusCodeAttribute = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, CRMAttributesResource.StatusCodeAttribute);
                    Guid buildingId = preImageIncident.Attributes.Contains("smp_problembuilding") ? ((EntityReference)preImageIncident["smp_problembuilding"]).Id : ((EntityReference)preImageIncident["smp_problembuilding"]).Id;
                    Guid priorityId = preImageIncident.Attributes.Contains("smp_priorityid") ? ((EntityReference)preImageIncident["smp_priorityid"]).Id : ((EntityReference)preImageIncident["smp_priorityid"]).Id;
                    object smp_contact = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, "smp_contact");
                    object caseOriginCode = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, "caseorigincode");
                    if (statusCodeAttribute != null)
                    {
                        try
                        {
                            object smp_portalsubmit = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, "smp_portalsubmit");
                            ////As part of UserStory:3882364 code changes implemented
                            if (((OptionSetValue)statusCodeAttribute).Value == (int)ServiceRequestEnum.StatusCode.Draft || ((OptionSetValue)statusCodeAttribute).Value == (int)ServiceRequestEnum.StatusCode.Open)
                            {
                                string timeZoneName = string.Empty;
                                DateTime submittedDate = DateTime.Now;
                                string submittedDateOnBuilidingTimeZone = string.Empty;
                                DateTime recordsubmitted = Convert.ToDateTime(DateTime.UtcNow, CultureInfo.CurrentCulture);
                                DateTime dueDateOnBuilidingTimeZone;
                                bool portalSubmit = (bool)smp_portalsubmit;
                                string symbol = string.Empty;
                                double min = 0;

                                //// As part of bug 3736044: code changes implemented.
                                trace.Trace("Submitteddatetime updated in CRM/Portal");
                                serviceRequest["smp_submitteddatetime"] = recordsubmitted;
                                if ((Guid)buildingId != Guid.Empty)
                                {
                                    string[] timeZoneDetails = TimeZoneHelper.GetTimeZoneIdFromBuilding(service, (Guid)buildingId, serviceRequest.Id).ToString().Split('@');
                                    Guid timeZoneId = new Guid(timeZoneDetails[0]);
                                    string finalcialStateCampus = timeZoneDetails[1].ToString();
                                    if (timeZoneId != Guid.Empty)
                                    {
                                        timeZoneName = TimeZoneHelper.GetTimeZoneNameFromTimeZone(service, (Guid)buildingId);
                                        if (!string.IsNullOrEmpty(timeZoneName))
                                        {
                                            string[] time = new string[0];
                                            string offset = timeZoneName;
                                            symbol = offset.Substring(0, 1);
                                            if (symbol == "+")
                                            {
                                                time = offset.Split('+');
                                            }
                                            else if (symbol == "-")
                                            {
                                                time = offset.Split('-');
                                            }

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
                                                submittedDateOnBuilidingTimeZone = submittedDate.AddMinutes(min).ToString();
                                            }
                                            else if (symbol == "-")
                                            {
                                                string addNegative = "-" + Convert.ToString(min, CultureInfo.InvariantCulture);
                                                double negativeMinutes = Convert.ToDouble(addNegative, CultureInfo.InvariantCulture);
                                                submittedDateOnBuilidingTimeZone = submittedDate.AddMinutes(negativeMinutes).ToString();
                                            }

                                            serviceRequest["smp_submitteddatetimebybuildingtimezone"] = string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(submittedDateOnBuilidingTimeZone, CultureInfo.CurrentCulture.DateTimeFormat));
                                        }
                                    }
                                }
                                ////As part of Bug 3965758 code changes implemented
                                int priorityHours = TimeZoneHelper.GetPriorityHours(service, trace, priorityId);
                                if (priorityHours != 0)
                                {
                                    int priorityMinutes = Convert.ToInt32(priorityHours, CultureInfo.CurrentCulture);
                                    dueDateOnBuilidingTimeZone = Convert.ToDateTime(submittedDateOnBuilidingTimeZone, CultureInfo.CurrentCulture).AddMinutes(Convert.ToDouble(Convert.ToString(priorityMinutes, CultureInfo.InvariantCulture), CultureInfo.CurrentCulture));
                                    dueDateOnBuilidingTimeZone = TimeZoneHelper.GetDueDateByBuildingTimeZoneConsideringHolidays(service, buildingId, dueDateOnBuilidingTimeZone, priorityMinutes);
                                    serviceRequest["smp_duedatebybuildingtimezone"] = string.Format(CultureInfo.InvariantCulture, "{0:MM/d/yyyy h:mm tt}", Convert.ToDateTime(dueDateOnBuilidingTimeZone, CultureInfo.CurrentCulture.DateTimeFormat));

                                    if (symbol == "-")
                                    {
                                        dueDateOnBuilidingTimeZone = dueDateOnBuilidingTimeZone.AddMinutes(min);
                                    }
                                    else if (symbol == "+")
                                    {
                                        string addNegative = "-" + Convert.ToString(min, CultureInfo.InvariantCulture);
                                        double negativeMinutes = Convert.ToDouble(addNegative, CultureInfo.InvariantCulture);
                                        dueDateOnBuilidingTimeZone = dueDateOnBuilidingTimeZone.AddMinutes(negativeMinutes);
                                    }

                                    int? runningUserTimeZoneCode = TimeZoneHelper.RetrieveUsersSettings(service, context.UserId, ((OptionSetValue)caseOriginCode).Value, portalSubmit, ((OptionSetValue)preImageIncident.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value, ((OptionSetValue)statusCodeAttribute).Value, trace);
                                    DateTime dueUserTimeZone = TimeZoneHelper.RetrieveLocalTimeFromUTCTime(dueDateOnBuilidingTimeZone, runningUserTimeZoneCode, service);
                                    serviceRequest["smp_duedate"] = Convert.ToDateTime(dueUserTimeZone, CultureInfo.CurrentCulture);
                                    serviceRequest["smp_duedatetimebybuildingtimezone"] = Convert.ToDateTime(dueUserTimeZone, CultureInfo.CurrentCulture);
                                }
                                else
                                {
                                    serviceRequest["smp_duedatebybuildingtimezone"] = null;
                                    serviceRequest["smp_duedatetimebybuildingtimezone"] = null;
                                    serviceRequest["smp_duedate"] = null;
                                }

                                var prevBuildingId = this.GetAttributeValue(CRMAttributesResource.ProblemBuildingIdAttribute, preImageIncident);
                                var prevProblemClassId = this.GetAttributeValue(CRMAttributesResource.ProblemClassIdAttribute, preImageIncident);
                                var prevproblemTypeId = this.GetAttributeValue(CRMAttributesResource.ProblemTypeIdAttribute, preImageIncident);
                                var prevStatusCode = ((OptionSetValue)preImageIncident.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value;
                                var recentBuildingId = this.GetAttributeValue(CRMAttributesResource.ProblemBuildingIdAttribute, serviceRequest);
                                var recentProblemClassId = this.GetAttributeValue(CRMAttributesResource.ProblemClassIdAttribute, serviceRequest);
                                var recentProblemTypeId = this.GetAttributeValue(CRMAttributesResource.ProblemTypeIdAttribute, serviceRequest);
                                recentBuildingId = recentBuildingId == Guid.Empty ? prevBuildingId : recentBuildingId;
                                recentProblemClassId = recentProblemClassId == Guid.Empty ? prevProblemClassId : recentProblemClassId;
                                recentProblemTypeId = recentProblemTypeId == Guid.Empty ? prevproblemTypeId : recentProblemTypeId;
                                if (prevStatusCode == (int)ServiceRequestEnum.StatusCode.Draft && (prevBuildingId != recentBuildingId || prevproblemTypeId != recentProblemTypeId || prevProblemClassId != recentProblemClassId))
                                {
                                    if (preImageIncident.Attributes.Contains(CRMAttributesResource.CustomerIdAttribute))
                                    {
                                        this.AssignProviderToServiceRequest(context, service, serviceRequest, recentBuildingId, recentProblemClassId, recentProblemTypeId, trace);
                                    }
                                }
                            }

                            if (smp_portalsubmit != null && ((OptionSetValue)statusCodeAttribute).Value == (int)ServiceRequestEnum.StatusCode.Draft && (((OptionSetValue)caseOriginCode).Value == (int)ServiceRequestEnum.ServiceRequestOrigin.Web || ((OptionSetValue)caseOriginCode).Value == (int)ServiceRequestEnum.ServiceRequestOrigin.IOT || ((OptionSetValue)caseOriginCode).Value == (int)ServiceRequestEnum.ServiceRequestOrigin.Phone))
                            {
                                if (smp_contact != null)
                                {
                                    object smp_requestorid = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, "smp_requestorid");
                                    if (smp_requestorid != null)
                                    {
                                        trace.Trace("In Requestor Details");
                                        Entity requestor = this.GetRequestorDetails(service, ((EntityReference)smp_requestorid).Id, trace);
                                        trace.Trace("In Requestor assign");
                                        if (!preImageIncident.Attributes.Contains("smp_requestorphone"))
                                        {
                                            if (requestor.Attributes.Contains("telephone1"))
                                            {
                                                serviceRequest["smp_requestorphone"] = requestor["telephone1"].ToString();
                                            }
                                            else if (((OptionSetValue)caseOriginCode).Value == (int)ServiceRequestEnum.ServiceRequestOrigin.Web)
                                            {
                                                serviceRequest["smp_requestorphone"] = "<None>";
                                            }
                                        }

                                        serviceRequest["smp_requestoralias"] = requestor.Attributes.Contains("smp_alias") ? requestor["smp_alias"].ToString() : string.Empty;
                                        serviceRequest["smp_requestoremail"] = requestor.Attributes.Contains("emailaddress1") ? requestor["emailaddress1"].ToString() : string.Empty;
                                        serviceRequest["smp_buildingid"] = requestor.Attributes.Contains("smp_buildingid") ? new EntityReference("smp_building", ((EntityReference)requestor["smp_buildingid"]).Id) : null;
                                        serviceRequest["new_requestorroomnumber"] = requestor.Attributes.Contains("new_requestorroomnumber") ? new EntityReference("smp_room", ((EntityReference)requestor["new_requestorroomnumber"]).Id) : null;
                                    }

                                    trace.Trace("Getting Other room");
                                    Entity otherRef = this.GetRoomWithOther(service, trace);
                                    trace.Trace("Getting Problem Room lookup");
                                    object room = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, "new_problemroomnumber");
                                    string roomName = room != null ? this.GetRoomName(service, ((EntityReference)room).Id) : string.Empty;
                                    trace.Trace("Comparing room values");
                                    if (string.IsNullOrEmpty(roomName))
                                    {
                                        trace.Trace("Problem room lookup is null, setting to " + (otherRef == null ? "null" : "Other"));
                                        serviceRequest["new_problemroomnumber"] = new EntityReference(otherRef.LogicalName, otherRef.Id);
                                    }
                                    else if (roomName != otherRef["smp_name"].ToString())
                                    {
                                        trace.Trace("Problem room lookup is not set to Other, copying value to text: " + roomName);
                                        serviceRequest["smp_problemroom"] = roomName;
                                    }

                                    if (serviceRequest.Attributes.Contains("smp_problembuilding") || preImageIncident.Attributes.Contains("smp_problembuilding"))
                                    {
                                        object smp_problembuilding = this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, "smp_problembuilding");
                                        if (smp_problembuilding != null)
                                        {
                                            Entity building = this.GetBuildingDetails(service, ((EntityReference)smp_problembuilding).Id, trace);
                                            if (building != null)
                                            {
                                                serviceRequest["smp_problembuildingaddressline1"] = building.Attributes.Contains("smp_addressline1") ? building["smp_addressline1"].ToString() : string.Empty;
                                                serviceRequest["smp_problembuildingaddressline2"] = building.Attributes.Contains("smp_addressline2") ? building["smp_addressline2"].ToString() : string.Empty;
                                                serviceRequest["smp_problembuildingstate"] = building.Attributes.Contains("smp_state") ? building["smp_state"].ToString() : string.Empty;
                                                serviceRequest["smp_problembuildingcountry"] = building.Attributes.Contains("smp_country") ? building["smp_country"].ToString() : string.Empty;
                                                serviceRequest["smp_problembuildingcity"] = building.Attributes.Contains("smp_city") ? building["smp_city"].ToString() : string.Empty;
                                                serviceRequest["smp_problembuildingzipcode"] = building.Attributes.Contains("smp_zipcode") ? building["smp_zipcode"].ToString() : string.Empty;
                                                serviceRequest["smp_problembuildingtimezone"] = building.Attributes.Contains("smp_timezoneid") ? new EntityReference("smp_timezone", ((EntityReference)building["smp_timezoneid"]).Id) : null;
                                            }
                                        }
                                    }

                                    if (((bool)smp_portalsubmit) == true)
                                    {
                                        serviceRequest["statuscode"] = new OptionSetValue(2);
                                        this.CheckNewProvider(context, service, preImageIncident, serviceRequest, trace);
                                    }
                                    else
                                    {
                                        //// Code Added by Sharatveer G to set Default provider for Copy SR from Portal
                                        this.CheckNewProvider(context, service, preImageIncident, serviceRequest, trace);
                                    }
                                }

                                trace.Trace("Completion of PreUpdate SR");
                            }

                            ////Provider will only be set if status is not draft
                            if (((OptionSetValue)statusCodeAttribute).Value != (int)ServiceRequestEnum.StatusCode.Draft && ((OptionSetValue)caseOriginCode).Value != (int)ServiceRequestEnum.ServiceRequestOrigin.Web)
                            {
                                if (preImageIncident.Attributes.Contains("new_problemroomnumber"))
                                {
                                    string roomName = this.GetRoomName(service, ((EntityReference)preImageIncident["new_problemroomnumber"]).Id);
                                    trace.Trace(roomName);
                                    if (roomName != "Other" && !string.IsNullOrEmpty(roomName))
                                    {
                                        serviceRequest["smp_problemroom"] = roomName;
                                    }
                                }

                                ////Setting the Approval manager & Pending Approval Status
                                if (smp_contact != null && (bool)this.GetFieldFromTargetOrImage(serviceRequest, preImageIncident, CRMAttributesResource.ServiceRequestIsApprovalRequired))
                                {
                                    this.AssignApprovalManager(service, serviceRequest, preImageIncident, ((EntityReference)smp_contact).Id);
                                    if (((OptionSetValue)statusCodeAttribute).Value == (int)ServiceRequestEnum.StatusCode.Open)
                                    {
                                        serviceRequest["statuscode"] = new OptionSetValue(3);
                                    }
                                }

                                this.CheckNewProvider(context, service, preImageIncident, serviceRequest, trace);
                            }
                        }
                        catch (CustomServiceManagementPortalException)
                        {
                        }
                    }
                }
            }
        }

        private string GetRoomName(IOrganizationService service, Guid roomId)
        {
            Entity result = service.Retrieve("smp_room", roomId, new ColumnSet("smp_name"));
            string name = (result != null && result.Attributes.Contains("smp_name")) ? result["smp_name"].ToString() : string.Empty;
            return name;
        }

        private Entity GetRoomWithOther(IOrganizationService service, ITracingService trace)
        {
            trace.Trace("In GetRoomWithOther");
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='smp_room'>
                                                            <attribute name='smp_roomid' />
                                                            <attribute name='smp_name' />
                                                            <attribute name='createdon' />
                                                            <order attribute='smp_name' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='smp_name' operator='eq' value='Other' />
                                                            </filter>
                                                          </entity>
                                                        </fetch>";
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            if (result != null && result.Entities != null && result.Entities.Count > 0)
            {
                trace.Trace($"Found {result.Entities.Count} matching Other rooms");
                Entity otherRoom = result.Entities.First();
                return otherRoom;
            }

            trace.Trace("Other room not found");
            return null;
        }

        /// <summary>
        /// Checks for the new provider.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="preImageIncident">The pre image incident.</param>
        /// <param name="serviceRequest">The Context.</param>
        /// <exception cref="CustomServiceManagementPortalException">Failed to Check new provider</exception>
        private void CheckNewProvider(IPluginExecutionContext context, IOrganizationService service, Entity preImageIncident, Entity serviceRequest, ITracingService trace)
        {
            try
            {
                trace.Trace("In CheckNewProvider ");
                var prevBuildingId = this.GetAttributeValue(CRMAttributesResource.ProblemBuildingIdAttribute, preImageIncident);
                var prevProblemClassId = this.GetAttributeValue(CRMAttributesResource.ProblemClassIdAttribute, preImageIncident);
                var prevproblemTypeId = this.GetAttributeValue(CRMAttributesResource.ProblemTypeIdAttribute, preImageIncident);
                var prevStatusCode = ((OptionSetValue)preImageIncident.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value;
                ////Populate the recent data
                var recentBuildingId = this.GetAttributeValue(CRMAttributesResource.ProblemBuildingIdAttribute, serviceRequest);
                var recentProblemClassId = this.GetAttributeValue(CRMAttributesResource.ProblemClassIdAttribute, serviceRequest);
                var recentProblemTypeId = this.GetAttributeValue(CRMAttributesResource.ProblemTypeIdAttribute, serviceRequest);
                recentBuildingId = recentBuildingId == Guid.Empty ? prevBuildingId : recentBuildingId;
                recentProblemClassId = recentProblemClassId == Guid.Empty ? prevProblemClassId : recentProblemClassId;
                recentProblemTypeId = recentProblemTypeId == Guid.Empty ? prevproblemTypeId : recentProblemTypeId;
                if (prevStatusCode == (int)ServiceRequestEnum.StatusCode.Draft && (prevBuildingId != recentBuildingId || prevproblemTypeId != recentProblemTypeId || prevProblemClassId != recentProblemClassId))
                {
                    trace.Trace("In Context AttributeCheck ");
                    if (preImageIncident.Attributes.Contains(CRMAttributesResource.CustomerIdAttribute))
                    {
                        this.AssignProviderToServiceRequest(context, service, serviceRequest, recentBuildingId, recentProblemClassId, recentProblemTypeId, trace);
                    }

                    if (preImageIncident.Attributes.Contains("smp_createdfrom") && ((OptionSetValue)preImageIncident.Attributes["smp_createdfrom"]).Value != 2 && ((OptionSetValue)preImageIncident.Attributes["smp_createdfrom"]).Value != 3 && Convert.ToBoolean(preImageIncident.Attributes["smp_portalsubmit"]) != false)
                    {
                        trace.Trace("Before FetchAllProblemTypeDescriptionAndAssign");
                        this.FetchAllProblemTypeDescriptionAndAssign(service, recentBuildingId, recentProblemTypeId, recentProblemClassId, serviceRequest.Id, trace);
                    }
                }
                else if (serviceRequest.Attributes.Contains(CRMAttributesResource.IoCode) == false || string.IsNullOrEmpty(Convert.ToString(serviceRequest.Attributes[CRMAttributesResource.IoCode], CultureInfo.InvariantCulture)))
                {
                    trace.Trace("In IOcode AttributeCheck ");
                    this.SetIoCodeToServiceRequest(service, serviceRequest);
                }
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed to Check new provider", ex.InnerException);
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="imageIncident">The pre image incident.</param>
        /// <returns>The GUID of the attribute</returns>
        private Guid GetAttributeValue(string attributeName, Entity imageIncident)
        {
            if (imageIncident.Attributes.Contains(attributeName))
            {
                return ((EntityReference)imageIncident.Attributes[attributeName]).Id;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Fetches all problem type description and assign.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="recentBuildingId">The recent building id.</param>
        /// <param name="recentProblemTypeId">The recent problem type id.</param>
        /// <param name="recentProblemClassId">The recent problem class id.</param>
        /// <param name="serviceRequestId">The service request id.</param>
        /// <exception cref="CustomServiceManagementPortalException">Throw the custom exception</exception>
        private void FetchAllProblemTypeDescriptionAndAssign(IOrganizationService service, Guid recentBuildingId, Guid recentProblemTypeId, Guid recentProblemClassId, Guid serviceRequestId, ITracingService trace)
        {
            try
            {
                if (recentBuildingId != Guid.Empty && recentProblemClassId != Guid.Empty && recentProblemTypeId != Guid.Empty)
                {
                    ConditionExpression condBuilding = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.BuildingIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { recentBuildingId }
                    };
                    ConditionExpression condProblemClass = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemClassIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { recentProblemClassId }
                    };
                    ConditionExpression condProblemType = new ConditionExpression
                    {
                        AttributeName = "smp_problemtype",
                        Operator = ConditionOperator.Equal,
                        Values = { recentProblemTypeId }
                    };
                    QueryExpression problemTypeDescQuery = new QueryExpression
                    {
                        EntityName = CRMAttributesResource.ProblemTypeDescriptionEntityName,
                        ColumnSet = new ColumnSet(CRMAttributesResource.ProblemTypeDescriptionIdAttribute),
                        Criteria =
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions = { condBuilding, condProblemClass, condProblemType }
                    }
                    };
                    if (service != null)
                    {
                        EntityCollection collection = service.RetrieveMultiple(problemTypeDescQuery);
                        foreach (Entity problemTypeDesc in collection.Entities)
                        {
                            if (problemTypeDesc.Attributes.Contains(CRMAttributesResource.ProblemTypeDescriptionIdAttribute))
                            {
                                Entity serviceRequestDynamicProblemType = new Entity(CRMAttributesResource.ServiceRequestProblemTypeEntity);
                                serviceRequestDynamicProblemType[CRMAttributesResource.ServiceRequestIdAttribute] = new EntityReference(CRMAttributesResource.IncidentEntity, serviceRequestId);
                                serviceRequestDynamicProblemType[CRMAttributesResource.ProblemTypeDescriptionIdAttribute] = new EntityReference(CRMAttributesResource.ProblemTypeDescriptionEntityName, new Guid(problemTypeDesc.Attributes[CRMAttributesResource.ProblemTypeDescriptionIdAttribute].ToString()));
                                service.Create(serviceRequestDynamicProblemType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed to fetch Problem Type Description", ex.InnerException);
            }
        }

        /// <summary>
        /// Assigns the provider to service request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">Target.</param>
        /// <param name="preImageIncident">PreImage.</param>
        /// <exception cref="CustomServiceManagementPortalException">Failed to assign Provider with Service Request</exception>
        private void AssignProviderToServiceRequest(IPluginExecutionContext context, IOrganizationService service, Entity serviceRequest, Guid problemBuilding, Guid problemClass, Guid problemType, ITracingService trace)
        {
            try
            {
                if ((problemBuilding != Guid.Empty) && (problemClass != Guid.Empty) && (problemType != Guid.Empty))
                {
                    trace.Trace("In AssignProvider ");
                    ConditionExpression condBuilding = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.BuildingIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { problemBuilding }
                    };
                    ConditionExpression condProblemClass = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemClassIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { problemClass }
                    };
                    ConditionExpression condProblemType = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemTypeIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { problemType }
                    };
                    ConditionExpression condStatus = new ConditionExpression
                    {
                        AttributeName = "statuscode",
                        Operator = ConditionOperator.Equal,
                        Values = { 1 }
                    };
                    QueryExpression providerMatrixQuery = new QueryExpression
                    {
                        EntityName = CRMAttributesResource.ProviderMatrixEntity,
                        ColumnSet = new ColumnSet(CRMAttributesResource.PrimaryProviderIdAttribute, CRMAttributesResource.ProviderMatrixIsApprovalRequired, CRMAttributesResource.ProviderMatrixIOCode),
                        Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = { condBuilding, condProblemClass, condProblemType, condStatus }
                        }
                    };
                    if (service != null)
                    {
                        EntityCollection providerMatrix_ = service.RetrieveMultiple(providerMatrixQuery);
                        Entity providerMatrix = providerMatrix_.Entities.Count > 0 ? providerMatrix_.Entities.First() : null;
                        if (providerMatrix != null && providerMatrix.Attributes.Contains(CRMAttributesResource.PrimaryProviderIdAttribute))
                        {
                            trace.Trace("In Provider Matrix");
                            bool approvalRequired = providerMatrix.GetAttributeValue<bool>(CRMAttributesResource.ProviderMatrixIsApprovalRequired);
                            string iocode = string.Empty;
                            object objIoCode = this.GetFieldFromTargetOrImage(serviceRequest, null, CRMAttributesResource.IoCode);
                            iocode = objIoCode == null ? providerMatrix.GetAttributeValue<string>(CRMAttributesResource.ProviderMatrixIOCode) : objIoCode.ToString();
                            if (this.ProviderIsActive(((EntityReference)providerMatrix.Attributes[CRMAttributesResource.PrimaryProviderIdAttribute]).Id, service, trace))
                            {
                                this.AssignProvider(context, ((EntityReference)providerMatrix.Attributes[CRMAttributesResource.PrimaryProviderIdAttribute]).Id, service, serviceRequest, false, approvalRequired, iocode, trace);
                                trace.Trace("Provider assigned");
                            }
                            else
                            {
                                this.ResetProvider(context, service, serviceRequest, trace);
                                var message = "Provider not set. No active Provider is available with specified inputs.";
                                Logger.Write(message, ExceptionType.SetProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, string.Empty);
                            }
                        }
                        else
                        {
                            this.ResetProvider(context, service, serviceRequest, trace);
                            var message = "Provider not set. Provider is not available in Provider Matrix with specified inputs.";
                            Logger.Write(message, ExceptionType.SetProviderFailed, service, "incident", "smp_servicerequestid", serviceRequest.Id, string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.PopulateExceptionLog(ex, service, serviceRequest.Id);
                throw new CustomServiceManagementPortalException("Failed to assign Provider with Service Request", ex);
            }
        }

        /// <summary>
        /// Sets io code for service request
        /// </summary>
        /// <param name="service">organization service</param>
        /// <param name="objIncident">incident object</param>
        private void SetIoCodeToServiceRequest(IOrganizationService service, Entity objIncident)
        {
            try
            {
                ////Check if building, roomtype, floor, problem class and problem type is not null. To be added, floor
                if (objIncident.Attributes.Contains(CRMAttributesResource.ProblemBuildingIdAttribute) && objIncident.Attributes.Contains(CRMAttributesResource.ProblemClassIdAttribute) && objIncident.Attributes.Contains(CRMAttributesResource.ProblemTypeIdAttribute))
                {
                    ConditionExpression condBuilding = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.BuildingIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)objIncident.Attributes[CRMAttributesResource.ProblemBuildingIdAttribute]).Id }
                    };
                    ConditionExpression condProblemClass = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemClassIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)objIncident.Attributes[CRMAttributesResource.ProblemClassIdAttribute]).Id }
                    };
                    ConditionExpression condProblemType = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemTypeIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)objIncident.Attributes[CRMAttributesResource.ProblemTypeIdAttribute]).Id }
                    };
                    ConditionExpression condStatus = new ConditionExpression
                    {
                        AttributeName = "statuscode",
                        Operator = ConditionOperator.Equal,
                        Values = { 1 }
                    };
                    QueryExpression providerMatrixQuery = new QueryExpression
                    {
                        EntityName = CRMAttributesResource.ProviderMatrixEntity,
                        ColumnSet = new ColumnSet(CRMAttributesResource.PrimaryProviderIdAttribute, CRMAttributesResource.ProviderMatrixIsApprovalRequired, CRMAttributesResource.ProviderMatrixIOCode),
                        Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = { condBuilding, condProblemClass, condProblemType, condStatus }
                        }
                    };

                    if (service != null)
                    {
                        EntityCollection providerMatrix_ = service.RetrieveMultiple(providerMatrixQuery);
                        Entity providerMatrix = providerMatrix_.Entities.Count > 0 ? providerMatrix_.Entities.First() : null;
                        if (providerMatrix != null && providerMatrix.Attributes.Contains(CRMAttributesResource.PrimaryProviderIdAttribute))
                        {
                            string iocode = string.Empty;

                            if (providerMatrix.Attributes.Contains(CRMAttributesResource.ProviderMatrixIOCode))
                            {
                                iocode = Convert.ToString(providerMatrix.Attributes[CRMAttributesResource.ProviderMatrixIOCode], CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                iocode = string.Empty;
                            }

                            objIncident[CRMAttributesResource.IoCode] = iocode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.PopulateExceptionLog(ex, service, objIncident.Id);
                throw new CustomServiceManagementPortalException("Failed to assign Io code to Service Request", ex);
            }
        }

        /// <summary>
        /// Whether the provider is active
        /// </summary>
        /// <param name="providerId">The provider id</param>
        /// <param name="service">The service</param>
        /// <returns>returns Whether the provider is active</returns>
        private bool ProviderIsActive(Guid providerId, IOrganizationService service, ITracingService trace)
        {
            trace.Trace("In ProviderIsActive");
            bool result = false;
            if (service != null)
            {
                Entity provider = service.Retrieve(CRMAttributesResource.AccountEntity, providerId, new ColumnSet("statecode"));
                if (provider != null)
                {
                    result = (provider.Attributes.Contains("statecode") && ((OptionSetValue)provider["statecode"]).Value == 0) ? true : false;
                }
            }

            trace.Trace("Out ProviderIsActive");
            return result;
        }

        /// <summary>
        /// Resets the provider if there is no matching criteria in provider matrix.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The incident.</param>
        private void ResetProvider(IPluginExecutionContext context, IOrganizationService service, Entity serviceRequest, ITracingService trace)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='account'>
                                                            <attribute name='name' />
                                                            <attribute name='accountid' />
                                                            <order attribute='name' descending='false' />
                                                            <order attribute='address1_stateorprovince' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='name' operator='eq' value='Not Assigned' />
                                                            </filter>
                                                            <link-entity name='contact' from='contactid' to='primarycontactid' visible='false' link-type='outer' alias='a_410707b195544cd984376608b1802904'>
                                                              <attribute name='emailaddress1' />
                                                            </link-entity>
                                                          </entity>
                                                        </fetch>";
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            if (result != null)
            {
                this.AssignProvider(context, result.Entities[0].Id, service, serviceRequest, true, false, string.Empty, trace);
            }
        }

        /// <summary>
        /// Logs the not all attributes populated.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="objectIncident">The object incident.</param>
        ////private void LogNotAllAttributesPopulated(IOrganizationService service, Entity objectIncident)
        ////{
        ////    var message = "Provider was not set.";
        ////    bool anyAttributeIsMissing = false;
        ////    if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemBuildingIdAttribute))
        ////    {
        ////        message = "Building, ";
        ////        anyAttributeIsMissing = true;
        ////    }

        ////    if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemRoomTypeIdAttribute))
        ////    {
        ////        message = message + "Problem Room, ";
        ////        anyAttributeIsMissing = true;
        ////    }

        ////    if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemClassIdAttribute))
        ////    {
        ////        message = message + "Problem Class, ";
        ////        anyAttributeIsMissing = true;
        ////    }

        ////    if (!objectIncident.Attributes.Contains(CRMAttributesResource.ProblemTypeIdAttribute))
        ////    {
        ////        message = message + "Problem Type";
        ////        anyAttributeIsMissing = true;
        ////    }

        ////    message = message.TrimEnd(',');
        ////    message = message + " is missing.";
        ////    if (anyAttributeIsMissing)
        ////    {
        ////        Logger.Write(message, ExceptionType.SetProviderFailed, service, "incident", "smp_servicerequestid", objectIncident.Id, string.Empty);
        ////    }
        ////}

        /// <summary>
        /// Assigns the provider.
        /// </summary>
        /// <param name="primaryProviderId">The primary provider id.</param>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The object incident.</param>
        /// <param name="isDefaultProvider">if set to <c>true</c> [is default provider].</param>
        /// <param name="isApprovalRequired">if set to <c>true</c> [is approval required].</param>
        /// <param name="iocode">The ioCode</param>
        private void AssignProvider(IPluginExecutionContext context, Guid primaryProviderId, IOrganizationService service, Entity serviceRequest, bool isDefaultProvider, bool isApprovalRequired, string iocode, ITracingService trace)
        {
            trace.Trace("Before AssignProvider");
            serviceRequest[CRMAttributesResource.CustomerIdAttribute] = new EntityReference(CRMAttributesResource.AccountEntity, primaryProviderId);
            serviceRequest[CRMAttributesResource.ServiceRequestIsApprovalRequired] = isApprovalRequired;
            if (!serviceRequest.Attributes.Contains(CRMAttributesResource.ServiceRequestIOCode))
            {
                serviceRequest[CRMAttributesResource.ServiceRequestIOCode] = iocode;
            }

            trace.Trace("Before AssignTeamAsOwner");
            this.AssignTeamAsOwner(context, primaryProviderId, service, serviceRequest, isDefaultProvider, trace);
        }

        /// <summary>
        /// Assigns the Team with the service request.
        /// </summary>
        /// <param name="primaryProviderId">The primary provider id.</param>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The object incident.</param>
        /// <param name="isDefaultProvider">if set to <c>true</c> [is default provider].</param>
        /// <exception cref="CustomServiceManagementPortalException">Failed to assign Team as owner</exception>
        private void AssignTeamAsOwner(IPluginExecutionContext context, Guid primaryProviderId, IOrganizationService service, Entity serviceRequest, bool isDefaultProvider, ITracingService trace)
        {
            try
            {
                if (!isDefaultProvider)
                {
                    trace.Trace("Not Default Provider");
                    this.AssignTeamForNewProvider(primaryProviderId, service, serviceRequest, trace);
                }
                else
                {
                    trace.Trace("In Default Provider");
                    this.AssignUserForDefaultProvider(context, service, serviceRequest);
                }
            }
            catch (Exception ex)
            {
                this.PopulateExceptionLog(ex, service, serviceRequest.Id);
                throw new CustomServiceManagementPortalException("Failed to assign Team as owner", ex.InnerException);
            }
        }

        /// <summary>
        /// Assigns the user for default provider.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The incident.</param>
        private void AssignUserForDefaultProvider(IPluginExecutionContext context, IOrganizationService service, Entity serviceRequest)
        {
            serviceRequest["ownerid"] = new EntityReference
            {
                LogicalName = "systemuser",
                Id = context.UserId
            };
        }

        /// <summary>
        /// Assigns the team for new provider.
        /// </summary>
        /// <param name="primaryProviderId">The primary provider id.</param>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequest">The incident.</param>
        private void AssignTeamForNewProvider(Guid primaryProviderId, IOrganizationService service, Entity serviceRequest, ITracingService trace)
        {
            trace.Trace("In AssignTeamForNewProvider");
            Entity provider = service.Retrieve(CRMAttributesResource.AccountEntity, primaryProviderId, new ColumnSet(CRMAttributesResource.ProviderTeamAttribute));
            if (provider != null)
            {
                serviceRequest["ownerid"] = new EntityReference
                {
                    LogicalName = CRMAttributesResource.TeamEntity,
                    Id = ((EntityReference)provider.Attributes[CRMAttributesResource.ProviderTeamAttribute]).Id
                };
            }

            trace.Trace("Out AssignTeamForNewProvider");
        }

        /// <summary>
        /// Populates the exception log.
        /// </summary>
        /// <param name="ex">The Exception</param>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The incident id.</param>
        private void PopulateExceptionLog(Exception ex, IOrganizationService service, Guid incidentId)
        {
            try
            {
                Logger.Write(ex, ExceptionType.SetPriorityFailed, service, "incident", "smp_servicerequestid", incidentId, string.Empty);
            }
            catch (Exception)
            {
                throw new CustomServiceManagementPortalException("Exception Log not populated", ex);
            }
        }

        /// <summary>
        /// Assigns the approval manager.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The service request id.</param>
        /// <param name="contactId">The contact id.</param>
        private void AssignApprovalManager(IOrganizationService service, Entity serviceRequest, Entity preImageIncident, Guid contactId)
        {
            Guid managerId = this.GetContactManager(service, contactId);
            if (managerId != Guid.Empty)
            {
                serviceRequest[CRMAttributesResource.ServiceRequestApprovalManager] = new EntityReference("contact", managerId);
            }
            else
            {
                serviceRequest[CRMAttributesResource.ServiceRequestApprovalManager] = null;
            }
        }

        /// <summary>
        /// Gets the contact manager.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="contactId">The contact id.</param>
        /// <returns>contact manager</returns>
        private Guid GetContactManager(IOrganizationService service, Guid contactId)
        {
            Guid managerId = Guid.Empty;
            if (service != null)
            {
                Entity contact = service.Retrieve("contact", contactId, new ColumnSet("smp_manager"));
                if (contact != null)
                {
                    managerId = contact.Attributes.Contains("smp_manager") ? ((EntityReference)contact["smp_manager"]).Id : Guid.Empty;
                }
            }

            return managerId;
        }

        /// <summary>
        /// Gets the SR approval status.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The object incident id.</param>
        /// <returns> SR status</returns>
        ////private bool GetSRApprovalStatus(IOrganizationService service, Guid incidentId)
        ////{
        ////    bool status = false;

        ////    ConditionExpression condincident = new ConditionExpression
        ////    {
        ////        AttributeName = "incidentid",
        ////        Operator = ConditionOperator.Equal,
        ////        Values = { incidentId }
        ////    };

        ////    QueryExpression incidentQuery = new QueryExpression
        ////    {
        ////        EntityName = "incident",
        ////        ColumnSet = new ColumnSet(CRMAttributesResource.ServiceRequestIsApprovalRequired),
        ////        Criteria =
        ////            {
        ////                FilterOperator = LogicalOperator.And,
        ////                Conditions = { condincident }
        ////            }
        ////    };

        ////    if (service != null)
        ////    {
        ////        Entity incident = service.RetrieveMultiple(incidentQuery) == null ? null : service.RetrieveMultiple(incidentQuery).Entities.FirstOrDefault();
        ////        if (incident != null)
        ////        {
        ////            if (incident.Attributes.Contains(CRMAttributesResource.ServiceRequestIsApprovalRequired))
        ////            {
        ////                status = Convert.ToBoolean(incident.Attributes[CRMAttributesResource.ServiceRequestIsApprovalRequired], CultureInfo.CurrentCulture);
        ////            }
        ////        }
        ////    }

        ////    return status;
        ////}
    }
}
