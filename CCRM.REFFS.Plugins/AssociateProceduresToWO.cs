// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociateProceduresToWO.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  AssociateProceduresToWO Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using CCRM.REFFS.Plugins.Common;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;

    public class AssociateProceduresToWO : Plugin
    {
        public AssociateProceduresToWO() : base(typeof(AssociateProceduresToWO))
        {
            this.RegisteredEvents.Add(new Tuple<int, string, string, Action<LocalPluginContext>>(40, Constants.Update, "msdyn_agreementbookingdate", new Action<LocalPluginContext>(this.AssociateProcedure)));
        }

        private void AssociateProcedure(LocalPluginContext localContext)
        {
            try
            {
                IPluginExecutionContext context = localContext.PluginExecutionContext;
                ITracingService tracingService = localContext.TracingService;
                IOrganizationService service = localContext.OrganizationService;
                tracingService.Trace("Procedure association");
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity agreementBooking = context.InputParameters["Target"] as Entity;
                    if (agreementBooking.GetAttributeValue<EntityReference>("msdyn_workorder") != null)
                    {
                        tracingService.Trace("Retrieveing the Booking setup");
                        Entity agreementbookingrecord = service.Retrieve(agreementBooking.LogicalName, agreementBooking.Id, new ColumnSet("msdyn_bookingsetup", "msdyn_workorder"));
                        Entity agreementbookingSetUp = service.Retrieve(agreementbookingrecord.GetAttributeValue<EntityReference>("msdyn_bookingsetup").LogicalName, agreementbookingrecord.GetAttributeValue<EntityReference>("msdyn_bookingsetup").Id, new ColumnSet("smp_procedureid"));
                        if (agreementbookingSetUp.Attributes.Contains("smp_procedureid"))
                        {
                            try
                            {
                                tracingService.Trace(" Creating new association");
                                EntityReference procedure = (EntityReference)agreementbookingSetUp.Attributes["smp_procedureid"];
                                this.UpdateProcedureTOWOrkorder(service, agreementBooking.GetAttributeValue<EntityReference>("msdyn_workorder").Id, procedure);
                                tracingService.Trace(" End association");
                            }
                            catch (Exception ex)
                            {
                                tracingService.Trace(" Error in association:" + ex.Message);
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

        private void UpdateProcedureTOWOrkorder(IOrganizationService service, Guid id, EntityReference procedure)
        {
            EntityReferenceCollection entityReferenceCollection = new EntityReferenceCollection();
            entityReferenceCollection.Add(procedure);
            service.Associate("msdyn_workorder", id, new Relationship("smp_smp_procedure_msdyn_workorder"), entityReferenceCollection);
        }
    }
}