//-----------------------------------------------------------------------
// <copyright file="TargetBindingInfo.cs" company="">
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
    /// Definition for TargetBindingInfo
    /// </summary>
    public abstract class TargetBindingInfo
    {
        protected TargetBindingInfo(TypeReference bindingType)
        {
            this.BindingType = bindingType;
        }

        public virtual bool CanHaveDynamicBinding
        { get { return false; } }

        public TypeReference BindingType
        { get; private set; }

        internal virtual
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            throw new NotImplementedException();
        }
    }
}
