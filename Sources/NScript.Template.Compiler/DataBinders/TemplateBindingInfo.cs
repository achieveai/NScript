//-----------------------------------------------------------------------
// <copyright file="TemplateBindingInfo.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.DataBinders
{
    using System;
    using System.Collections.Generic;
    using NScript.PELoader.Reflection;

    /// <summary>
    /// Template Binding info.
    /// </summary>
    public class TemplateBindingInfo : DataBindingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateBindingInfo"/> class.
        /// </summary>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="bindingValues">The binding values.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="templatedControlType">Type of the templated control.</param>
        public TemplateBindingInfo(
            BoundProperty targetProperty,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            TypeReference templatedControlType)
            : base(targetProperty, bindingValues, resolver, templatedControlType)
        {
        }

        /// <summary>
        /// Gets the add binder string.
        /// </summary>
        /// <returns>AddBinder string.</returns>
        protected override string GetAddBinderString()
        {
            return "AddTemplateBinder";
        }
    }
}
