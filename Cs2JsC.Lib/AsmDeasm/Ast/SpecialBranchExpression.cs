using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class SpecialBranchExpression : BaseBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public SpecialBranchExpression(
            ExecutionContext context,
            bool isBreak)
        {
            this.IsBreak = isBreak;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public override AstType NodeType
        {
            get { return AstType.Block; }
        }

        public bool IsBreak
        { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write("{0}{1};",
                prefix,
                this.IsBreak ? "break" : "continue");
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
