using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using System.IO;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class Return : Expression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public Return(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, expressions)
        {
            if (instruction.Instruction.StackBefore > 0)
            {
                this.ReturnValue = expressions[0];
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Expression ReturnValue
        { get; private set; }

        public override AstType NodeType
        {
            get { return AstType.Return; }
        }
        #endregion

        #region public functions
        public override void Write(
            TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.ReturnValue != null)
            {
                textWriter.Write("{0}return ", prefix);
                this.ReturnValue.Write(textWriter);
                textWriter.Write(";");
            }
            else if (this.Context.MethodSignature.IsConstructor &&
                !this.Context.IsDefaultConstructor(this.Context.MethodSignature))
            {
                // This is to take care of the fact that JS does not support constructor overloads
                // so we use different init methods as overloads and they should return value so that
                // it can be assigned as new object.
                //
                textWriter.Write("{0}return this;", prefix);
            }
            else
            {
                textWriter.Write("{0}return;", prefix);
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
