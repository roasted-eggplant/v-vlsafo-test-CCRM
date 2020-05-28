// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreSRUpdatePriorityEscalationNotes.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PreSRUpdatePriorityEscalationNotes Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class PreSRUpdatePriorityEscalationNotes : Plugin
    {
        /// <summary>
        /// The target param
        /// </summary>
        private readonly string targetParam = "Target";

        public PreSRUpdatePriorityEscalationNotes()
            : base(typeof(PreSRUpdatePriorityEscalationNotes))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(20, Constants.Update, Constants.IncidentEntityaName, new Action<LocalPluginContext>(this.ExecutePriorityNotes)));
        }

        protected void ExecutePriorityNotes(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                return;
            }

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            if (context != null && context.InputParameters[this.targetParam] != null && context.Depth <= 1)
            {
                var service = localContext.OrganizationService;
                var trace = localContext.TracingService;
                var objIncident = (Entity)context.InputParameters[this.targetParam];
                if (service != null && objIncident != null)
                {
                    if (objIncident.Attributes.Contains(Constants.PriorityEscalationPopupNotesSchemaName))
                    {
                        //// check for Escalation Details from Popup Notes for this SR
                        QueryExpression query = new QueryExpression(Constants.AnnotationEntityName);
                        query.ColumnSet = new ColumnSet(new string[] { Constants.ObjectId, Constants.Subject });
                        query.Criteria.AddCondition(Constants.ObjectId, ConditionOperator.Equal, objIncident.Id);
                        query.Criteria.AddCondition(Constants.Subject, ConditionOperator.Equal, Constants.EscalationDetails);
                        EntityCollection result = service.RetrieveMultiple(query);
                        if (result != null && result.Entities.Count > 0)
                        {
                            trace.Trace(Constants.PopupMessage);
                            return;
                        }

                        Entity noteEntity = new Entity(Constants.AnnotationEntityName);
                        noteEntity.Attributes.Add(Constants.Subject, Constants.EscalationDetails);
                        noteEntity.Attributes.Add(Constants.PriorityNoteText, (string)objIncident.Attributes[Constants.PriorityEscalationPopupNotesSchemaName]);
                        noteEntity.Attributes.Add(Constants.ObjectId, new EntityReference(objIncident.LogicalName, objIncident.Id));
                        noteEntity.Attributes.Add(Constants.ObjectTypeCode, 112);
                        service.Create(noteEntity);
                    }
                }
            }
        }
    }
}
