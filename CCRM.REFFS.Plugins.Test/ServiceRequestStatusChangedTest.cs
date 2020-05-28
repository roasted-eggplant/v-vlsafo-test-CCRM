//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="ServiceRequestStatusChangedTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  ServiceRequestStatusChangedTest Plug in Test
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
    public class ServiceRequestStatusChangedTest
    {
        public static void PluginVariables(StubIServiceProvider serviceProvider, StubIPluginExecutionContext pluginContext, StubIOrganizationService organizationService, int stageNumber, string messageName)
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
        public void ServiceRequestStatusChangedToCancelledTestMethod()
        {
            using (ShimsContext.Create())
            {
                ServiceRequestStatusChanged serviceRequestStatus = new StubServiceRequestStatusChanged();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();
                var tracingService = new StubITracingService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["statuscode"] = new OptionSetValue(6);
                targetIncident.Attributes["smp_cancellingfromwo"] = false;
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
                        workorder.Attributes["msdyn_servicerequest"] = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                        workorder.Attributes["msdyn_systemstatus"] = new OptionSetValue(690970001);
                        collection.Entities.Add(workorder);
                    }

                    if (entityName == "bookableresourcebooking")
                    {
                        Entity bookings = new Entity("bookableresourcebooking");
                        bookings.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        bookings.Attributes["msdyn_workorder"] = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                        bookings.Attributes["statecode"] = new OptionSetValue(1);
                        collection.Entities.Add(bookings);
                    }

                    if (entityName == "bookingstatus")
                    {
                        Entity bookingStatus = new Entity("bookingstatus");
                        bookingStatus.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        ////bookingStatus.Attributes["msdyn_workorder"] = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                        bookingStatus.Attributes["status"] = new OptionSetValue(3);
                        collection.Entities.Add(bookingStatus);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
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
                        incident.Attributes["smp_cancellingfromwo"] = false;

                        return incident;
                    }

                    return null;
                };

                serviceRequestStatus.Execute(serviceProvider);
            }
        }
    }
}