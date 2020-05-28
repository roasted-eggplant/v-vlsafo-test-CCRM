//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="AssociateProcedureToWOTest.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  AssociateProcedureToWOTest Plug in Test
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
    public class AssociateProcedureToWOTest
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
        public void AssociateKBArticlesToWOTestMethod()
        {
            using (ShimsContext.Create())
            {
                AssociateProceduresToWO associateProceedureToWO = new StubAssociateProceduresToWO();

                var serviceProvider = new StubIServiceProvider();
                var pluginContext = new StubIPluginExecutionContext();
                var organizationService = new StubIOrganizationService();

                pluginContext.PrimaryEntityNameGet = () => "msdyn_agreementbookingdate";
                pluginContext.PrimaryEntityIdGet = () => new Guid("54D94FC2-52AD-E511-8158-1458D04DB4D1");
                ParameterCollection paramCollection = new ParameterCollection();
                Entity bookingDate = new Entity("msdyn_agreementbookingdate");
                bookingDate.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                bookingDate.Attributes["msdyn_workorder"] = new EntityReference("msdyn_workorder", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                paramCollection.Add("Target", bookingDate);

                pluginContext.InputParametersGet = () => paramCollection;

                PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Update", null);

                organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
                {
                    if (entity == "msdyn_agreementbookingdate")
                    {
                        Entity bookingdate = new Entity("msdyn_workorder");
                        bookingdate.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        bookingdate.Attributes["smp_reclassificationreason"] = new OptionSetValue(180620002);
                        bookingdate.Attributes["msdyn_workorder"] = new EntityReference("msdyn_workorder", new Guid("884A078B-0467-E711-80F5-3863BB3C0660"));
                        bookingdate.Attributes["msdyn_bookingsetup"] = new EntityReference("msdyn_bookingsetup", new Guid("884A078B-0467-E711-80F5-3863BB3C0661"));
                        bookingdate.Attributes["smp_comment"] = "2018-01-08";
                        return bookingdate;
                    }

                    if (entity == "msdyn_workorder")
                    {
                        Entity workorder = new Entity("msdyn_workorder");
                        workorder.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                        return workorder;
                    }

                    if (entity == "msdyn_bookingsetup")
                    {
                        Entity bookingsetup = new Entity("msdyn_bookingsetup");
                        bookingsetup.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0661");
                        bookingsetup.Attributes["smp_procedureid"] = new EntityReference("smp_knowledgearticle", new Guid("884A078B-0467-E711-80F5-3863BB3C0661"));
                        return bookingsetup;
                    }

                    return null;
                };

                associateProceedureToWO.Execute(serviceProvider);
            }
        }
    }
}
