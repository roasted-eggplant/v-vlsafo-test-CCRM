// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestCreateTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PreServiceRequestCreateTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Fakes;

    [TestClass]
    public class PreServiceRequestCreateTest
    {
        [TestMethod]
        public void CreateSR()
        {
            PreServiceRequestCreate servicerequestCreate = new PreServiceRequestCreate();
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(915240000);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = true;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_integrationstatus"] = true;
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
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

                if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    collection.Entities.Add(matrix);
                }

                if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }

                if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }

                return null;
            };

            servicerequestCreate.Execute(serviceProvider);
        }

        [TestMethod]
        public void CreateSRCaseOriginCode()
        {
            PreServiceRequestCreate servicerequestCreate = new PreServiceRequestCreate();
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
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
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident["smp_requestorid"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
            incident["telephone1"] = "777";
            incident["smp_alias"] = "testalias";
            incident["emailaddress1"] = "testalias@test.com";
            incident.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["new_requestorroomnumber"] = "1222";
            incident.Attributes["telephone1"] = "77889990";

            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);

            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
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

                if (entityName == "smp_providermatrix")
                {
                    Entity matrix = new Entity("smp_providermatrix");
                    matrix.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    matrix["smp_primaryproviderid"] = new EntityReference("account", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                    matrix.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A178B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
                    matrix.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A278B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                    matrix.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A378B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                    matrix.Attributes["smp_approvalrequired"] = true;
                    matrix.Attributes["smp_billablecode"] = "1234";
                    collection.Entities.Add(matrix);
                }

                if (entityName == "smp_room")
                {
                    Entity room = new Entity(entityName);
                    room.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    collection.Entities.Add(room);
                }

                if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                    account["statuscode"] = 1;
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_room")
                {
                    Entity room = new Entity(entity);
                    room.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    room["smp_name"] = "test room";
                    room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                    return room;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-us";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = false;
                    contact["smp_alias"] = "testalias";
                    contact["firstname"] = "first";
                    contact["lastname"] = "last";
                    contact["funllname"] = "last";
                    contact["emailaddress1"] = "test@microsoft.com";
                    contact["smp_manager"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0679"));

                    return contact;
                }
                else if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    building["smp_feedstoreid"] = "Sample StoreId";
                    building["smp_buildingname"] = "sample building";
                    building["smp_addressline1"] = "test addressline1";
                    building["smp_addressline2"] = "test addressline2";
                    building["smp_state"] = "test state";
                    building["smp_country"] = "test country";
                    building["smp_city"] = "test city";
                    building["smp_zipcode"] = "1222333";
                    building["smp_timezoneid"] = "test zone";
                    building["statuscode"] = new OptionSetValue(2);
                    building["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));

                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_donotallowsurvey"] = true;
                    problemClass["smp_allowemailnotification"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_donotallowsurvey"] = true;
                    problemType["smp_allowemailnotification"] = true;
                    return problemType;
                }
                else if (entity == "smp_problemtypedescription")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_problemtypedescriptionname"] = "sample description";
                    return problemType;
                }
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority.Id = Guid.NewGuid();
                    priority["smp_name"] = "sample name";
                    return priority;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity problemclass = new Entity(entity);
                    problemclass.Id = Guid.NewGuid();
                    problemclass["smp_roomtype"] = "sample room";
                    return problemclass;
                }

                return null;
            };
            servicerequestCreate.Execute(serviceProvider);
        }
    }
}