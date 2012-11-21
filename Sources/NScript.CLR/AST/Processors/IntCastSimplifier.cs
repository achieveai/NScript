//-----------------------------------------------------------------------
// <copyright file="IntCastSimplifier.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST.Processors
{
    using System;
    using Mono.Cecil;


    /// <summary>
    /// Definition for IntCastSimplifier
    /// </summary>
    public static class IntCastSimplifier
    {
        /// <summary>
        /// Processes the binary expression.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// TypeCheckExpression if this expression results to is operator.
        /// </returns>
        public static Node ProcessTypeCheckExpression(
            IAstProcessor processor,
            TypeCheckExpression expression)
        {
            if (expression.CheckType != TypeCheckType.CastType)
            {
                return expression;
            }

            if (expression.Expression is IntLiteral)
            {
                long value = ((IntLiteral)expression.Expression).Value;

                switch (expression.Type.MetadataType)
                {
                    case MetadataType.Boolean:
                        return new BooleanLiteral(
                            expression.Context,
                            expression.Location,
                            value != 0);
                    case MetadataType.Byte:
                        return new UIntLiteral(
                            expression.Context,
                            expression.Location,
                            (byte)value);
                    case MetadataType.Char:
                        return new CharLiteral(
                            expression.Context,
                            expression.Location,
                            (char)value);
                    case MetadataType.Int16:
                        return new IntLiteral(
                            expression.Context,
                            expression.Location,
                            (short)value);
                    case MetadataType.Int32:
                        return new IntLiteral(
                            expression.Context,
                            expression.Location,
                            (int)value);
                    case MetadataType.Int64:
                        return new IntLiteral(
                            expression.Context,
                            expression.Location,
                            value);
                    case MetadataType.IntPtr:
                        return new IntLiteral(
                            expression.Context,
                            expression.Location,
                            (IntPtr)value);
                    case MetadataType.SByte:
                        return new IntLiteral(
                            expression.Context,
                            expression.Location,
                            (sbyte)value);
                    case MetadataType.UInt16:
                        return new IntLiteral(
                            expression.Context,
                            expression.Location,
                            (ushort)value);
                    case MetadataType.UInt32:
                        return new UIntLiteral(
                            expression.Context,
                            expression.Location,
                            (uint)value);
                    case MetadataType.UInt64:
                        return new UIntLiteral(
                            expression.Context,
                            expression.Location,
                            (ulong)value);
                    case MetadataType.UIntPtr:
                        return new UIntLiteral(
                            expression.Context,
                            expression.Location,
                            (UIntPtr)value);
                }
            }

            if (expression.Type.IsSame(expression.Expression.ResultType))
            {
                return expression.Expression;
            }

            return expression;
        }
    }
}
