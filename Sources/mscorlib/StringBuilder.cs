namespace System
{
    using System.Runtime.CompilerServices;

    [ScriptNamespace("ss")]
    public sealed class StringBuilder
    {
        private NativeArray internalArray;

        public StringBuilder()
        {
            this.internalArray = new NativeArray(0);
        }

        public StringBuilder(string initialText)
        {
            this.internalArray = new NativeArray(0);
            this.Append(initialText);
        }

        public StringBuilder Append(bool b)
        {
            this.Append(b.ToString());
            return this;
        }

        public StringBuilder Append(char c)
        {
            this.Append(c.ToString());
            return this;
        }

        public StringBuilder Append(Number n)
        {
            this.Append(n.ToString());
            return this;
        }

        public StringBuilder Append(string s)
        {
            this.internalArray.Push(s);
            return this;
        }

        public StringBuilder AppendLine()
        {
            this.Append("\n");
            return this;
        }

        public StringBuilder AppendLine(bool b)
        {
            this.Append(b);
            this.AppendLine();
            return this;
        }

        public StringBuilder AppendLine(char c)
        {
            this.Append(c);
            this.AppendLine();
            return this;
        }

        public StringBuilder AppendLine(Number n)
        {
            this.Append(n);
            this.AppendLine();
            return this;
        }

        public StringBuilder AppendLine(string s)
        {
            this.Append(s);
            this.AppendLine();
            return this;
        }

        public void Clear()
        {
            this.internalArray = new NativeArray(0);
        }

        public override string ToString()
        {
            return this.internalArray.Join();
        }

        public string ToString(string separator)
        {
            return this.internalArray.Join(separator);
        }

        public bool IsEmpty
        {
            get
            {
                return this.internalArray.Length == 0;
            }
        }
    }
}

