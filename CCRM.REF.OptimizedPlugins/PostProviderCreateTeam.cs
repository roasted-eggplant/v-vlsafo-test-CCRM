// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostProviderCreateTeam.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PostProviderCreateTeam Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins
{
    using CCRM.REF.OptimizedPlugins.Helpers;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Linq;

    /// <summary>
    /// PostProviderCreateTeam Plugin.
    /// </summary>    
    public class PostProviderCreateTeam : Plugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostProviderCreateTeam"/> class.
        /// </summary>
        public PostProviderCreateTeam()
            : base(typeof(PostProviderCreateTeam))
        {
            RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Create, Constants.AccountEntityName, new Action<LocalPluginContext>(this.ExecutePostProviderCreateTeam)));

            // Note : you can register for more events here if this plugin is not specific to an individual entity and message combination.
            // You may also need to update your RegisterFile.crmregister plug-in registration file to reflect any change.
        }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected void ExecutePostProviderCreateTeam(LocalPluginContext localContext)
        {
            string accountName = string.Empty;
            Guid accountId = Guid.Empty;
            if (localContext == null)
            {
                return;
            }

            // TODO: Implement your custom Plug-in business logic.
            IPluginExecutionContext context = localContext.PluginExecutionContext;

            if (context.InputParameters.Contains(Constants.Target) && context.InputParameters[Constants.Target] is Entity)
            {
                Entity entity = (Entity)context.InputParameters[Constants.Target];
                var service = localContext.OrganizationService;
                if (context.Depth <= 1)
                {
                    if (entity.LogicalName == Constants.AccountEntityName)
                    {
                        if (entity.Attributes.Contains(Constants.AccountTypeSchemaName) && ((OptionSetValue)entity.Attributes[Constants.AccountTypeSchemaName]).Value == 180620000)
                        {
                            return;
                        }

                        accountId = entity.Id;
                        accountName = entity.Attributes[Constants.Name].ToString();
                        try
                        {
                            this.UpdateAccount(service, accountId, context, this.CreateTeam(service, context, accountName, context.BusinessUnitId, accountId));
                        }
                        catch (CustomServiceManagementPortalException ex)
                        {
                            Logger.Write(ex, ExceptionType.SettingTeamToProviderFailed, service, Constants.AccountEntityName, Constants.ProviderSchemaName, accountId, ex.Message);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Creates the team.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="context">The context.</param>
        /// <param name="accountName">The account name.</param>
        /// <param name="businessUnitId">The business unit id.</param>
        /// <param name="accountId">The account id.</param>
        /// <returns>Team Id.</returns>
        private Guid CreateTeam(IOrganizationService service, IExecutionContext context, string accountName, Guid businessUnitId, Guid accountId)
        {
            Guid teamId = Guid.Empty;
            try
            {
                QueryExpression teamQuery = new QueryExpression()
                {
                    EntityName = Constants.Team,
                    ColumnSet = new ColumnSet(Constants.Name),
                    Criteria =
                    {
                        Conditions =
                                {
                                    new ConditionExpression(Constants.Name, ConditionOperator.Equal, accountName + "'s Team"),
                                }
                    }
                };

                if (service != null)
                {
                    Entity teamEntity = service.RetrieveMultiple(teamQuery).Entities.FirstOrDefault();
                    if (teamEntity == null)
                    {
                        Entity team = new Entity();
                        team.LogicalName = Constants.Team;
                        team.Attributes.Add(Constants.Name, accountName + "'s Team");
                        team.Attributes.Add(Constants.BusinessunitIdSchemaName, new EntityReference(Constants.BusinessunitSchemaName, businessUnitId));
                        if (context.Depth <= 1)
                        {
                            if (service != null)
                            {
                                teamId = service.Create(team);
                                this.AssignSecurityRole(service, teamId, accountId);
                            }
                        }
                    }
                    else
                    {
                        teamId = (Guid)teamEntity.Attributes["teamid"];
                    }
                }

                return teamId;
            }
            catch (Exception ex)
            {
                this.PopulateExceptionLog(ex, service, accountId);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(Constants.TeamErrorMsg, ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Updates the account.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="context">The context.</param>
        /// <param name="teamId">The team id.</param>
        private void UpdateAccount(IOrganizationService service, Guid accountId, IExecutionContext context, Guid teamId)
        {
            try
            {
                Entity account = new Entity(Constants.AccountEntityName, accountId);
                account.Attributes.Add(Constants.ProviderTeamSchemaName, new EntityReference(Constants.Team, teamId));
                if (context.Depth <= 1)
                {
                    if (service != null)
                    {
                        service.Update(account);
                    }
                }
            }
            catch (Exception ex)
            {
                this.PopulateExceptionLog(ex, service, accountId);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(Constants.ProviderAssigningErrorMsg, ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Assign Security Role.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="teamId">The team id.</param>
        /// <param name="accountId">The account id.</param>
        private void AssignSecurityRole(IOrganizationService service, Guid teamId, Guid accountId)
        {
            try
            {
                QueryExpression query = new QueryExpression
                {
                    EntityName = Constants.RoleEntityName,
                    ColumnSet = new ColumnSet(Constants.RoleIdSchemaName),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = Constants.Name,
                                Operator = ConditionOperator.Equal,
                                Values = { Constants.SecurityRoleProvider }
                            }
                        }
                    }
                };

                if (service != null)
                {
                    Entity role = service.RetrieveMultiple(query).Entities.FirstOrDefault();
                    if (role != null)
                    {
                        Guid roleId = new Guid(role.Attributes[Constants.RoleIdSchemaName].ToString());
                        service.Associate(Constants.Team, teamId, new Relationship(Constants.TeamRolesRelationshipName), new EntityReferenceCollection() { new EntityReference(Constants.RoleEntityName, roleId) });
                    }
                }
            }
            catch (Exception ex)
            {
                this.PopulateExceptionLog(ex, service, accountId);
                CustomServiceManagementPortalException customEx = new CustomServiceManagementPortalException(Constants.ProviderSecurityRoleAssigningErrorMsg, ex);
                throw customEx;
            }
        }

        /// <summary>
        /// Populates the exception log.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="service">The service.</param>
        /// <param name="accountId">The account id.</param>
        private void PopulateExceptionLog(Exception ex, IOrganizationService service, Guid accountId)
        {
            try
            {
                Logger.Write(ex, ExceptionType.SettingTeamToProviderFailed, service, Constants.AccountEntityName, Constants.ProviderSchemaName, accountId, string.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
