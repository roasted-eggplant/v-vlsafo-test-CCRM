// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreProviderCreateDuplicateDetection.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreProviderCreateDuplicateDetection Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// PreProviderCreateDuplicateDetection Plugin.
    /// </summary>    
    public class PreProviderCreateDuplicateDetection : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreProviderCreateDuplicateDetection"/> class.
        /// </summary>
        public PreProviderCreateDuplicateDetection()
            : base(typeof(PreProviderCreateDuplicateDetection))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, "Create", "account", new Action<LocalPluginContext>(this.ExecutePreProviderCreateDuplicateDetection)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected void ExecutePreProviderCreateDuplicateDetection(LocalPluginContext localContext)
        {
            string accountName = string.Empty;
            if (localContext == null)
            {
                return;
            }

            // TODO: Implement your custom Plug-in business logic.
            IPluginExecutionContext context = localContext.PluginExecutionContext;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                var service = localContext.OrganizationService;
                if (context.Depth <= 1)
                {
                    if (entity.LogicalName == "account")
                    {
                        try
                        {
                            accountName = entity.Attributes["name"].ToString();
                            GetDuplicateProvider(service, accountName);
                        }
                        catch (CustomServiceManagementPortalException)
                        {
                            ////throw;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the duplicate provider.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <exception cref="Microsoft.Xrm.Sdk.InvalidPluginExecutionException">Provider with the same name already exist</exception>
        private static void GetDuplicateProvider(IOrganizationService service, string accountName)
        {
            try
            {
                if (!string.IsNullOrEmpty(accountName))
                {
                    QueryExpression providerquery = new QueryExpression()
                    {
                        EntityName = "account",
                        ColumnSet = new ColumnSet("name"),
                        Criteria =
                        {
                            Conditions =
                                {
                                    new ConditionExpression("name", ConditionOperator.Equal, accountName),
                                }
                        }
                    };

                    if (service != null)
                    {
                        Entity provider = service.RetrieveMultiple(providerquery).Entities.FirstOrDefault();
                        if (provider != null)
                        {
                            throw new InvalidPluginExecutionException("Provider with the same name already exist.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
