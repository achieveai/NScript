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

    public class SerializationHelper
    {
        public static Dictionary<IMethodSymbol, MethodBody> ExpressionVisitMap(
            CSharpCompilation compilation,
            string outputPath,
            string moduleName,
            string runtimeMetadataVersion = null)
        {
            var context = new SerializationContext(
                new SymbolSerializer());

            var serializer = new BoundAstToAstBase();
            var rv = new Dictionary<IMethodSymbol, MethodBody>();
            compilation.OnBoundExpressionGenerated = (methodSymbol, bsl) =>
            {
                // dict[model.GetDeclaredSymbol((MethodDeclarationSyntax)bsl.Syntax)]
                //     = serializer.SerializeMethodBody(bsl, context);
                rv.Add(
                    methodSymbol,
                    serializer.GetMethodBody(
                        methodSymbol,
                        bsl,
                        context));
            };

            var astResource = new ResourceDescription(
                "$$BstInfo$$",
                () => ToAstStream(context, rv),
                true);

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
                var result = compilation.Emit(
                    outputStream,
                    pdbStream: outputPdbStream,
                    options: emitOptions,
                    manifestResources: new ResourceDescription[] { astResource });

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
