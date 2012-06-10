using System;
using System.Collections.Generic;
using Cs2JsC.Template.Compiler.PropertyBuilders;
using Cs2JsC.Template.Compiler.Attributes;
using Cs2JsC.Template.Compiler.DataBinders;
using Cs2JsC.PELoader.Reflection;

namespace Cs2JsC.Template.Compiler
{
    public class ElementInfo
    {
        #region memberVariables
        private const string TemplateParserStr = "TemplateParser";
        public const string CssBinderPrefixStr = "CssBinder.";
        private const string LegacyBinderPrefixStr = "data-cssBinder-";
        public const string ControlAttach = "data-control";

        string knownId;
        string htmlElementId;
        bool strictDefinition = false;
        bool isFactory = false;
        TypeReference controlTypeInfo;
        TypeReference parentDataContextType;
        TypeReference currentDataContextType;
        TypeReference typeReference;
        List<DataBindingInfo> dataBindings = new List<DataBindingInfo>();
        List<TemplateBindingInfo> templateBindings = new List<TemplateBindingInfo>();
        List<CssBindingInfo> cssBindings = new List<CssBindingInfo>();
        List<Tuple<BoundProperty, PropertyValue>> propertiesToSet = new List<Tuple<BoundProperty, PropertyValue>>();
        HashSet<string> propertiesSet = new HashSet<string>();
        ElementInfo parentPanel = null;
        #endregion

        #region constructor/Factories
        public ElementInfo(TypeReference parentDataContextType)
        {
            this.parentDataContextType = parentDataContextType;
            this.currentDataContextType = parentDataContextType;
        }

        public ElementInfo(
            TypeReference dataContextType,
            TypeReference controlTypeInfo)
        {
            this.parentDataContextType = dataContextType;
            this.currentDataContextType = dataContextType;
            this.controlTypeInfo = controlTypeInfo;
            this.strictDefinition = true;
        }
        #endregion

        #region Properties
        public string KnownId
        { get { return this.knownId; } }

        public TypeReference ControlTypeInfo
        { get { return this.controlTypeInfo; } }

        public bool IsFactoryRoot
        {
            get
            {
                return this.isFactory;
            }

            set
            {
                this.isFactory = value;
            }
        }

        public TypeReference DataTypeInfo
        {
            get
            {
                return this.currentDataContextType;
            }
        }

        public ElementInfo ParentPanel
        {
            get { return this.parentPanel; }
            set { this.parentPanel = value; }
        }
        #endregion

        #region private functions
        private bool LoadCssBinders(
            TypeInfoResolver resolver,
            AttributeInfo attribute,
            TypeReference dataContextType,
            TypeReference templatedControlType)
        {
            if (attribute.LocalName.StartsWith(ElementInfo.CssBinderPrefixStr) ||
                attribute.LocalName.StartsWith(ElementInfo.LegacyBinderPrefixStr))
            {
                string cssClass;

                if (attribute.LocalName.StartsWith(ElementInfo.LegacyBinderPrefixStr))
                {
                    cssClass = attribute.LocalName.Substring(ElementInfo.LegacyBinderPrefixStr.Length);

                    Logger.Instance.LogWarning(
                        new ErrorInfo(
                            resolver.FileName,
                            attribute.LineNumber,
                            attribute.ColumnNumber,
                            "data-cssBinder- is deprecated. Use CssBinder. instead"));
                }
                else
                {
                    cssClass = attribute.LocalName.Substring(ElementInfo.CssBinderPrefixStr.Length);
                }

                this.InitializeTypeInfo();

                var bindingValues = BindingInfo.ParseBindingInfo(attribute.Value);
                bool isSelfBinding = false;

                if (bindingValues == null)
                {
                    bindingValues = BindingInfo.ParseBindingInfo(
                        string.Format("[{0}]", attribute.Value));

                    if (bindingValues == null)
                    {
                        throw new ParserException(
                            string.Format(
                                "Can't parse binding info {0}, lineNumber:{1},{2}",
                                attribute.Value),
                            attribute.LineNumber,
                            attribute.ColumnNumber);
                    }

                    isSelfBinding = true;
                }

                try
                {
                    if (bindingValues.Item1 == BindingInfo.StaticBindingStr)
                    {
                        throw new ApplicationException("CssBinder does not support static binding.");
                    }

                    this.cssBindings.Add(
                        new CssBindingInfo(
                            cssClass,
                            bindingValues,
                            resolver,
                            dataContextType,
                            isSelfBinding ? this.controlTypeInfo : templatedControlType,
                            isSelfBinding));
                }
                catch (ApplicationException ex)
                {
                    throw new ParserException(
                        ex.Message,
                        attribute.LineNumber,
                        attribute.ColumnNumber,
                        ex);
                }
                return true;
            }

            return false;
        }

