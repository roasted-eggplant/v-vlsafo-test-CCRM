// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCodeClassInfo.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// ErrorCodeClassInfo class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    using System;

    [AttributeUsageAttribute(AttributeTargets.Class)]
    public sealed class ErrorCodeClassInfo : System.Attribute
    {
        public ErrorCodeClassInfo()
        {
        }

        public ErrorCodeClassInfo(string uniqueKey, int minValue, int maxValue)
        {
            this.UniqueKey = uniqueKey;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public string UniqueKey { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}
