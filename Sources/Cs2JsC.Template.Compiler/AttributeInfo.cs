using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Cs2JsC.Template.Compiler
{
    public class AttributeInfo
    {
        string attributeNS;
        string localName;
        string prefix;
        string name;
        string value;
        int lineNumber;
        int columnNumber;

        public AttributeInfo(XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Attribute)
            {
                throw new ArgumentException();
            }

            this.attributeNS = reader.NamespaceURI;
            this.localName = reader.LocalName;
            this.name = reader.Name;
            this.prefix = reader.Prefix;
            this.value = reader.Value;

            var positionInfo = AttributeInfo.GetLineInfo(reader);

            lineNumber = positionInfo.Key;
            columnNumber = positionInfo.Value;
        }

        public string AttributeNS
        { get { return this.attributeNS; } }

        public string LocalName
        { get { return this.localName; } }

        public string Prefix
        { get { return this.prefix; } }

        public string Name
        { get { return this.name; } }

        public string Value
        { get { return this.value; } }

        public int LineNumber
        { get { return this.lineNumber; } }

        public int ColumnNumber
        { get { return this.columnNumber; } }

        public static KeyValuePair<int, int> GetLineInfo(XmlReader reader)
        {
            IXmlLineInfo lineInfo = reader as IXmlLineInfo;
            if (lineInfo != null)
            {
                return new KeyValuePair<int, int>(lineInfo.LineNumber, lineInfo.LinePosition);
            }
            else
            {
                return new KeyValuePair<int, int>(-1, -1);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}='{1}'", this.LocalName, this.Value);
        }
    }
}