        private bool LoadDataBinders(
            TypeInfoResolver resolver,
            AttributeInfo attribute,
            TypeReference dataContextType,
            TypeReference templatedControlType)
        {
            if ((this.strictDefinition && attribute.LocalName != "id") ||
                attribute.LocalName.StartsWith("data-"))
            {
                if (attribute.LocalName.StartsWith("data-"))
                {
                    Logger.Instance.LogWarning(
                        new ErrorInfo(
                            resolver.FileName,
                            attribute.LineNumber,
                            attribute.ColumnNumber,
                            "use of 'data-' is deprecated"));
                }

                this.InitializeTypeInfo();

                BoundProperty boundProperty = BoundProperty.Create(
                    resolver,
                    this.controlTypeInfo,
                    attribute,
                    this.strictDefinition);

                if (boundProperty != null &&
                    this.propertiesSet.Contains(boundProperty.PropertyName))
                {
                    throw new ParserException(
                        string.Format(
                            "Property already bound {0}, lineNumber:{1},{2}",
                            boundProperty.PropertyName),
                        attribute.LineNumber,
                        attribute.ColumnNumber);
                }

                if (boundProperty == null)
                {
                    throw new ParserException(
                            string.Format(
                                "Can't attach to propertyName: {0}",
                                attribute.Name),
                            attribute.LineNumber,
                            attribute.ColumnNumber);
                }

                this.propertiesSet.Add(boundProperty.PropertyName);

                var bindingValues = BindingInfo.ParseBindingInfo(attribute.Value);

                try
                {
                    if (bindingValues == null)
                    {
                        // We know more about css class property. If we are trying to set CssClass property
                        // then let's get obfuscated class name.
                        if (this.IsCssProperty(boundProperty))
                        {
                            this.propertiesToSet.Add(
                                Tuple.Create<BoundProperty, PropertyValue>(
                                    boundProperty,
                                    new StringPropertyValue(resolver.GetStyleNames(attribute.Value))));
                        }
                        else if (boundProperty.PropertyType.TypeName.Equals(KnowTypeReferences.StringTypeSignature))
                        {
                            this.propertiesToSet.Add(
                                Tuple.Create<BoundProperty, PropertyValue>(
                                    boundProperty,
                                    new StringPropertyValue(attribute.Value)));
                        }
                        else if (boundProperty.PropertyType.TypeName.Equals(KnowTypeReferences.IntTypeSignature))
                        {
                            int value;

                            if (!int.TryParse(attribute.Value, out value))
                            {
                                throw new ParserException(
                                    string.Format("Can't convert {0} to int", attribute.Value),
                                    attribute.LineNumber,
                                    attribute.ColumnNumber);
                            }

                            this.propertiesToSet.Add(
                                Tuple.Create<BoundProperty, PropertyValue>(
                                    boundProperty,
                                    new IntPropertyValue(value)));
                        }
                        else if (boundProperty.PropertyType.TypeName.Equals(KnowTypeReferences.BoolTypeSignature))
                        {
                            bool value;

                            if (!bool.TryParse(attribute.Value, out value))
                            {
                                throw new ParserException(
                                    string.Format("Can't convert {0} to bool", attribute.Value),
                                    attribute.LineNumber,
                                    attribute.ColumnNumber);
                            }

                            this.propertiesToSet.Add(
                                Tuple.Create<BoundProperty, PropertyValue>(
                                    boundProperty,
                                    new BoolPropertyValue(value)));
                        }
                        else if (boundProperty.PropertyType.Type.Extends.Equals(KnowTypeReferences.EnumTypeSignature))
                        {
                            this.propertiesToSet.Add(
                                Tuple.Create<BoundProperty, PropertyValue>(
                                    boundProperty,
                                    new EnumPropertyValue(
                                        boundProperty.PropertyType,
                                        attribute.Value)));
                        }
                        else
                        {
                            throw new ParserException(
                                string.Format("Parsing of type {0} is not supported", boundProperty.PropertyType),
                                attribute.LineNumber,
                                attribute.ColumnNumber);
                        }
                    }
                    else if (bindingValues.Item1 == BindingInfo.TemplatBindingStr)
                    {
                        // We need to pass templatedControlType to TemplateBinding.
                        //
                        this.templateBindings.Add(
                            new TemplateBindingInfo(
                                boundProperty,
                                bindingValues,
                                resolver,
                                templatedControlType));
                    }
                    else if (bindingValues.Item1 == BindingInfo.StaticBindingStr)
                    {
                        this.dataBindings.Add(
                            StaticBindingInfo.Create(
                                boundProperty,
                                bindingValues,
                                resolver));
                    }
                    else
                    {
                        // For data binidng, dataContextType is what we need for validation.
                        //
                        this.dataBindings.Add(
                            new DataBindingInfo(
                                boundProperty,
                                bindingValues,
                                resolver,
                                dataContextType));
                    }
                }
                catch (ApplicationException ex)
                {
                    throw new ParserException(
                        ex.Message,
                        attribute.LineNumber,
                        attribute.ColumnNumber,
                        ex);
                }

                return true;
            }

            return false;
        }

