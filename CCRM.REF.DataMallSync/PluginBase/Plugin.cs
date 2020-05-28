// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Plugin Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.DataMallSync
{
    using CCRM.REF.DataMallSync.Common;
    using CCRM.REF.DataMallSync.Contracts;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class Plugin : IPlugin
    {
        private Collection<Tuple<int, string, string, Action<LocalPluginContext>>> registeredEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="childClassName">The <see cref=" cred="Type"/> of the derived class.</param>
        public Plugin(Type childClassName)
        {
            this.ChildClassName = childClassName.ToString();
        }

        public Plugin()
        {
            this.ChildClassName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        public string ChildClassName
        {
            get;
            set;
        }

        #region ErrorHandler

        public ErrorHandlerUtility PluginErrorHandlerUtility { get; set; }

        #endregion

        public LocalPluginContext LocalContext { get; set; }

        protected Collection<Tuple<int, string, string, Action<LocalPluginContext>>> RegisteredEvents
        {
            get
            {
                if (this.registeredEvents == null)
                {
                    this.registeredEvents = new Collection<Tuple<int, string, string, Action<LocalPluginContext>>>();
                }

                return this.registeredEvents;
            }
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                //// Construct the Local plug-in context.
                this.LocalContext = new LocalPluginContext(serviceProvider);

                //// Initialize ErrorHandler and ErrorHandlerUtility
                //// If the requirements call for it, initialize your ErrorLogger here
                this.PluginErrorHandlerUtility = new ErrorHandlerUtility(this.LocalContext);

                Utility pluginUtility = new Utility(this.LocalContext);
                pluginUtility.PluginErrorHandlerUtility = this.PluginErrorHandlerUtility;

                //// Iterate over all of the expected registered events to ensure that the plugin
                //// has been invoked by an expected event
                //// For any given plug-in event at an instance in time, we would expect at most 1 result to match.
                Action<LocalPluginContext> entityAction =
                    (from a in this.RegisteredEvents
                     where (
                     a.Item1 == this.LocalContext.PluginExecutionContext.Stage &&
                     a.Item2 == this.LocalContext.PluginExecutionContext.MessageName &&
                     (string.IsNullOrWhiteSpace(a.Item3) ? true : a.Item3 == this.LocalContext.PluginExecutionContext.PrimaryEntityName))
                     select a.Item4).FirstOrDefault();

                if (entityAction != null)
                {
                    entityAction.Invoke(this.LocalContext);
                    return;
                }
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                this.PluginErrorHandlerUtility.ExecuteErrorHandlers<InvalidPluginExecutionException>(invalidPluginExecutionException);
            }
            catch (Exception exception)
            {
                this.PluginErrorHandlerUtility.ExecuteErrorHandlers<Exception>(exception);
            }
        }

        /// <summary>
        /// Gets the List of events that the plug-in should fire for. Each List
        /// Item is a <see cref="System.Tuple"/> containing the Pipeline Stage, Message and (optionally) the Primary Entity. 
        /// In addition, the fourth parameter provide the delegate to invoke on a matching registration.
        /// </summary>       
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

                ////Create an instance of IocContainer for Dependency Injection
                this.DependencyContainer = new IocContainer();

                if (serviceProvider.GetService(typeof(ISystemNetWebClient)) != null)
                {
                    this.DependencyContainer.RegisterSingle<ISystemNetWebClient>(() => (ISystemNetWebClient)serviceProvider.GetService(typeof(ISystemNetWebClient)));
                }
                else
                {
                    this.DependencyContainer.RegisterSingle<ISystemNetWebClient>(() => new SystemNetWebClient());
                }
            }

            private LocalPluginContext()
            {
            }

            public IServiceProvider ServiceProvider
            {
                get;
                set;
            }

            public IocContainer DependencyContainer
            {
                get;
                set;
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

            public void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                this.TracingService.Trace(message);
            }
        }
    }
}
