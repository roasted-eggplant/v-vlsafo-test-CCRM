// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidExtension.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// EntityReferenceExtension class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.PluginBase
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Linq;

    public static class GuidExtension
    {
        public static T ToEntity<T>(this Guid? recordId, IOrganizationService service, string entityLogicalName, ColumnSet columnSet = null, string primaryIdAttributeLogicalName = "")
        where T : Entity
        {
            if (recordId == null)
            {
                recordId = Guid.Empty;
            }

            return ToEntity<T>((Guid)recordId, service, entityLogicalName, columnSet, primaryIdAttributeLogicalName);
        }

        public static Entity ToEntity(this Guid? recordId, IOrganizationService service, string entityLogicalName, ColumnSet columnSet = null, string primaryIdAttributeLogicalName = "")
        {
            if (recordId == null)
            {
                recordId = Guid.Empty;
            }

            return ToEntity<Entity>((Guid)recordId, service, entityLogicalName, columnSet, primaryIdAttributeLogicalName);
        }

        public static Entity ToEntity(this Guid recordId, IOrganizationService service, string entityLogicalName, ColumnSet columnSet = null, string primaryIdAttributeLogicalName = "")
        {
            if (recordId == null)
            {
                recordId = Guid.Empty;
            }

            return ToEntity<Entity>(recordId, service, entityLogicalName, columnSet, primaryIdAttributeLogicalName);
        }

        public static T ToEntity<T>(this Guid recordId, IOrganizationService service, string entityLogicalName, ColumnSet columnSet = null, string primaryIdAttributeLogicalName = "")
            where T : Entity
        {
            if (recordId == Guid.Empty || string.IsNullOrEmpty(entityLogicalName) || string.IsNullOrWhiteSpace(entityLogicalName))
            {
                return default(T);
            }
            else
            {
                if (columnSet == null)
                {
                    columnSet = new ColumnSet(true);
                }

                if (string.IsNullOrEmpty(primaryIdAttributeLogicalName) || string.IsNullOrWhiteSpace(primaryIdAttributeLogicalName))
                {
                    primaryIdAttributeLogicalName = string.Format("{0}id", entityLogicalName);
                }

                QueryExpression query = new QueryExpression
                {
                    EntityName = entityLogicalName,
                    ColumnSet = columnSet,
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = primaryIdAttributeLogicalName,
                                Operator = Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal,
                                Values = { recordId }
                            }
                        }
                    }
                };

                EntityCollection entityCollection = service.RetrieveMultiple(query);
                if (entityCollection.Entities.Count == 0)
                {
                    return default(T);
                }
                else
                {
                    return entityCollection.Entities.FirstOrDefault().ToEntity<T>();
                }
            }
        }
    }
}
