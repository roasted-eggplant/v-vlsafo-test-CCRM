// --------------------------------------------------------------------------------------------------------------------
// <copyright file= "CreateInspectionResponse.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  CreateInspectionResponse Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class CreateInspectionResponse : Plugin
    {
        public CreateInspectionResponse() : base(typeof(CreateInspectionResponse))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, "smp_inspection", new Action<LocalPluginContext>(this.CreateResponseRecords)));
        }

        protected void CreateResponseRecords(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                string fileName = string.Empty;
                string documentBody = string.Empty;
                Entity inspection = context.InputParameters["Target"] as Entity;
                Guid inspectionTypeId = inspection.GetAttributeValue<EntityReference>("smp_inspectiontype").Id;
                EntityCollection questionsColletion = this.GettheQuestions(service, inspectionTypeId);
                if (questionsColletion != null)
                {
                    tracingService.Trace("Question collection  : " + questionsColletion.Entities.Count);
                    this.CreateInspectionResponcerecords(questionsColletion, inspection, service, tracingService);
                }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private EntityCollection GettheQuestions(IOrganizationService service, Guid inspectiontypeid)
        {
            {
                EntityCollection inspectionQuestions;
                QueryExpression problemquery = new QueryExpression("smp_inspectionquestion");
                problemquery.ColumnSet = new ColumnSet("smp_questiontext", "smp_order");
                problemquery.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                problemquery.Criteria.AddCondition("smp_inspectiontypeid", ConditionOperator.Equal, inspectiontypeid);
                problemquery.Criteria.AddFilter(LogicalOperator.And);
                inspectionQuestions = service.RetrieveMultiple(problemquery);
                if (inspectionQuestions.Entities.Count == 0)
                {
                    return null;
                }

                return inspectionQuestions;
            }
        }

        private void CreateInspectionResponcerecords(EntityCollection questionsColletion, Entity inspection, IOrganizationService service, ITracingService tracingService)
        {
            foreach (Entity question in questionsColletion.Entities)
            {
                Entity newResponse = new Entity("smp_inspectionrespons");

                newResponse.Attributes["smp_inspectionquestion"] = question["smp_questiontext"];
                newResponse.Attributes["smp_inspectionid"] = new EntityReference("smp_inspection", inspection.Id);
                newResponse.Attributes["ownerid"] = new EntityReference(inspection.GetAttributeValue<EntityReference>("ownerid").LogicalName, inspection.GetAttributeValue<EntityReference>("ownerid").Id);                
                if (question.Attributes.Contains("smp_order"))
                {
                    newResponse.Attributes["smp_order"] = question["smp_order"];
                }

                service.Create(newResponse);
            }
        }
    }
}
