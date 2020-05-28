//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="EmailTeamTests.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////  EmailTeamTests Workflow in Test
//// </summary>
//// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Workflows.Test
{
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Fakes;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Workflow;
    using Microsoft.Xrm.Sdk.Workflow.Fakes;
    using System;
    using System.Activities;
    using System.Collections.Generic;

    [TestClass]
    public class EmailTeamTests
    {
        [TestMethod]
        public void EmailTeam_1_Team_1_User_1_Existing()
        {
            var workflowUserId = Guid.NewGuid();
            var workflowCorrelationId = Guid.NewGuid();
            var workflowInitiatingUserId = Guid.NewGuid();

            Guid emailID = new Guid("968843B2-717F-4771-8382-C02A0EF340BD");
            Entity email = new Entity("email");
            email.Id = emailID;
            email["activityid"] = emailID;

            Guid id2 = Guid.NewGuid();
            Entity activityParty = new Entity("activityparty");
            activityParty.Id = id2;
            activityParty["activitypartyid"] = id2;
            activityParty["activityid"] = new EntityReference("email", emailID);
            activityParty["partyid"] = new EntityReference("contact", Guid.NewGuid());
            activityParty["participationtypemask"] = new OptionSetValue(2);

            EntityCollection to = new EntityCollection();
            to.Entities.Add(activityParty);
            email["to"] = to;

            Entity teamrecord = new Entity("team")
            {
                Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0970"),
            };
            Entity systemUser1 = new Entity("systemuser");

            systemUser1.Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0870");
            systemUser1["internalemailaddress"] = "test1@test.com";
            systemUser1["isdisabled"] = false;

            Entity systemUser2 = new Entity("systemuser");

            systemUser2.Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0900");
            systemUser2["internalemailaddress"] = "test3@test.com";
            systemUser2["isdisabled"] = false;

            Entity teammembership = new Entity("teammembership");
            teammembership.Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0670");
            teammembership["teamid"] = teamrecord.Id;
            teammembership["systemuserid"] = systemUser1.Id;

            Entity teammembership2 = new Entity("teammembership");
            teammembership2.Id = new Guid("884A078c-0467-E711-80E5-3863BB4C0970");
            teammembership2["teamid"] = teamrecord.Id;
            teammembership2["systemuserid"] = systemUser2.Id;

            teamrecord.Attributes["smp_emailserviceaccount"] = new EntityReference(systemUser2.LogicalName, systemUser2.Id);

            var inputs = new Dictionary<string, object>
            {
                { "EmailToSend", email.ToEntityReference() },
                { "RecipientTeam", teamrecord.ToEntityReference() },
                { "SendEmail", false }
            };

            var service = new StubIOrganizationService();

            var factory = new StubIOrganizationServiceFactory();
            factory.CreateOrganizationServiceNullableOfGuid = t1 =>
            {
                return service;
            };
            var workflowContext = new StubIWorkflowContext();
            workflowContext.UserIdGet = () =>
            {
                return workflowUserId;
            };
            workflowContext.CorrelationIdGet = () =>
            {
                return workflowCorrelationId;
            };
            workflowContext.InitiatingUserIdGet = () =>
            {
                return workflowInitiatingUserId;
            };

            service.RetrieveStringGuidColumnSet = delegate (string entity, Guid guid, ColumnSet secondaryUserColumnSet)
            {
                if (entity == "email")
                {
                    return email;
                }

                if (entity == "systemuser")
                {
                    return systemUser2;
                }

                if (entity == "team")
                {
                    return teamrecord;
                }

                return null;
            };

            service.RetrieveMultipleQueryBase = (query) =>
            {
                EntityCollection collection = new EntityCollection();
                string entityName = string.Empty;
                if (query.GetType().Name.Equals("QueryExpression"))
                {
                    entityName = ((QueryExpression)query).EntityName;
                }
                else
                {
                    entityName = ((QueryByAttribute)query).EntityName;
                }

                if (entityName == "smp_woemailnotification")
                {
                    Entity woEmailNotification = new Entity("smp_woemailnotification")
                    {
                        Id = new Guid("884A078c-0467-E711-80E5-3863BB3C0960"),
                    };

                    woEmailNotification.Attributes["smp_team"] = new EntityReference(teamrecord.LogicalName, teamrecord.Id);
                    woEmailNotification.Attributes["smp_user"] = new EntityReference(systemUser2.LogicalName, systemUser2.Id);
                    collection.Entities.Add(woEmailNotification);
                }

                if (entityName == "team")
                {
                    collection.Entities.Add(teamrecord);
                }

                return collection;
            };

            EmailTeam target = new EmailTeam();
            using (ShimsContext.Create())
            {
                var invoker = new WorkflowInvoker(target);
                invoker.Extensions.Add<IWorkflowContext>(() => workflowContext);
                invoker.Extensions.Add<IOrganizationServiceFactory>(() => factory);
                const int Expected = 2;
                var outputs = invoker.Invoke(inputs);

                ////Assert
                Assert.AreEqual(Expected, outputs["UsersAdded"]);
            }
        }
    }
}