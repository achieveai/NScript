//-----------------------------------------------------------------------
// <copyright file="ParameterBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for ParameterBlock
    /// </summary>
    public class ParameterBlock : ScopeBlock
    {
        ThisVariable thisVariable = null;
        List<ParameterVariable> parameterVariables = new List<ParameterVariable>();
        Dictionary<string, ParameterVariable> paramMaps = new Dictionary<string, ParameterVariable>();
        List<Variable> escapingVariables = new List<Variable>();

        ReadOnlyCollection<ParameterVariable> readonlyVariables;
        ReadOnlyCollection<Variable> readonlyEscapingVariables;

        public ParameterBlock(
            ClrContext context,
            Location location)
            : base(context, location)
        {
            this.readonlyVariables =
                new ReadOnlyCollection<ParameterVariable>(this.parameterVariables);

            this.readonlyEscapingVariables =
                new ReadOnlyCollection<Variable>(this.escapingVariables);
        }

        public IList<ParameterVariable> Parameters
        {
            get { return this.readonlyVariables; }
        }

        public IList<Variable> EscapingVariables
        { get { return this.readonlyEscapingVariables; } }

        public bool GetParameterVariable(string paramName, out ParameterVariable paramVar)
        {
            return this.paramMaps.TryGetValue(paramName, out paramVar);
        }

        public ThisVariable GetThisParameter()
        {
            return this.thisVariable;
        }

        public void AddEscapingVariable(Variable rv)
        {
            for (int iVar = 0; iVar < this.escapingVariables.Count; iVar++)
            {
                if (this.escapingVariables[iVar] == rv)
                {
                    return;
                }
            }

            rv.SetHoisted();
            this.escapingVariables.Add(rv);
        }

        public void AddParameter(ParameterDefinition paramDef)
        {
            var parameterVariable =
                new ParameterVariable(
                    paramDef,
                    this);

            this.paramMaps.Add(paramDef.Name, parameterVariable);

            this.parameterVariables.Add(parameterVariable);
        }

        public void AddThisParameter(ParameterDefinition paramDef)
        {
            var thisVariable =
                new ThisVariable(
                    paramDef,
                    this);

            this.paramMaps.Add(thisVariable.Name, thisVariable);

            this.parameterVariables.Add(thisVariable);

            this.thisVariable = thisVariable;
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            base.ProcessThroughPipeline(processor);
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            if (serializationInfo.Version >= 2
                && this.parameterVariables.Count > 0)
            {
                serializationInfo.AddValue(
                    "parameters",
                    this.parameterVariables,
                    delegate(ICustomSerializer s, ParameterVariable p)
                    {
                        s.AddValue("name", p.Name);
                        s.AddValue("type", p.Type.ToString());
                    });

                if (this.escapingVariables.Count > 0)
                {
                    serializationInfo.AddValue(
                        "escapingVariables",
                        this.readonlyEscapingVariables);
                }
            }

            base.Serialize(serializationInfo);
        }
    }
}