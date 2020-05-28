// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCode.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//   ErrorCode Enumeration
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins.Common
{
    public enum ErrorCode
    {
        OutOfTheBox = 10000000,

        Unknown = 10000001,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "CompareAttributeValue1", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.CompareAttributeValue")]
        ////ErrorCode1 = 2147482647,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "CompareAttributeValue2", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.CompareAttributeValue")]
        ////ErrorCode2 = 2147482648,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetAttributeMetadata", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetAttributeMetadata")]
        ////ErrorCode3 = 2147482649,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetEntityMetadata", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetEntityMetadata")]
        ////ErrorCode4 = 2147482650,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetPreImage1", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetPreImage")]
        ////ErrorCode5 = 2147482651,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetPreImage2", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetPreImage")]
        ////ErrorCode6 = 2147482652,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetPostImage1", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetPostImage")]
        ////ErrorCode7 = 2147482653,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetPostImage2", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetPostImage")]
        ////ErrorCode8 = 2147482654,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetEntityTypeInputParameter1", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetEntityTypeInputParameter")]
        ////ErrorCode9 = 2147482655,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetEntityTypeInputParameter2", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetEntityTypeInputParameter")]
        ////ErrorCode10 = 2147482656,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetInputParameter", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetInputParameter")]
        ////ErrorCode11 = 2147482657,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetEntityTypeOutParameter1", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetEntityTypeOutParameter")]
        ////ErrorCode12 = 2147482658,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetEntityTypeOutParameter2", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetEntityTypeOutParameter")]
        ////ErrorCode13 = 2147482659,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetOutputParameters", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetOutputParameters")]
        ////ErrorCode14 = 2147482660,

        ////[ErrorCodeEnumItemInfo(ClassKey = "Utility", MemberKey = "GetSharedVariable", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.Common.Utility.GetSharedVariable")]
        ////ErrorCode15 = 2147482661,

        ////[ErrorCodeEnumItemInfo(ClassKey = "PreAccountCreate", MemberKey = "ExecutePreAccountCreate", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.PreAccountCreate.ExecutePreAccountCreate")]
        ////ErrorCode16 = 30000000,

        ////[ErrorCodeEnumItemInfo(ClassKey = "PreAccountCreate", MemberKey = "Method1", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.PreAccountCreate.Method1")]
        ////ErrorCode17 = 30000001,

        ////[ErrorCodeEnumItemInfo(ClassKey = "PreAccountCreate", MemberKey = "Method2", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.PreAccountCreate.Method2")]
        ////ErrorCode18 = 30000002,

        ////[ErrorCodeEnumItemInfo(ClassKey = "PreAccountCreate", MemberKey = "Method3", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.PreAccountCreate.Method3")]
        ////ErrorCode19 = 30000003,

        ////[ErrorCodeEnumItemInfo(ClassKey = "PreAccountCreate", MemberKey = "Method4", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.PreAccountCreate.Method4")]
        ////ErrorCode20 = 30000004,

        ////[ErrorCodeEnumItemInfo(ClassKey = "PreAccountCreate", MemberKey = "Method5", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.PreAccountCreate.Method5")]
        ////ErrorCode21 = 30000005,

        ////[ErrorCodeEnumItemInfo(ClassKey = "SampleAccountAsync", MemberKey = "ExecutePostAccountCreateAsync", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.SampleAccountAsync.ExecutePostAccountCreateAsync")]
        ////ErrorCode22 = 50000000,

        ////[ErrorCodeEnumItemInfo(ClassKey = "SamplePlugin", MemberKey = "PreCreate", FullyQualifiedName = "CCRM.Magnum.Plugin.Ticket.SamplePlugin.PreCreate")]
        ////ErrorCode23 = 60000000,
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public partial class ErrorCodeEnumItemInfo : System.Attribute
    {
        private string classKey;

        private string memberKey;

        private string fullyQualifiedName;

        public ErrorCodeEnumItemInfo()
        {
        }

        public string ClassKey
        {
            get
            {
                return this.classKey;
            }

            set
            {
                this.classKey = value;
            }
        }

        public string MemberKey
        {
            get
            {
                return this.memberKey;
            }

            set
            {
                this.memberKey = value;
            }
        }

        public string FullyQualifiedName
        {
            get
            {
                return this.fullyQualifiedName;
            }

            set
            {
                this.fullyQualifiedName = value;
            }
        }
    }
}
