//-----------------------------------------------------------------------
// <copyright file="InlineDelegateClass.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InlineDelegateClass
    /// </summary>
    public class InlineDelegateClass
    {
        /// <summary>
        /// Backing field for DelegateStarterBlock.
        /// </summary>
        private readonly int functionStartIndex = -1;

        /// <summary>
        /// Backing field for Delegate Class;
        /// </summary>
        private readonly TypeReference delegateClass = null;

        /// <summary>
        /// Mappings for delegate class field and arguments.
        /// </summary>
        private readonly List<Tuple<FieldReference, Variable>> argumentMappings =
            new List<Tuple<FieldReference, Variable>>();

        /// <summary>
        /// Mapping for delegate class fields and locals.
        /// </summary>
        private readonly List<Tuple<FieldReference, string, Expression>> localMappings =
            new List<Tuple<FieldReference,string, Expression>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDelegateClass"/> class.
        /// </summary>
        /// <param name="functionStartIndex">Start index of the function.</param>
        /// <param name="delegateClass">The delegate class.</param>
        /// <param name="argumentMappings">The argument mappings.</param>
        /// <param name="localMappings">The local mappings.</param>
        public InlineDelegateClass(
            int functionStartIndex,
            TypeReference delegateClass,
            List<Tuple<FieldReference, Variable>> argumentMappings,
            List<Tuple<FieldReference, string, Expression>> localMappings)
        {
            this.functionStartIndex = functionStartIndex;
            this.delegateClass = delegateClass;
            this.argumentMappings = argumentMappings;
            this.localMappings = localMappings;
        }

        /// <summary>
        /// Gets the delegate starter.
        /// </summary>
        public int FunctionStartIndex
        {
            get { return this.functionStartIndex; }
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
        public List<Tuple<FieldReference, Variable>> ArgumentMapping
        {
            get { return this.argumentMappings; }
        }

        /// <summary>
        /// Gets the local mapping.
        /// </summary>
        public List<Tuple<FieldReference, string, Expression>> LocalMapping
        {
            get { return this.localMappings; }
        }
    }
}
