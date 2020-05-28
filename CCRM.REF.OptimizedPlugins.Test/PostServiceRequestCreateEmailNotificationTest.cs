// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestCreateEmailNotificationTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostServiceRequestCreateEmailNotificationTest
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
    public class PostServiceRequestCreateEmailNotificationTest
    {
        [TestMethod]
        public void CreateEmail()
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
            ////incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
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
            incident.Attributes["customerid"] = new EntityReference("contact", new Guid());
            incident.Attributes["smp_allowemailnotification"] = true;
            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["statuscode"] = new OptionSetValue(1);
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////pluginContext.PostEntityImagesGet = () => { return postImage; };

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

                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "test@test.com";
                    //// configuration["statecode"] = new OptionSetValue(0);

                    collection.Entities.Add(configuration);
                    Entity configuration9 = new Entity("smp_configuration");
                    configuration9.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration9["smp_title"] = "Sample One";
                    configuration9["smp_value"] = "testsample@microsoft.com";
                    collection.Entities.Add(configuration9);

                    //////
                    Entity configuration10 = new Entity("smp_configuration");
                    configuration10.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration10["smp_title"] = "8/8RoutingPriorities";
                    configuration10["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(configuration10);
                }

                if (entityName == "systemuser")
                {
                    Entity systemUser = new Entity(entityName);
                    systemUser.Id = Guid.NewGuid();
                    systemUser["systemuserid"] = systemUser.Id;
                    collection.Entities.Add(systemUser);
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
                else if (entityName == "annotation")
                {
                    Entity annotation = new Entity(entityName);
                    annotation.Id = Guid.NewGuid();
                    annotation["annotationid"] = annotation.Id;
                    annotation["subject"] = "Sample subject";
                    annotation["notetext"] = "Sample text";
                    annotation["filename"] = "Sample filename4";
                    annotation["isdocument"] = true;
                    annotation["documentbody"] = "Sample body";
                    annotation["createdon"] = DateTime.Now.AddDays(-10);
                    collection.Entities.Add(annotation);
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

                    return incident1;
                }

                if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account.Id = new Guid("58d19398-b350-490a-9e2d-6b30391304a0");
                    account["name"] = "test";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_alias"] = "testalias";
                    contact["firstname"] = "first";
                    contact["lastname"] = "last";
                    contact["funllname"] = "last";
                    contact["emailaddress1"] = "test@microsoft.com";
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

            PostServiceRequestCreateEmailNotification pluginClass = new PostServiceRequestCreateEmailNotification();
            pluginClass.Execute(serviceProvider);
        }

        [TestMethod]
        public void SendStatusChangeEmail()
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
            incident.Attributes["customerid"] = new EntityReference("contact", new Guid());
            incident.Attributes["smp_allowemailnotification"] = true;
            incident["caseorigincode"] = new OptionSetValue(180620001);
            ////Entity preIncident = new Entity("incident");
            ////preIncident["incidentid"] = Guid.NewGuid();
            incident["statuscode"] = new OptionSetValue(2);
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////pluginContext.PostEntityImagesGet = () => { return postImage; };

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
                    Entity configuration9 = new Entity("smp_configuration");
                    configuration9.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration9["smp_title"] = "Sample One";
                    configuration9["smp_value"] = "testsample@microsoft.com";
                    collection.Entities.Add(configuration9);
                    Entity configuration10 = new Entity("smp_configuration");
                    configuration10.Id = new Guid();
                    configuration10["smp_title"] = "sample name";
                    configuration10["smp_value"] = "04/10/2018";
                    collection.Entities.Add(configuration10);
                    Entity configuration11 = new Entity("smp_configuration");
                    configuration11.Id = new Guid();
                    configuration11["smp_title"] = "en-us";
                    configuration11["smp_value"] = "2018-09-09";
                    collection.Entities.Add(configuration11);
                    Entity configuration12 = new Entity("smp_configuration");
                    configuration12.Id = new Guid();
                    configuration12["smp_title"] = "ServiceRequestConfirmation/ToBeScheduled";
                    configuration12["smp_value"] = "2018-09-09";
                    collection.Entities.Add(configuration12);
                    Entity configuration13 = new Entity("smp_configuration");
                    configuration13.Id = new Guid();
                    configuration13["smp_title"] = "ServiceRequestPortalLink";
                    configuration13["smp_value"] = "https://test.microsofotcrmportals.com";
                    collection.Entities.Add(configuration13);
                    Entity configuration14 = new Entity("smp_configuration");
                    configuration14.Id = new Guid();
                    configuration14["smp_title"] = "ServiceRequestSurveyLink";
                    configuration14["smp_value"] = "https://test@survey.com";
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
                }

                if (entityName == "systemuser")
                {
                    Entity systemUser = new Entity(entityName);
                    systemUser.Id = Guid.NewGuid();
                    systemUser["systemuserid"] = systemUser.Id;
                    collection.Entities.Add(systemUser);
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
                else if (entityName == "annotation")
                {
                    Entity annotation = new Entity(entityName);
                    annotation.Id = Guid.NewGuid();
                    annotation["annotationid"] = annotation.Id;
                    annotation["subject"] = "Sample subject";
                    annotation["notetext"] = "Sample text";
                    annotation["filename"] = "Sample filename4";
                    annotation["isdocument"] = true;
                    annotation["documentbody"] = "Sample body";
                    annotation["createdon"] = DateTime.Now.AddDays(-10);
                    collection.Entities.Add(annotation);
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

                    return incident1;
                }

                if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account.Id = new Guid("58d19398-b350-490a-9e2d-6b30391304a0");
                    account["name"] = "test";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
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

            PostServiceRequestCreateEmailNotification pluginClass = new PostServiceRequestCreateEmailNotification();
            pluginClass.Execute(serviceProvider);
        }
    }
}
