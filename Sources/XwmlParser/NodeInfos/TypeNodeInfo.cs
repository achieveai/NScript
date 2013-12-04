//-----------------------------------------------------------------------
// <copyright file="TypeNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using XwmlParser.Binding;
    using NScript.CLR;
    using XwmlParser.StaticValues;
    using System.Linq;
    using NScript.JST;
    using NScript.Converter;
    using NScript.Utils;

    /// <summary>
    /// Definition for TypeNodeInfo
    /// </summary>
    public class TypeNodeInfo : NodeInfo
    {
        private Dictionary<MemberReference, StaticValue> staticInitializers
            = new Dictionary<MemberReference, StaticValue>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">     The type. </param>
        /// <param name="node">     The node. </param>
        /// <param name="tagName">  Name of the tag. </param>
        public TypeNodeInfo(
            TypeReference type,
            HtmlNode node,
            Tuple<string, string> tagName)
            : base(node, tagName)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public TypeReference Type
        { get; private set; }

        /// <summary>
        /// Parse node.
        /// </summary>
        /// <param name="parser"> The parser. </param>
        public override void ParseNode(TemplateParser parser)
        {
            var parserContext = parser.DocumentContext.ParserContext;
            var clrResolver = parserContext.ClrResolver;
            foreach (var attr in this.Node.Attributes)
            {
                if (!this.ParseAttribute(attr, parser))
                {
                    throw new ConverterLocationException(
                        new Location(
                            parser.HtmlParser.ResourceName,
                            attr.Line,
                            attr.LinePosition),
                        string.Format(
                            "Can't resolve {0} for type {1} and {0} is not html attribute",
                            attr.OriginalName,
                            this.Type));
                }
            }
        }

        /// <summary>
        /// Generates a code.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        public override void GenerateCode(SkinCodeGenerator codeGenerator)
        {
            int objectIndex = codeGenerator.GetObjectIndexForNode(this);

            codeGenerator.AddStatement(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        codeGenerator.Scope,
                        new IdentifierExpression(
                            codeGenerator.GetObjectStorageIdentifier(),
                            codeGenerator.Scope),
                        new NumberLiteralExpression(
                            codeGenerator.Scope,
                            objectIndex)),
                    this.GetCtorExpression(codeGenerator)));

            this.GenerateStaticInitializers(
                codeGenerator,
                objectIndex);

            this.GenerateBindingCode(
                objectIndex,
                codeGenerator);
        }

        /// <summary>
        /// Gets target binder information.
        /// </summary>
        /// <param name="fullAttributeName"> Name of the full attribute. </param>
        /// <param name="context">           The context. </param>
        /// <returns>
        /// The target binder information.
        /// </returns>
        public virtual TargetBindingInfo GetTargetBinderInfo(
            Tuple<string, string> fullAttributeName,
            IDocumentContext context)
        {
            if (string.IsNullOrEmpty(fullAttributeName.Item1))
            {
                var propertyReference = context.Resolver.GetPropertyReference(
                    this.Type,
                    fullAttributeName.Item2);
            }

            return null;
        }

        public virtual BinderInfo GetSourceBiningInfo(
            string attributeValue,
            IDocumentContext context,
            TargetBindingInfo targetBinding)
        {
            var binder = BindingParser.ParseBinding(targetBinding, attributeValue, context, null, null);
            if (binder != null)
            {
                return binder;
            }

            TypeDefinition targetTypeDefinition = targetBinding.BindingType.Resolve();
            if (targetTypeDefinition.IsEnum)
            {
            }

            if (targetTypeDefinition.IsIntegerOrEnum())
            {
                return null; // new BinderInfo();
            }
            else if (targetTypeDefinition.IsDouble())
            {
                return null; //new BinderInfo();
            }
            else if (targetTypeDefinition.IsBoolean())
            {
                return null; //new BinderInfo();
            }

            return null;
        }

        protected virtual bool ParseAttribute(
            HtmlAttribute attribute,
            TemplateParser parser)
        {
            try
            {
                var parserContext = parser.DocumentContext.ParserContext;
                var resolver = parserContext.ClrResolver;
                var knownTypes = parserContext.KnownTypes;
                var attrValue = attribute.Value;
                PropertyReference prop = resolver.GetPropertyReference(
                    this.Type,
                    attribute.OriginalName);
                FieldReference field = prop != null
                    ? null
                    : resolver.GetFieldReference(this.Type, attribute.Name);
                MemberReference member = ((MemberReference)prop) ?? field;
                if (prop != null
                    || field != null)
                {
                    if (BindingParser.IsBindingText(attrValue))
                    {
                        this.AddBinder(
                            BindingParser.ParseBinding(
                                prop != null
                                    ? (TargetBindingInfo)new PropertyTargetBindingInfo(prop)
                                    : new FieldTargetBindingInfo(field),
                                attrValue,
                                parser.DocumentContext,
                                parser.DataContextType,
                                parser.ControlType));

                        return true;
                    }
                    else if (TypeNodeInfo.IsCssClassType(prop, field, knownTypes))
                    {
                        this.staticInitializers.Add(
                            ((MemberReference)prop) ?? field,
                            new CssNameValue(
                                parser.DocumentContext,
                                attrValue));
                    }
                    else
                    {
                        this.staticInitializers.Add(
                            member,
                            TypeNodeInfo.GetValue(
                                prop.PropertyType,
                                parserContext,
                                attrValue));
                    }

                    return true;
                }

                EventReference evt = resolver.GetEventReference(this.Type, attribute.Name);
                if (evt != null)
                {
                    if (BindingParser.IsBindingText(attrValue))
                    {
                        this.AddBinder(
                            BindingParser.ParseBinding(
                                new EventTargetBindingInfo(evt),
                                attrValue,
                                parser.DocumentContext,
                                parser.DataContextType,
                                parser.ControlType));

                        return true;
                    }
                }
            }
            catch(ConverterLocationException ex)
            {
                parser.HtmlParser.ParserContext.ConverterContext.AddError(
                    ex.Location,
                    ex.Message,
                    false);
            }
            catch(ApplicationException ex)
            {
                parser.HtmlParser.ParserContext.ConverterContext.AddError(
                    new Location(
                        parser.HtmlParser.ResourceName,
                        attribute.Line,
                        attribute.LinePosition),
                    ex.Message,
                    false);
            }

            return false;
        }

        protected static bool IsCssClassType(
            PropertyReference propertyReference,
            FieldReference fieldReference,
            KnownTemplateTypes knownTypes)
        {
            var customAttributes = propertyReference != null
                ? propertyReference.Resolve().CustomAttributes
                : fieldReference.Resolve().CustomAttributes;

            foreach (var attr in customAttributes)
            {
                if (attr.AttributeType.IsSameDefinition(knownTypes.CssNameAttribute))
                {
                    return true;
                }
            }

            return false;
        }

        protected static StaticValue GetValue(
            TypeReference typeReference,
            ParserContext parserContext,
            string value)
        {
            var knownReferences = parserContext.ConverterContext.ClrKnownReferences;
            if (typeReference.IsEnum())
            {
                return new EnumValue(
                    typeReference,
                    parserContext,
                    value);
            }
            else if (typeReference.IsSame(knownReferences.String))
            {
                return new StringValue(value);
            }
            else if (typeReference.IsSame(knownReferences.Boolean))
            {
                return new BoolValue(value);
            }
            else if (typeReference.IsIntegerOrEnum() || typeReference.IsDouble())
            {
                return new NumberValue(
                    value,
                    typeReference.IsIntegerOrEnum());
            }
            else
            {
                throw new ApplicationException(
                    string.Format("Don't know how to parse type {0}", typeReference));
            }
        }

        protected Expression GetCtorExpression(
            SkinCodeGenerator codeGenerator)
        {
            var ctor = this.GetCtorReference(codeGenerator.CodeGenerator.ParserContext);
            var args = new List<Expression>();
            this.GetCtorArgs(codeGenerator, args);
            return new MethodCallExpression(
                    null,
                    codeGenerator.Scope,
                    IdentifierExpression.Create(
                        null,
                        codeGenerator.Scope,
                        codeGenerator.CodeGenerator.Resolver.ResolveFactory(ctor)),
                    args);
        }

        protected virtual MethodReference GetCtorReference(
            ParserContext parserContext)
        {
            MethodReference rv = null;
            var typeDef = this.Type.Resolve();
            var methods = typeDef.Methods;
            for (int iMethod = 0, methodCount = methods.Count; iMethod < methodCount; iMethod++)
            {
                var method = methods[iMethod];
                if (method.IsStatic
                    || !method.IsConstructor
                    || !method.IsPublic)
                {
                    continue;
                }

                if (!this.IsConstructorMatch(parserContext, method))
                {
                    continue;
                }

                if (rv != null)
                {
                    throw new ApplicationException(
                        string.Format("Too many ctors found for {0}",
                            this.Type));
                }

                rv = method;
            }

            if (rv == null)
            {
                throw new ApplicationException(
                    string.Format("No Suitable constructor found for {0}",
                        this.Type));
            }

            return rv;
        }

        protected virtual bool IsConstructorMatch(
            ParserContext parserContext,
            MethodReference ctor,
            int startParameterIndex = 0)
        {
            return ctor.Parameters.Count <= startParameterIndex;
        }

        protected virtual void GetCtorArgs(
            SkinCodeGenerator codeGenerator,
            List<Expression> args)
        {
        }

        private void GenerateStaticInitializers(
            SkinCodeGenerator codeGenerator,
            int objectIndex)
        {
            foreach (var propValue in this.staticInitializers)
            {
                PropertyReference prop = propValue.Key as PropertyReference;
                CodeGenerator globalCodeGenerator = codeGenerator.CodeGenerator;
                var instanceExpression =
                        new IndexExpression(
                            null,
                            codeGenerator.Scope,
                            new IdentifierExpression(
                                codeGenerator.GetObjectStorageIdentifier(),
                                codeGenerator.Scope),
                            new NumberLiteralExpression(
                                codeGenerator.Scope,
                                objectIndex));
                var valueExpression =
                        propValue.Value.GetInitializationExpression(codeGenerator);
                if (prop != null)
                {
                    codeGenerator.AddStatement(
                        new ExpressionStatement(
                            null,
                            codeGenerator.Scope,
                            CodeGenerator.GetPropertySetterExpression(
                                globalCodeGenerator.ScopeManager,
                                codeGenerator.Scope,
                                prop,
                                globalCodeGenerator.Resolver,
                                instanceExpression,
                                valueExpression)));
                }
                else
                {
                    codeGenerator.AddStatement(
                        new ExpressionStatement(
                            null,
                            codeGenerator.Scope,
                            CodeGenerator.GetFieldSetterExpression(
                                globalCodeGenerator.ScopeManager,
                                codeGenerator.Scope,
                                (FieldReference)propValue.Key,
                                globalCodeGenerator.Resolver,
                                instanceExpression,
                                valueExpression)));
                }
            }
        }
    }
}
