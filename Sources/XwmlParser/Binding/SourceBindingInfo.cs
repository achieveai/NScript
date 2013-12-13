//-----------------------------------------------------------------------
// <copyright file="SourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for SourceBindingInfo
    /// </summary>
    public abstract class SourceBindingInfo
    {
        internal virtual bool IsStatic
        { get { return true; } }

        internal abstract TypeReference ValueType
        {
            get;
        }

        internal virtual Tuple<IList<string>, IList<IIdentifier>, IIdentifier>
            GenerateGetterSetterInfo(
                SkinCodeGenerator codeGenerator,
                BindingMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
