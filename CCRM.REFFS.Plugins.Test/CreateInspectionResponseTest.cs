//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="CreateInspectionResponseTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////   public class CreateInspectionResponseTest Plug in Test
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

    [TestClass]
    public class CreateInspectionResponseTest
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
            pluginContext.InitiatingUserIdGet = () => Guid.Parse("F83DA6A6-748E-E411-9412-00155D614A70");
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
        public void CreateInspectionResponseTestMethod()
        {
            using (ShimsContext.Create())
            {
                CreateInspectionResponse createInspectionResponse = new StubCreateInspectionResponse();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "smp_inspection";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E512-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity inspection = new Entity("smp_inspection");
                inspection.Attributes["smp_inspectiontype"] = new EntityReference("smp_inspectiontype", new Guid("884A078B-0467-E721-80F5-3863BB3C0660"));
                inspection.Attributes["ownerid"] = new EntityReference("systemuser", new Guid("884A078B-0467-E721-80F5-3863BB3C0668"));
                paramCollection.Add("Target", inspection);

                pluginContext.InputParametersGet = () => paramCollection;

                ////Entity incidentImage = new Entity("msdyn_workorder");
                ////incidentImage.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                ////var postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incidentImage)) };
                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", null);

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((FetchExpression)query).Query.Contains("<entity name='smp_inspectionquestion'>"))
                        {
                            entityName = "smp_inspectionquestion";
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

                    if (entityName == "smp_inspectionquestion")
                    {
                        Entity question = new Entity("smp_inspectionquestion");
                        question.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        question.Attributes["smp_questiontext"] = "unittest1";
                        collection.Entities.Add(question);
                    }

                    return collection;
                };

                createInspectionResponse.Execute(serviceProvider);
            }
        }
    }
}