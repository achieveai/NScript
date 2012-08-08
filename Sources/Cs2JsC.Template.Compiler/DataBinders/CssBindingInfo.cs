//-----------------------------------------------------------------------
// <copyright file="CssBindingInfo.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler.DataBinders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cs2JsC.PELoader.Reflection;

    /// <summary>
    /// Css binding modes.
    /// </summary>
    public enum CssBindingMode
    {
        /// <summary>
        /// Bound to data.
        /// </summary>
        DataBound = 0,

        /// <summary>
        /// Bound to template parent
        /// </summary>
        TemplateBound = 1,

        /// <summary>
        /// Bound to self.
        /// </summary>
        SelfBound = 2
    }

    /// <summary>
    /// Css binding info.
    /// </summary>
    public class CssBindingInfo : BindingInfo
    {
        /// <summary>
        /// CssClassBinder type reference.
        /// </summary>
        private static TypeReference cssClassBinderTypeReference = null;

        /// <summary>
        /// Backing field for cssClass.
        /// </summary>
        private string cssClass;

        /// <summary>
        /// Binding mode
        /// </summary>
        private CssBindingMode mode;

        /// <summary>
        /// Is self binding.
        /// </summary>
        private bool isSelfBinding;

        /// <summary>
        /// Backing field for converterClass.
        /// </summary>
        private TypeReference converterClass;

        /// <summary>
        /// Convert Type Reference.
        /// </summary>
        private TypeReference converterTypeReference = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CssBindingInfo"/> class.
        /// </summary>
        /// <param name="cssClass">The CSS class.</param>
        /// <param name="bindingValues">The binding values.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <param name="templatedControlType">Type of the templated control.</param>
        /// <param name="isSelfBinding">if set to <c>true</c> [is self binding].</param>
        public CssBindingInfo(
            string cssClass,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            TypeReference dataContextType,
            TypeReference templatedControlType,
            bool isSelfBinding)
        {
            this.isSelfBinding = isSelfBinding;
            this.cssClass = cssClass;
            this.mode = isSelfBinding
                ? CssBindingMode.SelfBound
                : bindingValues.Item1 == BindingInfo.TemplatBindingStr
                    ? CssBindingMode.TemplateBound
                    : CssBindingMode.DataBound;

            this.SourcePropertyPath = bindingValues.Item2;

            string converterClass;
            if (bindingValues.Item3.TryGetValue(BindingInfo.ConverterStr, out converterClass))
            {
                this.converterClass = resolver.ResolveType(
                    converterClass,
                    DefaultSettings.Default.ConvertersNS);

                if (this.converterClass == null)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't resolve type {0}",
                            converterClass));
                }
            }

            PropertyReference propertyReference;
            if ((this.mode == CssBindingMode.TemplateBound || this.mode == CssBindingMode.SelfBound)
                && templatedControlType != null)
            {
                if (!DataBindingInfo.GetEndPropertyGetter(
                    this.SourcePropertyPath,
                    templatedControlType,
                    out propertyReference))
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't find propertyPath {0} on {1}. Please check the propertyPath.",
                            this.SourcePropertyPath,
                            templatedControlType));
                }
            }
            else if (this.mode == CssBindingMode.DataBound && dataContextType != null)
            {
                if (!DataBindingInfo.GetEndPropertyGetter(
                    this.SourcePropertyPath,
                    dataContextType,
                    out propertyReference))
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't find propertyPath {0} on {1}. Please check the propertyPath.",
                            this.SourcePropertyPath,
                            dataContextType));
                }
            }
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public TypeReference Converter
        {
            get
            {
                return this.converterClass;
            }
        }

        /// <summary>
        /// Gets the CSS class.
        /// </summary>
        /// <value>The CSS class.</value>
        public string CssClass
        {
            get
            {
                return this.cssClass;
            }
        }

        /// <summary>
        /// Adds to reference.
        /// </summary>
        public override void AddToReference()
        {
            base.AddToReference();

            if (CssBindingInfo.cssClassBinderTypeReference == null)
            {
                CssBindingInfo.cssClassBinderTypeReference = new TypeReference(CssBindingInfo.CssClassBinderType);
                TypeManager.AddTypeReference(CssBindingInfo.cssClassBinderTypeReference);
            }

            if (this.converterClass != null)
            {
                this.converterTypeReference = new TypeReference(this.converterClass);
                TypeManager.AddTypeReference(this.converterTypeReference);
            }
        }

        /// <summary>
        /// Writes the binding.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="styleNameMapping">The style name mapping.</param>
        internal void WriteBinding(
            string elementId,
            System.IO.StreamWriter writer,
            StyleMapping styleNameMapping)
        {
            writer.Write(
                "{0}.{1}(",
                elementId,
                BindingInfo.UIObjectType.ResolveMethod(
                    "AddCssBinder",
                    false,
                    BindingInfo.CssClassBinderType));

            writer.Write(
                "new {0}(",
                TypeManager.GetTypeReferenceId(new TypeReference(BindingInfo.CssClassBinderType)));

            writer.Write(
                "\"{0}\",",
                styleNameMapping.ContainsStyle(this.cssClass) ? styleNameMapping.GetStyleId(this.cssClass) : this.cssClass);

            writer.Write("\"{0}\",", this.SourcePropertyPath);

            if (this.converterTypeReference != null)
            {
                writer.Write(
                    "new {0}(),",
                    TypeManager.GetTypeReferenceId(this.converterTypeReference));
            }
            else
            {
                writer.Write("null,");
            }

            writer.WriteLine(
                "{0}));",
                (int)this.mode);
        }
    }
}
