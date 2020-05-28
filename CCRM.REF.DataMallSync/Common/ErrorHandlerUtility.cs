// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorHandlerUtility.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
// ErrorHandlerUtility class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CCRM.REF.DataMallSync.Common
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Container for ErrorHandler instances
    /// </summary>
    public sealed class ErrorHandlerUtility : Plugin
    {
        private string errorMessage;

        /// <summary>
        /// Public constructor with initializations
        /// </summary>
        /// <param name="localContext"></param>
        public ErrorHandlerUtility(LocalPluginContext localContext) : base(typeof(ErrorHandlerUtility))
        {
            this.LocalContext = localContext;
            this.ErrorStack = new Stack();
            this.ErrorHeader = new ErrorLogHeader();
        }

        /// <summary>
        /// Default parameter-less constructor
        /// </summary>
        private ErrorHandlerUtility() : base(typeof(ErrorHandlerUtility))
        {
            ////make the default constructor private to hide it.
        }

        /// <summary>
        /// Container for Stack Trace Methods
        /// </summary>
        private Stack ErrorStack { get; set; }

        /// <summary>
        /// Header Information
        /// </summary>
        private ErrorLogHeader ErrorHeader { get; set; }

        /// <summary>
        /// Invokes all error handlers registered to the Utility
        /// </summary>
        public void ExecuteErrorHandlers<T>(T exception) where T : Exception
        {
            if (this.ErrorHeader.ErrorCode == ErrorCode.OutOfTheBox)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, (int)this.ErrorHeader.ErrorCode, this.ErrorHeader.ErrorMessage);
            }
            else
            {
                if (this.ErrorStack.Count == 0)
                {
                    if (typeof(T) != typeof(InvalidPluginExecutionException))
                    {
                        throw new InvalidPluginExecutionException(OperationStatus.Failed, (int)ErrorCode.Unknown, exception.Message);
                    }
                    else
                    {
                        throw exception;
                    }
                }
                else
                {
#if RELEASE
                    this.ErrorMessage = this.ErrorHeader.SupportInformation;
                    LocalContext.Trace(BuildDetailedErrorMessage().Replace(@"<br/>", string.Empty).Replace(@"<BR/>", string.Empty));
#else
                    this.errorMessage = this.BuildDetailedErrorMessage().Replace(@"<br/>", string.Empty).Replace(@"<BR/>", string.Empty);
#endif

                    throw new InvalidPluginExecutionException(OperationStatus.Failed, (int)this.ErrorHeader.ErrorCode, this.errorMessage);
                }
            }
        }

        /// <summary>
        /// Create detailed error message
        /// </summary>
        /// <returns></returns>
        private string BuildDetailedErrorMessage()
        {
            try
            {
                StringBuilder detailedErrorMessage = new StringBuilder();
                detailedErrorMessage.Append(string.Format("{0}{0}====ERROR LOG====", Environment.NewLine));
                detailedErrorMessage.Append(string.Format("{0}{1}", Environment.NewLine, this.ErrorHeader.SupportInformation));
                detailedErrorMessage.Append(string.Format("{0}----INFORMATION----", Environment.NewLine));
                detailedErrorMessage.Append(string.Format("{0}ORGANIZATION: {1}", Environment.NewLine, this.ErrorHeader.OrganizationName));
                detailedErrorMessage.Append(string.Format("{0}INITIATING USER: {1}", Environment.NewLine, this.ErrorHeader.InitiatingUser));
                detailedErrorMessage.Append(string.Format("{0}ENTITY: {1}", Environment.NewLine, this.ErrorHeader.Entity));
                detailedErrorMessage.Append(string.Format("{0}DEPTH: {1}", Environment.NewLine, this.ErrorHeader.Depth));
                detailedErrorMessage.Append(string.Format("{0}PLUGIN MESSAGE: {1}", Environment.NewLine, this.ErrorHeader.Message));
                detailedErrorMessage.Append(string.Format("{0}STAGE: {1}", Environment.NewLine, this.ErrorHeader.Stage));
                detailedErrorMessage.Append(string.Format("{0}EXCEPTION TYPE: {1}", Environment.NewLine, this.ErrorHeader.ExceptionType));
                detailedErrorMessage.Append(string.Format("{0}SOURCE: {1}", Environment.NewLine, this.ErrorHeader.ErrorSource));
                detailedErrorMessage.Append(string.Format("{0}ERROR CODE: {1}", Environment.NewLine, (int)this.ErrorHeader.ErrorCode));
                detailedErrorMessage.Append(string.Format("{0}OCCURENCE: {1}", Environment.NewLine, this.ErrorHeader.Occurence));
                detailedErrorMessage.Append(string.Format("{0}MESSAGE: {1}", Environment.NewLine, this.ErrorHeader.ErrorMessage));
                detailedErrorMessage.Append(string.Format("{0}----STACK TRACE----{0}", Environment.NewLine));

                while (this.ErrorStack.Count > 0)
                {
                    StackMethodEntry stackMethodEntry = (StackMethodEntry)this.ErrorStack.Pop();
                    if (stackMethodEntry != null)
                    {
                        detailedErrorMessage.Append(string.Format("{0}Source: {1}", Environment.NewLine, stackMethodEntry.ErrorSource));
                        detailedErrorMessage.Append(string.Format("{0}Error Code: {1}", Environment.NewLine, (int)stackMethodEntry.MethodErrorCode));
                        detailedErrorMessage.Append(string.Format("{0}Message: {1}{0}", Environment.NewLine, stackMethodEntry.ErrorMessage));
                    }
                }

                detailedErrorMessage.Append(string.Format("{0}{0}", Environment.NewLine));
                return detailedErrorMessage.ToString();
            }
            catch (InvalidPluginExecutionException error)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.BuildDetailedErrorMessage. {0}", error.Message));
            }
            catch (Exception error)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.BuildDetailedErrorMessage. {0}", error.Message));
            }
        }        
    }
}
