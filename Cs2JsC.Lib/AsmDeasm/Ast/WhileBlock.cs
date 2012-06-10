using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    class WhileBlock : BaseBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public WhileBlock(
            ExecutionContext context,
            Expression conditional,
            bool isDoWhileLoop,
            IList<IAstNode> innerBlocks)
        {
            this.Conditional = conditional;
            this.Context = context;
            this.IsDoWhileLoop = isDoWhileLoop;
            this.InnerBlocks = innerBlocks;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ExecutionContext Context
        { get; private set; }

        public Expression Conditional
        { get; private set; }

        public IList<IAstNode> InnerBlocks
        { get; private set; }

        public override AstType NodeType
        {
            get { return AstType.Block; }
        }

        public bool IsDoWhileLoop { get; private set; }
        #endregion

        #region public functions
        public override void Write(
            System.IO.TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            if (this.IsDoWhileLoop)
            {
                textWriter.Write("{0}do{0}{{",
                    prefix);
            }
            else
            {
                textWriter.Write("{0}while (",
                    prefix);
                this.Conditional.Write(textWriter);
                textWriter.Write("){0}{{", prefix);
            }

            string newPrefix = prefix + indentation;
            foreach (var astNode in this.InnerBlocks)
            {
                astNode.Write(textWriter, newPrefix, indentation);
            }

            if (this.IsDoWhileLoop)
            {
                textWriter.Write("{0}}}{0}while (",
                    prefix);
                this.Conditional.Write(textWriter);
                textWriter.Write(");");
            }
            else
            {
                textWriter.Write("{0}}}",
                    prefix);
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
