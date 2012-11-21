using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.MetaData;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class LoadField : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadField(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, instruction, expressions)
        {
            this.Field = (FieldSignature)instruction.Instruction.OpCodeArgument;

            if (instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadStaticFieldAddress ||
                instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadStaticField)
            {
                this.IsStatic = true;
            }

            if (instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadStaticFieldAddress ||
                instruction.Instruction.CodeOp.Code == Ops.IlOpCode.LoadFieldAddress)
            {
                this.IsReference = true;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public FieldSignature Field
        { get; private set; }

        public MethodSignature Method
        { get; private set; }

        public bool IsStatic
        { get; private set; }

        public bool IsReference
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.IsStatic)
            {
                textWriter.Write("{0}{1}.{2}",
                    prefix,
                    this.Context.ResolveClassName(this.Field.Class),
                    this.Context.ResolveFieldName(this.Field));

                return;
            }
            else
            {
                Expression.WriteWrapped(this.DependentExpressions[0], textWriter);
            }

            textWriter.Write(".{0}",
                this.Context.ResolveFieldName(this.Field));
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
