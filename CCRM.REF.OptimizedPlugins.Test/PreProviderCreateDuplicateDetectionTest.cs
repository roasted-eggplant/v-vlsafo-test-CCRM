// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreProviderCreateDuplicateDetectionTest.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreProviderCreateDuplicateDetectionTest Test Class
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
    public class PreProviderCreateDuplicateDetectionTest
    {
        /// <summary>
        /// Executes the update with all attributes.
        /// </summary>
        [TestMethod]
        
        public void ExecuteUpdateWithAllAttributes()
        {
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            pluginContext.PrimaryEntityNameGet = () => "account";
            pluginContext.PrimaryEntityIdGet = () => new Guid();
            pluginContext.BusinessUnitIdGet = () => new Guid();
            pluginContext.DepthGet = () => -1;
            var entity = new Entity();
            entity.LogicalName = "account";
            ////entity.Attributes.Add("accountid", new Guid());
            entity.Attributes.Add("name", "Mock Account Name");
            ParameterCollection paramCollection = new ParameterCollection();
            paramCollection.Add("Target", entity);
            pluginContext.InputParametersGet = () => paramCollection;

            //// Mole the basic Plugin objects
            Helper.Helper.PluginVariables(serviceProvider, pluginContext, organizationService, 20, "Create", null);
            organizationService.RetrieveMultipleQueryBase = query =>
            {
                var entityCollection = new EntityCollection();
                var entity1 = new Entity();
                switch (((QueryExpression)query).EntityName)
                {
                    ////case "team":
                    ////    entityCollection.Entities.Add(entity1);
                    ////    break;
                    //case "account":
                    //    entity1.Attributes.Add("name", string.Empty);
                    //    entityCollection.Entities.Add(entity1);
                    //    break;
                }

                return entityCollection;
            };
            ////organizationService.UpdateEntity = param => { };
            ////organizationService.CreateEntity = parameter =>
            ////{
            ////    return new Guid();
            ////};
            var pluginClass = new PreProviderCreateDuplicateDetection();
            pluginClass.Execute(serviceProvider);
        }
    }
}
