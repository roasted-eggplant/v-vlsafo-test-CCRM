// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostCreateSRCreateDynNotes.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostCreateSRCreateDynNotes WorkFlow
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Activities;

    public sealed class PostCreateSRCreateDynNotes : CodeActivity
    {
        [ReferenceTarget("incident")]
        [Input("ServiceRequest")]
        public InArgument<EntityReference> ServiceRequest { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ITracingService trace = context.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            EntityReference serviceRequest = this.ServiceRequest.Get<EntityReference>(context);
            Entity entity = service.Retrieve(serviceRequest.LogicalName, serviceRequest.Id, new ColumnSet("caseorigincode", "smp_createdfrom", CRMAttributesResource.ProblemBuildingIdAttribute, CRMAttributesResource.ProblemClassIdAttribute, CRMAttributesResource.ProblemTypeIdAttribute, "smp_originalservicerequestid", "statecode"));
            if (entity != null)
            {
                try
                {
                    int caseoriginCode = ((OptionSetValue)entity["caseorigincode"]).Value;
                    int createdFrom = ((OptionSetValue)entity["smp_createdfrom"]).Value;
                    if (caseoriginCode != 3 && createdFrom != 3 && entity.GetAttributeValue<OptionSetValue>("statecode").Value != 2)
                    {
                        this.FetchAllProblemTypeDescriptionAndAssign(entity, service, trace);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidWorkflowException("Error from PostCreateSRCreateDynNotes :" + ex.Message);
                }
            }
        }

        private void FetchAllProblemTypeDescriptionAndAssign(Entity image, IOrganizationService service, ITracingService trace)
        {
            try
            {
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
                throw ex;
            }
        }

        /// <summary>
        /// Get the newly created Service Request
        /// </summary>
        /// <param name="targetEntityName">the target entity.</param>
        /// <param name="sourceEntity">the source entity.</param>
        /// <param name="strAttributestoRemove">the attributes to remove.</param>
        /// <returns>gets provider id</returns>
        ////private Entity CloneRecordForEntity(string targetEntityName, Entity sourceEntity, string[] strAttributestoRemove)
        ////{
        ////    Entity clonedEntity = new Entity(targetEntityName);
        ////    AttributeCollection attributeKeys = sourceEntity.Attributes;
        ////    foreach (string key in attributeKeys.Keys)
        ////    {
        ////        //// Check if key is not there in the list of removed keys
        ////        if (Array.IndexOf(strAttributestoRemove, key) == -1)
        ////        {
        ////            clonedEntity[key] = sourceEntity[key];
        ////        }
        ////    }

        ////    return clonedEntity;
        ////}
    }
}
