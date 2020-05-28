// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestOnUpdate.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  ServiceRequestOnUpdate Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.ServiceModel;

    public class ServiceRequestOnUpdate : Plugin
    {
        public ServiceRequestOnUpdate() : base(typeof(ServiceRequestOnUpdate))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, "Update", "incident", new Action<LocalPluginContext>(this.UpdateProblemTypeDescription)));
        }

        ////This Plugin will fire on Work order pre Create. it will map all the Required Fields from the Service Request.
        public void UpdateProblemTypeDescription(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                if (context.Depth > 2)
                {
                    return;
                }
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity target = context.InputParameters["Target"] as Entity;
                    if (target.Attributes.Contains("smp_requestorid") || target.Attributes.Contains("smp_requestoremail") || target.Attributes.Contains("smp_requestorphone") || target.Attributes.Contains("smp_contact") || target.Attributes.Contains("smp_contactphone") || target.Attributes.Contains("smp_contactemail") || target.Attributes.Contains("smp_requestedduedate") || target.Attributes.Contains("smp_problemtypedescription") || target.Attributes.Contains("smp_iocode") || target.Attributes.Contains("smp_costcentercode"))
                    {
                        QueryExpression exp = new QueryExpression("msdyn_workorder");
                        exp.ColumnSet = new ColumnSet("msdyn_workorderid");
                        exp.Criteria.AddFilter(LogicalOperator.Or);
                        exp.Criteria.AddCondition("msdyn_servicerequest", ConditionOperator.Equal, target.Id);
                        EntityCollection existedWorkorders = service.RetrieveMultiple(exp);
                        if (existedWorkorders.Entities != null && existedWorkorders.Entities.Count > 0)
                        {
                            foreach (Entity workorder in existedWorkorders.Entities)
                            {
                                if (target.Attributes.Contains("smp_problemtypedescription"))
                                {
                                    workorder.Attributes.Add("msdyn_workordersummary", target.GetAttributeValue<string>("smp_problemtypedescription"));
                                }

                                if (target.Attributes.Contains("smp_iocode"))
                                {
                                    workorder.Attributes.Add("smp_iocode", target.GetAttributeValue<string>("smp_iocode"));
                                }

                                if (target.Attributes.Contains("smp_costcentercode"))
                                {
                                    workorder.Attributes.Add("smp_costcentercode", target.GetAttributeValue<string>("smp_costcentercode"));
                                }

                                if (target.Attributes.Contains("smp_requestedduedate"))
                                {
                                    workorder.Attributes.Add("msdyn_datewindowend", target.GetAttributeValue<DateTime>("smp_requestedduedate"));
                                }

                                if (target.Attributes.Contains("smp_contactphone"))
                                {
                                    workorder.Attributes.Add("smp_contactphonenumber", target.GetAttributeValue<string>("smp_contactphone"));
                                }

                                if (target.Attributes.Contains("smp_contactemail"))
                                {
                                    workorder.Attributes.Add("smp_contactemail", target.GetAttributeValue<string>("smp_contactemail"));
                                }

                                if (target.Attributes.Contains("smp_contact"))
                                {
                                    workorder.Attributes.Add("msdyn_reportedbycontact", target.GetAttributeValue<EntityReference>("smp_contact"));
                                }

                                if (target.Attributes.Contains("smp_requestorphone"))
                                {
                                    workorder.Attributes.Add("smp_requestorphonenumber", target.GetAttributeValue<string>("smp_requestorphone"));
                                }

                                if (target.Attributes.Contains("smp_requestoremail"))
                                {
                                    workorder.Attributes.Add("smp_requestoremail", target.GetAttributeValue<string>("smp_requestoremail"));
                                }

                                if (target.Attributes.Contains("smp_requestorid"))
                                {
                                    workorder.Attributes.Add("smp_requestorname", target.GetAttributeValue<EntityReference>("smp_requestorid"));
                                }

                                service.Update(workorder);
                            }
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}