// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemNetWebClient.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   ISystemNetWebClient Interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.DataMallSync.Contracts
{
    public interface ISystemNetWebClient
    {
        string DownloadString(string webAddress, string vlcmPackageNumber);
    }
}
