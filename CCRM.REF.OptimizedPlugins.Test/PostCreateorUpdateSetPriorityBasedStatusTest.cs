// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateorUpdateSetPriorityBasedStatusTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostCreateorUpdateSetPriorityBasedStatusTest
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
    public class PostCreateorUpdateSetPriorityBasedStatusTest
    {
        [TestMethod]
        public void SetPriorityBasedStatus()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            ParameterCollection paramCollection = new ParameterCollection();
            ParameterCollection paramCollectionPostImage = new ParameterCollection();
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
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);
            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("QueryExpression"))
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
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }

                return null;
            };

            PostCreateorUpdateSetPriorityBasedStatus setPriorityBasedStatus = new PostCreateorUpdateSetPriorityBasedStatus();
            setPriorityBasedStatus.Execute(serviceProvider);
        }
    }
}
