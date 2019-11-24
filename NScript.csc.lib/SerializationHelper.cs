namespace NScript.Csc.Lib
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;

    public class SerializationHelper
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
            compilation.OnBoundExpressionGenerated = (methodSymbol, bsl, initializers) =>
            {
                var serializer = new BoundAstToAstBase();
                var methodBody = 
                    serializer.GetMethodBody(
                        methodSymbol,
                        bsl,
                        initializers,
                        context);
                lock(rv)
                {
                    rv.Add(
                        methodSymbol,
                        methodBody);
                }
            };

            var astResource = new ResourceDescription(
                "$$BstInfo$$",
                () => ToAstStream(context, rv),
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
