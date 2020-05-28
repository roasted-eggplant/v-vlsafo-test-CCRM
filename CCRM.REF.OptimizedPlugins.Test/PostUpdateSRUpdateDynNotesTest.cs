// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostUpdateSRUpdateDynNotesTest.cs" company="Microsoft">
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

    using System.Collections.Generic;
    using System.Fakes;
    [TestClass]
    public class PostUpdateSRUpdateDynNotesTest
    {
        ////Unused Variable Reported in Fortify Scan
        //PostUpdateSRUpdateDynNotes servicerequestUpdate = new PostUpdateSRUpdateDynNotes();
        StubIServiceProvider serviceProvider = new StubIServiceProvider();
        StubIPluginExecutionContext pluginContext = new StubIPluginExecutionContext();
        ////Unused Variable Reported in Fortify Scan
        //StubIPluginExecutionContext organizationServiceContext = new StubIPluginExecutionContext();
        StubIOrganizationService organizationService = new StubIOrganizationService();

        [TestMethod]
        public void SRUpdate()
        {
            {
                this.pluginContext.PrimaryEntityNameGet = () => "incident";
                this.pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
                Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
                Entity incident = new Entity("incident");
                incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                incident["smp_createdfrom"] = new OptionSetValue(1);
                incident["smp_portalsubmit"] = true;
                incident.Attributes["caseorigincode"] = new OptionSetValue(3);
                incident.Attributes["smp_reclassifiedsr"] = new EntityReference("smp_reclassifiedsr", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "incident" };
                incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                paramCollection.Add("Target", incident);
                this.pluginContext.InputParametersGet = () => paramCollection;
                EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
                Helper.Helper.PluginVariables(this.serviceProvider, this.pluginContext, this.organizationService, 40, "Update", postImage);

                this.organizationService.RetrieveMultipleQueryBase = (query) =>
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

                    if (entityName == "smp_problemtypedescription")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionid"] = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionname"] = "Sample Name";

                        collection.Entities.Add(configuration);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity servicerequestproblemtype = new Entity(entityName);
                        servicerequestproblemtype.Id = Guid.NewGuid();
                        servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_answer"] = "Sample Answer";
                        servicerequestproblemtype["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(servicerequestproblemtype);
                    }

                    return collection;
                };

                this.organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "incident")
                    {
                        Entity incidents = new Entity(entity);
                        incidents["caseorigincode"] = new OptionSetValue(1);
                        incidents["smp_createdfrom"] = new OptionSetValue(1);
                        incident["smp_portalsubmit"] = true;
                        incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                        incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        return incidents;
                    }
                    else if (entity == "smp_servicerequestproblemtypedesc")
                    {
                        Entity dpnEntity = new Entity(entity);
                        dpnEntity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        dpnEntity["statecode"] = 1;
                        dpnEntity["statuscode"] = 2;
                    }

                    return null;
                };

                PostUpdateSRUpdateDynNotes srUpdateDynNotes = new PostUpdateSRUpdateDynNotes();
                srUpdateDynNotes.Execute(this.serviceProvider);
            }
        }

        [TestMethod]
        public void SRUpdate1()
        {
            {
                this.pluginContext.PrimaryEntityNameGet = () => "incident";
                this.pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
                ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
                Entity incident = new Entity("incident");
                incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                incident["smp_createdfrom"] = new OptionSetValue(3);
                incident.Attributes["caseorigincode"] = new OptionSetValue(1);
                incident["smp_portalsubmit"] = true;
                incident.Attributes["smp_reclassifiedsr"] = new EntityReference("smp_reclassifiedsr", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "incident" };
                incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                paramCollection.Add("Target", incident);
                this.pluginContext.InputParametersGet = () => paramCollection;
                EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
                Helper.Helper.PluginVariables(this.serviceProvider, this.pluginContext, this.organizationService, 40, "Update", postImage);

                this.organizationService.RetrieveMultipleQueryBase = (query) =>
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

                    if (entityName == "smp_problemtypedescription")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionid"] = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionname"] = "Sample Name";

                        collection.Entities.Add(configuration);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity servicerequestproblemtype = new Entity(entityName);
                        servicerequestproblemtype.Id = Guid.NewGuid();
                        servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_answer"] = "Sample Answer";
                        servicerequestproblemtype["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(servicerequestproblemtype);
                    }

                    return collection;
                };

                this.organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "incident")
                    {
                        Entity incidents = new Entity(entity);
                        incidents["caseorigincode"] = new OptionSetValue(1);
                        incidents["smp_createdfrom"] = new OptionSetValue(1);
                        incident["smp_portalsubmit"] = true;
                        incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());

                        incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        return incidents;
                    }
                    else if (entity == "smp_servicerequestproblemtypedesc")
                    {
                        Entity dpnEntity = new Entity(entity);
                        dpnEntity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        dpnEntity["statecode"] = 1;
                        dpnEntity["statuscode"] = 2;
                    }

                    return null;
                };

                PostUpdateSRUpdateDynNotes srupdatedynnotes = new PostUpdateSRUpdateDynNotes();
                srupdatedynnotes.Execute(this.serviceProvider);
            }
        }

        [TestMethod]
        public void SRUpdate2()
        {
            {
                this.pluginContext.PrimaryEntityNameGet = () => "incident";
                this.pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                ParameterCollection paramCollection = new ParameterCollection();
                ParameterCollection paramCollectionPostImage = new ParameterCollection();
                Entity incident = new Entity("incident");
                incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                incident["smp_createdfrom"] = new OptionSetValue(3);
                incident.Attributes["caseorigincode"] = new OptionSetValue(1);
                incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                paramCollection.Add("Target", incident);
                this.pluginContext.InputParametersGet = () => paramCollection;
                EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
                Helper.Helper.PluginVariables(this.serviceProvider, this.pluginContext, this.organizationService, 40, "Update", postImage);

                this.organizationService.RetrieveMultipleQueryBase = (query) =>
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

                    if (entityName == "smp_problemtypedescription")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionid"] = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionname"] = "Sample Name";

                        collection.Entities.Add(configuration);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity servicerequestproblemtype = new Entity(entityName);
                        servicerequestproblemtype.Id = Guid.NewGuid();
                        servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_answer"] = "Sample Answer";
                        servicerequestproblemtype["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(servicerequestproblemtype);
                    }

                    return collection;
                };

                this.organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "incident")
                    {
                        Entity incidents = new Entity(entity);
                        incidents["caseorigincode"] = new OptionSetValue(1);
                        incidents["smp_createdfrom"] = new OptionSetValue(1);
                        incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incident["smp_portalsubmit"] = true;
                        incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        return incidents;
                    }
                    else if (entity == "smp_servicerequestproblemtypedesc")
                    {
                        Entity dpnEntity = new Entity(entity);
                        dpnEntity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        dpnEntity["statecode"] = 1;
                        dpnEntity["statuscode"] = 2;
                    }

                    return null;
                };

                PostUpdateSRUpdateDynNotes srupdatedynnotes = new PostUpdateSRUpdateDynNotes();
                srupdatedynnotes.Execute(this.serviceProvider);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void SRUpdate3()
        {
            {
                this.pluginContext.PrimaryEntityNameGet = () => "incident";
                this.pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                ParameterCollection paramCollection = new ParameterCollection();
                ParameterCollection paramCollectionPostImage = new ParameterCollection();
                Entity incident = new Entity("incident");
                incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                incident["smp_createdfrom"] = new OptionSetValue(3);
                incident["smp_portalsubmit"] = true;
                ////incident.Attributes["caseorigincode"] = new OptionSetValue(1);
                incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                paramCollection.Add("Target", incident);
                this.pluginContext.InputParametersGet = () => paramCollection;
                EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
                Helper.Helper.PluginVariables(this.serviceProvider, this.pluginContext, this.organizationService, 40, "Update", postImage);

                this.organizationService.RetrieveMultipleQueryBase = (query) =>
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

                    if (entityName == "smp_problemtypedescription")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionid"] = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                        configuration["smp_problemtypedescriptionname"] = "Sample Name";

                        collection.Entities.Add(configuration);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity servicerequestproblemtype = new Entity(entityName);
                        servicerequestproblemtype.Id = Guid.NewGuid();
                        servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                        servicerequestproblemtype["smp_answer"] = "Sample Answer";
                        servicerequestproblemtype["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(servicerequestproblemtype);
                    }

                    return collection;
                };

                this.organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "incident")
                    {
                        Entity incidents = new Entity(entity);
                        incidents["caseorigincode"] = new OptionSetValue(1);
                        incidents["smp_createdfrom"] = new OptionSetValue(1);
                        incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());
                        incident["smp_portalsubmit"] = true;
                        incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        return incidents;
                    }
                    else if (entity == "smp_servicerequestproblemtypedesc")
                    {
                        Entity dpnEntity = new Entity(entity);
                        dpnEntity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        dpnEntity["statecode"] = 1;
                        dpnEntity["statuscode"] = 2;
                    }

                    return null;
                };

                PostUpdateSRUpdateDynNotes srupdatedynnotes = new PostUpdateSRUpdateDynNotes();
                srupdatedynnotes.Execute(this.serviceProvider);
            }
        }

        [TestMethod]
        public void SRUpdate4()
        {
            this.pluginContext.PrimaryEntityNameGet = () => "incident";
            this.pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            ParameterCollection paramCollection = new ParameterCollection();
            ParameterCollection paramCollectionPostImage = new ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident["smp_createdfrom"] = new OptionSetValue(3);
            incident.Attributes["caseorigincode"] = new OptionSetValue(1);
            incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
            paramCollection.Add("Target", incident);
            this.pluginContext.InputParametersGet = () => paramCollection;

            Entity postIncident = new Entity("incident");
            postIncident["incidentid"] = Guid.NewGuid();
            postIncident["statuscode"] = new OptionSetValue(1);
            postIncident["smp_createdfrom"] = new OptionSetValue(3);
            postIncident["smp_portalsubmit"] = true;
            postIncident.Attributes["caseorigincode"] = new OptionSetValue(1);
            postIncident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
            postIncident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
            postIncident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };

            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", postIncident)) };
            Helper.Helper.PluginVariables(this.serviceProvider, this.pluginContext, this.organizationService, 40, "Update", postImage);

            this.organizationService.RetrieveMultipleQueryBase = (query) =>
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

                if (entityName == "smp_problemtypedescription")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                    configuration["smp_problemtypedescriptionid"] = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                    configuration["smp_problemtypedescriptionname"] = "Sample Name";

                    collection.Entities.Add(configuration);
                }

                if (entityName == "smp_servicerequestproblemtypedesc")
                {
                    Entity servicerequestproblemtype = new Entity(entityName);
                    servicerequestproblemtype.Id = Guid.NewGuid();
                    servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_answer"] = "Sample Answer";
                    servicerequestproblemtype["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                    servicerequestproblemtype["statecode"] = "Acive";
                    collection.Entities.Add(servicerequestproblemtype);
                }

                return collection;
            };

            this.organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incident["smp_portalsubmit"] = true;
                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }
                else if (entity == "smp_servicerequestproblemtypedesc")
                {
                    Entity dpnEntity = new Entity(entity);
                    dpnEntity.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    dpnEntity["statecode"] = 1;
                    dpnEntity["statuscode"] = 2;
                }

                return null;
            };

            PostUpdateSRUpdateDynNotes srupdatedynnotes = new PostUpdateSRUpdateDynNotes();
            srupdatedynnotes.Execute(this.serviceProvider);
        }
    }
}
