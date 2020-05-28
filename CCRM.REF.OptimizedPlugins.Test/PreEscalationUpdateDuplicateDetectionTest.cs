// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreEscalationUpdateDuplicateDetectionTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PreEscalationUpdateDuplicateDetectionTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Fakes;

    [TestClass]
    public class PreEscalationUpdateDuplicateDetectionTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void EscalationDuplicate()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "smp_escalation";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            Microsoft.Xrm.Sdk.ParameterCollection paramCollection = new Microsoft.Xrm.Sdk.ParameterCollection();
            Entity escalation = new Entity("smp_escalation");
            escalation.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            escalation["smp_name"] = "test escalation";
            paramCollection.Add("Target", escalation);
            pluginContext.InputParametersGet = () => paramCollection;
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Update", null);
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

                if (entityName == "smp_escalation")
                {
                    Entity escaltionCollection = new Entity("smp_escalation");
                    escaltionCollection.Id = new Guid("884A078B-0466-E711-80F5-3863BB3C0560");
                    escaltionCollection["smp_name"] = "test";
                    collection.Entities.Add(escaltionCollection);
                }

                return collection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate(string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "incident")
                {
                    Entity building = new Entity(entity);
                    building.Id = Guid.NewGuid();
                    building["smp_issurveyservicerequest"] = true;
                    building["smp_cancelledforreclassification"] = true;
                    return building;
                }

                return null;
            };

            PreEscalationUpdateDuplicateDetection escaltionDetect = new PreEscalationUpdateDuplicateDetection();
            escaltionDetect.Execute(serviceProvider);
        }
    }
}
