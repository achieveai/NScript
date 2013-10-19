//-----------------------------------------------------------------------
// <copyright file="CssClassTargetBindingInfo.cs" company="">
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
    /// Definition for CssClassTargetBindingInfo
    /// </summary>
    public class CssClassTargetBindingInfo : TargetBindingInfo
    {
        public CssClassTargetBindingInfo(
            TypeReference booleanType,
            IIdentifier className)
            : base(booleanType)
        {
            this.ClassName = className;
        }

        public IIdentifier ClassName
        { get; private set; }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            return new Tuple<string, IIdentifier, IIdentifier, Expression, Expression>(
                null,
                codeGenerator.CodeGenerator.GetCssClassSetter(),
                null,
                new IdentifierStringExpression(
                    null,
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    new IdentifierExpression(
                        this.ClassName,
                        codeGenerator.CodeGenerator.ScopeManager.Scope)),
                new BooleanLiteralExpression(codeGenerator.Scope, false));
        }
    }
}
