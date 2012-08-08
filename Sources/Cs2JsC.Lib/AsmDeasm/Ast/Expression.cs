using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    abstract class Expression : IAstNode
    {
        #region member variables
        #endregion

        #region constructors/Factories
        protected Expression(ExecutionContext context, Expression[] expressions)
        {
            this.Context = context;
            this.DependentExpressions = expressions;

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    expression.Parent = this;
                }
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public abstract AstType NodeType
        { get; }

        public ExecutionContext Context
        { get; private set; }

        public Expression[] DependentExpressions
        { get; private set; }

        public Expression Parent
        { get; private set; }

        public bool IsStatement
        { get; set; }
        #endregion

        #region public functions
        public abstract void Write(
            TextWriter textWriter,
            string prefix = "",
            string indentation = "");

        public static void WriteWrapped(
            Expression expression,
            TextWriter textWriter,
            string prefix = "",
            string indentation = "")
        {
            bool needsWrapping = false;

            if (expression.NodeType == AstType.BinaryExpression ||
                expression.NodeType == AstType.UnaryExpression ||
                expression.NodeType == AstType.Constructor)
            {
                needsWrapping = true;
            }

            if (needsWrapping)
            {
                textWriter.Write('(');
            }

            expression.Write(textWriter, prefix, indentation);

            if (needsWrapping)
            {
                textWriter.Write(')');
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
