// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityReferenceExtension.cs" company="Microsoft">
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
    using System.Linq;

    public static class EntityReferenceExtension
    {
        public static Entity ToEntity(this EntityReference entityReference, IOrganizationService service, ColumnSet columnSet = null, string primaryIdAttributeLogicalName = "")
        {
            return ToEntity<Entity>(entityReference, service, columnSet, primaryIdAttributeLogicalName);
        }

        public static T ToEntity<T>(this EntityReference entityReference, IOrganizationService service, ColumnSet columnSet = null, string primaryIdAttributeLogicalName = "")
            where T : Entity
        {
            if (entityReference == null)
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
                    primaryIdAttributeLogicalName = string.Format("{0}id", entityReference.LogicalName);
                }

                QueryExpression query = new QueryExpression
                {
                    EntityName = entityReference.LogicalName,
                    ColumnSet = columnSet,
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = primaryIdAttributeLogicalName,
                                Operator = Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal,
                                Values = { entityReference.Id }
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
