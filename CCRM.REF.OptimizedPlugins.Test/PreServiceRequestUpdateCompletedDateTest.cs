// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestUpdateCompletedDateTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PreServiceRequestUpdateCompletedDateTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins.Test
{
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
    public class PreServiceRequestUpdateCompletedDateTest
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
            incident.Attributes["statuscode"] = new OptionSetValue(180620011);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
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

                if (entityName == "smp_timezone")
                {
                    Entity timeZone = new Entity(entityName);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    collection.Entities.Add(timeZone);
                }

                return collection;
            };
            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
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
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }

                return null;
            };

            PreServiceRequestUpdateCompletedDate completionDate = new PreServiceRequestUpdateCompletedDate();
            completionDate.Execute(serviceProvider);
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
            incident.Attributes["statuscode"] = new OptionSetValue(180620011);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
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

                if (entityName == "smp_timezone")
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
                if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-02:30", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-03:30", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-04:30", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }

                return null;
            };

            PreServiceRequestUpdateCompletedDate completionDate = new PreServiceRequestUpdateCompletedDate();
            completionDate.Execute(serviceProvider);
        }

        [TestMethod]
        public void UpdateCompletionDate3()
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
            incident.Attributes["statuscode"] = new OptionSetValue(180620011);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
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

                if (entityName == "smp_timezone")
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
                if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+02", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+03", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("+04", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }

                return null;
            };

            PreServiceRequestUpdateCompletedDate completionDate = new PreServiceRequestUpdateCompletedDate();
            completionDate.Execute(serviceProvider);
        }

        [TestMethod]
        public void UpdateCompletionDate4()
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
            incident.Attributes["statuscode"] = new OptionSetValue(180620011);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C0652")) { Name = "building" };
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

                if (entityName == "smp_timezone")
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
                if (entity == "smp_timezone")
                {
                    Entity timeZone = new Entity(entity);
                    timeZone["smp_timezonename"] = "test timezone";
                    timeZone["smp_offset"] = new OptionSetValue(2);
                    return timeZone;
                }

                return null;
            };

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+02:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-02", 1033) })), 0));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+03:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-03", 1033) })), 1));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label(new LocalizedLabel("+04:30", 1033), (new LocalizedLabel[] { new LocalizedLabel("-04", 1033) })), 2));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }

                return null;
            };

            PreServiceRequestUpdateCompletedDate completionDate = new PreServiceRequestUpdateCompletedDate();
            completionDate.Execute(serviceProvider);
        }
    }
}
