using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Cs2JsC.Template.Compiler.Attributes;
using Cs2JsC.Template.Compiler.PropertyBuilders;
using Cs2JsC.PELoader.Reflection;

namespace Cs2JsC.Template.Compiler
{
    public class TemplateParser
    {
        #region nestedClasses
        private class ControlElementInfo
        {
            public string templateElementId = null;
            public string outHtmlTempId = null;
            public ElementInfo elementInfo = null;
        }
        #endregion

        #region member variables
        private const string templateTagName = "template";
        private const string templateIdAttributeName = "TemplateId";
        private const string dataTypeAttributeName = "DataType";
        private const string controlTypeAttributeName = "ControlType";
        static int tempTemplateId = 0;
        static int tmpElementId = 0;

        List<ElementInfo> elementInfos = new List<ElementInfo>();
        Dictionary<string, string> knownIdMappings = new Dictionary<string,string>();
        Dictionary<string, Tuple<TypeReference, bool>> templateParts = new Dictionary<string,Tuple<TypeReference,bool>>();
        KeyValuePair<int, int> templateStartPosition;
        TypeInfoResolver resolver;
        TypeReference dataContextType;
        TypeReference templatedControlType;
        Html html;
        string htmlStr;
        string templateId;
        #endregion

        #region constructors/Factories
        private TemplateParser(
            Html html,
            string templateId,
            TypeInfoResolver resolver,
            TypeReference dataContextType,
            TypeReference templatedControlType)
        {
            this.html = html;
            this.templateId = templateId;
            this.dataContextType = dataContextType;
            this.templatedControlType = templatedControlType;
            this.resolver = resolver;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public List<ElementInfo> TemplateElements
        { get { return this.elementInfos; } }

        public Dictionary<string, string> KnownIdMappings
        { get { return this.knownIdMappings; } }

        public Html Html
        { get { return this.html; } }
        #endregion

        #region public functions
        public static Template Parse(
            XmlReader reader,
            Html html)
        {
            return TemplateParser.Parse(
                reader,
                html,
                false,
                null);
        }

        public static Template Parse(
            XmlReader reader,
            Html html,
            bool isAnonymous,
            ElementInfo elementInfo)
        {
            string templateId = null;
            List<AttributeInfo> attributes = null;
            KeyValuePair<int, int> positionInfo = AttributeInfo.GetLineInfo(reader);
            TypeReference dataContextType, templatedControlType;

            TypeInfoResolver resolver = new TypeInfoResolver(
                reader,
                html.StyleMapping,
                html.FileName);

            // In case of anonymous, we are already in the node so subTree won't work.
            //
            if (!isAnonymous)
            {
                string tagName = reader.Name;

                attributes = TemplateParser.GetNodeAttributes(reader);

                if (tagName == "noscript" || tagName == TemplateParser.templateTagName)
                {
                    templateId = TemplateParser.GetTemplateId(attributes);

                    if (templateId == null)
                    {
                        Logger.Instance.LogWarning(
                            new ErrorInfo(
                                html.FileName,
                                positionInfo.Key,
                                positionInfo.Value,
                                "Ignoring template decleration because it doesn't have templateId"));

                        return null;
                    }
                }
                else if (tagName != "body")
                {
                    return null;
                }

                dataContextType = TemplateParser.GetTemplateType(
                    attributes,
                    resolver,
                    false);

                templatedControlType = TemplateParser.GetTemplateType(
                    attributes,
                    resolver,
                    true);
            }
            else
            {
                attributes = TemplateParser.GetNodeAttributes(reader);

                templateId = TypeManager.GetIdString(
                    "_tid",
                    System.Threading.Interlocked.Increment(
                        ref TemplateParser.tempTemplateId));

                dataContextType = elementInfo.DataTypeInfo;
                templatedControlType = elementInfo.ControlTypeInfo;
            }

            if (dataContextType == null)
            {
                Logger.Instance.LogWarning(
                    new ErrorInfo(
                        resolver.FileName,
                        positionInfo.Key,
                        positionInfo.Value,
                        "DataType is not defined for template"));
            }

            if (templatedControlType == null)
            {
                Logger.Instance.LogWarning(
                    new ErrorInfo(
                        resolver.FileName,
                        positionInfo.Key,
                        positionInfo.Value,
                        "ControlType is not defined for template"));
            }

            if (templateId == null)
            {
                templateId = html.TemplateId;
            }
            else
            {
                templateId = string.Format("{0}.{1}", html.TemplateId, templateId);
            }

            TemplateParser parser = new TemplateParser(
                html,
                templateId,
                resolver,
                dataContextType,
                templatedControlType);

            parser.templateStartPosition = positionInfo;

            StringBuilder strBuilder = new StringBuilder();

            if (parser.ParseChildNodes(
                    strBuilder,
                    reader,
                    elementInfo,
                    false))
            {
                parser.CheckTemplateParts();

                parser.htmlStr = strBuilder.ToString();

                Template returnValue = new Template(
                    parser.htmlStr,
                    templateId,
                    html,
                    dataContextType,
                    templatedControlType,
                    parser.elementInfos,
                    parser.knownIdMappings);

                html.Templates.Add(returnValue);

                return returnValue;
            }
            else
            {
                return null;
            }
        }

        public static Tuple<ElementInfo, string> ParseFactory(
            XmlReader reader,
            TypeInfoResolver resolver,
            TypeReference dataContextType,
            Html html)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                TemplateParser parser = new TemplateParser(
                    html,
                    null,
                    resolver,
                    dataContextType,
                    null);

                StringBuilder strBuilder = new StringBuilder();
                ElementInfo element = parser.ParseNode(
                    strBuilder,
                    reader,
                    null);

                return Tuple.Create(element, strBuilder.ToString());
            }

