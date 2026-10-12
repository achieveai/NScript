namespace NScript.Csc.Lib
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Symbols;
    using Microsoft.CodeAnalysis.Emit;
    using NScript.Utils;

    public static class SerializationHelper
    {
        public static Dictionary<IMethodSymbol, MethodBody> ExpressionVisitMap(
            CSharpCompilation compilation,
            string outputPath,
            string moduleName,
            string runtimeMetadataVersion = null)
        {
            var emitOptions = new EmitOptions(
                debugInformationFormat: DebugInformationFormat.Pdb,
                fileAlignment: 512,
                subsystemVersion: SubsystemVersion.None,
                runtimeMetadataVersion: runtimeMetadataVersion,
                tolerateErrors: false,
                includePrivateMembers: true);

            var outputStream = File.Open(
                Path.Combine(outputPath, moduleName),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);

            var outputPdbStream = File.Open(
                Path.Combine(outputPath, Path.GetFileNameWithoutExtension(moduleName) + ".pdb"),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);

            try
            {
                var (resources, rv) = InjectIntoCompilation(compilation);
                var result = compilation.Emit(
                    outputStream,
                    pdbStream: outputPdbStream,
                    options: emitOptions,
                    manifestResources: resources);

                var errors = result
                    .Diagnostics
                    .Where(diag => diag.Severity == DiagnosticSeverity.Error)
                    .ToArray();

                if (result.Success)
                { return rv; }
                else
                {
                    return null;
                }
            }
            finally
            {
                outputStream.Close();
                outputPdbStream.Close();
            }
        }

        public static (ResourceDescription[], Dictionary<IMethodSymbol, MethodBody>) InjectIntoCompilation(
            CSharpCompilation compilation)
        {
            var context = new SerializationContext(
                new SymbolSerializer());

            var rv = new Dictionary<IMethodSymbol, MethodBody>();
            compilation.OnBoundExpressionGenerated = (methodSymbol, boundBody, initializers) =>
            {
                // WI-93: Skip method bodies that already carry binding errors.
                // Roslyn still invokes this callback on error-bearing trees, but
                // walking them surfaces ErrorTypeSymbol / similar shapes that the
                // serializer can't model. Throwing here short-circuits
                // Compilation.Emit before it can return its CS-coded diagnostics
                // — the process crashes instead of producing a clean error.
                // Returning early lets Roslyn's normal diagnostic flow run.
                // HasErrors is set only for error-severity nodes (not warnings),
                // and Emit will still return result.Success == false so the
                // caller sees the failure.
                if ((boundBody != null && boundBody.HasErrors) ||
                    (initializers != null && initializers.HasErrors))
                {
                    if (CompilerLog.IsEnabled)
                    {
                        CompilerLog.ForComponent("Csc.Serialization").Debug(
                            "BoundBodySkippedHasErrors {Method}",
                            methodSymbol?.ToDisplayString());
                    }
                    return;
                }

                var serializer = new BoundAstToAstBase();
                var methodBody =
                    serializer.GetMethodBody(
                        methodSymbol,
                        boundBody,
                        initializers,
                        context);
                lock (rv)
                {
                    rv.Add(
                        (IMethodSymbol)((ISymbolInternal)methodSymbol).GetISymbol(),
                        methodBody);
                }

                if (CompilerLog.IsEnabled)
                {
                    CompilerLog.ForComponent("Csc.Serialization").Debug(
                        "BoundBodyCaptured {Method}",
                        methodSymbol?.ToDisplayString());
                }
            };

            var astResource = new ResourceDescription(
                "$$BstInfo$$",
                () =>
                {
                    if (CompilerLog.IsEnabled)
                    {
                        CompilerLog.ForComponent("Csc.Serialization").Information(
                            "BstInfoResourceWritten MethodCount={MethodCount}",
                            rv.Count);
                    }
                    return ToAstStream(context, rv);
                },
                true);

            var srcResource = new ResourceDescription(
                "$$SrcInfo$$",
                () => ToSrcInfoStream(compilation),
                false);

            return (new ResourceDescription[] { astResource, srcResource }, rv);

            /*
            var astJResource = new ResourceDescription(
                "$$JstInfo$$",
                () => ToAstJStream(context, rv),
                true);

            return (new ResourceDescription[] { astJResource, astResource }, rv);
            */
        }

        private static Stream ToAstJStream(
            SerializationContext context,
            Dictionary<IMethodSymbol, MethodBody> methodMaps)
        {
            var fullAst = new FullAst
            {
                Methods = new List<MethodBody>(methodMaps.Values),
                TypeInfo = context.SymbolSerializer.GetTypesInfo()
            };

            var memStream = new MemoryStream();
            Serializer.Serialize(memStream, fullAst, Serializer.SerializationKind.Json);

            memStream.Position = 0;
            return memStream;
        }

        /// <summary>
        /// Writes which source files the assembly came from and which files declare each type,
        /// so a build session can tell which types a recompile changed. One line per item:
        /// <c>O\t&lt;options&gt;</c> with the options that change code without changing a file
        /// (defines, language version, optimization, overflow checks, unsafe, platform,
        /// nullable), then <c>F\t&lt;path&gt;\t&lt;checksum hex&gt;</c> per syntax tree, in
        /// compilation order, then <c>T\t&lt;Cecil full name&gt;\t&lt;file indexes, comma
        /// separated&gt;</c> per source type.
        /// </summary>
        private static Stream ToSrcInfoStream(CSharpCompilation compilation)
        {
            var text = new System.Text.StringBuilder();
            var parseOptions = compilation.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions;
            var options = compilation.Options;
            text.Append("O\t")
                .Append(string.Join(";", parseOptions?.PreprocessorSymbolNames ?? Enumerable.Empty<string>()))
                .Append('|').Append(parseOptions?.LanguageVersion)
                .Append('|').Append(options.OptimizationLevel)
                .Append('|').Append(options.CheckOverflow)
                .Append('|').Append(options.AllowUnsafe)
                .Append('|').Append(options.Platform)
                .Append('|').Append(options.NullableContextOptions)
                .Append('\n');

            var fileIndex = new Dictionary<SyntaxTree, int>();
            foreach (var tree in compilation.SyntaxTrees)
            {
                fileIndex[tree] = fileIndex.Count;
                text.Append("F\t")
                    .Append(tree.FilePath)
                    .Append('\t')
                    .Append(System.Convert.ToHexString(tree.GetText().GetChecksum().AsSpan()))
                    .Append('\n');
            }

            void AddTypes(IEnumerable<INamedTypeSymbol> types, string prefix)
            {
                foreach (var type in types)
                {
                    var fullName = prefix + type.MetadataName;
                    var files = type.DeclaringSyntaxReferences
                        .Select(reference => fileIndex.TryGetValue(reference.SyntaxTree, out var index) ? index : -1)
                        .Where(index => index >= 0)
                        .Distinct()
                        .OrderBy(index => index);
                    text.Append("T\t")
                        .Append(fullName)
                        .Append('\t')
                        .Append(string.Join(",", files))
                        .Append('\n');
                    AddTypes(type.GetTypeMembers(), fullName + "/");
                }
            }

            void AddNamespace(INamespaceSymbol ns)
            {
                var prefix = ns.IsGlobalNamespace ? string.Empty : ns.ToDisplayString() + ".";
                AddTypes(ns.GetTypeMembers(), prefix);
                foreach (var child in ns.GetNamespaceMembers())
                {
                    AddNamespace(child);
                }
            }

            AddNamespace(((Compilation)compilation).Assembly.GlobalNamespace);
            return new MemoryStream(new System.Text.UTF8Encoding(false).GetBytes(text.ToString()));
        }

        private static Stream ToAstStream(
            SerializationContext context,
            Dictionary<IMethodSymbol, MethodBody> methodMaps)
        {
            var fullAst = new FullAst
            {
                Methods = new List<MethodBody>(methodMaps.Values),
                TypeInfo = context.SymbolSerializer.GetTypesInfo()
            };

            var memStream = new MemoryStream();
            Serializer.Serialize(memStream, fullAst, Serializer.SerializationKind.NetSerializer);
            memStream.Position = 0;
            return memStream;
        }
    }
}
