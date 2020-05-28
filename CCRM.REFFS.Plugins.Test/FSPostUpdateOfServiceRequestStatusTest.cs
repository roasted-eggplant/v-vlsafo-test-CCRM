//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="FSPostUpdateOfServiceRequestStatusTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  FSPostUpdateOfServiceRequestStatusTest Plug in Test
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
    public class FSPostUpdateOfServiceRequestStatusTest
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
        public void FSPostUpdateOfServiceRequestStatusTestMethod()
        {
            using (ShimsContext.Create())
            {
                FSPostUpdateOfServiceRequestStatus fspostUpdateOfServiceRequestStatus = new StubFSPostUpdateOfServiceRequestStatus();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["statuscode"] = new OptionSetValue(180620002);
                paramCollection.Add("Target", targetIncident);

                pluginContext.InputParametersGet = () => paramCollection;

                Entity incidentImage = new Entity("incident");
                incidentImage.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                incidentImage.Attributes["statuscode"] = new OptionSetValue(180620002);
                incidentImage.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                incidentImage.Attributes["smp_problembuilding"] = new EntityReference("building", new Guid("884A078B-0458-E711-80F5-3863BB3C0660"));
                incidentImage.Attributes["smp_workorderwithind365"] = true;
                incidentImage.Attributes["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid("884A078B-0468-E721-80F5-3863BB3C0660"));
                var postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incidentImage)) };
                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage);

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
                        workorder.Attributes["msdyn_servicerequest"] = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                        collection.Entities.Add(workorder);
                    }

                    if (entityName == "pricelevel")
                    {
                        Entity pricelevel = new Entity("pricelevel");
                        pricelevel.Id = new Guid("884A078B-0468-E712-80F5-3863BB3C0560");
                        pricelevel.Attributes["name"] = "US";
                        collection.Entities.Add(pricelevel);
                    }

                    if (entityName == "account")
                    {
                        Entity pricelevel = new Entity("account");
                        pricelevel.Id = new Guid("884A078B-0468-E712-80F8-3863BB3C0560");
                        pricelevel.Attributes["name"] = "BuildingName";
                        collection.Entities.Add(pricelevel);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        return incident;
                    }

                    if (entity == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0468-E721-80F5-3863BB3C0660");
                        incident.Attributes["smp_reclassifiedfromworkorder"] = false;
                        return incident;
                    }

                    return null;
                };

                fspostUpdateOfServiceRequestStatus.Execute(serviceProvider);
            }
        }

        [TestMethod]
        public void FSPostUpdateOfServiceRequestStatusTestMethod2()
        {
            using (ShimsContext.Create())
            {
                FSPostUpdateOfServiceRequestStatus fspostUpdateOfServiceRequestStatus = new StubFSPostUpdateOfServiceRequestStatus();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "incident";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity targetIncident = new Entity("incident");
                targetIncident.Id = new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                targetIncident.Attributes["statuscode"] = new OptionSetValue(180620002);
                paramCollection.Add("Target", targetIncident);

                pluginContext.InputParametersGet = () => paramCollection;

                Entity incidentImage = new Entity("incident");
                incidentImage.Attributes["statuscode"] = new OptionSetValue(180620002);
                incidentImage.Attributes["customerid"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                incidentImage.Attributes["smp_workorderwithind365"] = true;
                incidentImage.Attributes["smp_problembuilding"] = new EntityReference("building", new Guid("884A078B-0458-E711-80F5-3863BB3C0660"));
                var postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incidentImage)) };

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", postImage);

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

                    if (entityName == "pricelevel")
                    {
                        Entity pricelevel = new Entity("pricelevel");
                        pricelevel.Id = new Guid("884A078B-0468-E712-80F5-3863BB3C0560");
                        pricelevel.Attributes["name"] = "US";
                        collection.Entities.Add(pricelevel);
                    }

                    if (entityName == "account")
                    {
                        Entity pricelevel = new Entity("account");
                        pricelevel.Id = new Guid("884A078B-0468-E712-80F8-3863BB3C0560");
                        pricelevel.Attributes["name"] = "BuildingName";
                        pricelevel.Attributes["smp_feedstoreid"] = new Guid("884A078B-0468-E712-80F8-3863BB3C0560");
                        collection.Entities.Add(pricelevel);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "account")
                    {
                        Entity incident = new Entity("account");
                        incident.Id = new Guid("884A078B-0468-E711-80F5-3863BB3C0660");
                        incident.Attributes["smp_workorderwithind365"] = true;
                        incident.Attributes["msdyn_servicerequest"] = new EntityReference("account", new Guid("884A078B-0468-E711-80F5-3863BB3C0660"));
                        incident.Attributes["msdyn_pricelist"] = new EntityReference("pricelevel", new Guid("884A078B-0468-E712-80F5-3863BB3C0560"));
                        return incident;
                    }

                    if (entity == "incident")
                    {
                        Entity incident = new Entity("incident");
                        incident.Id = new Guid("884A078B-0468-E721-80F5-3863BB3C0660");
                        incident.Attributes["smp_reclassifiedfromworkorder"] = false;
                        incident.Attributes["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid("884A078B-0468-E721-80F5-3863BB3C0660"));

                        return incident;
                    }

                    if (entity == "building")
                    {
                        Entity building = new Entity("building");
                        building.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C1560");
                        building.Attributes["smp_city"] = "city";
                        building.Attributes["smp_country"] = "Country";
                        building.Attributes["msdyn_address1"] = "Address1";
                        building.Attributes["smp_addressline2"] = "Address2";
                        building.Attributes["smp_state"] = "State";
                        building.Attributes["smp_zipcode"] = "ZipCode";

                        return building;
                    }

                    if (entity == "pricelevel")
                    {
                        Entity pricelist = new Entity("pricelevel");
                        pricelist.Id = new Guid("984A078B-0467-E711-80F5-3863BB3C0660");
                        pricelist.Attributes["name"] = "US";
                        pricelist.Attributes["transactioncurrencyid"] = new EntityReference("transactioncurrency", new Guid("884A078B-0467-E711-80F5-3863BB3C1560"));
                        return pricelist;
                    }

                    return null;
                };

                fspostUpdateOfServiceRequestStatus.Execute(serviceProvider);
            }
        }
    }
}
