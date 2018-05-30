namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    /// <summary>
    /// Scope block.
    /// </summary>
    public class ScopeBlock : ExplicitBlock
    {
        /// <summary>
        /// Backing collection for LocalVariables.
        /// </summary>
        private readonly List<LocalVariable> localVariables =
            new List<LocalVariable>();

        /// <summary>
        /// Used variables.
        /// </summary>
        private readonly HashSet<LocalVariable> usedVariables =
            new HashSet<LocalVariable>();

        /// <summary>
        /// Backing field for LocalVariables.
        /// </summary>
        private readonly ReadOnlyCollection<LocalVariable> readonlyLocalVariables;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeBlock"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        public ScopeBlock(
            ClrContext context,
            Location location,
            List<(LocalVariable localVariable, bool isUsed)> variables)
            : base(context, location)
        {
            if (variables != null)
            {
                variables.ForEach(_ =>
                {
                    _.localVariable.DefiningScope = this;
                    localVariables.Add(_.localVariable);

                    if (_.isUsed)
                    { usedVariables.Add(_.localVariable); }
                });
            }

            this.readonlyLocalVariables = new ReadOnlyCollection<LocalVariable>(this.localVariables);
        }

        /// <summary>
        /// Gets the local variables.
        /// </summary>
        /// <value>The local variables.</value>
        public IList<LocalVariable> LocalVariables
        {
            get
            {
                return this.readonlyLocalVariables;
            }
        }

        /// <summary>
        /// Gets the used variable count.
        /// </summary>
        /// <value>The used variable count.</value>
        public int UsedVariableCount
        {
            get
            {
                return this.usedVariables.Count;
            }
        }

        /// <summary>
        /// Creates the variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public LocalVariable CreateVariable(
            string variableName,
            TypeReference typeReference)
        {
            string addedVariableName = variableName;
            int changIndex = -1;
            for (int variableIndex = 0; variableIndex < this.localVariables.Count; variableIndex++)
            {
                if (this.localVariables[variableIndex].Name == addedVariableName)
                {
                    addedVariableName = variableName + "_" + (++changIndex);
                    variableIndex = -1;
                }
            }

            LocalVariable returnValue = new LocalVariable(
                new VariableDefinition(typeReference, variableName),
                this);

            this.localVariables.Add(returnValue);

            return returnValue;
        }

        /// <summary>
        /// Moves the variables.
        /// </summary>
        /// <param name="scopeBlock">The scope block.</param>
        public void MoveVariablesFrom(ScopeBlock scopeBlock)
        {
            for (int iVariable = scopeBlock.localVariables.Count - 1; iVariable >= 0; iVariable--)
            {
                var variable = scopeBlock.localVariables[iVariable];
                variable.DefiningScope = this;
                this.localVariables.Add(variable);
            }
        }

        /// <summary>
        /// Determines whether variable with given name is used or not.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <returns>
        /// <c>true</c> if variable is used otherwise, <c>false</c>.
        /// </returns>
        public bool IsVariableUsed(string variableName)
        {
            foreach (LocalVariable variable in this.LocalVariables)
            {
                if (variable.Name == variableName)
                {
                    return this.usedVariables.Contains(variable);
                }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Determines whether given variable is used.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>
        /// <c>true</c> if given variable is used otherwise, <c>false</c>.
        /// </returns>
        public bool IsVariableUsed(LocalVariable variable)
        {
            return this.usedVariables.Contains(variable);
        }

        /// <summary>
        /// Resolves the variable.
        /// </summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <returns></returns>
        public LocalVariable ResolveVariable(string variableName)
        {
            foreach (LocalVariable variable in this.LocalVariables)
            {
                if (variable.Name == variableName)
                {
                    if (!this.usedVariables.Contains(variable))
                    {
                        this.usedVariables.Add(variable);
                    }

                    return variable;
                }
            }

            return null;
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(
            IAstProcessor processor)
        {
            base.ProcessThroughPipeline(processor);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(ICustomSerializer serializationInfo)
        {
            if (serializationInfo.Version >= 2
                && this.localVariables.Count > 0)
            {
                serializationInfo.AddValue(
                    "locals",
                    this.localVariables,
                    delegate(ICustomSerializer s, LocalVariable v)
                    {
                        s.AddValue("name", v.Name);
                        s.AddValue("type", v.Type.ToString());
                    });
            }

            base.Serialize(serializationInfo);
        }
    }
}