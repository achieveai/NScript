//-----------------------------------------------------------------------
// <copyright file="StaticSourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for StaticSourceBindingInfo
    /// </summary>
    public class StaticSourceBindingInfo : PropertySourceBindingInfo
    {
        public StaticSourceBindingInfo(
            TypeReference staticType,
            List<MemberReference> propertyReferences)
            : base(staticType, propertyReferences)
        {
        }
    }
}