        private bool AssignTypeInfo(
            TypeInfoResolver resolver,
            AttributeInfo attribute)
        {
            if (attribute.LocalName == ElementInfo.ControlAttach)
            {
                Logger.Instance.LogWarning(
                    new ErrorInfo(
                        resolver.FileName,
                        attribute.LineNumber,
                        attribute.ColumnNumber,
                        "use of 'data-control' for attaching control is deprecated. Use xml node tags to do so"));

                if (this.controlTypeInfo != null)
                {
                    throw new ParserException(
                        "Element already attached to a control, bindings are declared before attach, lineInfo:{0},{1}",
                        attribute.LineNumber,
                        attribute.ColumnNumber);
                }

                var returnValue = resolver.ResolveType(
                    attribute.Value,
                    DefaultSettings.Default.ControlsNS);

                if (returnValue == null)
                {
                    throw new ParserException(
                        string.Format(
                            "Can't resolve type {0}",
                            attribute.Value),
                        attribute.LineNumber,
                        attribute.ColumnNumber);
                }

                this.controlTypeInfo = returnValue;

                return true;
            }

            return false;
        }

        private void InitializeTypeInfo()
        {
            if (this.controlTypeInfo == null)
            {
                this.controlTypeInfo = TypeManager.ResolveType(KnowTypeReferences.UIObjectSignature);
            }
        }

        /// <summary>
        /// Determines whether given propertyInfo is CSS property.
        /// </summary>
        /// <param name="boundProperty">The bound property.</param>
        /// <returns>
        /// <c>true</c> if given property is CssClass; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCssProperty(BoundProperty boundProperty)
        {
            if (boundProperty.PropertyType.TypeName.Equals(KnowTypeReferences.StringTypeSignature))
            {
                return AttributeBase.GetAttribute<CssClassAttribute>(
                    boundProperty.PropertyReference != null
                        ? boundProperty.PropertyReference.Definition.Attributes
                        : boundProperty.FieldReference.Definition.Attributes) != null;
            }

            return false;
        }
        #endregion

