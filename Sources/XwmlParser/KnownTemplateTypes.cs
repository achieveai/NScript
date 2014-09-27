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

        public readonly TypeReference ContextBindableObject;

        public readonly TypeReference UIElement;

        public readonly TypeReference UISkinableElement;

        public readonly TypeReference UIPanel;

        public readonly TypeReference ExtensibleObject;

        public readonly TypeReference ObservableObject;

        public readonly TypeReference ArrayType;

        public readonly TypeReference IListType;

        public readonly TypeReference SkinAttribute;

        public readonly TypeDefinition ElementRef;

        public readonly MethodReference CloneNodeMethodReference;

        public readonly TypeDefinition ObservableInterface;

        public readonly TypeDefinition BinderHelper;

        public readonly TypeDefinition NodeRef;

        public readonly MethodReference AttributeSetter;

        public readonly MethodReference DataContextSetter;

        public readonly MethodReference TextContentSetter;

        public readonly MethodReference CssClassSetter;

        public readonly TypeDefinition SkinBinderInfo;

        public readonly MethodReference SkinBinderCtorOneTime1;

        public readonly MethodReference SkinBinderCtorOneTime2;

        public readonly MethodReference SkinBinderCtorOneWay1;

        public readonly MethodReference SkinBinderCtorOneWay2;

        public readonly MethodReference SkinBinderCtorTwoWay;

        public readonly MethodReference ElementFromPathGetter;

        public readonly TypeDefinition SkinInstance;

        public readonly MethodReference SkinCtor;

        public readonly MethodReference SkinInstanceCtor;

        public readonly TypeDefinition Skin;

        public readonly TypeDefinition DocumentRef;

        public readonly MethodReference CssInitializerMethod;

        public readonly TypeDefinition BinderType;

        public readonly GenericInstanceType DomEventType2;

        public readonly GenericInstanceType DomEventType1;

        public readonly TypeDefinition DomEventType0;

        public readonly TypeDefinition SkinPartAttribute;

        public readonly TypeDefinition TagNameAttribute;

        public readonly TypeDefinition DomAttributeAttribute;

        public readonly TypeDefinition DefaultDataBindingAttribute;

        public readonly TypeDefinition CssNameAttribute;

        public readonly ClrKnownReferences ClrKnownReference;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clrKnownReferences"> The colour known references. </param>
        public KnownTemplateTypes(ClrKnownReferences clrKnownReferences)
        {
            ClrContext clrContext = clrKnownReferences.ClrContext;

            this.ClrKnownReference = clrKnownReferences;

            var nativeArray = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray"));

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

            this.SkinAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".SkinAttribute"));

            this.CssNameAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".CssNameAttribute"));

            this.DefaultDataBindingAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".DefaultDataBindingAttribute"));

            this.DomAttributeAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".DomAttributeAttribute"));

            this.TagNameAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".TagNameAttribute"));

            this.SkinPartAttribute = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, attributesNamespace + ".SkinPartAttribute"));

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
                clrKnownReferences.String).Resolve();

            this.TextContentSetter = clrContext.GetMethodReference(
                "SetTextContent",
                clrKnownReferences.Void,
                this.BinderHelper,
                this.ElementRef,
                clrKnownReferences.String).Resolve();

            this.DataContextSetter = clrContext.GetMethodReference(
                "SetDataContext",
                clrKnownReferences.Void,
                this.BinderHelper,
                this.UIElement,
                clrKnownReferences.Object).Resolve();

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

            var nativeArrayInt32 = new GenericInstanceType(nativeArray1);
            nativeArrayInt32.GenericArguments.Add(clrKnownReferences.Int32);

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
                nativeArrayInt32,
                nativeArray,
                nativeArraySkinBinderInfo,
                clrKnownReferences.Object,
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
    }
}
