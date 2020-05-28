//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="UpdateNotesFormSRToWOTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  UpdateNotesFormSRToWOTest Plug in Test
//// </summary>
//// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Test
{
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Fakes;

    [TestClass]
    public class UpdateNotesFormSRToWOTest
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

        [TestMethod]
        public void UpdateNotesFormSRToWOTestMethod()
        {
            using (ShimsContext.Create())
            {
                UpdateNotesFormSRToWO updateNotesFormSRToWO = new UpdateNotesFormSRToWO();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "annotation";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity annotation = new Entity("annotation");
                annotation.Attributes["objectid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                annotation.Attributes["filename"] = "filename";
                annotation.Attributes["documentbody"] = "documentbody";
                annotation.Attributes["notetext"] = "notetext";
                annotation.Attributes["objecttypecode"] = "objecttypecode";
                annotation.Attributes["mimetype"] = "mimetype";
                annotation.Attributes["subject"] = "subject";
                paramCollection.Add("Target", annotation);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((Microsoft.Xrm.Sdk.Query.FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
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
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(workorder);
                    }

                    return collection;
                };

                updateNotesFormSRToWO.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void UpdateNotesFormSRToWOTestMethod2()
        {
            using (ShimsContext.Create())
            {
                UpdateNotesFormSRToWO updateNotesFormSRToWO = new UpdateNotesFormSRToWO();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "annotation";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity annotation = new Entity("annotation");
                annotation.Attributes["objectid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                annotation.Attributes["filename"] = "filename";
                annotation.Attributes["documentbody"] = "documentbody";
                ////annotation.Attributes["notetext"] = "notetext";
                annotation.Attributes["objecttypecode"] = "objecttypecode";
                annotation.Attributes["mimetype"] = "mimetype";
                annotation.Attributes["subject"] = "subject";
                paramCollection.Add("Target", annotation);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((Microsoft.Xrm.Sdk.Query.FetchExpression)query).Query.Contains("<entity name='ava_keyvaluepair'>"))
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
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(workorder);
                    }

                    return collection;
                };

                updateNotesFormSRToWO.Execute(serviceProvider);
            }
        }
    }
}