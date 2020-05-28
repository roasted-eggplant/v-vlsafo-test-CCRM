// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingConstants.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Constants declaration for the Telemetry Mappings
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    public class MappingConstants
    {
        ////Retrieving to set Internal Email Address of a User and Contact
        public const string RetrieveContactEmailAddressSequenceId = "15.2";
        public const string RetrieveContactEmailAddressEventName = "Retrieving to set Internal Email Address of a User and Contact";
        public const string RetrieveCurrentUserIdSuccessEventId = "100015.2.1";
        public const string RetrieveCurrentUserIdSuccessEventMessage = "Retrieving Current User Id Success";
        public const string RetrieveInternalEmailSuccessEventId = "100015.2.2";
        public const string RetrieveInternalEmailSuccessEventMessage = "Retrieving Internal Email Address of a User Success";
        public const string RetrieveContactByEmailSuccessEventId = "100015.2.3";
        public const string RetrieveContactByEmailSuccessEventMessage = "Retrieving Contact By Primary Email Address of a Contact Success";
        public const string RetrieveDefaultContactByEmailSuccessEventId = "100015.2.4";
        public const string RetrieveDefaultContactByEmailSuccessEventMessage = "Retrieving Default Contact By Primary Email Address of a Contact Success";
        public const string RetrieveContactEmailAddressSuccessEventId = "100015.2.5";
        public const string RetrieveContactEmailAddressSuccessEventMessage = "Retrieving to set Internal Email Address of a User and Contact Success";
        public const string RetrieveContactEmailAddressFailedEventId = "500015.2";
        public const string RetrieveContactEmailAddressFailedEventMessage = "Retrieving to set Internal Email Address of a User and Contact Failed";

        ////Service Request Created
        public const string ServiceRequestCreatedSequenceId = "15.3";
        public const string ServiceRequestCreatedEventName = "Service Request Created";
        public const string ServiceRequestCreatedSuccessEventId = "100015.3";
        public const string ServiceRequestCreatedSuccessEventMessage = "Service Request Created Success";
        public const string ServiceRequestCreatedFailedEventId = "500015.3";
        public const string ServiceRequestCreatedFailedEventMessage = "Service Request Created Failed";

        ////Work Order Created
        public const string WorkOrderCreatedSequenceId = "16.1";
        public const string WorkOrderCreatedEventName = "Work Order Created";
        public const string WorkOrderCreatedSuccessEventId = "100016.1.1";
        public const string WorkOrderCreatedSuccessEventMessage = "Work Order Created Success";
        public const string IOTAlertMappedSuccessEventId = "100016.1.2";
        public const string IOTAlertMappedSuccessEventMessage = "IOT Alert mapped in Work Order Success";
        public const string CorrelationIDMappedSuccessEventId = "100016.1.3";
        public const string CorrelationIDMappedSuccessEventMessage = "Correlation ID mapped in Work Order";
        public const string CustomerAssetMappedSuccessEventId = "100016.1.4";
        public const string CustomerAssetMappedSuccessEventMessage = "Customer Asset mapped in Work Order Success";
        public const string WorkOrderCreatedFailedEventId = "500016.1";
        public const string WorkOrderCreatedFailedEventMessage = "Work Order Created Failed";

        ////Get Work Order Related Service Request
        public const string GetWORelatedToSRSequenceId = "16.2";
        public const string GetWORelatedToSREventName = "Get Work Order Related Service Request";
        public const string GetWORelatedToSRSuccessEventId = "100016.2";
        public const string GetWORelatedToSRSuccessEventMessage = "Get Work Order Related Service Request Success";
        public const string GetWORelatedToSRFailedEventId = "500016.2";
        public const string GetWORelatedToSRFailedEventMessage = "Get Work Order Related Service Request Failed";
    }
}
