using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Cs2JsC.Lib.MetaData;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class FieldAssignement : AssignementExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public FieldAssignement(
            ExecutionContext context,
            InstructionBlock instruction,
            bool isStatement,
            params Expression[] expressions)
            : base(
                context,
                instruction,
                expressions.Length == 2 ? expressions[1] : expressions[0],
                isStatement,
                expressions)
        {
            this.Field = (FieldSignature)instruction.Instruction.OpCodeArgument;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public FieldSignature Field
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.Field.IsStatic)
            {
                textWriter.Write("{0}{1}.{2} = ",
                    prefix,
                    this.Field.Class,
                    this.Field.Name);

                this.Value.Write(textWriter);
            }
            else
            {
                Expression.WriteWrapped(this.DependentExpressions[0], textWriter);

                textWriter.Write(".{0} = ",
                    this.Field.Name);

                this.Value.Write(textWriter);
            }

            if (this.IsStatement)
            {
                textWriter.Write(';');
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
