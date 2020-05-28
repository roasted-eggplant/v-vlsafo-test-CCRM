// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostUpdateSRUpdateDynNotes.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostUpdateSRUpdateDynNotes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    /// <summary>
    /// PostUpdateSRUpdateDynNotes
    /// </summary>
    public class PostUpdateSRUpdateDynNotes : Plugin
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity's attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// No Attributes
        /// </summary>
        private readonly string postImageAlias = "PostImage";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostUpdateSRUpdateDynNotes"/> class.
        /// </summary>
        public PostUpdateSRUpdateDynNotes()
           : base(typeof(PostUpdateSRUpdateDynNotes))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "incident", new Action<LocalPluginContext>(this.ExecutePostUpdateSRUpdateDynNotes)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        ///   ExecutePostUpdateSRUpdateDynNotes
        /// </summary>
        /// <param name="localContext"></param>
        protected void ExecutePostUpdateSRUpdateDynNotes(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService trace = localContext.TracingService;
            var service = localContext.OrganizationService;
            var postImageIncident = (context.PostEntityImages != null && context.PostEntityImages.Contains(this.postImageAlias)) ? context.PostEntityImages[this.postImageAlias] : null;

            if (context.Depth <= 1)
            {
                if (service != null && postImageIncident != null)
                {
                    try
                    {
                        int caseoriginCode = ((OptionSetValue)postImageIncident["caseorigincode"]).Value;
                        int createdFrom = ((OptionSetValue)postImageIncident["smp_createdfrom"]).Value;
                        bool portalsubmit = ((bool)postImageIncident["smp_portalsubmit"]);
                        if (caseoriginCode != 3 || (caseoriginCode == 3 && createdFrom == 3 && portalsubmit == false))
                        {
                            this.FetchAllProblemTypeDescriptionAndAssign(postImageIncident, service, trace);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        ///   FetchAllProblemTypeDescriptionAndAssign
        /// </summary>
        /// <param name="image"></param>
        /// <param name="service"></param>
        /// <param name="trace"></param>
        private void FetchAllProblemTypeDescriptionAndAssign(Entity image, IOrganizationService service, ITracingService trace)
        {
            try
            {
                int createdfrom = ((OptionSetValue)image["smp_createdfrom"]).Value;
                string referenceSR = image.Attributes.Contains("smp_referencesr") ? ((EntityReference)image["smp_referencesr"]).Id.ToString() : string.Empty;
                if (createdfrom == 3 && referenceSR == string.Empty)
                {
                    ConditionExpression condServiceRequest = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ServiceRequestIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { image.Id }
                    };
                    QueryExpression dynamicProblemNotesQuery = new QueryExpression
                    {
                        EntityName = CRMAttributesResource.ServiceRequestProblemTypeEntity,
                        ColumnSet = new ColumnSet(CRMAttributesResource.ProblemTypeDescriptionIdAttribute),
                        Criteria =
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions = { condServiceRequest }
                                }
                    };
                    EntityCollection dpncollection = service.RetrieveMultiple(dynamicProblemNotesQuery);
                    foreach (Entity dpnEntity in dpncollection.Entities)
                    {
                        SetStateRequest setStateRequest = new SetStateRequest()
                        {
                            EntityMoniker = new EntityReference
                            {
                                Id = dpnEntity.Id,
                                LogicalName = CRMAttributesResource.ServiceRequestProblemTypeEntity,
                            },
                            State = new OptionSetValue(1),
                            Status = new OptionSetValue(2)
                        };
                        service.Execute(setStateRequest);
                    }
                }

                if (image.Attributes.Contains(CRMAttributesResource.ProblemBuildingIdAttribute) && image.Attributes.Contains(CRMAttributesResource.ProblemClassIdAttribute) && image.Attributes.Contains(CRMAttributesResource.ProblemTypeIdAttribute))
                {
                    ConditionExpression condBuilding = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.BuildingIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)image.Attributes[CRMAttributesResource.ProblemBuildingIdAttribute]).Id }
                    };
                    ConditionExpression condProblemClass = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemClassIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)image.Attributes[CRMAttributesResource.ProblemClassIdAttribute]).Id }
                    };
                    ConditionExpression condProblemType = new ConditionExpression
                    {
                        AttributeName = "smp_problemtype",
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)image.Attributes[CRMAttributesResource.ProblemTypeIdAttribute]).Id }
                    };
                    ConditionExpression condActive = new ConditionExpression
                    {
                        AttributeName = "statecode",
                        Operator = ConditionOperator.Equal,
                        Values = { "Active" }
                    };
                    QueryExpression problemTypeDescQuery = new QueryExpression
                    {
                        EntityName = CRMAttributesResource.ProblemTypeDescriptionEntityName,
                        ColumnSet = new ColumnSet(CRMAttributesResource.ProblemTypeDescriptionIdAttribute, CRMAttributesResource.ProblemTypeDescriptionNameAttribute),
                        Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = { condBuilding, condProblemClass, condProblemType, condActive }
                        }
                    };

                    if (service != null)
                    {
                        EntityCollection collection = service.RetrieveMultiple(problemTypeDescQuery);
                        foreach (Entity problemTypeDesc in collection.Entities)
                        {
                            if (problemTypeDesc.Attributes.Contains(CRMAttributesResource.ProblemTypeDescriptionIdAttribute))
                            {
                                Entity serviceRequestDynamicProblemType = new Entity(CRMAttributesResource.ServiceRequestProblemTypeEntity);
                                serviceRequestDynamicProblemType[CRMAttributesResource.ServiceRequestIdAttribute] = new EntityReference(CRMAttributesResource.IncidentEntity, image.Id);
                                serviceRequestDynamicProblemType[CRMAttributesResource.ProblemTypeDescriptionIdAttribute] = new EntityReference(CRMAttributesResource.ProblemTypeDescriptionEntityName, new Guid(problemTypeDesc.Attributes[CRMAttributesResource.ProblemTypeDescriptionIdAttribute].ToString()));
                                serviceRequestDynamicProblemType["new_value"] = problemTypeDesc[CRMAttributesResource.ProblemTypeDescriptionNameAttribute].ToString();
                                service.Create(serviceRequestDynamicProblemType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}