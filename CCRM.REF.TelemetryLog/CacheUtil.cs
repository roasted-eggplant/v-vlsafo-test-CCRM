// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheUtil.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   CacheUtil class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using System;
    using System.Runtime.Caching;

    /// <summary>
    /// Cache Helper class that stores values in memory
    /// </summary>
    public static class CacheUtil
    {
        /// <summary>
        /// Internal cache
        /// </summary>
        private static ObjectCache cache = MemoryCache.Default;

        /// <summary>
        /// Returns a CRM Configuration value based on the name
        /// </summary>
        /// <typeparam name="T">Data type of the cache value</typeparam>
        /// <param name="name">a cache value by name</param>
        /// <returns>value stored in cache</returns>
        public static T Get<T>(string name)
        {
            ////ThrowIf.ArgumentNull(name, "name");

            T result;

            if (cache.Contains(name) == false)
            {
                result = default(T);
            }
            else
            {
                result = (T)cache.Get(name);
            }

            return result;
        }

        /// <summary>
        /// Sets the cache
        /// </summary>
        /// <param name="name">key to lookup value in the future</param>
        /// <param name="value">value to remember</param>
        /// <param name="minutesFromNow">Number of minutes to cache the value</param>
        public static void Set(string name, object value, double minutesFromNow = 10)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(minutesFromNow);
            cache.Add(name, value, policy);
        }
    }
}
