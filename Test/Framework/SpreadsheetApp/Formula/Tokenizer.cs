namespace SpreadsheetApp.Formula
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Splits formula text (without the leading '=') into tokens. Characters
    /// are handled as integer codes because NScript compiles char to number.
    /// </summary>
    public class Tokenizer
    {
        private const int Dollar = 36;
        private const int Quote = 34;
        private const int Dot = 46;
        private const int Underscore = 95;
        private const int Hash = 35;

        private string text;
        private int pos;

        public Tokenizer(string text)
        {
            this.text = text;
            this.pos = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (true)
            {
                Token t = this.Next();
                tokens.Add(t);
                if (t.Kind == Token.End) break;
            }
            return tokens;
        }

        private int CodeAt(int index)
        {
            return index < this.text.Length ? (int)this.text.CharCodeAt(index) : -1;
        }

        private int Peek(int offset)
        {
            return this.CodeAt(this.pos + offset);
        }

        private static bool IsDigit(int c) { return c >= 48 && c <= 57; }
        private static bool IsLetter(int c) { return (c >= 65 && c <= 90) || (c >= 97 && c <= 122); }
        private static bool IsSpace(int c) { return c == 32 || c == 9 || c == 10 || c == 13; }

        private Token Next()
        {
            while (IsSpace(this.Peek(0))) this.pos++;
            int start = this.pos;
            int c = this.Peek(0);
            if (c < 0) return new Token(Token.End, "", start);

            if (IsDigit(c) || (c == Dot && IsDigit(this.Peek(1))))
                return this.ReadNumber(start);

            if (c == Quote)
                return this.ReadText(start);

            if (c == Dollar || IsLetter(c))
                return this.ReadWord(start);

            if (c == Hash)
                return this.ReadError(start);

            this.pos++;
            string one = this.text.Substring(start, 1);
            if (c == 40) return new Token(Token.LParen, one, start);
            if (c == 41) return new Token(Token.RParen, one, start);
            if (c == 44 || c == 59) return new Token(Token.Comma, one, start);
            if (c == 58) return new Token(Token.Colon, one, start);

            // Two-character comparison operators.
            int n = this.Peek(0);
            if (c == 60 && n == 62) { this.pos++; return new Token(Token.Op, "<>", start); }
            if (c == 60 && n == 61) { this.pos++; return new Token(Token.Op, "<=", start); }
            if (c == 62 && n == 61) { this.pos++; return new Token(Token.Op, ">=", start); }

            // + - * / ^ & = < > %
            if (c == 43 || c == 45 || c == 42 || c == 47 || c == 94 || c == 38
                || c == 61 || c == 60 || c == 62 || c == 37)
                return new Token(Token.Op, one, start);

            throw new FormulaException("#NAME?", "Unexpected character '" + one + "' at " + start.ToString());
        }

        private Token ReadNumber(int start)
        {
            while (IsDigit(this.Peek(0))) this.pos++;
            if (this.Peek(0) == Dot)
            {
                this.pos++;
                while (IsDigit(this.Peek(0))) this.pos++;
            }
            int e = this.Peek(0);
            bool exponent = (e == 69 || e == 101)
                && (IsDigit(this.Peek(1)) || ((this.Peek(1) == 43 || this.Peek(1) == 45) && IsDigit(this.Peek(2))));
            if (exponent)
            {
                this.pos = this.pos + 2;
                while (IsDigit(this.Peek(0))) this.pos++;
            }
            string s = this.text.Substring(start, this.pos - start);
            var t = new Token(Token.Number, s, start);
            t.NumberValue = double.Parse(s);
            return t;
        }

        private Token ReadText(int start)
        {
            this.pos++; // opening quote
            string value = "";
            while (true)
            {
                int c = this.Peek(0);
                if (c < 0) throw new FormulaException("#NAME?", "Unterminated text at " + start.ToString());
                this.pos++;
                if (c == Quote)
                {
                    if (this.Peek(0) == Quote) { this.pos++; value = value + "\""; continue; }
                    break;
                }
                value = value + string.FromCharCode((char)c);
            }
            return new Token(Token.TextKind, value, start);
        }

        /// <summary>An error literal such as #REF! or #NAME?: '#' up to and including '!' or '?'.</summary>
        private Token ReadError(int start)
        {
            int p = this.pos + 1;
            while (true)
            {
                int c = this.CodeAt(p);
                if (c < 0) throw new FormulaException("#NAME?", "Bad error literal at " + start.ToString());
                p++;
                if (c == 33 || c == 63) break;
            }
            this.pos = p;
            return new Token(Token.ErrorKind, this.text.Substring(start, p - start).ToUpperCase(), start);
        }

        private Token ReadWord(int start)
        {
            // Try an A1 reference first: [$]letters[$]digits not followed by more word characters.
            bool colAbs = false;
            int p = this.pos;
            if (this.CodeAt(p) == Dollar) { colAbs = true; p++; }
            int letterStart = p;
            while (IsLetter(this.CodeAt(p))) p++;
            int letterEnd = p;
            bool rowAbs = false;
            if (letterEnd > letterStart && this.CodeAt(p) == Dollar) { rowAbs = true; p++; }
            int digitStart = p;
            while (IsDigit(this.CodeAt(p))) p++;
            int digitEnd = p;
            int after = this.CodeAt(p);
            bool wordContinues = IsLetter(after) || IsDigit(after) || after == Underscore;

            bool looksLikeRef = letterEnd > letterStart && letterEnd - letterStart <= 3
                && digitEnd > digitStart && !wordContinues;
            if (looksLikeRef)
            {
                int col = ParseColumn(this.text.Substring(letterStart, letterEnd - letterStart).ToUpperCase());
                int row = (int)double.Parse(this.text.Substring(digitStart, digitEnd - digitStart)) - 1;
                if (row >= 0)
                {
                    this.pos = p;
                    var t = new Token(Token.Ref, this.text.Substring(start, p - start), start);
                    t.RefValue = new CellRef(col, row, colAbs, rowAbs);
                    return t;
                }
            }

            if (colAbs)
                throw new FormulaException("#NAME?", "Bad reference at " + start.ToString());

            // Otherwise a name: letters, digits, underscore, dot.
            p = this.pos;
            while (true)
            {
                int c = this.CodeAt(p);
                if (IsLetter(c) || IsDigit(c) || c == Underscore || c == Dot) p++; else break;
            }
            this.pos = p;
            return new Token(Token.Name, this.text.Substring(start, p - start).ToUpperCase(), start);
        }

        /// <summary>"A" -> 0, "Z" -> 25, "AA" -> 26. Input must be upper case.</summary>
        public static int ParseColumn(string letters)
        {
            int col = 0;
            for (int i = 0; i < letters.Length; i++)
                col = col * 26 + ((int)letters.CharCodeAt(i) - 64);
            return col - 1;
        }
    }
}
