using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    enum ConstType
    {
        String,
        Double,
        Int,
        Null
    }

    class LoadConstant : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadConstant(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, instruction, expressions)
        {
            if (instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadConstantDouble ||
                instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadConstantFloat)
            {
                this.ConstType = Ast.ConstType.Double;
                this.DoubleValue = (double)instruction.Instruction.OpCodeArgument;
            }
            else if (instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadConstantInt)
            {
                this.ConstType = Ast.ConstType.Int;
                this.IntValue = (int)instruction.Instruction.OpCodeArgument;
            }
            else if (instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadString)
            {
                this.ConstType = Ast.ConstType.String;
                this.StringValue = (string)instruction.Instruction.OpCodeArgument;
            }
            else if (instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadNull)
            {
                this.ConstType = Ast.ConstType.Null;
                this.IsNull = true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ConstType ConstType
        { get; private set; }

        public string StringValue
        { get; private set; }

        public int? IntValue
        { get; private set; }

        public double? DoubleValue
        { get; private set; }

        public bool IsNull
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write(prefix);
            switch (this.ConstType)
            {
                case ConstType.String:
                    textWriter.Write(this.StringValue);
                    break;
                case ConstType.Double:
                    textWriter.Write(this.DoubleValue);
                    break;
                case ConstType.Int:
                    textWriter.Write(this.IntValue);
                    break;
                case ConstType.Null:
                    textWriter.Write("null");
                    break;
                default:
                    break;
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
