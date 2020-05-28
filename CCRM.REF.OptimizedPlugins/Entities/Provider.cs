// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Provider.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  Provider Entity Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Entities
{
    using System;

    /// <summary>
    /// Holds the provider instance
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Gets or sets the provider id.
        /// </summary>
        /// <value>The provider id.</value>
        public Guid ProviderId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is integration enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is integration enabled; otherwise, <c>false</c>.</value>
        public bool IsIntegrationEnabled { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string ProviderServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is provider updated.
        /// </summary>
        /// <value><c>true</c> if this instance is provider updated; otherwise, <c>false</c>.</value>
        public bool IsProviderUpdated { get; set; }

        /// <summary>
        /// User Story 3344963: 
        /// AAd/ SSL Implementationfor Tier1's -Outbound
        /// </summary>
        public bool HostedOnAzure { get; set; }

        public Guid ProviderTeamId { get; set; }
    }
}
