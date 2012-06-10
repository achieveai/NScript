//-----------------------------------------------------------------------
// <copyright file="AttributeBase.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Cs2JsC.Template.Compiler.Attributes
{
    using System.Linq;
    using Cs2JsC.Template.Compiler.DataBinders;
    using System.Collections.Generic;
    using System;
    using Cs2JsC.PELoader.Reflection;

    /// <summary>
    /// Base for all the attribute readers.
    /// </summary>
    public class AttributeBase
    {
        /// <summary>
        /// Creates the specified attribute row.
        /// </summary>
        /// <param name="attributeRow">The attribute row.</param>
        /// <param name="currentAssembly">The current assembly.</param>
        /// <returns>Instance of AttributeBase.</returns>
        public static AttributeBase Create(
            GenericCustomAttribute attribute)
        {
            string typeName = attribute.Ctor.Parent.TypeName;

            if (typeName.EndsWith("DefaultTagNameAttribute"))
            {
                return new DefaultTagNameAttribute(
                    (string)attribute.Arguments[0]);
            }
            else if (typeName.EndsWith("CssClassAttribute"))
            {
                return new CssClassAttribute();
            }
            else if (typeName.EndsWith("BindingAttribute"))
            {
                BindingMode mode =
                    attribute.Properties.Contains(BindingAttribute.DefaultBinding)
                        ? (DataBinders.BindingMode)(int)attribute.GetPropretyValue(BindingAttribute.DefaultBinding)
                        : DataBinders.BindingMode.OneWay;

                bool isStrict =
                    attribute.Properties.Contains(BindingAttribute.IsStrictBinding)
                        ? (bool)attribute.GetPropretyValue(BindingAttribute.IsStrictBinding)
                        : false;

                return new BindingAttribute(
                    mode,
                    isStrict);
            }
            else if (typeName.EndsWith("ConverterTypeAttribute"))
            {
                return new ConverterAttribute(
                    (TypeReferenceBase)attribute.Arguments[0],
                    (TypeReferenceBase)attribute.Arguments[1]);
            }
            else if (typeName.EndsWith("TemplatePartAttribute"))
            {
                TypeReferenceBase templatePartType =
                    attribute.Properties.Contains("Type")
                        ? (TypeReferenceBase)attribute.GetPropretyValue("Type")
                        : null;

                bool isRequired =
                    attribute.Properties.Contains("Required")
                        ? (bool)attribute.GetPropretyValue("Required")
                        : false;

                return new TemplatePartAttribute(
                    (TypeReference)templatePartType,
                    isRequired);
            }
            else if (typeName.EndsWith("AttachedDependencyPropertyAttribute"))
            {
                return new AttachedPropertyAttribute(
                    (TypeReference)attribute.Arguments[0]);
            }

            return null;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes">The attributes.</param>
        /// <returns>Attribute name</returns>
        public static T GetAttribute<T>(
            IList<CustomAttributeBase> attributes) where T : AttributeBase
        {
            string typeName = string.Empty;

            if (typeof(T) == typeof(DefaultTagNameAttribute))
            {
                typeName = "DefaultTagNameAttribute";
            }
            else if (typeof(T) == typeof(CssClassAttribute))
            {
                typeName = "CssClassAttribute";
            }
            else if (typeof(T) == typeof(BindingAttribute))
            {
                typeName = "BindingAttribute";
            }
            else if (typeof(T) == typeof(ConverterAttribute))
            {
                typeName = "ConverterTypeAttribute";
            }
            else if (typeof(T) == typeof(TemplatePartAttribute))
            {
                typeName = "TemplatePartAttribute";
            }
            else if (typeof(T) == typeof(AttachedPropertyAttribute))
            {
                typeName = "AttachedDependencyPropertyAttribute";
            }
            else
            {
                return null;
            }

            foreach (var attribute in attributes)
            {
                GenericCustomAttribute genericAttribute = attribute as GenericCustomAttribute;
                if (genericAttribute != null
                    && genericAttribute.Ctor.Parent.TypeName == typeName)
                {
                    return (T)AttributeBase.Create(genericAttribute);
                }
            }

            return null;
        }
    }
}
