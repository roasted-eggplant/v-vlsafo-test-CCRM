// --------------------------------------------------------------------------------------------------------------------
// <copyright file="trackedProperties.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  JSON tracked properties for Telemetry Log
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    public class trackedProperties
    {
        public string application_id { get; set; }

        public string application_name { get; set; }

        public string correlation_id { get; set; }

        public string correlation_name { get; set; }

        public string error_code { get; set; }

        public string error_message { get; set; }

        public string error_stack { get; set; }

        public string event_category { get; set; }

        public string event_id { get; set; }

        public string event_message { get; set; }

        public string event_name { get; set; }

        public string fault_id { get; set; }

        public string receiver_id { get; set; }

        public string receiver_name { get; set; }

        public string sender_id { get; set; }

        public string sender_name { get; set; }

        public string sequence_id { get; set; }

        public string service_request_id { get; set; }

        public string severity { get; set; }

        public string success_code { get; set; }

        public string success_message { get; set; }

        public string timestamp { get; set; }

        public string warnings { get; set; }

        public string work_order { get; set; }

        public string workflow_id { get; set; }

        public string workflow_name { get; set; }
    }
}
