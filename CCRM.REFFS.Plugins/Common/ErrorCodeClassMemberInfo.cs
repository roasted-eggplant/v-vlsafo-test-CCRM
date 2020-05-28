// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCodeClassMemberInfo.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// ErrorHandlerUtility class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    using System;

    [AttributeUsageAttribute(AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ErrorCodeClassMemberInfo : System.Attribute
    {
        public ErrorCodeClassMemberInfo()
        {
        }

        public ErrorCodeClassMemberInfo(string uniqueKey)
        {
            this.UniqueKey = uniqueKey;
        }

        public string UniqueKey { get; set; }
    }
}
