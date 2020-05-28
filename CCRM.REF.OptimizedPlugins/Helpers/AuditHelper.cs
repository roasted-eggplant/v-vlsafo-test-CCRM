// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AuditHelper Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.ServiceModel;

    /// <summary>
    /// Helper class for Audit functionality.
    /// </summary>
    public static class AuditHelper
    {
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="service">The service.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The value of the attribute</returns>
        public static string GetPropertyValue(object attribute, IOrganizationService service, string attributeName)
        {
            if (attribute == null)
            {
                return string.Empty;
            }

            string value = "Not Mentioned";
            switch (attribute.ToString())
            {
                case "bool":
                    value = Convert.ToString((bool)attribute, CultureInfo.InvariantCulture);
                    break;
                case "System.DateTime":
                    DateTime dateTime = (DateTime)attribute;
                    value = dateTime.ToString(CultureInfo.InvariantCulture);
                    break;
                case "Microsoft.Xrm.Sdk.OptionSetValue":
                    OptionSetValue crmOptionSet = (OptionSetValue)attribute;
                    ////For now, make it work only for incident
                    value = GetOptionsSetTextOnValue(service, CRMAttributesResource.IncidentEntity, attributeName, crmOptionSet.Value);
                    break;
                case "System.Guid":
                    value = Convert.ToString((Guid)attribute, CultureInfo.InvariantCulture);
                    break;
                case "Microsoft.Xrm.Sdk.EntityReference":
                    value = Convert.ToString(((EntityReference)attribute).Name, CultureInfo.InvariantCulture);
                    break;
                case "Microsoft.Xrm.Sdk.Money":
                    value = Convert.ToString(((Money)attribute).Value, CultureInfo.InvariantCulture);
                    break;
                case "string":
                    value = attribute.ToString();
                    break;
                default:
                    value = attribute.ToString();
                    break;
            }

            return value;
        }

        /// <summary>
        /// Gets the options set text on value.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <returns>The option set label</returns>
        internal static string GetOptionsSetTextOnValue(IOrganizationService service, string entityName, string attributeName, int selectedValue)
        {
            try
            {
                RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest { EntityLogicalName = entityName, LogicalName = attributeName, RetrieveAsIfPublished = true };
                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)service.Execute(retrieveAttributeRequest);
                OptionMetadata[] optionList = null;
                if (((MemberInfo)retrieveAttributeResponse.AttributeMetadata.GetType()).Name.ToUpperInvariant() == "STATUSATTRIBUTEMETADATA")
                {
                    StatusAttributeMetadata retrievedPicklistAttributeMetadata = (StatusAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
                    optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
                }

                if (((MemberInfo)retrieveAttributeResponse.AttributeMetadata.GetType()).Name.ToUpperInvariant() == "PICKLISTATTRIBUTEMETADATA")
                {
                    PicklistAttributeMetadata retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
                    optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
                }

                if (((MemberInfo)retrieveAttributeResponse.AttributeMetadata.GetType()).Name.ToUpperInvariant() == "STATEATTRIBUTEMETADATA")
                {
                    StateAttributeMetadata retrievedPicklistAttributeMetadata = (StateAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;
                    optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
                }

                string selectedOptionLabel = string.Empty;
                if (optionList.Length > 0)
                {
                    foreach (OptionMetadata oMD in optionList)
                    {
                        if (oMD.Value == selectedValue)
                        {
                            selectedOptionLabel = oMD.Label.UserLocalizedLabel.Label;
                            break;
                        }
                    }
                }

                return selectedOptionLabel;
            }
            catch (FaultException)
            {
                return string.Empty;
            }
        }
    }
}
