// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestEnum.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ServiceRequestEnum
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Helpers
{
    public class ServiceRequestEnum
    {
        /// <summary>
        /// Status Code contains all the status of the Service Request.
        /// </summary>
        public enum StatusCode
        {
            Acknowledged = 180620004,

            Approved = 4,

            Backordered = 180620018,

            CallBack = 180620001,

            Completed = 180620011,

            Declined = 180620000,

            Dispatched = 180620002,

            Draft = 1,

            InProgress = 180620005,

            OnHoldPendingCustomerScheduling = 180620017,

            OnHoldPendingParts = 180620007,

            OnHoldPendingVendorArrival = 180620008,

            OnHoldPendingVendorScheduling = 180620009,

            Open = 2,

            PendingApproval = 3,

            PendingCSRDispatch = 180620012,

            PendingTransmittal = 180620003,

            Referred = 180620010,

            Revised = 180620006,

            WaitingForApproval = 180620013,

            InformationProvided = 1000,

            Closed = 5,

            Cancelled = 6,
        }

        /// <summary>
        /// Service Request Origin contains all the origins of the Service Request.
        /// </summary>
        public enum ServiceRequestOrigin
        {
            Facebook = 2483,

            Twitter = 3986,

            GarconBadgeProject = 100009000,

            IOT = 100008999,

            Integration = 180620004,

            Mobile = 180620005,

            Reclassified = 180620003,

            Phone = 1,

            Web = 3,

            Fax = 180620000,

            Chat = 180620001,

            PDA = 180620002,

            Email = 2
        }
    }
}
