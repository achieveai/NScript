//-----------------------------------------------------------------------
// <copyright file="TargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for TargetBindingInfo
    /// </summary>
    public abstract class TargetBindingInfo
    {
        public virtual bool CanHaveDynamicBinding
        { get { return false; } }

        public abstract TypeReference BindingType
        { get; }
    }
}
