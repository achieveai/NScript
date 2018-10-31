namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Emit;
    using Newtonsoft.Json.Linq;

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
        }

        private static Stream ToAstStream(
            SerializationContext context,
            Dictionary<IMethodSymbol, MethodBody> methodMaps)
        {
            var fullAst = new FullAst
            {
                Methods = new LinkedList<MethodBody>(methodMaps.Values),
                TypeInfo = context.SymbolSerializer.GetTypesInfo()
            };

            var memStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memStream, fullAst);
            memStream.Position = 0;
            return memStream;
        }
    }
}
