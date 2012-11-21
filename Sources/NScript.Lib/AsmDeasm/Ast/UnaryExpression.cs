using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class UnaryExpression : SimpleExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public UnaryExpression(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, instruction, expressions)
        {
            switch (instruction.Instruction.CodeOp.Code)
            {
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfFalse:
                    this.Operator = UnaryOperator.Not;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.BranchIfTrue:
                    this.Operator = UnaryOperator.NoOp;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Neg:
                    this.Operator = UnaryOperator.Negate;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Not:
                    this.Operator = UnaryOperator.Not;
                    break;
                case NScript.Lib.AsmDeasm.Ops.IlOpCode.Pop:
                    this.Operator = UnaryOperator.NoOp;
                    break;
            }

            if (this.Operator == UnaryOperator.Not &&
                this.DependentExpressions[0].NodeType == AstType.BinaryExpression &&
                BinaryExpression.IsConditionalExpression(((BinaryExpression)this.DependentExpressions[0]).Operator))
            {
                ((BinaryExpression)this.DependentExpressions[0]).Negate();
                this.Negate();
            }
        }

        public UnaryExpression(
            ExecutionContext context,
            UnaryOperator unaryOperator,
            params Expression[] expressions)
            : base(context, null, expressions)
        {
            this.Operator = unaryOperator;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.UnaryExpression; }
        }

        public UnaryOperator Operator
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.Operator == UnaryOperator.NoOp)
            {
                this.DependentExpressions[0].Write(textWriter, prefix, indentation);
            }
            else
            {
                textWriter.Write("{0} {1}(",
                    prefix,
                    this.Operator == UnaryOperator.Not ? "!" : "-");

                this.DependentExpressions[0].Write(textWriter);
                textWriter.Write(")");
            }

            if (this.IsStatement)
            {
                textWriter.Write(";");
            }
        }

        public void Negate()
        {
            if (this.Operator == UnaryOperator.Not)
            {
                this.Operator = UnaryOperator.NoOp;
            }
            else if (this.Operator == UnaryOperator.NoOp)
            {
                if (this.DependentExpressions[0].NodeType == AstType.BinaryExpression &&
                    BinaryExpression.IsConditionalExpression(((BinaryExpression)this.DependentExpressions[0]).Operator))
                {
                    ((BinaryExpression)this.DependentExpressions[0]).Negate();
                }
                else if (this.DependentExpressions[0].NodeType == AstType.UnaryExpression)
                {
                    ((UnaryExpression)this.DependentExpressions[0]).Negate();
                }
                else
                {
                    this.Operator = UnaryOperator.Not;
                }
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
