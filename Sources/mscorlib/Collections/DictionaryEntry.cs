namespace System.Collections
{
    using System;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public sealed class DictionaryEntry
    {
        internal extern DictionaryEntry();

        public extern string Key
        { get; }

        public extern object Value
        { get; }
    }
}

