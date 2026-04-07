using System;
using Mono.Cecil;
using NScript.CLR;
using NScript.RazorSkin.TemplateIR;
using Serilog;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Resolves all runtime MethodDefinition and TypeDefinition values needed
    /// for graph descriptor JST emission. Mirrors the pattern established by
    /// KnownTemplateTypes (XwmlParser) and RazorTemplatingPlugin.ResolveRuntimeIdentifiers.
    /// </summary>
    public class RazorKnownTypes
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private const string UiFrameworkDll = "Sunlight.Framework.UI";
        private const string SystemWebHtmlDll = "System.Web.Html";

        public readonly MethodDefinition SetTextContent;
        public readonly MethodDefinition SetAttribute;
        public readonly MethodDefinition SetCssClass;
        public readonly MethodDefinition GetElementFromPath;
        public readonly MethodDefinition SkinInstanceCtor;
        public readonly MethodDefinition SkinCtor;
        public readonly TypeDefinition UISkinableElement;

        // Exposed type definitions for use by ResolveRuntimeIdentifiers
        public readonly TypeDefinition SkinType;
        public readonly TypeDefinition SkinInstanceType;
        public readonly TypeDefinition SkinBinderInfoType;
        public readonly TypeDefinition BinderHelperType;
        public readonly TypeDefinition ElementRefType;
        public readonly TypeDefinition NodeRefType;
        public readonly TypeDefinition DocumentRefType;

        // Framework attribute types for sub-control tag resolution
        public readonly TypeDefinition TagNameAttribute;
        public readonly TypeDefinition DomAttributeAttribute;

        public RazorKnownTypes(ClrContext clrContext, ClrKnownReferences clrKnownRefs)
        {
            // --- Look up key framework types (same as KnownTemplateTypes) ---
            var skinType = clrContext.GetTypeDefinition(
                Tuple.Create(UiFrameworkDll, UiFrameworkDll + ".Skin"));
            var skinInstanceType = clrContext.GetTypeDefinition(
                Tuple.Create(UiFrameworkDll, UiFrameworkDll + ".Helpers.SkinInstance"));
            var skinBinderInfoType = clrContext.GetTypeDefinition(
                Tuple.Create(UiFrameworkDll, UiFrameworkDll + ".Helpers.SkinBinderInfo"));
            var binderHelperType = clrContext.GetTypeDefinition(
                Tuple.Create(UiFrameworkDll, UiFrameworkDll + ".Helpers.SkinBinderHelper"));
            var elementRefType = clrContext.GetTypeDefinition(
                Tuple.Create(SystemWebHtmlDll, SystemWebHtmlDll + ".Element"));
            var nodeRefType = clrContext.GetTypeDefinition(
                Tuple.Create(SystemWebHtmlDll, SystemWebHtmlDll + ".Node"));
            var documentRefType = clrContext.GetTypeDefinition(
                Tuple.Create(SystemWebHtmlDll, SystemWebHtmlDll + ".Document"));

            UISkinableElement = clrContext.GetTypeDefinition(
                Tuple.Create(UiFrameworkDll, UiFrameworkDll + ".UISkinableElement"));

            // Store for external access by ResolveRuntimeIdentifiers
            SkinType = skinType;
            SkinInstanceType = skinInstanceType;
            SkinBinderInfoType = skinBinderInfoType;
            BinderHelperType = binderHelperType;
            ElementRefType = elementRefType;
            NodeRefType = nodeRefType;
            DocumentRefType = documentRefType;

            // --- Generic type building for constructor signatures ---
            var nativeArray = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray"));
            var nativeArray1 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray`1"));
            var func3 = clrContext.GetTypeDefinition(
                Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Func`3"));

            var nativeArrayInt = new GenericInstanceType(nativeArray1);
            nativeArrayInt.GenericArguments.Add(clrKnownRefs.Int32);

            var nativeArraySkinBinderInfo = new GenericInstanceType(nativeArray1);
            nativeArraySkinBinderInfo.GenericArguments.Add(skinBinderInfoType);

            // --- Resolve SkinBinderHelper static methods ---

            // SetTextContent(Element elem, string text)
            SetTextContent = clrContext.GetMethodReference(
                "SetTextContent",
                clrKnownRefs.Void,
                binderHelperType,
                elementRefType,
                clrKnownRefs.String).Resolve();

            // SetAttribute(Node node, string attrName, string attrValue)
            SetAttribute = clrContext.GetMethodReference(
                "SetAttribute",
                clrKnownRefs.Void,
                binderHelperType,
                nodeRefType,
                clrKnownRefs.String,
                clrKnownRefs.String).Resolve();

            // SetCssClass(Element elem, bool add, string className) — optional
            try
            {
                SetCssClass = clrContext.GetMethodReference(
                    "SetCssClass",
                    clrKnownRefs.Void,
                    binderHelperType,
                    elementRefType,
                    clrKnownRefs.Boolean,
                    clrKnownRefs.String).Resolve();
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Could not resolve SetCssClass — optional method");
                SetCssClass = null;
            }

            // GetElementFromPath(Element root, NativeArray<int> path)
            GetElementFromPath = clrContext.GetMethodReference(
                "GetElementFromPath",
                elementRefType,
                binderHelperType,
                elementRefType,
                nativeArrayInt).Resolve();

            // --- Resolve SkinInstance 8-param constructor ---
            // SkinInstance(Skin, Element, NativeArray<int>, NativeArray, NativeArray<SkinBinderInfo>, object, int, int)
            SkinInstanceCtor = clrContext.GetMethodReference(
                ".ctor",
                clrKnownRefs.Void,
                skinInstanceType,
                skinType,
                elementRefType,
                nativeArrayInt,
                nativeArray,
                nativeArraySkinBinderInfo,
                clrKnownRefs.Object,
                clrKnownRefs.Int32,
                clrKnownRefs.Int32).Resolve();

            // --- Resolve Skin 4-param constructor ---
            // Skin(Type controlType, Type modelType, Func<Skin,Document,SkinInstance> factory, string dataIndex)
            var func3SkinDocSI = new GenericInstanceType(func3);
            func3SkinDocSI.GenericArguments.Add(skinType);
            func3SkinDocSI.GenericArguments.Add(documentRefType);
            func3SkinDocSI.GenericArguments.Add(skinInstanceType);

            SkinCtor = clrContext.GetMethodReference(
                ".ctor",
                clrKnownRefs.Void,
                skinType,
                clrKnownRefs.TypeType,
                clrKnownRefs.TypeType,
                func3SkinDocSI,
                clrKnownRefs.String).Resolve();

            // --- Resolve framework attribute types for sub-control tag resolution ---
            try
            {
                TagNameAttribute = clrContext.GetTypeDefinition(
                    Tuple.Create(UiFrameworkDll, "Sunlight.Framework.UI.Attributes.TagNameAttribute"));
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Could not resolve TagNameAttribute — custom tags disabled");
                TagNameAttribute = null;
            }

            try
            {
                DomAttributeAttribute = clrContext.GetTypeDefinition(
                    Tuple.Create(UiFrameworkDll, "Sunlight.Framework.UI.Attributes.DomAttributeAttribute"));
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Could not resolve DomAttributeAttribute — custom DOM attributes disabled");
                DomAttributeAttribute = null;
            }
        }

        /// <summary>
        /// Returns the appropriate setter MethodDefinition for the given expression target.
        /// Used by GraphDescriptorJSTEmitter to emit targetInfo setter references.
        /// </summary>
        public MethodDefinition GetSetterMethod(ExpressionTarget target)
        {
            switch (target)
            {
                case ExpressionTarget.TextContent:
                    return SetTextContent;
                case ExpressionTarget.Attribute:
                case ExpressionTarget.Style:
                    return SetAttribute;
                case ExpressionTarget.CssClass:
                    return SetCssClass ?? SetAttribute;
                default:
                    return SetTextContent;
            }
        }
    }
}
