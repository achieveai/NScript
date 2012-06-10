namespace System.Xml
{
    using System;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Imported]
    public class XmlNode
    {
        internal XmlNode()
        {
        }

        public XmlNode AppendChild(XmlNode child)
        {
            return null;
        }

        public XmlNode CloneNode(bool deepClone)
        {
            return null;
        }

        public XmlNodeList GetElementsByTagName(string tagName)
        {
            return null;
        }

        public bool HasChildNodes()
        {
            return false;
        }

        public XmlNode InsertBefore(XmlNode child, XmlNode refChild)
        {
            return null;
        }

        public XmlNode RemoveChild(XmlNode child)
        {
            return null;
        }

        public XmlNode ReplaceChild(XmlNode child, XmlNode oldChild)
        {
            return null;
        }

        public XmlNodeList SelectNodes(string xpath)
        {
            return null;
        }

        public XmlNode SelectSingleNode(string xpath)
        {
            return null;
        }

        public string TransformNode(XmlDocument stylesheet)
        {
            return null;
        }

        [IntrinsicProperty]
        public XmlNamedNodeMap Attributes
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public string BaseName
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNodeList ChildNodes
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNode FirstChild
        {
            get
            {
                return null;
            }
        }

        [ScriptName("text"), IntrinsicProperty]
        public string InnerText
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNode LastChild
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty, ScriptName("nodeName")]
        public virtual string Name
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNode NextSibling
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNodeType NodeType
        {
            get
            {
                return (XmlNodeType) 0;
            }
        }

        [IntrinsicProperty, ScriptName("xml")]
        public string OuterXml
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlDocument OwnerDocument
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNode ParentNode
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public string Prefix
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public XmlNode PreviousSibling
        {
            get
            {
                return null;
            }
        }

        [ScriptName("nodeValue"), IntrinsicProperty]
        public virtual string Value
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

