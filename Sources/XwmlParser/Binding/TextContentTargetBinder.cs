//-----------------------------------------------------------------------
// <copyright file="TextContentTargetBinder.cs" company="">
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
    /// Definition for TextContentTargetBinder
    /// </summary>
    public class TextContentTargetBinder : TargetBindingInfo
    {
        public TextContentTargetBinder(
            TypeReference stringType)
            : base(stringType)
        {
        }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            IIdentifier textContentSetter = codeGenerator.CodeGenerator.GetTextContentSetter();
            return Tuple.Create(
                (string)null,
                textContentSetter,
                (IIdentifier)null,
                (Expression)null,
                (Expression)new StringLiteralExpression(codeGenerator.Scope, string.Empty));
        }
    }
}
