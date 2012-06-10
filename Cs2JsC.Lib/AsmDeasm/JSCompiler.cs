using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using System.IO;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm
{
    class JSCompiler
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public JSCompiler(
            ExecutionContext context,
            RootBlock rootBlock)
        {
            this.RootBlock = rootBlock;
            this.Context = context;
            this.NewLine = "\r\n";
            this.Indentation = string.Empty;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public RootBlock RootBlock { get; private set; }
        public ExecutionContext Context { get; private set; }

        public string NewLine
        { get; set; }

        public string Indentation
        { get; set; }
        #endregion

        #region public functions
        public void Write(
            TextWriter writer,
            string prefix = "\r\n",
            string indentation = "    ")
        {
            foreach (var expression in this.RootBlock.ToExpressions(this.Context, new Stack<Ast.Expression>()))
            {
                expression.Write(
                    writer,
                    prefix,
                    indentation);
            }

            writer.WriteLine(string.Empty);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
