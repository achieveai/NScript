//-----------------------------------------------------------------------
// <copyright file="Resolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler
{
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Cs2JsC.PELoader.Reflection;

    /// <summary>
    /// Resolver for misc data (mostly type related).
    /// </summary>
    public class TypeInfoResolver
    {
        /// <summary>
        /// namespace uri regex string
        /// </summary>
        private const string TypeNamespaceUriCheckStr = @"^\w+(\.\w+)*:\w+(\.\w+)*$";

        /// <summary>
        /// namespace uri parser regex.
        /// </summary>
        private static readonly Regex typeNamespaceUriCheckRegex =
            new Regex(
                    TypeNamespaceUriCheckStr,
                    RegexOptions.Compiled);

        /// <summary>
        /// Xml reader being used for processing.
        /// </summary>
        private readonly XmlReader reader;

        /// <summary>
        /// Style mapping.
        /// </summary>
        private readonly StyleMapping styleMapping;

        /// <summary>
        /// fileName of the file being processed.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoResolver"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="styleMapping">The style mapping.</param>
        public TypeInfoResolver(
            XmlReader reader,
            StyleMapping styleMapping,
            string fileName)
        {
            this.fileName = fileName;
            this.reader = reader;
            this.styleMapping = styleMapping;
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
        /// Resolves the type.
        /// </summary>
        /// <param name="str">The typename string.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        /// <returns>Resolve type info object.</returns>
        public TypeReference ResolveType(
            string str,
            string defaultNamespace)
        {
            var typeParts = str.Split(':');
            string typeName = null;

            if (typeParts.Length != 2)
            {
                throw new System.ApplicationException();
            }
            else
            {
                string namesp = this.reader.LookupNamespace(typeParts[0]);

                if (string.IsNullOrEmpty(namesp))
                {
                    DefaultSettings.Default.KnownNamespaces.TryGetValue(
                        typeParts[0],
                        out namesp);
                }

                typeName = namesp + '.' + typeParts[1];

                var typeInfos = TypeManager.ResolveType(typeName);

                if (typeInfos != null && typeInfos.Count == 1)
                {
                    return typeInfos[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Resolves the property.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Resolved PropertyInfo.</returns>
        public PropertyReference ResolveProperty(
            TypeReference typeInfo,
            string propertyName)
        {
            return TypeManager.ResolveProperty(typeInfo, propertyName);
        }

        /// <summary>
        /// Resolves the static field.
        /// </summary>
        /// <param name="attributeStr">The attribute STR.</param>
        /// <returns>resolved field for given name</returns>
        public FieldReference ResolveStaticField(
            string attributeStr)
        {
            var typeParts = attributeStr.Split(':');
            string fullTypeName;

            if (typeParts.Length == 2)
            {
                fullTypeName =
                    string.Format("{0}.{1}",
                        this.reader.LookupNamespace(typeParts[0]),
                        typeParts[1]);
            }
            else if (typeParts.Length == 1)
            {
                fullTypeName = typeParts[0];
            }
            else
            {
                return null;
            }

            int lastDotIndex = fullTypeName.LastIndexOf('.');

            if (lastDotIndex <= 0)
            {
                return null;
            }

            string fieldName = fullTypeName.Substring(lastDotIndex + 1);
            fullTypeName = fullTypeName.Substring(0, lastDotIndex);

            var typeInfos = TypeManager.ResolveType(fullTypeName);

            // For now let's not iterate through all the types to
            // find the attached property.
            if (typeInfos == null || typeInfos.Count == 0 || typeInfos.Count > 1)
            {
                return null;
            }

            return TypeManager.ResolveField(typeInfos[0], fieldName);
        }

        /// <summary>
        /// Determines whether given string is TypeNamespaceUri
        /// </summary>
        /// <param name="str">The typeNamespaceUri string.</param>
        /// <returns>
        /// <c>true</c> if given string is TypeNamespaceUri; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTypeNamespaceUri(string str)
        {
            return TypeInfoResolver.typeNamespaceUriCheckRegex.IsMatch(str);
        }

        /// <summary>
        /// Gets the name of the style. 
        /// </summary>
        /// <param name="classNames">class names.</param>
        /// <returns>Mangled style names.</returns>
        public string GetStyleNames(string classNames)
        {
            string[] parts = classNames.Split(' ', '\t');

            StringBuilder cssClasses = new StringBuilder();

            for (int iPart = 0; iPart < parts.Length; iPart++)
            {
                if (iPart > 0)
                {
                    cssClasses.Append(' ');
                }

                cssClasses.Append(this.GetSingleStyleName(parts[iPart]));
            }

            return cssClasses.ToString();
        }

        /// <summary>
        /// Gets the name of the style. Expects a single class name. 
        /// </summary>
        /// <param name="className">Name of a single class.</param>
        /// <returns>Mangled style name.</returns>
        private string GetSingleStyleName(string className)
        {
            if (this.styleMapping.ContainsStyle(className))
            {
                return this.styleMapping.GetStyleId(className);
            }
            else
            {
                return className;
            }
        }

        /// <summary>
        /// Determines whether given nodeName is propertyNode.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns>Propertyinfo for the node.</returns>
        internal PropertyReference IsPropertyNode(
            TypeReference typeInfo,
            string nodeName)
        {
            string[] parts = nodeName.Split(':');

            if (parts.Length == 1)
            {
                return this.ResolveProperty(typeInfo, nodeName);
            }
            else if (parts.Length == 2)
            {
                string namespaceUri = this.reader.LookupNamespace(parts[0]);
                string[] nameParts = parts[1].Split('.');

                if (nameParts.Length != 2)
                {
                    return null;
                }

                var types = TypeManager.ResolveType(
                    namespaceUri + '.' + nameParts[0]);

                if (types == null || types.Count != 0)
                {
                    // TODO: throw exception for clashing types.
                    return null;
                }

                return this.ResolveProperty(
                    types[0],
                    nameParts[1]);
            }

            return null;
        }
    }
}
