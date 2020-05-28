//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="WorkOrderReclassificationTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  WorkOrderReclassificationTest Plug in Test
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
    public class WorkOrderReclassificationTest
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
        public void WorkOrderReclassificationTestMethod()
        {
            using (ShimsContext.Create())
            {
                WorkOrderReclassification workOrderReclassification = new StubWorkOrderReclassification();
                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));

                workOrder.Attributes["msdyn_workordertype"] = new EntityReference("msdyn_workordertype", new Guid("884A078B-0467-E731-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E721-80F5-3863BB3C0660"));

                paramCollection.Add("Target", workOrder);

                pluginContext.InputParametersGet = () => paramCollection;

                Entity incidentImage = new Entity("msdyn_workorder");
                incidentImage.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                incidentImage.Attributes["msdyn_workordertype"] = new EntityReference("msdyn_workordertype", new Guid("884A078B-0467-E731-80F5-3863BB3C0660")) { Name = "workordertype" };
                incidentImage.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E721-80F5-3863BB3C0660")) { Name = "IncidentType" };
                incidentImage.Attributes["smp_reclassificationreason"] = new OptionSetValue(180620002);

                var postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incidentImage)) };
                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage);

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

                    if (entityName == "smp_providermatrix")
                    {
                        Entity annotation = new Entity("smp_providermatrix");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        annotation.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        annotation.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                        annotation.Attributes["smp_fieldserviceteam"] = new EntityReference("team", new Guid("884A078B-0469-E711-80E5-3863BB3C0560")) { Name = "Team1" };
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_problemclass")
                    {
                        Entity annotation = new Entity("smp_problemclass");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_problemclassname"] = "workordertype";                        
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_problemtype")
                    {
                        Entity annotation = new Entity("smp_problemtype");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_problemtypename"] = "IncidentType";
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        collection.Entities.Add(incident);
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

                    if (entityName == "msdyn_priority")
                    {
                        Entity annotation = new Entity("msdyn_priority");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["msdyn_name"] = "Priority";
                        collection.Entities.Add(annotation);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
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
                        return incident;
                    }

                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        workorder.Attributes["smp_reclassificationreason"] = new OptionSetValue(180620002);                        
                        workorder.Attributes["smp_comment"] = "2018-01-08";                        
                        return workorder;
                    }

                    if (entity == "smp_priority")
                    {
                        Entity annotation = new Entity("smp_priority");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_name"] = "Priority";

                        return annotation;
                    }

                    return null;
                };

                workOrderReclassification.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void WorkOrderReclassificationTestMethod2()
        {
            using (ShimsContext.Create())
            {
                WorkOrderReclassification workOrderReclassification = new StubWorkOrderReclassification();
                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_workorder";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity workOrder = new Entity("msdyn_workorder");
                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));

                workOrder.Attributes["msdyn_workordertype"] = new EntityReference("msdyn_workordertype", new Guid("884A078B-0467-E731-80F5-3863BB3C0660"));
                workOrder.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E721-80F5-3863BB3C0660"));

                paramCollection.Add("Target", workOrder);

                pluginContext.InputParametersGet = () => paramCollection;

                Entity incidentImage = new Entity("msdyn_workorder");
                incidentImage.Attributes["msdyn_servicerequest"] = new EntityReference("incident", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                incidentImage.Attributes["msdyn_workordertype"] = new EntityReference("msdyn_workordertype", new Guid("884A078B-0467-E731-80F5-3863BB3C0660")) { Name = "workordertype" };
                incidentImage.Attributes["msdyn_primaryincidenttype"] = new EntityReference("msdyn_primaryincidenttype", new Guid("884A078B-0467-E721-80F5-3863BB3C0660")) { Name = "IncidentType" };
                incidentImage.Attributes["smp_reclassificationreason"] = new OptionSetValue(180620001);

                var postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incidentImage)) };
                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage);

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

                    if (entityName == "smp_providermatrix")
                    {
                        Entity annotation = new Entity("smp_providermatrix");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_buildingid"] = new EntityReference("smp_building", new Guid("884A078B-0467-E711-80F5-3863BB3C1560")) { Name = "building" };
                        annotation.Attributes["smp_problemclassid"] = new EntityReference("smp_problemclass", new Guid("884A078B-0468-E711-80F5-3863BB3C0560")) { Name = "problemClass" };
                        annotation.Attributes["smp_problemtypeid"] = new EntityReference("smp_problemtype", new Guid("884A078B-0469-E711-80F5-3863BB3C0560")) { Name = "problemType" };
                        annotation.Attributes["smp_fieldserviceteam"] = new EntityReference("team", new Guid("884A078B-0469-E711-80E5-3863BB3C0560")) { Name = "Team1" };
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_problemclass")
                    {
                        Entity annotation = new Entity("smp_problemclass");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_problemclassname"] = "workordertype";
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "smp_problemtype")
                    {
                        Entity annotation = new Entity("smp_problemtype");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_problemtypename"] = "IncidentType";
                        collection.Entities.Add(annotation);
                    }

                    if (entityName == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["name"] = "building";
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        collection.Entities.Add(incident);
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

                    if (entityName == "msdyn_priority")
                    {
                        Entity annotation = new Entity("msdyn_priority");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["msdyn_name"] = "Priority";
                        collection.Entities.Add(annotation);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
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
                        return incident;
                    }

                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        workorder.Attributes["smp_reclassificationreason"] = new OptionSetValue(180620002);
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    if (entity == "smp_priority")
                    {
                        Entity annotation = new Entity("smp_priority");
                        annotation.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        annotation.Attributes["smp_name"] = "Priority";

                        return annotation;
                    }

                    return null;
                };

                workOrderReclassification.Execute(serviceProvider);
            }
        }
    }
}