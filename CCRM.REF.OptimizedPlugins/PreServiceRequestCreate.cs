// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreServiceRequestCreate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Plugin Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Globalization;
    using System.Linq;
    using CCRM.REF.TelemetryLog;

    public class PreServiceRequestCreate : IPlugin
    {
        IRequestLogging requestLogging;
        public void Execute(IServiceProvider serviceProvider)
        {
            CCRM.REF.TelemetryLog.LocalPluginContext localPluginContext = new CCRM.REF.TelemetryLog.LocalPluginContext(serviceProvider);
            IConfigurationRetrieval configurationRetrieval = new ConfigurationRetrieval();
            this.requestLogging = new RequestLogging(configurationRetrieval, localPluginContext);
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context != null && context.InputParameters["Target"] != null)
            {
                var serRequest = (Entity)context.InputParameters["Target"];

                if (service != null && serRequest != null)
                {
                    bool statusCode = serRequest.Attributes.Contains(CRMAttributesResource.StatusCodeAttribute) ? true : false;
                    int caseorigincode = ((OptionSetValue)serRequest["caseorigincode"]).Value;
                    try
                    {
                        if (statusCode)
                        {
                            bool smp_portalsubmit = (bool)serRequest["smp_portalsubmit"];
                            if ((caseorigincode == (int)ServiceRequestEnum.ServiceRequestOrigin.Web || caseorigincode == (int)ServiceRequestEnum.ServiceRequestOrigin.IOT || caseorigincode == (int)ServiceRequestEnum.ServiceRequestOrigin.Phone) && ((OptionSetValue)serRequest.Attributes[CRMAttributesResource.StatusCodeAttribute]).Value == (int)ServiceRequestEnum.StatusCode.Draft)
                            {
                                object smp_contact = this.GetFieldFromTargetOrImage(serRequest, "smp_contact");
                                if (smp_contact != null)
                                {
                                    object smp_requestorid = this.GetFieldFromTargetOrImage(serRequest, "smp_requestorid");
                                    if (smp_requestorid != null)
                                    {
                                        Entity requestor = this.GetRequestorDetails(service, ((EntityReference)smp_requestorid).Id, trace);
                                        if (requestor != null)
                                        {
                                            if (!serRequest.Attributes.Contains("smp_requestorphone"))
                                            {
                                                if (requestor.Attributes.Contains("telephone1"))
                                                {
                                                    serRequest["smp_requestorphone"] = requestor["telephone1"].ToString();
                                                }
                                                else if (caseorigincode == (int)ServiceRequestEnum.ServiceRequestOrigin.Web)
                                                {
                                                    serRequest["smp_requestorphone"] = "<None>";
                                                }
                                            }

                                            serRequest["smp_requestoralias"] = this.GetFieldFromTargetOrImage(requestor, "smp_alias");
                                            serRequest["smp_requestoremail"] = this.GetFieldFromTargetOrImage(requestor, "emailaddress1");
                                            serRequest["smp_buildingid"] = this.GetFieldFromTargetOrImage(requestor, "smp_buildingid");
                                            serRequest["new_requestorroomnumber"] = this.GetFieldFromTargetOrImage(requestor, "new_requestorroomnumber");
                                        }
                                    }

                                    EntityReference otherRef = this.GetRoomWithOther(service, trace);
                                    EntityReference problemRef = serRequest.Attributes.ContainsKey("new_problemroomnumber") ?
                                        (EntityReference)serRequest["new_problemroomnumber"] : null;
                                    if (problemRef != null)
                                    {
                                        problemRef.Name = this.GetRoomName(service, problemRef.Id);
                                    }

                                    if (problemRef == null)
                                    {
                                        serRequest["new_problemroomnumber"] = otherRef;
                                    }
                                    else if (problemRef.Id != otherRef.Id)
                                    {
                                        serRequest["smp_problemroom"] = problemRef.Name;
                                    }
                                    else
                                    {
                                        trace.Trace("Problem room lookup is Other, doing nothing");
                                    }

                                    if (serRequest.Attributes.Contains("smp_problembuilding"))
                                    {
                                        Entity building = this.GetBuildingDetails(service, ((EntityReference)serRequest["smp_problembuilding"]).Id, trace);
                                        if (building != null)
                                        {
                                            serRequest["smp_problembuildingaddressline1"] = this.GetFieldFromTargetOrImage(building, "smp_addressline1");
                                            serRequest["smp_problembuildingaddressline2"] = this.GetFieldFromTargetOrImage(building, "smp_addressline2");
                                            serRequest["smp_problembuildingstate"] = this.GetFieldFromTargetOrImage(building, "smp_state");
                                            serRequest["smp_problembuildingcountry"] = this.GetFieldFromTargetOrImage(building, "smp_country");
                                            serRequest["smp_problembuildingcity"] = this.GetFieldFromTargetOrImage(building, "smp_city");
                                            serRequest["smp_problembuildingzipcode"] = this.GetFieldFromTargetOrImage(building, "smp_zipcode");
                                            serRequest["smp_problembuildingtimezone"] = this.GetFieldFromTargetOrImage(building, "smp_timezoneid");
                                        }
                                    }

                                    if (smp_portalsubmit == true)
                                    {
                                        serRequest["statuscode"] = new OptionSetValue(2);
                                        this.AssignProviderToServiceRequest(service, serRequest, trace);
                                    }
                                    else
                                    {
                                        //// Code to set the Default Provider When SR is Saved as Draft from Portal.
                                        this.AssignProviderToServiceRequest(service, serRequest, trace);
                                    }

                                    ServiceRequestHelper.SetIntegrationStatusandProblemOccuredDateTime(serRequest, false);
                                }
                            }
                            else
                            {
                                trace.Trace("in CRM");
                                string roomName = string.Empty;
                                EntityReference problemRef = serRequest.Attributes.ContainsKey("new_problemroomnumber") ?
                                        (EntityReference)serRequest["new_problemroomnumber"] : null;
                                if (problemRef != null)
                                {
                                    roomName = this.GetRoomName(service, problemRef.Id);
                                }
                                else
                                {
                                    serRequest["new_problemroomnumber"] = this.GetRoomWithOther(service, trace);
                                    serRequest["smp_problemroom"] = serRequest.Attributes.Contains("smp_problemroom") ? serRequest["smp_problemroom"].ToString() : string.Empty;
                                }

                                if (roomName != "Other" && !string.IsNullOrEmpty(roomName))
                                {
                                    serRequest["smp_problemroom"] = roomName;
                                }

                                this.AssignProviderToServiceRequest(service, serRequest, trace);
                                serRequest["smp_integrationstatus"] = false;
                            }
                        }

                        ////this.requestLogging.LogPluginTrace(serRequest, MappingConstants.ServiceRequestCreatedSequenceId, MappingConstants.ServiceRequestCreatedSuccessEventId, MappingConstants.ServiceRequestCreatedEventName, MappingConstants.ServiceRequestCreatedSuccessEventMessage);
                    }
                    catch (CustomServiceManagementPortalException ex)
                    {
                        ////this.requestLogging.LogPluginException(serRequest, ex, MappingConstants.ServiceRequestCreatedSequenceId, MappingConstants.ServiceRequestCreatedFailedEventId, MappingConstants.ServiceRequestCreatedEventName, MappingConstants.ServiceRequestCreatedFailedEventMessage);
                        ////Kill the exception so it does not effect other plugin execution
                    }
                }
            }
        }

        public Entity GetBuildingDetails(IOrganizationService service, Guid buildingId, ITracingService trace)
        {
            Entity retunBuilding = null;
            try
            {
                trace.Trace("in Retrieve Building Details");
                return retunBuilding = service.Retrieve("smp_building", buildingId, new ColumnSet("smp_addressline1", "smp_addressline2", "smp_state", "smp_country", "smp_city", "smp_zipcode", "smp_timezoneid"));
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /*Code Added by sharatveer G on 02-12-2017*/
        public Entity GetRequestorDetails(IOrganizationService service, Guid requestorId, ITracingService trace)
        {
            Entity returnRequestor = null;
            try
            {
                trace.Trace("in Retrieve Requestor Details");
                return returnRequestor = service.Retrieve("contact", requestorId, new ColumnSet("emailaddress1", "smp_alias", "telephone1", "smp_buildingid", "smp_roomnumber"));
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        public object GetFieldFromTargetOrImage(Entity target, string fieldName)
        {
            object rtnValue = null;
            if (target.Contains(fieldName))
            {
                rtnValue = target[fieldName];
            }

            return rtnValue;
        }

        private string GetRoomName(IOrganizationService service, Guid roomId)
        {
            string name = string.Empty;
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='smp_room'>
                                                            <attribute name='smp_roomid' />
                                                            <attribute name='smp_name' />
                                                            <attribute name='createdon' />
                                                            <order attribute='smp_name' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='smp_roomid' operator='eq' uitype='smp_room' value='" + roomId + @"' />
                                                            </filter>
                                                          </entity>
                                                        </fetch>";
            EntityCollection roomCollection = service.RetrieveMultiple(new FetchExpression(query));
            if (roomCollection != null && roomCollection.Entities.Count > 0)
            {
                name = roomCollection.Entities[0]["smp_name"].ToString();
            }

            return name;
        }

        private void SetDefaultProvider(IOrganizationService service, Entity serviceRequest)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='account'>
                                                            <attribute name='name' />
                                                            <attribute name='accountid' />
                                                            <order attribute='name' descending='false' />
                                                            <order attribute='address1_stateorprovince' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='name' operator='eq' value='Not Assigned' />
                                                            </filter>
                                                            <link-entity name='contact' from='contactid' to='primarycontactid' visible='false' link-type='outer' alias='a_410707b195544cd984376608b1802904'>
                                                              <attribute name='emailaddress1' />
                                                            </link-entity>
                                                          </entity>
                                                        </fetch>";
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            if (result != null && result.Entities.Count > 0)
            {
                serviceRequest["customerid"] = new EntityReference("account", result.Entities[0].Id);
            }
        }

        private EntityReference GetRoomWithOther(IOrganizationService service, ITracingService trace)
        {
            trace.Trace("In GetRoomWithOther");
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='smp_room'>
                                                            <attribute name='smp_roomid' />
                                                            <attribute name='smp_name' />
                                                            <attribute name='createdon' />
                                                            <order attribute='smp_name' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='smp_name' operator='eq' value='Other' />
															  <condition attribute='statecode' operator='eq' value='0' />
															  <condition attribute='smp_building' operator='null' />
															</filter>
                                                          </entity>
                                                        </fetch>";
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            if (result != null && result.Entities != null && result.Entities.Count > 0)
            {
                trace.Trace($"Found {result.Entities.Count} matching Other rooms");
                EntityReference otherRoom = new EntityReference(result.Entities.First().LogicalName, result.Entities.First().Id);
                return otherRoom;
            }

            trace.Trace("Other room not found");
            return null;
        }

        /// <summary>
        /// Assigns the provider to service request.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="objectIncident">The object incident.</param>
        private void AssignProviderToServiceRequest(IOrganizationService service, Entity objectIncident, ITracingService trace)
        {
            try
            {
                ////Check if building, roomtype, floor, problem class and problem type is not null. To be added, floor
                if (objectIncident.Attributes.Contains(CRMAttributesResource.ProblemBuildingIdAttribute) && objectIncident.Attributes.Contains(CRMAttributesResource.ProblemClassIdAttribute) && objectIncident.Attributes.Contains(CRMAttributesResource.ProblemTypeIdAttribute))
                {
                    ConditionExpression condBuilding = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.BuildingIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)objectIncident.Attributes[CRMAttributesResource.ProblemBuildingIdAttribute]).Id }
                    };
                    ConditionExpression condProblemClass = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemClassIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)objectIncident.Attributes[CRMAttributesResource.ProblemClassIdAttribute]).Id }
                    };
                    ConditionExpression condProblemType = new ConditionExpression
                    {
                        AttributeName = CRMAttributesResource.ProblemTypeIdAttribute,
                        Operator = ConditionOperator.Equal,
                        Values = { ((EntityReference)objectIncident.Attributes[CRMAttributesResource.ProblemTypeIdAttribute]).Id }
                    };
                    ConditionExpression condStatus = new ConditionExpression
                    {
                        AttributeName = "statuscode",
                        Operator = ConditionOperator.Equal,
                        Values = { 1 }
                    };
                    QueryExpression providerMatrixQuery = new QueryExpression
                    {
                        EntityName = CRMAttributesResource.ProviderMatrixEntity,
                        ColumnSet = new ColumnSet(CRMAttributesResource.PrimaryProviderIdAttribute, CRMAttributesResource.ProviderMatrixIsApprovalRequired, CRMAttributesResource.ProviderMatrixIOCode),
                        Criteria =
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = { condBuilding, condProblemClass, condProblemType, condStatus }
                        }
                    };
                    if (service != null)
                    {
                        EntityCollection provider_Matrix = service.RetrieveMultiple(providerMatrixQuery);
                        ////Entity providerMatrix = service.RetrieveMultiple(providerMatrixQuery) == null ? null : service.RetrieveMultiple(providerMatrixQuery).Entities.FirstOrDefault();
                        Entity providerMatrix = provider_Matrix.Entities.FirstOrDefault();
                        if (providerMatrix != null)
                        {
                            if (providerMatrix.Attributes.Contains(CRMAttributesResource.PrimaryProviderIdAttribute))
                            {
                                bool approvalRequired = false;
                                string io_Code = string.Empty;
                                if (this.ProviderIsActive(((EntityReference)providerMatrix.Attributes[CRMAttributesResource.PrimaryProviderIdAttribute]).Id, service))
                                {
                                    if (providerMatrix.Attributes.Contains(CRMAttributesResource.ProviderMatrixIsApprovalRequired))
                                    {
                                        approvalRequired = Convert.ToBoolean(providerMatrix.Attributes[CRMAttributesResource.ProviderMatrixIsApprovalRequired], CultureInfo.CurrentCulture);
                                    }
                                    else
                                    {
                                        approvalRequired = false;
                                    }

                                    if (objectIncident.Attributes.Contains(CRMAttributesResource.IoCode) == false || string.IsNullOrEmpty(Convert.ToString(objectIncident.Attributes[CRMAttributesResource.IoCode], CultureInfo.InvariantCulture)))
                                    {
                                        if (providerMatrix.Attributes.Contains(CRMAttributesResource.ProviderMatrixIOCode))
                                        {
                                            io_Code = Convert.ToString(providerMatrix.Attributes[CRMAttributesResource.ProviderMatrixIOCode], CultureInfo.InvariantCulture);
                                        }
                                        else
                                        {
                                            io_Code = string.Empty;
                                        }
                                    }

                                    this.AssignProvider(((EntityReference)providerMatrix.Attributes[CRMAttributesResource.PrimaryProviderIdAttribute]).Id, service, objectIncident, approvalRequired, io_Code, trace);
                                }
                            }
                            else
                            {
                                trace.Trace("In Default Provider as No primary Provider matrix is Found");
                                this.SetDefaultProvider(service, objectIncident);
                            }
                        }
                        else
                        {
                            trace.Trace("In Default Provider as not Provider matrix is Found");
                            this.SetDefaultProvider(service, objectIncident);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomServiceManagementPortalException("Failed to assign provider with the service request on Pre Create.", ex);
            }
        }

        /// <summary>
        /// Assigns the provider.
        /// </summary>
        /// <param name="primaryProviderId">The primary provider id.</param>
        /// <param name="service">The service.</param>
        /// <param name="objectIncident">The object incident.</param>
        /// <param name="isApprovalRequired">if set to <c>true</c> [is approval required].</param>
        /// <param name="io_Code">The ioCode</param>
        private void AssignProvider(Guid primaryProviderId, IOrganizationService service, Entity objectIncident, bool isApprovalRequired, string io_Code, ITracingService trace)
        {
            this.SetProvider(primaryProviderId, service, objectIncident, isApprovalRequired, io_Code, trace);
        }

        private void SetProvider(Guid primaryProviderId, IOrganizationService service, Entity objectIncident, bool isApprovalRequired, string io_Code, ITracingService trace)
        {
            objectIncident[CRMAttributesResource.CustomerIdAttribute] = new EntityReference(CRMAttributesResource.AccountEntity, primaryProviderId);
            objectIncident[CRMAttributesResource.ServiceRequestIsApprovalRequired] = isApprovalRequired;
            if (!objectIncident.Attributes.Contains(CRMAttributesResource.ServiceRequestIOCode))
            {
                objectIncident[CRMAttributesResource.ServiceRequestIOCode] = io_Code;
            }

            if (isApprovalRequired == true)
            {
                objectIncident["statuscode"] = new OptionSetValue(3);
                Guid managerId = this.GetContactManager(service, objectIncident);
                if (managerId != Guid.Empty)
                {
                    objectIncident[CRMAttributesResource.ServiceRequestApprovalManager] = new EntityReference("contact", managerId);
                }
            }
        }

        /// <summary>
        /// Whether the provider is active
        /// </summary>
        /// <param name="providerId">The providerid</param>
        /// <param name="service">The service</param>
        /// <returns>Whether the provider is active</returns>
        private bool ProviderIsActive(Guid providerId, IOrganizationService service)
        {
            bool result = false;
            ConditionExpression condProvider = new ConditionExpression
            {
                AttributeName = "accountid",
                Operator = ConditionOperator.Equal,
                Values = { providerId }
            };

            ConditionExpression condProviderStatus = new ConditionExpression
            {
                AttributeName = "statuscode",
                Operator = ConditionOperator.Equal,
                Values = { 1 }
            };

            QueryExpression providerQuery = new QueryExpression
            {
                EntityName = CRMAttributesResource.AccountEntity,
                ColumnSet = new ColumnSet(CRMAttributesResource.AccountIdAttribute),
                Criteria =
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions = { condProvider, condProviderStatus }
                }
            };

            if (service != null)
            {
                Entity provider = service.RetrieveMultiple(providerQuery) == null ? null : service.RetrieveMultiple(providerQuery).Entities[0];
                if (provider != null && provider.Attributes.Contains(CRMAttributesResource.AccountIdAttribute))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the contact manager.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="sRequest">The obj incident.</param>
        /// <returns>contact manager</returns>
        private Guid GetContactManager(IOrganizationService service, Entity request)
        {
            Guid managerId = Guid.Empty;
            if (request.Attributes.Contains("smp_contact"))
            {
                if (service != null)
                {
                    Entity contact = service.Retrieve("contact", ((EntityReference)request["smp_contact"]).Id, new ColumnSet("smp_manager"));
                    if (contact != null)
                    {
                        if (contact.Attributes.Contains("smp_manager"))
                        {
                            managerId = ((EntityReference)contact["smp_manager"]).Id;
                        }
                    }
                }
            }

            return managerId;
        }
    }
}
