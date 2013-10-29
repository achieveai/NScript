//-----------------------------------------------------------------------
// <copyright file="FieldTargetBindingInfocs.cs" company="">
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
    /// Definition for FieldTargetBindingInfocs
    /// </summary>
    public class FieldTargetBindingInfo : TargetBindingInfo
    {
        public FieldTargetBindingInfo(
            FieldReference field)
            : base(field.FieldType)
        {
            this.Field = field;
        }

        public FieldReference Field
        { get; private set; }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            IIdentifier setter = codeGenerator.CodeGenerator.GetSetterForProperty(
                this.Field);
            IIdentifier getter = null;
            string name = null;

            return Tuple.Create(
                name,
                setter,
                getter,
                (Expression)null,
                (Expression)NScript.Converter.ExpressionsConverter.DefaultValueConverter.GetDefaultValue(
                    codeGenerator.CodeGenerator.Resolver,
                    codeGenerator.CodeGenerator.ScopeManager,
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    this.Field.FieldType));
        }
    }
}
