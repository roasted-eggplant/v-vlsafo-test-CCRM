// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkCRUDHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  BulkCRUDHelper Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using System;

    public static class BulkCRUDHelper
    {
        public static void BulkCreate(IOrganizationService service, EntityCollection collection)
        {
            try
            {
                ////create the request
                var bulkCreate = new ExecuteMultipleRequest()
                {
                    Settings = new ExecuteMultipleSettings()
                    {
                        ////want to see the errors but don't need the response
                        ContinueOnError = false,
                        ReturnResponses = false
                    },
                    Requests = new OrganizationRequestCollection()
                };
                ////loop through the collection   
                foreach (var entity in collection.Entities)
                {
                    CreateRequest singleRequest = new CreateRequest { Target = entity };
                    bulkCreate.Requests.Add(singleRequest);
                }
                ////trace.Trace("before execute multiple response");
                //// send all at once
                ExecuteMultipleResponse multipleResponse = (ExecuteMultipleResponse)service.Execute(bulkCreate);
                ////trace.Trace("before execute multiple response");
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed in Bulk Creation :", ex.InnerException);
            }
        }
    }
}
