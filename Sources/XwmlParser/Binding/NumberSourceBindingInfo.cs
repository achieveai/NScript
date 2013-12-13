//-----------------------------------------------------------------------
// <copyright file="IntegerSourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for IntegerSourceBindingInfo
    /// </summary>
    public class NumberSourceBindingInfo : SourceBindingInfo
    {
        internal override Mono.Cecil.TypeReference ValueType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
