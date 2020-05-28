// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestLogging.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  IRequestLogging interface to log the Telemetry
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using Microsoft.Xrm.Sdk;
    using System;

    /// <summary>
    /// Interface of Logging entity for external requests
    /// </summary>
    public interface IRequestLogging
    {
        void LogPluginException(Entity contextEntity, Exception exception, string sequenceId, string eventId, string eventName, string eventMessage);
        void LogPluginTrace(Entity contextEntity, string sequenceId, string eventId, string eventName, string eventMessage);
        void LogWorkflowException(Exception exception, string sequenceId, string eventId, string eventName, string eventMessage);
        void LogWorkflowTrace(string sequenceId, string eventId, string eventName, string eventMessage);
    }
}
