// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePlugin.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  BasePlugin Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public abstract class BasePlugin : IPlugin
    {
        protected IPluginExecutionContext context;
        protected IOrganizationServiceFactory serviceFactory;
        protected IOrganizationService service;
        protected ITracingService tracingService;

        protected BasePlugin()
        {
            ////No Implementation
        }

        protected BasePlugin(string unsecureConfig, string secureConfig, string schemaString, Type configurationType)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
            this.SchemaString = schemaString;
            this.ConfigurationType = configurationType;
        }

        public string SecureConfig
        {
            get;
        }

        public string UnsecureConfig
        {
            get;
        }

        protected string SchemaString
        {
            get;
            set;
        }

        protected Type ConfigurationType
        {
            get;
            set;
        }

        public abstract void Execute();

        #region IPlugin Members

        /// <summary>
        /// Initiate the base plugin context and all member variables.
        /// </summary>
        /// <param name="serviceProvider">The ServiceProvider object passed by the Execute method</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("ServiceProvider is null.");
                }

                // Obtain the execution context from the service provider.
                this.context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (this.context == null)
                {
                    throw new Exception("Could not execute plugin:  The plugin context was null");
                }

                this.serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                if (this.serviceFactory != null)
                {
                    this.service = this.serviceFactory.CreateOrganizationService(this.context.UserId);
                }
                else
                {
                    throw new Exception("Could not execute plugin:  The organization service was null");
                }

                ////Extract the tracing service for use in debugging sandboxed plug-ins.
                this.tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                ////call abstract method
                this.Execute();
            }
            catch (Exception e)
            {
                if (e is InvalidPluginExecutionException)
                {
                    //// Let InvalidPluginExecutionExceptions flow without wrapping them
                    throw;
                }

                var ex = new InvalidPluginExecutionException($"An error has occurred in CRM plugin '{GetType().Name}': {e.Message}", e);
                throw ex;
            }
        }

        #endregion

        #region Generic Helper methods..
        public string GetOptionSetValueLabel(string entityName, string fieldName, int optionSetValue)
        {
            var attReq = new RetrieveAttributeRequest();
            attReq.EntityLogicalName = entityName;
            attReq.LogicalName = fieldName;
            attReq.RetrieveAsIfPublished = true;

            var attResponse = (RetrieveAttributeResponse)this.service.Execute(attReq);
            var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;

            return attMetadata.OptionSet.Options.Where(x => x.Value == optionSetValue).FirstOrDefault().Label.UserLocalizedLabel.Label;
        }

        /// <summary>
        /// Get the Target Entity data from the record that triggered the plugin execution
        /// </summary>
        /// <param name="parameterCollection">A collection containing Input Parameter data for the plugin execution</param>
        /// <returns>Entity object containing the record fields that were updated and triggered the plugin.</returns>
        protected T GetEntityFromTarget<T>(DataCollection<string, object> parameterCollection)
        {
            if (parameterCollection == null)
            {
                throw new ArgumentException("No Input Parameters were found in the context. Please contact your system administrator.");
            }

            if (!parameterCollection.Contains("Target"))
            {
                throw new ArgumentException("No Target Entity was found in the context. Please contact your system administrator.");
            }

            T entity = (T)Convert.ChangeType(parameterCollection["Target"], typeof(T));
            return entity;
        }

        protected string GetInputParameterValue(string parameterName)
        {
            if (this.context.InputParameters.Contains(parameterName) && !string.IsNullOrEmpty(Convert.ToString(this.context.InputParameters[parameterName])))
            {
                return Convert.ToString(this.context.InputParameters[parameterName]).Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        protected T GetInputParameterValue<T>(string parameterName)
        {
            if (this.context.InputParameters.Contains(parameterName) && this.context.InputParameters[parameterName] != null)
            {
                return (T)Convert.ChangeType(this.context.InputParameters[parameterName], typeof(T));
            }

            return default(T);
        }

        ////public string MakeWebRequest(string url, string data, string requestMethod, Dictionary<string, string> headers, string contentType = Constants.CONSTANT_CONTENETTYPE)
        ////{
        ////    string response = string.Empty;
        ////    try
        ////    {
        ////        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        ////        byte[] requestInFormOfBytes = System.Text.Encoding.UTF8.GetBytes(data);
        ////        request.Method = requestMethod;
        ////        request.Accept = Constants.CONSTANT_CONTENETTYPE;
        ////        request.ContentType = contentType;
        ////        request.ContentLength = requestInFormOfBytes.Length;
        ////        // Add all the Header information
        ////        if (headers != null)
        ////        {
        ////            foreach (string key in headers.Keys)
        ////                request.Headers.Add(key, headers[key]);
        ////        }

        ////        tracingService.Trace("Web API Call Initiated at: {0}", DateTime.Now);

        ////        // Write the request body for POST calls.
        ////        if (requestMethod == Constants.CONSTANT_REQUESTMETHODPOST)
        ////        {
        ////            Stream requestStream = request.GetRequestStream();
        ////            requestStream.Write(requestInFormOfBytes, 0, requestInFormOfBytes.Length);
        ////            requestStream.Close();
        ////        }
        ////        // Try and retrieve the Response
        ////        HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
        ////        using (webResponse)
        ////        {
        ////            using (Stream webStream = webResponse.GetResponseStream())
        ////            {
        ////                if (webStream != null)
        ////                {
        ////                    StreamReader responseReader = new StreamReader(webStream);
        ////                    response = responseReader.ReadToEnd();
        ////                }
        ////            }
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        response = "ERROR";
        ////        tracingService.Trace("Error in MakeWebRequest: {0}", ex.Message);
        ////    }
        ////    finally
        ////    {
        ////        tracingService.Trace("Web API Call Completed at: {0}", DateTime.Now);
        ////    }
        ////    return response;
        ////}

        #region GetEntityAttribute Overloaded methods

        protected T GetEntityAttributeValue<T>(string entityName, string attribute, string attributeKey, object attributeValue)
        {
            T returnObj = default(T);
            EntityCollection coll = this.GetEntityAttributeValue(entityName, new List<string> { attribute }, attributeKey, attributeValue);
            if (coll != null && coll.Entities != null && coll.Entities.Count > 0 && coll.Entities[0].Attributes != null && coll.Entities[0].Attributes.Contains(attribute))
            {
                ////returnObj = (T)Convert.ChangeType(coll.Entities[0].Attributes[attribute], typeof(T));
                returnObj = (T)Convert.ChangeType(coll.Entities[0], typeof(T));
            }

            return returnObj;
        }

        protected EntityCollection GetEntityAttributeValue(string entityName, List<string> attributeList, string attributeKey, object attributeValue)
        {
            this.tracingService.Trace("Querying the entity {0} where {1} == {2}", entityName, attributeKey, attributeValue);
            ConditionExpression condition = new ConditionExpression();
            condition.AttributeName = attributeKey;
            condition.Operator = ConditionOperator.Equal;
            condition.Values.Add(attributeValue);

            FilterExpression filter = new FilterExpression();
            filter.AddCondition(condition);

            return this.GetEntityAttributeValue(entityName, attributeList, filter);
        }

        protected EntityCollection GetEntityAttributeValue(string entityName, List<string> returnAttributes, FilterExpression queryFilter)
        {
            QueryExpression query = new QueryExpression(entityName);
            if ((returnAttributes != null) && (returnAttributes.Count > 0))
            {
                query.ColumnSet.AddColumns(returnAttributes.ToArray());
            }

            query.Criteria.AddFilter(queryFilter);

            return this.GetEntityAttributeValue(query);
        }

        protected EntityCollection GetEntityAttributeValue(QueryExpression expression)
        {
            this.tracingService.Trace("Executing QueryExpression on Entity: " + expression.EntityName);
            return this.service.RetrieveMultiple(expression);
        }

        protected Entity GetEntityAttributeValue(string entityName, Guid entityId, List<string> returnAttributes)
        {
            this.tracingService.Trace("Executing Retrieve on Entity: " + entityName);
            return this.service.Retrieve(entityName, entityId, new ColumnSet(returnAttributes.ToArray()));
        }

        #endregion        

        /// <summary>
        /// Checks to see if the entity has the provided attribute and if so returns the string representation of the value.
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        protected string TryExtractValidAttributeValue(Entity ent, string attributeName)
        {
            if (ent.Attributes.Contains(attributeName) && (ent[attributeName] != null))
            {
                return Convert.ToString(ent.Attributes[attributeName]);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the exact value of the Attribute for the entity.
        /// If the entity doesn't have a value, a default for the value is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ent"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        protected T TryExtractValidAttributeValue<T>(Entity ent, string attributeName)
        {
            if (ent.Attributes.Contains(attributeName) && (ent[attributeName] != null))
            {
                return (T)Convert.ChangeType(ent.Attributes[attributeName], typeof(T));
            }

            return default(T);
        }

        #endregion
    }
}
