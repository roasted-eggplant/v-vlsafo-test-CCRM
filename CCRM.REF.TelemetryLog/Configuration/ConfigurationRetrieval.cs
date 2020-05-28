// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationRetrieval.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ConfigurationRetrieval class to retrieve Configuration values from CRM entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Linq;

    public class ConfigurationRetrieval : IConfigurationRetrieval
    {
        /// <summary>
        /// Configuration Entity Name
        /// </summary>
        private const string ConfigurationEntity = "smp_configuration";

        /// <summary>
        /// Configuration name field
        /// </summary>
        private const string ConfigurationLookup = "smp_title";

        /// <summary>
        /// Gets a value from CRM Configuration entity or cache
        /// </summary>
        /// <param name="name">Name of the config value</param>
        /// <param name="service">CRM Organization Service</param>
        /// <returns>Configuration Value</returns>
        public string Get(string name, IOrganizationService service)
        {
            ////return this.GetConfigurationValues(name, service);

            string value = CacheUtil.Get<string>(name);
            if (string.IsNullOrEmpty(value))
            {
                double cacheDurationInMinutes = 10;

                this.GetConfigurationValues(name, service, out value);
                if (string.IsNullOrEmpty(value) == false)
                {
                    CacheUtil.Set(name, value, cacheDurationInMinutes);
                }
            }

            return value;
        }

        /// <summary>
        /// Connect to CRM pull data from CRM Configuration entity
        /// </summary>
        /// <param name="name">Name of the Configuration</param>
        /// <param name="orgService">CRM Organization Service</param>
        /// <param name="value">Value of the Configuration</param>
        private void GetConfigurationValues(string name, IOrganizationService orgService, out string value)
        {
            //// query configuration entity by name 
            QueryExpression queryConfig = new QueryExpression();
            queryConfig.ColumnSet = new ColumnSet("smp_value");
            queryConfig.EntityName = ConfigurationEntity;
            queryConfig.Criteria.AddCondition(ConfigurationLookup, ConditionOperator.Equal, name);

            EntityCollection configurationCollection = orgService.RetrieveMultiple(queryConfig);
            Entity entity = configurationCollection.Entities.Where(x => x.Attributes["smp_title"].ToString().ToLower().Equals(name.ToLower())).First();
            if (entity != null)
            {
                string valueString = entity.Attributes.ContainsKey("smp_value") ? entity.GetAttributeValue<string>("smp_value").Trim() : string.Empty;
                value = valueString;
            }
            else
            {
                value = string.Empty;
            }
        }
    }
}
