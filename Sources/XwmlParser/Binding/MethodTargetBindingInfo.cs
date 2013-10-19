//-----------------------------------------------------------------------
// <copyright file="MethodTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MethodTargetBindingInfo
    /// </summary>
    public class MethodTargetBindingInfo : TargetBindingInfo
    {
        public MethodTargetBindingInfo(
            MethodReference method)
            : base(null)
        {
            this.Method = method;
        }

        public MethodReference Method { get; private set; }
    }
}
