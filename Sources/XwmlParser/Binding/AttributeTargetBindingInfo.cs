//-----------------------------------------------------------------------
// <copyright file="AttributeTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for AttributeTargetBindingInfo
    /// </summary>
    public class AttributeTargetBindingInfo : TargetBindingInfo
    {
        public override Mono.Cecil.TypeReference BindingType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
