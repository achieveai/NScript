using System.Collections.Generic;
using NScript.Utils;
namespace NScript.JST
{
    /// <summary>
    /// Index expression is used for both dot member access and index.
    /// </summary>
    public class IndexExpression : Expression
    {
        /// <summary>
        /// Backing field for ForceIndexer.
        /// </summary>
        private readonly bool forceIndexer;

        /// <summary>
        /// Backing field for Left Expression.
        /// </summary>
        private readonly Expression leftExpression;

        /// <summary>
        /// Backing field for RightExpression.
        /// </summary>
        private readonly Expression rightExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexExpression"/> class.
        /// </summary>
        /// <param name="loc">The loc.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="rightExpression">The right expression.</param>
        public IndexExpression(
            Location loc,
            IdentifierScope scope,
            Expression leftExpression,
            Expression rightExpression,
            bool forceIndexer = false)
            : base(loc, scope)
        {
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
            this.forceIndexer = forceIndexer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexExpression"/> class.
        /// </summary>
        /// <param name="loc">The loc.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="indexes">The indexes.</param>
        public IndexExpression(
            Location loc,
            IdentifierScope scope,
            IList<IIdentifier> indexes,
            int lastIndex)
            : base(loc, scope)
        {
            this.rightExpression = new IdentifierExpression(indexes[--lastIndex], this.Scope);
            this.leftExpression = IdentifierExpression.Create(
                loc,
                scope,
                indexes,
                lastIndex);
        }

        /// <summary>
        /// Gets the left expression.
        /// </summary>
        /// <value>The left expression.</value>
        public Expression LeftExpression
        {
            get
            {
                return this.leftExpression;
            }
        }

        /// <summary>
        /// Gets the right expression.
        /// </summary>
        /// <value>The right expression.</value>
        public Expression RightExpression
        {
            get
            {
                return this.rightExpression;
            }
        }

        public bool ForceIndexer => forceIndexer;

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <value>The precedence.</value>
        public override Precedence Precedence
        {
            get
            {
                return Precedence.Primary;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is left to right.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is left to right; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeftToRight
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("collection", this.leftExpression);
            serializer.AddValue("index", this.RightExpression);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            IdentifierExpression identifierExpresison = this.leftExpression as IdentifierExpression;
            if (identifierExpresison != null && identifierExpresison.Identifier.IsEmpty)
            {
                writer.Write(this.RightExpression);
                return;
            }

            identifierExpresison = this.rightExpression as IdentifierExpression;
            if (identifierExpresison != null && identifierExpresison.Identifier.IsEmpty)
            {
                writer.Write(this.leftExpression);
                return;
            }

            writer.Write(
                this.LeftExpression,
                this.LeftExpression.OperatorPlacement != JST.OperatorPlacement.Postfix
                    && this.Precedence > this.LeftExpression.Precedence);

            // This is case optimized for virtual method names on interfaces.
            //
            BinaryExpression rightBinaryExpression = this.RightExpression as BinaryExpression;

            if (!this.forceIndexer
                && rightBinaryExpression != null
                && rightBinaryExpression.Operator == BinaryOperator.Plus
                && rightBinaryExpression.Left is IdentifierStringExpression
                && rightBinaryExpression.Right is StringLiteralExpression)
            {
                writer.Write(Symbols.Dot)
                    .WriteIdentifier(
                        IdentifierStringExpression.GetString(rightBinaryExpression.Left)
                        + ((StringLiteralExpression)rightBinaryExpression.Right)
                            .StringLiteral);
            }
            else if (!this.forceIndexer && rightExpression is IdentifierExpression)
            {
                writer.Write(Symbols.Dot)
                    .Write(this.rightExpression);
            }
            else if (!this.forceIndexer && rightExpression is StringLiteralExpression
                    && Utils.IsValidIdentifier(
                        ((StringLiteralExpression)rightExpression).StringLiteral))
            {
                StringLiteralExpression str = (StringLiteralExpression)rightExpression;

                writer.Write(Symbols.Dot)
                    .WriteIdentifier(str.StringLiteral);
            }
            else
            {
                writer.Write(Symbols.BrackedOpenSquare)
                    .Write(this.rightExpression)
                    .Write(Symbols.BracketCloseSquare);
            }
        }
    }
}
