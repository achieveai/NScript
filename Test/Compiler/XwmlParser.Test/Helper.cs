//-----------------------------------------------------------------------
// <copyright file="Helper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Definition for Helper
    /// </summary>
    public class Helper
    {
        const string MsCorLib = "MsCorLib.dll";
        const string SystemWeb = "System.Web.dll";
        const string SystemWebHtml = "System.Web.Html.dll";
        const string SunlightFramework = "Sunlight.Framework.dll";
        const string SunlightFrameworkUi = "Sunlight.Framework.UI.dll";
        const string SunlightUnit = "SunlightUnit.dll";
        const string SunlightFrameworkTest = "Sunlight.Framework.Test.dll";
        const string SunlightFrameworkUITest = "Sunlight.Framework.UI.Test.dll";

        static ClrContext context;
        static TypeResolver resolver;
        static RuntimeScopeManager runtimeScopeManager;

        public static void Initialize()
        {
            if (context == null)
            {
                var path = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(typeof(Helper).Assembly.Location),
                    @"..\..\..\Sunlight.Framework.UI.Test\bin\Debug");

                context = new ClrContext();
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        MsCorLib),
                    false);
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        SystemWeb),
                    false);
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        SystemWebHtml),
                    false);
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        SunlightFramework),
                    false);
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        SunlightUnit),
                        false);
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        SunlightFrameworkUi),
                    false);
                context.LoadAssembly(
                    System.IO.Path.Combine(
                        path,
                        SunlightFrameworkUITest),
                    false);

                ConverterContext converterContext =
                    new ConverterContext(
                        context,
                        null,
                        null);
                runtimeScopeManager = new RuntimeScopeManager(converterContext);

                resolver = new TypeResolver(
                    runtimeScopeManager,
                    context);
            }
        }

        internal static TypeResolver CreateResolver()
        {
            return new TypeResolver(
                Helper.runtimeScopeManager,
                Helper.context);
        }

        internal static ParserContext GetParserContext()
        {
            ConverterContext converterContext =
                new ConverterContext(
                    context,
                    null,
                    null);
            var runtimeScopeManager = new RuntimeScopeManager(converterContext);

            resolver = new TypeResolver(
                runtimeScopeManager,
                context);

            KnownTemplateTypes knownTemplateTypes = new KnownTemplateTypes(
                context.KnownReferences);
            return new ParserContext(
                knownTemplateTypes,
                new CodeGenerator(
                    runtimeScopeManager,
                    knownTemplateTypes),
                resolver,
                resolver,
                new List<string>());
        }

        internal static ClrContext GetClrContext()
        {
            return Helper.context;
        }

        internal static XwmlParser.XwmlTemplatingPlugin CreatePlugin(
            List<Tuple<string,string>> args)
        {
            XwmlTemplatingPlugin rv = new XwmlTemplatingPlugin();
            ConverterContext converterContext =
                new ConverterContext(
                    context,
                    null,
                    null);
            var runtimeScopeManager = new RuntimeScopeManager(converterContext);
            rv.ParseArgs(args);
            rv.Initialize(
                context,
                runtimeScopeManager);

            return rv;
        }

        internal static void LoadHtmlParser(
            string resourceName,
            ParserContext parserContext)
        {
            using (var stream = typeof(Helper).Assembly.GetManifestResourceStream("XwmlParser.Test.Templates." + resourceName))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(stream);

                HtmlParser parser = new HtmlParser(
                    resourceName,
                    doc,
                    parserContext);

                parserContext.RegisterHtmlParser(
                    resourceName,
                    parser);
            }
        }

        internal static string ConvertCodeToString(IList<Statement> code)
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            var serializer = new NScript.JST.JSWriter(
                true,
                false);

            foreach (var statement in code)
            {
                serializer.Write(statement);
            }

            serializer.Write(writer);
            return writer.ToString();
        }

        internal static void CheckCode(
            string codeFileName,
            IList<Statement> code)
        {
            string expectedValue;
            using (var stream = typeof(Helper).Assembly.GetManifestResourceStream("XwmlParser.Test.TemplateCode." + codeFileName))
            {
                var reader = new System.IO.StreamReader(stream);

                expectedValue = reader.ReadToEnd().Trim();
            }

            var actualValue = Helper.ConvertCodeToString(code).Trim();
            if (expectedValue != actualValue)
            {
                Console.Error.WriteLine("====== Expected ================================> ");
                Console.Error.WriteLine(expectedValue);
                Console.Error.WriteLine("====== Actual ==================================> ");
                Console.Error.WriteLine(actualValue);
            }

            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
