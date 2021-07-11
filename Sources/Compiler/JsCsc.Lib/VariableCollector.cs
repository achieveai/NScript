//-----------------------------------------------------------------------
// <copyright file="BondToAst.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System.Collections.Generic;
    using Mono.Cecil;
    using NScript.CLR.AST;
    using Mono.Cecil.Cil;
    using System.Linq;
    using System;

    public class VariableCollector
    {
        private readonly bool _isParamBlock;
        private List<LocalVariable> localVariables
            = new List<LocalVariable>();
        private HashSet<LocalVariable> usedVariables
            = new HashSet<LocalVariable>();
        private HashSet<Variable> escapingVariable
            = new HashSet<Variable>();
        private List<LocalFunctionVariable> localFunctionVariables
            = new List<LocalFunctionVariable>();

        private readonly List<ParameterVariable> parameters;

        private readonly Dictionary<string, ParameterVariable> parameterMap;

        public VariableCollector(
            int blockId,
            bool isParamBlock = false,
            ParameterDefinition thisParameter = null,
            List<ParameterDefinition> parameters = null)
        {
            _isParamBlock = isParamBlock;
            BlockId = blockId;

            if (_isParamBlock)
            {
                ThisVariable = thisParameter != null
                    ? new ThisVariable(
                        thisParameter,
                        null)
                    : null;
            }

            if (parameters != null)
            {
                this.parameters = parameters
                    .Select(_ => new ParameterVariable(
                        _,
                        null))
                    .ToList();

                parameterMap = this.parameters.ToDictionary(_ => _.Name);
            }
        }

        public bool IsParamBlock
        { get { return _isParamBlock; } }

        public int BlockId { get; }

        public ThisVariable ThisVariable { get; }

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
                null);

            this.localVariables.Add(returnValue);

            return returnValue;
        }

        public LocalFunctionVariable CreateFunctionVariable(
            string variableName)
        {
            string addedVariableName = variableName;
            int changIndex = -1;
            for (int variableIndex = 0; variableIndex < this.localFunctionVariables.Count; variableIndex++)
            {
                if (this.localFunctionVariables[variableIndex].Name == addedVariableName)
                {
                    addedVariableName = variableName + "_" + (++changIndex);
                    variableIndex = -1;
                }
            }

            var returnValue = new LocalFunctionVariable(variableName);

            this.localFunctionVariables.Add(returnValue);

            return returnValue;
        }

        public LocalVariable ResolveVariable(string variableName)
        {
            foreach (LocalVariable variable in this.localVariables)
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

        public LocalFunctionVariable ResolveLocalFunctionVariable(string variableName)
            => this.localFunctionVariables
                .Where(lfv => lfv.Name == variableName)
                .FirstOrDefault();

        public void AddEscapingVariable(Variable variable)
        {
            if (!_isParamBlock)
            { throw new System.InvalidOperationException(); }

            escapingVariable.Add(variable);
            variable.SetHoisted();
        }

        public List<(LocalVariable variable, bool isUSed)> GetCapturedVariables()
        {
            return localVariables
                .Select(_ => (_, usedVariables.Contains(_)))
                .ToList();
        }

        public List<LocalFunctionVariable> GetLocalFunctionVariables()
        {
            return this.localFunctionVariables;
        }

        public (List<Variable> escapingVars, List<ParameterVariable> paras, ThisVariable thisVar)
            GetParamBlockVariables()
        {
            return (
                escapingVariable.ToList(),
                parameters,
                ThisVariable);
        }

        public bool GetParameterVariable(string name, out ParameterVariable rv)
        { return parameterMap.TryGetValue(name, out rv); }
    }
}
