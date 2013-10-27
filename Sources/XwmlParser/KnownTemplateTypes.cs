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
        /// The system web HTML DLL.
        /// </summary>
        private const string systemWebHtmlDll = "System.Web.Html";

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

            var nativeArray1 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray`1"));

            this.UIElement = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UIElement"));

            this.UIPanel = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UIPanel"));

            this.UISkinableElement = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UISkinableElement"));

            this.ObservableObject = clrContext.GetTypeDefinition(
                Tuple.Create(frameworkDll, observableNamespace + ".ObservableObject"));

            this.ObservableInterface = clrContext.GetTypeDefinition(
                Tuple.Create(frameworkDll, observableNamespace + ".INotifyPropertyChanged"));

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

            this.DocumentRef = clrContext.GetTypeDefinition(
                Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Document"));

            this.ElementRef = clrContext.GetTypeDefinition(
                Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Element"));

            this.NodeRef = clrContext.GetTypeDefinition(
                Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Node"));

            this.BinderHelper = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderHelper"));

            this.BinderType = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.BinderType"));

            this.SkinBinderInfo = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderInfo"));

            this.SkinInstance = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinInstance"));

            this.Skin = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Skin"));

            this.AttributeSetter = clrContext.GetMethodReference(
                "SetAttribute",
                clrKnownReferences.Void,
                this.BinderHelper,
                this.NodeRef,
                clrKnownReferences.String,
                clrKnownReferences.String);

            this.TextContentSetter = clrContext.GetMethodReference(
                "SetTextContent",
                clrKnownReferences.Void,
                this.BinderHelper,
                this.ElementRef,
                clrKnownReferences.String);

            this.CssClassSetter = clrContext.GetMethodReference(
                "SetCssClass",
                clrKnownReferences.Void,
                this.BinderHelper,
                this.ElementRef,
                clrKnownReferences.Boolean,
                clrKnownReferences.String);

            var nativeArrayInt = new GenericInstanceType(nativeArray1);
            nativeArrayInt.GenericArguments.Add(clrKnownReferences.Int32);

            this.ElementFromPathGetter = clrContext.GetMethodReference(
                "GetElementFromPath",
                this.ElementRef,
                this.BinderHelper,
                this.ElementRef,
                nativeArrayInt);

            this.CloneNodeMethodReference = clrContext.GetMethodReference(
                "CloneNode",
                this.ElementRef,
                this.ElementRef,
                clrKnownReferences.Boolean);

            var func2 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Func`2"));

            var func3 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Func`3"));

            var act0 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Action"));

            var act1 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Action`1"));

            var act2 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Action`2"));

            var act3 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Action`3"));

            var funcObjObj = new GenericInstanceType(func2);
            funcObjObj.GenericArguments.Add(clrKnownReferences.Object);
            funcObjObj.GenericArguments.Add(clrKnownReferences.Object);

            var act2ObjObj = new GenericInstanceType(act2);
            act2ObjObj.GenericArguments.Add(clrKnownReferences.Object);
            act2ObjObj.GenericArguments.Add(clrKnownReferences.Object);

            var act3ObjObjObj = new GenericInstanceType(act3);
            act3ObjObjObj.GenericArguments.Add(clrKnownReferences.Object);
            act3ObjObjObj.GenericArguments.Add(clrKnownReferences.Object);
            act3ObjObjObj.GenericArguments.Add(clrKnownReferences.Object);

            var nativeArray1Func2 = new GenericInstanceType(nativeArray1);
            nativeArray1Func2.GenericArguments.Add(funcObjObj);

            var nativeArray1Str = new GenericInstanceType(nativeArray1);
            nativeArray1Str.GenericArguments.Add(clrKnownReferences.String);

            this.SkinBinderCtorOneTime1 = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.SkinBinderInfo,
                nativeArray1Func2,
                act2ObjObj,
                this.BinderType,
                clrKnownReferences.Int32,
                funcObjObj,
                clrKnownReferences.Object).Resolve();

            this.SkinBinderCtorOneTime2 = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.SkinBinderInfo,
                nativeArray1Func2,
                act3ObjObjObj,
                clrKnownReferences.Object,
                this.BinderType,
                clrKnownReferences.Int32,
                funcObjObj,
                clrKnownReferences.Object,
                clrKnownReferences.Int32).Resolve();

            this.SkinBinderCtorOneWay1 = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.SkinBinderInfo,
                nativeArray1Func2,
                nativeArray1Str,
                act2ObjObj,
                this.BinderType,
                clrKnownReferences.Int32,
                clrKnownReferences.Int32,
                funcObjObj,
                clrKnownReferences.Object).Resolve();

            this.SkinBinderCtorOneWay2 = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.SkinBinderInfo,
                nativeArray1Func2,
                nativeArray1Str,
                act3ObjObjObj,
                clrKnownReferences.Object,
                this.BinderType,
                clrKnownReferences.Int32,
                clrKnownReferences.Int32,
                funcObjObj,
                clrKnownReferences.Object,
                clrKnownReferences.Int32).Resolve();

            this.SkinBinderCtorTwoWay = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.SkinBinderInfo,
                nativeArray1Func2,
                act2ObjObj,
                nativeArray1Str,
                act2ObjObj,
                funcObjObj,
                clrKnownReferences.String,
                this.BinderType,
                clrKnownReferences.Int32,
                clrKnownReferences.Int32,
                funcObjObj,
                funcObjObj,
                clrKnownReferences.Object).Resolve();

            var nativeArrayUIElement = new GenericInstanceType(nativeArray1);
            nativeArrayUIElement.GenericArguments.Add(this.UIElement);

            var nativeArrayObject = new GenericInstanceType(nativeArray1);
            nativeArrayObject.GenericArguments.Add(clrKnownReferences.Object);

            var nativeArraySkinBinderInfo = new GenericInstanceType(nativeArray1);
            nativeArraySkinBinderInfo.GenericArguments.Add(this.SkinBinderInfo);

            this.SkinInstanceCtor = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.SkinInstance,
                this.Skin,
                this.ElementRef,
                nativeArrayUIElement,
                nativeArrayObject,
                nativeArraySkinBinderInfo,
                clrKnownReferences.Int32,
                clrKnownReferences.Int32).Resolve();

            var func3SkinDocSI = new GenericInstanceType(func3);
            func3SkinDocSI.GenericArguments.Add(this.Skin);
            func3SkinDocSI.GenericArguments.Add(this.DocumentRef);
            func3SkinDocSI.GenericArguments.Add(this.SkinInstance);

            this.SkinCtor = clrContext.GetMethodReference(
                ".ctor",
                clrKnownReferences.Void,
                this.Skin,
                clrKnownReferences.TypeType,
                clrKnownReferences.TypeType,
                func3SkinDocSI,
                clrKnownReferences.String).Resolve();

            this.CssInitializerMethod = clrContext.GetMethodReference(
                "InitializeCss",
                clrKnownReferences.Void,
                this.BinderHelper,
                this.DocumentRef,
                nativeArrayInt,
                nativeArray1Str,
                clrKnownReferences.Int32);

            this.DomEventType2 = new GenericInstanceType(act2);
            this.DomEventType2.GenericArguments.Add(this.ElementRef);
            var elementEvent = clrContext.GetTypeDefinition(Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".ElementEvent"));
            this.DomEventType2.GenericArguments.Add(elementEvent);

            this.DomEventType1 = new GenericInstanceType(act1);
            this.DomEventType1.GenericArguments.Add(this.ElementRef);

            this.DomEventType0 = act0;
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

        /// <summary>
        /// Gets or sets the element reference.
        /// </summary>
        /// <value>
        /// The element reference.
        /// </value>
        public TypeDefinition ElementRef { get; private set; }

        /// <summary>
        /// Gets or sets the clone node method reference.
        /// </summary>
        /// <value>
        /// The clone node method reference.
        /// </value>
        public MethodReference CloneNodeMethodReference { get; private set; }

        /// <summary>
        /// Gets or sets the observable interface.
        /// </summary>
        /// <value>
        /// The observable interface.
        /// </value>
        public TypeDefinition ObservableInterface { get; private set; }

        public TypeDefinition BinderHelper { get; set; }

        public TypeDefinition NodeRef { get; set; }

        public MethodReference AttributeSetter { get; set; }

        public MethodReference TextContentSetter { get; set; }

        public MethodReference CssClassSetter { get; set; }

        public TypeDefinition SkinBinderInfo { get; set; }

        public MethodReference SkinBinderCtorOneTime1 { get; set; }

        public MethodReference SkinBinderCtorOneTime2 { get; set; }

        public MethodReference SkinBinderCtorOneWay1 { get; set; }

        public MethodReference SkinBinderCtorOneWay2 { get; set; }

        public MethodReference SkinBinderCtorTwoWay { get; set; }

        public MethodReference ElementFromPathGetter { get; set; }

        public TypeDefinition SkinInstance { get; set; }

        public MethodReference SkinCtor { get; set; }

        public MethodReference SkinInstanceCtor { get; set; }

        public TypeDefinition Skin { get; set; }

        public TypeDefinition DocumentRef { get; set; }

        public MethodReference CssInitializerMethod { get; set; }

        public TypeDefinition BinderType { get; set; }

        public GenericInstanceType DomEventType2 { get; set; }

        public GenericInstanceType DomEventType1 { get; set; }

        public TypeDefinition DomEventType0 { get; set; }
    }
}
