// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestStatusChangeTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostServiceRequestStatusChangeTest
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
    public class PostServiceRequestStatusChangeTest
    {
        [TestMethod]
        public void StatusChange_DeclinedStatus()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(0);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            //// incident["statuscode"] = new OptionSetValue(1);
            incident["statuscode"] = new OptionSetValue(180620000);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void StatusChange_CancelledStatus_false()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(0);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["statuscode"] = new OptionSetValue(6);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void StatusChange_CancelledStatus_true()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(0);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["statuscode"] = new OptionSetValue(6);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["smp_cancelledforreclassification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void StatusChange_CompletedStatus()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(0);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["statuscode"] = new OptionSetValue(180620011);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void StatusChange_WaitingForApprovalStatus()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(0);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["statuscode"] = new OptionSetValue(180620013);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = true;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void StatusChange_statucodenotnull()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(180620009);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["statuscode"] = new OptionSetValue(180620017);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = false;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void StatusChange_prepostonetwo()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();

            Entity preIncident = new Entity("incident");
            preIncident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            preIncident["statuscode"] = new OptionSetValue(1);
            EntityImageCollection preImageCollection = new EntityImageCollection();
            preImageCollection.Add("PreImage", preIncident);
            ////pluginContext.PreEntityImagesGet = () => preImageCollection;

            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["statuscode"] = new OptionSetValue(2);
            incident["smp_problemclassid"] = new EntityReference("smp_problemclass", Guid.NewGuid());
            incident["smp_problemtypeid"] = new EntityReference("smp_problemtype", Guid.NewGuid());
            incident["smp_problembuilding"] = new EntityReference("smp_building", Guid.NewGuid());
            incident["smp_requestorid"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_cc"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_approver"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_contact"] = new EntityReference("contact", Guid.NewGuid());
            incident["smp_allowemailnotification"] = true;
            incident["customerid"] = new EntityReference("account", Guid.NewGuid());
            ////smp_problemclassid smp_problemtypeid smp_problembuilding smp_requestorid smp_contact
            paramCollection.Add("Target", incident);

            pluginContext.InputParametersGet = () => paramCollection;
            ////EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            ////Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", postImage);

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);

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
                    configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration["smp_title"] = "EmailSenderDomainName";
                    configuration["smp_value"] = "2";
                    collection.Entities.Add(configuration);
                }
                else if (entityName == "systemuser")
                {
                    Entity user = new Entity(entityName);
                    user["systemuserid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    user["domainname"] = "refuat@microsoft.com";
                    collection.Entities.Add(user);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_building")
                {
                    Entity building = new Entity(entity);
                    building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    building["smp_timezoneid"] = new EntityReference("smp_timezone", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    building["smp_isfinancialstatecampus"] = true;
                    building["smp_buildinglevelemailnotification"] = true;
                    return building;
                }
                else if (entity == "smp_problemclass")
                {
                    Entity problemClass = new Entity(entity);
                    problemClass.Id = Guid.NewGuid();
                    problemClass["smp_allowemailnotification"] = true;
                    problemClass["smp_donotallowsurvey"] = true;
                    return problemClass;
                }
                else if (entity == "smp_problemtype")
                {
                    Entity problemType = new Entity(entity);
                    problemType.Id = Guid.NewGuid();
                    problemType["smp_allowemailnotification"] = true;
                    problemType["smp_donotallowsurvey"] = true;
                    return problemType;
                }
                else if (entity == "contact")
                {
                    Entity contact = new Entity(entity);
                    contact.Id = Guid.NewGuid();
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_isprovideruser"] = false;
                    contact["smp_costcenter"] = "11110";
                    contact["smp_allowemailnotification"] = false;
                    contact["smp_preferredlanguage"] = "en-US";
                    contact["smp_alias"] = "test alias";
                    contact["firstname"] = "firstname";
                    contact["lastname"] = "lastname";
                    contact["emailaddress1"] = "email1@test.com";
                    return contact;
                }

                return null;
            };

            PostServiceRequestStatusChange postServiceRequestStatus = new PostServiceRequestStatusChange();
            postServiceRequestStatus.Execute(serviceProvider);
        }
    }
}
