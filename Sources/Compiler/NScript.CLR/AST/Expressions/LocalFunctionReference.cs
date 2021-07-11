//-----------------------------------------------------------------------
// <copyright file="LocalFunctionReference.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;
    using System;

    /// <summary>
    /// Definition for LocalFunctionReference
    /// </summary>
    public class LocalFunctionReference
        : Expression
    {
        private readonly LocalFunctionVariable _variable;
        private readonly TypeReference _returnType;

        public LocalFunctionReference(
            ClrContext context,
            Location location,
            LocalFunctionVariable variable,
            TypeReference returnType)
            : base(context, location)
        {
            _variable = variable;
            _returnType = returnType;
        }

        public LocalFunctionVariable Variable
            => _variable;

        public TypeReference ReturnType
            => _returnType;

        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("variable", _variable);
        }

        public override bool Equals(object obj)
        {
            var right = obj as VariableReference;

            return right != null
                && this.Variable.Name.Equals(right.Variable.Name);
        }

        public override int GetHashCode()
        {
            return typeof(VariableReference).GetHashCode()
                ^ this.Variable.Name.GetHashCode();
        }
    }
}
