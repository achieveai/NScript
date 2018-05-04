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

    public class SerializationHelper
    {
        public static string ExpressionVisitMap(
            CSharpCompilation compilation,
            SemanticModel model)
        {
            var context = new SerializationContext(
                model,
                new SymbolSerializer());

            var serializer = new BoundAstToSerialization();
            var dict = new Dictionary<IMethodSymbol, AstBase>();
            compilation.OnBoundExpressionGenerated = (bsl) =>
            {
                // dict[model.GetDeclaredSymbol((MethodDeclarationSyntax)bsl.Syntax)]
                //     = serializer.SerializeMethodBody(bsl, context);
                serializer.Visit(bsl, context);
            };

            compilation.Emit(
                new MemoryStream());

            return serializer.sb.ToString();
        }
    }
}
