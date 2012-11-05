namespace System.Collections
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, PsudoType]
    public sealed class Dictionary : IEnumerable
    {
        public extern Dictionary();

        public extern void Clear();

        [Script(@"return typeof(this[key]) != ""undefined"";")]
        public extern bool ContainsKey(string key);

        [Script(@"return o;")]
        public extern static Dictionary GetDictionary(object o);

        [Script(@"delete this[key];")]
        public extern void Remove(string key);

        extern IEnumerator IEnumerable.GetEnumerator();

        public extern int Count { get; }

        public extern object this[string key] { get; set; }
    }
}