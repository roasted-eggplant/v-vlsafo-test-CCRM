// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateSRCreateDynNotesTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// PostCreateSRCreateDynNotesTest
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk.Workflow.Fakes;
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Fakes;

    [TestClass]
    public class PostCreateSRCreateDynNotesTest
    {
        private StubIOrganizationService Service { get; set; }

        private StubIOrganizationServiceFactory Factory { get; set; }

        private StubIWorkflowContext IWorKFlowContext { get; set; }

        private StubITracingService TracingService { get; set; }

        [TestMethod]
        public void CreateDynNotes()
        {
            var workflowUserId = Guid.NewGuid();
            var workflowCorrelationId = Guid.NewGuid();
            var workflowInitiatingUserId = Guid.NewGuid();

            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();

            this.Service = new StubIOrganizationService();
            this.IWorKFlowContext = new StubIWorkflowContext();
            this.Factory = new StubIOrganizationServiceFactory();
            this.TracingService = new StubITracingService();

            this.IWorKFlowContext.UserIdGet = () =>
            {
                return workflowUserId;
            };

            this.Factory.CreateOrganizationServiceNullableOfGuid = id =>
            {
                return Service;
            };

            PostCreateSRCreateDynNotes updateDeviceRequestEmployeeDetails = new PostCreateSRCreateDynNotes();
            var invoker = new WorkflowInvoker(updateDeviceRequestEmployeeDetails);
            invoker.Extensions.Add<ITracingService>(() => this.TracingService);
            invoker.Extensions.Add<IWorkflowContext>(() => this.IWorKFlowContext);
            invoker.Extensions.Add<IOrganizationServiceFactory>(() => this.Factory);
            pluginContext.PrimaryEntityNameGet = () => "incident";
            pluginContext.PrimaryEntityIdGet = () => new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            ParameterCollection paramCollection = new ParameterCollection();
            ParameterCollection paramCollectionPostImage = new ParameterCollection();
            Entity incident = new Entity("incident");
            incident.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
            paramCollection.Add("Target", incident);
            pluginContext.InputParametersGet = () => paramCollection;
            EntityImageCollection postImage = new EntityImageCollection { (new KeyValuePair<string, Entity>("PostImage", incident)) };
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, this.Service, 40, "Create", postImage);

            this.Service.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;

                if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_problemtypedescription")
                {
                    Entity configuration = new Entity("smp_configuration");
                    configuration.Id = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                    configuration["smp_problemtypedescriptionid"] = new Guid("884A078B-0466-E712-80F5-3863BB3C0560");
                    configuration["smp_problemtypedescriptionname"] = "Sample Name";

                    collection.Entities.Add(configuration);
                }

                return collection;
            };

            this.Service.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "incident")
                {
                    Entity incidents = new Entity(entity);
                    incidents["caseorigincode"] = new OptionSetValue(1);
                    incidents["smp_createdfrom"] = new OptionSetValue(1);
                    incidents["smp_problembuilding"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemclassid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents["smp_problemtypeid"] = new EntityReference(string.Empty, Guid.NewGuid());
                    incidents.Attributes["statecode"] = new OptionSetValue(1);
                    incidents.Id = new Guid("884A078B-0467-E711-80F5-3863BB3C0660");
                    return incidents;
                }

                return null;
            };

            IDictionary<string, object> inputs = new Dictionary<string, object>();
            EntityReference newIncident = new EntityReference(incident.LogicalName, incident.Id);
            inputs["ServiceRequest"] = newIncident;
            var output = invoker.Invoke(inputs);
        }
    }
}
