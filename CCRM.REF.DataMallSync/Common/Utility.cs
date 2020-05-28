// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   Utility class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.DataMallSync.Common
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [ErrorCodeClassInfo(UniqueKey = "Utility", MinValue = 2147482647, MaxValue = 2147483647)]
    public partial class Utility : Plugin
    {
        [ThreadStatic]
        private static Utility instance;
        
        public Utility(LocalPluginContext localPluginContext)
            : base(typeof(Utility))
        {
            this.LocalContext = localPluginContext;
            instance = this;
        }
    }
}
