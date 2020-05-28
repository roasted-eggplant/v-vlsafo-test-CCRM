using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.ServiceModel;
using CCRM.REFFS.Workflows.Helper;
using CCRM.REF.TelemetryLog;

namespace CCRM.REFFS.Workflows
{
    public class GetContactByUser : CodeActivity
    {
        IRequestLogging requestLogging;
        [Input("Current UserId")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> CurrentUserId { get; set; }

        [Output("Contact Related To User")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> ContactRelatedToUser { get; set; }
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
                tracer.Trace("Gettting Current User Id from execution context " + DateTime.Now.ToString());
                var currentUserId = CurrentUserId.Get<EntityReference>(executionContext);
                ////requestLogging.LogWorkflowTrace(MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveCurrentUserIdSuccessEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveCurrentUserIdSuccessEventMessage);

                tracer.Trace("Retrieving Internal Email Address of a User " + DateTime.Now.ToString());
                var emailAddress = WorkflowHelper.GetPrimaryEmailAddressOfUser(service, currentUserId.Id);
                tracer.Trace("Retrived Internal Email Address of a User " + DateTime.Now.ToString());
                ////requestLogging.LogWorkflowTrace(MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveInternalEmailSuccessEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveInternalEmailSuccessEventMessage);

                tracer.Trace("Retrieving Contact By Primary Email Address of a Contact " + DateTime.Now.ToString());
                var contact = WorkflowHelper.GetContactByEmailAddress(service, emailAddress);
                tracer.Trace("Retrived Contact By Primary Email Address of a Contact " + DateTime.Now.ToString());
                ////requestLogging.LogWorkflowTrace(MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveContactByEmailSuccessEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveContactByEmailSuccessEventMessage);

                if (contact != null)
                {
                    ContactRelatedToUser.Set(executionContext, contact?.ToEntityReference());
                }
                else
                {
                    tracer.Trace("Retrieving Default Contact from Configuration " + DateTime.Now.ToString());
                    string ESBDefaultEmailAddress = WorkflowHelper.GetConfigurationValue(service, "ESBDefaultContact");
                    tracer.Trace("Retrived Default Contact from Configuration " + DateTime.Now.ToString());

                    tracer.Trace("Retrieving Default Contact By Primary Email Address of a Contact " + DateTime.Now.ToString());
                    var defaultContact = WorkflowHelper.GetContactByEmailAddress(service, ESBDefaultEmailAddress);
                    tracer.Trace("Retrived Default Contact By Primary Email Address of a Contact " + DateTime.Now.ToString());
                    ////requestLogging.LogWorkflowTrace(MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveDefaultContactByEmailSuccessEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveDefaultContactByEmailSuccessEventMessage);
                    ContactRelatedToUser.Set(executionContext, defaultContact?.ToEntityReference());
                }
                ////requestLogging.LogWorkflowTrace(MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveContactEmailAddressSuccessEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveContactEmailAddressSuccessEventMessage);
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                ////requestLogging.LogWorkflowException(e, MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveContactEmailAddressFailedEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveContactEmailAddressFailedEventMessage);
                tracer.Trace("GetContactByUser Code Activity : {0}", e.ToString());
                throw new InvalidPluginExecutionException("An error occurred in GetContactByUser Code Activity." + e.Message);
            }
            catch (Exception ex)
            {
                ////requestLogging.LogWorkflowException(ex, MappingConstants.RetrieveContactEmailAddressSequenceId, MappingConstants.RetrieveContactEmailAddressFailedEventId, MappingConstants.RetrieveContactEmailAddressEventName, MappingConstants.RetrieveContactEmailAddressFailedEventMessage);
                tracer.Trace("GetContactByUser Code Activity : {0}", ex.ToString());
                throw;
            }
        }

    }
}