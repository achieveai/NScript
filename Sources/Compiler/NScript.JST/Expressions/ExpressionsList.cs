//-----------------------------------------------------------------------
// <copyright file="ExpressionsList.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ExpressionsList
    /// </summary>
    public class ExpressionsList : Expression
    {
        private List<Expression> expressions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsList"/> class.
        /// </summary>
        /// <param name="location">    The location. </param>
        /// <param name="scope">       The scope. </param>
        /// <param name="makeUnit">    true to make unit (encapsulates result in ()). </param>
        /// <param name="expressions"> The expressions. </param>
        public ExpressionsList(
            Location location,
            IdentifierScope scope,
            IList<Expression> expressions)
            : base(location, scope)
        {
            this.expressions = new List<Expression>(expressions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsList"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="expressions">The expressions.</param>
        public ExpressionsList(
            Location location,
            IdentifierScope scope,
            params Expression[] expressions)
            : this(location, scope, (IList<Expression>)expressions)
        {
        }

        public IList<Expression> Expressions
        {
            get { return this.expressions; }
        }

        public override Precedence Precedence
        {
            get
            {
                return this.expressions.Count > 1
                    ? Precedence.Comma
                    : this.expressions[0].Precedence;
            }
        }

        public override void Write(JSWriter writer)
        {
            writer.Write(this.expressions[0]);

            for (int iExpr = 1; iExpr < this.expressions.Count; iExpr++)
            {
                writer.Write(Symbols.Comma).Write(this.expressions[iExpr]);
            }
        }

        public override void Serialize(ICustomSerializer serializer)
        {
            serializer.AddValue("expressions", this.expressions);
        }
    }
}