using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Cs2JsC.Lib.MetaData;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class MethodCall : Expression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public MethodCall(
            ExecutionContext context,
            InstructionBlock instruction,
            bool isVirtual,
            params Expression[] expressions)
            : base (context, expressions)
        {
            this.Method = (MethodSignature)instruction.Instruction.OpCodeArgument;

            if (this.Method.Type == null)
            {
                this.IsStatement = true;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.Method; }
        }

        public MethodSignature Method
        { get; private set; }

        public bool IsVirtual
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.Method.IsStatic)
            {
                textWriter.Write("{0}{1}.{2}(",
                    prefix,
                    this.Context.ResolveClassName(this.Method.Class),
                    this.IsVirtual ?
                        this.Context.ResolveVirutalMethodName(this.Method) :
                        this.Context.ResolveMethodName(this.Method));
            }
            else
            {
                textWriter.Write(prefix);
                Expression.WriteWrapped(this.DependentExpressions[0], textWriter);
                textWriter.Write(".{0}(",
                    this.Context.ResolveMethodName(this.Method));
            }

            for (int i = 0; i < this.Method.Arguments.Count; i++)
            {
                if (i != 0)
                {
                    textWriter.Write(", ");
                }

                int adjustment = this.Method.IsStatic ? 0 : 1;
                this.DependentExpressions[i + adjustment].Write(textWriter);
            }

            textWriter.Write(")");

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
