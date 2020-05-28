// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentURLOnCreation.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AttachmentURLOnCreation Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public class AttachmentURLOnCreation : Plugin
    {
        public AttachmentURLOnCreation() : base(typeof(AttachmentURLOnCreation))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Create", "smp_attachmentsurl", new Action<LocalPluginContext>(this.OnAttachmentURLCreation)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void OnAttachmentURLCreation(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService service = localContext.OrganizationService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                string fileName = string.Empty;
                Entity target = context.InputParameters["Target"] as Entity;

                if (target.Attributes.Contains("smp_source") && target.GetAttributeValue<OptionSetValue>("smp_source").Value == Constants.SourceAX)
                {
                    Entity note = new Entity("annotation");
                    if (target.Attributes.Contains("smp_name"))
                    {
                        fileName = Convert.ToString(target.Attributes["smp_name"]);
                        if (fileName != null)
                        {
                            note.Attributes["filename"] = fileName;
                        }
                    }

                    if (target.Attributes.Contains("smp_objectid") && target.GetAttributeValue<string>("smp_objectid") != string.Empty && target.GetAttributeValue<string>("smp_objectid") != "{00000000-0000-0000-0000-000000000000}" && target.GetAttributeValue<string>("smp_objectid") != "00000000-0000-0000-0000-000000000000")
                    {
                        string regardingid = target.GetAttributeValue<string>("smp_objectid");

                        if (regardingid.Contains("{"))
                        {
                            regardingid = regardingid.Replace("{", string.Empty).ToString().Replace("}", string.Empty);
                        }

                        if (target.Attributes.Contains("smp_objecttypecode") && target.GetAttributeValue<string>("smp_objecttypecode") != string.Empty)
                        {
                            string objecttypecode = target.GetAttributeValue<string>("smp_objecttypecode");
                            Entity regardingentity = null;

                            Dictionary<string, string> list = AttachmentHelper.RetrieveNotesAttachmentEntities(service);
                            if (list.ContainsKey(objecttypecode) && objecttypecode != Constants.ProductObjectCode)
                            {
                                regardingentity = this.GetReferenceEntity(service, list[objecttypecode], new Guid(regardingid));
                            }

                            if (regardingentity != null)
                            {
                                note.Attributes["objectid"] = new EntityReference(regardingentity.LogicalName, regardingentity.Id);
                            }
                        }
                    }

                    if (target.Attributes.Contains("smp_keyvalue") && target.GetAttributeValue<string>("smp_keyvalue") != string.Empty)
                    {
                        string objecttypecode = target.GetAttributeValue<string>("smp_objecttypecode");
                        Entity productRegardingEntity = null;
                        if (objecttypecode == Constants.ProductObjectCode)
                        {
                            QueryExpression query = new QueryExpression("product");
                            query.ColumnSet = new ColumnSet("productid");
                            query.Criteria.AddFilter(LogicalOperator.Or);
                            query.Criteria.AddCondition(new ConditionExpression("productnumber", ConditionOperator.Equal, target.GetAttributeValue<string>("smp_keyvalue")));
                            EntityCollection entityCollection = service.RetrieveMultiple(query);
                            if (entityCollection.Entities.Count > 0)
                            {
                                productRegardingEntity = entityCollection.Entities[0];
                            }

                            if (productRegardingEntity != null)
                            {
                                note.Attributes["objectid"] = new EntityReference(productRegardingEntity.LogicalName, productRegardingEntity.Id);
                            }
                        }
                    }

                    note.Attributes["mimetype"] = target.GetAttributeValue<string>("smp_mimetype");
                    if (target.Attributes.Contains("smp_bloburl") && target.GetAttributeValue<string>("smp_bloburl") != string.Empty)
                    {
                        var url = target.GetAttributeValue<string>("smp_bloburl");
                        if (target.GetAttributeValue<string>("smp_mimetype") == "text/plain")
                        {
                            string textFromFile = (new WebClient()).DownloadString(url);
                            byte[] filename = Encoding.ASCII.GetBytes(textFromFile);
                            note.Attributes["documentbody"] = Convert.ToBase64String(filename);
                        }

                        if (target.GetAttributeValue<string>("smp_mimetype") == "image/png" || target.GetAttributeValue<string>("smp_mimetype") == "application/pdf")
                        {
                            byte[] imageData = (new WebClient()).DownloadData(url);
                            note.Attributes["documentbody"] = Convert.ToBase64String(imageData);
                        }
                        else
                        {
                            byte[] imageData = (new WebClient()).DownloadData(url);
                            note.Attributes["documentbody"] = Convert.ToBase64String(imageData);
                        }
                    }

                    tracingService.Trace("Creating new Note");
                    Guid notesid = service.Create(note);
                    Entity attachemnturl = new Entity(target.LogicalName);
                    attachemnturl.Id = target.Id;
                    attachemnturl.Attributes.Add("smp_notesid", notesid.ToString());
                    service.Update(attachemnturl);
                }
            }
        }

        public Entity GetReferenceEntity(IOrganizationService service, string referenceentityname, Guid referenceentityid)
        {
            Entity referenceentity = service.Retrieve(referenceentityname, referenceentityid, new ColumnSet());
            return referenceentity;
        }
    }
}