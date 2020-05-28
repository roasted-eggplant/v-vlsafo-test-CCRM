// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AttributeHelper Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using System;
    using System.Linq;

    public static class AttributeHelper
    {
        public static void MapStringValue(string sourceAttribute, Entity sourceEntity, string targetAttribute, ref Entity targetEntity)
        {
            if (sourceEntity.Attributes.Contains(sourceAttribute) && sourceEntity.GetAttributeValue<string>(sourceAttribute) != string.Empty)
            {
                targetEntity[targetAttribute] = sourceEntity.GetAttributeValue<string>(sourceAttribute);
            }
        }

        ////public static void MapDecimalValue(string sourceAttribute, Entity sourceEntity, string targetAttribute, ref Entity targetEntity)
        ////{
        ////    if (sourceEntity.Attributes.Contains(sourceAttribute))
        ////    {
        ////        targetEntity[targetAttribute] = sourceEntity.GetAttributeValue<Int32>(sourceAttribute);
        ////    }
        ////}

        ////public static void MapDataTimeValueToString(string sourceAttribute, Entity sourceEntity, string targetAttribute, ref Entity targetEntity)
        ////{
        ////    if (sourceEntity.Attributes.Contains(sourceAttribute) && Convert.ToDateTime(sourceEntity[sourceAttribute]) != DateTime.MinValue)
        ////    {
        ////        targetEntity.Attributes[targetAttribute] = Convert.ToDateTime(sourceEntity.GetAttributeValue<string>(sourceAttribute));
        ////    }
        ////}

        public static void MapDataTimeValue(string sourceAttribute, Entity sourceEntity, string targetAttribute, ref Entity targetEntity)
        {
            if (sourceEntity.Attributes.Contains(sourceAttribute) && Convert.ToDateTime(sourceEntity[sourceAttribute]) != DateTime.MinValue)
            {
                targetEntity.Attributes[targetAttribute] = sourceEntity.GetAttributeValue<DateTime>(sourceAttribute);
            }
        }

        public static string GetOptionSetValueLabel(string entityName, string fieldName, int optionSetValue, IOrganizationService service)
        {
            var attReq = new RetrieveAttributeRequest();
            attReq.EntityLogicalName = entityName;
            attReq.LogicalName = fieldName;
            attReq.RetrieveAsIfPublished = true;

            var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
            if (attResponse != null)
            {
                var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

                if (attMetadata != null)
                {
                    return attMetadata.OptionSet.Options.Where(x => x.Value == optionSetValue).FirstOrDefault().Label.UserLocalizedLabel.Label;
                }
            }

            return string.Empty;
        }
    }
}