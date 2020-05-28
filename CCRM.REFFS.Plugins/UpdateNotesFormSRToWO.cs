// --------------------------------------------------------------------------------------------------------------------
// <copyright file= "UpdateNotesFormSRToWO.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  UpdateNotesFormSRToWO Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class UpdateNotesFormSRToWO : Plugin
    {
        public UpdateNotesFormSRToWO() : base(typeof(NotesFromSRToWorkOrder))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, "annotation", new Action<LocalPluginContext>(this.UpdateNotesFromSRToWorkOrder)));
        }

        protected void UpdateNotesFromSRToWorkOrder(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService service = localContext.OrganizationService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                string fileName = string.Empty;
                string documentBody = string.Empty;
                Entity noteRecord = context.InputParameters["Target"] as Entity;

                EntityReference objectRecord = (EntityReference)noteRecord.Attributes["objectid"];
                if (objectRecord.LogicalName != "incident")
                {
                    return;
                }
                else
                {
                    Entity workOrder = this.GetworkorderDetails(service, objectRecord.Id, tracingService);
                    if (workOrder != null)
                    {
                        string caseURL = string.Empty;
                        Entity note = new Entity("annotation");
                        if (noteRecord.Attributes.Contains("filename") && noteRecord.Attributes["filename"] != null)
                        {
                            fileName = Convert.ToString(noteRecord.Attributes["filename"]);
                            tracingService.Trace("FileName :" + fileName);
                            if (fileName != null)
                            {
                                AttributeHelper.MapStringValue("filename", noteRecord, "filename", ref note);
                            }
                        }

                        if (noteRecord.Attributes.Contains("documentbody") && noteRecord.Attributes["documentbody"] != null)
                        {
                            AttributeHelper.MapStringValue("documentbody", noteRecord, "documentbody", ref note);
                        }

                        if (noteRecord.Attributes.Contains("notetext") && noteRecord.Attributes["notetext"] != null)
                        {
                            note.Attributes["notetext"] = noteRecord.Attributes["notetext"];
                        }

                        if (noteRecord.Attributes.Contains("subject") && noteRecord.Attributes["subject"] != null)
                        {
                            AttributeHelper.MapStringValue("subject", noteRecord, "subject", ref note);
                        }

                        note.Attributes["objectid"] = new EntityReference(workOrder.LogicalName, workOrder.Id);
                        tracingService.Trace("Creating new Note");
                        service.Create(note);
                    }
                }
            }
        }

        protected Entity GetworkorderDetails(IOrganizationService service, Guid incidentid, ITracingService trace)
        {
            try
            {
                if (service != null)
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "msdyn_workorder";
                    query.ColumnSet = new ColumnSet(true);
                    FilterExpression filter = new FilterExpression(LogicalOperator.And);
                    filter.AddCondition(new ConditionExpression("msdyn_servicerequest", ConditionOperator.Equal, incidentid));
                    query.Criteria = filter;
                    EntityCollection entityCollection = service.RetrieveMultiple(query);
                    if (entityCollection.Entities.Count == 1)
                    {
                        return entityCollection.Entities[0];
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace("Error in Service request " + ex.Message);
            }

            return null;
        }
    }
}
