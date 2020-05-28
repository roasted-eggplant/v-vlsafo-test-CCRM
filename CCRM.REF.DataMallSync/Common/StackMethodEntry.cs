// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StackMethodEntry.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// StackMethodEntry class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.DataMallSync.Common
{
    public class StackMethodEntry
    {
        internal string ErrorSource { get; set; }

        internal ErrorCode MethodErrorCode { get; set; }

        internal string ErrorMessage { get; set; }
    }
}
