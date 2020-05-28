//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="ServiceRequestOnUpdateTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  ServiceRequestOnUpdateTest Plug in Test
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
    public class ServiceRequestOnUpdateTest
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
        public void ServiceRequestOnUpdateofProblemDescription()
        {
            using (ShimsContext.Create())
            {
                ServiceRequestOnUpdate serviceRequestOnUpdate = new StubServiceRequestOnUpdate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["smp_problemtypedescription"] = "updating the problem description";
                paramCollection.Add("Target", targetIncident);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(workorder);
                    }

                    return collection;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void ServiceRequestOnUpdateofCostCenterCode()
        {
            using (ShimsContext.Create())
            {
                ServiceRequestOnUpdate serviceRequestOnUpdate = new StubServiceRequestOnUpdate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["smp_costcentercode"] = "1234890";
                paramCollection.Add("Target", targetIncident);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(workorder);
                    }

                    return collection;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void ServiceRequestOnUpdateofIOCode()
        {
            using (ShimsContext.Create())
            {
                ServiceRequestOnUpdate serviceRequestOnUpdate = new StubServiceRequestOnUpdate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["smp_iocode"] = "345678";
                paramCollection.Add("Target", targetIncident);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(workorder);
                    }

                    return collection;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void ServiceRequestOnUpdateofIOCodeTest()
        {
            using (ShimsContext.Create())
            {
                ServiceRequestOnUpdate serviceRequestOnUpdate = new StubServiceRequestOnUpdate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["smp_requestorid"] = new EntityReference("contact", new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1"));
                targetIncident.Attributes["smp_requestoremail"] = "test@test.com";
                targetIncident.Attributes["smp_requestorphone"] = "99999999";
                targetIncident.Attributes["smp_contact"] = new EntityReference("contact", new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1"));
                targetIncident.Attributes["smp_contactphone"] = "99999999";
                targetIncident.Attributes["smp_contactemail"] = "test@test.com";
                targetIncident.Attributes["smp_requestedduedate"] = new DateTime(2018, 1, 8);
                targetIncident.Attributes["smp_problemtypedescription"] = "test";
                targetIncident.Attributes["smp_costcentercode"] = "11223";
                targetIncident.Attributes["smp_iocode"] = "345678";
                paramCollection.Add("Target", targetIncident);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(workorder);
                    }

                    return collection;
                };

                serviceRequestOnUpdate.Execute(serviceProvider);
            }
        }
    }
}