            return null;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static List<AttributeInfo> GetNodeAttributes(
            XmlReader reader)
        {
            List<AttributeInfo> attributeInfos = new List<AttributeInfo>();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    attributeInfos.Add(new AttributeInfo(reader));
                } while (reader.MoveToNextAttribute());
            }

            return attributeInfos;
        }

        private static TypeReference GetTemplateType(
            List<AttributeInfo> attributes,
            TypeInfoResolver resolver,
            bool controlType)
        {
            for (int iAttribute = 0; iAttribute < attributes.Count; iAttribute++)
            {
                var attribute = attributes[iAttribute];

                if ((!controlType && (attribute.LocalName == "data-type" || attribute.LocalName == TemplateParser.dataTypeAttributeName)) ||
                    (controlType && (attribute.LocalName == "data-controlType" || attribute.LocalName == TemplateParser.controlTypeAttributeName)))
                {
                    if (attribute.LocalName == "data-type")
                    {
                        Logger.Instance.LogWarning(
                            new ErrorInfo(
                                resolver.FileName,
                                attribute.LineNumber,
                                attribute.ColumnNumber,
                                "use of data-type is deprecated, use DataType"));
                    }
                    else if (attribute.LocalName == "data-controlType")
                    {
                        Logger.Instance.LogWarning(
                            new ErrorInfo(
                                resolver.FileName,
                                attribute.LineNumber,
                                attribute.ColumnNumber,
                                "use of data-controlType is deprecated, use ControlType"));
                    }

                    TypeReference returnValue = resolver.ResolveType(
                        attribute.Value,
                        controlType
                            ? DefaultSettings.Default.ControlsNS
                            : null);

                    if (returnValue == null)
                    {
                        if (controlType)
                        {
                            throw new ParserException(
                                string.Format("Can't resolve attached Control type {0}", attribute.Value),
                                attribute.LineNumber,
                                attribute.ColumnNumber);
                        }
                        else
                        {
                            throw new ParserException(
                                string.Format("Can't resolve DataContext type {0}", attribute.Value),
                                attribute.LineNumber,
                                attribute.ColumnNumber);
                        }
                    }

                    return returnValue;
                }
            }

            return null;
        }

        private static string GetTemplateId(
            List<AttributeInfo> attributes)
        {
            string templateId = null;

            for (int iAttribute = 0; iAttribute < attributes.Count; iAttribute++)
            {
                var attribute = attributes[iAttribute];

                switch (attribute.LocalName)
                {
                    case "data-templateId":
                    case TemplateParser.templateIdAttributeName:
                        templateId = attribute.Value;
                        break;
                    case "data-type":
                    case "data-controlType":
                    case TemplateParser.dataTypeAttributeName:
                    case TemplateParser.controlTypeAttributeName:
                        break;
                    default:
                        Console.WriteLine("Ignoring attribute {0} at {1}-{2}",
                                attribute.Name,
                                attribute.LineNumber,
                                attribute.ColumnNumber);
                        break;
                }
            }

            return templateId;
        }

        private bool ParseChildNodes(
            StringBuilder htmlBuilder,
            XmlReader reader,
            ElementInfo currentNodeElementInfo,
            bool currentNodeIsPanel)
        {
            bool returnValue = false;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "noscript" || reader.Name == TemplateParser.templateTagName)
                    {
                        TemplateParser.Parse(
                                reader,
                                this.html);
                    }
                    else
                    {
                        PropertyReference propertyInfo = null;

                        if (currentNodeElementInfo != null)
                        {
                            propertyInfo = resolver.IsPropertyNode(
                                currentNodeElementInfo.ControlTypeInfo,
                                reader.Name);
                        }

                        if (propertyInfo != null)
                        {
                            this.ParsePropertyNode(
                                reader,
                                currentNodeElementInfo,
                                propertyInfo);
                        }
                        else
                        {
                            returnValue = true;
                            this.ParseNode(
                                htmlBuilder,
                                reader,
                                currentNodeIsPanel ? currentNodeElementInfo : null);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (reader.NodeType == XmlNodeType.Whitespace ||
                    reader.NodeType == XmlNodeType.SignificantWhitespace)
                {
                    // More than one space has no meaning in html.
                    //
                    htmlBuilder.Append(' ');
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    TemplateParser.EncodeString(htmlBuilder, reader.ReadContentAsString());
                    returnValue = true;

                    // Reading content will get us to he endNode so we should break here.
                    //
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Parse a node
        /// </summary>
        /// <param name="htmlBuilder">HTML builder</param>
        /// <param name="reader">XML reader</param>
        /// <param name="parentPanelElementInfo">(Optional) If parent element is a panel, its ElementInfo object</param>
        /// <returns></returns>
        private ElementInfo ParseNode(
            StringBuilder htmlBuilder,
            XmlReader reader,
            ElementInfo parentPanelElementInfo)
        {
            var nodeLineInfo = AttributeInfo.GetLineInfo(reader);

            string nodeName = reader.Name;
            bool isEmpty = reader.IsEmptyElement;

            // First let's process all the attributes. These attributes will help
            // us determine if we need to create UIObject for this node or not.
            //
            List<AttributeInfo> attributes = TemplateParser.GetNodeAttributes(reader);

            // Check if the control type is declared as the tag name.
            //
            TypeReference nodeControlType = resolver.ResolveType(
                nodeName,
                DefaultSettings.Default.ControlsNS);

            ControlElementInfo controlElementInfo;

            if (nodeControlType != null)
            {
                nodeName = TemplateParser.GetTagName(nodeControlType);
                htmlBuilder.AppendFormat(
                    "<{0}", nodeName);

                controlElementInfo = this.ParseControlNodeAttributes(
                        nodeControlType,
                        attributes,
                        htmlBuilder);
            }
            else
            {
                htmlBuilder.AppendFormat(
                    "<{0}", nodeName);

                controlElementInfo = this.ParseClassicNodeAttributes(
                        attributes,
                        htmlBuilder);
            }

            ElementInfo elementInfo = (controlElementInfo != null) ? controlElementInfo.elementInfo : null;

            // If the parent is a panel then we need to keep track of it
            //
            if (parentPanelElementInfo != null)
            {
                // The element needs to be represented by a UIObject so that the panel can manipulate it
                //
                if (elementInfo == null)
                {
                    elementInfo = new ElementInfo(parentPanelElementInfo.DataTypeInfo);
                }

                elementInfo.ParentPanel = parentPanelElementInfo;
            }

            if (elementInfo != null)
            {
                string templateElementId = (controlElementInfo != null) ? controlElementInfo.templateElementId : null;
                string outHtmlTempId = (controlElementInfo != null) ? controlElementInfo.outHtmlTempId : null;

                // If we don't already have a temporary ID, generate one
                //
                if (outHtmlTempId == null)
                {
                    outHtmlTempId = TemplateParser.GetTempId();

                    if (controlElementInfo != null)
                    {
                        controlElementInfo.outHtmlTempId = outHtmlTempId;
                    }
                }

                htmlBuilder.AppendFormat(
                    " id='{0}'",
                    outHtmlTempId);

                elementInfo.Finalize(templateElementId, outHtmlTempId);
                this.elementInfos.Add(elementInfo);
            }

            if (isEmpty)
            {
                htmlBuilder.Append("/>");
            }
            else
            {
                htmlBuilder.Append(">");

                // If a control has children, they are treated as the control's template
                //
                if (controlElementInfo != null &&
                    TypeManager.ResolveType(KnowTypeReferences.ControlSignature).IsBaseClass(
                        controlElementInfo.elementInfo.ControlTypeInfo))
                {
                    var template = TemplateParser.Parse(
                        reader,
                        this.html,
                        true,
                        controlElementInfo.elementInfo);

                    if (template != null)
                    {
                        if (!controlElementInfo.elementInfo.AddTemplateId(template.TemplateId))
                        {
                            throw new ParserException(
                                "Can't use implicit template. TemplateId already defined",
                                nodeLineInfo.Key,
                                nodeLineInfo.Value);
                        }
                    }
                }
                else
                {
                    bool isPanel = (controlElementInfo != null &&
                        TypeManager.ResolveType(KnowTypeReferences.PanelSignature).IsBaseClass(
                            controlElementInfo.elementInfo.ControlTypeInfo));

                    this.ParseChildNodes(
                        htmlBuilder,
                        reader,
                        controlElementInfo != null
                            ? controlElementInfo.elementInfo
                            : null,
                        isPanel);
                }
                htmlBuilder.AppendFormat("</{0}>", nodeName);
            }

            if (controlElementInfo != null)
            {
                return controlElementInfo.elementInfo;
            }

            return null;
        }

        private void ParsePropertyNode(
            XmlReader reader,
            ElementInfo controlElementInfo,
            PropertyReference property)
        {
            if (property.PropertyDefinition.PropertyType.Equals(
                    TypeManager.ResolveType(KnowTypeReferences.FactoryDelegate)))
            {
                this.ParseFactoryDelegate(
                    reader,
                    controlElementInfo,
                    property);
            }
            else
            {
                var lineInfos = AttributeInfo.GetLineInfo(reader);

                throw new ParserException(
                    string.Format("Parsing type {0} is not supported", property.PropertyDefinition.PropertyType),
                    lineInfos.Key,
                    lineInfos.Value);
            }
        }

        private void ParseFactoryDelegate(
            XmlReader reader,
            ElementInfo controlElementInfo,
            PropertyReference property)
        {
            List<AttributeInfo> attributes = TemplateParser.GetNodeAttributes(reader);

            // TODO: add code to check dataType.
            //

            bool elementFound = false;

            if (reader.IsEmptyElement)
            {
                var lineInfo = AttributeInfo.GetLineInfo(reader);
                throw new ParserException(
                    "Control Factory should have exactly one child node.",
                    lineInfo.Key,
                    lineInfo.Value);
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (elementFound)
                    {
                        var lineInfo = AttributeInfo.GetLineInfo(reader);
                        throw new ParserException(
                            "Control Factory can have only one child node.",
                            lineInfo.Key,
                            lineInfo.Value);
                    }

                    controlElementInfo.AddProperty(
                        property,
                        new ControlFactoryPropertyValue(
                            reader,
                            this.html,
                            this.resolver,
                            (TypeReference)property.PropertyDefinition.PropertyType.ApplyGenericArguments(
                                    property.Parent.Parameters,
                                    null).Parameters[0]));

                    elementFound = true;
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    var lineInfo = AttributeInfo.GetLineInfo(reader);

                    throw new ParserException(
                        "Control Factory should have exactly one non text child node.",
                        lineInfo.Key,
                        lineInfo.Value);
                }
            }

            if (!elementFound)
            {
                var lineInfo = AttributeInfo.GetLineInfo(reader);
                throw new ParserException(
                    "Control Factory should have exactly one child node.",
                    lineInfo.Key,
                    lineInfo.Value);
            }
        }

        private bool IsPropertyNode(
            TypeInfoResolver resolver,
            TypeReference parentNodeControl,
            string nodeName,
            out PropertyReference property)
        {
            property = null;

            if (parentNodeControl == null)
            {
                return false;
            }

            string[] propertyParts = nodeName.Split('.');

            // Let's check the type of property we may be encountering.
            // If the property is dependency property, it should be specified as
            // ns:CtrlName.DependencyProperty or if this property is regular
            // property it should be named as DependencyProperty (first letter is upper case).
            //
            // TODO: Add support for dependency property.
            if (char.IsUpper(nodeName[0]))
            {
                property = TypeManager.ResolveProperty(
                        parentNodeControl,
                        nodeName);

                return true;
            }

            return false;
        }

        private ControlElementInfo ParseControlNodeAttributes(
            TypeReference nodeControlType,
            List<AttributeInfo> attributes,
            StringBuilder htmlBuilder)
        {
            ControlElementInfo returnValue = new ControlElementInfo();
            returnValue.elementInfo =
                new ElementInfo(
                    this.dataContextType,
                    nodeControlType);

            for (int iAttribute = 0; iAttribute < attributes.Count; iAttribute++)
            {
                var attribute = attributes[iAttribute];

                if (!returnValue.elementInfo.ProcessAttributes(
                    this.resolver,
                    attribute,
                    this.dataContextType,
                    this.templatedControlType))
                {
                    if (string.IsNullOrEmpty(attribute.AttributeNS) && attribute.Name == "id")
                    {
                        returnValue.templateElementId = attribute.Value;
                        returnValue.outHtmlTempId = TemplateParser.GetTempId();

                        // TODO: add error about clashing Ids.
                        this.knownIdMappings.Add(
                            returnValue.templateElementId,
                            returnValue.outHtmlTempId);

                        htmlBuilder.AppendFormat(
                            " id='{0}'",
                            returnValue.outHtmlTempId);

                    }
                    else if (!attribute.Name.StartsWith("xmlns"))
                    {
                        throw new ParserException(
                            string.Format(
                                "Don't know how to parse attribute name:{0}, value:{1}",
                                attribute.Name,
                                attribute.Value),
                            attribute.LineNumber,
                            attribute.ColumnNumber);
                    }
                }
            }

            return returnValue;
        }

        private ControlElementInfo ParseClassicNodeAttributes(
            List<AttributeInfo> attributes,
            StringBuilder htmlBuilder)
        {
            ControlElementInfo returnValue = new ControlElementInfo();
            returnValue.elementInfo = new ElementInfo(this.dataContextType);
            bool createUIObject = false;

            for (int iAttribute = 0; iAttribute < attributes.Count; iAttribute++)
            {
                var attribute = attributes[iAttribute];

                if (returnValue.elementInfo.ProcessAttributes(
                    this.resolver,
                    attribute,
                    this.dataContextType,
                    this.templatedControlType))
                {
                    createUIObject = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(attribute.AttributeNS))
                    {
                        if (attribute.Name == "id")
                        {
                            createUIObject = true;

                            returnValue.templateElementId = attribute.Value;
                            returnValue.outHtmlTempId = TemplateParser.GetTempId();

                            // TODO: add error about clashing Ids.
                            this.knownIdMappings.Add(
                                returnValue.templateElementId,
                                returnValue.outHtmlTempId);

                            htmlBuilder.AppendFormat(
                                " id='{0}'",
                                returnValue.outHtmlTempId);

                        }
                        else if (attribute.Name == "class")
                        {
                            htmlBuilder.AppendFormat(" class='{0}'", this.resolver.GetStyleNames(attribute.Value));
                        }
                        else
                        {
                            htmlBuilder.Append(" ");
                            htmlBuilder.Append(attribute.Name);
                            TemplateParser.EncodeString(htmlBuilder, "=\"");
                            TemplateParser.EncodeString(htmlBuilder, attribute.Value);
                            TemplateParser.EncodeString(htmlBuilder, "\"");
                        }
                    }
                    else if (!attribute.Name.StartsWith("xmlns"))
                    {
                        Console.WriteLine("Ignoring attribute {0} at {1}-{2}",
                            attribute.Name,
                            attribute.LineNumber,
                            attribute.ColumnNumber);
                    }
                }
            }

            if (createUIObject)
            {
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        private void CheckTemplateParts()
        {
            if (this.templatedControlType == null)
            {
                return;
            }

            this.GetRequiredTemplateParts();

            Dictionary<string, ElementInfo> elementInfos = new Dictionary<string, ElementInfo>();

            for (int elementInfoIndex = 0; elementInfoIndex < this.elementInfos.Count; elementInfoIndex++)
            {
                ElementInfo elementInfo = this.elementInfos[elementInfoIndex];

                if (elementInfo.KnownId != null)
                {
                    if (this.templateParts.ContainsKey(elementInfo.KnownId))
                    {
                        elementInfos.Add(
                            elementInfo.KnownId,
                            elementInfo);
                    }
                    else
                    {
                        Logger.Instance.LogWarning(
                            new ErrorInfo(
                                resolver.FileName,
                                this.templateStartPosition.Key,
                                this.templateStartPosition.Value,
                                string.Format("Ignoring Id:{0}, Is this a typo?", elementInfo.KnownId)));
                    }
                }
            }

            foreach (var kvPair in this.templateParts)
            {
                if (!elementInfos.ContainsKey(kvPair.Key))
                {
                    // Let's check for required TemplateItemPart.
                    if (kvPair.Value.Item2)
                    {
                        throw new ParserException(
                            string.Format(
                                "Required TemplatePart: {0} of type {1} not defined in the template",
                                kvPair.Key,
                                kvPair.Value.Item1),
                            this.templateStartPosition.Key,
                            this.templateStartPosition.Value);
                    }
                }
                else
                {
                    if (!kvPair.Value.Item1.Type.Equals(elementInfos[kvPair.Key].ControlTypeInfo)
                        && !TypeManager.ResolveType(kvPair.Value.Item1.Type.Name)[0].IsBaseClass(
                            elementInfos[kvPair.Key].ControlTypeInfo))
                    {
                        throw new ParserException(
                            string.Format(
                                "TemplatePart: {0} is of type {1}, but it should be {2}",
                                kvPair.Key,
                                elementInfos[kvPair.Key].ControlTypeInfo,
                                kvPair.Value.Item1),
                            this.templateStartPosition.Key,
                            this.templateStartPosition.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the required template parts.
        /// </summary>
        /// <returns>Dictionary of id to control type mapping</returns>
        private void GetRequiredTemplateParts()
        {
            if (this.templatedControlType == null)
            {
                return;
            }

            TypeReference controlType = this.templatedControlType;
            while (controlType != null)
            {
                foreach (FieldDefinition fieldInfo in ((TypeDefinition)controlType.Type).Fields)
                {
                    if (fieldInfo.IsConst
                        && fieldInfo.Type.Equals(
                            TypeManager.ResolveType(KnowTypeReferences.StringTypeSignature)))
                    {
                        TemplatePartAttribute attribute = AttributeBase.GetAttribute<TemplatePartAttribute>(fieldInfo.Attributes);

                        if (attribute != null)
                        {
                            if (!this.templateParts.ContainsKey((string)fieldInfo.ConstValue))
                            {
                                this.templateParts.Add(
                                    (string)fieldInfo.ConstValue,
                                    Tuple.Create(
                                        attribute.Type,
                                        attribute.Required));
                            }
                            else
                            {
                                throw new ApplicationException(
                                    string.Format(
                                        "TemplatePartId: {0} defined more than once in hierarchy of control: {1}",
                                        fieldInfo.ConstValue,
                                        this.templatedControlType));
                            }
                        }
                    }
                }

                controlType = controlType.Extends;
            }
        }

        /// <summary>
        /// Looks up the DefaultTagName attribute on the type and returns defined attribute.
        /// </summary>
        /// <param name="controlTypeInfo">The control type info.</param>
        /// <returns></returns>
        private static string GetTagName(TypeReference controlTypeInfo)
        {
            TypeDefinition typeDefinition = (TypeDefinition)controlTypeInfo.Type;
            if (typeDefinition.Attributes != null)
            {
                DefaultTagNameAttribute tagNameAttribute = AttributeBase.GetAttribute<DefaultTagNameAttribute>(typeDefinition.Attributes);
                if (tagNameAttribute != null)
                {
                    return tagNameAttribute.DefaultTagName;
                }
            }

            return "div";
        }

        /// <summary>
        /// Encodes the string for HTML consumption.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="str">The STR.</param>
        private static void EncodeString(StringBuilder sb, string str)
        {
            for (int iStr = 0; iStr < str.Length; iStr++)
            {
                char ch = str[iStr];
                if (ch == '\r')
                {
                    sb.Append("\\r");
                }
                else if (ch == '\n')
                {
                    sb.Append("\\n");
                }
                else if (ch == '\\')
                {
                    sb.Append("\\\\");
                }
                else if (ch == '"')
                {
                    sb.Append("\\\"");
                }
                else
                {
                    sb.Append(ch);
                }
            }
        }

        private static string GetTempId()
        {
            return string.Format("_t{0}", System.Threading.Interlocked.Increment(ref TemplateParser.tmpElementId));
        }
        #endregion
    }
}
