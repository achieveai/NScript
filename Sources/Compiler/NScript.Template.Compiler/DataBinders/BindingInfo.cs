//-----------------------------------------------------------------------
// <copyright file="BindingInfo.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.DataBinders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using NScript.PELoader.Reflection;

    /// <summary>
    /// Binding info base for all binding classes.
    /// </summary>
    public class BindingInfo
    {
        /// <summary>
        /// Template binding string.
        /// </summary>
        public const string TemplatBindingStr = "TemplateBinding";

        /// <summary>
        /// Data binding string.
        /// </summary>
        public const string DataBindingStr = "DataBinding";

        /// <summary>
        /// Static binding string.
        /// </summary>
        public const string StaticBindingStr = "StaticBinding";

        /// <summary>
        /// Binding group string.
        /// </summary>
        public const string BindingGroupStr = @"binding";

        /// <summary>
        /// Path group string.
        /// </summary>
        public const string PathGroupStr = @"path";

        /// <summary>
        /// Namespace group string.
        /// </summary>
        public const string NamespaceGroupStr = @"namespace";

        /// <summary>
        /// more optinos group string.
        /// </summary>
        public const string MoreOptionsGroupStr = "moreOptions";

        /// <summary>
        /// Property group string.
        /// </summary>
        public const string PropGroupStr = @"prop";

        /// <summary>
        /// Binding value group string.
        /// </summary>
        public const string PropValueGroupStr = @"value";

        /// <summary>
        /// Binding converter string.
        /// </summary>
        public const string ConverterStr = "Converter";

        /// <summary>
        /// Binding mode string.
        /// </summary>
        public const string ModeStr = "Mode";

        /// <summary>
        /// Binder parser regex string.
        /// </summary>
        private const string ParseBinderStr = @"^[{\[]\s*(?<path>(?<namespace>[a-zA-Z_$][a-zA-Z_$0-9-]*:)?[a-zA-Z_$][a-zA-Z_$0-9-]*(\.[a-zA-Z_$][a-zA-Z_$0-9-]*)*)\s*(?<moreOptions>(,\s*[a-zA-Z_][a-zA-Z0-9_$-]*\s*=\s*[a-zA-Z_][a-zA-Z0-9_$-]*(:[a-zA-Z_][a-zA-Z0-9_$-]*)?\s*)*[}\]])$";

        /// <summary>
        /// Binding more info parser regex string.
        /// </summary>
        private const string ParseMoreInfoStr = @"(?<prop>(Converter)|(Mode)|(CanCheck))\s*=\s*(?<value>[a-zA-Z_][a-zA-Z_$0-9-]*(:[a-zA-Z_][a-zA-Z0-9_$-]*)?)\s*[,}\]]";

        /// <summary>
        /// Binder regex.
        /// </summary>
        private static readonly Regex parseBinderRegex =
            new Regex(
                ParseBinderStr,
                RegexOptions.Compiled);

        /// <summary>
        /// Moreinfo parser regex.
        /// </summary>
        private static readonly Regex parseMoreInfoRegex =
            new Regex(
                ParseMoreInfoStr,
                RegexOptions.Compiled);

        /// <summary>
        /// Backing field for DataTypeBinder
        /// </summary>
        private static readonly TypeReference dataBinderType = new TypeReference(KnowTypeReferences.DataBinderSignature);

        /// <summary>
        /// Backing field for IValueConverter
        /// </summary>
        private static readonly TypeReference ivalueConverterType = new TypeReference(KnowTypeReferences.IValueConverterSignature);

        /// <summary>
        /// Backing field for String Type
        /// </summary>
        private static readonly TypeReference stringType = new TypeReference(KnowTypeReferences.StringTypeSignature);

        /// <summary>
        /// Backing field for ObjecType.
        /// </summary>
        private static readonly TypeReference objectType = new TypeReference(KnowTypeReferences.ObjectTypeSignature);

        /// <summary>
        /// Backing field for CssClassBinder
        /// </summary>
        private static readonly TypeReference cssClassBinderType = new TypeReference(KnowTypeReferences.CssClassBinderSignature);

        /// <summary>
        /// Backing field for UI Object.
        /// </summary>
        private static readonly TypeReference uiObjectType = new TypeReference(KnowTypeReferences.UIObjectTypeSignature);

        /// <summary>
        /// Backing field for Target Property Setter.
        /// </summary>
        private static readonly TypeReference targetPropertySetterType = new TypeReference(KnowTypeReferences.TargetPropertySetter);

        /// <summary>
        /// Backing field for Target Property Getter.
        /// </summary>
        private static readonly TypeReference targetPropertyGetterType = new TypeReference(KnowTypeReferences.TargetPropertyGetter);

        /// <summary>
        /// Backing field for BindingMode type.
        /// </summary>
        private static readonly TypeReference bindingModeType = new TypeReference(KnowTypeReferences.BindingMode);

        /// <summary>
        /// Backign field for dependency Property Type.
        /// </summary>
        private static readonly TypeReference dependencyPropertyType = new TypeReference(KnowTypeReferences.DepedencyObjectSignature);

        /// <summary>
        /// Source property path.
        /// </summary>
        private string sourcePropertyPath;

        /// <summary>
        /// Gets or sets the type of the data binder.
        /// </summary>
        /// <value>The type of the data binder.</value>
        public static TypeReference DataBinderType
        {
            get { return BindingInfo.dataBinderType; }
        }

        /// <summary>
        /// Gets or sets the type of the I value converter.
        /// </summary>
        /// <value>The type of the I value converter.</value>
        public static TypeReference IValueConverterType
        {
            get { return BindingInfo.ivalueConverterType; }
        }

        /// <summary>
        /// Gets or sets the type of the string.
        /// </summary>
        /// <value>The type of the string.</value>
        public static TypeReference StringType
        {
            get { return BindingInfo.stringType; }
        }

        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        /// <value>The type of the object.</value>
        public static TypeReference ObjectType
        {
            get { return BindingInfo.objectType; }
        }

        /// <summary>
        /// Gets or sets the type of the CSS class binder.
        /// </summary>
        /// <value>The type of the CSS class binder.</value>
        public static TypeReference CssClassBinderType
        {
            get { return BindingInfo.cssClassBinderType; }
        }

        /// <summary>
        /// Gets or sets the type of the UI object.
        /// </summary>
        /// <value>The type of the UI object.</value>
        public static TypeReference UIObjectType
        {
            get { return BindingInfo.uiObjectType; }
        }

        /// <summary>
        /// Gets or sets the type of the target property setter.
        /// </summary>
        /// <value>
        /// The type of the target property setter.
        /// </value>
        public static TypeReference TargetPropertySetterType
        {
            get { return BindingInfo.targetPropertySetterType; }
        }

        /// <summary>
        /// Gets or sets the type of the target property getter.
        /// </summary>
        /// <value>
        /// The type of the target property getter.
        /// </value>
        public static TypeReference TargetPropertyGetterType
        {
            get { return BindingInfo.targetPropertyGetterType; }
        }

        /// <summary>
        /// Gets or sets the type of the binding.
        /// </summary>
        /// <value>The type of the binding.</value>
        public static TypeReference BindingModeType
        {
            get { return BindingInfo.bindingModeType; }
        }

        /// <summary>
        /// Gets or sets the type of the dependency property.
        /// </summary>
        /// <value>The type of the dependency property.</value>
        public static TypeReference DependencyPropertyType
        {
            get { return BindingInfo.dependencyPropertyType; }
        }

        /// <summary>
        /// Gets or sets the source property path.
        /// </summary>
        /// <value>The source property path.</value>
        public string SourcePropertyPath
        {
            get
            {
                return this.sourcePropertyPath;
            }

            protected set
            {
                this.sourcePropertyPath = value;
            }
        }

        /// <summary>
        /// Parses the binding info.
        /// </summary>
        /// <param name="bindingValue">The binding value.</param>
        /// <returns>Information about the binding.</returns>
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

            string propertyPath = binderMatch.Groups[PathGroupStr].Value;
            string moreOptions = binderMatch.Groups[MoreOptionsGroupStr].Success
                ? binderMatch.Groups[MoreOptionsGroupStr].Value
                : null;

            if (binderMatch.Groups[NamespaceGroupStr].Success)
            {
                if (binderType == TemplatBindingStr)
                {
                    throw new ApplicationException("Static binding cannot use template binding syntax.");
                }

                binderType = StaticBindingStr;
            }

            Dictionary<string, string> moreInfoProperties = new Dictionary<string, string>();

            if (moreOptions != null)
            {
                var matches = parseMoreInfoRegex.Matches(moreOptions);

                for (int matchIndex = 0; matchIndex < matches.Count; matchIndex++)
                {
                    moreInfoProperties.Add(
                        matches[matchIndex].Groups[PropGroupStr].Value,
                        matches[matchIndex].Groups[PropValueGroupStr].Value);
                }
            }

            return new Tuple<string, string, Dictionary<string, string>>(
                binderType,
                propertyPath,
                moreInfoProperties);
        }

        /// <summary>
        /// Adds to reference.
        /// </summary>
        public virtual void AddToReference()
        {
        }
    }
}
