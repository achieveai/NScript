//-----------------------------------------------------------------------
// <copyright file="TupleDeconstructExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for TupleDeconstructExpression
    /// </summary>
    public class TupleDeconstructExpression : Expression
    {
        public TupleDeconstructExpression(
            ClrContext context,
            Utils.Location location,
            Expression[] leftExpressions,
            Expression rightExpression,
            bool isMethodCall)
            : base(context, location)
        {
            LeftExpressions = leftExpressions;
            RightExpression = rightExpression;
            IsMethodCall = isMethodCall;
        }

        public Expression[] LeftExpressions { get; }

        public Expression RightExpression { get; }

        public readonly bool IsMethodCall;

        public override bool Equals(object obj)
            => obj is TupleDeconstructExpression tupleDeconstruct
                && tupleDeconstruct.LeftExpressions.Length == this.LeftExpressions.Length
                && tupleDeconstruct.RightExpression.Equals(this.RightExpression)
                && Enumerable.Range(0, this.LeftExpressions.Length)
                    .All(idx => tupleDeconstruct.LeftExpressions[idx].Equals(this.LeftExpressions[idx]));

        public override int GetHashCode()
            => typeof(TupleDeconstructExpression).GetHashCode()
                ^ this.RightExpression.GetHashCode()
                ^ Enumerable.Range(0, this.LeftExpressions.Length)
                    .Select(idx =>
                    {
                        var hash = this.LeftExpressions[idx].GetHashCode();
                        return ((hash << (idx % 32)) + (hash >> (32 - idx % 32))) ^ idx;
                    })
                    .Sum();

        public override TypeReference ResultType => RightExpression.ResultType;
    }
}
