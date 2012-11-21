namespace NScript.CLR.Decompiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NScript.CLR.AST;
    using ParameterDefinition = Mono.Cecil.ParameterDefinition;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    internal class VariableResolver
    {
        /// <summary>
        /// Backing field for topBlock.
        /// </summary>
        private readonly TopLevelBlock topBlock;

        /// <summary>
        /// Backing field for methodBlock
        /// </summary>
        private readonly MethodExecutionBlock methodBlock;

        /// <summary>
        /// Backing collection for scope stack.
        /// </summary>
        private readonly List<ScopeBlock> scopeStack =
            new List<ScopeBlock>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableResolver"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="topBlock">The top block.</param>
        public VariableResolver(
            MethodExecutionBlock method,
            TopLevelBlock topBlock)
        {
            this.methodBlock = method;
            this.topBlock = topBlock;
        }

        /// <summary>
        /// Gets the top block.
        /// </summary>
        /// <value>The top block.</value>
        public TopLevelBlock TopBlock
        {
            get
            {
                return this.topBlock;
            }
        }

        /// <summary>
        /// Pushes the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public void PushScope(ScopeBlock scope)
        {
            this.scopeStack.Add(scope);
        }

        /// <summary>
        /// Pops the scope.
        /// </summary>
        public void PopScope()
        {
            this.scopeStack.RemoveAt(this.scopeStack.Count-1);
        }

        /// <summary>
        /// Resolves the local.
        /// </summary>
        /// <param name="localIndex">Index of the local.</param>
        /// <returns></returns>
        public LocalVariable ResolveLocal(VariableDefinition local)
        {
            return new LocalVariable(
                local,
                this.topBlock.RootBlock);
        }

        /// <summary>
        /// Resolves the parameter.
        /// </summary>
        /// <param name="parameterIndex">Index of the parameter.</param>
        /// <returns>ParameterVariable</returns>
        public ParameterVariable ResolveParameter(ParameterDefinition parameter)
        {
            if (this.methodBlock.Method.HasThis
                && this.methodBlock.Method.Body.ThisParameter == parameter)
            {
                return new ThisVariable(
                    parameter,
                    this.topBlock.RootBlock);
            }

            return new ParameterVariable(
                parameter,
                this.topBlock.RootBlock);
        }
    }
}
