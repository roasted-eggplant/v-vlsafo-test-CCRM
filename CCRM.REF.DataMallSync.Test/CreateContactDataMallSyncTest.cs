
namespace CCRM.REF.DataMallSync.Test
{
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Fakes;
    using CCRM.REF.DataMallSync;

    [TestClass]
    public class ContactCreateTest
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
        public void ContactPreCreateTest()
        {
            string resultEntityName = string.Empty;

            using (ShimsContext.Create())
            {
                CCRM.REF.DataMallSync.CreateUpdateContactDataMallSync contactPreCreate = new CreateUpdateContactDataMallSync();
                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "contact";
                pluginContext.PrimaryEntityIdGet = () => new Guid("BF4A4DA6-2204-E911-A964-000D3A3406C4");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity contact = new Entity("contact");
                contact.Id = new Guid("801BD880-6A5F-E911-A981-000D3A30DC0A");
                contact.Attributes["smp_buildingnumber"] = "20";
                contact.Attributes["smp_roomno"] = "12754253";
                contact.Attributes["smp_mgrpersonalnumber"] = "1331341";
                contact.Attributes["smp_companyid"] = "Microsoft";
                paramCollection.Add("Target", contact);
                pluginContext.InputParametersGet = () => paramCollection;
                EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", contact)) };
                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");
                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((FetchExpression)query).Query.Contains("<entity name='smp_building'>"))
                        {
                            entityName = "smp_building";
                        }
                        if (((FetchExpression)query).Query.Contains("<entity name='smp_room'>"))
                        {
                            entityName = "smp_room";
                        }
                        if (((FetchExpression)query).Query.Contains("<entity name='contact'>"))
                        {
                            entityName = "contact";
                        }
                        if (((FetchExpression)query).Query.Contains("<entity name='smp_company'>"))
                        {
                            entityName = "smp_company";
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
                    if (entityName == "smp_room")
                    {
                        Entity room = new Entity(entityName);
                        room.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                        room["smp_roomid"] = new Guid("884A078B-0466-E711-80F5-3863BB3C0679");
                        room["smp_feedstoreid"] = "12754253";
                        room["smp_name"] = "1001";
                        room["status"] = 0;
                        room["smp_buildingnumber"] = new EntityReference("building", new Guid("F06E8975-7AF4-E711-8100-3863BB2E0660"));
                        room["smp_roomno"] = new EntityReference("room", new Guid("735AA200-B954-E911-A985-000D3A30DC28"));
                        collection.Entities.Add(room);
                    }
                    if (entityName == "smp_building")
                    {
                        Entity building = new Entity(entityName);
                        building.Id = new Guid("F06E8975-7AF4-E711-8100-3863BB2E0660");
                        building["smp_buildingid"] = new Guid("F06E8975-7AF4-E711-8100-3863BB2E0660");
                        building["smp_feedstoreid"] = "20";
                        building["smp_buildingnumber"] = new EntityReference("building", new Guid("735AA200-B954-E911-A985-000D3A30DC28"));
                        collection.Entities.Add(building);
                    }
                    if (entityName == "contact")
                    {
                        Entity managerpersonalnumber = new Entity(entityName);
                        managerpersonalnumber.Id = new Guid("801BD880-6A5F-E911-A981-000D3A30DC0A");
                        managerpersonalnumber["smp_personalnumber"] = "1331341";
                        collection.Entities.Add(managerpersonalnumber);
                    }
                    if (entityName == "smp_company")
                    {
                        Entity company = new Entity(entityName);
                        company.Id = new Guid("801BD880-6A5F-E911-A981-000D3A30DB0A");
                        company["smp_name"] = "Microsoft";
                        collection.Entities.Add(company);
                    }
                    return collection;
                };

                organizationService.UpdateEntity = (ent) =>
                {
                    resultEntityName = ent.LogicalName;
                };
                contactPreCreate.Execute(serviceProvider);
                Assert.AreEqual("contact", resultEntityName);
            }
        }
    }
}


