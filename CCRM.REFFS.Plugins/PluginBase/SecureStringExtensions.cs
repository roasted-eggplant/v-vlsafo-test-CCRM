// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureStringExtensions.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  SecureStringExtensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REFFS.Plugins
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureStringExtensions
    {
        public static string ToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
            {
                throw new ArgumentNullException("secureString");
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ToSecureString(this string unsecureString)
        {
            if (string.IsNullOrEmpty(unsecureString) || string.IsNullOrWhiteSpace(unsecureString))
            {
                throw new ArgumentNullException("unsecureString");
            }

            return unsecureString.Aggregate(new SecureString(), AppendChar, MakeReadOnly);
        }

        private static SecureString MakeReadOnly(SecureString secureString)
        {
            secureString.MakeReadOnly();
            return secureString;
        }

        private static SecureString AppendChar(SecureString secureString, char chr)
        {
            secureString.AppendChar(chr);
            return secureString;
        }
    }
}
