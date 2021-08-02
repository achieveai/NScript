//-----------------------------------------------------------------------
// <copyright file="TupleDeconstructExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST.Statements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for TupleDeconstructExpression
    /// </summary>
    public class TupleDeconstructStatement : Statement
    {
        public TupleDeconstructStatement(
            ClrContext context,
            Utils.Location location,
            Expression[] leftExpressions,
            Expression rightExpression)
            : base(context, location)
        {
            LeftExpressions = leftExpressions;
            RightExpression = rightExpression;
        }

        public Expression[] LeftExpressions { get; }

        public Expression RightExpression { get; }

        public override bool Equals(object obj)
            => obj is TupleDeconstructStatement tupleDeconstruct
                && tupleDeconstruct.LeftExpressions.Length == this.LeftExpressions.Length
                && tupleDeconstruct.RightExpression.Equals(this.RightExpression)
                && Enumerable.Range(0, this.LeftExpressions.Length)
                    .All(idx => tupleDeconstruct.LeftExpressions[idx].Equals(this.LeftExpressions[idx]));

        public override int GetHashCode()
            => typeof(TupleDeconstructStatement).GetHashCode()
                ^ this.RightExpression.GetHashCode()
                ^ Enumerable.Range(0, this.LeftExpressions.Length)
                    .Select(idx =>
                    {
                        var hash = this.LeftExpressions[idx].GetHashCode();
                        return ((hash << (idx % 32)) + (hash >> (32 - idx % 32))) ^ idx;
                    })
                    .Sum();
    }
}
