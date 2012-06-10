using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Cs2JsC.Lib.MetaData;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class LoadMethod : UnitExpression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public LoadMethod(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, instruction, expressions)
        {
            this.Method = (MethodSignature)instruction.Instruction.OpCodeArgument;

            this.IsVirtualMethod = this.DependentExpressions != null && this.DependentExpressions.Length > 0;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public MethodSignature Method
        { get; private set; }

        public bool IsVirtualMethod
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (!this.IsVirtualMethod)
            {
                textWriter.Write("{0}{1}.{2}",
                    prefix,
                    this.Context.ResolveClassName(this.Method.Class),
                    this.Context.ResolveMethodName(this.Method));
            }
            else
            {
                // Only virtual functions can be nonStatic functions.
                //
                textWriter.Write(prefix);

                Expression.WriteWrapped(
                    this.DependentExpressions[0],
                    textWriter);

                textWriter.Write(".{1}",
                    this.Context.ResolveVirutalMethodName(this.Method));
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
