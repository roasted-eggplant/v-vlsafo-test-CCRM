// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestsmp_CopySRTest.cs" company="Microsoft">
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
    public class PostServiceRequestsmp_CopySRTest
    {
        [TestMethod]
        public void ServiceRequestsmp_CopySR()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("AE6226C5-4AB7-E811-A842-000D3A37C8E1");
            ParameterCollection paramCollection = new ParameterCollection();
            ParameterCollection paramCollectionPostImage = new ParameterCollection();
            EntityReference incident = new EntityReference("incident", new Guid("AE6226C5-4AB7-E811-A842-000D3A37C8E1"));
            Entity inc = new Entity("incident");
            inc["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
            inc["caseorigincode"] = new OptionSetValue(1);
            inc["smp_flag"] = false;
            inc["smp_createdfrom"] = new OptionSetValue(3);
            inc["smp_problemoccureddatetime"] = Convert.ToDateTime(DateTime.UtcNow);
            inc["smp_portalsubmit"] = false;

            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            pluginContext.OutputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", inc)) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "smp_CopySR", postImage);
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

                if (entityName == "account")
                {
                    Entity account = new Entity("account");
                    account["accountid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C0652");
                    account.Id = new Guid("884A078B-0469-E711-80F5-3863BB3C0560");
                    account["name"] = "test Provider";
                    account["smp_iscmmsintegrationenabled"] = true;
                    account["smp_cmmsurl"] = "testurl";
                    account["smp_hostedonazure"] = false;
                    account["smp_providerteam"] = new EntityReference("team", new Guid("884A078B-0467-E711-80F5-3863BB3C0652"));
                    collection.Entities.Add(account);
                }

                if (entityName == "smp_servicerequestproblemtypedesc")
                {
                    Entity servicerequestproblemtypedesc = new Entity("smp_servicerequestproblemtypedesc");
                    servicerequestproblemtypedesc.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    servicerequestproblemtypedesc["smp_problemtypedescriptionid"] = new EntityReference("smp_servicerequestproblemtypedesc", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    servicerequestproblemtypedesc["smp_answer"] = "8/8RoutingPriorities";
                    servicerequestproblemtypedesc["smp_value"] = "P1,P2,PS1,PS2";
                    collection.Entities.Add(servicerequestproblemtypedesc);
                    Entity servicerequest = new Entity("smp_servicerequest");
                    servicerequest.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    collection.Entities.Add(servicerequest);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "incident")
                {
                    inc["customerid"] = new EntityReference("account", new Guid("884A078B-0469-E711-80F5-3863BB3C0560"));
                    inc["caseorigincode"] = new OptionSetValue(1);
                    inc["smp_flag"] = false;
                    inc["smp_createdfrom"] = new OptionSetValue(3);
                    inc["smp_problemoccureddatetime"] = Convert.ToDateTime(DateTime.UtcNow);
                    inc["smp_portalsubmit"] = false;

                    return inc;
                }

                if (entity == "smp_servicerequestproblemtypedesc")
                {
                    Entity servicerequestproblemtypedesc = new Entity("smp_servicerequestproblemtypedesc");
                    servicerequestproblemtypedesc.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    servicerequestproblemtypedesc["smp_answer"] = "8/8RoutingPriorities";
                    servicerequestproblemtypedesc["smp_value"] = "P1,P2,PS1,PS2";
                    return servicerequestproblemtypedesc;
                }

                return null;
            };

            Guid expected = Guid.NewGuid();
            Entity clonedEntity = null;

            organizationService.CreateEntity = e =>
            {
                clonedEntity = e;
                return expected;
            };

            PostServiceRequestsmp_CopySR CopySR = new PostServiceRequestsmp_CopySR();
            CopySR.Execute(serviceProvider);
        }
    }
}
