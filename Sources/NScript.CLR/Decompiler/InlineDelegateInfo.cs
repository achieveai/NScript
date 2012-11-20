//-----------------------------------------------------------------------
// <copyright file="InlineDelegateInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using NScript.CLR.Decompiler.Blocks;
    using NScript.CLR.AST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InlineDelegateInfo
    /// </summary>
    internal class InlineDelegateInfo
    {
        /// <summary>
        /// Backing field for DelegateStarterBlock.
        /// </summary>
        private readonly int startFuncIndex = -1;

        /// <summary>
        /// Backing field for Delegate Class;
        /// </summary>
        private readonly TypeReference delegateClass = null;

        /// <summary>
        /// Mappings for delegate class field and arguments.
        /// </summary>
        private readonly List<Tuple<FieldReference, ParameterDefinition>> argumentMappings =
            new List<Tuple<FieldReference, ParameterDefinition>>();

        /// <summary>
        /// Mapping for delegate class fields and locals.
        /// </summary>
        private readonly List<Tuple<FieldReference, string, Block>> localMappings =
            new List<Tuple<FieldReference,string, Block>>();

        /// <summary>
        /// Inlines the delegate block.
        /// </summary>
        /// <param name="starterFuncIndex">The starter.</param>
        /// <param name="delegateClass">The delegate class.</param>
        /// <param name="argumentMappings">The argument mappings.</param>
        /// <param name="localMappings">The local mappings.</param>
        public InlineDelegateInfo(
            int starterFuncIndex,
            TypeReference delegateClass,
            List<Tuple<FieldReference, ParameterDefinition>> argumentMappings,
            List<Tuple<FieldReference, string, Block>> localMappings)
        {
            this.startFuncIndex = starterFuncIndex;
            this.delegateClass = delegateClass;
            this.argumentMappings = argumentMappings;
            this.localMappings = localMappings;
        }

        /// <summary>
        /// Gets the delegate starter.
        /// </summary>
        public int StartFunctionIndex
        {
            get { return this.startFuncIndex; }
        }

        /// <summary>
        /// Gets the delegate class.
        /// </summary>
        public TypeReference DelegateClass
        {
            get { return this.delegateClass; }
        }

        /// <summary>
        /// Gets the argument mapping.
        /// </summary>
        public List<Tuple<FieldReference, ParameterDefinition>> ArgumentMapping
        {
            get { return this.argumentMappings; }
        }

        /// <summary>
        /// Gets the local mapping.
        /// </summary>
        public List<Tuple<FieldReference, string, Block>> LocalMapping
        {
            get { return this.localMappings; }
        }

        /// <summary>
        /// Toes the ast.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST.InlineDelegateClass</returns>
        public AST.InlineDelegateClass ToAst(VariableResolver variableResolver)
        {
            return new AST.InlineDelegateClass(
                this.startFuncIndex,
                this.delegateClass,
                this.argumentMappings.Select((a) =>
                    Tuple.Create(
                        a.Item1,
                        (Variable)variableResolver.ResolveParameter(a.Item2)))
                    .ToList(),
                this.localMappings.Select((a) =>
                    Tuple.Create(
                        a.Item1,
                        a.Item2,
                        a.Item3 != null
                            ? (AST.Expression)a.Item3.ToAstNode(variableResolver)
                            : null))
                    .ToList());
        }
    }
}