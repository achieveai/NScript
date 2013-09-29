//-----------------------------------------------------------------------
// <copyright file="KnownTemplateTypes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.CLR;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for KnownTemplateTypes.
    /// </summary>
    public class KnownTemplateTypes
    {
        /// <summary>
        /// The framework DLL.
        /// </summary>
        private const string uiFrameworkDll = "Sunlight.Framework.UI";

        /// <summary>
        /// The framework DLL.
        /// </summary>
        private const string frameworkDll = "Sunlight.Framework";

        /// <summary>
        /// The observable namespace.
        /// </summary>
        private const string observableNamespace = frameworkDll + ".Observables";

        /// <summary>
        /// The attributes namespace.
        /// </summary>
        private const string attributesNamespace = uiFrameworkDll + ".Attributes";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clrKnownReferences"> The colour known references. </param>
        public KnownTemplateTypes(ClrKnownReferences clrKnownReferences)
        {
            ClrContext clrContext = clrKnownReferences.ClrContext;

            this.UIElement = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UIElement"));

            this.UIPanel = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UIPanel"));

            this.UISkinableElement = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UISkinableElement"));

            this.ObservableObject = clrContext.GetTypeDefinition(
                Tuple.Create(frameworkDll, observableNamespace + ".ObservableObject"));

            this.ExtensibleObject = clrContext.GetTypeDefinition(
                Tuple.Create(frameworkDll, observableNamespace + ".ExtensibleObservableObject"));

            this.ContextBindableObject = clrContext.GetTypeDefinition(
                Tuple.Create(frameworkDll, "Sunlight.Framework.Binders.ContextBindableObject"));

            this.ArrayType = clrKnownReferences.SystemArray;
            this.IListType = clrContext.GetTypeDefinition(
                Tuple.Create("NScript.MsCorlib", "System.Collections.Generic.IList`1"));

            this.CssClassAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".CssNameAttribute"));

            this.SkinAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".SkinAttribute"));
        }

        /// <summary>
        /// Gets or sets the context bindable object.
        /// </summary>
        /// <value>
        /// The context bindable object.
        /// </value>
        public TypeReference ContextBindableObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The user interface element.
        /// </value>
        public TypeReference UIElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the skinable element.
        /// </summary>
        /// <value>
        /// The user interface skinable element.
        /// </value>
        public TypeReference UISkinableElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the panel.
        /// </summary>
        /// <value>
        /// The user interface panel.
        /// </value>
        public TypeReference UIPanel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the extensible object.
        /// </summary>
        /// <value>
        /// The extensible object.
        /// </value>
        public TypeReference ExtensibleObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the observable object.
        /// </summary>
        /// <value>
        /// The observable object.
        /// </value>
        public TypeReference ObservableObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the array.
        /// </summary>
        /// <value>
        /// The type of the array.
        /// </value>
        public TypeReference ArrayType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the list.
        /// </summary>
        /// <value>
        /// The type of the list.
        /// </value>
        public TypeReference IListType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the CSS class attribute.
        /// </summary>
        /// <value>
        /// The CSS class attribute.
        /// </value>
        public TypeReference CssClassAttribute
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the skin attribute.
        /// </summary>
        /// <value>
        /// The skin attribute.
        /// </value>
        public TypeReference SkinAttribute
        {
            get;
            set;
        }
    }
}
