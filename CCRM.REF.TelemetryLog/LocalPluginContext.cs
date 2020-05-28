// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalPluginContext.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  LocalPluginContext class to get all the required services from IServiceProvider
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using Microsoft.Xrm.Sdk;
    using System;

    public class LocalPluginContext
    {
        public LocalPluginContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            //// Obtain the execution context service from the service provider.
            this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //// Obtain the tracing service from the service provider.
            this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //// Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            //// Use the factory to generate the Organization Service.
            this.OrganizationService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);

            ////Use the factory to generate a new Organization Service for System User calls.
            this.SystemOrganizationService = factory.CreateOrganizationService(null);
        }

        private LocalPluginContext()
        {
        }

        public IOrganizationService OrganizationService
        {
            get;
            set;
        }

        public IOrganizationService SystemOrganizationService
        {
            get;
            set;
        }

        public IPluginExecutionContext PluginExecutionContext
        {
            get;
            set;
        }

        public ITracingService TracingService
        {
            get;
            set;
        }
    }
}
