//-----------------------------------------------------------------------
// <copyright file="IdentifierStringExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;
    using System.Text;

    /// <summary>
    /// Definition for IdentifierStringExpression
    /// </summary>
    public class IdentifierStringExpression : Expression
    {
        /// <summary>
        /// Inner expression.
        /// </summary>
        private readonly List<Expression> expressions;

        /// <summary>
        /// Backing field for premendString.
        /// </summary>
        private readonly string prependString;

        /// <summary>
        /// Backing field for appendString.
        /// </summary>
        private readonly string appendString;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierStringExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="prependString">The prepend string.</param>
        /// <param name="appendString">The append string.</param>
        public  IdentifierStringExpression(
            Location location,
            IdentifierScope scope,
            Expression expression,
            string prependString = "",
            string appendString = "")
            : base(location, scope)
        {
            this.expressions = new List<Expression>();
            this.expressions.Add(expression);
            this.prependString = prependString;
            this.appendString = appendString;

            if (!IdentifierStringExpression.VerifyExpression(expression))
            {
                throw new ArgumentException(
                    "expression should be type of either IndexExpression with nested with only IndexExpression and IdentifierExpressions");
            }
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public IList<Expression> Expression
        {
            get { return this.expressions; }
        }

        public string PrependString => this.prependString;

        public string AppendString => this.appendString;

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <value>The precendence.</value>
        public override Precedence Precedence
        {
            get { return JST.Precedence.Primary; }
        }

        /// <summary>
        /// Appends an expr.
        /// </summary>
        /// <param name="expr"> The expression to append. </param>
        public void Append(IdentifierExpression expr)
        {
            this.expressions.Add(expr);
        }

        /// <summary>
        /// Appends an expr.
        /// </summary>
        /// <param name="expr"> The expression to append. </param>
        public void Append(StringLiteralExpression expr)
        {
            this.expressions.Add(expr);
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("inner", this.Expression);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteStr(IdentifierStringExpression.GetString(this));
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>string for given expression</returns>
        public static string GetString(Expression expression)
        {
            StringBuilder sb = new StringBuilder();
            IdentifierStringExpression identifierStringExpression = expression as IdentifierStringExpression;
            if (identifierStringExpression != null)
            {
                sb.Append(identifierStringExpression.prependString);
                for (int iExpr = 0; iExpr < identifierStringExpression.expressions.Count; iExpr++)
                {
                    sb.Append(IdentifierStringExpression.GetString(
                        identifierStringExpression.expressions[iExpr]));
                }

                sb.Append(identifierStringExpression.appendString);
                return sb.ToString();
            }

            IdentifierExpression identifierExpression = expression as IdentifierExpression;
            if (identifierExpression != null)
            {
                return identifierExpression.Identifier.GetName();
            }

            IndexExpression indexExpression = expression as IndexExpression;
            if (indexExpression != null)
            {
                return IdentifierStringExpression.GetString(indexExpression.LeftExpression)
                    + "."
                    + ((IdentifierExpression)indexExpression.RightExpression).Identifier.GetName();
            }

            StringLiteralExpression stringLiteralExpression = expression as StringLiteralExpression;
            return stringLiteralExpression.StringLiteral;
        }

        /// <summary>
        /// Verifies the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>True if expression is good expression.</returns>
        private static bool VerifyExpression(Expression expression)
        {
            if (expression is IdentifierExpression)
            {
                return true;
            }

            IndexExpression indexExpression = expression as IndexExpression;

            if (indexExpression == null)
            {
                return false;
            }

            return IdentifierStringExpression.VerifyExpression(indexExpression.LeftExpression)
                && indexExpression.RightExpression is IdentifierExpression;
        }
    }
}
