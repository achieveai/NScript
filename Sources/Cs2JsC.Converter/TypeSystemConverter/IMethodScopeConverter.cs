namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
using Cs2JsC.Utils;
    using Mono.Cecil;

    public interface IMethodScopeConverter : IResolver
    {
        JST.IdentifierScope Scope
        { get; }

        RuntimeScopeManager RuntimeManager
        { get; }

        ConverterKnownReferences KnownReferences
        { get; }

        ClrKnownReferences ClrKnownReferences
        { get; }

        void PushScopeBlock(ScopeBlock scopeBlock);

        void PopScopeBlock();

        JST.Identifier GetTempVariable();

        JST.Expression GetReplacementExpression(Expression clrExpression);

        JST.Identifier ResolveLocal(string localName);

        JST.Expression ResolveThis(JST.IdentifierScope identifierScope);

        JST.Expression ProcessParameterBlock(ParameterBlock parameterBlock);

        void ReleaseTempVariable(JST.Identifier tmpIdentifier);

        JST.Identifier ResolveArgument(string argumentName);

        JST.Identifier Resolve(Mono.Cecil.PropertyReference keyReference);

        JST.Expression CreateInstanceMethodCallExpression(
            Location location,
            JST.IdentifierScope scope,
            JST.Expression instance,
            MethodReference methodReference,
            IList<JST.Expression> arguments);
    }
}
