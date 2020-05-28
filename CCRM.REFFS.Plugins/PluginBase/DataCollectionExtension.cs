// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCollectionExtension.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// DataCollectionExtension class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.PluginBase
{
    using Microsoft.Xrm.Sdk;
    using System.Collections.Generic;
    using System.Linq;

    public static class DataCollectionExtension
    {
        public static List<T> ToList<T>(this DataCollection<Entity> entities) where T : Entity
        {
            return entities.Select(item => item.ToEntity<T>()).ToList<T>();
        }
    }
}
