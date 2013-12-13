//-----------------------------------------------------------------------
// <copyright file="EnumSourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for EnumSourceBindingInfo
    /// </summary>
    public class EnumSourceBindingInfo : SourceBindingInfo
    {
        internal override Mono.Cecil.TypeReference ValueType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
