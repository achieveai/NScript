using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class BinaryExpression : SimpleExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public BinaryExpression(
            ExecutionContext context,
            InstructionBlock instructionBlock,
            params Expression[] expressions)
            : base(context, instructionBlock, expressions)
        {
            switch (this.Block.Instruction.CodeOp.Code)
            {
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Add:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.AddOvfSigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.AddOvfUnsigned:
                    this.Operator = BinaryOperator.Add;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.And:
                    this.Operator = BinaryOperator.BitwiseAnd;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfEqual:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.CheckEquals:
                    this.Operator = BinaryOperator.IdentityEquality;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfGreaterOrEqual:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfGreaterOrEqualUnsigned:
                    this.Operator = BinaryOperator.GreaterThanOrEqual;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.CheckGreater:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.CheckGreaterUnsigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfGreaterUnsigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfGreater:
                    this.Operator = BinaryOperator.GreaterThan;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfLessOrEqualUnsigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfLessOrEqual:
                    this.Operator = BinaryOperator.LessThanOrEqual;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.CheckLesser:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.CheckLesserUnsigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfLessThan:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfLessThanUnsigned:
                    this.Operator = BinaryOperator.LessThan;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfNotEqualsUnsigned:
                    this.Operator = BinaryOperator.ValueInequality;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.DivUnsigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Div:
                    this.Operator = BinaryOperator.Divide;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.MulOvf:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Mul:
                    this.Operator = BinaryOperator.Multiply;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Or:
                    this.Operator = BinaryOperator.BitwiseOr;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Remainder:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.RemainderUnsigned:
                    this.Operator = BinaryOperator.Modulus;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.ShiftLeft:
                    this.Operator = BinaryOperator.ShiftLeft;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.ShiftRightUnsigned:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.ShiftRight:
                    this.Operator = BinaryOperator.ShiftRight;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.SubOvf:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Sub:
                    this.Operator = BinaryOperator.Subtract;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Xor:
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Switch:
                    this.Operator = BinaryOperator.BitwiseExclusiveOr;
                    break;
            }
        }

        public BinaryExpression(
            ExecutionContext context,
            BinaryOperator binaryOperator,
            params Expression[] expressions)
            : base(context, null, expressions)
        {
            this.Operator = binaryOperator;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        { get { return AstType.BinaryExpression; } }

        public BinaryOperator Operator
        { get; private set; }

        public Expression LeftExpression
        {
            get { return this.DependentExpressions[0]; }
            private set { this.DependentExpressions[0] = value; }
        }

        public Expression RightExpression
        {
            get { return this.DependentExpressions[1]; }
            private set { this.DependentExpressions[1] = value; }
        }
        #endregion

        #region public functions
        public void Negate()
        {
            switch (this.Operator)
            {
                case BinaryOperator.IdentityEquality:
                    this.Operator = BinaryOperator.IdentityInequality;
                    break;
                case BinaryOperator.IdentityInequality:
                    this.Operator = BinaryOperator.IdentityEquality;
                    break;
                case BinaryOperator.ValueEquality:
                    this.Operator = BinaryOperator.ValueInequality;
                    break;
                case BinaryOperator.ValueInequality:
                    this.Operator = BinaryOperator.ValueEquality;
                    break;
                case BinaryOperator.LessThan:
                    this.Operator = BinaryOperator.GreaterThanOrEqual;
                    break;
                case BinaryOperator.LessThanOrEqual:
                    this.Operator = BinaryOperator.GreaterThan;
                    break;
                case BinaryOperator.GreaterThan:
                    this.Operator = BinaryOperator.LessThanOrEqual;
                    break;
                case BinaryOperator.GreaterThanOrEqual:
                    this.Operator = BinaryOperator.LessThan;
                    break;
                case BinaryOperator.BooleanOr:
                    this.Operator = BinaryOperator.BooleanAnd;
                    this.LeftExpression = BinaryExpression.NegateExpression(this.LeftExpression);
                    this.RightExpression = BinaryExpression.NegateExpression(this.RightExpression);
                    break;
                case BinaryOperator.BooleanAnd:
                    this.Operator = BinaryOperator.BooleanOr;
                    this.LeftExpression = BinaryExpression.NegateExpression(this.LeftExpression);
                    this.RightExpression = BinaryExpression.NegateExpression(this.RightExpression);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public static bool IsConditionalExpression(BinaryOperator op)
        {
            switch (op)
            {
                case BinaryOperator.IdentityEquality:
                case BinaryOperator.IdentityInequality:
                case BinaryOperator.ValueEquality:
                case BinaryOperator.ValueInequality:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                case BinaryOperator.BooleanAnd:
                case BinaryOperator.BooleanOr:
                    return true;
            }

            return false;
        }

        public static Expression NegateExpression(
            Expression exp)
        {
            if (exp is BinaryExpression)
            {
                ((BinaryExpression)exp).Negate();
            }
            else if (exp is UnaryExpression)
            {
                ((UnaryExpression)exp).Negate();
            }
            else
            {
                exp = new UnaryExpression(
                    exp.Context,
                    UnaryOperator.Not,
                    exp);
            }

            return exp;
        }

        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            string exp = "";
            switch (this.Operator)
            {
                case BinaryOperator.Add:
                    exp = "+";
                    break;
                case BinaryOperator.Subtract:
                    exp = "-";
                    break;
                case BinaryOperator.Multiply:
                    exp = "*";
                    break;
                case BinaryOperator.Divide:
                    exp = "/";
                    break;
                case BinaryOperator.Modulus:
                    exp = "%";
                    break;
                case BinaryOperator.ShiftLeft:
                    exp = "<<";
                    break;
                case BinaryOperator.ShiftRight:
                    exp = ">>";
                    break;
                case BinaryOperator.IdentityEquality:
                    exp = "==";
                    break;
                case BinaryOperator.IdentityInequality:
                    exp = "!=";
                    break;
                case BinaryOperator.ValueEquality:
                    exp = "==";
                    break;
                case BinaryOperator.ValueInequality:
                    exp = "!=";
                    break;
                case BinaryOperator.BitwiseOr:
                    exp = "|";
                    break;
                case BinaryOperator.BitwiseAnd:
                    exp = "&";
                    break;
                case BinaryOperator.BitwiseExclusiveOr:
                    exp = "^";
                    break;
                case BinaryOperator.BooleanOr:
                    exp = "||";
                    break;
                case BinaryOperator.BooleanAnd:
                    exp = "&&";
                    break;
                case BinaryOperator.LessThan:
                    exp = "<";
                    break;
                case BinaryOperator.LessThanOrEqual:
                    exp = "<=";
                    break;
                case BinaryOperator.GreaterThan:
                    exp = ">";
                    break;
                case BinaryOperator.GreaterThanOrEqual:
                    exp = ">=";
                    break;
                default:
                    break;
            }

            textWriter.Write("");
            this.LeftExpression.Write(
                textWriter);
            textWriter.Write(" {0} ", exp);
            this.RightExpression.Write(
                textWriter);
            textWriter.Write("");
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
