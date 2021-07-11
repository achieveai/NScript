//-----------------------------------------------------------------------
// <copyright file="StyleTargetBindingInfo.cs" company="">
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
    /// Definition for StyleTargetBindingInfo
    /// </summary>
    public class StyleTargetBindingInfo : TargetBindingInfo
    {
        public StyleTargetBindingInfo(
            TypeReference targetType,
            string styleName)
            : base(targetType)
        {
            this.StyleName = styleName;
        }

        public string StyleName
        { get; private set; }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            return new Tuple<string, IIdentifier, IIdentifier, Expression, Expression>(
                null,
                codeGenerator.CodeGenerator.GetStyleSetter(this.StyleName),
                null,
                null,
                new NullLiteralExpression(codeGenerator.Scope));
        }
    }
}
