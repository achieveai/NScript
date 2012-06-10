//-----------------------------------------------------------------------
// <copyright file="Html.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Html base template.
    /// </summary>
    public class Html
    {
        /// <summary>
        /// backing field for stypeMapping.
        /// </summary>
        private readonly StyleMapping styleMapping;

        /// <summary>
        /// List of templates.
        /// </summary>
        private readonly List<Template> templates = new List<Template>();

        /// <summary>
        /// Backing field for tempalteId.
        /// </summary>
        private readonly string templateId;

        /// <summary>
        /// Backing field for fileName
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// Backing field for head.
        /// </summary>
        private Head head;

        /// <summary>
        /// Initializes a new instance of the <see cref="Html"/> class.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <param name="fileName">Name of the file.</param>
        public Html(string templateId, string fileName, List<string> globalStyleNames)
        {
            this.fileName = fileName;
            this.templateId = templateId;
            this.styleMapping = new StyleMapping(this.templateId, globalStyleNames);
        }

        /// <summary>
        /// Gets the style mapping.
        /// </summary>
        /// <value>The style mapping.</value>
        public StyleMapping StyleMapping
        {
            get
            {
                return this.styleMapping;
            }
        }

        /// <summary>
        /// Gets the head.
        /// </summary>
        /// <value>The   head.</value>
        public Head Head
        {
            get
            {
                return this.head;
            }
        }

        /// <summary>
        /// Gets the template id.
        /// </summary>
        /// <value>The template id.</value>
        public string TemplateId
        {
            get
            {
                return this.templateId;
            }
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        /// <summary>
        /// Gets the templates.
        /// </summary>
        /// <value>The templates.</value>
        public List<Template> Templates
        {
            get
            {
                return this.templates;
            }
        }

        /// <summary>
        /// Parses the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="globalStyleNames">List of global style names.</param>
        /// <returns>Html template.</returns>
        public static Html Parse(string fileName, List<ErrorInfo> errors, List<string> globalStyleNames)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                try
                {
                    if (reader.ReadToFollowing("html"))
                    {
                        var returnValue = Html.ParseXml(
                            reader,
                            fileName,
                            globalStyleNames);
                        return returnValue;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (ParserException ex)
                {
                    errors.Add(
                        new ErrorInfo(
                            fileName,
                            ex.LineNumber,
                            ex.ColumnNumber,
                            ex.Message));

                    return null;
                }
                catch (Exception ex)
                {
                    IXmlLineInfo lineInfo = (IXmlLineInfo)reader;
                    errors.Add(new ErrorInfo(fileName, lineInfo.LineNumber, lineInfo.LinePosition, ex.Message));

                    return null;
                }
            }
        }

        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="globalStyleNames">List of global style names.</param>
        /// <returns>Html templates</returns>
        public static Html ParseXml(XmlReader reader, string fileName, List<string> globalStyleNames)
        {
            Html returnValue = new Html(
                Path.GetFileNameWithoutExtension(fileName),
                fileName,
                globalStyleNames);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "head")
                    {
                        var subTree = reader.ReadSubtree();
                        subTree.Read();
                        returnValue.head = Head.Parse(returnValue, subTree);
                    }
                    else if (reader.Name == "body")
                    {
                        // IF template is created, it will automatically add itself to html.
                        TemplateParser.Parse(reader, returnValue);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }

            if (returnValue.Templates.Count == 0)
            {
                throw new ApplicationException("No Template Found");
            }

            return returnValue;
        }

        /// <summary>
        /// Writes the js.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void WriteJs(System.IO.StreamWriter writer)
        {
            foreach (var template in this.Templates)
            {
                writer.WriteLine("(function(){");
                template.WriteTemplateInitializationFunction(writer);
                writer.WriteLine("})();");
            }
        }

        /// <summary>
        /// Writes the CSS.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void WriteCss(System.IO.StreamWriter writer)
        {
            writer.WriteLine(
                "/* Css for TemplateId: {0} */",
                this.TemplateId);

            for (int styleIndex = 0; styleIndex < this.Head.Styles.Count; styleIndex++)
            {
                Style style = this.Head.Styles[styleIndex];
                style.WriteCss(writer);
            }
        }
    }
}
