// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateUpdateIncidentInPortalTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostCreateUpdateIncidentInPortalTest
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
    public class PostCreateUpdateIncidentInPortalTest
    {
        [TestMethod]
        public void SetDynamicNotesFromPortal()
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
            incident["smp_answer"] = @"{""data"":[{""id"":""ba43e8a7-c9ec-e211-ad57-78e3b511b663"",""answer"":""please ignore this request""},{""id"":""b643e8a7-c9ec-e211-ad57-78e3b511b663"",""answer"":""please ignore this request""},{""id"":""b443e8a7-c9ec-e211-ad57-78e3b511b663"",""answer"":""please ignore this request""}]}";
            incident["smp_portalsubmit"] = true;
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "TestName" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "Test Record" };
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);
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
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@microsoft.com";
                    configuration["smp_title"] = "PendingDispatchStatusCode";
                    configuration["smp_value"] = "2";
                    configuration["smp_title"] = "DefaultProviderName";
                    configuration["smp_value"] = "NotAssigned";
                    collection.Entities.Add(configuration);
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
                else if (entityName == "smp_problemtypedescription")
                {
                    Entity problemDescription = new Entity(entityName);
                    problemDescription["smp_problemtypedescriptionid"] = Guid.NewGuid();
                    problemDescription["smp_problemtypedescriptionname"] = "test";
                    collection.Entities.Add(problemDescription);
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

            PostCreateUpdateIncidentInPortal portalIncident = new PostCreateUpdateIncidentInPortal();
            portalIncident.Execute(serviceProvider);
        }
    }
}
