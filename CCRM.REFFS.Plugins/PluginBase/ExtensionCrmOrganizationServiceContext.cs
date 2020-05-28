// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionCrmOrganizationServiceContext.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// ExtensionCrmOrganizationServiceContext class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.PluginBase
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class ExtensionCrmOrganizationServiceContext
    {
        public static IEnumerable<T> RetrieveMultiple<T>(this IOrganizationService service, QueryByAttribute queryByAttribute) where T : Entity
        {
            EntityCollection result = new EntityCollection();

            if (queryByAttribute.TopCount == 0 && queryByAttribute.PageInfo.Count == 0)
            {
                queryByAttribute.PageInfo = new PagingInfo
                {
                    Count = 100,
                    PageNumber = 1
                };
            }

            while (true)
            {
                EntityCollection retrieveMultipleResult = service.RetrieveMultiple(queryByAttribute);

                if (retrieveMultipleResult.Entities != null)
                {
                    result.Entities.AddRange(retrieveMultipleResult.Entities);
                }

                //// Check for more records, if it returns true.
                if (retrieveMultipleResult.MoreRecords)
                {
                    //// Increment the page number to retrieve the next page.
                    queryByAttribute.PageInfo.PageNumber++;
                    //// Set the paging cookie to the paging cookie returned from current results.
                    queryByAttribute.PageInfo.PagingCookie = retrieveMultipleResult.PagingCookie;
                }
                else
                {
                    //// If no more records are in the result nodes, exit the loop.
                    break;
                }
            }

            return result.Entities.Select(item => item.ToEntity<T>());
        }

        public static IEnumerable<T> RetrieveMultiple<T>(this IOrganizationService service, QueryExpression queryExpression) where T : Entity
        {
            EntityCollection result = new EntityCollection();

            if (queryExpression.TopCount == 0 && queryExpression.PageInfo.Count == 0)
            {
                queryExpression.PageInfo = new PagingInfo
                {
                    Count = 100,
                    PageNumber = 1
                };
            }

            while (true)
            {
                EntityCollection retrieveMultipleResult = service.RetrieveMultiple(queryExpression);

                if (retrieveMultipleResult.Entities != null)
                {
                    result.Entities.AddRange(retrieveMultipleResult.Entities);
                }

                //// Check for more records, if it returns true.
                if (retrieveMultipleResult.MoreRecords)
                {
                    //// Increment the page number to retrieve the next page.
                    queryExpression.PageInfo.PageNumber++;
                    //// Set the paging cookie to the paging cookie returned from current results.
                    queryExpression.PageInfo.PagingCookie = retrieveMultipleResult.PagingCookie;
                }
                else
                {
                    //// If no more records are in the result nodes, exit the loop.
                    break;
                }
            }

            return result.Entities.Select(item => item.ToEntity<T>());
        }

        public static IEnumerable<T> RetrieveMultiple<T>(this IOrganizationService service, FetchExpression fetchExpression) where T : Entity
        {
            FetchXmlToQueryExpressionRequest conversionRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = fetchExpression.Query
            };

            FetchXmlToQueryExpressionResponse conversionResponse = (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

            return service.RetrieveMultiple<T>(conversionResponse.Query);
        }
    }
}
