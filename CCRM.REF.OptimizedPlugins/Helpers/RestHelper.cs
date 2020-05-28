// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Resthelper to connect the storage container in azure.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class RestHelper
    {
        protected static bool IsTableStorage { get; set; }

        /// <summary>
        /// Get set  accessor for Endpoint
        /// </summary>
        public static string EndPoint { get; internal set; }

        /// <summary>
        /// Get set  accessor for Storage account
        /// </summary>
        public static string StorageAccount { get; internal set; }

        /// <summary>
        /// Get set  accessor for storage key
        /// </summary>
        public static string SasKey { get; internal set; }

        /// <summary>
        /// Assign endpoint, storage account details
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="storageAccount"></param>
        /// <param name="sasKey"></param>
        public RestHelper(string endpoint, string storageAccount, string sasKey)
        {
            EndPoint = endpoint;
            StorageAccount = storageAccount;
            SasKey = sasKey;
        }

        #region REST HTTP Request Helper Methods
        /// <summary>
        /// Construct and issue a REST request and return the response.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="resource"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <param name="ifMatch"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        public static HttpWebRequest CreateRestRequest(string method, string resource, string requestBody = null, SortedList<string, string> headers = null, string ifMatch = "", string md5 = "")
        {
            byte[] byteArray = null;
            var now = DateTime.UtcNow;
            var uri = EndPoint + resource + SasKey;

            var request = WebRequest.Create(uri) as HttpWebRequest;
            if (request == null)
            {
                return null;
            }

            request.Method = method;
            request.ContentLength = 0;
            request.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
            request.Headers.Add("x-ms-version", "2015-12-11");

            if (IsTableStorage)
            {
                request.ContentType = "application/atom+xml";

                request.Headers.Add("DataServiceVersion", "1.0;NetFx");
                request.Headers.Add("MaxDataServiceVersion", "1.0;NetFx");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Headers.Add("Accept-Charset", "UTF-8");
                byteArray = Convert.FromBase64String(requestBody);
                request.ContentLength = byteArray.Length;
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                if (byteArray != null)
                {
                    request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
                }
            }

            return request;
        }

        /// <summary>
        /// Get canonicalized headers.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetCanonicalizedHeaders(HttpWebRequest request)
        {
            var headerNameList = new ArrayList();
            var sb = new StringBuilder();
            foreach (string headerName in request.Headers.Keys)
            {
                if (headerName.ToLowerInvariant().StartsWith("x-ms-", StringComparison.Ordinal))
                {
                    headerNameList.Add(headerName.ToLowerInvariant());
                }
            }

            headerNameList.Sort();
            foreach (string headerName in headerNameList)
            {
                var builder = new StringBuilder(headerName);
                var separator = ":";
                foreach (string headerValue in this.GetHeaderValues(request.Headers, headerName))
                {
                    var trimmedValue = headerValue.Replace("\r\n", string.Empty);
                    builder.Append(separator);
                    builder.Append(trimmedValue);
                    separator = ",";
                }

                sb.Append(builder);
                sb.Append("\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get header values.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public ArrayList GetHeaderValues(NameValueCollection headers, string headerName)
        {
            var list = new ArrayList();
            var values = headers.GetValues(headerName);
            if (values != null)
            {
                foreach (var str in values)
                {
                    list.Add(str.TrimStart(null));
                }
            }

            return list;
        }

        /// <summary>
        ///  Get canonicalized resource.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        ////public string GetCanonicalizedResource(Uri address, string accountName)
        ////{
        ////    var str = new StringBuilder();
        ////    var builder = new StringBuilder("/");
        ////    builder.Append(accountName);
        ////    builder.Append(address.AbsolutePath);
        ////    str.Append(builder);
        ////    var values2 = new NameValueCollection();
        ////    if (!IsTableStorage)
        ////    {
        ////        var values = address.ParseQueryString();
        ////        foreach (string str2 in values.Keys)
        ////        {
        ////            var list = new ArrayList(values.GetValues(str2));
        ////            list.Sort();
        ////            var builder2 = new StringBuilder();
        ////            foreach (var obj2 in list)
        ////            {
        ////                if (builder2.Length > 0)
        ////                {
        ////                    builder2.Append(",");
        ////                }

        ////                builder2.Append(obj2);
        ////            }

        ////            values2.Add(str2?.ToLowerInvariant(), builder2.ToString());
        ////        }
        ////    }

        ////    var list2 = new ArrayList(values2.AllKeys);
        ////    list2.Sort();
        ////    foreach (string str3 in list2)
        ////    {
        ////        var builder3 = new StringBuilder(string.Empty);
        ////        builder3.Append(str3);
        ////        builder3.Append(":");
        ////        builder3.Append(values2[str3]);
        ////        str.Append("\n");
        ////        str.Append(builder3);
        ////    }

        ////    return str.ToString();
        ////}
        #endregion

        #region Retry Delegate
        public delegate T RetryDelegate<out T>();
        public delegate void RetryDelegate();
        const int RetryCount = 3;
        const int RetryIntervalMs = 200;

        /// <summary>
        /// Retry delegate with default retry settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        /// <returns></returns>
        public static T Retry<T>(RetryDelegate<T> del)
        {
            return Retry(del, RetryCount, RetryIntervalMs);
        }

        /// <summary>
        /// Retry delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        /// <param name="numberOfRetries"></param>
        /// <param name="msPause"></param>
        /// <returns></returns>
        public static T Retry<T>(RetryDelegate<T> del, int numberOfRetries, int msPause)
        {
            var counter = 0;
            RetryLabel:

            try
            {
                counter++;
                return del.Invoke();
            }
            catch (Exception ex)
            {
                if (counter > numberOfRetries)
                {
                    throw;
                }

                if (msPause > 0)
                {
                    Thread.Sleep(msPause);
                }

                goto RetryLabel;
            }
        }

        /// <summary>
        /// Retry delegate with default retry settings.
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public static bool Retry(RetryDelegate del)
        {
            return Retry(del, RetryCount, RetryIntervalMs);
        }

        /// <summary>
        /// Retry delegate
        /// </summary>
        /// <param name="del"></param>
        /// <param name="numberOfRetries"></param>
        /// <param name="msPause"></param>
        /// <returns></returns>
        public static bool Retry(RetryDelegate del, int numberOfRetries, int msPause)
        {
            var counter = 0;

            RetryLabel:
            try
            {
                counter++;
                del.Invoke();
                return true;
            }
            catch (Exception)
            {
                if (counter > numberOfRetries)
                {
                    throw;
                }

                if (msPause > 0)
                {
                    Thread.Sleep(msPause);
                }

                goto RetryLabel;
            }
        }
        #endregion
    }
}