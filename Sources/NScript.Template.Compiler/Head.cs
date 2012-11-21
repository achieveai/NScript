using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NScript.Template.Compiler
{
    public class Head
    {
        List<Style> styles = new List<Style>();

        private Head(Html html)
        { }

        private void AddStyle(Style style)
        {
            this.styles.Add(style);
        }

        public static Head Parse(
            Html html,
            XmlReader reader)
        {
            Head returnValue = new Head(html);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "noscript")
                    {
                        TemplateParser.Parse(reader, html);
                    }
                    else
                    {
                        var subTree = reader.ReadSubtree();
                        subTree.Read();

                        if (subTree.Name == "style")
                        {
                            Style style = Style.ParseStyles(
                                reader.ReadElementContentAsString(),
                                html.StyleMapping);

                            returnValue.styles.Add(style);
                        }
                    }
                }
            }

            return returnValue;
        }

        public List<Style> Styles
        {
            get { return this.styles; }
        }
    }
}
