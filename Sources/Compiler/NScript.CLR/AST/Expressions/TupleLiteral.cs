//-----------------------------------------------------------------------
// <copyright file="TupleLiteral.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using Mono.Cecil;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for TupleLiteral
    /// </summary>
    public class TupleLiteral : LiteralExpression
    {
        public TupleLiteral(
            ClrContext context,
            Location location,
            TypeReference tupleType,
            Expression[] tupleArgs)
            : base(context, location)
        {
            ResultType = tupleType;
            TupleArgs = tupleArgs;
        }

        public override TypeReference ResultType { get; }

        public Expression[] TupleArgs { get; }

        public override bool Equals(object obj)
            => obj is TupleLiteral tupleLiteral
                && tupleLiteral.TupleArgs.Length == this.TupleArgs.Length
                && Enumerable.Range(0, this.TupleArgs.Length)
                    .All(idx => tupleLiteral.TupleArgs[idx].Equals(this.TupleArgs[idx]));

        public override int GetHashCode()
            => typeof(TupleLiteral).GetHashCode()
                + Enumerable.Range(0, this.TupleArgs.Length)
                    .Select(idx =>
                    {
                        var hash = this.TupleArgs[idx].GetHashCode();
                        return ((hash << (idx % 32)) + (hash >> (32 - idx % 32))) ^ idx;
                    })
                    .Sum();
    }
}
