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
            CSharpCompilation compilation)
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
                false,
                DebugInformationFormat.Pdb,
                null,
                null,
                512,
                0,
                false,
                SubsystemVersion.None,
                "4",
                false,
                true);

            compilation.Emit(
                new MemoryStream(),
                null,
                null,
                null,
                null,
                emitOptions);

            return rv;
        }
    }
}
