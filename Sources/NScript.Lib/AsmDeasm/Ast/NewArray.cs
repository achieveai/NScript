using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ast
{
    class NewArray : Expression
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public NewArray(
            ExecutionContext context,
            InstructionBlock instruction,
            params Expression[] expressions)
            : base(context, expressions)
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.Constructor; }
        }
        #endregion

        #region public functions
        public override void Write(System.IO.TextWriter textWriter, string prefix = "", string indentation = "")
        {
            textWriter.Write("{0}new Array(", prefix);
            this.DependentExpressions[0].Write(textWriter);
            textWriter.Write(")");
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
