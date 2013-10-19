//-----------------------------------------------------------------------
// <copyright file="PropertyTargetBindingInfo.cs" company="">
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
    /// Definition for PropertyTargetBindingInfo
    /// </summary>
    public class PropertyTargetBindingInfo : TargetBindingInfo
    {
        public PropertyTargetBindingInfo(
            PropertyReference property)
            : base(property.PropertyType)
        {
            this.Property = property;
        }

        public PropertyReference Property
        { get; private set; }

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            IIdentifier setter = codeGenerator.CodeGenerator.GetSetterForProperty(
                this.Property);
            IIdentifier getter = null;
            string name = null;

            if (isTwoWay)
            {
                getter = codeGenerator.CodeGenerator.GetGetterForPropertyPath(
                    new MemberReference[] { this.Property });
                name = this.Property.Name;
            }

            return Tuple.Create(
                name,
                setter,
                getter,
                (Expression)null,
                (Expression)NScript.Converter.ExpressionsConverter.DefaultValueConverter.GetDefaultValue(
                    codeGenerator.CodeGenerator.Resolver,
                    codeGenerator.CodeGenerator.ScopeManager,
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    this.Property.PropertyType));
        }
    }
}
