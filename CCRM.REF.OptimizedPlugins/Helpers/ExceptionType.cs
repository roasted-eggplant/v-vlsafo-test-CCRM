// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionType.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ExceptionType enum
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    /// <summary>
    /// Exception Type contains different types of exception which could be set.
    /// </summary>
    public enum ExceptionType
    {
        /// <summary>
        /// Types are still to be decided upon
        /// </summary>
        None = 0,

        /// <summary>
        /// The set provider
        /// </summary>
        SetProviderFailed = 1,

        /// <summary>
        /// The set priority
        /// </summary>
        SetPriorityFailed = 2,

        /// <summary>
        /// The update service request by provider
        /// </summary>
        StatusUpdateFromProviderFailed = 3,

        /// <summary>
        /// The send service request to provider
        /// </summary>
        SendServiceRequestToProviderFailed = 4,

        /// <summary>
        /// The service request number not available
        /// </summary>
        ServiceRequestNumberNotFound = 5,

        /// <summary>
        /// The set building time zone failed
        /// </summary>
        SetBuildingTimeZoneFailed = 6,

        /// <summary>
        /// The missing required data
        /// </summary>
        MissingRequiredData = 7,

        /// <summary>
        /// The setting team to provider
        /// </summary>
        SettingTeamToProviderFailed = 8
    }
}
