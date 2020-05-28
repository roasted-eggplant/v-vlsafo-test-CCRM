// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentURLUpdateOnNoteDelete.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AttachmentURLUpdateOnNoteDelete Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class AttachmentURLUpdateOnNoteDelete : Plugin
    {
        public AttachmentURLUpdateOnNoteDelete() : base(typeof(AttachmentURLUpdateOnNoteDelete))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Delete", "annotation", new Action<LocalPluginContext>(this.UpdateAttachmentURL)));
        }

        public void UpdateAttachmentURL(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    string fileName = string.Empty;
                    EntityReference target = context.InputParameters["Target"] as EntityReference;
                    tracingService.Trace("Note deleted");
                    Entity attachmentUrl = this.GetAttachmentUrl(service, target.Id.ToString());
                    tracingService.Trace("Update the Attchment url " + attachmentUrl);
                    if (attachmentUrl != null)
                    {
                        try
                        {
                            tracingService.Trace("Update the Attchment url " + attachmentUrl.Id);
                            Entity attachemnturlEntity = new Entity(attachmentUrl.LogicalName);
                            attachemnturlEntity.Id = attachmentUrl.Id;
                            attachemnturlEntity.Attributes.Add("smp_isdelete", true);
                            service.Update(attachemnturlEntity);
                        }
                        catch (Exception ex)
                        {
                            tracingService.Trace("Update failed" + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Entity GetAttachmentUrl(IOrganizationService service, string noteId)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = "smp_attachmentsurl";
            query.ColumnSet = new ColumnSet("smp_name");
            query.Criteria.AddCondition("smp_notesid", ConditionOperator.Equal, noteId);
            EntityCollection entityCollection = service.RetrieveMultiple(query);
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection.Entities[0];
            }
            else
            {
                return null;
            }
        }
    }
}