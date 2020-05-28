// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageUtilities.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ImageUtilities Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using System;

    public static class ImageUtilities
    {
        /// <summary>
        /// This utility method can be used to find the final attribute of an entity in a given time.
        /// If the attribute is being updated it will retrieve the updating version of the attribute, otherwise it will use the value of the pre-image
        /// </summary>
        public static object GetFinalAttribute(string attributeName, string preEntityImageName, IExecutionContext context)
        {
            bool gettingSet;
            return GetFinalAttribute(attributeName, preEntityImageName, context, out gettingSet);
        }

        public static object GetFinalAttribute(string attributeName, string preEntityImageName, IExecutionContext context, out bool gettingSet)
        {
            object contextAtt = GetContextAttribute(attributeName, context, out gettingSet);
            if (gettingSet)
            {
                return contextAtt;
            }

            return GetOriginalAttribute(attributeName, preEntityImageName, context);
        }

        internal static object GetContextAttribute(string attributeName, IExecutionContext context, out bool gettingSet)
        {
            // Use gettingSet boolean because if the value is getting set to null there is no other way to tell whether
            // the attribute is not getting set (not in the context) or whether it is getting set to null explicitly.
            gettingSet = false;

            if (context.InputParameters.ContainsKey("Target"))
            {
                Entity newVersion = context.InputParameters["Target"] as Entity;
                if (newVersion != null && newVersion.Contains(attributeName))
                {
                    //// The attribute is being updated, use the new value
                    gettingSet = true;
                    return newVersion[attributeName];
                }
            }
            else if (attributeName.Equals("statecode", StringComparison.InvariantCultureIgnoreCase) && context.InputParameters.Contains("State"))
            {
                gettingSet = true;
                return context.InputParameters["State"];
            }
            else if (attributeName.Equals("statuscode", StringComparison.InvariantCultureIgnoreCase) && context.InputParameters.Contains("Status"))
            {
                gettingSet = true;
                return context.InputParameters["Status"];
            }

            return null;
        }

        internal static object GetOriginalAttribute(string attributeName, string preEntityImageName, IExecutionContext context)
        {
            if (context.PreEntityImages.Contains(preEntityImageName))
            {
                Entity oldVersion = context.PreEntityImages[preEntityImageName];
                if (oldVersion.Contains(attributeName))
                {
                    // Return the attribute value from the PreImage
                    return oldVersion[attributeName];
                }
            }

            return null;
        }

        internal static object GetPostImageAttribute(string attributeName, string postEntityImageName, IExecutionContext context)
        {
            if (context.PostEntityImages.Contains(postEntityImageName))
            {
                Entity newVersion = context.PostEntityImages[postEntityImageName];
                if (newVersion.Contains(attributeName))
                {
                    // Return the attribute value from the PostImage
                    return newVersion[attributeName];
                }
            }

            return null;
        }

        internal static bool IsEntityReferenceEqual(EntityReference e1, EntityReference e2)
        {
            if (e1 == null && e2 == null)
            {
                return true;
            }

            if (e1 == null || e2 == null)
            {
                return false;
            }

            return e1.Id == e2.Id;
        }

        internal static bool IsOptionSetEqual(OptionSetValue o1, OptionSetValue o2)
        {
            if (o1 == null && o2 == null)
            {
                return true;
            }

            if (o1 == null || o2 == null)
            {
                return false;
            }

            return o1.Value == o2.Value;
        }

        internal static bool IsAttributeImageEqual(string attributeName, string preEntityImageName, IExecutionContext context)
        {
            var originalValue = GetOriginalAttribute(attributeName, preEntityImageName, context);
            var finalValue = GetFinalAttribute(attributeName, preEntityImageName, context);

            return IsValueDifferent(finalValue, originalValue);
        }

        internal static bool IsStringEqual(string s1, string s2)
        {
            // In CRM pipeline null is treated as string.Empty sometimes, so treat these equally.
            // For example, when setting string field to null, the plugin will actually catch "" value in the attributes
            if (s1 == null && s2 == null)
            {
                return true;
            }

            if (s1 == null && s2 == string.Empty)
            {
                return true;
            }

            if (s1 == string.Empty && s2 == null)
            {
                return true;
            }

            return s1 == s2;
        }

        internal static bool IsValueDifferent(object newValue, object oldValue)
        {
            if (newValue == null)
            {
                if (oldValue == null)
                {
                    return false;
                }

                return true;
            }

            string stringNewValue = newValue as string;
            if (stringNewValue != null)
            {
                string stringOldValue = oldValue as string;
                return !IsStringEqual(stringOldValue, stringNewValue);
            }

            OptionSetValue osvNewValue = newValue as OptionSetValue;
            if (osvNewValue != null)
            {
                OptionSetValue osvOldValue = oldValue as OptionSetValue;
                return !IsOptionSetEqual(osvOldValue, osvNewValue);
            }

            EntityReference erNewValue = newValue as EntityReference;
            if (erNewValue != null)
            {
                EntityReference erOldValue = oldValue as EntityReference;
                return !IsEntityReferenceEqual(erOldValue, erNewValue);
            }

            int? iNewValue = newValue as int?;
            if (iNewValue != null)
            {
                int? iOldValue = oldValue as int?;
                return iOldValue != iNewValue;
            }

            bool? bNewValue = newValue as bool?;
            if (bNewValue != null)
            {
                bool? bOldValue = oldValue as bool?;
                return bOldValue != bNewValue;
            }

            return true; // Other datatypes not covered, assume value is different
        }
    }
}