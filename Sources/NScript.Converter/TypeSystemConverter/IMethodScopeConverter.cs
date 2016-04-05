namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Utils;
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

        JST.IIdentifier GetTempVariable();

        JST.Expression GetReplacementExpression(Expression clrExpression);

        JST.IIdentifier ResolveLocal(string localName);

        JST.Expression ResolveThis(
            JST.IdentifierScope identifierScope,
            Location loc);

        JST.Expression ProcessParameterBlock(ParameterBlock parameterBlock);

        void ReleaseTempVariable(JST.IIdentifier tmpIdentifier);

        JST.IIdentifier ResolveArgument(string argumentName);

        JST.IIdentifier Resolve(Mono.Cecil.PropertyReference keyReference);

        JST.Expression CreateInstanceMethodCallExpression(
            Location location,
            JST.IdentifierScope scope,
            JST.Expression instance,
            MethodReference methodReference,
            IList<JST.Expression> arguments);
    }
}
