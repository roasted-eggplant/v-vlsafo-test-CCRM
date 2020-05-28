// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginExtensions.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  PluginExtensions 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using System;

    internal static class PluginExtensions
    {
        internal const string PostEntityImageName = "PostImage";
        internal const string PreEntityImageName = "PreImage";

        private enum SdkMessageProcessingStepStage
        {
            InitialPreoperation_Forinternaluseonly = 5,

            Prevalidation = 10,

            InternalPreoperationBeforeExternalPlugins_Forinternaluseonly = 15,

            Preoperation = 20,

            InternalPreoperationAfterExternalPlugins_Forinternaluseonly = 25,

            MainOperation_Forinternaluseonly = 30,

            InternalPostoperationBeforeExternalPlugins_Forinternaluseonly = 35,

            Postoperation = 40,

            InternalPostoperationAfterExternalPlugins_Forinternaluseonly = 45,

            Postoperation_Deprecated = 50,

            FinalPostoperation_Forinternaluseonly = 55,
        }

        public static EntityReference GetEntityReferenceFromContext(this IExecutionContext context, bool isCheckTarget = false)
        {
            if (isCheckTarget && context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
            {
                return context.InputParameters["Target"] as EntityReference;
            }

            return new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId);
        }

        public static EntityReferenceCollection GetEntityReferenceCollection(this IExecutionContext context)
        {
            if (context.InputParameters.Contains("RelatedEntities"))
            {
                return context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
            }

            return null;
        }

        public static bool IsAssignOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Assign", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsAssociateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Associate", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsDisassociateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Disassociate", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static Entity GetEntityFromContext(this IExecutionContext context)
        {
            if (context.IsRetrieveOperation() && context.OutputParameters.Contains("BusinessEntity"))
            {
                return context.OutputParameters["BusinessEntity"] as Entity;
            }

            if (!context.IsRetrieveOperation() && context.InputParameters.Contains("Target"))
            {
                return context.InputParameters["Target"] as Entity;
            }

            return null;
        }

        internal static bool IsPreValidationStage(this IPluginExecutionContext context)
        {
            return context.Stage == (int)SdkMessageProcessingStepStage.Prevalidation;
        }

        internal static bool IsPreOperationStage(this IPluginExecutionContext context)
        {
            return context.Stage == (int)SdkMessageProcessingStepStage.Preoperation;
        }

        internal static bool IsPostOperationStage(this IPluginExecutionContext context)
        {
            return context.Stage == (int)SdkMessageProcessingStepStage.Postoperation;
        }

        internal static Entity GetPreEntity(this IExecutionContext context)
        {
            if (context.PreEntityImages.Contains(PreEntityImageName) && context.PreEntityImages[PreEntityImageName] != null)
            {
                return context.PreEntityImages[PreEntityImageName];
            }

            return null;
        }

        internal static Entity GetPostEntity(this IExecutionContext context)
        {
            if (context.PostEntityImages != null && context.PostEntityImages.Contains(PostEntityImageName) && context.PostEntityImages[PostEntityImageName] != null)
            {
                return context.PostEntityImages[PostEntityImageName];
            }

            return null;
        }

        /// <summary>
        ///     Return true when it matches the name of operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsSetStateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "SetState", StringComparison.InvariantCultureIgnoreCase) || string.Equals(context.MessageName, "SetStateDynamicEntity", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static bool IsSetStateOperation(this IExecutionContext context, int targetState)
        {
            if (context.IsSetStateOperation())
            {
                // Verify the target state
                var oldState = ImageUtilities.GetFinalAttribute("statecode", PreEntityImageName, context) as OptionSetValue;
                var newState = ImageUtilities.GetFinalAttribute("statecode", PreEntityImageName, context) as OptionSetValue;

                if (newState != null && (ImageUtilities.IsOptionSetEqual(oldState, newState) && newState.Value == targetState))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return true when it is create operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsCreateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Create", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static bool IsRetrieveOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Retrieve", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Return true when it is update operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsUpdateOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Update", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Return true when it is delete operation
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static bool IsDeleteOperation(this IExecutionContext context)
        {
            return string.Equals(context.MessageName, "Delete", StringComparison.InvariantCultureIgnoreCase);
        }

        internal static Guid? GetActualId(this Entity currentEntity, Entity savedEntity, string attributeName)
        {
            if (currentEntity.GetAttributeValue<EntityReference>(attributeName) != null)
            {
                return currentEntity.GetAttributeValue<EntityReference>(attributeName).Id;
            }

            if (!currentEntity.Contains(attributeName) && savedEntity.GetAttributeValue<EntityReference>(attributeName) != null)
            {
                return savedEntity.GetAttributeValue<EntityReference>(attributeName).Id;
            }

            return null;
        }
    }
}