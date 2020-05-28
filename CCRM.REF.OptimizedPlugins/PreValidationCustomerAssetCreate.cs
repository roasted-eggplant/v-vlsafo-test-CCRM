namespace CCRM.REF.OptimizedPlugins
{
    using Microsoft.Xrm.Sdk;
    using System;
    using CCRM.REF.OptimizedPlugins.Helpers;

    public class PreValidationCustomerAssetCreate : IPlugin
    {
        //This can't be a list from the .resx, because IPlugins are stateless; it must be fully static including in content
        static readonly string[] requiredAttributes = {
            "msdyn_name",
            "msdyn_account",
            "smp_spacefloor",
            "ownerid",
            "statuscode",
            "smp_customerassetclass",
            "smp_assetnumber",
            "smp_serialnumber",
            "smp_manufacturer",
            "smp_model",
            "smp_assetrank",
        };

        public void Execute(IServiceProvider serviceProvider)
        {
            //Run the new asset through validation on the required fields
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            Entity preImage = (Entity)context.InputParameters["Target"];
            ValidationHelper.ValidateEntity(preImage, requiredAttributes); //Don't catch exceptions on this
        }
    }
}
