//-----------------------------------------------------------------------
// <copyright file="IdentifierStringExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for IdentifierStringExpression
    /// </summary>
    public class IdentifierStringExpression : Expression
    {
        /// <summary>
        /// Inner expression.
        /// </summary>
        private readonly Expression expression;

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
            this.expression = expression;
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
        public Expression Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>The precendence.</value>
        public override Precedence Precedence
        {
            get { return JST.Precedence.Primary; }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(Cs2JsC.Utils.ICustomSerializer serializer)
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
            IdentifierStringExpression identifierStringExpression = expression as IdentifierStringExpression;
            if (identifierStringExpression != null)
            {
                return identifierStringExpression.prependString
                    + IdentifierStringExpression.GetString(identifierStringExpression.Expression)
                    + identifierStringExpression.appendString;
            }

            IdentifierExpression identifierExpression = expression as IdentifierExpression;
            if (identifierExpression != null)
            {
                return identifierExpression.Identifier.GetName();
            }

            IndexExpression indexExpression = expression as IndexExpression;
            return IdentifierStringExpression.GetString(indexExpression.LeftExpression)
                + "."
                + ((IdentifierExpression)indexExpression.RightExpression).Identifier.GetName();
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
