//-----------------------------------------------------------------------
// <copyright file="MethodTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MethodTargetBindingInfo
    /// </summary>
    public class MethodTargetBindingInfo : TargetBindingInfo
    {
        public override Mono.Cecil.TypeReference BindingType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
