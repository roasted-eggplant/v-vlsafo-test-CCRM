//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="SetDueDateonWOTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////   public classSetDueDateonWOTest Plug in Test
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
    public class SetDueDateonWOTest
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
        public void SetDueDateonWOTestMethod()
        {
            using (ShimsContext.Create())
            {
                SetDueDateonWO setDueDateon = new StubSetDueDateonWO();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_agreementbookingdate";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E512-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity agreementBooking = new Entity("msdyn_agreementbookingdate");
                agreementBooking.Id = new Guid("54D94FC2-52AD-E512-8148-1458D04DB4D1");
                agreementBooking.Attributes["msdyn_workorder"] = new EntityReference("msdyn_workorder", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                paramCollection.Add("Target", agreementBooking);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", null);

                organizationService.RetrieveMultipleQueryBase = (query) =>
                {
                    EntityCollection collection = new EntityCollection();
                    string entityName = string.Empty;
                    if (query.GetType().Name.Equals("FetchExpression"))
                    {
                        if (((FetchExpression)query).Query.Contains("<entity name='msdyn_workorder'>"))
                        {
                            entityName = "msdyn_workorder";
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
                        Entity workoOrder = new Entity("msdyn_workorder");
                        workoOrder.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                        collection.Entities.Add(workoOrder);
                    }

                    return collection;
                };

                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)

                {
                    if (entity == "msdyn_agreementbookingdate")
                    {
                        Entity agreementBooking1 = new Entity("msdyn_agreementbookingdate");
                        agreementBooking1.Id = new Guid("54D94FC2-52AD-E512-8148-1458D04DB4D1");
                        agreementBooking1.Attributes["msdyn_bookingdate"] = DateTime.Now;
                        agreementBooking1.Attributes["msdyn_workorder"] = new EntityReference("msdyn_workorder", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        return agreementBooking1;
                    }

                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        workorder.Attributes["smp_reclassificationreason"] = new OptionSetValue(180620002);
                        workorder.Attributes["smp_comment"] = "2018-01-08";
                        return workorder;
                    }

                    return null;
                };

                setDueDateon.Execute(serviceProvider);
            }
        }
    }
}
