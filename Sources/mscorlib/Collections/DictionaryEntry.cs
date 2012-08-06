namespace System.Collections
{
    using System;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Imported]
    public sealed class DictionaryEntry
    {
        internal DictionaryEntry()
        {
        }

        [IntrinsicProperty]
        public extern string Key
        {
            get;
        }

        [IntrinsicProperty]
        public extern object Value
        {
            get;
        }
    }
}

