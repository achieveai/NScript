using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Owa.TemplateParser.TypeData;

namespace Owa.TemplateParser
{
    public class BindingInfo
    {
        public const string TemplatBindingStr = "TemplateBinding";
        private const string DataBindingStr = "DataBinding";

        public static TypeInfo DataBinderType = null;
        public static TypeInfo IValueConverterType = null;
        public static TypeInfo StringType = null;
        public static TypeInfo ObjectType = null;

        public static TypeInfo CssClassBinderType = null;
        public static TypeInfo UIObjectType = null;

        public const string bindingGroupStr = @"binding";
        public const string pathGroupStr = @"path";
        public const string moreOptionsGroupStr = "moreOptions";
        public const string propGroupStr = @"prop";
        public const string propValueGroupStr = @"value";
        public const string converterStr = "Converter";
        public const string modeStr = "Mode";
        public const string canCheckStr = "CanCheck";

        const string parseBinderStr = @"^[{\[]\s*(?<path>[a-zA-Z_$][a-zA-Z_$0-9-]*(\.[a-zA-Z_$][a-zA-Z_$0-9-]*)*)\s*(?<moreOptions>(,\s*[a-zA-Z_][a-zA-Z0-9_$-]*\s*=\s*[a-zA-Z_][a-zA-Z0-9_$-]*(:[a-zA-Z_][a-zA-Z0-9_$-]*)?\s*)*[}\]])$";

        const string parseMoreInfoStr = @"(?<prop>(Converter)|(Mode)|(CanCheck))\s*=\s*(?<value>[a-zA-Z_][a-zA-Z_$0-9-]*(:[a-zA-Z_][a-zA-Z0-9_$-]*)?)\s*[,}\]]";

        public static readonly Regex parseBinderRegex =
            new Regex(
                parseBinderStr,
                RegexOptions.Compiled);

        public static readonly Regex parseMoreInfoRegex =
            new Regex(
                parseMoreInfoStr,
                RegexOptions.Compiled);

        protected string sourcePropertyPath;

        public static Tuple<string, string, Dictionary<string, string>> ParseBindingInfo(string bindingValue)
        {
            bindingValue = bindingValue.Trim();
            var binderMatch = parseBinderRegex.Match(bindingValue);

            if (!binderMatch.Success)
            {
                return null;
            }

            string binderType = bindingValue[0] == '{'
                    ? DataBindingStr
                    : TemplatBindingStr;

            string propertyPath = binderMatch.Groups[pathGroupStr].Value;
            string moreOptions = binderMatch.Groups[moreOptionsGroupStr].Success
                ? binderMatch.Groups[moreOptionsGroupStr].Value
                : null;

            Dictionary<string, string> moreInfoProperties = new Dictionary<string, string>();

            if (moreOptions != null)
            {
                var matches = parseMoreInfoRegex.Matches(moreOptions);

                for (int iMatch = 0; iMatch < matches.Count; iMatch++)
                {
                    moreInfoProperties.Add(
                        matches[iMatch].Groups[propGroupStr].Value,
                        matches[iMatch].Groups[propValueGroupStr].Value);
                }
            }

            return new Tuple<string, string, Dictionary<string, string>>(
                binderType,
                propertyPath,
                moreInfoProperties);
        }

        public virtual void AddToReference()
        {
            if (BindingInfo.DataBinderType == null)
            {
                BindingInfo.DataBinderType = TypeManager.ResolveType(KnownTypeSignaturesAndInfos.DataBinderSignature);
                BindingInfo.IValueConverterType = TypeManager.ResolveType(KnownTypeSignaturesAndInfos.IValueConverterSignature);
                BindingInfo.StringType = TypeManager.ResolveType(KnownTypeSignaturesAndInfos.StringTypeSignature);
                BindingInfo.ObjectType = TypeManager.ResolveType(KnownTypeSignaturesAndInfos.ObjectTypeSignature);
                BindingInfo.CssClassBinderType = TypeManager.ResolveType(KnownTypeSignaturesAndInfos.CssClassBinderSignature);
                BindingInfo.UIObjectType = TypeManager.ResolveType(KnownTypeSignaturesAndInfos.UIObjectTypeSignature);

                TypeManager.AddTypeReference(new TypeReferenceInfo(CssBindingInfo.CssClassBinderType));
            }
        }

        public string SourcePropertyPath
        { get { return this.sourcePropertyPath; } }
    }

    public class DataBindingInfo : BindingInfo
    {
        public const string OneWayStr = "OneWay";
        public const string OneTimeStr = "OneTime";
        private const string TwoWayStr = "TwoWay";

