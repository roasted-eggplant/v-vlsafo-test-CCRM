// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FSPostUpdateOfServiceRequestStatus.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  FSPostUpdateOfServiceRequestStatus Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class FSPostUpdateOfServiceRequestStatus : Plugin
    {
        public FSPostUpdateOfServiceRequestStatus() : base(typeof(FSPostUpdateOfServiceRequestStatus))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Update, "incident", new Action<LocalPluginContext>(this.FSServiceRequestStatusUpdate)));
        }

        ////Thid Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void FSServiceRequestStatusUpdate(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    tracingService.Trace("Post Update SR");
                    tracingService.Trace("Depth:" + context.Depth);
                    Entity target = context.InputParameters["Target"] as Entity;
                    Entity postImage = context.PostEntityImages["PostImage"] as Entity;
                    tracingService.Trace("Status:" + target.GetAttributeValue<OptionSetValue>("statuscode").Value);
                    if (target.Attributes.Contains("statuscode") && target.GetAttributeValue<OptionSetValue>("statuscode").Value == 180620002)
                    {
                        Entity servicerequest = service.Retrieve("incident", target.Id, new ColumnSet(true));
                        if (servicerequest.Attributes.Contains("smp_reclassifiedfromworkorder") && (bool)servicerequest.Attributes["smp_reclassifiedfromworkorder"] == false)
                        {
                            QueryExpression exp = new QueryExpression("msdyn_workorder");
                            exp.ColumnSet = new ColumnSet(true);
                            exp.Criteria.AddFilter(LogicalOperator.Or);
                            exp.Criteria.AddCondition("msdyn_servicerequest", ConditionOperator.Equal, target.Id);
                            EntityCollection existedWorkorders = service.RetrieveMultiple(exp);
                            tracingService.Trace("work orders:" + existedWorkorders.Entities.Count);
                            if (existedWorkorders.Entities.Count == 0)
                            {
                                tracingService.Trace("No existing work Orders");
                                if (postImage.Attributes.Contains("customerid") && postImage.GetAttributeValue<EntityReference>("customerid") != null)
                                {
                                    EntityReference providerid = postImage.GetAttributeValue<EntityReference>("customerid");
                                    Entity provider = service.Retrieve("account", providerid.Id, new ColumnSet(true));
                                    if (provider.Attributes.Contains("smp_workorderwithind365") && provider.GetAttributeValue<bool>("smp_workorderwithind365") == true)
                                    {
                                        this.CreateWorkorder(postImage, service, tracingService);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Entity GetPricelist(IOrganizationService service, ITracingService tracing, Entity serviceaccount)
        {
            try
            {
                if (service != null)
                {
                    QueryExpression query = new QueryExpression();
                    query.EntityName = "pricelevel";
                    query.ColumnSet = new ColumnSet("name");
                    if (serviceaccount.GetAttributeValue<EntityReference>("transactioncurrencyid") != null)
                    {
                        tracing.Trace("transactioncurrency existed");
                        query.Criteria.AddCondition(new ConditionExpression("transactioncurrencyid", ConditionOperator.Equal, serviceaccount.GetAttributeValue<EntityReference>("transactioncurrencyid").Id));
                    }
                    else
                    {
                        tracing.Trace("No transactioncurrency existed");
                        query.Criteria.AddCondition(new ConditionExpression("name", ConditionOperator.Equal, "US"));
                    }

                    EntityCollection entityCollection = service.RetrieveMultiple(query);
                    tracing.Trace("PriceList Collection" + entityCollection.Entities.Count);
                    if (entityCollection.Entities.Count > 0)
                    {
                        return entityCollection.Entities[0];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in price list " + ex.Message);
            }

            return null;
        }

        public void CreateWorkorder(Entity postImage, IOrganizationService service, ITracingService tracingService)
        {
            try
            {
                tracingService.Trace("Work Order Creating");

                Entity workOrder = new Entity("msdyn_workorder");
                if (postImage.Attributes.Contains("smp_problembuilding") && postImage.GetAttributeValue<EntityReference>("smp_problembuilding") != null)
                {
                    tracingService.Trace("mapping servicing account with Buliding Name");
                    EntityReference buildingref = postImage.GetAttributeValue<EntityReference>("smp_problembuilding");
                    Entity building = service.Retrieve(buildingref.LogicalName, buildingref.Id, new ColumnSet(true));

                    QueryExpression query = new QueryExpression("account");
                    query.ColumnSet = new ColumnSet("transactioncurrencyid");
                    query.Criteria.AddCondition(new ConditionExpression("accountnumber", ConditionOperator.Equal, building.GetAttributeValue<string>("smp_feedstoreid")));

                    EntityCollection entityCollection = service.RetrieveMultiple(query);

                    if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                    {
                        tracingService.Trace("buildings matched" + entityCollection.Entities);
                        workOrder.Attributes["msdyn_serviceaccount"] = new EntityReference("account", entityCollection.Entities[0].Id);
                        Entity priceList = this.GetPricelist(service, tracingService, entityCollection.Entities[0]);
                        if (priceList != null)
                        {
                            workOrder.Attributes["msdyn_pricelist"] = new EntityReference(priceList.LogicalName, priceList.Id);
                        }
                    }
                    else
                    {
                        tracingService.Trace("No Service Account existed with Same Building");
                        throw new Exception("There is No Account Existed with Building Name, So Work Order can not be Created");
                    }
                }

                workOrder.Attributes["msdyn_servicerequest"] = new EntityReference(postImage.LogicalName, postImage.Id);

                service.Create(workOrder);
                tracingService.Trace("Work Order Created");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
