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
            Location location,
            List<(LocalVariable variable, bool isUsed)> variables,
            (List<Variable> escapingVars, List<ParameterVariable> paras, ThisVariable thisVar)
                paramBlockVariables)
            : base(context, location, variables)
        {
            this.readonlyVariables =
                new ReadOnlyCollection<ParameterVariable>(this.parameterVariables);

            this.readonlyEscapingVariables =
                new ReadOnlyCollection<Variable>(this.escapingVariables);

            if (paramBlockVariables.escapingVars != null)
            { paramBlockVariables.escapingVars.ForEach(_ => this.escapingVariables.Add(_)); }

            if (paramBlockVariables.thisVar != null)
            {
                if (paramBlockVariables.thisVar.DefiningScope != null)
                { throw new System.InvalidOperationException(); }

                this.thisVariable = paramBlockVariables.thisVar;
                this.thisVariable.DefiningScope = this;
                this.paramMaps.Add(thisVariable.Name, thisVariable);
                this.parameterVariables.Add(thisVariable);
            }

            if (paramBlockVariables.paras != null)
            {
                foreach (var _ in paramBlockVariables.paras)
                {
                    if (_.DefiningScope != null)
                    { throw new System.InvalidOperationException(); }

                    _.DefiningScope = this;
                    this.paramMaps.Add(_.Name, _);

                    this.parameterVariables.Add(_);
                }
            }
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