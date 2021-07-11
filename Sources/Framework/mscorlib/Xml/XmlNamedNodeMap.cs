namespace System.Xml
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public sealed class XmlNamedNodeMap : IEnumerable
    {
        internal XmlNamedNodeMap()
        {
        }

        public XmlNode GetNamedItem(string name)
        {
            return null;
        }

        public XmlNode RemoveNamedItem(string name)
        {
            return null;
        }

        public XmlNode SetNamedItem(XmlNode node)
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        [ScriptName("length"), IntrinsicProperty]
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

