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

        public void ParseNode(TemplateParser parser)
        {
        }

        public virtual bool ParseAttribute(HtmlAttribute attribute, IResolver resolver)
        {
            return false;
        }

        public virtual TargetBindingInfo GetTargetBinderInfo(
            Tuple<string, string> fullAttributeName,
            IDocumentContext context)
        {
            if (string.IsNullOrEmpty(fullAttributeName.Item1))
            {
                List<PropertyReference> propertyReferences = context.Resolver.GetPropertyReference(
                    this.Type,
                    fullAttributeName.Item2);

                if (propertyReferences.Count > 1)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't resolve PropertyName: {0}, more than one properties found"
                }
            }

            return null;
        }

        public virtual BinderInfo GetSourceBiningInfo(
            string attributeValue,
            IDocumentContext context,
            TargetBindingInfo targetBinding)
        {
            var binder = BindingParser.ParseBinding(targetBinding, attributeValue, context);
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
                return new BinderInfo();
            }
            else if (targetTypeDefinition.IsDouble())
            {
                return new BinderInfo();
            }
            else if (targetTypeDefinition.IsBoolean())
            {
                return new BinderInfo();
            }

            return null;
        }
    }
}
