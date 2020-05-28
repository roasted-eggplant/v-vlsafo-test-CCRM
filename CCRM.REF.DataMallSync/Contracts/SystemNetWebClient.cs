// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemNetWebClient.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  SystemNetWebClient 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.DataMallSync.Contracts
{
    ////using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.IO;
    using System.Net;
    using System.Text;

    public class SystemNetWebClient : CCRM.REF.DataMallSync.Contracts.ISystemNetWebClient
    {
        public string DownloadString(string webAddress, string vlcmPackageNumber)
        {
            try
            {
                WebClient client = new WebClient();
                ////client.Headers[HttpRequestHeader.Authorization] = "Bearer" + result.AccessToken;
                //// Output Response
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(webAddress + vlcmPackageNumber);
            }
            catch (WebException e)
            {
                string resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                throw new WebException(resp);
            }
        }

        ////--------------------------------------------------Sample----------------------------------------------------//
        ////WebClientHandler client = new WebClientHandler();
        ////client.UseDefaultCredentials = false;
        ////client.Encoding = Encoding.UTF8;
        ////return client.DownloadString(webAddress + vlcmPackageNumber);

        ////--------------------------------------------------Sample----------------------------------------------------//

        ////class WebClientHandler : WebClient
        ////{
        ////    protected override WebRequest GetWebRequest(Uri address)
        ////    {
        ////        var request = base.GetWebRequest(address);

        ////        //Read Certificate
        ////        X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        ////        certStore.Open(OpenFlags.ReadOnly);
        ////        X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, "6A9D9AB506F410B89BBDF3F7E5493C171F3BA91B", false);

        ////        var cert = certCollection[0];

        ////        (request as HttpWebRequest).ClientCertificates.Add(cert);

        ////        return request;
        ////    }
        ////}
    }
}