        #region public function
        /// <summary>
        /// Processes the attributes.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="dataContextType">Type of the data context.</param>
        /// <param name="templatedControlType">Type of the templated control.</param>
        /// <returns>true if attribute was processed.</returns>
        public bool ProcessAttributes(
            TypeInfoResolver resolver,
            AttributeInfo attribute,
            TypeReference dataContextType,
            TypeReference templatedControlType)
        {
            return 
                this.AssignTypeInfo(resolver, attribute) ||
                    this.LoadCssBinders(resolver, attribute, dataContextType, templatedControlType) ||
                    this.LoadDataBinders(resolver, attribute, dataContextType, templatedControlType);
        }

        public void Finalize(string knownId, string htmlElementId)
        {
            this.knownId = knownId;
            this.htmlElementId = htmlElementId;
            this.InitializeTypeInfo();
        }

        public void AddTypeReferences()
        {
            this.typeReference = new TypeReference(this.controlTypeInfo);
            TypeManager.AddTypeReference(this.typeReference);

            foreach (var binder in this.dataBindings)
            {
                binder.AddToReference();
            }
            foreach (var binder in this.templateBindings)
            {
                binder.AddToReference();
            }
            foreach (var binder in this.cssBindings)
            {
                binder.AddToReference();
            }
            foreach (var propertyValues in this.propertiesToSet)
            {
                propertyValues.Item2.AddTypeReferences();
            }
        }

        public bool AddTemplateId(string templateId)
        {
            if (this.controlTypeInfo == null)
            {
                throw new InvalidOperationException();
            }

            if (!this.propertiesSet.Contains("TemplateId"))
            {
                this.propertiesSet.Add("TemplateId");
                this.propertiesToSet.Add(
                    new Tuple<BoundProperty, PropertyValue>(
                        BoundProperty.Create(
                            this.controlTypeInfo,
                            "TemplateId"),
                        new StringPropertyValue(templateId)));

                return true;
            }

            return false;
        }

        public void AddProperty(
            PropertyReference property,
            PropertyValue propertyValue)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException("propertyValue");
            }

            this.propertiesSet.Add(property.Name);
            this.propertiesToSet.Add(
                Tuple.Create(
                    BoundProperty.Create(
                        this.controlTypeInfo,
                        property.Name),
                    propertyValue));
        }

        public void WriteToJs(
            string templateRootId,
            string elementId,
            StyleMapping styleNameMapping,
            System.IO.StreamWriter writer)
        {
            writer.WriteLine(
                "{0}=new {1}({2}.find(\"#{3}\"){4});",
                elementId,
                TypeManager.GetTypeReferenceId(this.typeReference),
                templateRootId,
                this.htmlElementId,
                this.IsFactoryRoot
                    ? ".clone()"
                    : string.Empty);

            foreach (var binding in this.dataBindings)
            {
                binding.WriteBinding(elementId, writer);
            }

            foreach (var binding in this.templateBindings)
            {
                binding.WriteBinding(elementId, writer);
            }

            foreach (var binding in this.cssBindings)
            {
                binding.WriteBinding(elementId, writer, styleNameMapping);
            }

            foreach (var propertyBinding in this.propertiesToSet)
            {
                if (propertyBinding.Item1.PropertyReference != null)
                {
                    writer.Write("{0}.{1}(",
                        elementId,
                        propertyBinding.Item1.PropertyReference.PropertyDefinition.Setter);
                }
                else
                {
                    writer.Write("{0}.{1}({2}.{3},",
                        elementId,
                        DataBindingInfo.UIObjectType.ResolveMethod(
                            "SetValue",
                            false,
                            DataBindingInfo.DependencyPropertyType,
                            DataBindingInfo.ObjectType),
                        TypeManager.GetTypeReferenceId(
                            propertyBinding.Item1.FieldReference.Parent),
                        propertyBinding.Item1.FieldReference);
                }

                propertyBinding.Item2.WriteValueToJs(writer);

                writer.WriteLine(");");
            }
        }
        #endregion
    }
}
