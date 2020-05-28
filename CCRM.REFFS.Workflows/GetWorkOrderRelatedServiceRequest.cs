using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.ServiceModel;
using CCRM.REF.TelemetryLog;

namespace CCRM.REFFS.Workflows
{
    public enum StatusCode_WO
    {
        Active = 1,
        Inactive = 2,
    }
    public class GetWorkOrderRelatedServiceRequest : CodeActivity
    {
        IRequestLogging requestLogging;
        [Input("Service Request Reference")]
        [ReferenceTarget("incident")]
        public InArgument<EntityReference> ServiceRequestId { get; set; }
        [Output("Work Order")]
        [ReferenceTarget("msdyn_workorder")]
        public OutArgument<EntityReference> WorkOrderId { get; set; }

        [Output("IsExists")]
        public OutArgument<bool> IsExists { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            LocalWorkflowContext localWorkflowContext = new LocalWorkflowContext(executionContext);
            IConfigurationRetrieval configurationRetrieval = new ConfigurationRetrieval();
            requestLogging = new RequestLogging(configurationRetrieval, localWorkflowContext);
            ITracingService tracer = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                tracer.Trace("Gettting Service Request Id from execution context " + DateTime.Now.ToString());
                var serviceRequestId = ServiceRequestId.Get<EntityReference>(executionContext);
                tracer.Trace("Retrieve Active Work Order ID using Service Request Id " + DateTime.Now.ToString());
                var workOrder = GetWorkOrderByServiceActivity(service, serviceRequestId);
                tracer.Trace("Retrieved Active Work Order ID  " + DateTime.Now.ToString());
                if (workOrder != null)
                {
                    WorkOrderId.Set(executionContext, workOrder.ToEntityReference());
                    IsExists.Set(executionContext, true);
                }
                else
                {
                    WorkOrderId.Set(executionContext, null);
                    IsExists.Set(executionContext, false);
                }
                ////requestLogging.LogWorkflowTrace(MappingConstants.GetWORelatedToSRSequenceId, MappingConstants.GetWORelatedToSRSuccessEventId, MappingConstants.GetWORelatedToSREventName, MappingConstants.GetWORelatedToSRSuccessEventMessage);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                ////requestLogging.LogWorkflowException(e, MappingConstants.GetWORelatedToSRSequenceId, MappingConstants.GetWORelatedToSRFailedEventId, MappingConstants.GetWORelatedToSREventName, MappingConstants.GetWORelatedToSRFailedEventMessage);
                tracer.Trace("GetWorkOrderRelatedServiceRequest Code Activity : {0}", e.ToString());
                throw new InvalidPluginExecutionException("An error occurred in GetWorkOrderRelatedServiceRequest Code Activity." + e.Message);
            }
            catch (Exception ex)
            {
                ////requestLogging.LogWorkflowException(ex, MappingConstants.GetWORelatedToSRSequenceId, MappingConstants.GetWORelatedToSRFailedEventId, MappingConstants.GetWORelatedToSREventName, MappingConstants.GetWORelatedToSRFailedEventMessage);
                tracer.Trace("GetWorkOrderRelatedServiceRequest Code Activity : {0}", ex.ToString());
                throw;
            }
        }
        public Entity GetWorkOrderByServiceActivity(IOrganizationService service, EntityReference serviceRequest)
        {
            var queryExpression = new QueryExpression
            {
                ColumnSet = new ColumnSet("msdyn_workorderid"),
                EntityName = "msdyn_workorder",
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("msdyn_servicerequest", ConditionOperator.Equal,serviceRequest.Id),
                        new ConditionExpression("statuscode", ConditionOperator.Equal,(int)StatusCode_WO.Active)
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
