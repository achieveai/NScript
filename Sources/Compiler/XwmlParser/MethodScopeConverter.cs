//-----------------------------------------------------------------------
// <copyright file="MethodScopeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MethodScopeConverter
    /// </summary>
    public class MethodScopeConverter : IMethodScopeConverter
    {
        private IdentifierScope scope;
        private RuntimeScopeManager runtimeManager;
        Dictionary<string, IIdentifier> localMap = new Dictionary<string,IIdentifier>();
        Dictionary<string, IIdentifier> argMap = new Dictionary<string, IIdentifier>();
        Queue<IIdentifier> tempVariables = new Queue<IIdentifier>();
        private IResolver resolver;

        public MethodScopeConverter(
            RuntimeScopeManager runtimeManager,
            IResolver resolver,
            IdentifierScope scope)
        {
            this.runtimeManager = runtimeManager;
            this.scope = scope;
            this.resolver = resolver;
            foreach (var item in this.scope.ParameterIdentifiers)
            {
                argMap.Add(
                    item.SuggestedName,
                    item);
            }
        }

        public NScript.JST.IdentifierScope Scope
        {
            get { return this.scope; }
        }

        public RuntimeScopeManager RuntimeManager
        {
            get { return this.runtimeManager; }
        }

        public NScript.Converter.ConverterKnownReferences KnownReferences
        {
            get { return this.runtimeManager.Context.KnownReferences; }
        }

        public NScript.CLR.ClrKnownReferences ClrKnownReferences
        {
            get { return this.runtimeManager.Context.ClrKnownReferences; }
        }

        public void PushScopeBlock(NScript.CLR.AST.ScopeBlock scopeBlock)
        {
            throw new NotImplementedException();
        }

        public void PopScopeBlock()
        {
            throw new NotImplementedException();
        }

        public NScript.JST.IIdentifier GetTempVariable()
        {
            // Check history for removed code.
            throw new NotImplementedException();
        }

        public NScript.JST.Expression GetReplacementExpression(NScript.CLR.AST.Expression clrExpression)
        {
            return null;
        }

        public NScript.JST.IIdentifier ResolveLocal(string localName)
        {
            IIdentifier rv;
            if (!this.localMap.TryGetValue(localName, out rv))
            {
                rv = SimpleIdentifier.CreateScopeIdentifier(
                    this.scope,
                    localName,
                    false);
            }

            return rv;
        }

        public NScript.JST.Expression ResolveThis(NScript.JST.IdentifierScope identifierScope, NScript.Utils.Location loc)
        {
            throw new NotImplementedException();
        }

        public NScript.JST.Expression ProcessParameterBlock(NScript.CLR.AST.ParameterBlock parameterBlock, IIdentifier _)
        {
            throw new NotImplementedException();
        }

        public void ReleaseTempVariable(NScript.JST.IIdentifier tmpIdentifier)
        {
            this.tempVariables.Enqueue(tmpIdentifier);
        }

        public NScript.JST.IIdentifier ResolveArgument(string argumentName)
        {
            return this.argMap[argumentName];
        }

        public NScript.JST.IIdentifier Resolve(Mono.Cecil.PropertyReference keyReference)
        {
            return this.runtimeManager.Resolve(keyReference);
        }

        public NScript.JST.Expression CreateInstanceMethodCallExpression(
            NScript.Utils.Location location,
            NScript.JST.IdentifierScope scope,
            NScript.JST.Expression instance,
            Mono.Cecil.MethodReference methodReference,
            IList<NScript.JST.Expression> arguments)
        {
            throw new NotImplementedException();
        }

        public NScript.JST.Expression ResolveVirtualMethod(
            Mono.Cecil.MethodReference methodReference,
            NScript.JST.IdentifierScope scope)
        {
            return this.resolver.ResolveVirtualMethod(methodReference, scope);
        }

        public NScript.JST.IIdentifier Resolve(Mono.Cecil.MethodReference memberReference)
        {
            return this.resolver.Resolve(memberReference);
        }

        public IList<NScript.JST.IIdentifier> Resolve(Mono.Cecil.TypeReference typeReference)
        {
            return this.resolver.Resolve(typeReference);
        }

        public IList<NScript.JST.IIdentifier> ResolveStaticMember(Mono.Cecil.MethodReference member)
        {
            return this.resolver.ResolveStaticMember(member);
        }

        public IList<NScript.JST.IIdentifier> ResolveFactory(Mono.Cecil.MethodReference methodReference)
        {
            return this.resolver.ResolveFactory(methodReference);
        }

        public IList<NScript.JST.IIdentifier> ResolveStaticMember(Mono.Cecil.FieldReference fieldReference)
        {
            return this.resolver.ResolveStaticMember(fieldReference);
        }

        public NScript.JST.IIdentifier Resolve(Mono.Cecil.FieldReference fieldReference)
        {
            return this.resolver.Resolve(fieldReference);
        }

        public NScript.JST.Expression ResolveMethodSlotName(Mono.Cecil.MethodReference methodReference, bool isVirtualCall, NScript.JST.IdentifierScope identifierScope)
        {
            return this.resolver.ResolveMethodSlotName(
                methodReference,
                isVirtualCall,
                scope);
        }

        public IIdentifier GetConditionalAccessTempVariable() => throw new NotImplementedException();

        public IIdentifier ResolveLocalFunction(string localFunctionName) => throw new NotImplementedException();
    }
}
