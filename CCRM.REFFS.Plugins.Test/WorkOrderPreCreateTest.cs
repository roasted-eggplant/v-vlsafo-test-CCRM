//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="WorkOrderPreCreateTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  WorkOrderPreCreateTest Plug in Test
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
    public class WorkOrderPreCreateTest
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
        public void WorkOrderPreCreate_WithAttributeValues()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                WorkOrderPreCreate workOrderPreCreate = new StubWorkOrderPreCreate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                paramCollection.Add("Target", workOrder);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create");
                var iotAlert = new EntityReference("msdyn_iotalert", new Guid("884A078B-0469-E411-80F5-3863BB3C0560")) { Name = "ICONICS ALERT 57887" };
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

                    if (entityName == "msdyn_priority")
                    {
                        Entity priority = new Entity("msdyn_priority");
                        priority.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "msdyn_workordertype")
                    {
                        Entity priority = new Entity("msdyn_workordertype");
                        priority.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "msdyn_incidenttype")
                    {
                        Entity priority = new Entity("msdyn_incidenttype");
                        priority.Id = new Guid("884A078B-0469-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        collection.Entities.Add(incident);
                    }

                    if (entityName == "smp_providermatrix")
                    {
                        Entity annotation = new Entity("smp_providermatrix");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        annotation.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        annotation.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                        annotation.Attributes["smp_fieldserviceteam"] = new EntityReference("team", new Guid("884A078B-0469-E711-80E5-3863BB3C0560")) { Name = "Team1" };
                        annotation.Attributes["smp_billable"] = true;
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity incident = new Entity("smp_servicerequestproblemtypedesc");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                        incident.Attributes["name"] = "Service Request Problem Type Description";
                        incident.Attributes["smp_answer"] = "answer";
                        incident.Attributes["smp_servicerequestid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }
                    if (entityName == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    resultEntityName = entity;
                    if (entity == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                        incident.Attributes["caseorigincode"] = new OptionSetValue(915240000);
                        incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
                        incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
                        incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };

                        incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0469-E111-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_cc"] = new EntityReference("contact", new Guid("884A078B-0469-E211-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0469-E311-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_requestorid"] = new EntityReference("contact", new Guid("884A078B-0469-E411-80F5-3863BB3C0560")) { Name = "Contact" };
                        incident.Attributes["msdyn_iotalert"] = iotAlert;
                        incident.Attributes["smp_costcentercode"] = "123456";
                        incident.Attributes["smp_iocode"] = "123";
                        return incident;
                    }
                    else if (entity == "smp_building")
                    {
                        Entity building = new Entity("smp_building");
                        building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                        building.Attributes["smp_city"] = "city";
                        building.Attributes["smp_country"] = "Country";
                        building.Attributes["msdyn_address1"] = "Address1";
                        building.Attributes["smp_addressline2"] = "Address2";
                        building.Attributes["smp_state"] = "State";
                        building.Attributes["smp_zipcode"] = "ZipCode";
                        return building;
                    }                   

                    return null;
                };

               workOrderPreCreate.Execute(serviceProvider);
                var workOrderAfterPopulatingValues = workOrderPreCreate.LocalContext.PluginExecutionContext.InputParameters["Target"] as Entity;
                Assert.AreEqual(workOrderAfterPopulatingValues.GetAttributeValue<EntityReference>("msdyn_iotalert"), iotAlert);
                Assert.AreEqual("msdyn_iotalert", resultEntityName);
            }
        }

        [TestMethod]
        public void WorkOrderPreCreate_WithNullAttributeValues()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                WorkOrderPreCreate workOrderPreCreate = new StubWorkOrderPreCreate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                paramCollection.Add("Target", workOrder);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create");

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

                    if (entityName == "msdyn_priority")
                    {
                        Entity priority = new Entity("msdyn_priority");
                        priority.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "msdyn_workordertype")
                    {
                        Entity priority = new Entity("msdyn_workordertype");
                        priority.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "msdyn_incidenttype")
                    {
                        Entity priority = new Entity("msdyn_incidenttype");
                        priority.Id = new Guid("884A078B-0469-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        collection.Entities.Add(incident);
                    }

                    if (entityName == "smp_providermatrix")
                    {
                        Entity annotation = new Entity("smp_providermatrix");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        annotation.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        annotation.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                        annotation.Attributes["smp_fieldserviceteam"] = new EntityReference("team", new Guid("884A078B-0469-E711-80E5-3863BB3C0560")) { Name = "Team1" };
                        annotation.Attributes["smp_billable"] = true;
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity incident = new Entity("smp_servicerequestproblemtypedesc");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                        incident.Attributes["name"] = "Service Request Problem Type Description";
                        incident.Attributes["smp_answer"] = "answer";
                        incident.Attributes["smp_servicerequestid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }
                    if (entityName == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    resultEntityName = entity;
                    if (entity == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                        incident.Attributes["caseorigincode"] = new OptionSetValue(915240000);
                        incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
                        incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
                        incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };

                        incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0469-E111-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_cc"] = new EntityReference("contact", new Guid("884A078B-0469-E211-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0469-E311-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_requestorid"] = new EntityReference("contact", new Guid("884A078B-0469-E411-80F5-3863BB3C0560")) { Name = "Contact" };
                        //incident.Attributes["msdyn_iotalert"] = new EntityReference("msdyn_iotalert", new Guid("884A078B-0469-E411-80F5-3863BB3C0560")) { Name = "ICONICS ALERT 57887" };
                        incident.Attributes["smp_costcentercode"] = "123456";
                        incident.Attributes["smp_iocode"] = "123";
                        return incident;
                    }
                    else if (entity == "smp_building")
                    {
                        Entity building = new Entity("smp_building");
                        building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                        building.Attributes["smp_city"] = "city";
                        building.Attributes["smp_country"] = "Country";
                        building.Attributes["msdyn_address1"] = "Address1";
                        building.Attributes["smp_addressline2"] = "Address2";
                        building.Attributes["smp_state"] = "State";
                        building.Attributes["smp_zipcode"] = "ZipCode";
                        return building;
                    }

                    return null;
                };

                workOrderPreCreate.Execute(serviceProvider);
                var workOrderAfterPopulatingValues = workOrderPreCreate.LocalContext.PluginExecutionContext.InputParameters["Target"] as Entity;

                Assert.AreEqual(workOrderAfterPopulatingValues.GetAttributeValue<EntityReference>("msdyn_iotalert"), null);

                Assert.AreEqual("smp_building", resultEntityName);
            }
        }

        [TestCategory("Standard")]
        [TestMethod]
        public void WorkOrderPreCreateTestMethodForAgreement()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                WorkOrderPreCreate workOrderPreCreate = new StubWorkOrderPreCreate();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_agreement"] = new EntityReference("msdyn_agreement", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660")) { Name = "building" };
                paramCollection.Add("Target", workOrder);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create");

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

                    if (entityName == "msdyn_priority")
                    {
                        Entity priority = new Entity("msdyn_priority");
                        priority.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "msdyn_workordertype")
                    {
                        Entity priority = new Entity("msdyn_workordertype");
                        priority.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "msdyn_incidenttype")
                    {
                        Entity priority = new Entity("msdyn_incidenttype");
                        priority.Id = new Guid("884A078B-0469-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(priority);
                    }

                    if (entityName == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));

                        collection.Entities.Add(incident);
                    }

                    if (entityName == "smp_providermatrix")
                    {
                        Entity annotation = new Entity("smp_providermatrix");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        annotation.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        annotation.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                        annotation.Attributes["smp_fieldserviceteam"] = new EntityReference("team", new Guid("884A078B-0469-E711-80E5-3863BB3C0560")) { Name = "Team1" };
                        annotation.Attributes["smp_billable"] = true;
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_building")
                    {
                        Entity building = new Entity("smp_building");
                        building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                        building.Attributes["smp_city"] = "city";
                        building.Attributes["smp_country"] = "Country";
                        building.Attributes["msdyn_address1"] = "Address1";
                        building.Attributes["smp_addressline2"] = "Address2";
                        building.Attributes["smp_state"] = "State";
                        building.Attributes["smp_zipcode"] = "ZipCode";

                        collection.Entities.Add(building);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity incident = new Entity("smp_servicerequestproblemtypedesc");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                        incident.Attributes["name"] = "Service Request Problem Type Description";
                        incident.Attributes["smp_answer"] = "answer";
                        incident.Attributes["smp_servicerequestid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }

                    return collection;
                };
                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    resultEntityName = entity;
                    if (entity == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problembuilding"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        incident.Attributes["new_problemroomnumber"] = new EntityReference("smp_room", new Guid("884A078B-0467-E711-80F5-3863BB3C0560")) { Name = "room" };
                        incident.Attributes["caseorigincode"] = new OptionSetValue(915240000);
                        incident.Attributes["smp_duedate"] = new DateTime(2018, 1, 8);
                        incident.Attributes["smp_duedatebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_occureddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_submitteddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_createddatetimebybuildingtimezone"] = "2018-01-08";
                        incident.Attributes["smp_priorityid"] = new EntityReference("smp_priority", new Guid("884A078B-0466-E711-80F5-3863BB3C0560")) { Name = "priority" };
                        incident.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        incident.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };

                        incident.Attributes["smp_problemroomtype"] = new EntityReference("smp_roomtype", new Guid("884A078B-0469-E111-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_cc"] = new EntityReference("contact", new Guid("884A078B-0469-E211-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_contact"] = new EntityReference("contact", new Guid("884A078B-0469-E311-80F5-3863BB3C0560")) { Name = "problemType" };
                        incident.Attributes["smp_requestorid"] = new EntityReference("contact", new Guid("884A078B-0469-E411-80F5-3863BB3C0560")) { Name = "Contact" };

                        return incident;
                    }
                    else if (entity == "smp_building")
                    {
                        Entity building = new Entity("smp_building");
                        building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                        building.Attributes["smp_city"] = "city";
                        building.Attributes["smp_country"] = "Country";
                        building.Attributes["msdyn_address1"] = "Address1";
                        building.Attributes["smp_addressline2"] = "Address2";
                        building.Attributes["smp_state"] = "State";
                        building.Attributes["smp_zipcode"] = "ZipCode";
                        return building;
                    }
                    else if (entity == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        return incident;
                    }

                    return null;
                };

                workOrderPreCreate.Execute(serviceProvider);
                Assert.AreEqual("account", resultEntityName);
            }
        }
    }

}