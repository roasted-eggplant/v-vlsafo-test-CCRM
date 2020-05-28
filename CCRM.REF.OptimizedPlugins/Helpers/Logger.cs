// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Logger Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using System;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Class Logger
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Writes the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="service">The service.</param>
        /// <param name="referencedEntityName">Name of the referenced entity.</param>
        /// <param name="referencedFieldName">Name of the referenced field.</param>
        /// <param name="referencedId">The referenced id.</param>
        /// <param name="missingData">The missing data.</param>
        public static void Write(Exception exception, ExceptionType exceptionType, IOrganizationService service, string referencedEntityName, string referencedFieldName, Guid? referencedId, string missingData)
        {
            try
            {
                if (service != null && exception != null && exceptionType != ExceptionType.None)
                {
                    Entity exceptionLog = new Entity();
                    exceptionLog.LogicalName = "smp_exceptionlog";
                    exceptionLog["smp_exceptiontype"] = new OptionSetValue((int)exceptionType);
                    if (exception.ToString().Length > 1999)
                    {
                        exceptionLog["smp_exceptiondetails"] = exception.ToString().Substring(0, 1999);
                    }
                    else
                    {
                        exceptionLog["smp_exceptiondetails"] = exception.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(exception.Message))
                    {
                        if (exception.Message.Length > 99)
                        {
                            exceptionLog["smp_description"] = exception.Message.Substring(0, 99);
                        }
                        else
                        {
                            exceptionLog["smp_description"] = exception.Message;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(missingData))
                    {
                        exceptionLog["smp_missingdatadetails"] = missingData;
                    }

                    if (referencedId.HasValue)
                    {
                        exceptionLog[referencedFieldName] = new EntityReference(referencedEntityName, referencedId.Value);
                    }

                    service.Create(exceptionLog);
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error occurred while logging", ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Writes the specified exception message.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="service">The service.</param>
        /// <param name="referencedEntityName">Name of the referenced entity, incident or account</param>
        /// <param name="referencedFieldName">Name of the referenced field.</param>
        /// <param name="referencedId">The referenced id.</param>
        /// <param name="missingData">The missing data.</param>
        public static void Write(string exceptionMessage, ExceptionType exceptionType, IOrganizationService service, string referencedEntityName, string referencedFieldName, Guid? referencedId, string missingData)
        {
            try
            {
                if (service != null && !string.IsNullOrWhiteSpace(exceptionMessage) && exceptionType != ExceptionType.None)
                {
                    Entity exceptionLog = new Entity();
                    exceptionLog.LogicalName = "smp_exceptionlog";
                    exceptionLog["smp_exceptiontype"] = new OptionSetValue((int)exceptionType);
                    if (!string.IsNullOrWhiteSpace(exceptionMessage))
                    {
                        if (exceptionMessage.Length > 99)
                        {
                            exceptionLog["smp_description"] = exceptionMessage.Substring(0, 99);
                        }
                        else
                        {
                            exceptionLog["smp_description"] = exceptionMessage;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(missingData))
                    {
                        exceptionLog["smp_missingdatadetails"] = missingData;
                    }

                    if (referencedId.HasValue)
                    {
                        exceptionLog[referencedFieldName] = new EntityReference(referencedEntityName, referencedId.Value);
                    }

                    service.Create(exceptionLog);
                }
            }
            catch (Exception ex)
            {
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException("Error occurred while logging", ex);
                throw customEx;
            }
        }
    }
}
