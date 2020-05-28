// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostUpdateofNoteIdAttachmentURL.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostUpdateofNoteIdAttachmentURL Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.ServiceModel;

    public class PostUpdateofNoteIdAttachmentURL : Plugin
    {
        public PostUpdateofNoteIdAttachmentURL() : base(typeof(PostUpdateofNoteIdAttachmentURL))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "smp_attachmentsurl", new Action<LocalPluginContext>(this.OnUpdateofNoteID)));
        }

        public void OnUpdateofNoteID(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    string fileName = string.Empty;
                    Entity target = context.InputParameters["Target"] as Entity;

                    if (target.Attributes.Contains("smp_notesid"))
                    {
                        string noteid = target.GetAttributeValue<string>("smp_notesid");
                        QueryExpression query = new QueryExpression();
                        query.EntityName = "smp_attachmentsurl";
                        query.ColumnSet = new ColumnSet();
                        query.Criteria.AddCondition("smp_notesid", ConditionOperator.Equal, noteid);
                        query.Criteria.AddCondition("smp_source", ConditionOperator.Equal, 180620000);
                        EntityCollection entityCollection = service.RetrieveMultiple(query);
                        if (entityCollection.Entities.Count > 0)
                        {
                            foreach (var att in entityCollection.Entities)
                            {
                                service.Delete(att.LogicalName, att.Id);
                            }
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
