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
    using NScript.JST.Visitors;

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

        private readonly int jsParts;

        private readonly bool minify;

        private readonly bool uglify;

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
            int jsParts,
            string mainAssembly,
            string[] references,
            IConverterPlugin[] plugins,
            bool minify,
            bool uglify)
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
            this.jsParts = jsParts;
            this.minify = minify;
            this.uglify = uglify;
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

            RuntimeScopeManager runtimeManager;
            ConverterContext converterContext;
            List<MethodDefinition> methodDefinitionsToEmit;
            MethodDefinition entryPoint;

            try
            {
                converterContext = new ConverterContext(
                    clrContext,
                    this.methodConverterPlugins,
                    this.typeConverterPlugins);
                runtimeManager = new RuntimeScopeManager(
                    converterContext,
                    this.minify);

                methodDefinitionsToEmit = new List<MethodDefinition>();
                entryPoint = this.GetEntryPoint(converterContext, Path.GetFileName(mainAssembly));
            }
            catch(System.Exception ex)
            {
                System.Console.Out.WriteLine(
                    string.Format("{0}({1},{2}): error ERR0123: {3}",
                        string.Empty,
                        0,
                        0,
                        ex.Message));

                System.Console.Out.WriteLine(ex.StackTrace);

                return false;
            }

            try
            {
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
                var statements = runtimeManager.Convert(methodDefinitionsToEmit, plugins);

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

                if (minify)
                {
                    var identCounter = new IdentifierCounterVisitor();
                    var unusedMethodRemover = new UnusedMethodRemover();
                    var inlinableVisitor = new InlineableVisitor();
                    var methodNameRemover = new MethodNameRemover();

                    statements.ForEach(((IJstVisitor)inlinableVisitor).DispatchStatement);
                    var proxyFixer = new ProxyFixer(inlinableVisitor.Functions);
                    statements = statements
                        .ConvertAll(((ITransformerVisitor)proxyFixer).DispatchStatement);

                    runtimeManager.Scope.ResetUsageCounter();
                    runtimeManager.JSBaseObjectScopeManager.InstanceScope.ResetUsageCounter();
                    statements.ForEach(((IJstVisitor)identCounter).DispatchStatement);
                    statements = statements
                        .ConvertAll(((ITransformerVisitor)methodNameRemover).DispatchStatement)
                        .ConvertAll(((ITransformerVisitor)unusedMethodRemover).DispatchStatement);
                }

                var stopWatch = new System.Diagnostics.Stopwatch();

                stopWatch.Start();
                IdentifierScope.IdentifierMinifiedNamer.MinifyNames(runtimeManager.Scope, this.minify);
                stopWatch.Stop();
                System.Console.WriteLine("Root scope naming time taken: {0}", stopWatch.ElapsedMilliseconds);
                stopWatch.Restart();
                IdentifierScope.IdentifierMinifiedNamer.MinifyNames(runtimeManager.JSBaseObjectScopeManager.InstanceScope, this.minify);
                System.Console.WriteLine("Instance scope naming time taken: {0}", stopWatch.ElapsedMilliseconds);

                var writer = new JSWriter(true, this.uglify);
                var initializerStatement = runtimeManager.GetVariableDeclarations();
                if (initializerStatement != null)
                {
                    writer.Write(initializerStatement);
                }

                foreach (var statement in statements)
                {
                    if (statement != null)
                    {
                        writer.Write(statement);
                    }
                }

                writer.Write(
                    this.jsScript,
                    string.Format(
                        "SrcMapper.ashx?js={0}&fname=",
                        Path.GetFileName(this.jsScript)));
            }
            catch(ConverterLocationException ex)
            {
                System.Console.Out.WriteLine(
                    string.Format("{0}({1},{2}): error ERR0123: {3}",
                        ex.Location.FileName,
                        ex.Location.StartLine,
                        ex.Location.StartColumn,
                        ex.Message));
            }
            catch(System.Exception ex)
            {
                System.Console.Out.WriteLine(
                    "NScript.Exe(0,0): error UNK0001: {0}",
                    ex.Message);
                System.Console.Out.WriteLine(ex.StackTrace);

                while(ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    System.Console.WriteLine("-------------------------");
                    System.Console.Out.WriteLine(ex.Message);
                    System.Console.Out.WriteLine(ex.StackTrace);
                }
            }

            foreach (var warning in converterContext.Warnings)
            {
                if (warning.Item1 != null)
                {
                    System.Console.Out.WriteLine(
                        string.Format("{0}({1},{2}): warning WRN0123: {3}",
                            warning.Item1.FileName,
                            warning.Item1.StartLine,
                            warning.Item1.StartColumn,
                            warning.Item2));
                }
            }

            foreach (var warning in converterContext.Errors)
            {
                if (warning.Item1 != null)
                {
                    System.Console.Out.WriteLine(
                        string.Format("{0}({1},{2}): error ERR0123: {3}",
                            warning.Item1.FileName,
                            warning.Item1.StartLine,
                            warning.Item1.StartColumn,
                            warning.Item2));
                }
                else
                {
                    System.Console.Out.WriteLine(
                        string.Format("{0}({1},{2}): error ERR0123: {3}",
                            string.Empty,
                            0,
                            0,
                            warning.Item2));
                }
            }

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
                    || item.HasGenericParameters)
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