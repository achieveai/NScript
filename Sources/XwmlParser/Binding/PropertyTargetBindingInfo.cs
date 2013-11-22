//-----------------------------------------------------------------------
// <copyright file="PropertyTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
    using NScript.CLR;
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

        public object DefaultValue
        {
            get;
            private set;
        }

        internal override BindingMode GetBindingMode(
            BindingMode specifiedMode,
            KnownTemplateTypes knownTypes)
        {
            var attr = this.Property.Resolve().CustomAttributes.SelectAttribute(knownTypes.DefaultDataBindingAttribute);

            // [DefaultDataBinding(DefaultValue=11, Mode=DataBindingMode.TwoWay, IsStrict=false)]
            if (attr != null)
            {
                BindingMode mode = specifiedMode;
                bool isStrict = false;

                foreach (var item in attr.Properties)
                {
                    CustomAttributeArgument customArgument = item.Argument;

                    if (customArgument.Value is CustomAttributeArgument)
                    {
                        customArgument = (CustomAttributeArgument)customArgument.Value;
                    }

                    switch (item.Name)
                    {
                        case "IsStrict":
                            isStrict = (bool)customArgument.Value;
                            break;
                        case "Mode":
                            int value = (int)customArgument.Value;
                            if (value == 0)
                            {
                                mode = BindingMode.OneTime;
                            } else if (value == 1)
                            {
                                mode = BindingMode.OneWay;
                            }
                            else
                            {
                                mode = BindingMode.TwoWay;
                            }
                            break;
                        case "DefaultValue":
                            if (!customArgument.Type.ExtendsType(this.Property.PropertyType))
                            {
                                throw new ApplicationException(
                                    string.Format(
                                        "Specified default type:'{0}' can't be assigned to given property type '{1}'",
                                        customArgument.Type,
                                        this.Property.PropertyType));
                            }
                            else if (customArgument.Type.IsIntegerOrEnum()
                                || customArgument.Type.IsSame(knownTypes.ClrKnownReference.String)
                                || customArgument.Type.IsBoolean())
                            {
                                this.DefaultValue = customArgument.Value;
                            }
                            else
                            {
                                throw new ApplicationException(
                                    string.Format(
                                        "Only string, numbers, enums and bools can be set as default value. Default value type: {0}",
                                        customArgument.Type));
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (isStrict
                    && specifiedMode != BindingMode.Default
                    && specifiedMode != mode)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Specified binding mode {0} and preffered binding mode {1} do not match and prefference is strict.",
                            specifiedMode,
                            mode));
                }
                else if (specifiedMode != BindingMode.Default)
                {
                    return specifiedMode;
                }
                else if (mode != BindingMode.Default)
                {
                    return mode;
                }
            }

            return base.GetBindingMode(specifiedMode, knownTypes);
        }

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
                this.GetDefaultValueExpression(codeGenerator));
        }

        private Expression GetDefaultValueExpression(SkinCodeGenerator codeGenerator)
        {
            if (this.DefaultValue == null)
            {
                return (Expression)NScript.Converter.ExpressionsConverter.DefaultValueConverter.GetDefaultValue(
                    codeGenerator.CodeGenerator.Resolver,
                    codeGenerator.CodeGenerator.ScopeManager,
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    this.Property.PropertyType);
            }
            if (this.DefaultValue is Boolean)
            {
                return new BooleanLiteralExpression(
                    codeGenerator.Scope,
                    (bool)this.DefaultValue);
            }
            else if (this.DefaultValue is string)
            {
                return new StringLiteralExpression(
                    codeGenerator.Scope,
                    (string)this.DefaultValue);
            }
            else if (this.DefaultValue is double
                || this.DefaultValue is float)
            {
                return new DoubleLiteralExpression(
                    codeGenerator.Scope,
                    Convert.ToDouble(this.DefaultValue));
            }
            else
            {
                return new NumberLiteralExpression(
                    codeGenerator.Scope,
                    Convert.ToInt64(this.DefaultValue));
            }
        }
    }
}
