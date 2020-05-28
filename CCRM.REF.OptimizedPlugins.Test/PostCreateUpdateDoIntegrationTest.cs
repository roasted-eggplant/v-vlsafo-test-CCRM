// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateUpdateDoIntegrationTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostCreateUpdateDoIntegrationTest
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
    using System.Linq;

    [TestClass]
    public class PostCreateUpdateDoIntegrationTest
    {
        [TestMethod]
        public void DoIntegration()
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
            incident["smp_submitteddatetime"] = new DateTime(2018, 1, 8);
            incident["createdon"] = new DateTime(2018, 1, 8);
            incident["smp_problemoccureddatetime"] = new DateTime(2018, 1, 8);
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "TestName" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["statuscode"] = new OptionSetValue(1);
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage, preImage);
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
                    configuration4["smp_value"] = "test";
                    collection.Entities.Add(configuration4);
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration5["smp_title"] = "StatusChangeOnCodes";
                    configuration5["smp_value"] = "Draft";
                    collection.Entities.Add(configuration5);
                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "OutBoundIntegrationtimeOutInSeconds";
                    configuration6["smp_value"] = "5";
                    collection.Entities.Add(configuration6);
                    Entity configuration7 = new Entity("smp_configuration");
                    configuration7.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration7["smp_title"] = "DebugOutBoundIntegration";
                    configuration7["smp_value"] = "4";
                    collection.Entities.Add(configuration7);
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
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }

                return null;
            };

            PostCreateUpdateDoIntegration doIntegration = new PostCreateUpdateDoIntegration();
            doIntegration.Execute(serviceProvider);
        }

        [TestMethod]
        public void DoIntegrationWithIsHostedinAzureFalse()
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
            incident["smp_submitteddatetime"] = new DateTime(2018, 1, 8);
            incident["createdon"] = new DateTime(2018, 1, 8);
            incident["smp_problemoccureddatetime"] = new DateTime(2018, 1, 8);
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "TestName" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["smp_requestedduedate"] = new DateTime(2018, 1, 8); ////"2018-01-08";
            incident.Attributes["title"] = "sample title";
            incident.Attributes["smp_iocode"] = "sample code";
            incident.Attributes["smp_tier1workcompletiondatebybuildingtimezone"] = "sample timezone";
            incident.Attributes["smp_providerduedatebybuildingtimezone"] = "sample title";
            incident.Attributes["smp_problembuildingaddressline1"] = "sample address1";
            incident.Attributes["smp_problembuildingaddressline2"] = "sample address1";
            incident.Attributes["smp_problembuildingcity"] = "sample city";
            incident.Attributes["smp_problembuildingcountry"] = "sample country";
            incident.Attributes["smp_problembuildingzipcode"] = "23432423";
            incident.Attributes["smp_problemcode"] = "sample code";
            incident.Attributes["smp_problemtypedescription"] = "sample description";
            incident.Attributes["smp_requestoralias"] = "sample alias";
            incident.Attributes["smp_requestorfirstname"] = "sample first name";
            incident.Attributes["smp_requestorlastname"] = "sample last name";
            incident.Attributes["smp_requestorphone"] = "sample phone";
            incident.Attributes["smp_requestorroomno"] = "sample promo";
            incident.Attributes["smp_requestorroomtype"] = "sample room type";

            Entity preIncident = new Entity("incident");

            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["statuscode"] = new OptionSetValue(1);
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage, preImage);

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
                    ////query.ExtensionData
                    var v = (((QueryExpression)query).Criteria.Conditions)[0].Values.First().ToString();
                    if (v == "AzureToeknService")
                    {
                        Entity configurationAzure = new Entity("smp_configuration");
                        configurationAzure.Id = Guid.NewGuid();
                        configurationAzure["smp_title"] = "AzureToeknService";
                        configurationAzure["smp_value"] = "https://refoutboundservice.azurewebsites.net/CmmsServiceRequestManagerAAD.svc";
                        collection.Entities.Add(configurationAzure);
                        return collection;
                    }

                    if (v == "TokenEndpointURL")
                    {
                        Entity configurationAzure = new Entity("smp_configuration");
                        configurationAzure.Id = Guid.NewGuid();
                        configurationAzure["smp_title"] = "TokenEndpointURL";
                        configurationAzure["smp_value"] = "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token";
                        collection.Entities.Add(configurationAzure);
                        return collection;
                    }

                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = Guid.NewGuid();
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
                    configuration4["smp_value"] = "test";
                    collection.Entities.Add(configuration4);
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration5["smp_title"] = "StatusChangeOnCodes";
                    configuration5["smp_value"] = "Draft";
                    collection.Entities.Add(configuration5);
                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "OutBoundIntegrationtimeOutInSeconds";
                    configuration6["smp_value"] = "5";
                    collection.Entities.Add(configuration6);
                    Entity configuration7 = new Entity("smp_configuration");
                    configuration7.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration7["smp_title"] = "DebugOutBoundIntegration";
                    configuration7["smp_value"] = "4";
                    collection.Entities.Add(configuration7);
                    Entity configuration8 = new Entity("smp_configuration");
                    configuration8.Id = Guid.NewGuid();
                    configuration8["smp_title"] = "ServiceRequestIntegrationFailEmailTemplateName";
                    configuration8["smp_value"] = "testsample1@microsoft.com";
                    collection.Entities.Add(configuration8);
                    Entity configuration9 = new Entity("smp_configuration");
                    configuration9.Id = Guid.NewGuid();
                    configuration9["smp_title"] = "SASToken";
                    configuration9["smp_value"] = "Test";
                    collection.Entities.Add(configuration9);
                    Entity configuration10 = new Entity("smp_configuration");
                    configuration10.Id = Guid.NewGuid();
                    configuration10["smp_title"] = "AttachmentBlobUrl";
                    configuration10["smp_value"] = "https://refstorage.blob.core.windows.net/msrefpuatblobnotes/";
                    collection.Entities.Add(configuration10);
                    Entity configuration11 = new Entity("smp_configuration");
                    configuration11.Id = Guid.NewGuid();
                    configuration11["smp_title"] = "Environment";
                    configuration11["smp_value"] = "Non-Prod";
                    collection.Entities.Add(configuration11);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode.Id = Guid.NewGuid();
                    srStatusCode["smp_name"] = "test1";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "smp_servicerequestproblemtypedesc")
                {
                    Entity servicerequestproblemtype = new Entity(entityName);
                    servicerequestproblemtype.Id = Guid.NewGuid();
                    servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_answer"] = "Sample Answer";
                    collection.Entities.Add(servicerequestproblemtype);
                }
                else if (entityName == "systemuser")
                {
                    Entity systemUser = new Entity(entityName);
                    systemUser.Id = Guid.NewGuid();
                    systemUser["systemuserid"] = systemUser.Id;
                    collection.Entities.Add(systemUser);
                }
                else if (entityName == "template")
                {
                    Entity annotation = new Entity(entityName);
                    annotation.Id = Guid.NewGuid();
                    annotation["templateid"] = annotation.Id;
                    annotation["subject"] = "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\" /><xsl:template match=\"/data\"><![CDATA[We haven't heard from you...]]></xsl:template></xsl:stylesheet>";
                    annotation["body"] = "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\"><![CDATA[<P>Dear ]]><xsl:choose><xsl:when test=\"account/name\"><xsl:value-of select=\"account/name\" /></xsl:when><xsl:otherwise>Valued Customer</xsl:otherwise></xsl:choose><![CDATA[,</P> <P>We have not heard from you for a while. We wanted to check in and make sure that you are still having a great experience using our product(s). We have asked ]]><xsl:choose><xsl:when test=\"account/ownerid/@name\"><xsl:value-of select=\"account/ownerid/@name\" /></xsl:when><xsl:otherwise>a customer service representative</xsl:otherwise></xsl:choose><![CDATA[ to contact you next week to get your feedback on the product(s) you are currently using and to give you details about our upcoming products.</P> <P>Thank you.</P>]]></xsl:template></xsl:stylesheet>";
                    collection.Entities.Add(annotation);
                }
                else if (entityName == "msdyn_azureblobstoragesetting")
                {
                    Entity blobSettings = new Entity(entityName);
                    blobSettings.Id = Guid.NewGuid();
                    blobSettings["msdyn_name"] = "refstorage";
                    blobSettings["msdyn_annotationcontainer"] = "msrefpuatblobnotes";
                    collection.Entities.Add(blobSettings);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account.Id = Guid.NewGuid();
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = false;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "English";
                    contact["smp_isprovideruser"] = true;
                    contact["smp_costcenter"] = "samdple center";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_alias"] = "sample alias";
                    contact["firstname"] = "sample firstname";
                    contact["lastname"] = "sample lastname";
                    contact["emailaddress1"] = "sample@test.com";
                    return contact;
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
                else if (entity == "smp_problemclass")
                {
                    Entity problemclass = new Entity(entity);
                    problemclass.Id = Guid.NewGuid();
                    problemclass["smp_problemclassname"] = "sample class";
                    return problemclass;
                }
                else if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_buildingname"] = "sample building";
                    building["smp_feedstoreid"] = "Sample StoreId";
                    return building;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity problemclass = new Entity(entity);
                    problemclass.Id = Guid.NewGuid();
                    problemclass["smp_roomtype"] = "sample room";
                    return problemclass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity smp_problemtype = new Entity(entity);
                    smp_problemtype.Id = Guid.NewGuid();
                    smp_problemtype["smp_problemtypename"] = "sample problemtype";
                    return smp_problemtype;
                }

                return null;
            };

            PostCreateUpdateDoIntegration doIntegration = new PostCreateUpdateDoIntegration();
            doIntegration.Execute(serviceProvider);
        }

        [TestMethod]
        [Ignore]
        public void DoIntegrationWithIsHostedinAzureTrue()
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
            incident["smp_submitteddatetime"] = new DateTime(2018, 1, 8);
            incident["createdon"] = new DateTime(2018, 1, 8);
            incident["smp_problemoccureddatetime"] = new DateTime(2018, 1, 8);
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "TestName" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["smp_requestedduedate"] = new DateTime(2018, 1, 8); ////"2018-01-08";
            incident.Attributes["title"] = "sample title";
            incident.Attributes["smp_iocode"] = "sample code";
            incident.Attributes["smp_tier1workcompletiondatebybuildingtimezone"] = "sample timezone";
            incident.Attributes["smp_providerduedatebybuildingtimezone"] = "sample title";
            incident.Attributes["smp_problembuildingaddressline1"] = "sample address1";
            incident.Attributes["smp_problembuildingaddressline2"] = "sample address1";
            incident.Attributes["smp_problembuildingcity"] = "sample city";
            incident.Attributes["smp_problembuildingcountry"] = "sample country";
            incident.Attributes["smp_problembuildingzipcode"] = "23432423";
            incident.Attributes["smp_problemcode"] = "sample code";
            incident.Attributes["smp_problemtypedescription"] = "sample description";
            incident.Attributes["smp_requestoralias"] = "sample alias";
            incident.Attributes["smp_requestorfirstname"] = "sample first name";
            incident.Attributes["smp_requestorlastname"] = "sample last name";
            incident.Attributes["smp_requestorphone"] = "sample phone";
            incident.Attributes["smp_requestorroomno"] = "sample promo";
            incident.Attributes["smp_requestorroomtype"] = "sample room type";

            Entity preIncident = new Entity("incident");

            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["statuscode"] = new OptionSetValue(1);
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage, preImage);

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
                    ////query.ExtensionData
                    var v = (((QueryExpression)query).Criteria.Conditions)[0].Values.First().ToString();
                    if (v == "AzureToeknService")
                    {
                        Entity configurationAzure = new Entity("smp_configuration");
                        configurationAzure.Id = Guid.NewGuid();
                        configurationAzure["smp_title"] = "AzureToeknService";
                        configurationAzure["smp_value"] = "https://gfmstokenservice.azurewebsites.net/CmmsServiceRequestManagerAAD.svc";
                        collection.Entities.Add(configurationAzure);
                        return collection;
                    }

                    if (v == "TokenEndpointURL")
                    {
                        Entity configurationAzure = new Entity("smp_configuration");
                        configurationAzure.Id = Guid.NewGuid();
                        configurationAzure["smp_title"] = "TokenEndpointURL";
                        configurationAzure["smp_value"] = "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/token";
                        collection.Entities.Add(configurationAzure);
                        return collection;
                    }

                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = Guid.NewGuid();
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
                    configuration4["smp_value"] = "test";
                    collection.Entities.Add(configuration4);
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration5["smp_title"] = "StatusChangeOnCodes";
                    configuration5["smp_value"] = "Draft";
                    collection.Entities.Add(configuration5);
                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "OutBoundIntegrationtimeOutInSeconds";
                    configuration6["smp_value"] = "5";
                    collection.Entities.Add(configuration6);
                    Entity configuration7 = new Entity("smp_configuration");
                    configuration7.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration7["smp_title"] = "DebugOutBoundIntegration";
                    configuration7["smp_value"] = "4";
                    collection.Entities.Add(configuration7);
                    Entity configuration8 = new Entity("smp_configuration");
                    configuration8.Id = Guid.NewGuid();
                    configuration8["smp_title"] = "ServiceRequestIntegrationFailEmailTemplateName";
                    configuration8["smp_value"] = "testsample1@microsoft.com";
                    collection.Entities.Add(configuration8);
                    Entity configuration9 = new Entity("smp_configuration");
                    configuration9.Id = Guid.NewGuid();
                    configuration9["smp_title"] = "SASToken";
                    configuration9["smp_value"] = "Test";
                    collection.Entities.Add(configuration9);
                    Entity configuration10 = new Entity("smp_configuration");
                    configuration10.Id = Guid.NewGuid();
                    configuration10["smp_title"] = "AttachmentBlobUrl";
                    configuration10["smp_value"] = "https://refstorage.blob.core.windows.net/msrefpuatblobnotes/";
                    collection.Entities.Add(configuration10);
                    Entity configuration11 = new Entity("smp_configuration");
                    configuration11.Id = Guid.NewGuid();
                    configuration11["smp_title"] = "Environment";
                    configuration11["smp_value"] = "Non-Prod";
                    collection.Entities.Add(configuration11);
                }
                else if (entityName == "smp_providerintegration")
                {
                    Entity providerintegration = new Entity(entityName);
                    providerintegration.Id = Guid.NewGuid();
                    providerintegration["smp_name"] = "Engineer Team Details";
                    providerintegration["smp_clientid"] = "";
                    providerintegration["smp_serviceprincipalid"] = "";
                    collection.Entities.Add(providerintegration);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode.Id = Guid.NewGuid();
                    srStatusCode["smp_name"] = "test1";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(2);
                    collection.Entities.Add(srStatusCode);
                }
                else if (entityName == "smp_servicerequestproblemtypedesc")
                {
                    Entity servicerequestproblemtype = new Entity(entityName);
                    servicerequestproblemtype.Id = Guid.NewGuid();
                    servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_answer"] = "Sample Answer";
                    collection.Entities.Add(servicerequestproblemtype);
                }
                else if (entityName == "systemuser")
                {
                    Entity systemUser = new Entity(entityName);
                    systemUser.Id = Guid.NewGuid();
                    systemUser["systemuserid"] = systemUser.Id;
                    collection.Entities.Add(systemUser);
                }
                else if (entityName == "template")
                {
                    Entity annotation = new Entity(entityName);
                    annotation.Id = Guid.NewGuid();
                    annotation["templateid"] = annotation.Id;
                    annotation["subject"] = "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\" /><xsl:template match=\"/data\"><![CDATA[We haven't heard from you...]]></xsl:template></xsl:stylesheet>";
                    annotation["body"] = "<?xml version=\"1.0\" ?><xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\" version=\"1.0\"><xsl:output method=\"text\" indent=\"no\"/><xsl:template match=\"/data\"><![CDATA[<P>Dear ]]><xsl:choose><xsl:when test=\"account/name\"><xsl:value-of select=\"account/name\" /></xsl:when><xsl:otherwise>Valued Customer</xsl:otherwise></xsl:choose><![CDATA[,</P> <P>We have not heard from you for a while. We wanted to check in and make sure that you are still having a great experience using our product(s). We have asked ]]><xsl:choose><xsl:when test=\"account/ownerid/@name\"><xsl:value-of select=\"account/ownerid/@name\" /></xsl:when><xsl:otherwise>a customer service representative</xsl:otherwise></xsl:choose><![CDATA[ to contact you next week to get your feedback on the product(s) you are currently using and to give you details about our upcoming products.</P> <P>Thank you.</P>]]></xsl:template></xsl:stylesheet>";
                    collection.Entities.Add(annotation);
                }
                else if (entityName == "msdyn_azureblobstoragesetting")
                {
                    Entity blobSettings = new Entity(entityName);
                    blobSettings.Id = Guid.NewGuid();
                    blobSettings["msdyn_name"] = "refstorage";
                    blobSettings["msdyn_annotationcontainer"] = "msrefpuatblobnotes";
                    collection.Entities.Add(blobSettings);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account.Id = Guid.NewGuid();
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = true;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "English";
                    contact["smp_isprovideruser"] = true;
                    contact["smp_costcenter"] = "samdple center";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_alias"] = "sample alias";
                    contact["firstname"] = "sample firstname";
                    contact["lastname"] = "sample lastname";
                    contact["emailaddress1"] = "sample@test.com";
                    return contact;
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
                else if (entity == "smp_problemclass")
                {
                    Entity problemclass = new Entity(entity);
                    problemclass.Id = Guid.NewGuid();
                    problemclass["smp_problemclassname"] = "sample class";
                    return problemclass;
                }
                else if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_buildingname"] = "sample building";
                    building["smp_feedstoreid"] = "Sample StoreId";
                    return building;
                }
                else if (entity == "smp_roomtype")
                {
                    Entity problemclass = new Entity(entity);
                    problemclass.Id = Guid.NewGuid();
                    problemclass["smp_roomtype"] = "sample room";
                    return problemclass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity smp_problemtype = new Entity(entity);
                    smp_problemtype.Id = Guid.NewGuid();
                    smp_problemtype["smp_problemtypename"] = "sample problemtype";
                    return smp_problemtype;
                }

                return null;
            };

            PostCreateUpdateDoIntegration doIntegration = new PostCreateUpdateDoIntegration();
            doIntegration.Execute(serviceProvider);
        }
    }
}
