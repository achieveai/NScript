namespace NScript.Template.Compiler.PropertyBuilders
{
    using System;
    using System.IO;
    using System.Xml;
    using NScript.PELoader.Reflection;

    public class ControlFactoryPropertyValue : PropertyValue
    {
        #region member variables
        TypeInfoResolver resolver;
        ElementInfo factoryElement;
        string htmlString;
        #endregion

        #region constructors/Factories
        public ControlFactoryPropertyValue(
            XmlReader reader,
            Html html,
            TypeInfoResolver resolver,
            TypeReference controlType)
        {
            var lineInfo = AttributeInfo.GetLineInfo(reader);

            var factoryValues = TemplateParser.ParseFactory(
                reader,
                resolver,
                null,
                html);

            this.factoryElement = factoryValues.Item1;
            this.htmlString = factoryValues.Item2;

            if (String.IsNullOrEmpty(htmlString) ||
                htmlString.Trim().Length == 0 ||
                this.factoryElement == null)
            {
                throw new ParserException(
                    "Invalid ControlFactory.",
                    lineInfo.Key,
                    lineInfo.Value);
            }

            if (!controlType.IsBaseClass(this.factoryElement.ControlTypeInfo))
            {
                throw new ParserException(
                    string.Format(
                        "ControlFactory is not of correct type, expected: {0}, actual: {1}",
                        controlType,
                        this.factoryElement.ControlTypeInfo),
                    lineInfo.Key,
                    lineInfo.Value);
            }

            this.resolver = resolver;
            this.factoryElement.IsFactoryRoot = true;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public override void WriteValueToJs(TextWriter writer)
        {
            writer.WriteLine("function(){{ var tr = $(\"<div>{0}</div>\");", this.htmlString);
            writer.WriteLine("var rv=null;");

            this.factoryElement.WriteToJs(
                "tr",
                "rv",
                this.resolver.StyleMapping,
                (StreamWriter)writer);
            writer.Write("return rv;}");
        }

        public override void AddTypeReferences()
        {
            this.factoryElement.AddTypeReferences();
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
