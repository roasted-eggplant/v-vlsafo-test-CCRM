using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ServiceModel;

namespace CCRM.REFFS.Workflows
{
    public enum StatusCode
    {
        Active = 1,
        Finished = 2,
        Aborted = 3
    }

    public class GetActiveBPFForIotAlert : CodeActivity
    {
        [Input("Iot Alert Reference")]
        [ReferenceTarget("msdyn_iotalert")]
        public InArgument<EntityReference> IotAlertId { get; set; }

        [Output("Active Business Process FlowId")]
        [ReferenceTarget("msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b")]
        public OutArgument<EntityReference> ActiveBusinessProcessFlowId { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                var iotAlertId = IotAlertId.Get<EntityReference>(executionContext);
                tracer.Trace("Gettting Iot Alert Id from execution context " + DateTime.Now.ToString());
                tracer.Trace("Retrieve Active Business Process Flow ID using Iot Alert Id " + DateTime.Now.ToString());
                var activeBPF = GetActiveBusinessProcessFlowRelatedToIotAlert(service, iotAlertId);
                tracer.Trace("Retrieved Active Business Process Flow ID  " + DateTime.Now.ToString());
                ActiveBusinessProcessFlowId.Set(executionContext, activeBPF != null ? activeBPF.ToEntityReference() : null);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracer.Trace("GetActiveBusinessProcessFlow Code Activity : {0}", e.ToString());
                throw new InvalidPluginExecutionException("An error occurred in GetActiveBusinessProcessFlow Code Activity." + e.Message);
            }
            catch (Exception ex)
            {
                tracer.Trace("GetActiveBusinessProcessFlow Code Activity : {0}", ex.ToString());
                throw;
            }
        }

        private Entity GetActiveBusinessProcessFlowRelatedToIotAlert(IOrganizationService service, EntityReference iotAlert)
        {
            var queryExpression = new QueryExpression
            {
                ColumnSet = new ColumnSet("businessprocessflowinstanceid", "bpf_name"),
                EntityName = "msdyn_bpf_477c16f59170487b8b4dc895c5dcd09b",
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("bpf_msdyn_iotalertid", ConditionOperator.Equal,iotAlert.Id),
                        new ConditionExpression("statuscode", ConditionOperator.Equal,(int)StatusCode.Active)
                    }
                }
            };
           
            var entityCollection = service.RetrieveMultiple(queryExpression);
            if (entityCollection.Entities.Count > 0)
            {
                return entityCollection.Entities[0];
            }

            return null;
        }
    }
}