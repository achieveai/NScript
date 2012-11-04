namespace System.Collections
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended, ScriptName("Object")]
    public sealed class Dictionary : IEnumerable
    {
        public extern Dictionary();

        public extern void Clear();

        [MakeStaticUsage]
        [Script(@"return typeof(this[key]) != ""undefined"";")]
        public extern bool ContainsKey(string key);

        [MakeStaticUsage]
        [Script(@"return this;")]
        public extern static Dictionary GetDictionary(object o);

        [Script(@"delete this[key];")]
        public extern void Remove(string key);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        public int Count
        {
            get
            {
                return 0;
            }
        }

        [IntrinsicProperty]
        public extern object this[string key]
        {
            get;
            set;
        }
    }
}