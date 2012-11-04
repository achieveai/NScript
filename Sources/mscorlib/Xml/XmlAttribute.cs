namespace System.Xml
{
    using System;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public sealed class XmlAttribute : XmlNode
    {
        internal XmlAttribute()
        {
        }

        [IntrinsicProperty]
        public override string Name
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public bool Specified
        {
            get
            {
                return false;
            }
        }

        [IntrinsicProperty]
        public override string Value
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

