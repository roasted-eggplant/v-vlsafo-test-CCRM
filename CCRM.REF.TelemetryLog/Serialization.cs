// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Serialization.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// Serialization class to serialize the JSON object
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.TelemetryLog
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public static class Serialization
    {
        /// <summary>
        /// Deserialize a JSON string into an object.
        /// </summary>
        /// <typeparam name="T">Data type of object</typeparam>
        /// <param name="json">JSON representation of a string</param>
        /// <returns>converted object</returns>
        public static T Deserialize<T>(string json)
        {
            //// ThrowIf.ArgumentNull(json, "json");

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = ConvertStringToStream(json);

            return (T)ser.ReadObject(stream);
        }

        /// <summary>
        /// Serialize an object into a JSON string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="obj">object to serialize</param>
        /// <returns>JSON string</returns>
        public static string Serialize<T>(T obj)
        {
            ////ThrowIf.ArgumentNull(obj, "obj");

            string json = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, obj);
                json = Encoding.Default.GetString(memoryStream.ToArray());
            }

            return json;
        }

        /// <summary>
        /// converts string into stream
        /// </summary>
        /// <param name="json">string to convert</param>
        /// <returns>Memory Stream of JSON</returns>
        private static MemoryStream ConvertStringToStream(string json)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(json));
        }
    }
}
