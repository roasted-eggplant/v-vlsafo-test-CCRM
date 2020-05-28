// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateUpdateIncidentInPortal.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostCreateUpdateIncidentInPortal Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    /// <summary>
    /// PostCreateUpdateIncidentInPortal Plugin.
    /// </summary>   
    public class PostCreateUpdateIncidentInPortal : IPlugin
    {
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
        /// 
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService trace =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            //// Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                if ((context.InputParameters == null) || (!context.InputParameters.Contains("Target")))
                {
                    return;
                }

                Entity incident = context.InputParameters["Target"] as Entity;

                if (context.MessageName == "Update")
                {
                    incident = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;

                    if (incident.GetAttributeValue<Boolean>("smp_cancelledforreclassification") == true)
                    {
                        trace.Trace("Reclassified SR.  Aborting.");
                        return;
                    }
                }

                if (service != null && incident != null)
                {
                    if ((!incident.Attributes.Contains("smp_portalsubmit")) || (Convert.ToBoolean(incident.Attributes["smp_portalsubmit"]) == false))
                    {
                        if (!incident.Attributes.Contains("smp_originalservicerequestid") && incident.GetAttributeValue<EntityReference>("smp_originalservicerequestid") != null)
                        {
                            trace.Trace("Portal Submit is FALSE or unavailable.  Aborting.");
                            return;
                        }
                    }

                    if (!incident.Attributes.Contains("smp_answer") || string.IsNullOrEmpty(Convert.ToString(incident.Attributes["smp_answer"])))
                    {
                        trace.Trace("Answer is empty.  Aborting.");
                        return;
                    }

                    if (!(context.MessageName == "Create" && incident.Attributes.Contains("smp_originalservicerequestid") && incident.GetAttributeValue<EntityReference>("smp_originalservicerequestid") != null))
                    {
                        FetchAllProblemTypeDescriptionAndAssign(incident, service, trace);
                    }
                }
            }
            catch (CustomServiceManagementPortalException)
            {
                ////Kill the exception
            }
        }

        /// <summary>
        /// Fetches all problem type description and assign.
        /// </summary>
        /// <param name="postImageEntity">The post image entity.</param>
        /// <param name="service">The service.</param>
        private static void FetchAllProblemTypeDescriptionAndAssign(Entity postImageEntity, IOrganizationService service, ITracingService trace)
        {
            try
            {
                trace.Trace("In FetchAllProblemTypeDescriptionAndAssign");
                //// Retrieve the JSON answer string from the Incident Post Image.
                string dynamicAnswers = Convert.ToString(postImageEntity.Attributes["smp_answer"]);
                trace.Trace(string.Format("Answer string: {0}", dynamicAnswers));
                //// Serialize the JSON string
                DynamicAnswers dynAnswers = JsonHelper.JsonDeserialize<DynamicAnswers>(dynamicAnswers);
                //// Retrieve all the ProblemTypeDescriptions based on the JSON values
                QueryExpression problemTypeDescQuery = new QueryExpression
                {
                    EntityName = CRMAttributesResource.ProblemTypeDescriptionEntityName,
                    ColumnSet = new ColumnSet(CRMAttributesResource.ProblemTypeDescriptionIdAttribute, CRMAttributesResource.ProblemTypeDescriptionNameAttribute),
                    Criteria =
                        {
                            FilterOperator = LogicalOperator.Or
                        }
                };

                foreach (Answer ans in dynAnswers.data)
                {
                    problemTypeDescQuery.Criteria.AddCondition(CRMAttributesResource.ProblemTypeDescriptionIdAttribute, ConditionOperator.Equal, ans.id);
                }

                if (service != null)
                {
                    EntityCollection problemTypeDescColl = service.RetrieveMultiple(problemTypeDescQuery);
                    if ((problemTypeDescColl == null) || (problemTypeDescColl.Entities == null))
                    {
                        trace.Trace("Zero Dynamic Problem Types found.");
                        return;
                    }

                    trace.Trace(string.Format("Retrieved a total of {0} 'ProblemTypeDescriptions'", problemTypeDescColl.Entities.Count));
                    //// Create a DynamicProblemType for each ProblemTypeDesction found.
                    EntityCollection dynamicProblemTypes = new EntityCollection();
                    foreach (Entity problemTypeDesc in problemTypeDescColl.Entities)
                    {
                        if (problemTypeDesc.Attributes.Contains(CRMAttributesResource.ProblemTypeDescriptionIdAttribute))
                        {
                            Entity serviceRequestDynamicProblemType = new Entity(CRMAttributesResource.ServiceRequestProblemTypeEntity);
                            serviceRequestDynamicProblemType[CRMAttributesResource.ServiceRequestIdAttribute] = new EntityReference(CRMAttributesResource.IncidentEntity, postImageEntity.Id);
                            serviceRequestDynamicProblemType[CRMAttributesResource.ProblemTypeDescriptionIdAttribute] = new EntityReference(CRMAttributesResource.ProblemTypeDescriptionEntityName, problemTypeDesc.Id);
                            //// Add the answer saved in the JSON string representation
                            Answer probAnswer = dynAnswers.data.Find((x) => x.id == problemTypeDesc.Id);
                            if (probAnswer == null || string.IsNullOrEmpty(probAnswer.answer))
                            {
                                trace.Trace(string.Format("No answer found for 'ProblemTypeDesc' with ID: {0}", problemTypeDesc.Id));
                                continue;
                            }

                            serviceRequestDynamicProblemType["smp_answer"] = probAnswer.answer;
                            service.Create(serviceRequestDynamicProblemType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed to associate Dynamic notes", ex.InnerException);
            }
        }
    }
}
