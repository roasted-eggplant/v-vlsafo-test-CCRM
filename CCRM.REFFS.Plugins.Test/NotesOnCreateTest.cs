//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="NotesOnCreateTest.cs" company="Microsoft">
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
namespace CCRM.REFFS.Plugins.Test
{
    using CCRM.REFFS.Plugins.Fakes;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Fakes;
    using System.Linq;

    [TestClass]
    public class NotesOnCreateTest
    {
        public static void PluginVariables(StubIServiceProvider serviceProvider, StubIPluginExecutionContext pluginContext, IOrganizationService organizationService, int stageNumber, string messageName)
        {
            var serviceFactory = new StubIOrganizationServiceFactory();
            var tracingService = new StubITracingService();

            if (serviceProvider != null)
            {
                serviceProvider.GetServiceType = type =>
                {
                    if (type == typeof(IPluginExecutionContext))
                    {
                        return pluginContext;
                    }
                    else if (type == typeof(ITracingService))
                    {
                        return tracingService;
                    }
                    else if (type == typeof(IOrganizationServiceFactory))
                    {
                        return serviceFactory;
                    }

                    return null;
                };
            }

            pluginContext.DepthGet = () => 1;
            pluginContext.UserIdGet = () => new Guid();
            pluginContext.MessageNameGet = () => messageName;
            pluginContext.StageGet = () => stageNumber;
            pluginContext.InitiatingUserIdGet = () => Guid.Parse("F83DA6A6-748E-E411-9412-00155D614A70");
            pluginContext.CorrelationIdGet = () => new Guid();
            pluginContext.PrimaryEntityIdGet = () =>
            {
                return Guid.NewGuid();
            };

            serviceFactory.CreateOrganizationServiceNullableOfGuid = t1 => organizationService;

            tracingService.TraceStringObjectArray = Trace;
        }

        public static void Trace(string message, params object[] value)
        {
        }

        [TestCategory("Standard")]
        [TestMethod]
        public void NotesOnCreateTestMethod()
        {
            using (ShimsContext.Create())
            {
                NotesOnCreate serviceRequestOnUpdate = new StubNotesOnCreate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                Entity workorder = new Entity("msdyn_workorder");
                workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");

                pluginContext.PrimaryEntityNameGet = () => "annotation";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new ParameterCollection();
                Entity target = new Entity("annotation");
                target.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                target.Attributes["objectid"] = new EntityReference(workorder.LogicalName, workorder.Id);
                target.Attributes["documentbody"] = "gaksdgkasdkasdh";
                target.Attributes["mimetype"] = "text/plain";
                target.Attributes["filename"] = "testfile.txt";
                paramCollection.Add("Target", target);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
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

                    if (entityName == "msdyn_workorder")
                    {
                        workorder.Attributes["smp_tier1provider"] = "CBRE";
                        collection.Entities.Add(workorder);
                    }

                    if (entityName == "smp_configuration")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        configuration["smp_title"] = "AttachmentBlobUrl";
                        configuration["smp_value"] = "https://attachmentblogurl.com";
                        collection.Entities.Add(configuration);
                    }

                    return collection;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }

        [TestCategory("Standard")]
        [TestMethod]
        public void NotesOnCreatePurchaseTestMethod()
        {
            using (ShimsContext.Create())
            {
                NotesOnCreate serviceRequestOnUpdate = new StubNotesOnCreate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                Entity purchaseorder = new Entity("msdyn_purchaseorder");
                purchaseorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");

                pluginContext.PrimaryEntityNameGet = () => "annotation";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new ParameterCollection();
                Entity target = new Entity("annotation");
                target.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                target.Attributes["objectid"] = new EntityReference(purchaseorder.LogicalName, purchaseorder.Id);
                target.Attributes["documentbody"] = "gaksdgkasdkasdh";
                target.Attributes["mimetype"] = "text/plain";
                target.Attributes["filename"] = "testfile.txt";
                paramCollection.Add("Target", target);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
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

                    if (entityName == "msdyn_workorder")
                    {
                        purchaseorder.Attributes["smp_tier1provider"] = "CBRE";
                        collection.Entities.Add(purchaseorder);
                    }

                    if (entityName == "smp_configuration")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        configuration["smp_title"] = "AttachmentBlobUrl";
                        configuration["smp_value"] = "https://attachmentblogurl.com";
                        collection.Entities.Add(configuration);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "msdyn_purchaseorder")
                    {
                        Entity pr = new Entity
                        {
                            LogicalName = "msdyn_purchaseorder",
                            Id = new Guid("884A078B-0467-E711-80F5-3863B43C0680")
                        };
                        return pr;
                    }

                    return null;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }

        [TestCategory("Standard")]
        [TestMethod]
        public void NotesOnCreatePurchaseOrderTestMethod()
        {
            using (ShimsContext.Create())
            {
                NotesOnCreate serviceRequestOnUpdate = new StubNotesOnCreate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                Entity purchaseorderProduct = new Entity("msdyn_purchaseorderproduct");
                purchaseorderProduct.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");

                pluginContext.PrimaryEntityNameGet = () => "annotation";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new ParameterCollection();
                Entity target = new Entity("annotation");
                target.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                target.Attributes["objectid"] = new EntityReference(purchaseorderProduct.LogicalName, purchaseorderProduct.Id);
                target.Attributes["documentbody"] = "gaksdgkasdkasdh";
                target.Attributes["mimetype"] = "text/plain";
                target.Attributes["filename"] = "testfile.txt";
                paramCollection.Add("Target", target);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
                        {
                            entityName = "ava_keyvaluepair";
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

                    if (entityName == "msdyn_workorder")
                    {
                        purchaseorderProduct.Attributes["smp_tier1provider"] = "CBRE";
                        collection.Entities.Add(purchaseorderProduct);
                    }

                    if (entityName == "smp_configuration")
                    {
                        Entity configuration = new Entity("smp_configuration");
                        configuration.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        configuration["smp_title"] = "AttachmentBlobUrl";
                        configuration["smp_value"] = "https://attachmentblogurl.com";
                        collection.Entities.Add(configuration);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "msdyn_purchaseorderProduct")
                    {
                        Entity prproduct = new Entity
                        {
                            LogicalName = "msdyn_purchaseorderProduct",
                            Id = new Guid("884A078B-0467-E711-80F5-3863B43C0680")
                        };
                        return prproduct;
                    }

                    return null;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }
    }
}