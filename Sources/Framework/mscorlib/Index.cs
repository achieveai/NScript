namespace System
{
    /// <summary>
    /// Represents a type that can be used to index a collection either from
    /// the start or the end. Mirrors the C# 8 <c>System.Index</c> shape so
    /// that Roslyn binds <c>^x</c> and <c>arr[^x]</c> against this facade.
    /// </summary>
    public readonly struct Index
    {
        private readonly int _value;

        public Index(int value, bool fromEnd = false)
        {
            this._value = fromEnd ? ~value : value;
        }

        public static Index Start => new Index(0, fromEnd: false);

        public static Index End => new Index(0, fromEnd: true);

        public static Index FromStart(int value) => new Index(value, fromEnd: false);

        public static Index FromEnd(int value) => new Index(value, fromEnd: true);

        public int Value => this._value < 0 ? ~this._value : this._value;

        public bool IsFromEnd => this._value < 0;

        public int GetOffset(int length)
        {
            int offset = this._value;
            if (offset < 0)
            {
                // _value stores `~value` for from-end, so `_value + length + 1 == length - value`.
                offset += length + 1;
            }

            return offset;
        }

        public static implicit operator Index(int value) => new Index(value, fromEnd: false);
    }
}