        string bindingMode = DataBindingInfo.OneWayStr;
        TypeInfo converterClass = null;
        PropertyInfo targetProperty = null;
        TypeReferenceInfo binderTypeReference = null;
        TypeReferenceInfo converterTypeReference = null;

        public DataBindingInfo(
            PropertyInfo targetProperty,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            TypeInfo dataContextType)
        {
            this.targetProperty = targetProperty;
            this.sourcePropertyPath = bindingValues.Item2;

            if (this.targetProperty.Getter == null ||
                this.targetProperty.Setter == null)
            {
                throw new ApplicationException(
                    string.Format(
                        "Target property for the control should have both setter and getter"));
            }

            string converterClass;
            if (bindingValues.Item3.TryGetValue(BindingInfo.modeStr, out this.bindingMode))
            {
                if (bindingMode != DataBindingInfo.OneTimeStr &&
                    bindingMode != DataBindingInfo.OneWayStr &&
                    bindingMode != DataBindingInfo.TwoWayStr)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Invalid BindingMode: {0}",
                            this.bindingMode));
                }
            }
            else
            {
                bindingMode = DataBindingInfo.OneWayStr;
            }

            if (bindingValues.Item3.TryGetValue(BindingInfo.converterStr, out converterClass))
            {
                this.converterClass = resolver.ResolveType(
                    converterClass,
                    DefaultSettings.Default.ConvertersNS);

                if (this.converterClass == null)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't resolve type {0}",
                            converterClass));
                }
            }

            PropertyInfo getter;
            if (!DataBindingInfo.GetEndPropertyGetter(
                this.sourcePropertyPath,
                dataContextType,
                out getter))
            {
                throw new ApplicationException(
                    string.Format(
                        "Can't find propertyPath {0} on {1}. Please check the propertyPath.",
                        this.sourcePropertyPath,
                        dataContextType));
            }
        }

        public string BindingMode
        { get { return this.bindingMode; } }

        public TypeInfo Converter
        { get { return this.converterClass; } }

        public PropertyInfo TargetProperty
        { get { return this.targetProperty; } }

        public override void AddToReference()
        {
            base.AddToReference();

            var controlTypeRef = new TypeReferenceInfo(this.TargetProperty.ParentType);
            var targetPropertyTypeRef = this.TargetProperty.PropertyType;

            TypeManager.AddTypeReference(controlTypeRef);
            TypeManager.AddTypeReference(targetPropertyTypeRef);

            if (this.Converter != null)
            {
                this.converterTypeReference = new TypeReferenceInfo(this.Converter);
                TypeManager.AddTypeReference(this.converterTypeReference);
            }

            this.binderTypeReference = 
                new TypeReferenceInfo(
                    DataBindingInfo.DataBinderType,
                    controlTypeRef,
                    targetPropertyTypeRef);

            TypeManager.AddTypeReference(this.binderTypeReference);
        }

        public void WriteBinding(string elementId, System.IO.StreamWriter writer)
        {
            writer.Write("{0}.{1}(",
                elementId,
                this.TargetProperty.ParentType.GetFunctionInfoUsingArgs(
                    this.GetAddBinderString(),
                    DataBindingInfo.DataBinderType).Slot);

            writer.Write("new {0}(",
                TypeManager.GetTypeReferenceId(this.binderTypeReference));

            writer.Write("\"{0}\",", this.SourcePropertyPath);
            writer.Write("\"{0}\",", this.TargetProperty.Name);

            writer.Write("function(a1){{return a1.{0}();}},",
                this.TargetProperty.Getter.Slot);

            writer.Write("function(a1,a2){{a1.{0}(a2);}},",
                this.TargetProperty.Setter.Slot);

            writer.Write("{0},", this.BindingModeToInt());
            if (this.converterTypeReference != null)
            {
                writer.Write("new {0}(),",
                    TypeManager.GetTypeReferenceId(this.converterTypeReference));
            }
            else
            {
                writer.Write("null,");
            }

            writer.WriteLine("null));");
        }

        public static bool GetEndPropertyGetter(
            string propertyPath,
            TypeInfo typeInfo,
            out PropertyInfo getterInfo)
        {
            getterInfo = null;

            if (typeInfo == null)
            {
                return true;
            }

            string[] propertyParts = propertyPath.Split('.');

            for (int iPart = 0; iPart < propertyParts.Length; iPart++)
            {
                if (iPart > 0)
                {
                    typeInfo = getterInfo.PropertyType.Type;
                }

                getterInfo = typeInfo.GetPropertyInfo(
                    propertyParts[iPart],
                    true);

                if (getterInfo == null ||
                    getterInfo.Getter == null)
                {
                    getterInfo = null;
                    return false;
                }
            }

            return true;
        }

        protected virtual string GetAddBinderString()
        {
            return "AddDataBinder";
        }

        private int BindingModeToInt()
        {
            switch (this.bindingMode)
            {
                case DataBindingInfo.OneWayStr:
                    return 1;
                case DataBindingInfo.TwoWayStr:
                    return 2;
                case DataBindingInfo.OneTimeStr:
                    return 0;
                default:
                    throw new InvalidOperationException("Invalid BindingMode");
            }
        }
    }

    class TemplateBindingInfo : DataBindingInfo
    {
        public TemplateBindingInfo(
            PropertyInfo targetProperty,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            TypeInfo templatedControlType)
            :base(targetProperty, bindingValues, resolver, templatedControlType)
        { }

        protected override string GetAddBinderString()
        {
            return "AddTemplateBinder";
        }
    }

    class CssBindingInfo : BindingInfo
    {
        string cssClass;
        bool isTemplateBound;
        TypeInfo converterClass;
        TypeReferenceInfo converterTypeReference = null;

        private static TypeReferenceInfo CssClassBinderTypeReference = null;

        public CssBindingInfo(
            string cssClass,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            TypeInfo dataContextType,
            TypeInfo templatedControlType)
        {
            this.cssClass = cssClass;
            this.isTemplateBound = bindingValues.Item1 == BindingInfo.TemplatBindingStr;
            this.sourcePropertyPath = bindingValues.Item2;

            string converterClass;
            if (bindingValues.Item3.TryGetValue(BindingInfo.converterStr, out converterClass))
            {
                this.converterClass = resolver.ResolveType(
                    converterClass,
                    DefaultSettings.Default.ConvertersNS);

                if (this.converterClass == null)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't resolve type {0}",
                            converterClass));
                }
            }

            if (this.isTemplateBound && templatedControlType != null)
            {
                PropertyInfo getter;
                if (!DataBindingInfo.GetEndPropertyGetter(
                    this.sourcePropertyPath,
                    templatedControlType,
                    out getter))
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't find propertyPath {0} on {1}. Please check the propertyPath.",
                            this.sourcePropertyPath,
                            templatedControlType));
                }
            }
            else if (!this.isTemplateBound && dataContextType != null)
            {
                PropertyInfo getter;

                if (!DataBindingInfo.GetEndPropertyGetter(
                    this.sourcePropertyPath,
                    dataContextType,
                    out getter))
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't find propertyPath {0} on {1}. Please check the propertyPath.",
                            this.sourcePropertyPath,
                            dataContextType));
                }
            }
        }

        public TypeInfo Converter
        { get { return this.converterClass; } }

        public string CssClass
        { get { return this.cssClass; } }

        public override void AddToReference()
        {
            base.AddToReference();

            if (CssBindingInfo.CssClassBinderTypeReference == null)
            {
                CssBindingInfo.CssClassBinderTypeReference = new TypeReferenceInfo(CssBindingInfo.CssClassBinderType);
                TypeManager.AddTypeReference(CssBindingInfo.CssClassBinderTypeReference);
            }

            if (this.converterClass != null)
            {
                this.converterTypeReference = new TypeReferenceInfo(this.converterClass);
                TypeManager.AddTypeReference(this.converterTypeReference);
            }
        }

        internal void WriteBinding(
            string elementId,
            System.IO.StreamWriter writer,
            StyleMapping styleNameMapping)
        {
            writer.Write("{0}.{1}(",
                elementId,
                BindingInfo.UIObjectType.GetFunctionInfoUsingArgs(
                    "AddCssBinder",
                    BindingInfo.CssClassBinderType).Slot);

            writer.Write("new {0}(",
                TypeManager.GetTypeReferenceId(new TypeReferenceInfo(BindingInfo.CssClassBinderType)));

            writer.Write("\"{0}\",",
                styleNameMapping.ContainsStyle(this.cssClass)
                    ? styleNameMapping.GetStyleId(this.cssClass)
                    : this.cssClass);

            writer.Write("\"{0}\",", this.SourcePropertyPath);

            if (this.converterTypeReference != null)
            {
                writer.Write("new {0}(),",
                    TypeManager.GetTypeReferenceId(this.converterTypeReference));
            }
            else
            {
                writer.Write("null,");
            }

            writer.WriteLine(
                "{0}));",
                this.isTemplateBound
                    ? "true"
                    : "false");
        }
    }
}
