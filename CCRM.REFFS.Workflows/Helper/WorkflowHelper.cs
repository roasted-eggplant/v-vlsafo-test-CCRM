// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  WorkflowHelper Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REFFS.Workflows.Helper
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;

    public class WorkflowHelper
    {
        /// <summary>
        /// Gets the query response.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <param name="fieldsToBeFetched">The fields to be fetched.</param>
        /// <param name="criteriaField">The criteria field.</param>
        /// <param name="criteriaValue">The criteria value.</param>
        /// <returns>Returns the Entity Collection.</returns>
        private static EntityCollection GetQueryResponse(IOrganizationService service, string entityLogicalName, string[] fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = entityLogicalName;
                query.ColumnSet = new ColumnSet(fieldsToBeFetched);
                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression(criteriaField, ConditionOperator.Equal, criteriaValue));
                query.Criteria = filter;
                EntityCollection entityCollection = service.RetrieveMultiple(query);
                return entityCollection;
            }

            return null;
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="title">The title.</param>
        /// <returns>Return the value of the title</returns>
        public static string GetConfigurationValue(IOrganizationService service, string title)
        {
            try
            {
                string value = string.Empty;
                EntityCollection entityCollection = GetQueryResponse(service, "smp_configuration", new string[] { "smp_value", "smp_title" }, "smp_title", title);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    foreach (Entity entity in entityCollection.Entities)
                    {
                        if (entity.Attributes["smp_title"].ToString().ToLower() == title.ToLower())
                        {
                            value = entity.Attributes["smp_value"].ToString();
                        }
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                //CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(string.Format(CultureInfo.InvariantCulture, "Fetching configuration value for {0} failed.", title), ex);
                throw ex;
            }
        }

        public static EntityCollection GetConfigurationValueCollection(IOrganizationService service)
        {
            try
            {
                QueryExpression query = new QueryExpression("smp_configuration");
                query.ColumnSet = new ColumnSet("smp_title", "smp_value");
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                EntityCollection collection = service.RetrieveMultiple(query);
                if (collection.Entities.Count > 0)
                {
                    return collection;
                }
            }
            catch (Exception ex)
            {
                //CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Fetching configuration value for failed.", ex);
                throw ex;
            }

            return null;
        }

        public static string GetPrimaryEmailAddressOfUser(IOrganizationService service, Guid userId)
        {
            var primaryEmailByUser = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='systemuser'>
                        <attribute name='internalemailaddress' />
                        <filter type='and'>
                          <condition attribute='systemuserid' operator='eq' value = '{userId}' />
                        </filter>
                      </entity>
                    </fetch>";

            var entityCollection = service.RetrieveMultiple(new FetchExpression(primaryEmailByUser));
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection.Entities[0].GetAttributeValue<string>("internalemailaddress");
            }
            return string.Empty;
        }
        public static Entity GetContactByEmailAddress(IOrganizationService service, string emailAddress)
        {
            var contactByEmailAddress = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='contact'>
                        <attribute name='smp_alias' />
                        <attribute name='smp_iocode' />
                        <attribute name='smp_costcenter' />
                        <attribute name='emailaddress1' />
                        <attribute name='telephone1' />
                        <attribute name='smp_isprovideruser' />
                        <attribute name='contactid' />
                        <filter type='and'>
                          <condition attribute='emailaddress1' operator='eq' value='{emailAddress}' />
                        </filter>
                      </entity>
                    </fetch>";
            var entityCollection = service.RetrieveMultiple(new FetchExpression(contactByEmailAddress));
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection.Entities[0];
            }
            return null;
        }
    }
}
