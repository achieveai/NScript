namespace System.Xml
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace]
    public sealed class XmlDocument : XmlNode
    {
        internal XmlDocument()
        {
        }

        public XmlAttribute CreateAttribute(string name)
        {
            return null;
        }

        [ScriptName("createCDATASection")]
        public XmlNode CreateCDataSection(string data)
        {
            return null;
        }

        public XmlNode CreateComment(string text)
        {
            return null;
        }

        public XmlNode CreateElement(string tagName)
        {
            return null;
        }

        public XmlNode CreateEntityReference(string name)
        {
            return null;
        }

        public XmlNode CreateProcessingInstruction(string target, string data)
        {
            return null;
        }

        public XmlText CreateTextNode(string text)
        {
            return null;
        }

        [IntrinsicProperty]
        public string Doctype
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNode DocumentElement
        {
            get
            {
                return null;
            }
        }
    }
}

