//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="Throw.cs" company="Microsoft">
////   Copyright (C) Microsoft.  All rights reserved.
////   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
////   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
////   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
////   PARTICULAR PURPOSE.
//// </copyright>
////
//// <summary>
////   Exception Helper class
//// </summary>
//// --------------------------------------------------------------------------------------------------------------------
////namespace CCRM.REFFS.Plugins.Common
////{
////    using System;

////    /// <summary>
////    /// Helpers for exceptions
////    /// </summary>
////    public static class Throw
////    {
////        /// <summary>
////        /// Throws Null Reference Exception
////        /// </summary>
////        /// <param name="test">test object</param>
////        /// <param name="msg">message to throw</param>
////        public static void NullReference(object test, string msg)
////        {
////            if (test == null)
////            {
////                throw new NullReferenceException(msg);
////            }
////        }

////        /// <summary>
////        /// Throws Null Reference Exception
////        /// </summary>
////        /// <param name="test">test object</param>
////        /// <param name="msg">message to throw</param>
////        public static void NullReference(string test, string msg)
////        {
////            if (string.IsNullOrWhiteSpace(test) == true
////                || string.IsNullOrEmpty(test) == true)
////            {
////                throw new NullReferenceException(msg);
////            }
////        }

////        /// <summary>
////        /// Throws Null Reference Exception
////        /// </summary>
////        /// <param name="test">test object</param>
////        /// <param name="msg">message to throw</param>
////        public static void NullReference(Guid test, string msg)
////        {
////            if (Guid.Empty == test)
////            {
////                throw new NullReferenceException(msg);
////            }
////        }

////        /// <summary>
////        /// Throws Argument Null Exception
////        /// </summary>
////        /// <param name="test">test object</param>
////        /// <param name="msg">message to throw</param>
////        public static void ArgumentNull(object test, string msg)
////        {
////            if (test == null)
////            {
////                throw new ArgumentNullException(msg);
////            }
////        }

////        /// <summary>
////        /// Throws Argument Null Exception
////        /// </summary>
////        /// <param name="test">test object</param>
////        /// <param name="msg">message to throw</param>
////        public static void ArgumentNull(string test, string msg)
////        {
////            if (string.IsNullOrWhiteSpace(test) == true
////                || string.IsNullOrEmpty(test) == true)
////            {
////                throw new ArgumentNullException(msg);
////            }
////        }

////        /// <summary>
////        /// Throws Argument Null Exception
////        /// </summary>
////        /// <param name="test">test object</param>
////        /// <param name="msg">message to throw</param>
////        public static void ArgumentNull(Guid test, string msg)
////        {
////            if (Guid.Empty == test)
////            {
////                throw new ArgumentNullException(msg);
////            }
////        }
////    }
////}
