// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostServiceRequestsmp_Reclassify.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostServiceRequestsmp_Reclassify Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;

    /// <summary>
    /// PostServiceRequest Reclassify Plugin.
    /// </summary>    
    public class PostServiceRequestsmp_Reclassify : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostServiceRequestsmp_Reclassify"/> class.
        /// </summary>
        public PostServiceRequestsmp_Reclassify()
            : base(typeof(PostServiceRequestsmp_Reclassify))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "smp_Reclassify", "incident", new Action<LocalPluginContext>(this.ExecutePostServiceRequestsmp_Reclassify)));

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
        protected void ExecutePostServiceRequestsmp_Reclassify(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            // TODO: Implement your custom Plug-in business logic.
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService trace = localContext.TracingService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
            {
                EntityReference entity = (EntityReference)context.InputParameters["Target"];
                var service = localContext.OrganizationService;
                if (entity.LogicalName == "incident")
                {
                    Entity serviceRequest = ServiceRequestHelper.GetServiceRequestDetails(service, entity.Id);
                    string[] strAttributesServiceRequestToRemove = new string[] { "ticketnumber", "title", "statuscode", "customerid", "smp_referencesr", "incidentid", "createdon", "createdby", "modifiedon", "modifiedby", "smp_issurveyservicerequest", "smp_cancelledforreclassification", "smp_slamet", "smp_priorityid", "smp_submitteddatetime", "smp_problemoccureddatetime", "smp_duedate", "smp_tier1workcompletiondatebybuildingtimezone", "smp_completeddate", "smp_completeddatebybuildingtimezone", "smp_requestedduedate", "smp_createddatetimebybuildingtimezone", "smp_submitteddatetimebybuildingtimezone", "smp_occureddatetimebybuildingtimezone", "smp_duedatebybuildingtimezone", "smp_providerduedatebybuildingtimezone", "ownerid", "smp_duedatetimebybuildingtimezone", "smp_statusonhold", "smp_problemtypedescriptionid", "smp_answer" };
                    Entity entNewIncident = this.CloneRecordForEntity("incident", serviceRequest, strAttributesServiceRequestToRemove);
                    entNewIncident["customerid"] = new EntityReference(CRMAttributesResource.AccountEntity, this.GetProviderId(service, "Not Assigned"));
                    entNewIncident["smp_referencesr"] = new EntityReference("incident", entity.Id);
                    entNewIncident["smp_createdfrom"] = new OptionSetValue(3);
                    entNewIncident["smp_problemoccureddatetime"] = Convert.ToDateTime(DateTime.UtcNow, CultureInfo.CurrentCulture);
                    entNewIncident["smp_flag"] = false;
                    entNewIncident["smp_portalsubmit"] = false;
                    Guid cloneId = service.Create(entNewIncident);
                    EntityCollection dynamicProblemTypeNotes = GetServiceRequestDynamicProblemTypeNotes(service, entity.Id);
                    foreach (Entity dynamicProblemTypeEntity in dynamicProblemTypeNotes.Entities)
                    {
                        Entity newDynamicProblemTypeNotes = new Entity();
                        newDynamicProblemTypeNotes.LogicalName = "smp_servicerequestproblemtypedesc";
                        if (dynamicProblemTypeEntity.Attributes.Contains("smp_problemtypedescriptionid"))
                        {
                            newDynamicProblemTypeNotes["smp_problemtypedescriptionid"] = new EntityReference("smp_problemtypedescription", ((Microsoft.Xrm.Sdk.EntityReference)dynamicProblemTypeEntity.Attributes["smp_problemtypedescriptionid"]).Id);
                        }

                        if (dynamicProblemTypeEntity.Attributes.Contains("smp_answer"))
                        {
                            newDynamicProblemTypeNotes["smp_answer"] = dynamicProblemTypeEntity.Attributes["smp_answer"];
                        }

                        newDynamicProblemTypeNotes["smp_servicerequestid"] = new EntityReference("incident", cloneId);
                        service.Create(newDynamicProblemTypeNotes);
                    }
                    this.CreateNotesFromServiceRequest(service, entity.Id, cloneId);
                    this.UpdateReferenceSR(service, entity.Id, cloneId, trace);
                    EntityReference serviceRequestId = new EntityReference("incident", cloneId);
                    context.OutputParameters["CloneEntity"] = serviceRequestId;
                }
            }
        }

        /// <summary>
        /// Get Provider Id by Name
        /// </summary>
        /// <param name="service">the service object.</param>
        /// <param name="providerName">the provider name object.</param>
        /// <returns>gets provider id</returns>
        protected Guid GetProviderId(IOrganizationService service, string providerName)
        {
            Guid providerId = Guid.Empty;
            EntityCollection entityCollection = GetQueryResponse(service, "account", new string[] { "accountid" }, "name", providerName);

            if (entityCollection != null && entityCollection.Entities.Count > 0)
            {
                Entity entity = entityCollection.Entities[0];
                if (entity.Attributes.Contains("accountid"))
                {
                    providerId = entity.Id;
                }
            }

            return providerId;
        }

        /// <summary>
        /// Gets the query response.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <param name="fieldsToBeFetched">The fields to be fetched.</param>
        /// <param name="criteriaField">The criteria field.</param>
        /// <param name="criteriaValue">The criteria value.</param>
        /// <returns>Returns the Entity Collection.</returns>
        private static EntityCollection GetQueryResponse(IOrganizationService service, string entityLogicalName, string[] fieldsToBeFetched, string criteriaField, object criteriaValue)
        {
            if (service != null)
            {
                QueryExpression query = new QueryExpression();
                query.EntityName = entityLogicalName;
                query.ColumnSet = new ColumnSet(fieldsToBeFetched);
                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                filter.AddCondition(new ConditionExpression(criteriaField, ConditionOperator.Equal, criteriaValue));
                query.Criteria = filter;
                EntityCollection entityCollection = service.RetrieveMultiple(query);
                return entityCollection;
            }

            return null;
        }

        /// <summary>
        /// Gets the query response.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="serviceRequestId">The Service Request Id.</param>
        /// <returns>Returns the Entity Collection.</returns>
        private static EntityCollection GetServiceRequestDynamicProblemTypeNotes(IOrganizationService service, Guid serviceRequestId)
        {
            try
            {
                if (service != null)
                {
                    EntityCollection entities = GetQueryResponse(service, "smp_servicerequestproblemtypedesc", new string[] { "smp_problemtypedescriptionid", "smp_answer" }, "smp_servicerequestid", serviceRequestId);
                    return entities;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Get Notes
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="serviceRequestId">The Service Request Id</param>
        /// <param name="cloneId">The Clone Id</param>
        private void CreateNotesFromServiceRequest(IOrganizationService service, Guid serviceRequestId, Guid cloneId)
        {
            string strFetchNotes = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                              <entity name='annotation'>
                                                <attribute name='subject' />
                                                <attribute name='notetext' />
                                                <attribute name='documentbody' /> 
                                                <attribute name='filename' />
                                                <attribute name='annotationid' />
                                                <attribute name='ownerid' />
                                                <order attribute='subject' descending='false' />
                                                <filter type='and'>
                                                <condition attribute='subject' operator='ne' value='Completion Notes' />
                                                </filter>
                                                <link-entity name='incident' from='incidentid' to='objectid' alias='aa'>
                                                  <filter type='and'>
                                                    <condition attribute='incidentid' operator='eq' value='{0}' />
                                                  </filter>
                                                </link-entity>
                                              </entity>
                                            </fetch>";
            strFetchNotes = string.Format(strFetchNotes, serviceRequestId);
            EntityCollection entNotes = (EntityCollection)service.RetrieveMultiple(new FetchExpression(strFetchNotes));
            if (entNotes != null && entNotes.Entities.Count > 0)
            {
                foreach (Entity entNote in entNotes.Entities)
                {
                    string[] strAttributesNotestoRemove = new string[] { "createdon", "createdby", "modifiedon", "modifiedby", "annotationid", "objecttypecode", "objectid" };
                    Entity entNewAnnotation = this.CloneRecordForEntity("annotation", entNote, strAttributesNotestoRemove);
                    entNewAnnotation["objectid"] = new EntityReference("incident", cloneId);
                    entNewAnnotation["objecttypecode"] = "incident";
                    service.Create(entNewAnnotation);
                }
            }
        }

        /// <summary>
        /// Get the newly created Service Request
        /// </summary>
        /// <param name="targetEntityName">the target entity.</param>
        /// <param name="sourceEntity">the source entity.</param>
        /// <param name="strAttributestoRemove">the attributes to remove.</param>
        /// <returns>gets provider id</returns>
        private Entity CloneRecordForEntity(string targetEntityName, Entity sourceEntity, string[] strAttributestoRemove)
        {
            Entity clonedEntity = new Entity(targetEntityName);
            AttributeCollection attributeKeys = sourceEntity.Attributes;

            foreach (string key in attributeKeys.Keys)
            {
                //// Check if key is not there in the list of removed keys
                if (Array.IndexOf(strAttributestoRemove, key) == -1)
                {
                    clonedEntity[key] = sourceEntity[key];
                }
            }

            return clonedEntity;
        }

        private void UpdateReferenceSR(IOrganizationService service, Guid referenceSR, Guid reclassifiedSR, ITracingService trace)
        {
            try
            {
                if (service != null)
                {
                    //// Update Request Cancelled Reason
                    Entity serviceRequest = new Entity();
                    serviceRequest.LogicalName = "incident";
                    serviceRequest.Id = referenceSR;
                    serviceRequest["smp_reclassifiedsr"] = new EntityReference("incident", reclassifiedSR);
                    trace.Trace("reclassified SR : " + reclassifiedSR);
                    serviceRequest["smp_requestcancelledreason"] = "Reclassification";
                    serviceRequest["smp_cancelledforreclassification"] = true;
                    trace.Trace("after updateding smp_cancelledforreclassification =  true ");
                    service.Update(serviceRequest);
                    trace.Trace("after update sr call");
                    //// Cancel Service Request
                    Microsoft.Crm.Sdk.Messages.SetStateRequest state = new Microsoft.Crm.Sdk.Messages.SetStateRequest();
                    //// Set the Request Object's Properties to cancelled
                    state.State = new OptionSetValue(2); //// cancelled
                    state.Status = new OptionSetValue(6); //// cancelled
                    //// case whose state is being changed
                    state.EntityMoniker = new EntityReference("incident", referenceSR);
                    //// Execute the Request
                    service.Execute(state);
                    ////this.IntegartionForCancelledServiceRequest(service, referenceSR,trace);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
