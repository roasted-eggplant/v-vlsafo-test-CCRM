// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationRetrieval.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  IConfigurationRetrieval interface to retrieve Configuration values from CRM entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using Microsoft.Xrm.Sdk;

    public interface IConfigurationRetrieval
    {
        /// <summary>
        /// Gets a value from CRM Configuration entity or cache
        /// </summary>
        /// <param name="name">Name of the config value</param>
        /// <param name="service">CRM Organization Service</param>
        /// <returns>Configuration Value</returns>
        string Get(string name, IOrganizationService service);
    }
}
