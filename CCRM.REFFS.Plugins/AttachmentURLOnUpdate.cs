// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachmentURLOnUpdate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AttachmentURLOnUpdate Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class AttachmentURLOnUpdate : Plugin
    {
        public AttachmentURLOnUpdate() : base(typeof(AttachmentURLOnUpdate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "smp_attachmentsurl", new Action<LocalPluginContext>(this.DeleteNotes)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void DeleteNotes(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    string fileName = string.Empty;
                    if (context.Depth > 1)
                    {
                        return;
                    }

                    Entity target = context.InputParameters["Target"] as Entity;
                    tracingService.Trace("Notes deleting" + target.GetAttributeValue<bool>("smp_isdelete"));
                    Entity attachmentUrl = service.Retrieve(target.LogicalName, target.Id, new ColumnSet("smp_source", "smp_notesid"));
                    if (attachmentUrl.Attributes.Contains("smp_source") && attachmentUrl.GetAttributeValue<OptionSetValue>("smp_source").Value == Constants.SourceAX && target.Attributes.Contains("smp_isdelete") && target.GetAttributeValue<bool>("smp_isdelete") == true)
                    {
                        Entity notesRecord = service.Retrieve("annotation", new Guid(attachmentUrl.GetAttributeValue<string>("smp_notesid")), new ColumnSet());
                        if (notesRecord != null)
                        {
                            service.Delete(notesRecord.LogicalName, notesRecord.Id);
                            tracingService.Trace("Notes Deleted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}