// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotesFromSRToWorkOrder.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  NotesFromSRToWorkOrder
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class NotesFromSRToWorkOrder : Plugin
    {
        public NotesFromSRToWorkOrder() : base(typeof(NotesFromSRToWorkOrder))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, "msdyn_workorder", new Action<LocalPluginContext>(this.CopyNotesFromSRToWorkOrder)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request
        protected void CopyNotesFromSRToWorkOrder(LocalPluginContext localContext)
        {
            IPluginExecutionContext context = localContext.PluginExecutionContext;
            ITracingService tracingService = localContext.TracingService;
            IOrganizationService service = localContext.OrganizationService;

            tracingService.Trace("Create notes form SR to WO");
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                string fileName = string.Empty;
                string documentBody = string.Empty;
                try
                {
                    Entity workOrder = context.InputParameters["Target"] as Entity;
                    if (workOrder.Attributes.Contains("msdyn_servicerequest"))
                    {
                        EntityReference serviceRequestReference = workOrder.GetAttributeValue<EntityReference>("msdyn_servicerequest");
                        if (serviceRequestReference.Id != null)
                        {
                            QueryExpression query = new QueryExpression("annotation");
                            query.ColumnSet = new ColumnSet("filename", "documentbody", "notetext", "objecttypecode", "mimetype", "subject");
                            query.Criteria = new FilterExpression(LogicalOperator.And);
                            query.Criteria.AddCondition("objectid", ConditionOperator.Equal, serviceRequestReference.Id);
                            EntityCollection results = service.RetrieveMultiple(query);
                            tracingService.Trace(" Result count :" + results.Entities.Count);

                            if (results.Entities.Count > 0)
                            {
                                string caseURL = string.Empty;

                                foreach (Entity tempEnt in results.Entities)
                                {
                                    Entity note = new Entity("annotation");
                                    fileName = string.Empty;
                                    if (tempEnt.Attributes.Contains("filename") && tempEnt.Attributes["filename"] != null)
                                    {
                                        fileName = Convert.ToString(tempEnt.Attributes["filename"]);
                                        tracingService.Trace("FileName :" + fileName);
                                        if (fileName != null)
                                        {
                                            note.Attributes["filename"] = fileName;
                                        }
                                    }

                                    AttributeHelper.MapStringValue("documentbody", tempEnt, "documentbody", ref note);
                                    AttributeHelper.MapStringValue("subject", tempEnt, "subject", ref note);
                                    if (tempEnt.Attributes.Contains("notetext") && tempEnt.Attributes["notetext"] != null)
                                    {
                                        note.Attributes["notetext"] = tempEnt.Attributes["notetext"];

                                        AttributeHelper.MapStringValue("filename", tempEnt, "filename", ref note);
                                        AttributeHelper.MapStringValue("documentbody", tempEnt, "documentbody", ref note);
                                    }

                                    if (tempEnt.Attributes.Contains("mimetype") && tempEnt.Attributes["mimetype"] != null)
                                    {
                                        note.Attributes["mimetype"] = tempEnt.Attributes["mimetype"];
                                    }

                                    note.Attributes["objectid"] = new EntityReference(workOrder.LogicalName, workOrder.Id);
                                    tracingService.Trace("Creating new Note");
                                    service.Create(note);
                                    note = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("Error Occured in this plugin " + ex.Message);
                }
            }
        }
    }
}
