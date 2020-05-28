// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestLogging.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  RequestLogging class to log the Telemetry
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Logging entity for external requests
    /// </summary>
    public class RequestLogging : IRequestLogging
    {
        /// <summary>
        /// Name of the logging entity
        /// </summary>
        private const string LoggingEntity = "smp_telemetrylog";

        /// <summary>
        /// Maximum length of the Stack trace field
        /// </summary>
        private const int MaxStacktraceLength = 4096;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLogging" /> class.
        /// </summary>
        /// <param name="configurationValue">Configuration Values</param>
        public RequestLogging(IConfigurationRetrieval configurationValue, LocalPluginContext localPluginContext)
        {
            this.ConfigurationValues = configurationValue;
            this.LocalPluginContext = localPluginContext;
        }

        public RequestLogging(IConfigurationRetrieval configurationValue, LocalWorkflowContext localWorkflowContext)
        {
            this.ConfigurationValues = configurationValue;
            this.LocalWorkflowContext = localWorkflowContext;
        }

        /// <summary>
        /// Gets or sets a reference for configurations
        /// </summary>
        private IConfigurationRetrieval ConfigurationValues
        {
            get;
            set;
        }

        private LocalPluginContext LocalPluginContext
        {
            get;
            set;
        }

        private LocalWorkflowContext LocalWorkflowContext
        {
            get;
            set;
        }

        public void LogPluginException(Entity contextEntity, Exception exception, string sequenceId, string eventId, string eventName, string eventMessage)
        {
            StackTrace stackTrace = new StackTrace();

            trackedProperties trackedProperties = new trackedProperties();
            trackedProperties.application_id = this.LocalPluginContext.PluginExecutionContext.OrganizationName;
            trackedProperties.application_name = "Dynamics 365 CRM";
            if (contextEntity.Attributes.Contains("smp_correlationid"))
            {
                trackedProperties.correlation_id = contextEntity["smp_correlationid"].ToString();
            }
            else
            {
                trackedProperties.correlation_id = this.LocalPluginContext.PluginExecutionContext.CorrelationId.ToString();
            }
            trackedProperties.correlation_name = "";
            trackedProperties.error_code = "";
            trackedProperties.error_message = exception.Message;
            trackedProperties.error_stack = exception.StackTrace;
            trackedProperties.event_category = "Plugin";
            trackedProperties.event_id = eventId;
            trackedProperties.event_message = eventMessage;
            trackedProperties.event_name = eventName;
            if (contextEntity.Attributes.Contains("msdyn_iotalert") && contextEntity.GetAttributeValue<EntityReference>("msdyn_iotalert") != null)
            {
                trackedProperties.fault_id = contextEntity.GetAttributeValue<EntityReference>("msdyn_iotalert").Name;
            }
            else
            {
                trackedProperties.fault_id = "";
            }
            trackedProperties.receiver_id = "";
            trackedProperties.receiver_name = "";
            trackedProperties.sender_id = "";
            trackedProperties.sender_name = "";
            trackedProperties.sequence_id = sequenceId;
            trackedProperties.service_request_id = "";
            trackedProperties.severity = "5";
            trackedProperties.success_code = "2";
            trackedProperties.success_message = "Failed";
            trackedProperties.timestamp = DateTime.UtcNow.ToString();
            trackedProperties.warnings = "";
            if (this.LocalPluginContext.PluginExecutionContext.PrimaryEntityName == "msdyn_workorder")
            {
                if (contextEntity.Attributes.Contains("msdyn_name") && contextEntity["msdyn_name"] != null)
                {
                    trackedProperties.work_order = contextEntity["msdyn_name"].ToString();
                }
                else
                {
                    trackedProperties.work_order = contextEntity.Id.ToString();
                }
            }
            else
            {
                trackedProperties.work_order = "";
            }
            trackedProperties.workflow_id = this.LocalPluginContext.PluginExecutionContext.RequestId.ToString();
            trackedProperties.workflow_name = this.LocalPluginContext.PluginExecutionContext.PrimaryEntityName;

            LogMessage logMessage = new LogMessage();
            logMessage.trackedProperties = trackedProperties;
            string json = Serialization.Serialize(logMessage);
            Entity entity = this.BuildEntity("", json, "Exception");
            LocalPluginContext.OrganizationService.Create(entity);

        }

        public void LogPluginTrace(Entity contextEntity, string sequenceId, string eventId, string eventName, string eventMessage)
        {
            StackTrace stackTrace = new StackTrace();

            trackedProperties trackedProperties = new trackedProperties();
            trackedProperties.application_id = this.LocalPluginContext.PluginExecutionContext.OrganizationName;
            trackedProperties.application_name = "Dynamics 365 CRM";
            if (contextEntity.Attributes.Contains("smp_correlationid"))
            {
                trackedProperties.correlation_id = contextEntity["smp_correlationid"].ToString();
            }
            else
            {
                trackedProperties.correlation_id = this.LocalPluginContext.PluginExecutionContext.CorrelationId.ToString();
            }
            trackedProperties.correlation_name = "";
            trackedProperties.error_code = "";
            trackedProperties.error_message = "";
            trackedProperties.error_stack = string.Empty;
            trackedProperties.event_category = "Plugin";
            trackedProperties.event_id = eventId;
            trackedProperties.event_message = eventMessage;
            trackedProperties.event_name = eventName;
            if (contextEntity.Attributes.Contains("msdyn_iotalert") && contextEntity.GetAttributeValue<EntityReference>("msdyn_iotalert") != null)
            {
                trackedProperties.fault_id = contextEntity.GetAttributeValue<EntityReference>("msdyn_iotalert").Name;
            }
            else
            {
                trackedProperties.fault_id = "";
            }
            trackedProperties.receiver_id = "";
            trackedProperties.receiver_name = "";
            trackedProperties.sender_id = "";
            trackedProperties.sender_name = "";
            trackedProperties.sequence_id = sequenceId;
            trackedProperties.service_request_id = "";
            trackedProperties.severity = "";
            trackedProperties.success_code = "1";
            trackedProperties.success_message = "Success";
            trackedProperties.timestamp = DateTime.UtcNow.ToString();
            trackedProperties.warnings = "";
            if (this.LocalPluginContext.PluginExecutionContext.PrimaryEntityName == "msdyn_workorder")
            {
                if (contextEntity.Attributes.Contains("msdyn_name") && contextEntity["msdyn_name"] != null)
                {
                    trackedProperties.work_order = contextEntity["msdyn_name"].ToString();
                }
                else
                {
                    trackedProperties.work_order = contextEntity.Id.ToString();
                }
            }
            else
            {
                trackedProperties.work_order = "";
            }
            trackedProperties.workflow_id = this.LocalPluginContext.PluginExecutionContext.RequestId.ToString();
            trackedProperties.workflow_name = this.LocalPluginContext.PluginExecutionContext.PrimaryEntityName;

            LogMessage logMessage = new LogMessage();
            logMessage.trackedProperties = trackedProperties;
            string json = Serialization.Serialize(logMessage);
            Entity entity = this.BuildEntity("", json, "Trace");
            LocalPluginContext.OrganizationService.Create(entity);
        }

        public void LogWorkflowException(Exception exception, string sequenceId, string eventId, string eventName, string eventMessage)
        {
            StackTrace stackTrace = new StackTrace();

            trackedProperties trackedProperties = new trackedProperties();
            trackedProperties.application_id = this.LocalWorkflowContext.WorkFlowContext.OrganizationName;
            trackedProperties.application_name = "Dynamics 365 CRM";
            trackedProperties.correlation_id = this.LocalWorkflowContext.WorkFlowContext.CorrelationId.ToString();
            trackedProperties.correlation_name = "";
            trackedProperties.error_code = "";
            trackedProperties.error_message = exception.Message;
            trackedProperties.error_stack = exception.StackTrace;
            trackedProperties.event_category = "Workflow";
            trackedProperties.event_id = eventId;
            trackedProperties.event_message = eventMessage;
            trackedProperties.event_name = eventName;
            trackedProperties.fault_id = "";
            trackedProperties.receiver_id = "";
            trackedProperties.receiver_name = "";
            trackedProperties.sender_id = "";
            trackedProperties.sender_name = "";
            trackedProperties.sequence_id = sequenceId;
            trackedProperties.service_request_id = "";
            trackedProperties.severity = "5";
            trackedProperties.success_code = "2";
            trackedProperties.success_message = "Failed";
            trackedProperties.timestamp = DateTime.UtcNow.ToString();
            trackedProperties.warnings = "";
            trackedProperties.work_order = "";
            trackedProperties.workflow_id = this.LocalWorkflowContext.WorkFlowContext.RequestId.ToString();
            trackedProperties.workflow_name = this.LocalWorkflowContext.WorkFlowContext.PrimaryEntityName;

            LogMessage logMessage = new LogMessage();
            logMessage.trackedProperties = trackedProperties; string json = Serialization.Serialize(logMessage);
            Entity entity = this.BuildEntity("", json, "Exception");
            LocalWorkflowContext.OrganizationService.Create(entity);
        }

        public void LogWorkflowTrace(string sequenceId, string eventId, string eventName, string eventMessage)
        {
            StackTrace stackTrace = new StackTrace();

            trackedProperties trackedProperties = new trackedProperties();
            trackedProperties.application_id = this.LocalWorkflowContext.WorkFlowContext.OrganizationName;
            trackedProperties.application_name = "Dynamics 365 CRM";
            trackedProperties.correlation_id = this.LocalWorkflowContext.WorkFlowContext.CorrelationId.ToString();
            trackedProperties.correlation_name = "";
            trackedProperties.error_code = "";
            trackedProperties.error_message = "";
            trackedProperties.error_stack = "";
            trackedProperties.event_category = "Workflow";
            trackedProperties.event_id = eventId;
            trackedProperties.event_message = eventMessage;
            trackedProperties.event_name = eventName;
            trackedProperties.fault_id = "";
            trackedProperties.receiver_id = "";
            trackedProperties.receiver_name = "";
            trackedProperties.sender_id = "";
            trackedProperties.sender_name = "";
            trackedProperties.sequence_id = sequenceId;
            trackedProperties.service_request_id = "";
            trackedProperties.severity = "";
            trackedProperties.success_code = "1";
            trackedProperties.success_message = "Success";
            trackedProperties.timestamp = DateTime.UtcNow.ToString();
            trackedProperties.warnings = "";
            trackedProperties.work_order = "";
            trackedProperties.workflow_id = this.LocalWorkflowContext.WorkFlowContext.RequestId.ToString();
            trackedProperties.workflow_name = this.LocalWorkflowContext.WorkFlowContext.PrimaryEntityName;

            LogMessage logMessage = new LogMessage();
            logMessage.trackedProperties = trackedProperties;
            string json = Serialization.Serialize(logMessage);
            Entity entity = this.BuildEntity("", json, "Exception");
            LocalWorkflowContext.OrganizationService.Create(entity);
        }

        /// <summary>
        /// Constructs a CRM entity record
        /// </summary>
        /// <param name="isError">true if the information is an error</param>
        /// <param name="uri">requesting URI</param>        
        /// <param name="startTime">Start time of Job</param>
        /// <param name="endTime">End time of Job</param>
        /// <param name="name">Name of the record to be created</param>
        /// <param name="stackTrace">stack trace if any</param>
        /// <param name="innerException">inner exception if any</param>
        /// <returns>entity that has NOT been saved to CRM</returns>
        private Entity BuildEntity(string name, string jsonLog, string messageType)
        {
            Entity entity = new Entity(LoggingEntity);

            if (jsonLog != null && jsonLog.Length >= MaxStacktraceLength)
            {
                jsonLog = jsonLog.Substring(0, MaxStacktraceLength);
            }

            Guid guid = Guid.NewGuid();
            entity["smp_name"] = guid.ToString();
            entity["smp_telemetrylogid"] = guid;
            entity["smp_jsontrackedproperty"] = jsonLog;
            return entity;
        }
    }
}
