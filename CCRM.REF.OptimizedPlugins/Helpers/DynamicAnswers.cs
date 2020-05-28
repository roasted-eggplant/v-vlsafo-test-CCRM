// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAnswers.cs" company="Microsoft">
//   Copyright (C) Microsoft.  All rights reserved.
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//   PARTICULAR PURPOSE.
// </copyright>
//
// <summary>
//  DynamicAnswers Plugin
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CCRM.REF.OptimizedPlugins.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class DynamicAnswers
    {
        [DataMember(EmitDefaultValue = false)]
        public List<Answer> data { get; set; }
    }

    [DataContract]
    public class Answer
    {
        [DataMember(EmitDefaultValue = false)]
        public Guid id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string answer { get; set; }
    }
}
