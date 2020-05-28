//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="PostServiceRequestsmp_ReclassifyTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  NotesOnCreateTest Plug in Test
//// </summary>
//// --------------------------------------------------------------------------------------------------------------------
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
    public class PostServiceRequestsmp_ReclassifyTest
    {
        [TestMethod]
        public void PostServiceRequestsmp_ReclassifyTestMethod()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            ParameterCollection paramCollection = new ParameterCollection();
            Microsoft.Xrm.Sdk.ParameterCollection paramCollectionPostImage = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            incident.Attributes["smp_incidentid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
            incident.Attributes["incidentid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
            incident.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            incident.Attributes["smp_createdfrom"] = new OptionSetValue(3);
            incident.Attributes["smp_referencesr"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
            incident.Attributes["smp_flag"] = false;
            incident.Attributes["smp_problemoccureddatetime"] = "2018-01-08";
            incident.Attributes["smp_portalsubmit"] = false;
            paramCollection.Add("Target", new EntityReference(incident.LogicalName, incident.Id));
            pluginContext.InputParametersGet = () => paramCollection;
            pluginContext.OutputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection
            {
                (new KeyValuePair<string, Entity>("PostImage", incident))
            };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "smp_Reclassify", postImage);
            organizationService.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("FetchExpression"))
                {
                    if (((FetchExpression)query).Query.Contains("<entity name='annotation'>"))
                    {
                        entityName = "annotation";
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
              
               if (entityName == "smp_servicerequestproblemtypedesc")
                {
                    Entity servicerequestproblemtype = new Entity(entityName);
                    servicerequestproblemtype.Id = Guid.NewGuid();
                    servicerequestproblemtype["smp_servicerequestproblemtypedescid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtype["smp_answer"] = "Sample Answer";
                    servicerequestproblemtype["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0466-E711-80F5-3863BB3C0560"));
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
                    annotation["objectid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                    annotation["objecttypecode"] = "incident";
                    collection.Entities.Add(annotation);
                }
               else if (entityName == "account")
                {
                    Entity account = new Entity(entityName);
                    account.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    account["name"] = "test";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    collection.Entities.Add(account);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "incident")
                {
                    Entity incident2 = new Entity(entity);
                    incident2.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    incident2["incidentid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                    incident2["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    incident2["smp_createdfrom"] = new OptionSetValue(3);
                    incident2["smp_referencesr"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                    incident2["smp_flag"] = false;
                    incident2["smp_problemoccureddatetime"] = "2018-01-08";
                    incident2["smp_portalsubmit"] = false;
                    return incident2;
                }

                if (entity == "account")
                {
                    Entity account = new Entity(entity);
                    account.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    account["name"] = "test";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "https://testurl/cmmsservicerequestmanagerAAD.svc";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    return account;
                }
             
                return null;
            };
            Guid expected = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Entity clonedEntity = null;

            organizationService.CreateEntity = e =>
            {
                clonedEntity = e;
                return expected;
            };

            PostServiceRequestsmp_Reclassify reclassify = new PostServiceRequestsmp_Reclassify();
            reclassify.Execute(serviceProvider);
        }
    }
}
