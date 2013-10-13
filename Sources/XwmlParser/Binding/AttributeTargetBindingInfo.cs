//-----------------------------------------------------------------------
// <copyright file="AttributeTargetBindingInfo.cs" company="">
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
    /// Definition for AttributeTargetBindingInfo
    /// </summary>
    public class AttributeTargetBindingInfo : TargetBindingInfo
    {
        private string attributeName;

        public AttributeTargetBindingInfo(
            TypeReference stringType,
            string attributeName)
            : base(stringType)
        {
            this.attributeName = attributeName;
        }

        public string AttributeName
        { get { return this.attributeName; } }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            IIdentifier attributeSetter = codeGenerator.CodeGenerator.GetAttributeSetter();
            return Tuple.Create(
                (string)null,
                attributeSetter,
                (IIdentifier)null,
                (Expression)new StringLiteralExpression(
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    this.AttributeName),
                (Expression)null);
        }
    }
}
