// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailTeam.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  EmailTeam
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Workflows
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;

    public class EmailTeam : CodeActivity
    {
        [RequiredArgument]
        [Input("Email To Send")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> EmailToSend { get; set; }

        [RequiredArgument]
        [Input("Recipient Team")]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> RecipientTeam { get; set; }

        [RequiredArgument]
        [Default("false")]
        [Input("Send Email?")]
        public InArgument<bool> SendEmail { get; set; }

        [Output("Users Added")]
        public OutArgument<int> UsersAdded { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                // Obtain the execution context service from the service provider.
                IWorkflowContext workflowExecutionContext = context.GetExtension<IWorkflowContext>();

                // Obtain the tracing service from the service provider.
                ITracingService tracingService = context.GetExtension<ITracingService>();

                // Obtain the Organization Service factory service from the service provider
                IOrganizationServiceFactory factory = context.GetExtension<IOrganizationServiceFactory>();

                // Use the factory to generate the Organization Service.
                IOrganizationService service = factory.CreateOrganizationService(workflowExecutionContext.UserId);

                EntityReference emailToSend = this.EmailToSend.Get(context);
                EntityReference recipientTeam = this.RecipientTeam.Get(context);
                bool sendEmail = this.SendEmail.Get(context);

                List<Entity> bccList = new List<Entity>();

                Entity email = RetrieveEmail(service, emailToSend.Id);

                if (email == null)
                {
                    this.UsersAdded.Set(context, 0);
                    return;
                }

                ////Add any pre-defined recipients specified to the array               
                foreach (Entity activityParty in email.GetAttributeValue<EntityCollection>("to").Entities)
                {
                    bccList.Add(activityParty);
                }

                EntityCollection teamMembers = GetMembersList(service, recipientTeam.Id);

                Entity teamrecord = service.Retrieve(recipientTeam.LogicalName, recipientTeam.Id, new ColumnSet("smp_emailserviceaccount"));

                bccList = ProcessUsers(service, teamMembers, bccList);                
                Entity teamRecord = service.Retrieve(recipientTeam.LogicalName, recipientTeam.Id, new ColumnSet("smp_emailserviceaccount"));
                if (teamRecord.GetAttributeValue<EntityReference>("smp_emailserviceaccount") != null)
                {
                    ////Update the email
                    Entity toactivityParty = new Entity("activityparty")
                    {
                        ["partyid"] = new EntityReference("systemuser", teamRecord.GetAttributeValue<EntityReference>("smp_emailserviceaccount").Id)
                    };
                    List<Entity> to1List = new List<Entity>();
                    to1List.Add(toactivityParty);
                    email["from"] = to1List.ToArray();
                    email["to"] = to1List.ToArray();
                    email["bcc"] = bccList.ToArray();
                    service.Update(email);

                    ////Send
                    if (sendEmail)
                    {
                        SendEmailRequest request = new SendEmailRequest
                        {
                            EmailId = emailToSend.Id,
                            TrackingToken = string.Empty,
                            IssueSend = true
                        };

                        service.Execute(request);
                    }

                    this.UsersAdded.Set(context, bccList.Count);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static Entity RetrieveEmail(IOrganizationService service, Guid emailId)
        {
            return service.Retrieve("email", emailId, new ColumnSet("to", "bcc"));
        }

        private static List<Entity> ProcessUsers(IOrganizationService service, EntityCollection teamMembers, List<Entity> bccList)
        {
            foreach (Entity e in teamMembers.Entities)
            {
                Entity user = service.Retrieve("systemuser", e.GetAttributeValue<EntityReference>("smp_user").Id, new ColumnSet("internalemailaddress", "isdisabled"));

                if (string.IsNullOrEmpty(user.GetAttributeValue<string>("internalemailaddress")))
                {
                    continue;
                }

                if (user.GetAttributeValue<bool>("isdisabled"))
                {
                    continue;
                }

                Entity activityParty = new Entity("activityparty")
                {
                    ["partyid"] = new EntityReference("systemuser", e.GetAttributeValue<EntityReference>("smp_user").Id)
                };

                if (bccList.Any(t => t.GetAttributeValue<EntityReference>("partyid").Id == e.GetAttributeValue<EntityReference>("smp_user").Id))
                {
                    continue;
                }

                bccList.Add(activityParty);
            }

            return bccList;
        }

        private static EntityCollection GetMembersList(IOrganizationService service, Guid teamId)
        {
            QueryExpression query = new QueryExpression("smp_woemailnotification");
            query.ColumnSet = new ColumnSet("smp_woemailnotificationid", "smp_user", "smp_name");
            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(new ConditionExpression("smp_team", ConditionOperator.Equal, teamId));
            filter.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            query.Criteria.AddFilter(filter);
            EntityCollection entityCollection = service.RetrieveMultiple(query);
            return entityCollection;
        }
    }
}
