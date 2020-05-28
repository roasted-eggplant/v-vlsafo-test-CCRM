// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorLogHeader.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// ErrorLogHeader class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    public class ErrorLogHeader
    {
        internal string OrganizationName { get; set; }

        internal string InitiatingUser { get; set; }

        internal string ExceptionType { get; set; }

        internal string ErrorSource { get; set; }

        internal ErrorCode ErrorCode { get; set; }

        internal string ErrorMessage { get; set; }

        internal string Occurence { get; set; }

        internal int Depth { get; set; }

        internal string Message { get; set; }

        internal string Stage { get; set; }

        internal string Entity { get; set; }

        internal string SupportInformation { get; set; }
    }
}
