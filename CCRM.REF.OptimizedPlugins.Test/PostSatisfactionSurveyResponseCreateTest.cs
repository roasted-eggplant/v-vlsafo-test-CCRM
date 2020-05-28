// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostSatisfactionSurveyResponseCreateTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostSatisfactionSurveyResponseCreateTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Fakes;

    [TestClass]
    public class PostSatisfactionSurveyResponseCreateTest
    {
        public static void PluginVariables(StubIServiceProvider serviceProvider, StubIPluginExecutionContext pluginContext, StubIOrganizationService organizationService, int stageNumber, string messageName, EntityImageCollection postImage)
        {
            var serviceFactory = new StubIOrganizationServiceFactory();
            var tracingService = new StubITracingService();

            if (serviceProvider != null)
            {
                serviceProvider.GetServiceType = type =>
                {
                    if (type == typeof(IPluginExecutionContext))
                    {
                        return pluginContext;
                    }
                    else if (type == typeof(ITracingService))
                    {
                        return tracingService;
                    }
                    else if (type == typeof(IOrganizationServiceFactory))
                    {
                        return serviceFactory;
                    }

                    return null;
                };
            }

            pluginContext.DepthGet = () => 4;
            pluginContext.UserIdGet = () => new Guid();
            pluginContext.MessageNameGet = () => messageName;
            pluginContext.StageGet = () => stageNumber;
            pluginContext.InitiatingUserIdGet = () => Guid.Parse("F83DA6A6-748E-E411-9412-00155D614A70");
            pluginContext.CorrelationIdGet = () => new Guid();
            pluginContext.PrimaryEntityIdGet = () =>
            {
                return Guid.NewGuid();
            };

            pluginContext.PostEntityImagesGet = () => { return postImage; };
            ////pluginContext.PreEntityImagesGet = () => { return postImage; };
            serviceFactory.CreateOrganizationServiceNullableOfGuid = t1 => organizationService;
        }

        public static void Trace(string message, params object[] value)
        {
        }

        [TestMethod]
        public void ServiceRequestFromSurvey()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();

            pluginContext.PrimaryEntityNameGet = () => "smp_srsatisfactionsurvey";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["statuscode"] = new OptionSetValue(2);
            incident.Attributes["smp_incidentid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
            incident.Attributes["caseorigincode"] = new OptionSetValue(1);
            incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
            incident.Attributes["smp_portalsubmit"] = false;
            incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_submitteddatetime"] = "2018-01-08";
            incident["createdon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident.Attributes["smp_7a5070ca45ac4bfb9cf14dc614b2c28b"] = new OptionSetValue(1);
            incident.Attributes["smp_df933ec69da9441986ea13ad23c16c16"] = new OptionSetValue(1);
            incident.Attributes["smp_a9dfbbbb5d9540d5bea5954f97a5b57e"] = new OptionSetValue(1);
            incident.Attributes["smp_74593f38170543a0a6af56948307ff8b"] = new OptionSetValue(1);
            incident.Attributes["smp_8ae246fa75ed4d47a4bb402c7f6b3339 "] = new OptionSetValue(3);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["smp_requestorid"] = new EntityReference("contact", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["smp_requestorphone"] = "88999";
            incident.Attributes["smp_requestoralias"] = "test";
            incident.Attributes["smp_requestorroomno"] = "88999";
            incident.Attributes["smp_requestoremail"] = "test@test.com";

            ////incident.Attributes["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);
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
                    configuration["smp_title"] = "8/8RoutingPriorities";
                    configuration["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration);
                    Entity configuration1 = new Entity("smp_configuration");
                    configuration1.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration1["smp_title"] = "EmailSenderDomainName";
                    configuration1["smp_value"] = "test@microsoft.com";
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
                    Entity configuration9 = new Entity("smp_configuration");
                    configuration9.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration9["smp_title"] = "ProviderQuestionOne";
                    configuration9["smp_value"] = "smp_7a5070ca45ac4bfb9cf14dc614b2c28b";
                    collection.Entities.Add(configuration9);
                    Entity configuration10 = new Entity("smp_configuration");
                    configuration10.Id = new Guid();
                    configuration10["smp_title"] = "ProviderQuestionTwo";
                    configuration10["smp_value"] = "smp_df933ec69da9441986ea13ad23c16c16 ";
                    collection.Entities.Add(configuration10);
                    Entity configuration11 = new Entity("smp_configuration");
                    configuration11.Id = new Guid();
                    configuration11["smp_title"] = "en-us";
                    configuration11["smp_value"] = "2018-09-09";
                    collection.Entities.Add(configuration11);
                    Entity configuration12 = new Entity("smp_configuration");
                    configuration12.Id = new Guid();
                    configuration12["smp_title"] = "CallCenterQuestionOne";
                    configuration12["smp_value"] = "smp_a9dfbbbb5d9540d5bea5954f97a5b57e";
                    collection.Entities.Add(configuration12);
                    Entity configuration13 = new Entity("smp_configuration");
                    configuration13.Id = new Guid();
                    configuration13["smp_title"] = "CallCenterQuestionTwo";
                    configuration13["smp_value"] = "smp_74593f38170543a0a6af56948307ff8b ";
                    collection.Entities.Add(configuration13);
                    Entity configuration14 = new Entity("smp_configuration");
                    configuration14.Id = new Guid();
                    configuration14["smp_title"] = "SingleResponseQuestion";
                    configuration14["smp_value"] = "smp_8ae246fa75ed4d47a4bb402c7f6b3339";
                    collection.Entities.Add(configuration14);
                    Entity configuration15 = new Entity("smp_configuration");
                    configuration15.Id = new Guid();
                    configuration15["smp_title"] = "Seemoreinformationaboutyourservicerequest";
                    configuration15["smp_value"] = "seemoreinformation";
                    collection.Entities.Add(configuration15);
                    Entity configuration16 = new Entity("smp_configuration");
                    ////configuration16["smp_entityname"] = "incident";
                    ////configuration16.Attributes["statuscode"] = new OptionSetValue(2);
                    ////configuration16["smp_attributename"] = "smp_duedate";
                    configuration16["smp_title"] = "PrioritiesToDisplayTheDueDate";
                    configuration16["smp_value"] = "2018-09-09";
                    collection.Entities.Add(configuration16);
                    Entity configuration17 = new Entity("smp_configuration");
                    configuration17.Id = new Guid();
                    configuration17["smp_title"] = "SurveyQuestions";
                    configuration17["smp_value"] = "survery1;survey2;survey3;survey4;survery5";
                    collection.Entities.Add(configuration17);
                    Entity configuration18 = new Entity("smp_configuration");
                    configuration18.Id = new Guid();
                    configuration18["smp_title"] = "SurveyProviderRelatedProblemClass";
                    configuration18["smp_value"] = "SurveyProviderRelatedProblemClass";
                    collection.Entities.Add(configuration18);
                    Entity configuration19 = new Entity("smp_configuration");
                    configuration19.Id = new Guid();
                    configuration19["smp_title"] = "SurveyProviderRelatedProblemType";
                    configuration19["smp_value"] = "SurveyProviderRelatedProblemType";
                    collection.Entities.Add(configuration19);
                    Entity configuration20 = new Entity("smp_configuration");
                    configuration20.Id = new Guid();
                    configuration20["smp_title"] = "SurveyCallCenterManagementRelatedProblemClass";
                    configuration20["smp_value"] = "SurveyCallCenterManagementRelatedProblemClass";
                    collection.Entities.Add(configuration20);
                    Entity configuration21 = new Entity("smp_configuration");
                    configuration21.Id = new Guid();
                    configuration21["smp_title"] = "SurveyCallCenterManagementRelatedProblemType";
                    configuration21["smp_value"] = "SurveyCallCenterManagementRelatedProblemType";
                    collection.Entities.Add(configuration21);
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
                else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account.Id = new Guid("58d19398-b350-490a-9e2d-6b30391304a0");
                    account["name"] = "test";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    collection.Entities.Add(account);
                }
                else if (entityName == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entityName);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_donotallowsurvey"] = true;
                    problemClass["smp_allowemailnotification"] = true;
                    collection.Entities.Add(problemClass);
                }
                else if (entityName == "smp_problemtype")
                {
                    Entity problemType = new Entity(entityName);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_donotallowsurvey"] = true;
                    problemType["smp_allowemailnotification"] = true;
                    collection.Entities.Add(problemType);
                }

                return collection;
            };
            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "incident")
                {
                    Entity incident1 = new Entity(entity);
                    incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    incident1["smp_issurveyservicerequest"] = true;
                    incident1["smp_cancelledforreclassification"] = true;
                    incident1["ticketnumber"] = "CAS-120083-X4D1Q2";
                    incident1["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                    incident1.Attributes["smp_requestorid"] = new EntityReference("contact", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident1.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident1.Attributes["smp_requestorphone"] = "88999";
                    incident1.Attributes["smp_requestoralias"] = "test";
                    incident1.Attributes["smp_requestorroomno"] = "88999";
                    incident1.Attributes["smp_requestoremail"] = "test@test.com";
                    incident1["smp_contactbuilding"] = new EntityReference("smp_building", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident1["smp_contactemail"] = "test@test.com";
                    incident1["smp_contactalias"] = "test";
                    incident1["smp_contactroom"] = "test";
                    incident1["smp_costcentercode"] = "11223";
                    incident1["smp_contactphone"] = "44555";
                    incident1["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident1["smp_problembuildingaddressline2"] = "test";
                    incident1["smp_problembuildingstate"] = "test";
                    incident1["smp_problembuildingzipcode"] = "5344";
                    incident1["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident1["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident1["smp_problembuildingaddressline1"] = "test";
                    incident1["smp_problembuildingcity"] = "test";
                    incident1["smp_problemroom"] = "test";

                    return incident1;
                }

                if (entity == "smp_building")
                {
                    Entity building = new Entity("entity");
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    building["smp_problembuildingaddressline2"] = "test";
                    building["smp_problembuildingstate"] = "test";
                    building["smp_problembuildingzipcode"] = "5344";
                    building["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    building["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    building["smp_problembuildingaddressline1"] = "test";
                    building["smp_problembuildingcity"] = "test";
                    building["smp_problemroom"] = "test";

                    building["smp_problembuildingtimezone"] = new EntityReference("smp_timezone", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    return incident;
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
                    contact.Id = new Guid("884a078b-0467-e711-80f5-3863bb3c0660");
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = false;
                    contact["smp_alias"] = "testalias";
                    contact["firstname"] = "first";
                    contact["lastname"] = "last";
                    contact["funllname"] = "last";
                    contact["emailaddress1"] = "test@microsoft.com";

                    return contact;
                }

                return null;
            };
            PostSatisfactionSurveyResponseCreate pluginClass = new PostSatisfactionSurveyResponseCreate();
            pluginClass.Execute(serviceProvider);
        }
    }
}
