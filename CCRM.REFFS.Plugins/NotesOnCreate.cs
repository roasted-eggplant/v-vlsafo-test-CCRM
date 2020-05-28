// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotesOnCreate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  NotesOnCreate Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NotesOnCreate : Plugin
    {
        public NotesOnCreate() : base(typeof(NotesOnCreate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "annotation", new Action<LocalPluginContext>(this.OnNotesCreation)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void OnNotesCreation(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    string fileName = string.Empty;
                    tracingService.Trace("Depth" + context.Depth);
                    Entity target = context.InputParameters["Target"] as Entity;
                    EntityReference objectRecord = (EntityReference)target.Attributes["objectid"];
                    if (target.Attributes.Contains("documentbody") && target.Attributes["documentbody"] != null)
                    {
                        Dictionary<string, string> list = AttachmentHelper.RetrieveNotesAttachmentEntities(service);
                        if (list.ContainsValue(objectRecord.LogicalName))
                        {
                            tracingService.Trace("Creating Attachment URL");
                            Entity attachementsUrl = new Entity("smp_attachmentsurl");
                            attachementsUrl.Attributes["smp_source"] = new OptionSetValue(Constants.SourceCRM);
                            attachementsUrl.Attributes["smp_notesid"] = target.Id.ToString();
                            if (target.Attributes.Contains("mimetype") && target.Attributes["mimetype"] != null)
                            {
                                attachementsUrl.Attributes["smp_mimetype"] = target.GetAttributeValue<string>("mimetype");
                            }

                            string bloburl = this.GetBlobUrlFromConfiguration(service);
                            if (target.Attributes.Contains("filename") && target.Attributes["filename"] != null)
                            {
                                fileName = Convert.ToString(target.GetAttributeValue<string>("filename"));
                                fileName = this.ReplaceSpecialCharecters(fileName);
                            }

                            bloburl = bloburl + target.Id.ToString() + "_" + fileName;

                            string sasToken = this.GetSASTokenFromConfiguration(service);
                            bloburl += sasToken;

                            attachementsUrl.Attributes["smp_bloburl"] = bloburl;
                            if (objectRecord.Id != null)
                            {
                                attachementsUrl.Attributes["smp_objectid"] = objectRecord.Id.ToString();
                            }

                            attachementsUrl.Attributes["smp_name"] = fileName;

                            if (list.ContainsValue(objectRecord.LogicalName))
                            {
                                string myKey = list.FirstOrDefault(x => x.Value == objectRecord.LogicalName).Key;
                                attachementsUrl.Attributes["smp_objecttypecode"] = myKey;
                            }

                            attachementsUrl.Attributes["smp_tier1provider"] = this.GetTierOneProvider(service, objectRecord);
                            service.Create(attachementsUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetBlobUrlFromConfiguration(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = "smp_configuration";
            query.ColumnSet = new ColumnSet("smp_value");
            query.Criteria.AddCondition("smp_title", ConditionOperator.Equal, "AttachmentBlobUrl");
            EntityCollection entityCollection = service.RetrieveMultiple(query);
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection.Entities[0].GetAttributeValue<string>("smp_value");
            }
            else
            {
                return null;
            }
        }

        public string GetSASTokenFromConfiguration(IOrganizationService service)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = "smp_configuration";
            query.ColumnSet = new ColumnSet("smp_value");
            query.Criteria.AddCondition("smp_title", ConditionOperator.Equal, "SASToken");
            EntityCollection entityCollection = service.RetrieveMultiple(query);
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection.Entities[0].GetAttributeValue<string>("smp_value");
            }
            else
            {
                return null;
            }
        }

        public string GetTierOneProvider(IOrganizationService service, EntityReference fromentity)
        {
            string tieroneprovider = string.Empty;
            if (fromentity.LogicalName == "msdyn_purchaseorderproduct")
            {
                Entity purchaseorderproduct = service.Retrieve(fromentity.LogicalName, fromentity.Id, new ColumnSet("msdyn_purchaseorder"));
                if (purchaseorderproduct != null)
                {
                    EntityReference purchaseorder = purchaseorderproduct.GetAttributeValue<EntityReference>("msdyn_purchaseorder");
                    Entity resultentity = service.Retrieve(purchaseorder.LogicalName, purchaseorder.Id, new ColumnSet("smp_tier1provider"));

                    if (resultentity != null)
                    {
                        tieroneprovider = resultentity.GetAttributeValue<string>("smp_tier1provider");
                    }
                }
            }
            else if (fromentity.LogicalName == "product")
            {
                Entity resultentity = service.Retrieve(fromentity.LogicalName, fromentity.Id, new ColumnSet("smp_tier1provider"));

                if (resultentity != null && resultentity.GetAttributeValue<EntityReference>("smp_tier1provider") != null)
                {
                    tieroneprovider = resultentity.GetAttributeValue<EntityReference>("smp_tier1provider").Name;
                }
            }
            else
            {
                Entity resultentity = service.Retrieve(fromentity.LogicalName, fromentity.Id, new ColumnSet("smp_tier1provider"));

                if (resultentity != null)
                {
                    tieroneprovider = resultentity.GetAttributeValue<string>("smp_tier1provider");
                }
            }

            return tieroneprovider;
        }

        private string ReplaceSpecialCharecters(string filename)
        {
            if (filename.Contains("%"))
            {
                filename = filename.Replace("%", "%25");
            }

            if (filename.Contains("#"))
            {
                filename = filename.Replace("#", "%23");
            }

            return filename;
        }
    }
}
