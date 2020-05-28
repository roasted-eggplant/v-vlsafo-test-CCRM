// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestCreateSetPriorityTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PreServiceRequestCreateSetPriorityTest
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
    using System.Fakes;
    using System.Globalization;

    [TestClass]
    public class PreServiceRequestCreateSetPriorityTest
    {
        ////Unused Variable Reported in Fortify Scan
        ////public OrganizationResponse raiseRequest { get; private set; }

        [TestMethod]
        public void SetPriority()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(1);
            incident.Attributes["smp_portalsubmit"] = false;
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty);
            incident.Attributes["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+02:30", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+03:30", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }
                else if (QueryBase.RequestName == "LocalTimeFromUtcTime")
                {
                    LocalTimeFromUtcTimeResponse localTimeResponse = new LocalTimeFromUtcTimeResponse();
                    ////localTimeResponse.Results.Add("", value);
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

                return null;
            };
            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                    {
                        entityName = "ava_keyvaluepair";
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

                if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "smp_slamatrix")
                {
                    Entity slaMatrix = new Entity(entityName);
                    slaMatrix["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0467-E711-80F5-3863BB3C1489"));
                    slaMatrix["smp_starthours"] = 2;
                    slaMatrix["smp_endhours"] = 1;
                    collection.Entities.Add(slaMatrix);
                }
                else if (entityName == "smp_buildingworkhours")
                {
                    Entity buildWorkHours = new Entity(entityName);
                    buildWorkHours["smp_starthours"] = new OptionSetValue(1);
                    buildWorkHours["smp_endhours"] = new OptionSetValue(2);
                    buildWorkHours.FormattedValues["smp_starthours"] = "08:30";
                    buildWorkHours.FormattedValues["smp_endhours"] = "08:30";
                    collection.Entities.Add(buildWorkHours);
                }
                else if (entityName == "smp_weekdays")
                {
                    Entity weekdays = new Entity(entityName);
                    weekdays["smp_name"] = Convert.ToDateTime("09-03-2017 02:30 AM");
                    collection.Entities.Add(weekdays);
                }
                else if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity(entityName);
                    configuration["smp_value"] = "Production";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity systemuser = new Entity(entityName);
                    systemuser.Id = Guid.NewGuid();
                    systemuser["systemuserid"] = systemuser.Id;
                    collection.Entities.Add(systemuser);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings["timezonecode"] = 85;
                    collection.Entities.Add(usersettings);
                }

                return collection;
            };

            PreServiceRequestCreateSetPriority prioritySet = new PreServiceRequestCreateSetPriority();
            prioritySet.Execute(serviceProvider);
        }

        [TestMethod]
        public void SetPriority1()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            ////incident.Attributes["smp_duedate"] = "2018-01-08";//Convert.ToDateTime(null, CultureInfo.CurrentCulture);
            incident.Attributes["smp_portalsubmit"] = false;
            ////incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            ////incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            //// incident["smp_submitteddatetime"] = Convert.ToDateTime(null, CultureInfo.CurrentCulture);//new DateTime(2018, 1, 8);
            incident["createdon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            ////incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            ////incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty);
            incident.Attributes["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);

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
                    ////localTimeResponse.Results.Add("", value);
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

                return null;
            };
            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                    {
                        entityName = "ava_keyvaluepair";
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

                if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "smp_slamatrix")
                {
                    Entity slaMatrix = new Entity(entityName);
                    slaMatrix["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0467-E711-80F5-3863BB3C1489"));
                    slaMatrix["smp_starthours"] = 2;
                    slaMatrix["smp_endhours"] = 1;
                    collection.Entities.Add(slaMatrix);
                }
                else if (entityName == "smp_buildingworkhours")
                {
                    ////Entity buildWorkHours = new Entity(entityName);
                    ////buildWorkHours["smp_starthours"] = new OptionSetValue(1);
                    ////buildWorkHours["smp_endhours"] = new OptionSetValue(2);
                    ////buildWorkHours.FormattedValues["smp_starthours"] = "08:30";
                    ////buildWorkHours.FormattedValues["smp_endhours"] = "08:30";
                    ////collection.Entities.Add(buildWorkHours);
                }
                else if (entityName == "smp_weekdays")
                {
                    Entity weekdays = new Entity(entityName);
                    weekdays["smp_name"] = Convert.ToDateTime("09-03-2017 02:30 AM");
                    collection.Entities.Add(weekdays);
                }
                else if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = Guid.NewGuid();
                    configuration["smp_title"] = "AzureBlobEnvironment";
                    configuration["smp_value"] = "UAT";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "EmailSenderDomainName";
                    configuration1["smp_value"] = "test@microsoft.com";
                    collection.Entities.Add(configuration1);
                }
                else if (entityName == "systemuser")
                {
                    Entity systemuser = new Entity(entityName);
                    systemuser.Id = Guid.NewGuid();
                    systemuser["systemuserid"] = systemuser.Id;
                    collection.Entities.Add(systemuser);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings["timezonecode"] = 85;
                    collection.Entities.Add(usersettings);
                }

                return collection;
            };

            PreServiceRequestCreateSetPriority prioritySet = new PreServiceRequestCreateSetPriority();
            prioritySet.Execute(serviceProvider);
        }

        [TestMethod]
        public void SetPriority2()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_portalsubmit"] = false;
            incident["createdon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = Convert.ToDateTime(null, CultureInfo.CurrentCulture);
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty) { Name = "priority" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty);
            incident.Attributes["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);

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

                return null;
            };

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                    {
                        entityName = "ava_keyvaluepair";
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

                if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "smp_slamatrix")
                {
                    Entity slaMatrix = new Entity(entityName);
                    slaMatrix["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0467-E711-80F5-3863BB3C1489"));
                    slaMatrix["smp_starthours"] = 2;
                    slaMatrix["smp_endhours"] = 1;
                    collection.Entities.Add(slaMatrix);
                }
                else if (entityName == "smp_buildingworkhours")
                {
                    ////Entity buildWorkHours = new Entity(entityName);
                    ////buildWorkHours["smp_starthours"] = new OptionSetValue(1);
                    ////buildWorkHours["smp_endhours"] = new OptionSetValue(2);
                    ////buildWorkHours.FormattedValues["smp_starthours"] = "08:30";
                    ////buildWorkHours.FormattedValues["smp_endhours"] = "08:30";
                    ////collection.Entities.Add(buildWorkHours);
                }
                else if (entityName == "smp_weekdays")
                {
                    Entity weekdays = new Entity(entityName);
                    weekdays["smp_name"] = Convert.ToDateTime("09-03-2017 02:30 AM");
                    collection.Entities.Add(weekdays);
                }
                else if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity(entityName);
                    configuration["smp_value"] = "Production";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity systemuser = new Entity(entityName);
                    systemuser.Id = Guid.NewGuid();
                    systemuser["systemuserid"] = systemuser.Id;
                    collection.Entities.Add(systemuser);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings["timezonecode"] = 85;
                    collection.Entities.Add(usersettings);
                }

                return collection;
            };

            PreServiceRequestCreateSetPriority prioritySet = new PreServiceRequestCreateSetPriority();
            prioritySet.Execute(serviceProvider);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void SetPriority3()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(3);
            incident.Attributes["smp_duedate"] = "2018-01-08";////Convert.ToDateTime(null, CultureInfo.CurrentCulture);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            ////incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            //// incident["smp_submitteddatetime"] = Convert.ToDateTime(null, CultureInfo.CurrentCulture);//new DateTime(2018, 1, 8);
            incident["createdon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            ////incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            ////incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", Guid.Empty);
            incident.Attributes["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);

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
                    ////localTimeResponse.Results.Add("", value);
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

                return null;
            };
            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                    {
                        entityName = "ava_keyvaluepair";
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

                if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    collection.Entities.Add(matrix);
                }
                else if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }
                else if (entityName == "smp_slamatrix")
                {
                    Entity slaMatrix = new Entity(entityName);
                    slaMatrix["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0467-E711-80F5-3863BB3C1489"));
                    slaMatrix["smp_starthours"] = 2;
                    slaMatrix["smp_endhours"] = 1;
                    collection.Entities.Add(slaMatrix);
                }
                else if (entityName == "smp_buildingworkhours")
                {
                    ////Entity buildWorkHours = new Entity(entityName);
                    ////buildWorkHours["smp_starthours"] = new OptionSetValue(1);
                    ////buildWorkHours["smp_endhours"] = new OptionSetValue(2);
                    ////buildWorkHours.FormattedValues["smp_starthours"] = "08:30";
                    ////buildWorkHours.FormattedValues["smp_endhours"] = "08:30";
                    ////collection.Entities.Add(buildWorkHours);
                }
                else if (entityName == "smp_weekdays")
                {
                    Entity weekdays = new Entity(entityName);
                    weekdays["smp_name"] = Convert.ToDateTime("09-03-2017 02:30 AM");
                    collection.Entities.Add(weekdays);
                }
                else if (entityName == "smp_configuration")
                {
                    Entity configuration = new Entity(entityName);
                    configuration["smp_value"] = "Production";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity systemuser = new Entity(entityName);
                    systemuser.Id = Guid.NewGuid();
                    systemuser["systemuserid"] = systemuser.Id;
                    collection.Entities.Add(systemuser);
                }
                else if (entityName == "usersettings")
                {
                    Entity usersettings = new Entity(entityName);
                    usersettings["timezonecode"] = 85;
                    collection.Entities.Add(usersettings);
                }

                return collection;
            };

            PreServiceRequestCreateSetPriority prioritySet = new PreServiceRequestCreateSetPriority();
            prioritySet.Execute(serviceProvider);
        }
    }
}