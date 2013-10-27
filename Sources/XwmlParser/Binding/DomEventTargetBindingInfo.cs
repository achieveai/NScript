//-----------------------------------------------------------------------
// <copyright file="DomEventTargetBindingInfo.cs" company="">
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
    /// Definition for DomEventTargetBindingInfo
    /// </summary>
    public class DomEventTargetBindingInfo : TargetBindingInfo
    {
        public DomEventTargetBindingInfo(
            TypeReference bindingType,
            string eventName)
            : base(bindingType)
        {
            this.EventName = eventName;
        }

        public string EventName { get; private set; }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            return new Tuple<string, IIdentifier, IIdentifier, Expression, Expression>(
                null,
                null,
                null,
                new StringLiteralExpression(
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    this.EventName),
                new NullLiteralExpression(codeGenerator.Scope));
        }
    }
}
