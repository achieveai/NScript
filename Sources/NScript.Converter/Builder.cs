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

    /// <summary>
    /// Definition for Builder
    /// </summary>
    public class Builder
    {
        private readonly string mainAssembly;
        private readonly string jsScript;
        private readonly string[] references;
        private readonly IConverterPlugin[] plugins;

        public Builder(
            string jsScript,
            string mainAssembly,
            string[] references,
            IConverterPlugin[] plugins)
        {
            this.mainAssembly = mainAssembly;
            this.jsScript = jsScript;
            this.references = references;
            this.plugins = plugins;
        }

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