//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="WorkOrderPostUpdateTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  WorkOrderPostUpdateTest Plug in Test
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
    public class WorkOrderPostUpdateTest
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
        public void WorkOrderPostUpdateTestMethod()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                WorkOrderPostUpdate workOrderPostUpdate = new StubWorkOrderPostUpdate();
                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660")) { Name = "building" };
                workOrder.Attributes["msdyn_timetopromised"] = new EntityReference("msdyn_timetopromised", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                //// workOrder.Attributes["smp_triggerwoemail"]=false;
                paramCollection.Add("Target", workOrder);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity incident = new Entity("smp_servicerequestproblemtypedesc");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_servicerequestproblemtypedescid"] = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                        incident.Attributes["name"] = "Service Request Problem Type Description";
                        incident.Attributes["smp_answer"] = "answer";
                        incident.Attributes["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(incident);
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
                        incident.Attributes["smp_tier1provider"] = "Provider";
                        collection.Entities.Add(incident);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_workorder",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                        };
                        workorder.Attributes["msdyn_agreement"] = new EntityReference("msdyn_agreement", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_customerasset"] = new EntityReference("msdyn_customerasset", new Guid("884A078c-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["ownerid"] = new EntityReference("team", new Guid("884A078c-0467-E711-80E5-3863BB3C0661"));
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "msdyn_incidenttype")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_incidenttype",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                        };
                        workorder.Attributes["smp_inspectiontypeid"] = new EntityReference("smp_inspectiontypeid", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes[" smp_building"] = new EntityReference(" smp_building", new Guid("884A078B-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "smp_inspectionn")
                    {
                        Entity inspection = new Entity
                        {
                            LogicalName = "smp_inspection",
                            Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560")
                        };
                        inspection.Attributes["smp_questiontext"] = "unittest1";
                        return inspection;
                    }

                    if (entity == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["smp_tier1provider"] = "Provider";
                        return incident;
                    }

                    return null;
                };

                workOrderPostUpdate.Execute(serviceProvider);
            }
        }

        [TestCategory("Standard")]
        [TestMethod]
        public void WorkOrderPostCreateTestMethodServiceRequest()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                WorkOrderPostUpdate workOrderPostUpdate = new StubWorkOrderPostUpdate();
                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                workOrder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_timetopromised"] = new EntityReference("msdyn_timetopromised", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_customerasset"] = new EntityReference("msdyn_customerasset", new Guid("884A078c-0467-E711-80E5-3863BB3C0660"));
                workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660")) { Name = "building" };
                paramCollection.Add("Target", workOrder);
                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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

                    ////if (entityName == "msdyn_customerasset")
                    ////{
                    ////    Entity question = new Entity
                    ////    {
                    ////        LogicalName = "msdyn_customerasset",
                    ////        Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0660")
                    ////    };
                    ////    //// question.Attributes["smp_questiontext"] = "unittest1";
                    ////    collection.Entities.Add(question);
                    ////}
                    if (entityName == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity incident = new Entity("smp_servicerequestproblemtypedesc");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                        incident.Attributes["name"] = "Service Request Problem Type Description";
                        incident.Attributes["smp_answer"] = "answer";
                        incident.Attributes["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        //// incident.Attributes["smp_workorder"] = new EntityReference("smp_workorder", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(incident);
                    }

                    if (entityName == "msdyn_customerasset")
                    {
                        Entity customerAsset = new Entity("msdyn_customerasset");
                        customerAsset.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        customerAsset.Attributes["msdyn_customerasset"] = new EntityReference("msdyn_customerasset", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        customerAsset.Attributes["msdyn_parentasset"] = new EntityReference("msdyn_customerasset", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        collection.Entities.Add(customerAsset);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_workorder",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                        };
                        workorder.Attributes["msdyn_agreement"] = new EntityReference("msdyn_agreement", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_customerasset"] = new EntityReference("msdyn_customerasset", new Guid("884A078c-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["msdyn_timetopromised"] = new EntityReference("msdyn_timetopromised", new Guid("884A078c-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["ownerid"] = new EntityReference("team", new Guid("884A078c-0467-E711-80E5-3863BB3C0661"));
                        workorder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660")) { Name = "building" };
                        workorder.Attributes["smp_triggerwoemail"] = false;
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "msdyn_incidenttype")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_incidenttype",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                        };
                        workorder.Attributes["smp_inspectiontypeid"] = new EntityReference("smp_inspectiontypeid", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes[" smp_building"] = new EntityReference(" smp_building", new Guid("884A078B-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "smp_inspectionn")
                    {
                        Entity inspection = new Entity
                        {
                            LogicalName = "smp_inspection",
                            Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560")
                        };
                        inspection.Attributes["smp_questiontext"] = "unittest1";
                        return inspection;
                    }

                    if (entity == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["smp_tier1provider"] = "Provider";
                        return incident;
                    }

                    return null;
                };

                workOrderPostUpdate.Execute(serviceProvider);
            }
        }

        [TestCategory("Standard")]
        [TestMethod]
        public void WorkOrderPostCreateTestMethodServiceRequestQuestion()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                WorkOrderPostUpdate workOrderPostUpdate = new StubWorkOrderPostUpdate();
                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                workOrder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660")) { Name = "building" };
                workOrder.Attributes["msdyn_timetopromised"] = new EntityReference("msdyn_timetopromised", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));

                paramCollection.Add("Target", workOrder);
                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update");

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

                    ////if (entityName == "msdyn_customerasset")
                    ////{
                    ////    Entity question = new Entity
                    ////    {
                    ////        LogicalName = "msdyn_customerasset",
                    ////        Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0660")
                    ////    };
                    ////    //// question.Attributes["smp_questiontext"] = "unittest1";
                    ////    collection.Entities.Add(question);
                    ////}

                    if (entityName == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        collection.Entities.Add(incident);
                    }

                    if (entityName == "smp_servicerequestproblemtypedesc")
                    {
                        Entity incident = new Entity("smp_servicerequestproblemtypedesc");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));

                        if (incident.Contains("smp_servicerequestid"))
                        {
                            incident.Attributes["smp_servicerequestproblemtypedescid"] = new EntityReference("incident", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                            incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                            incident.Attributes["name"] = "Service Request Problem Type Description";
                            incident.Attributes["smp_answer"] = "answer";
                            incident.Attributes["smp_servicerequestid"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));

                            collection.Entities.Add(incident);
                        }

                        if (incident.Contains("smp_workorder"))
                        {
                            incident.Attributes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", new Guid("884A078B-0468-E711-82F5-3863BB3C0660")) { Name = "ProblemTypeDescription" };
                            incident.Attributes["name"] = "Service Request Problem Type Description";
                            incident.Attributes["smp_answer"] = "answer";
                            ////incident.Attributes["smp_workorder"] = new EntityReference("smp_workorder", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                            incident.Attributes["smp_servicerequestid"] = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                            collection.Entities.Add(incident);
                        }
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_workorder",
                            ////Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                        };
                        workorder.Attributes["msdyn_agreement"] = new EntityReference("msdyn_agreement", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["msdyn_customerasset"] = new EntityReference("msdyn_customerasset", new Guid("884A078c-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["msdyn_timetopromised"] = new EntityReference("msdyn_timetopromised", new Guid("884A078c-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["ownerid"] = new EntityReference("team", new Guid("884A078c-0467-E711-80E5-3863BB3C0661"));
                        workorder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes["smp_triggerwoemail"] = false;
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "msdyn_incidenttype")
                    {
                        Entity workorder = new Entity
                        {
                            LogicalName = "msdyn_incidenttype",
                            Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660")
                        };
                        workorder.Attributes["smp_inspectiontypeid"] = new EntityReference("smp_inspectiontypeid", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        workorder.Attributes[" smp_building"] = new EntityReference(" smp_building", new Guid("884A078B-0467-E711-80E5-3863BB3C0660"));
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "smp_inspectionn")
                    {
                        Entity inspection = new Entity
                        {
                            LogicalName = "smp_inspection",
                            Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560")
                        };
                        inspection.Attributes["smp_questiontext"] = "unittest1";
                        return inspection;
                    }

                    if (entity == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        incident.Attributes["smp_tier1provider"] = "Provider";
                        return incident;
                    }

                    return null;
                };

                workOrderPostUpdate.Execute(serviceProvider);
            }
        }
    }
}
