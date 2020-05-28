// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   Utility class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [ErrorCodeClassInfo(UniqueKey = "Utility", MinValue = 2147482647, MaxValue = 2147483647)]
    public partial class Utility : Plugin
    {
        [ThreadStatic]
        private static Utility instance;

        //[ThreadStatic]
        //private static Dictionary<string, EntityMetadata> entityMetadata;

        //[ThreadStatic]
        //private static Dictionary<string, AttributeMetadata> attributeMetadata;

        public Utility(LocalPluginContext localPluginContext)
            : base(typeof(Utility))
        {
            this.LocalContext = localPluginContext;
            instance = this;
        }

        ////private Utility()
        ////    : base(typeof(Utility))
        ////{
        ////    ////make the default constructor private to hide it.
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "CompareAttributeValue1")]
        ////public static AttributeValue<T> CompareAttributeValue<T>(string attributelogicalname, Entity primarySourceOfValueEntity)
        ////{
        ////    try
        ////    {
        ////        return CompareAttributeValue<T>(attributelogicalname, primarySourceOfValueEntity, null);
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "CompareAttributeValue2")]
        ////public static AttributeValue<T> CompareAttributeValue<T>(string attributelogicalname, Entity primarySourceOfValueEntity, Entity secondarySourceOfValueEntity)
        ////{
        ////    try
        ////    {
        ////        Entity inputParameters = primarySourceOfValueEntity;
        ////        Entity preImageEntity = secondarySourceOfValueEntity;

        ////        AttributeValue<T> returnValue = new AttributeValue<T>();
        ////        returnValue.Changed = false;

        ////        if (inputParameters != null && inputParameters.Contains(attributelogicalname))
        ////        {
        ////            if (inputParameters[attributelogicalname] != null)
        ////            {
        ////                returnValue.Value = (T)inputParameters[attributelogicalname];
        ////            }

        ////            if (preImageEntity != null && preImageEntity.Contains(attributelogicalname) && preImageEntity[attributelogicalname] != null)
        ////            {
        ////                returnValue.PreviousValue = (T)preImageEntity[attributelogicalname];
        ////            }

        ////            if (inputParameters[attributelogicalname] != null && preImageEntity == null)
        ////            {
        ////                returnValue.Changed = true;
        ////            }

        ////            if (typeof(T) == typeof(EntityReference))
        ////            {
        ////                ////Guid Comparizon if value was changed
        ////                AttributeValue<EntityReference> compareObjectEntityReference = returnValue as AttributeValue<EntityReference>;
        ////                if (object.Equals(compareObjectEntityReference.Value, compareObjectEntityReference.PreviousValue) == false ||
        ////                    (compareObjectEntityReference.Value != null &&
        ////                    compareObjectEntityReference.PreviousValue != null &&
        ////                    object.Equals(compareObjectEntityReference.Value.Id, compareObjectEntityReference.PreviousValue.Id) == false))
        ////                {
        ////                    returnValue.Changed = true;
        ////                }
        ////            }
        ////            else if (typeof(T) == typeof(OptionSetValue))
        ////            {
        ////                ////OptionSetValue Int Comparison if value was changed
        ////                AttributeValue<OptionSetValue> compareObjectOptionSetValue = returnValue as AttributeValue<OptionSetValue>;
        ////                if (object.Equals(compareObjectOptionSetValue.Value, compareObjectOptionSetValue.PreviousValue) == false ||
        ////                    (compareObjectOptionSetValue.Value != null &&
        ////                    compareObjectOptionSetValue.PreviousValue != null &&
        ////                    object.Equals(compareObjectOptionSetValue.Value.Value, compareObjectOptionSetValue.PreviousValue.Value) == false))
        ////                {
        ////                    returnValue.Changed = true;
        ////                }
        ////            }
        ////            else if (typeof(T) == typeof(Money))
        ////            {
        ////                ////Money Comparison if value was changed
        ////                AttributeValue<Money> compareObjectMoney = returnValue as AttributeValue<Money>;
        ////                if (object.Equals(compareObjectMoney.Value, compareObjectMoney.PreviousValue) == false ||
        ////                    (compareObjectMoney.Value != null &&
        ////                    compareObjectMoney.PreviousValue != null &&
        ////                    object.Equals(compareObjectMoney.Value.Value, compareObjectMoney.PreviousValue.Value) == false))
        ////                {
        ////                    returnValue.Changed = true;
        ////                }
        ////            }
        ////            else if (typeof(T) == typeof(string))
        ////            {
        ////                AttributeValue<string> compareObjectString = returnValue as AttributeValue<string>;

        ////                if (string.IsNullOrEmpty(compareObjectString.Value) || string.IsNullOrWhiteSpace(compareObjectString.Value))
        ////                {
        ////                    compareObjectString.Value = null;
        ////                }

        ////                if (string.IsNullOrEmpty(compareObjectString.PreviousValue) || string.IsNullOrWhiteSpace(compareObjectString.PreviousValue))
        ////                {
        ////                    compareObjectString.PreviousValue = null;
        ////                }

        ////                if (object.Equals(compareObjectString.Value, compareObjectString.PreviousValue) == false)
        ////                {
        ////                    returnValue.Changed = true;
        ////                }
        ////            }
        ////            else
        ////            {
        ////                if (object.Equals(returnValue.Value, returnValue.PreviousValue) == false)
        ////                {
        ////                    returnValue.Changed = true;
        ////                }
        ////            }
        ////        }
        ////        else
        ////        {
        ////            if (preImageEntity != null && preImageEntity.Contains(attributelogicalname) && preImageEntity[attributelogicalname] != null)
        ////            {
        ////                returnValue.Value = (T)preImageEntity[attributelogicalname];
        ////                returnValue.PreviousValue = (T)preImageEntity[attributelogicalname];
        ////            }
        ////        }

        ////        return returnValue;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}
        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetAttributeMetadata")]
        ////public static AttributeMetadata GetAttributeMetadata(string entityLogicalName, string attributeLogicalName)
        ////{
        ////    try
        ////    {
        ////        string key = string.Format("{0}.{1}", entityLogicalName, attributeLogicalName);

        ////        if (attributeMetadata == null)
        ////        {
        ////            attributeMetadata = new Dictionary<string, AttributeMetadata>();
        ////        }

        ////        if (attributeMetadata.ContainsKey(key) == false)
        ////        {
        ////            try
        ////            {
        ////                RetrieveAttributeRequest request = new RetrieveAttributeRequest();
        ////                request.EntityLogicalName = entityLogicalName;
        ////                request.LogicalName = attributeLogicalName;
        ////                RetrieveAttributeResponse response = (RetrieveAttributeResponse)instance.LocalContext.SystemOrganizationService.Execute(request);

        ////                attributeMetadata.Add(key, response.AttributeMetadata);

        ////                return response.AttributeMetadata;
        ////            }
        ////            catch (System.ServiceModel.FaultException faultexception)
        ////            {
        ////                if (faultexception.Message == "Could not find attribute")
        ////                {
        ////                    instance.PluginErrorHandlerUtility.ShowBusinessProcessError(string.Format("Could not find attribute logical name ({0}) of {1}.", attributeLogicalName, entityLogicalName));
        ////                }

        ////                throw faultexception;
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return attributeMetadata[key];
        ////        }
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetEntityMetadata")]
        ////public static EntityMetadata GetEntityMetadata(string entityLogicalName)
        ////{
        ////    try
        ////    {
        ////        if (entityMetadata == null)
        ////        {
        ////            entityMetadata = new Dictionary<string, EntityMetadata>();
        ////        }

        ////        if (entityMetadata.ContainsKey(entityLogicalName) == false)
        ////        {
        ////            try
        ////            {
        ////                RetrieveEntityRequest request = new RetrieveEntityRequest();
        ////                request.LogicalName = entityLogicalName;
        ////                request.EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity;
        ////                RetrieveEntityResponse response = (RetrieveEntityResponse)instance.LocalContext.SystemOrganizationService.Execute(request);

        ////                entityMetadata.Add(entityLogicalName, response.EntityMetadata);

        ////                return response.EntityMetadata;
        ////            }
        ////            catch (System.ServiceModel.FaultException faultexception)
        ////            {
        ////                if (faultexception.Message == "Could not find entity")
        ////                {
        ////                    instance.PluginErrorHandlerUtility.ShowBusinessProcessError(string.Format("Could not find entity logical name ({0}).", entityLogicalName));
        ////                }

        ////                throw faultexception;
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return entityMetadata[entityLogicalName];
        ////        }
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        /// <summary>
        /// Returns the PreImage based on Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">localContext</exception>
        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetPreImage1")]
        ////public static T GetPreImage<T>(string alias = Constants.PREIMAGESTRINGALIAS)
        ////    where T : Entity
        ////{
        ////    try
        ////    {
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;
        ////        T preImage = default(T);

        ////        ////Check if input parameter is of type entity
        ////        if (context.PreEntityImages.Contains(alias)
        ////            && context.PreEntityImages[alias] is Entity)
        ////        {
        ////            ////Initialize inputParameters
        ////            if (context.PreEntityImages[alias] is Entity && typeof(T).IsSubclassOf(typeof(Entity)))
        ////            {
        ////                preImage = ((Entity)context.PreEntityImages[alias]).ToEntity<T>();
        ////            }
        ////            else if (context.PreEntityImages[alias] is Entity)
        ////            {
        ////                preImage = (T)context.PreEntityImages[alias];
        ////            }
        ////        }

        ////        return preImage;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        /// <summary>
        /// Returns the PreImage based on Entity
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">localContext</exception>
        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetPreImage2")]
        ////public static Entity GetPreImage(string alias = Constants.PREIMAGESTRINGALIAS)
        ////{
        ////    try
        ////    {
        ////        return GetPreImage<Entity>(alias);
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        /// <summary>
        /// Returns the PostImage based on Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">localContext</exception>
        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetPostImage1")]
        ////public static T GetPostImage<T>(string alias = Constants.POSTIMAGESTRINGALIAS)
        ////    where T : Entity
        ////{
        ////    try
        ////    {
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;
        ////        T postImage = default(T);

        ////        ////Check if input parameter is of type entity
        ////        if (context.PostEntityImages.Contains(alias)
        ////            && context.PostEntityImages[alias] is Entity)
        ////        {
        ////            ////Initialize inputParameters
        ////            if (context.PostEntityImages[alias] is Entity && typeof(T).IsSubclassOf(typeof(Entity)))
        ////            {
        ////                postImage = ((Entity)context.PostEntityImages[alias]).ToEntity<T>();
        ////            }
        ////            else if (context.PostEntityImages[alias] is Entity)
        ////            {
        ////                postImage = (T)context.PostEntityImages[alias];
        ////            }
        ////        }

        ////        return postImage;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        /// <summary>
        /// Returns the PostImage based on Entity
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">localContext</exception>
        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetPostImage2")]
        ////public static Entity GetPostImage(string alias = Constants.POSTIMAGESTRINGALIAS)
        ////{
        ////    try
        ////    {
        ////        return GetPostImage<Entity>(alias);
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetEntityTypeInputParameter1")]
        ////public static T GetEntityTypeInputParameter<T>(string key)
        ////    where T : Entity
        ////{
        ////    try
        ////    {
        ////        ////Initialize inputParameters as null
        ////        T inputParameter = default(T);
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;

        ////        ////Check if input parameter is of type entity
        ////        if (context.InputParameters.Contains(key))
        ////        {
        ////            if (context.InputParameters[key] is Entity && typeof(T).IsSubclassOf(typeof(Entity)))
        ////            {
        ////                inputParameter = ((Entity)context.InputParameters[key]).ToEntity<T>();
        ////            }
        ////            else if (context.InputParameters[key] is Entity)
        ////            {
        ////                inputParameter = (T)context.InputParameters[key];
        ////            }
        ////        }

        ////        return inputParameter;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetEntityTypeInputParameter2")]
        ////public static Entity GetEntityTypeInputParameter(string key)
        ////{
        ////    try
        ////    {
        ////        return GetEntityTypeInputParameter<Entity>(key);
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetInputParameter")]
        ////public static T GetInputParameter<T>(string key)
        ////{
        ////    try
        ////    {
        ////        if (typeof(T) == typeof(Entity) || typeof(T).IsSubclassOf(typeof(Entity)))
        ////        {
        ////            throw new ApplicationException("Use GetEntityTypeInputParameter method instead.");
        ////        }

        ////        ////Initialize inputParameters as null
        ////        T inputParameter = default(T);
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;

        ////        ////Check if input parameter is of type entity
        ////        if (context.InputParameters.Contains(key))
        ////        {
        ////            if (context.InputParameters[key] is T)
        ////            {
        ////                inputParameter = (T)context.InputParameters[key];
        ////            }
        ////        }

        ////        return inputParameter;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetEntityTypeOutParameter1")]
        ////public static T GetEntityTypeOutParameter<T>(string key)
        ////    where T : Entity
        ////{
        ////    try
        ////    {
        ////        ////Initialize OutParameter as null
        ////        T outputParameter = default(T);
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;

        ////        ////Check if output parameter is of type entity
        ////        if (context.OutputParameters.Contains(key))
        ////        {
        ////            if (context.OutputParameters[key] is Entity && typeof(T).IsSubclassOf(typeof(Entity)))
        ////            {
        ////                outputParameter = ((Entity)context.OutputParameters[key]).ToEntity<T>();
        ////            }
        ////            else if (context.OutputParameters[key] is Entity)
        ////            {
        ////                outputParameter = (T)context.OutputParameters[key];
        ////            }
        ////        }

        ////        return outputParameter;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetEntityTypeOutParameter2")]
        ////public static Entity GetEntityTypeOutParameter(string key)
        ////{
        ////    try
        ////    {
        ////        return GetEntityTypeOutParameter<Entity>(key);
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetOutputParameters")]
        ////public static T GetOutputParameters<T>(string key)
        ////{
        ////    try
        ////    {
        ////        if (typeof(T) == typeof(Entity) || typeof(T).IsSubclassOf(typeof(Entity)))
        ////        {
        ////            throw new ApplicationException("Use GetEntityTypeOutParameter method instead.");
        ////        }

        ////        ////Initialize output paramater as null
        ////        T outputParameter = default(T);
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;

        ////        ////Check if output parameter is of type entity
        ////        if (context.OutputParameters.Contains(key))
        ////        {
        ////            if (context.OutputParameters[key] is T)
        ////            {
        ////                outputParameter = (T)context.OutputParameters[key];
        ////            }
        ////        }

        ////        return outputParameter;
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ///// <summary>
        ///// This method is triggered on DealRegistration Pre Create Plugin under under Zones.CRM.Sales.Plugins.Core namespace.
        ///// <para>Operation: Determines the shared variable value regardless if it comes from Parent context or Plugin Exececution Context</para>
        ///// </summary>
        ///// <param name="key"></param>
        ///// 
        ///// <returns></returns>
        ////[ErrorCodeClassMemberInfo(UniqueKey = "GetSharedVariable")]
        ////public static T GetSharedVariable<T>(string key)
        ////{
        ////    try
        ////    {
        ////        IPluginExecutionContext context = instance.LocalContext.PluginExecutionContext;

        ////        ////verify if the shared variable is present in the Plugin Execution Context
        ////        if (context.SharedVariables.Contains(key))
        ////        {
        ////            return (T)context.SharedVariables[key];
        ////        }
        ////        ////verify if the shared variable is present in the parent context
        ////        if (context.ParentContext.SharedVariables.Contains(key))
        ////        {
        ////            return (T)context.ParentContext.SharedVariables[key];
        ////        }

        ////        return default(T);
        ////    }
        ////    catch (InvalidPluginExecutionException invalidPluginExecutionException)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<InvalidPluginExecutionException, MemberInfo>(invalidPluginExecutionException, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        throw instance.PluginErrorHandlerUtility.ThrowException<Exception, MemberInfo>(exception, System.Reflection.MethodBase.GetCurrentMethod());
        ////    }
        ////}

        ////public class AttributeValue<T>
        ////{
        ////    public T Value { get; set; }

        ////    public T PreviousValue { get; set; }

        ////    public bool Changed { get; set; }
        ////}
    }
}
