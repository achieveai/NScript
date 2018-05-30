//-----------------------------------------------------------------------
// <copyright file="ForEachLoop.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ForEachLoop
    /// </summary>
    public class ForEachLoop : ScopeBlock
    {
        /// <summary>
        /// Backing field for IteratorVariable.
        /// </summary>
        private LocalVariable iteratorVariable;

        /// <summary>
        /// Backing field for Collection.
        /// </summary>
        private Expression collection;

        /// <summary>
        /// Backing field for Scope.
        /// </summary>
        private ScopeBlock scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForEachLoop"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="iteratorVariable">The iterator variable.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="scope">The scope.</param>
        public ForEachLoop(
            ClrContext context,
            Location location,
            LocalVariable iteratorVariable,
            Expression collection,
            ScopeBlock scope,
            List<(LocalVariable, bool)> variables)
            : base(context, location, variables)
        {
            this.iteratorVariable = iteratorVariable;
            this.collection = collection;
            this.scope = scope;
            base.AddStatement(scope);
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        public LocalVariable Variable
        {
            get { return this.iteratorVariable; }
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        public Expression Collection
        {
            get { return this.collection; }
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public ScopeBlock Scope
        {
            get { return this.scope; }
        }

        /// <summary>
        /// Adds the statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public override void AddStatement(Statement statement)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("iterator", this.Variable);
            serializationInfo.AddValue("collection", this.Collection);
            serializationInfo.AddValue("Scope", this.Scope);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.collection = (Expression)processor.Process(this.Collection);
            this.scope = (ScopeBlock)processor.Process(this.scope);
        }
    }
}
