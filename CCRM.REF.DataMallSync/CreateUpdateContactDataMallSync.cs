using System;
using CCRM.REF.DataMallSync.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CCRM.REF.DataMallSync
{
    public class CreateUpdateContactDataMallSync : Plugin
    {
        public CreateUpdateContactDataMallSync() : base(typeof(CreateUpdateContactDataMallSync))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, Constants.Contact, new Action<LocalPluginContext>(this.UpdateContactDetails)));
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Update, Constants.Contact, new Action<LocalPluginContext>(this.UpdateContactDetails)));
        }

        protected void UpdateContactDetails(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService service = localContext.OrganizationService;

            tracingService.Trace("Depth : " + context.Depth.ToString());

            if (context != null && context.InputParameters["Target"] != null)
            {
                if (context.Depth == 1)
                {
                    try
                    {
                        var contactRequest = (Entity)context.InputParameters["Target"];
                        Entity contact = new Entity(contactRequest.LogicalName, contactRequest.Id);
                        tracingService.Trace("UpdateContactDetails");
                        if (context.MessageName == "Create")
                        {
                            tracingService.Trace("Message Name : " + context.MessageName);
                            string entityName = "smp_company";
                            EntityReference companyReference = GetReferenceEntity(service, entityName, "smp_name", "Microsoft", tracingService);
                            if (companyReference != null)
                            {
                                contact.Attributes["smp_companyid"] = companyReference;
                                tracingService.Trace("Company Id mapped");
                            }
                        }

                        if (service != null && contactRequest != null)
                        {
                            EntityReference buildingReference = null;
                            tracingService.Trace("Building Number mapping..");
                            if (contactRequest.Contains("smp_buildingnumber") && contactRequest.GetAttributeValue<string>("smp_buildingnumber") != null)
                            {

                                string feedStoreId = contactRequest.GetAttributeValue<string>("smp_buildingnumber").Trim();
                                if (feedStoreId.Length > 0)
                                {
                                    string entityName = "smp_building";
                                    buildingReference = GetReferenceEntity(service, entityName, "smp_feedstoreid", feedStoreId, tracingService);
                                    if (buildingReference != null)
                                    {
                                        contact.Attributes["smp_buildingid"] = buildingReference;
                                        tracingService.Trace("Building Id mapped");
                                    }
                                }
                                else
                                {
                                    contact.Attributes["smp_buildingid"] = null;
                                }
                            }
                            else
                            {
                                if (context.PostEntityImages.Contains("PostImage"))
                                {
                                    Entity postImage = context.PostEntityImages["PostImage"] as Entity;
                                    if (postImage.Attributes.Contains("smp_buildingid"))
                                    {
                                        buildingReference = postImage.GetAttributeValue<EntityReference>("smp_buildingid");
                                        tracingService.Trace("Building Id mapped");
                                    }
                                }
                            }

                            if (contactRequest.Contains("smp_roomno") && contactRequest.GetAttributeValue<string>("smp_roomno") != null && buildingReference != null)
                            {
                                tracingService.Trace("Room Number mapping..");
                                string roomName = contactRequest.GetAttributeValue<string>("smp_roomno").Trim();
                                if (roomName.Length > 0)
                                {
                                    EntityReference roomReference = GetRoomReferenceEntity(service, roomName, buildingReference.Id, tracingService); ////GetReferenceEntity(service, entityName, "smp_feedstoreid", feedStoreId, tracingService);
                                    if (roomReference != null)
                                    {
                                        contact.Attributes["smp_roomnumber"] = roomReference;
                                        tracingService.Trace("Room Number mapped");
                                    }
                                    else
                                    {
                                        contact.Attributes["smp_roomnumber"] = null;
                                    }
                                }
                                else
                                {
                                    contact.Attributes["smp_roomnumber"] = null;
                                }
                            }

                            if (contactRequest.Contains("smp_mgrpersonalnumber") && contactRequest.GetAttributeValue<string>("smp_mgrpersonalnumber") != null)
                            {
                                tracingService.Trace("Personal Number mapping..");
                                string personalNumber = contactRequest.GetAttributeValue<string>("smp_mgrpersonalnumber").Trim();
                                if (personalNumber.Length > 0)
                                {
                                    string entityName = "contact";
                                    EntityReference managerReference = GetReferenceEntity(service, entityName, "smp_personalnumber", personalNumber, tracingService);
                                    if (managerReference != null)
                                    {
                                        contact.Attributes["smp_manager"] = managerReference;
                                        tracingService.Trace("Manager mapped");
                                    }
                                }
                                else
                                {
                                    contact.Attributes["smp_manager"] = null;
                                }
                            }
                            else
                            {
                                contact.Attributes["smp_manager"] = null;
                            }

                            if (contact.Attributes.Count > 0)
                            {
                                service.Update(contact);
                                tracingService.Trace("Contact record updated");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("Error Occured in this plugin. Exception : " + ex.Message);
                    }

                }
            }
        }

        private EntityReference GetReferenceEntity(IOrganizationService service, string entityName, string conditionAttribute, string conditionAttibuteValue, ITracingService trace)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='{0}'>
                                        <attribute name='{1}' />
                                        <filter type='and'>
                                          <condition attribute='{2}' operator='eq' value='{3}' />
                                          <condition attribute='statecode' operator='eq' value='0' />
                                        </filter>
                                      </entity>
                                    </fetch>";
            query = string.Format(query, entityName, entityName + "id", conditionAttribute, conditionAttibuteValue);
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            trace.Trace(entityName + " EntityCollection Count : " + result.Entities.Count.ToString());
            if (result != null && result.Entities.Count > 0)
            {
                return result.Entities[0].ToEntityReference();
            }

            return null;
        }

        private EntityReference GetRoomReferenceEntity(IOrganizationService service, string roomName, Guid buildingGuid, ITracingService trace)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='smp_room'>
                                        <attribute name='smp_roomid' />
                                        <filter type='and'>
                                            <condition attribute='statecode' operator='eq' value='0' />
                                            <condition attribute='smp_name' operator='eq' value='{0}' />
                                            <condition attribute='smp_building' operator='eq' value='{1}' />
                                        </filter>
                                      </entity>
                                    </fetch>";
            query = string.Format(query, roomName, buildingGuid);
            EntityCollection result = service.RetrieveMultiple(new FetchExpression(query));
            trace.Trace("Room records count : " + result.Entities.Count.ToString());
            if (result != null && result.Entities.Count > 0)
            {
                return result.Entities[0].ToEntityReference();
            }

            return null;
        }
    }
}
