// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginBase.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   Plugin Base
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Test
{
    using Contracts;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using System;
    using System.Collections.Generic;
    using System.Fakes;

    public class PluginBase
    {
        public PluginBase()
        {
            this.SetupServices();
        }

        public IServiceProvider ServiceProviderFake { get; set; }

        public EntityCollection EntityCol { get; set; }

        public IOrganizationService ServiceFake { get; set; }

        public IPluginExecutionContext PluginContextFake { get; set; }

        public string MessageName { get; set; }

        public int Stage { get; set; }

        public string PrimaryEntityName { get; set; }

        public Guid PrimaryEntityId { get; set; }

        public void SetupServices()
        {
            ////Entity entity = new Entity();
            ////entity.Attributes["ava_name"] = "CMSHostURL";
            ////entity.Attributes["ava_value"] = "CMSHostURLValue";
            ////EntityCollection entityCol = new EntityCollection();
            ////entityCol.Entities.Add(entity);

            this.ServiceFake = new StubIOrganizationService();
            ////{
            ////    RetrieveMultipleQueryBase = (queryBase) => entityCol,
            ////    ExecuteOrganizationRequest = (request) => null
            ////};

            IOrganizationServiceFactory factoryFake = new StubIOrganizationServiceFactory()
            {
                CreateOrganizationServiceNullableOfGuid = (userId) => this.ServiceFake
            };

            ITracingService tracingServiceFake = new StubITracingService()
            {
                TraceStringObjectArray = (format, args) => { return; }
            };

            this.ServiceProviderFake = new StubIServiceProvider()
            {
                GetServiceType = (type) =>
                {
                    if (type == typeof(IOrganizationService))
                    {
                        return ServiceFake;
                    }

                    if (type == typeof(IPluginExecutionContext))
                    {
                        return PluginContextFake;
                    }

                    if (type == typeof(ITracingService))
                    {
                        return tracingServiceFake;
                    }

                    if (type == typeof(IOrganizationServiceFactory))
                    {
                        return factoryFake;
                    }

                    if (type == typeof(ISystemNetWebClient))
                    {
                        return null;
                    }

                    return null;
                }
            };
        }
    }
}
