// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestUpdateSetIntegrationStatusTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PreServiceRequestUpdateSetIntegrationStatusTest
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
    public class PreServiceRequestUpdateSetIntegrationStatusTest
    {
        [TestMethod]
        public void UpdateCompletionDate()
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
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", incident)) };
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
                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "DraftStatusCode";
                    configuration6["smp_value"] = "DRAFT";
                    collection.Entities.Add(configuration6);
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

                if (entity == "incident")
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

            PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
            setIntegrationStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void UpdateCompletionDate2()
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
            incident.Attributes["statuscode"] = new OptionSetValue(3);
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
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", incident)) };
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
                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "DraftStatusCode";
                    configuration6["smp_value"] = "DRAFT";
                    collection.Entities.Add(configuration6);
                }
                else if (entityName == "smp_servicerequeststatuscode")
                {
                    Entity srStatusCode = new Entity(entityName);
                    srStatusCode["smp_name"] = "test";
                    srStatusCode["smp_servicerequeststatus"] = new OptionSetValue(1);
                    collection.Entities.Add(srStatusCode);
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

                if (entity == "incident")
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

            PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
            setIntegrationStatus.Execute(serviceProvider);
        }

        [TestClass]
        public class PreServiceRequestUpdateSetIntegrationStatusSurveyTest
        {
            [TestMethod]
            public void UpdateCompletionDate()
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
                incident["smp_issurveyservicerequest"] = true;

                paramCollection.Add("Target", incident);
                pluginContext.InputParametersGet = () => paramCollection;
                EntityImageCollection preImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PreImage", incident)) };
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
                        Entity configuration6 = new Entity("smp_configuration");
                        configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        configuration6["smp_title"] = "DraftStatusCode";
                        configuration6["smp_value"] = "DRAFT";
                        collection.Entities.Add(configuration6);
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

                    if (entity == "incident")
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

                PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
                setIntegrationStatus.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void UpdateCompletionDatewithPriority()
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
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };

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
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-81F5-3863BB3C0560");
                    configuration5["smp_title"] = "8/8RoutingPriorities";
                    configuration5["smp_value"] = "P1,P2,PS1,PS2,PS3";
                    collection.Entities.Add(configuration5);

                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "DraftStatusCode";
                    configuration6["smp_value"] = "DRAFT";
                    collection.Entities.Add(configuration6);
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
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_name"] = "Priority";
                    return priority;
                }

                return null;
            };

            PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
            setIntegrationStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void UpdateCompletionDatewithPriority1()
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
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(2);

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
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-81F5-3863BB3C0560");
                    configuration5["smp_title"] = "8/8RoutingPriorities";
                    configuration5["smp_value"] = "P1,P2,PS1,PS2,PS3";
                    collection.Entities.Add(configuration5);

                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "DraftStatusCode";
                    configuration6["smp_value"] = "DRAFT";
                    collection.Entities.Add(configuration6);
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
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_name"] = "P1";
                    return priority;
                }

                return null;
            };

            PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
            setIntegrationStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void UpdateCompletionDatewithPriority2()
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
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "P1" };
            preIncident.Attributes["statuscode"] = new OptionSetValue(1);

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
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-81F5-3863BB3C0560");
                    configuration5["smp_title"] = "8/8RoutingPriorities";
                    configuration5["smp_value"] = "P1,P2,PS1,PS2,PS3";
                    collection.Entities.Add(configuration5);

                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "DraftStatusCode";
                    configuration6["smp_value"] = "DRAFT";
                    collection.Entities.Add(configuration6);
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
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_name"] = "P1";
                    return priority;
                }

                return null;
            };

            PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
            setIntegrationStatus.Execute(serviceProvider);
        }

        [TestMethod]
        public void UpdateCompletionDatewithoutPriority()
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
            incident["modifiedon"] = "2018-01-08";
            incident["smp_problemoccureddatetime"] = "2018-01-08";
            incident["smp_createdfrom"] = new OptionSetValue(1);
            incident["smp_integrationstatus"] = true;
            incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
            incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
            incident["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "contact" };
            incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "roomtype" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            paramCollection.Add("Target", incident);

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            ////preIncident["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };

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
                    Entity configuration5 = new Entity("smp_configuration");
                    configuration5.Id = new Guid("884A078B-0466-E711-81F5-3863BB3C0560");
                    configuration5["smp_title"] = "8/8RoutingPriorities";
                    configuration5["smp_value"] = "P1,P2,PS1,PS2,PS3";
                    collection.Entities.Add(configuration5);

                    Entity configuration6 = new Entity("smp_configuration");
                    configuration6.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    configuration6["smp_title"] = "DraftStatusCode";
                    configuration6["smp_value"] = "DRAFT";
                    collection.Entities.Add(configuration6);
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
                else if (entity == "smp_priority")
                {
                    Entity priority = new Entity(entity);
                    priority["smp_name"] = "Priority";
                    return priority;
                }

                return null;
            };

            PreServiceRequestUpdateSetIntegrationStatus setIntegrationStatus = new PreServiceRequestUpdateSetIntegrationStatus();
            setIntegrationStatus.Execute(serviceProvider);
        }
    }
}
