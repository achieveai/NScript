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

    /// <summary>
    /// Definition for TypeNodeInfo
    /// </summary>
    public class TypeNodeInfo : NodeInfo
    {
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
        }

        /// <summary>
        /// Parse attribute.
        /// </summary>
        /// <param name="attribute"> The attribute. </param>
        /// <param name="resolver">  The resolver. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public virtual bool ParseAttribute(HtmlAttribute attribute, IResolver resolver)
        {
            return false;
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
    }
}
