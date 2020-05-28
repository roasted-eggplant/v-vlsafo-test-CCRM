// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   Constants
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    public class Constants
    {
        #region Plugin Base Class Variables

        /// <summary>
        /// The path to the Project Configuration File
        /// </summary>
        public const string PROJECTCONFIG = "";

        /// <summary>
        /// This constant is used in GetAdministratorId method in Plugin.LocalPluginContext class
        /// </summary>
        public const string ADMINISTRATORID = "";

        /// <summary>
        /// The Pre Image string Alias
        /// </summary>
        public const string PREIMAGESTRINGALIAS = "PreImage";

        /// <summary>
        /// The Post Image string alias
        /// </summary>
        public const string POSTIMAGESTRINGALIAS = "PostImage";

        /// <summary>
        /// The Plugin local context
        /// </summary>
        public const string LOCALCONTEXT = "localContext";

        /// <summary>
        /// The plugin target
        /// </summary>
        public const string PLUGINTARGET = "Target";
        #endregion

        #region Plugin Messages
        /// <summary>
        /// Retrieve message.
        /// </summary>
        public const string Retrieve = "Retrieve";

        /// <summary>
        /// Create message.
        /// </summary>
        public const string Create = "Create";

        /// <summary>
        /// Associate message.
        /// </summary>
        public const string Associate = "Associate";

        /// <summary>
        /// Update message.
        /// </summary>
        public const string Update = "Update";
        #endregion       

        public const string ProductObjectCode = "1024";

        public const int SourceCRM = 180620000;
        public const int SourceAX = 180620001;
    }
}
