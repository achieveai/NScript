namespace SpreadsheetApp.Formula
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Recursive-descent parser. Precedence, lowest first:
    /// comparison (= &lt;&gt; &lt; &gt; &lt;= &gt;=), concatenation (&amp;), additive (+ -),
    /// multiplicative (* /), power (^), unary sign, postfix percent, primary.
    /// </summary>
    public class Parser
    {
        private List<Token> tokens;
        private int index;

        private Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.index = 0;
        }

        /// <summary>Parses formula text without the leading '='.</summary>
        public static Node Parse(string text)
        {
            var parser = new Parser(new Tokenizer(text).Tokenize());
            Node node = parser.ParseComparison();
            if (parser.Current.Kind != Token.End)
                throw new FormulaException("#NAME?", "Unexpected '" + parser.Current.Text + "' at " + parser.Current.Position.ToString());
            return node;
        }

        private Token Current
        {
            get { return this.tokens[this.index]; }
        }

        private Token Advance()
        {
            Token t = this.tokens[this.index];
            if (t.Kind != Token.End) this.index++;
            return t;
        }

        private bool IsOp(string op)
        {
            return this.Current.Kind == Token.Op && this.Current.Text == op;
        }

        private Node ParseComparison()
        {
            Node left = this.ParseConcat();
            while (this.IsOp("=") || this.IsOp("<>") || this.IsOp("<") || this.IsOp(">") || this.IsOp("<=") || this.IsOp(">="))
            {
                string op = this.Advance().Text;
                left = Node.MakeBinary(op, left, this.ParseConcat());
            }
            return left;
        }

        private Node ParseConcat()
        {
            Node left = this.ParseAdditive();
            while (this.IsOp("&"))
            {
                this.Advance();
                left = Node.MakeBinary("&", left, this.ParseAdditive());
            }
            return left;
        }

        private Node ParseAdditive()
        {
            Node left = this.ParseMultiplicative();
            while (this.IsOp("+") || this.IsOp("-"))
            {
                string op = this.Advance().Text;
                left = Node.MakeBinary(op, left, this.ParseMultiplicative());
            }
            return left;
        }

        private Node ParseMultiplicative()
        {
            Node left = this.ParsePower();
            while (this.IsOp("*") || this.IsOp("/"))
            {
                string op = this.Advance().Text;
                left = Node.MakeBinary(op, left, this.ParsePower());
            }
            return left;
        }

        private Node ParsePower()
        {
            Node left = this.ParseUnary();
            if (this.IsOp("^"))
            {
                this.Advance();
                // Right-associative: 2^3^2 = 2^(3^2).
                return Node.MakeBinary("^", left, this.ParsePower());
            }
            return left;
        }

        private Node ParseUnary()
        {
            if (this.IsOp("-") || this.IsOp("+"))
            {
                string op = this.Advance().Text;
                return Node.MakeUnary(op, this.ParseUnary());
            }
            return this.ParsePostfix();
        }

        private Node ParsePostfix()
        {
            Node node = this.ParsePrimary();
            while (this.IsOp("%"))
            {
                this.Advance();
                node = Node.MakePercent(node);
            }
            return node;
        }

        private Node ParsePrimary()
        {
            Token t = this.Current;
            if (t.Kind == Token.Number)
            {
                this.Advance();
                return Node.MakeNumber(t.NumberValue);
            }
            if (t.Kind == Token.TextKind)
            {
                this.Advance();
                return Node.MakeText(t.Text);
            }
            if (t.Kind == Token.ErrorKind)
            {
                this.Advance();
                return Node.MakeError(t.Text);
            }
            if (t.Kind == Token.Ref)
            {
                this.Advance();
                if (this.Current.Kind == Token.Colon)
                {
                    this.Advance();
                    Token end = this.Advance();
                    if (end.Kind != Token.Ref)
                        throw new FormulaException("#NAME?", "Expected a cell after ':' at " + end.Position.ToString());
                    return Node.MakeRange(t.RefValue, end.RefValue);
                }
                return Node.MakeRef(t.RefValue);
            }
            if (t.Kind == Token.LParen)
            {
                this.Advance();
                Node inner = this.ParseComparison();
                this.Expect(Token.RParen, ")");
                return inner;
            }
            if (t.Kind == Token.Name)
            {
                this.Advance();
                if (t.Text == "TRUE") return Node.MakeBool(true);
                if (t.Text == "FALSE") return Node.MakeBool(false);
                this.Expect(Token.LParen, "(");
                var args = new List<Node>();
                if (this.Current.Kind != Token.RParen)
                {
                    while (true)
                    {
                        args.Add(this.ParseComparison());
                        if (this.Current.Kind == Token.Comma) { this.Advance(); continue; }
                        break;
                    }
                }
                this.Expect(Token.RParen, ")");
                return Node.MakeCall(t.Text, args);
            }
            throw new FormulaException("#NAME?", "Unexpected '" + t.Text + "' at " + t.Position.ToString());
        }

        private void Expect(int kind, string what)
        {
            if (this.Current.Kind != kind)
                throw new FormulaException("#NAME?", "Expected '" + what + "' at " + this.Current.Position.ToString());
            this.Advance();
        }
    }
}
