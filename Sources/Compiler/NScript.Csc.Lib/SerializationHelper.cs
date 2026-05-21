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

            return (new ResourceDescription[] { astResource }, rv);

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
