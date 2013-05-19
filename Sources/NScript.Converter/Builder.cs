//-----------------------------------------------------------------------
// <copyright file="Builder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using System.Collections.Generic;
    using System.IO;
    using NScript.CLR;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;
    using Mono.Cecil;
    using System.Linq;

    /// <summary>
    /// Definition for Builder.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// The main assembly.
        /// </summary>
        private readonly string mainAssembly;

        /// <summary>
        /// The js script.
        /// </summary>
        private readonly string jsScript;

        /// <summary>
        /// The references.
        /// </summary>
        private readonly string[] references;

        /// <summary>
        /// The plugins.
        /// </summary>
        private readonly IRuntimeConverterPlugin[] plugins;

        /// <summary>
        /// The method converter plugins.
        /// </summary>
        private readonly IMethodConverterPlugin[] methodConverterPlugins;

        /// <summary>
        /// The type converter plugins.
        /// </summary>
        private readonly ITypeConverterPlugin[] typeConverterPlugins;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsScript">               The js script. </param>
        /// <param name="mainAssembly">           The main assembly. </param>
        /// <param name="references">             The references. </param>
        /// <param name="plugins">                The plugins. </param>
        /// <param name="typeConverterPlugins">   The type converter plugins. </param>
        /// <param name="methodConverterPlugins"> The method converter plugins. </param>
        public Builder(
            string jsScript,
            string mainAssembly,
            string[] references,
            IConverterPlugin[] plugins)
        {
            this.mainAssembly = mainAssembly;
            this.jsScript = jsScript;
            this.references = references;
            this.plugins = (from p in plugins where p is IRuntimeConverterPlugin select p as IRuntimeConverterPlugin)
                .ToArray<IRuntimeConverterPlugin>();
            this.methodConverterPlugins = (from p in plugins where p is IMethodConverterPlugin select p as IMethodConverterPlugin)
                .ToArray<IMethodConverterPlugin>();
            this.typeConverterPlugins = (from p in plugins where p is IRuntimeConverterPlugin select p as ITypeConverterPlugin)
                .ToArray<ITypeConverterPlugin>();
        }

        /// <summary>
        /// Executes this object.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool Execute()
        {
            if (!this.VerifyPaths())
            {
                return false;
            }

            ClrContext clrContext = new ClrContext();
            foreach (var reference in references)
            {
                clrContext.LoadAssembly(reference);
            }

            clrContext.LoadAssembly(this.mainAssembly);

            ConverterContext converterContext = new ConverterContext(clrContext);
            RuntimeScopeManager runtimeManager = new RuntimeScopeManager(converterContext);
            List<MethodDefinition> methodDefinitionsToEmit = new List<MethodDefinition>();
            MethodDefinition entryPoint =
                Path.GetExtension(mainAssembly) == ".exe"
                    ? this.GetEntryPoint(converterContext, Path.GetFileName(mainAssembly))
                    : null;

            if (entryPoint != null)
            {
                methodDefinitionsToEmit.Add(entryPoint);
            }

            // Let's go through first pass and collect all the method references
            // to emit.
            if (this.plugins != null)
            {
                foreach (var plugin in this.plugins)
                {
                    plugin.Initialize(clrContext, runtimeManager);

                    var methodsToEmit = plugin.GetMethodsToEmitPass1();

                    // Let's resolve references for all the methods that we may be emitting. This will
                    // cause runtimeManager to traverse these methods as well during analysis.
                    if (methodsToEmit != null)
                    {
                        for (int methodIndex = 0; methodIndex < methodsToEmit.Count; methodIndex++)
                        {
                            runtimeManager.Resolve(methodsToEmit[methodIndex]);
                            methodDefinitionsToEmit.Add(methodsToEmit[methodIndex].Resolve());
                        }
                    }
                }
            }

            // Let's convert all the code to JS.
            var statements = runtimeManager.Convert(methodDefinitionsToEmit);

            if (this.plugins != null)
            {
                foreach (var plugin in this.plugins)
                {
                    var pluginJsStatements = plugin.GetPreJavascript();
                    if (pluginJsStatements != null)
                    { statements.InsertRange(0, pluginJsStatements); }

                    pluginJsStatements = plugin.GetPostJavascript();
                    if (pluginJsStatements != null)
                    { statements.AddRange(pluginJsStatements); }
                }
            }

            if (entryPoint != null)
            {
                // Not at the end, let's insert call to entryPoint.
                statements.Add(
                    JST.ExpressionStatement.CreateMethodCallExpression(
                        new JST.IdentifierExpression(runtimeManager.ResolveFunctionName(entryPoint), runtimeManager.Scope)));
            }

            JSWriter writer = new JSWriter(true, false);
            foreach (var statement in statements)
            {
                if (statement != null)
                {
                    writer.Write(statement);
                }
            }

            writer.Write(this.jsScript, string.Format("SrcMapper.ashx?js={0}&fname=", Path.GetFileName(this.jsScript)));

            return true;
        }

        /// <summary>
        /// Determines if we can verify paths.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        private bool VerifyPaths()
        {
            bool returnValue = true;
            if (!File.Exists(mainAssembly))
            {
                returnValue = false;
                Logger.Instance.LogError(
                    string.Format("main assembly: ({0}) not found", mainAssembly));
            }

            foreach (var reference in this.references)
            {
                if (!File.Exists(reference))
                {
                    returnValue = false;
                    Logger.Instance.LogError(
                        string.Format("reference: ({0}) not found", reference));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets entry point.
        /// </summary>
        /// <param name="context">      The context. </param>
        /// <param name="mainAssembly"> The main assembly. </param>
        /// <returns>
        /// The entry point.
        /// </returns>
        private MethodDefinition GetEntryPoint(ConverterContext context, string mainAssembly)
        {
            ModuleDefinition module;
            context.ClrContext.TryGetModuleDefinition(mainAssembly, out module);

            foreach (var item in module.Types)
            {
                if (item.IsInterface
                    || item.IsValueType
                    || item.HasGenericParameters
                    || item.IsAbstract)
                {
                    continue;
                }

                foreach (var method in item.Methods)
                {
                    if (method.HasGenericParameters
                        || method.HasAssociatedMember()
                        || !method.IsStatic)
                    {
                        continue;
                    }

                    if (method.IsPublic
                        && method.ReturnType.FullName == context.ClrKnownReferences.Void.FullName
                        && !method.HasParameters
                        && method.Parameters.Count == 0
                        && method.CustomAttributes.SelectAttribute(context.KnownReferences.EntryPointAttribute) != null)
                    {
                        return method;
                    }
                }
            }

            return null;
        }
    }
}