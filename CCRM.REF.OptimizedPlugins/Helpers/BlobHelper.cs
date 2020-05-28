// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobHelper.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// Blobhelper to perform operations on azure storage such as create, delete, get etc.
// </summary>
// ----------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    public class BlobHelper : RestHelper
    {
        /// <summary>
        /// Constructor for blob helper
        /// </summary>
        /// <param name="storageAccount"></param>
        /// <param name="sasKey"></param>
        public BlobHelper(string storageAccount, string sasKey) : base("https://" + storageAccount + ".blob.core.windows.net/", storageAccount, sasKey)
        {
        }

        /// <summary>
        /// Reads or downloads a blob from the Blob service, including its user-defined metadata and system properties.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public byte[] GetBlob(string container, string blob)
        {
            return Retry(delegate
            {
                try
                {
                    var response = CreateRestRequest("GET", container + "/" + blob).GetResponse() as HttpWebResponse;

                    var stream = response.GetResponseStream();

                    var buffer = new byte[16 * 1024];
                    using (var ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }

                        return ms.ToArray();
                    }
                }
                catch (WebException ex)
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// Creates a new blob or replaces an existing blob within a container.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="content"></param>
        /// <param name="metadataList"></param>
        /// <returns></returns>
        public static bool PutBlob(string container, string blob, string content, SortedList<string, string> metadataList = null)
        {
            return Retry(delegate
            {
                try
                {
                    var headers = new SortedList<string, string>
                    {
                        {"x-ms-blob-type", "BlockBlob"}
                    };

                    if (metadataList != null)
                    {
                        foreach (var value in metadataList)
                        {
                            headers.Add("x-ms-meta-" + value.Key, value.Value);
                        }
                    }

                    var response =
                        CreateRestRequest("PUT", container + "/" + blob, content, headers).GetResponse() as HttpWebResponse;
                    response?.Close();
                    return true;
                }
                catch (WebException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        /// <summary>
        /// Marks a blob for deletion. return true on success, false if not found, throw exception on error.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public bool DeleteBlob(string container, string blob)
        {
            return Retry(delegate
            {
                try
                {
                    var response = CreateRestRequest("DELETE", container + "/" + blob).GetResponse() as HttpWebResponse;
                    response?.Close();
                    return true;
                }
                catch (WebException ex)
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// Retrieve a blob's properties.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public SortedList<string, string> GetBlobProperties(string container, string blob)
        {
            return Retry(delegate
            {
                var propertiesList = new SortedList<string, string>();

                try
                {
                    var response = CreateRestRequest("HEAD", container + "/" + blob).GetResponse() as HttpWebResponse;
                    if (response != null)
                    {
                        response.Close();

                        if ((int)response.StatusCode == 200)
                        {
                            if (response.Headers != null)
                            {
                                for (var i = 0; i < response.Headers.Count; i++)
                                {
                                    propertiesList.Add(response.Headers.Keys[i], response.Headers[i]);
                                }
                            }
                        }
                    }

                    return propertiesList;
                }
                catch (WebException ex)
                {
                    throw;
                }
            });
        }
    }
}