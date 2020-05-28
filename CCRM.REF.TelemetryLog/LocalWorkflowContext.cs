// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalWorkflowContext.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  LocalWorkflowContext class to get all the required services from CodeActivityContext
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Activities;

    public class LocalWorkflowContext
    {
        public LocalWorkflowContext(CodeActivityContext codeActivityContext)
        {
            if (codeActivityContext == null)
            {
                throw new ArgumentNullException("codeActivityContext");
            }

            //// Obtain the execution context service from the service provider.
            this.WorkFlowContext = codeActivityContext.GetExtension<IWorkflowContext>();

            //// Obtain the tracing service from the service provider.
            this.TracingService = codeActivityContext.GetExtension<ITracingService>();

            //// Obtain the Organization Service factory service from the service provider
            IOrganizationServiceFactory factory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();

            //// Use the factory to generate the Organization Service.
            this.OrganizationService = factory.CreateOrganizationService(this.WorkFlowContext.UserId);

            ////Use the factory to generate a new Organization Service for System User calls.
            this.SystemOrganizationService = factory.CreateOrganizationService(null);
        }

        private LocalWorkflowContext()
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

        public IWorkflowContext WorkFlowContext
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
