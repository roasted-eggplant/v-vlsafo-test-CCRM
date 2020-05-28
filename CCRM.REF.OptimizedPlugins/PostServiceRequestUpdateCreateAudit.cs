// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestUpdateCreateAudit.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostServiceRequestUpdateCreateAudit Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// PostServiceRequestUpdateCreateAudit Plugin.
    /// Fires when the following attributes are updated:
    /// All Attributes
    /// </summary>    
    public class PostServiceRequestUpdateCreateAudit : Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostServiceRequestUpdateCreateAudit"/> class.
        /// </summary>
        public PostServiceRequestUpdateCreateAudit()
            : base(typeof(PostServiceRequestUpdateCreateAudit))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "incident", new Action<LocalPluginContext>(this.ExecutePostServiceRequestUpdateCreateAudit)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected void ExecutePostServiceRequestUpdateCreateAudit(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            Entity preImageServiceRequest = (context.PreEntityImages != null && context.PreEntityImages.Contains(this.preImageAlias)) ? context.PreEntityImages[this.preImageAlias] : null;
            Entity postImageServiceRequest = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;
            var service = localContext.OrganizationService;
            try
            {
                if (service != null && preImageServiceRequest != null && postImageServiceRequest != null)
                {
                    List<string> auditConfigDetails = RetrieveAuditConfig(preImageServiceRequest.LogicalName, service);
                    if (auditConfigDetails != null && auditConfigDetails.Count > 0)
                    {
                        BuildAuditLog(auditConfigDetails, preImageServiceRequest, postImageServiceRequest, service, context);
                    }
                }
            }
            catch (CustomServiceManagementPortalException)
            {
                ////Kill the exception so it does not affect the other plugin pipeline execution
            }
        }

        /// <summary>
        /// Builds the audit log.
        /// </summary>
        /// <param name="auditConfigDetails">The audit config details.</param>
        /// <param name="preImageServiceRequest">The pre image service request.</param>
        /// <param name="postImageServiceRequest">The post image service request.</param>
        /// <param name="service">The service.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="CustomServiceManagementPortalException">Throw the exception caught in the main method</exception>
        private static void BuildAuditLog(List<string> auditConfigDetails, Entity preImageServiceRequest, Entity postImageServiceRequest, IOrganizationService service, IPluginExecutionContext context)
        {
            try
            {
                string serviceRequestStatus = postImageServiceRequest.Attributes[CRMAttributesResource.StatusCodeAttribute] == null ? string.Empty : AuditHelper.GetOptionsSetTextOnValue(service, CRMAttributesResource.IncidentEntity, CRMAttributesResource.StatusCodeAttribute, ((OptionSetValue)postImageServiceRequest.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value);
                foreach (string attributeName in auditConfigDetails)
                {
                    Entity auditLog = new Entity { LogicalName = CRMAttributesResource.AuditLogEntityName };
                    string previousValue = preImageServiceRequest.Contains(attributeName) ? AuditHelper.GetPropertyValue(preImageServiceRequest[attributeName], service, attributeName) : string.Empty;
                    string recentValue = postImageServiceRequest.Contains(attributeName) ? AuditHelper.GetPropertyValue(postImageServiceRequest[attributeName], service, attributeName) : string.Empty;
                    if (previousValue != recentValue)
                    {
                        auditLog[CRMAttributesResource.AuditLogEventAttribute] = "Update";
                        auditLog[CRMAttributesResource.AuditLogEventTimeAttribute] = DateTime.UtcNow;
                        auditLog[CRMAttributesResource.AuditLogIncidentStatusAttribute] = serviceRequestStatus;
                        auditLog[CRMAttributesResource.AuditLogInitiatingUserAttribute] = new EntityReference(CRMAttributesResource.SystemUserEntity, context.InitiatingUserId);
                        auditLog[CRMAttributesResource.AuditLogPreviousValueAttribute] = previousValue;
                        auditLog[CRMAttributesResource.AuditLogServiceRequestAttribute] = new EntityReference(CRMAttributesResource.IncidentEntity, postImageServiceRequest.Id);
                        auditLog[CRMAttributesResource.AuditLogAttributeNameAttribute] = attributeName;
                        auditLog[CRMAttributesResource.AuditLogCurrentValueAttribute] = recentValue;
                        service.Create(auditLog);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed to create Audit Entity", ex);
            }
        }

        /// <summary>
        /// Retrieves the audit config.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="service">The service.</param>
        /// <returns>A list with all attributes from config entity</returns>
        /// <exception cref="CustomServiceManagementPortalException">Throw the custom  exception caught in the main method</exception>
        private static List<string> RetrieveAuditConfig(string entityName, IOrganizationService service)
        {
            try
            {
                List<string> attributeList = new List<string>();
                ConditionExpression condIncident = new ConditionExpression
                {
                    AttributeName = CRMAttributesResource.AuditConfigEntityNameAttribute,
                    Operator = ConditionOperator.Equal,
                    Values = { entityName }
                };
                ConditionExpression condStatus = new ConditionExpression
                {
                    AttributeName = CRMAttributesResource.StatusCodeAttribute,
                    Operator = ConditionOperator.Equal,
                    Values = { CRMAttributesResource.ActiveStatusCodeValue }
                };
                QueryExpression auditConfigQuery = new QueryExpression
                {
                    EntityName = CRMAttributesResource.AuditConfigurationEntity,
                    ColumnSet = new ColumnSet(new string[] { CRMAttributesResource.AuditConfigEntityNameAttribute, CRMAttributesResource.AuditConfigAttributeNameAttribute }),
                    Criteria =
                    {
                        FilterOperator = LogicalOperator.And,
                        Conditions = { condIncident, condStatus }
                    }
                };
                if (service != null)
                {
                    foreach (Entity auditConfig in service.RetrieveMultiple(auditConfigQuery).Entities)
                    {
                        if (auditConfig.Attributes.Contains(CRMAttributesResource.AuditConfigAttributeNameAttribute))
                        {
                            attributeList.Add(Convert.ToString(auditConfig.Attributes[CRMAttributesResource.AuditConfigAttributeNameAttribute], CultureInfo.InvariantCulture));
                        }
                    }
                }

                return attributeList;
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed to retrieve the audit configuration details.", ex);
            }
        }
    }
}
