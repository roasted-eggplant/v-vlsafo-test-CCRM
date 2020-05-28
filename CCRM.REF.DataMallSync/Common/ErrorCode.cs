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
namespace CCRM.REF.DataMallSync.Common
{
    public enum ErrorCode
    {
        OutOfTheBox = 10000000,

        Unknown = 10000001,        
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
