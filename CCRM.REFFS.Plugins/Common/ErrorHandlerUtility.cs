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
namespace CCRM.REFFS.Plugins.Common
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
        /// Adds exception details to the ErrorStack
        /// Also adds details to the ErrorHeader if need be
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="exception"></param>
        /// 
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        ////public InvalidPluginExecutionException ThrowException<T1, T2>(T1 exception, T2 memberInfo)
        ////    where T1 : Exception
        ////    where T2 : MemberInfo
        ////{
        ////    try
        ////    {
        ////        if (this.ErrorHeader.ErrorCode == ErrorCode.OutOfTheBox)
        ////        {
        ////            return new InvalidPluginExecutionException(OperationStatus.Failed, (int)this.ErrorHeader.ErrorCode, this.ErrorHeader.ErrorMessage);
        ////        }

        ////        ErrorCode errorCode = this.GetErrorCode(memberInfo);
        ////        StackTrace stackTrace = new StackTrace();
        ////        if (exception != null)
        ////        {
        ////            if (this.ErrorStack.Count == 0)
        ////            {
        ////                this.BuildErrorHeader<T1, T2>(exception, errorCode, memberInfo);

        ////                this.ErrorHeader.SupportInformation = this.GetSupportInformation((int)errorCode);
        ////            }

        ////            this.ErrorStack.Push(new StackMethodEntry
        ////            {
        ////                ErrorMessage = exception.Message,
        ////                MethodErrorCode = errorCode,
        ////                ErrorSource = string.Format("{0}.{1}", memberInfo.DeclaringType.FullName, memberInfo.Name)
        ////            });

        ////            if (exception.GetType() == typeof(InvalidPluginExecutionException)
        ////                && ((InvalidPluginExecutionException)((Exception)exception)).ErrorCode == (int)ErrorCode.OutOfTheBox)
        ////            {
        ////                return (InvalidPluginExecutionException)((Exception)exception);
        ////            }
        ////            else if (errorCode == ErrorCode.Unknown)
        ////            {
        ////                return new InvalidPluginExecutionException(OperationStatus.Failed, (int)ErrorCode.Unknown, "Unknown Error");
        ////            }
        ////        }

        ////        return new InvalidPluginExecutionException();
        ////    }
        ////    catch (InvalidPluginExecutionException error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.ThrowException. {0}", error.Message));
        ////    }
        ////    catch (Exception error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.ThrowException. {0}", error.Message));
        ////    }
        ////}

        /// <summary>
        /// Throws an out of the box Business Process Error
        /// </summary>
        /// <param name="message">String to be prompted to the user</param>
        ////public void ShowBusinessProcessError(string message)
        ////{
        ////    this.ErrorHeader.ErrorCode = ErrorCode.OutOfTheBox;
        ////    this.ErrorHeader.ErrorMessage = this.StringWrap(message);
        ////    throw new InvalidPluginExecutionException(OperationStatus.Failed, (int)ErrorCode.OutOfTheBox, this.StringWrap(message));
        ////}

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
        /// Builds the Support Information for the Error Logs
        /// </summary>
        /// 
        /// <param name="stringParams"></param>
        /// <returns></returns>
        ////private string GetSupportInformation(params int[] stringParams)
        ////{
        ////    try
        ////    {
        ////        string message = string.Empty;
        ////        string errorNumberStr = string.Empty;

        ////        string supportInfoPath = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(delegate(string s)
        ////        {
        ////            if (s.Contains("SupportInformation.txt"))
        ////            {
        ////                return true;
        ////            }

        ////            return false;
        ////        }).Single();

        ////        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(supportInfoPath))
        ////        {
        ////            using (StreamReader reader = new StreamReader(stream))
        ////            {
        ////                message = reader.ReadToEnd();
        ////            }
        ////        }

        ////        if (stringParams.Count() > 1)
        ////        {
        ////            errorNumberStr = string.Join(", ", stringParams);
        ////        }
        ////        else if (stringParams.Count() == 1)
        ////        {
        ////            errorNumberStr = stringParams[0].ToString();
        ////        }

        ////        message = message.ToString();
        ////        message = string.Format(message, errorNumberStr);

        ////        return message;
        ////    }
        ////    catch (InvalidPluginExecutionException error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.GetSupportInformation. {0}", error.Message));
        ////    }
        ////    catch (Exception error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.GetSupportInformation. {0}", error.Message));
        ////    }
        ////}

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

        /// <summary>
        /// Gets the error code given the method base
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        ////private ErrorCode GetErrorCode<T>(T methodBase)
        ////   where T : MemberInfo
        ////{
        ////    try
        ////    {
        ////        string memberKey = string.Empty;
        ////        string classKey = string.Empty;
        ////        string uniqueKeyStr = "UniqueKey";
        ////        string classKeyStr = "ClassKey";
        ////        string memberKeyStr = "MemberKey";
        ////        ErrorCode result = ErrorCode.Unknown;

        ////        CustomAttributeData seriesInfoAttribute = (from t in methodBase.DeclaringType.CustomAttributes
        ////                                                   where t.AttributeType.Name.Equals("ErrorCodeClassInfo")
        ////                                                   select t).FirstOrDefault();

        ////        if (seriesInfoAttribute != null)
        ////        {
        ////            classKey = (string)seriesInfoAttribute.NamedArguments.Where(w => w.MemberName.Equals(uniqueKeyStr)).Select(s => s.TypedValue.Value).FirstOrDefault();
        ////        }

        ////        CustomAttributeData classMemberInfoAttribute = (from t in methodBase.CustomAttributes
        ////                                                        where t.AttributeType.Name.Equals("ErrorCodeClassMemberInfo")
        ////                                                        select t).FirstOrDefault();

        ////        if (classMemberInfoAttribute != null)
        ////        {
        ////            memberKey = (string)classMemberInfoAttribute.NamedArguments.Where(w => w.MemberName.Equals(uniqueKeyStr)).Select(s => s.TypedValue.Value).FirstOrDefault();
        ////        }

        ////        Func<IList<CustomAttributeNamedArgument>, bool> func = (attributeNamedArguments) =>
        ////        {
        ////            if (attributeNamedArguments != null)
        ////            {
        ////                bool isClassKeyValid = (from t in attributeNamedArguments
        ////                                        where t.MemberName.Equals(classKeyStr) && t.TypedValue.Value.Equals(classKey)
        ////                                        select t.MemberInfo).FirstOrDefault() != null;

        ////                bool isMemberKeyValid = (from t in attributeNamedArguments
        ////                                         where t.MemberName.Equals(memberKeyStr) && t.TypedValue.Value.Equals(memberKey)
        ////                                         select t.MemberInfo).FirstOrDefault() != null;

        ////                return isClassKeyValid && isMemberKeyValid;
        ////            }
        ////            else
        ////            {
        ////                return false;
        ////            }
        ////        };

        ////        FieldInfo fieldInfo = (from t in typeof(ErrorCode).GetFields()
        ////                               where t.IsLiteral &&
        ////                               func.Invoke(t.CustomAttributes.Where(w => w.AttributeType.Name.Equals("ErrorCodeEnumItemInfo")).Select(s => s.NamedArguments).FirstOrDefault())
        ////                               select t).FirstOrDefault();

        ////        if (fieldInfo != null)
        ////        {
        ////            result = ((int)fieldInfo.GetRawConstantValue()).ToEnum<ErrorCode>();
        ////        }

        ////        return result;
        ////    }
        ////    catch (InvalidPluginExecutionException error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.GetErrorCode. {0}", error.Message));
        ////    }
        ////    catch (Exception error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.GetErrorCode. {0}", error.Message));
        ////    }
        ////}

        ///// <summary>
        ///// Assigns value to the Error Header
        ///// </summary>
        ///// <typeparam name="T1"></typeparam>
        ///// <param name="exception"></param>
        ///// <param name="errorCode"></param>
        ///// 
        ///// <param name="memberInfo"></param>
        ///// <returns></returns>
        ////private void BuildErrorHeader<T1, T2>(T1 exception, ErrorCode errorCode, T2 memberInfo)
        ////    where T1 : Exception
        ////    where T2 : MemberInfo
        ////{
        ////    try
        ////    {
        ////        this.ErrorHeader.OrganizationName = this.LocalContext.PluginExecutionContext.OrganizationName;
        ////        this.ErrorHeader.InitiatingUser = this.LocalContext.PluginExecutionContext.InitiatingUserId.ToString();
        ////        this.ErrorHeader.ExceptionType = exception.GetType().ToString();
        ////        this.ErrorHeader.ErrorSource = string.Format("{0}.{1}", memberInfo.DeclaringType.FullName, memberInfo.Name);
        ////        this.ErrorHeader.ErrorCode = errorCode;
        ////        this.ErrorHeader.ErrorMessage = exception.Message;
        ////        this.ErrorHeader.Occurence = DateTime.UtcNow.ToString();
        ////        this.ErrorHeader.Depth = LocalContext.PluginExecutionContext.Depth;
        ////        this.ErrorHeader.Message = LocalContext.PluginExecutionContext.MessageName;
        ////        this.ErrorHeader.Stage = LocalContext.PluginExecutionContext.Stage.ToString();
        ////        this.ErrorHeader.Entity = LocalContext.PluginExecutionContext.PrimaryEntityName;
        ////    }
        ////    catch (InvalidPluginExecutionException error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.BuildErrorHeader. {0}", error.Message));
        ////    }
        ////    catch (Exception error)
        ////    {
        ////        throw new InvalidPluginExecutionException(OperationStatus.Failed, string.Format("Please check ErrorHandler.BuildErrorHeader. {0}", error.Message));
        ////    }
        ////}

        ///// <summary>
        ///// This method formats the message to be displayed in the business process error
        ///// </summary>
        ///// <param name="stringToWrap"></param>
        ///// <param name="stringLimit"></param>
        ///// <param name="wrapIt"></param>
        ///// <returns></returns>
        ////private string StringWrap(string stringToWrap, int stringLimit = 70)
        ////{
        ////    string[] words = stringToWrap.Split(' ');

        ////    StringBuilder wrappedString = new StringBuilder();

        ////    StringBuilder line = new StringBuilder();
        ////    foreach (string word in words)
        ////    {
        ////        if (string.Format("{0}{1}", line.ToString(), word).Length > stringLimit)
        ////        {
        ////            if (string.IsNullOrEmpty(line.ToString()) == false)
        ////            {
        ////                wrappedString.AppendLine(line.ToString());
        ////            }

        ////            line.Clear();
        ////        }

        ////        line.Append(string.Format("{0} ", word));
        ////    }

        ////    if (line.Length > 0)
        ////    {
        ////        wrappedString.AppendLine(line.ToString());
        ////    }

        ////    return wrappedString.ToString();
        ////}
    }
}
