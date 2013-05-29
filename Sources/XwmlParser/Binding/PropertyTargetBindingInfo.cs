//-----------------------------------------------------------------------
// <copyright file="PropertyTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for PropertyTargetBindingInfo
    /// </summary>
    public class PropertyTargetBindingInfo : TargetBindingInfo
    {
        public override Mono.Cecil.TypeReference BindingType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
