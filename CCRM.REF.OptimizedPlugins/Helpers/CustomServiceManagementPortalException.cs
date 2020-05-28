// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomServiceManagementPortalException.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  CustomServiceManagementPortalException Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Exception
    /// </summary>
    [Serializable]
    public class CustomServiceManagementPortalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceManagementPortalException" /> class.
        /// </summary>
        public CustomServiceManagementPortalException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceManagementPortalException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CustomServiceManagementPortalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceManagementPortalException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CustomServiceManagementPortalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceManagementPortalException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected CustomServiceManagementPortalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
