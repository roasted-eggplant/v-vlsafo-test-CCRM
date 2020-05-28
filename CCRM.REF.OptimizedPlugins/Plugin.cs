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
namespace CCRM.REF.OptimizedPlugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;

    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public class Plugin : IPlugin
    {
        private Collection<Tuple<int, string, string, Action<LocalPluginContext>>> registeredEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="childClassName">The <see cref=" cred="Type"/> of the derived class.</param>
        internal Plugin(Type childClassName)
        {
            this.ChildClassName = childClassName.ToString();
        }

        /// <summary>
        /// Gets the List of events that the plug-in should fire for. Each List
        /// Item is a <see cref="System.Tuple"/> containing the Pipeline Stage, Message and (optionally) the Primary Entity. 
        /// In addition, the fourth parameter provide the delegate to invoke on a matching registration.
        /// </summary>
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
        /// Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        protected string ChildClassName
        {
            get;

            private set;
        }

        /// <summary>
        /// Gets the incident is survey service request.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The incident id.</param>
        /// <returns>Survey Service Request</returns>
        public static bool GetIncidentIsSurveyServiceRequest(IOrganizationService service, Guid incidentId)
        {
            bool isSurveyServicerequest = false;
            try
            {
                if (service != null)
                {
                    Entity incident = service.Retrieve("incident", incidentId, new ColumnSet("smp_issurveyservicerequest"));
                    if (incident != null)
                    {
                        if (incident.Attributes.Contains("smp_issurveyservicerequest"))
                        {
                            isSurveyServicerequest = Convert.ToBoolean(incident.Attributes["smp_issurveyservicerequest"].ToString(), CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return isSurveyServicerequest;
        }

        /// <summary>
        /// Gets the incident is cancelled for reclassification.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="incidentId">The incident id.</param>
        /// <returns>Survey Service Request</returns>
        public static bool GetIncidentIsReclassificationCancellation(IOrganizationService service, Guid incidentId)
        {
            bool isCancelledForReclassification = false;
            try
            {
                if (service != null)
                {
                    Entity incident = service.Retrieve("incident", incidentId, new ColumnSet("smp_cancelledforreclassification"));
                    if (incident != null)
                    {
                        if (incident.Attributes.Contains("smp_cancelledforreclassification"))
                        {
                            isCancelledForReclassification = Convert.ToBoolean(incident.Attributes["smp_cancelledforreclassification"].ToString(), CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return isCancelledForReclassification;
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
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            //// Construct the Local plug-in context.
            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);
            localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName));
            try
            {
                ITracingService trace = localcontext.TracingService;
                if (GetIncidentIsSurveyServiceRequest(localcontext.OrganizationService, localcontext.PluginExecutionContext.PrimaryEntityId) == true)
                {
                    if (localcontext.PluginExecutionContext.Depth > 6)
                    {
                        localcontext.Trace("Depth- " + localcontext.PluginExecutionContext.Depth.ToString());
                        localcontext.Trace("name- " + localcontext.PluginExecutionContext.ToString());
                        return;
                    }
                }
                else if (GetIncidentIsReclassificationCancellation(localcontext.OrganizationService, localcontext.PluginExecutionContext.PrimaryEntityId) == true)
                {
                    if (localcontext.PluginExecutionContext.Depth > 3)
                    {
                        return;
                    }
                }
                else if (this.ChildClassName == "CCRM.REF.Plugins.PostSatisfactionSurveyResponseCreate")
                {
                    localcontext.Trace("Depth- " + localcontext.PluginExecutionContext.Depth.ToString());
                    if (localcontext.PluginExecutionContext.Depth > 4)
                    {
                        return;
                    }
                }
                else if (this.ChildClassName == "CCRM.REF.OptimizedPlugins.PostServiceRequestUpdateCreateAudit" || this.ChildClassName == "CCRM.REF.OptimizedPlugins.PreServiceRequestUpdateCompletedDate")
                {
                    localcontext.Trace("Depth- " + localcontext.PluginExecutionContext.Depth.ToString());
                    if (localcontext.PluginExecutionContext.Depth > 2)
                    {
                        return;
                    }
                }
                else
                {
                    if (localcontext.PluginExecutionContext.Depth > 1 && localcontext.PluginExecutionContext.MessageName != "Create")
                    {
                        return;
                    }
                }

                //// Iterate over all of the expected registered events to ensure that the plugin
                //// has been invoked by an expected event
                //// For any given plug-in event at an instance in time, we would expect at most 1 result to match.
                Action<LocalPluginContext> entityAction =
                    (from a in this.RegisteredEvents
                     where (
                     a.Item1 == localcontext.PluginExecutionContext.Stage &&
                     a.Item2 == localcontext.PluginExecutionContext.MessageName &&
                     (string.IsNullOrWhiteSpace(a.Item3) ? true : a.Item3 == localcontext.PluginExecutionContext.PrimaryEntityName))
                     select a.Item4).FirstOrDefault();

                if (entityAction != null)
                {
                    localcontext.Trace(string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} is firing for Entity: {1}, Message: {2}",
                        this.ChildClassName,
                        localcontext.PluginExecutionContext.PrimaryEntityName,
                        localcontext.PluginExecutionContext.MessageName));
                    entityAction.Invoke(localcontext);

                    //// now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                    //// guard against multiple executions.
                    return;
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));
                //// Handle the exception.
                throw;
            }
            finally
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName));
            }
        }

        protected class LocalPluginContext
        {
            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                //// Obtain the execution context service from the service provider.
                this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                //// Obtain the tracing service from the service provider.
                this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the Organization Service factory service from the service provider
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                //// Use the factory to generate the Organization Service.
                this.OrganizationService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);
            }

            private LocalPluginContext()
            {
            }

            internal IServiceProvider ServiceProvider
            {
                get;

                private set;
            }

            internal IOrganizationService OrganizationService
            {
                get;

                private set;
            }

            internal IPluginExecutionContext PluginExecutionContext
            {
                get;

                private set;
            }

            internal ITracingService TracingService
            {
                get;

                private set;
            }

            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                if (this.PluginExecutionContext == null)
                {
                    this.TracingService.Trace(message);
                }
                else
                {
                    this.TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this.PluginExecutionContext.CorrelationId,
                        this.PluginExecutionContext.InitiatingUserId);
                }
            }
        }
    }
}
