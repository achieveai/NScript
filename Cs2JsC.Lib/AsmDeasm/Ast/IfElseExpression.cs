using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class IfElseExpression : BaseBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public IfElseExpression(
            Expression conditionalExpression,
            IList<IAstNode> ifNodes,
            IList<IAstNode> elseNodes)
        {
            this.Conditional = conditionalExpression;
            this.IfNodes = ifNodes;
            this.ElseNodes = elseNodes;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Expression Conditional
        { get; private set; }

        public IList<IAstNode> IfNodes
        { get; private set; }

        public IList<IAstNode> ElseNodes
        { get; private set; }

        public override AstType NodeType
        {
            get { return AstType.Block; }
        }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            textWriter.Write("{0}if (", prefix);
            this.Conditional.Write(textWriter);
            textWriter.Write("){0}{{", prefix);

            string newPrefix = prefix + indentation;
            foreach (var expression in this.IfNodes)
            {
                expression.Write(textWriter, newPrefix, indentation);
            }

            textWriter.Write("{0}}}", prefix);

            if (this.ElseNodes != null)
            {
                textWriter.Write("{0}else{0}{{", prefix);
                foreach (var expression in this.ElseNodes)
                {
                    expression.Write(textWriter, newPrefix, indentation);
                }
                textWriter.Write("{0}}}", prefix);
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
