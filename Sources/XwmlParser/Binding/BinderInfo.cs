//-----------------------------------------------------------------------
// <copyright file="BinderInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using XwmlParser.Binding;
    using System.Linq;
    using NScript.Converter.ExpressionsConverter;

    public enum SourceType
    {
        Static,
        Parent,
        DataContext,
        NamedPart
    }

    [Flags]
    public enum BinderType
    {
        DataContext = 0x1,
        Static = 0x2,
        TemplateParent = 0x3,
        SourceType = 0xf,
        PropertyBinder = 0x10,
        AttachedPropertyBinder = 0x20,
        EventBinder = 0x30,
        DomEventBinder = 0x40,
        CssBinder = 0x50,
        StyleBinder = 0x60,
        AttributeBinder = 0x70,
        BinderTypeMask = 0xf0
    }

    /// <summary>
    /// Definition for BinderInfo
    /// </summary>
    public class BinderInfo
    {
        public BinderInfo(
            TargetBindingInfo targetBindingInfo,
            SourceBindingInfo sourceBindingInfo,
            ConverterInfo converterInfo,
            BindingMode mode,
            SourceType sourceType,
            string namedPartName)
        {
            this.TargetBindingInfo = targetBindingInfo;
            this.SourceBindingInfo = sourceBindingInfo;
            this.ConverterInfo = converterInfo;
            this.SourceType = sourceType;
            this.NamedPartName = namedPartName;
            this.Mode = mode;
        }

        public BindingMode Mode { get; private set; }

        public TargetBindingInfo TargetBindingInfo { get; private set; }

        public SourceBindingInfo SourceBindingInfo { get; private set; }

        public ConverterInfo ConverterInfo { get; private set; }

        public SourceType SourceType { get; private set; }

        public string NamedPartName { get; private set; }

        internal Expression
            GenerateCode(
                int objectIndex,
                SkinCodeGenerator codeGenerator,
                NodeInfos.NodeInfo nodeInfo)
        {
            Tuple<IList<string>, IList<IIdentifier>, IIdentifier> sourceInfo
                = this.SourceBindingInfo.GenerateGetterSetterInfo(
                    codeGenerator,
                    this.Mode);

            Tuple<string, IIdentifier, IIdentifier, Expression, Expression> targetInfo
                = this.TargetBindingInfo.GenerateGetterSetter(
                    codeGenerator, this.Mode == BindingMode.TwoWay);

            Tuple<IIdentifier, IIdentifier> converterInfo = null;
            if (this.ConverterInfo != null)
            {
                converterInfo = this.ConverterInfo.GetToAndFromMethods(
                    codeGenerator.CodeGenerator);
            }

            return new MethodCallExpression(
                null,
                codeGenerator.Scope,
                IdentifierExpression.Create(
                    null,
                    codeGenerator.Scope,
                    codeGenerator.CodeGenerator.Resolver.ResolveFactory(
                        this.GetBinderInfoCtor(
                            codeGenerator,
                            targetInfo))),
                this.GetCtorArgs(
                    objectIndex,
                    codeGenerator,
                    sourceInfo,
                    targetInfo,
                    converterInfo));
        }

        private MethodReference GetBinderInfoCtor(
            SkinCodeGenerator codeGenerator,
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression> targetInfo)
        {
            if (this.Mode == BindingMode.OneTime)
            {
                return targetInfo.Item4 != null
                    ? codeGenerator.CodeGenerator.KnownTypes.SkinBinderCtorOneTime2
                    : codeGenerator.CodeGenerator.KnownTypes.SkinBinderCtorOneTime1;
            }
            else if (this.Mode == BindingMode.OneWay)
            {
                return targetInfo.Item4 != null
                    ? codeGenerator.CodeGenerator.KnownTypes.SkinBinderCtorOneWay2
                    : codeGenerator.CodeGenerator.KnownTypes.SkinBinderCtorOneWay1;
            }
            else
            {
                return codeGenerator.CodeGenerator.KnownTypes.SkinBinderCtorTwoWay;
            }
        }

        private List<Expression> GetCtorArgs(
            int objectIndex,
            SkinCodeGenerator codeGenerator,
            Tuple<IList<string>, IList<IIdentifier>, IIdentifier> sourceInfo,
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression> targetInfo,
            Tuple<IIdentifier, IIdentifier> converterInfo)
        {
            List<Expression> args = new List<Expression>();
            List<Expression> tmpExprArray = new List<Expression>();
            BinderType binderType;
            int binderIndex = -1;
            foreach (var identifier in sourceInfo.Item2)
            {
                tmpExprArray.Add(new IdentifierExpression(
                    identifier,
                    codeGenerator.Scope));
            }

            // Add Getter path.
            args.Add(new InlineNewArrayInitialization(
                null,
                codeGenerator.Scope,
                tmpExprArray));

            // Add PropertyNames if live binding required.
            if (this.Mode != BindingMode.OneTime)
            {
                tmpExprArray = new List<Expression>();
                foreach (var str in sourceInfo.Item1)
                {
                    tmpExprArray.Add(new StringLiteralExpression(
                        codeGenerator.Scope,
                        str));
                }

                args.Add(new InlineNewArrayInitialization(
                    null,
                    codeGenerator.Scope,
                    tmpExprArray));

                binderIndex = codeGenerator.GetBinderIndex(this);
            }

            // Target setter.
            if (targetInfo.Item2 != null)
            {
                args.Add(new IdentifierExpression(
                    targetInfo.Item2,
                    codeGenerator.Scope));
            }
            else
            {
                args.Add(new NullLiteralExpression(codeGenerator.Scope));
            }

            // Target setter argument.
            if (targetInfo.Item4 != null)
            {
                args.Add(targetInfo.Item4);
            }

            if (this.Mode == BindingMode.TwoWay)
            {
                // Target Property Getter.
                args.Add(
                    new IdentifierExpression(
                        targetInfo.Item3,
                        codeGenerator.Scope));

                // Target Property Name.
                args.Add(
                    new StringLiteralExpression(
                        codeGenerator.Scope,
                        targetInfo.Item1));
            }

            // BinderType, should use enum value.
            binderType = this.GetBinderType();
            args.Add(new NumberLiteralExpression(
                codeGenerator.Scope,
                (int)binderType));

            // Object Index.
            args.Add(new NumberLiteralExpression(
                codeGenerator.Scope,
                objectIndex));

            if (binderIndex >= 0)
            {
                // Binder Index.
                args.Add(new NumberLiteralExpression(
                    codeGenerator.Scope,
                    binderIndex));
            }

            // Forward converter.
            if (converterInfo != null
                && converterInfo.Item1 != null)
            {
                args.Add(new IdentifierExpression(
                    converterInfo.Item1,
                    codeGenerator.Scope));
            }
            else
            {
                args.Add(new NullLiteralExpression(codeGenerator.Scope));
            }

            // Back converter.
            if (this.Mode == BindingMode.TwoWay)
            {
                if (converterInfo != null
                    && converterInfo.Item2 != null)
                {
                    args.Add(new IdentifierExpression(
                        converterInfo.Item1,
                        codeGenerator.Scope));
                }
                else
                {
                    args.Add(new NullLiteralExpression(codeGenerator.Scope));
                }
            }

            // Default Value
            args.Add(targetInfo.Item5);

            if ((binderType & BinderType.BinderTypeMask) == BinderType.EventBinder
                || (binderType & BinderType.BinderTypeMask) == BinderType.DomEventBinder)
            {
                int extraObjectIndex = codeGenerator.GetExtraObjectIndex(this);
                args.Add(new NumberLiteralExpression(
                    codeGenerator.Scope,
                    extraObjectIndex));
            }
            else if (targetInfo.Item4 != null)
            {
                args.Add(new NumberLiteralExpression(
                    codeGenerator.Scope,
                    0));
            }

            return args;
        }

        private BinderType GetBinderType()
        {
            BinderType rv = (BinderType)0;
            switch (this.SourceType)
            {
                case SourceType.Static:
                    rv = BinderType.Static;
                    break;
                case SourceType.Parent:
                    rv = BinderType.TemplateParent;
                    break;
                case SourceType.DataContext:
                    rv = BinderType.DataContext;
                    break;
                case SourceType.NamedPart:
                default:
                    throw new NotSupportedException();
            }

            if (this.TargetBindingInfo is PropertyTargetBindingInfo)
            {
                rv |= BinderType.PropertyBinder;
            }
            else if (this.TargetBindingInfo is CssClassTargetBindingInfo)
            {
                rv |= BinderType.CssBinder;
            }
            else if (this.TargetBindingInfo is AttributeTargetBindingInfo)
            {
                rv |= BinderType.AttributeBinder;
            }
            else if (this.TargetBindingInfo is StyleTargetBindingInfo)
            {
                rv |= BinderType.StyleBinder;
            }
            else if (this.TargetBindingInfo is AttachedPropertyTargetBindingInfo)
            {
                rv |= BinderType.AttachedPropertyBinder;
            }
            else if (this.TargetBindingInfo is EventTargetBindingInfo)
            {
                rv |= BinderType.EventBinder;
            }
            else if (this.TargetBindingInfo is DomEventTargetBindingInfo)
            {
                rv |= BinderType.DomEventBinder;
            }

            return rv;
        }
    }
}
