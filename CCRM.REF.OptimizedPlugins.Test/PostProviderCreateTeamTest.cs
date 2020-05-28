// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostProviderCreateTeamTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostProviderCreateTeamTest Test Class
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

    /// <summary>
    /// Unit Test for Post Update Service request Code Plugin
    /// </summary>
    [TestClass]
    public class PostProviderCreateTeamTest
    {
        /// <summary>
        /// Executes the update with all attributes.
        /// </summary>
        [TestMethod]
        public void CreateTeamFromAccount()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();

            pluginContext.PrimaryEntityNameGet = () => "account";
            pluginContext.PrimaryEntityIdGet = () => Guid.NewGuid();
            pluginContext.BusinessUnitIdGet = () => Guid.NewGuid();
            pluginContext.DepthGet = () => -1;
            var entity = new Entity();
            entity.LogicalName = "account";
            entity.Attributes.Add("accountid", Guid.NewGuid());
            entity.Attributes.Add("name", "Mock Account Name");
            ParameterCollection paramCollection = new ParameterCollection();
            paramCollection.Add("Target", entity);
            pluginContext.InputParametersGet = () => paramCollection;

            //// Mole the basic Plugin objects
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create", null);
            organizationService.RetrieveMultipleQueryBase = query =>
            {
                var entityCollection = new EntityCollection();
                var entity1 = new Entity();
                switch (((QueryExpression)query).EntityName)
                {
                    case "team":
                        break;
                    case "account":
                        entityCollection.Entities.Add(entity1);
                        break;
                    case "role":
                        var entityRole = new Entity();
                        entityRole.LogicalName = "role";
                        entityRole.Attributes.Add("roleid", Guid.NewGuid());
                        entityRole.Attributes.Add("name", "Provider");
                        entityCollection.Entities.Add(entityRole);
                        break;
                }

                return entityCollection;
            };

            organizationService.RetrieveStringGuidColumnSet = delegate (string _entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (_entity == "incident")
                {
                    Entity incident = new Entity(_entity);
                    incident["smp_issurveyservicerequest"] = true;
                    incident["smp_cancelledforreclassification"] = true;
                    incident.Id = Guid.NewGuid();
                    return incident;
                }

                return null;
            };

            organizationService.UpdateEntity = param => { };
            organizationService.CreateEntity = parameter =>
            {
                return new Guid();
            };
            var pluginClass = new PostProviderCreateTeam();
            pluginClass.Execute(serviceProvider);
        }
    }
}
