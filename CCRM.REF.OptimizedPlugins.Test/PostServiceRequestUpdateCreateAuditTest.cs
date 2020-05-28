// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestUpdateCreateAuditTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostServiceRequestUpdateCreateAuditTest
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
    public class PostServiceRequestUpdateCreateAuditTest
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

            Entity preIncident = new Entity("incident");
            preIncident["incidentid"] = Guid.NewGuid();
            preIncident["statuscode"] = new OptionSetValue(1);

            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            EntityImageCollection preImage = new EntityImageCollection { new KeyValuePair<string, Entity>("PreImage", preIncident) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage, preImage);
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

                if (entityName == "smp_auditconfiguration")
                {
                    Entity auditConfiguration = new Entity(entityName);
                    auditConfiguration["smp_entityname"] = "incident";
                    auditConfiguration.Attributes["statuscode"] = new OptionSetValue(1);
                    auditConfiguration["smp_attributename"] = "smp_contact";
                    collection.Entities.Add(auditConfiguration);

                    Entity auditConfiguration1 = new Entity(entityName);
                    auditConfiguration1["smp_entityname"] = "incident";
                    auditConfiguration1.Attributes["statuscode"] = new OptionSetValue(1);
                    auditConfiguration1["smp_attributename"] = "smp_csr";
                    collection.Entities.Add(auditConfiguration1);

                    Entity auditConfiguration2 = new Entity(entityName);
                    auditConfiguration2["smp_entityname"] = "incident";
                    auditConfiguration2.Attributes["statuscode"] = new OptionSetValue(1);
                    auditConfiguration2["smp_attributename"] = "smp_duedate";
                    collection.Entities.Add(auditConfiguration2);
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

            organizationService.ExecuteOrganizationRequest = QueryBase =>
            {
                if (QueryBase.RequestName == "RetrieveAttribute")
                {
                    PicklistAttributeMetadata picklistAttributeMetadata = new PicklistAttributeMetadata();
                    picklistAttributeMetadata.OptionSet = new OptionSetMetadata();
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label("Option1", 1), null));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label("Option2", 2), null));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label("Option3", 3), null));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label("Option4", 4), null));
                    picklistAttributeMetadata.OptionSet.Options.Add(new OptionMetadata(new Label("Option5", 5), null));

                    RetrieveAttributeResponse response = new RetrieveAttributeResponse();
                    response.Results.Add("AttributeMetadata", picklistAttributeMetadata);

                    return response;
                }

                return null;
            };

            PostServiceRequestUpdateCreateAudit postServiceRequestUpdateCreateAudit = new PostServiceRequestUpdateCreateAudit();
            postServiceRequestUpdateCreateAudit.Execute(serviceProvider);
        }
    }
}
