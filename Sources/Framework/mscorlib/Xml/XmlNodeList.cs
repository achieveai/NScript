namespace System.Xml
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public sealed class XmlNodeList : IEnumerable
    {
        internal XmlNodeList()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        [IntrinsicProperty, ScriptName("length")]
        public int Count
        {
            get
            {
                return 0;
            }
        }

        [IntrinsicProperty]
        public XmlNode this[int index]
        {
            get
            {
                return null;
            }
        }
    }
}

