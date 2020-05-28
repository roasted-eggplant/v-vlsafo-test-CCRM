//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="AttachmentURLUpdateOnNoteDeleteTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  AttachmentURLUpdateOnNoteDeleteTest Plug in Test
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
    using System.Collections.Generic;
    using System.Fakes;

    [TestClass]
    public class AttachmentURLUpdateOnNoteDeleteTest
    {
        public static void PluginVariables(StubIServiceProvider serviceProvider, StubIPluginExecutionContext pluginContext, StubIOrganizationService organizationService, int stageNumber, string messageName, EntityImageCollection postImage)
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
            pluginContext.InitiatingUserIdGet = () => Guid.Parse("F83DA6A6-748E-E412-9412-00155D614A70");
            pluginContext.CorrelationIdGet = () => new Guid();
            pluginContext.PrimaryEntityIdGet = () =>
            {
                return Guid.NewGuid();
            };

            pluginContext.PostEntityImagesGet = () => { return postImage; };
            pluginContext.PreEntityImagesGet = () => { return postImage; };
            serviceFactory.CreateOrganizationServiceNullableOfGuid = t1 => organizationService;

            tracingService.TraceStringObjectArray = Trace;
        }

        public static void Trace(string message, params object[] value)
        {
        }

        [TestMethod]
        public void AttachmentURLUpdateOnNoteDeleteTestMethod()
        {
            using (ShimsContext.Create())
            {
                AttachmentURLUpdateOnNoteDelete attachmentURLUpdateOnNoteDelete = new StubAttachmentURLUpdateOnNoteDelete();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                pluginContext.PrimaryEntityNameGet = () => "annotation";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                EntityReference annotation = new EntityReference("annotation");
                annotation.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                paramCollection.Add("Target", annotation);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Delete", null);
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

                    if (entityName == "msdyn_customerasset")
                    {
                        Entity question = new Entity
                        {
                            LogicalName = "msdyn_customerasset",
                            Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0660")
                        };
                        //// question.Attributes["smp_questiontext"] = "unittest1";
                        collection.Entities.Add(question);
                    }

                    if (entityName == "smp_attachmentsurl")
                    {
                        Entity attachmentUrl = new Entity("smp_attachmentsurl");
                        attachmentUrl.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        attachmentUrl.Attributes["smp_objectid"] = "54D94FC2-52AD-E511-8158-1458D04DB4D1";
                        attachmentUrl.Attributes["smp_name"] = "Test";
                        collection.Entities.Add(attachmentUrl);
                    }

                    return collection;
                };

                attachmentURLUpdateOnNoteDelete.Execute(serviceProvider);
            }
        }
    }
}
