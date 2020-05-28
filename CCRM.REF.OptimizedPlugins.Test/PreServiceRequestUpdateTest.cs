// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestUpdateTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PreServiceRequestUpdateTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Test
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Fakes;

    [TestClass]
    public class PreServiceRequestUpdateTest
    {
        [TestMethod]
        public void SRUpdate()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(1);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_isapprovalrequired"] = true;
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(2);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = false;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);
            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };
            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    return contact;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate2()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(2);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = false;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "EmailSenderDomainName";
                    configuration1["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration1);
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "8/8RoutingPriorities";
                    configuration["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                    Entity configuration4 = new Entity("smp_configuration");
                    configuration4.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration4["smp_title"] = "NoCmmsIntegrationActionCodes";
                    configuration4["smp_value"] = "Draft";
                    collection.Entities.Add(configuration4);
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration5["smp_title"] = "StatusChangeOnCodes";
                    configuration5["smp_value"] = "Draft";
                    collection.Entities.Add(configuration5);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate3()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = false;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate4()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = false;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate5()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = false;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate6()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = true;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = true;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate7()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = true;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = true;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", postImage);

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = -08;
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return incident;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+02:30", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+03:30", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("-04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }
                else if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", Guid.NewGuid());
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_isprovideruser"] = false;
                    return contact;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate8()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = true;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(2);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = true;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            pluginContext.InputParametersGet = () => paramCollection;

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = -08;
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+02:30", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+03:30", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("-04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }
                else if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", Guid.NewGuid());
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_isprovideruser"] = false;
                    return contact;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate9()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = true;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = true;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            pluginContext.InputParametersGet = () => paramCollection;

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = -08;
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+02:30", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+03:30", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("-04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }
                else if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", Guid.NewGuid());
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_isprovideruser"] = false;
                    return contact;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }

        [TestMethod]
        public void SRUpdate10()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(2);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = true;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A478B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB3C0560"));
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(2);
            preIncident["smp_issurveyservicerequest"] = false;
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            preIncident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            preIncident.Attributes["smp_portalsubmit"] = true;
            preIncident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            preIncident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            preIncident["smp_submitteddatetime"] = "2018-01-08";
            preIncident["createdon"] = "2018-01-08";
            preIncident["smp_problemoccureddatetime"] = "2018-01-08";
            preIncident.Attributes["customerid"] = new EntityReference("account", new Guid("884A578B-0469-E711-80F5-3863BB5C0560"));
            preIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
            preIncident["smp_createdfrom"] = new OptionSetValue(1);
            preIncident["smp_isapprovalrequired"] = true;
            pluginContext.InputParametersGet = () => paramCollection;

            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", preIncident)) };
            Helper.Helper.PreImagePluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", preImage);

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_8x8mailbox'>"))
                    {
                        entityName = "smp_8x8mailbox";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                    {
                        entityName = "smp_room";
                    }
                    else if (((FetchExpression)query).Query.Contains("<entity name='account'>"))
                    {
                        entityName = "account";
                    }
                }
                else if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "8/8RoutingPriorities";
                    configuration1["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration1);
                    Entity configuration2 = new Entity("smp_configuration");
                    configuration2.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration2["smp_title"] = "PendingDispatchStatusCode";
                    configuration2["smp_value"] = "2";
                    collection.Entities.Add(configuration2);
                    Entity configuration3 = new Entity("smp_configuration");
                    configuration3.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration3["smp_title"] = "DefaultProviderName";
                    configuration3["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration3);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }
                else if (entityName == "smp_8x8mailbox")
                {
                    Entity eightByEight = new Entity(entityName);
                    eightByEight["smp_8x8mailboxid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(eightByEight);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = -08;
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["createdon"] = "2018-01-08";
                    room["smp_name"] = "Other";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-82F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = false;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    usersettings["timezonecode"] = "-08";
                    collection.Entities.Add(usersettings);
                }
                else if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["telephone1"] = "9090909090";
                    contact["smp_alias"] = "Alias";
                    contact["emailaddress1"] = "test@email.com";
                    contact["smp_building"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
                    contact["smp_room"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                    return contact;
                }
                else if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+02:30", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+03:30", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("-04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }
                else if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["UtcTime"]).AddMinutes(Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    localTimeResponse.Results.Add("LocalTime", dateTime);
                    return localTimeResponse;
                }
                else if (QueryBase.RequestName == "UtcTimeFromLocalTime")
                {
                    UtcTimeFromLocalTimeResponse utcTimeResponse = new UtcTimeFromLocalTimeResponse();
                    DateTime dateTime = Convert.ToDateTime(QueryBase.Parameters["LocalTime"]).AddMinutes(-Convert.ToInt32(QueryBase.Parameters["TimeZoneCode"]));
                    utcTimeResponse.Results.Add("UtcTime", dateTime);
                    return utcTimeResponse;
                }

                return null;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", Guid.NewGuid());
                    building["smp_isfinancialstatecampus"] = true;
                    return building;
                }
                else if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_noofminutes"] = 100;
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity roomType = new Entity(entity);
                    roomType["smp_zone"] = new OptionSetValue(1);
                    return roomType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact["smp_isprovideruser"] = false;
                    return contact;
                }

                return null;
            };

            PreServiceRequestUpdate preServiceRequestUpdate = new PreServiceRequestUpdate();
            preServiceRequestUpdate.Execute(serviceProvider);
        }
    }
}
