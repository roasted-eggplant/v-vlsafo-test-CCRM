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
//  CCRMConstants Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.OptimizedPlugins.Helpers
{
    /// <summary>
    /// Constants file for maintaing hard coded values.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///  To limit the access to the project, declaring variable as internal.
        ///  PriorityEscalationPopupNotesSchemaName
        /// </summary>
        internal static string PriorityEscalationPopupNotesSchemaName = "smp_priorityescalationpopupnotes";

        /// <summary>
        ///   AnnotationEntityName
        /// </summary>
        internal static string AnnotationEntityName = "annotation";

        /// <summary>
        ///  ObjectId
        /// </summary>
        internal static string ObjectId = "objectid";

        /// <summary>
        /// Subject
        /// </summary>
        internal static string Subject = "subject";

        /// <summary>
        ///   EscalationDetails
        /// </summary>
        internal static string EscalationDetails = "Escalation Details from Popup";

        /// <summary>
        ///   ObjectTypeCode
        /// </summary>
        internal static string ObjectTypeCode = "objecttypecode";

        /// <summary>
        ///  PriorityNotesNoteText
        /// </summary>
        internal static string PriorityNoteText = "notetext";

        /// <summary>
        ///   Target
        /// </summary>
        internal static string Target = "Target";

        /// <summary>
        ///    Update
        /// </summary>
        internal static string Update = "Update";

        /// <summary>
        ///  Create
        /// </summary>
        internal static string Create = "Create";

        /// <summary>
        ///   IncidentEntityaName
        /// </summary>
        internal static string IncidentEntityaName = "incident";

        /// <summary>
        ///    AccountEntityName
        /// </summary>
        internal static string AccountEntityName = "account";

        /// <summary>
        ///    AccounttypeSchemaName
        /// </summary>
        internal static string AccountTypeSchemaName = "smp_accounttype";

        /// <summary>
        /// NameSchemaName
        /// </summary>
        internal static string Name = "name";

        /// <summary>
        ///  ProviderSchemaName
        /// </summary>
        internal static string ProviderSchemaName = "smp_provider";

        /// <summary>
        ///   PopupMessage
        /// </summary>
        internal static string PopupMessage = "Already a PoP up Message exists hence Aborting ........";

        /// <summary>
        ///  Team
        /// </summary>
        internal static string Team = "team";

        /// <summary>
        ///   BusinessunitSchemaName
        /// </summary>
        internal static string BusinessunitSchemaName = "businessunit";

        /// <summary>
        ///  BusinessunitIdSchemaName
        /// </summary>
        internal static string BusinessunitIdSchemaName = "businessunitid";

        /// <summary>
        ///   TeamErrorMsg
        /// </summary>
        internal static string TeamErrorMsg = "Error while creating team.";

        /// <summary>
        ///   ProviderTeamSchemaName
        /// </summary>
        internal static string ProviderTeamSchemaName = "smp_providerteam";

        /// <summary>
        ///   ProviderAssigningErrorMsg
        /// </summary>
        internal static string ProviderAssigningErrorMsg = "Error while assigning team to provider.";

        /// <summary>
        ///   role
        /// </summary>
        internal static string RoleEntityName = "role";

        /// <summary>
        ///   roleid
        /// </summary>
        internal static string RoleIdSchemaName = "roleid";

        /// <summary>
        ///   SecurityRoleProvider
        /// </summary>
        internal static string SecurityRoleProvider = "Provider";

        /// <summary>
        ///   TeamRolesRelationshipName
        /// </summary>
        internal static string TeamRolesRelationshipName = "teamroles_association";

        /// <summary>
        ///   ProviderSecurityRoleAssigningErrorMsg
        /// </summary>
        internal static string ProviderSecurityRoleAssigningErrorMsg = "Error while assigning provider security role to team.";

        /// <summary>
        ///   Service Request Status
        /// </summary>
        internal static int SRStatus_Cancelled = 6;

        /// <summary>
        ///   Employee or other staff category
        /// </summary>
        internal enum StaffingResourceType
        {
            Regular = 180620000,
            Other = 180620001,
            Vendor = 180620002
        }
    }
}
