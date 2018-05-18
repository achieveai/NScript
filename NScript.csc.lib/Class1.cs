namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
                compilation.Emit(
                    outputStream,
                    outputPdbStream,
                    null,
                    null,
                    null,
                    emitOptions);
            }
            finally
            {
                outputStream.Close();
                outputPdbStream.Close();
            }

            return rv;
        }
    }
}
