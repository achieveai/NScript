//-----------------------------------------------------------------------
// <copyright file="ScriptSharpAttributes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using NScript.PELoader.Reflection;

    /// <summary>
    /// Definition for ScriptSharpAttributes
    /// </summary>
    public class ScriptSharpAttributes
    {
        /// <summary>
        /// Backing field for AlternateSignatureAttribute
        /// </summary>
        private static readonly TypeReference alternateSignatureAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.AlternateSignatureAttribute"));

        /// <summary>
        /// Backing field for AttachedPropertyAttribute
        /// </summary>
        private static readonly TypeReference attachedPropertyAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.AttachedPropertyAttribute"));

        /// <summary>
        /// Backing field for GlobalMethodsAttribute
        /// </summary>
        private static readonly TypeReference globalMethodsAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.GlobalMethodsAttribute"));

        /// <summary>
        /// Backing field for IgnoreNamespaceAttribute
        /// </summary>
        private static readonly TypeReference ignoreNamespaceAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.IgnoreNamespaceAttribute"));

        /// <summary>
        /// Backing field for ImplementAttribute
        /// </summary>
        private static readonly TypeReference implementAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ImplementAttribute"));

        /// <summary>
        /// Backing field for ExtendedAttribute
        /// </summary>
        private static readonly TypeReference extendedAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ExtendedAttribute"));

        /// <summary>
        /// Backing field for IntrinsicAttribute
        /// </summary>
        private static readonly TypeReference intrinsicPropertyAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.IntrinsicPropertyAttribute"));

        /// <summary>
        /// Backing field for IntrinsicAttribute
        /// </summary>
        private static readonly TypeReference intrinsicFieldAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.IntrinsicFieldAttribute"));

        /// <summary>
        /// Backing field for NonScriptableAttribute
        /// </summary>
        private static readonly TypeReference nonScriptableAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.NonScriptableAttribute"));

        /// <summary>
        /// Backing field for NumericValuesAttribute
        /// </summary>
        private static readonly TypeReference numericValuesAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.NumericValuesAttribute"));

        /// <summary>
        /// Backing field for PreserveCaseAttribute
        /// </summary>
        private static readonly TypeReference preserveCaseAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.PreserveCaseAttribute"));

        /// <summary>
        /// Backing field for PreserveNameAttribute
        /// </summary>
        private static readonly TypeReference preserveNameAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.PreserveNameAttribute"));

        /// <summary>
        /// Backing field for RequiresAssemblyAttribute
        /// </summary>
        private static readonly TypeReference requiresAssemblyAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.RequiresAssemblyAttribute"));

        /// <summary>
        /// Backing field for ScriptAliasAttribute
        /// </summary>
        private static readonly TypeReference scriptAliasAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ScriptAliasAttribute"));

        /// <summary>
        /// Backing field for ScriptNameAttribute
        /// </summary>
        private static readonly TypeReference scriptNameAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ScriptNameAttribute"));

        /// <summary>
        /// Backing field for ScriptNamespaceAttribute
        /// </summary>
        private static readonly TypeReference scriptNamespaceAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ScriptNamespaceAttribute"));

        /// <summary>
        /// Backing field for ScriptQualiferAttribute
        /// </summary>
        private static readonly TypeReference scriptQualiferAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ScriptQualiferAttribute"));

        /// <summary>
        /// Backing field for ScriptSkipAttribute
        /// </summary>
        private static readonly TypeReference scriptSkipAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ScriptSkipAttribute"));

        /// <summary>
        /// Backing field for ScriptAttribute.
        /// </summary>
        private static readonly TypeReference scriptAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.ScriptAttribute"));

        /// <summary>
        /// Backing field for MakeStaticUsageAttribute.
        /// </summary>
        private static TypeReference makeStaticUsageAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.MakeStaticUsageAttribute"));

        /// <summary>
        /// Backing field for MakeStaticUsageAttribute.
        /// </summary>
        private static TypeReference psudoTypeAttribute =
            new TypeReference(
                Tuple.Create(
                    string.Empty,
                    "System.Runtime.CompilerServices.PsudoTypeAttribute"));

        /// <summary>
        /// Gets the alternate signature attribute.
        /// This attribute is used to mark over methods so that we can use single implementation without this attribute.
        /// </summary>
        /// <value>The alternate signature attribute.</value>
        public static TypeReference AlternateSignatureAttribute
        {
            get { return ScriptSharpAttributes.alternateSignatureAttribute; }
        }

        /// <summary>
        /// Gets the attached property attribute.
        /// </summary>
        /// <value>The attached property attribute.</value>
        public static TypeReference AttachedPropertyAttribute
        {
            get { return ScriptSharpAttributes.attachedPropertyAttribute; }
        }

        /// <summary>
        /// Gets the global methods attribute.
        /// This attribute is used to point that all the methods are in global scope.
        /// </summary>
        /// <value>The global methods attribute.</value>
        public static TypeReference GlobalMethodsAttribute
        {
            get { return ScriptSharpAttributes.globalMethodsAttribute; }
        }

        /// <summary>
        /// Gets the ignore namespace attribute.
        /// The type name is in global scope, so ignore the namespace.
        /// </summary>
        /// <value>The ignore namespace attribute.</value>
        public static TypeReference IgnoreNamespaceAttribute
        {
            get { return ScriptSharpAttributes.ignoreNamespaceAttribute; }
        }

        /// <summary>
        /// Gets the extended attribute.
        /// This is used for extended class where we don't need to implement any class internals.
        /// </summary>
        /// <value>The extended attribute.</value>
        public static TypeReference ExtendedAttribute
        {
            get { return ScriptSharpAttributes.extendedAttribute; }
        }

        /// <summary>
        /// Gets the intrinsic attribute.
        /// This is used for properties to tell that we don't need to generate get_ and set_ prefixed functions for them.
        /// </summary>
        /// <value>The intrinsic attribute.</value>
        public static TypeReference IntrinsicPropertyAttribute
        {
            get { return ScriptSharpAttributes.intrinsicPropertyAttribute; }
        }

        /// <summary>
        /// Gets the intrinsic attribute.
        /// This is used for properties to tell that we don't need to generate get_ and set_ prefixed functions for them.
        /// </summary>
        /// <value>The intrinsic attribute.</value>
        public static TypeReference IntrinsicFieldAttribute
        {
            get { return ScriptSharpAttributes.intrinsicFieldAttribute; }
        }

        /// <summary>
        /// Gets the non scriptable attribute.
        /// We don't even need to generate class with this attribute.
        /// </summary>
        /// <value>The non scriptable attribute.</value>
        public static TypeReference NonScriptableAttribute
        {
            get { return ScriptSharpAttributes.nonScriptableAttribute; }
        }

        /// <summary>
        /// Gets the numeric values attribute.
        /// </summary>
        /// <value>The numeric values attribute.</value>
        public static TypeReference NumericValuesAttribute
        {
            get { return ScriptSharpAttributes.numericValuesAttribute; }
        }

        /// <summary>
        /// Gets the preserve case attribute.
        /// </summary>
        /// <value>The preserve case attribute.</value>
        public static TypeReference PreserveCaseAttribute
        {
            get { return ScriptSharpAttributes.preserveCaseAttribute; }
        }

        /// <summary>
        /// Gets the preserve name attribute.
        /// </summary>
        /// <value>The preserve name attribute.</value>
        public static TypeReference PreserveNameAttribute
        {
            get { return ScriptSharpAttributes.preserveNameAttribute; }
        }

        /// <summary>
        /// Gets the requires assembly attribute.
        /// This is helpful in indicating cross assembly dependencies.
        /// </summary>
        /// <value>The requires assembly attribute.</value>
        public static TypeReference RequiresAssemblyAttribute
        {
            get { return ScriptSharpAttributes.requiresAssemblyAttribute; }
        }

        /// <summary>
        /// Gets the script alias attribute.
        /// The name function in JS should be specified by given alias.
        /// </summary>
        /// <value>The script alias attribute.</value>
        public static TypeReference ScriptAliasAttribute
        {
            get { return ScriptSharpAttributes.scriptAliasAttribute; }
        }

        /// <summary>
        /// Gets the script name attribute.
        /// ScripeName should be used for the class.
        /// </summary>
        /// <value>The script name attribute.</value>
        public static TypeReference ScriptNameAttribute
        {
            get { return ScriptSharpAttributes.scriptNameAttribute; }
        }

        /// <summary>
        /// Gets the script namespace attribute.
        /// Use the given namespace in JScript.
        /// </summary>
        /// <value>The script namespace attribute.</value>
        public static TypeReference ScriptNamespaceAttribute
        {
            get { return ScriptSharpAttributes.scriptNamespaceAttribute; }
        }

        /// <summary>
        /// Gets the script qualifier attribute.
        /// </summary>
        /// <value>The script qualifier attribute.</value>
        public static TypeReference ScriptQualifierAttribute
        {
            get { return ScriptSharpAttributes.scriptQualiferAttribute; }
        }

        /// <summary>
        /// Gets the script skip attribute.
        /// </summary>
        /// <value>The script skip attribute.</value>
        public static TypeReference ScriptSkipAttribute
        {
            get { return ScriptSharpAttributes.scriptSkipAttribute; }
        }

        /// <summary>
        /// Gets the script attribute.
        /// </summary>
        public static TypeReference ScriptAttribute
        {
            get { return ScriptSharpAttributes.scriptAttribute; }
        }

        /// <summary>
        /// Gets the make static usage attribute.
        /// </summary>
        public static TypeReference MakeStaticUsageAttribute
        {
            get { return ScriptSharpAttributes.makeStaticUsageAttribute; }
        }

        /// <summary>
        /// Gets the psudo type attribute.
        /// </summary>
        /// <value>
        /// The psudo type attribute.
        /// </value>
        public static TypeReference PsudoTypeAttribute
        {
            get { return ScriptSharpAttributes.psudoTypeAttribute; }
        }

        /// <summary>
        /// Determines whether the specified type reference is extended.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type reference is extended; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExtended(TypeReference typeReference)
        {
            return ScriptSharpAttributes.IsExtended(typeReference.Type);
        }

        /// <summary>
        /// Determines whether the specified type definition base is extended.
        /// </summary>
        /// <param name="typeDefinitionBase">The type definition base.</param>
        /// <returns>
        /// <c>true</c> if the specified type definition base is extended; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExtended(TypeDefinitionBase typeDefinitionBase)
        {
            TypeDefinition typeDefinition = typeDefinitionBase as TypeDefinition;

            if (typeDefinition == null)
            {
                // Only proper typeDefinition can be extended.
                return false;
            }

            for (int attributeIndex = 0;
                attributeIndex < typeDefinition.Attributes.Count;
                attributeIndex++)
            {
                GenericCustomAttribute attribute =
                    typeDefinition.Attributes[attributeIndex] as GenericCustomAttribute;

                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Ctor.Parent.Equals(
                    ScriptSharpAttributes.ExtendedAttribute))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="typeDefinitionBase">The type definition base.</param>
        /// <returns>Type name of the given type. If ScriptName attribute is set then use it.</returns>
        public static string GetTypeName(TypeDefinitionBase typeDefinitionBase)
        {
            TypeDefinition typeDefinition = typeDefinitionBase as TypeDefinition;

            if (typeDefinition == null)
            {
                return typeDefinitionBase.Name;
            }

            bool ignoreNamespace = false;
            string nameSpace = null;

            foreach (var customAttributeBase in typeDefinition.Attributes)
            {
                GenericCustomAttribute attribute =
                    customAttributeBase as GenericCustomAttribute;
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Ctor.Parent.Equals(
                    ScriptSharpAttributes.scriptNameAttribute))
                {
                    return (string)attribute.Arguments[0];
                }

                if (attribute.Ctor.Parent.Equals(
                    ScriptSharpAttributes.IgnoreNamespaceAttribute))
                {
                    ignoreNamespace = true;
                }

                if (attribute.Ctor.Parent.Equals(
                    ScriptSharpAttributes.GlobalMethodsAttribute))
                {
                    return null;
                }

                if (attribute.Ctor.Parent.Equals(
                    ScriptSharpAttributes.ScriptNamespaceAttribute))
                {
                    ignoreNamespace = true;
                    nameSpace = (string)attribute.Arguments[0];
                }
            }

            string returnValue = typeDefinition.Name;

            if (ignoreNamespace)
            {
                int dotIndex = returnValue.LastIndexOf('.');

                returnValue = returnValue.Substring(dotIndex + 1);
            }

            if (nameSpace != null)
            {
                returnValue = nameSpace + '.' + returnValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="memberDefinition">The member definition.</param>
        /// <param name="isFixedName">if set to <c>true</c> [is fixed name].</param>
        /// <returns>Member's name</returns>
        public static string GetMemberName(
            MemberDefinition memberDefinition,
            out bool isFixedName,
            out bool isAlias)
        {
            bool isParentExtended =
                ScriptSharpAttributes.IsExtended(memberDefinition.Parent);
            isFixedName = false;
            isAlias = false;

            MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
            PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;

            if (methodDefinition != null &&
                methodDefinition.AssociatedMember is PropertyDefinition)
            {
                propertyDefinition =
                    (PropertyDefinition) methodDefinition.AssociatedMember;
            }

            string name = null;
            GenericCustomAttribute attribute;

            if ((attribute = ScriptSharpAttributes.GetAttribute(
                memberDefinition.Attributes,
                ScriptSharpAttributes.ScriptNameAttribute)) != null)
            {
                isFixedName = true;
                return (string)attribute.Arguments[0];
            }
            else if (ScriptSharpAttributes.GetAttribute(
                memberDefinition.Attributes,
                ScriptSharpAttributes.preserveNameAttribute) != null)
            {
                isFixedName = true;
                return char.ToLowerInvariant(memberDefinition.Name[0])
                    + memberDefinition.Name.Substring(1);
            }
            else if ((attribute = ScriptSharpAttributes.GetAttribute(
                memberDefinition.Attributes,
                ScriptSharpAttributes.PreserveCaseAttribute)) != null)
            {
                isFixedName = true;
                return memberDefinition.Name;
            }
            else if (isParentExtended
                && !ScriptSharpAttributes.IsImplemented(memberDefinition))
            {
                isFixedName = true;
                attribute = ScriptSharpAttributes.GetAttribute(
                    memberDefinition.Attributes,
                    ScriptSharpAttributes.ScriptAliasAttribute);
                if (attribute != null)
                {
                    isAlias = true;
                    return (string)attribute.Arguments[0];
                }

                attribute = ScriptSharpAttributes.GetAttribute(
                    memberDefinition.Attributes,
                    ScriptSharpAttributes.ScriptNameAttribute);
                if (attribute != null)
                {
                    return (string)attribute.Arguments[0];
                }

                if (propertyDefinition != null)
                {
                    attribute = ScriptSharpAttributes.GetAttribute(
                        propertyDefinition.Attributes,
                        ScriptSharpAttributes.ScriptAliasAttribute);

                    if (attribute != null)
                    {
                        isAlias = true;
                        return (string)attribute.Arguments[0];
                    }

                    attribute = ScriptSharpAttributes.GetAttribute(
                        propertyDefinition.Attributes,
                        ScriptSharpAttributes.ScriptNameAttribute);

                    if (attribute != null)
                    {
                        return (string)attribute.Arguments[0];
                    }

                    if (propertyDefinition.Parent.Name == "System.Number")
                    {
                        // All properties of Number are AS IS properties and don't change the names.
                        name = propertyDefinition.Name;
                    }

                    if (ScriptSharpAttributes.GetAttribute(
                        propertyDefinition.Attributes,
                        ScriptSharpAttributes.IntrinsicPropertyAttribute) != null)
                    {
                        if (!isAlias)
                        {
                            string rv = propertyDefinition.Name;
                            name = char.ToLowerInvariant(rv[0]) + rv.Substring(1);
                        }
                    }
                }
                else if (ScriptSharpAttributes.IsCapsName(memberDefinition.Name))
                {
                    name = memberDefinition.Name;
                }
            }

            if (name != null)
            {
                return name;
            }

            if (propertyDefinition != null)
            {
                methodDefinition = methodDefinition ?? propertyDefinition.Getter ?? propertyDefinition.Setter;
                return methodDefinition.Name.Substring(0, 5).ToLowerInvariant()
                       + methodDefinition.Name.Substring(5);
            }

            return char.ToLowerInvariant(memberDefinition.Name[0])
                + memberDefinition.Name.Substring(1);
        }

        /// <summary>
        /// Determines whether the specified property definition is intrinsic property.
        /// </summary>
        /// <param name="propertyDefinition">The property definition.</param>
        /// <returns>
        /// <c>true</c> if [is intrinsic property] [the specified property definition]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIntrinsicProperty(PropertyDefinition propertyDefinition)
        {
            if (null !=
                ScriptSharpAttributes.GetAttribute(
                    propertyDefinition.Attributes,
                    ScriptSharpAttributes.IntrinsicPropertyAttribute))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="attributeBases">The attribute bases.</param>
        /// <param name="typeReferenceToMatch">The type reference to match.</param>
        /// <returns>GenericCustomAttribute if there is match, else null</returns>
        public static GenericCustomAttribute GetAttribute(
            IList<CustomAttributeBase> attributeBases,
            TypeReference typeReferenceToMatch)
        {
            for (int attributeIndex = 0;
                attributeIndex < attributeBases.Count;
                attributeIndex++)
            {
                GenericCustomAttribute attribute =
                    attributeBases[attributeIndex] as GenericCustomAttribute;

                if (attribute != null
                    && typeReferenceToMatch.Equals(attribute.Ctor.Parent))
                {
                    return attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="memberDefinition">The member definition.</param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsImplemented(MemberDefinition memberDefinition)
        {
            if (!ScriptSharpAttributes.IsExtended(memberDefinition.Parent))
            {
                return true;
            }

            MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
            if (methodDefinition != null)
            {
                return MethodConverter.CanImplement(methodDefinition);
            }

            FieldDefinition fieldDefinition = memberDefinition as FieldDefinition;
            if (fieldDefinition != null)
            {
                return ScriptSharpAttributes.GetAttribute(
                    fieldDefinition.Attributes,
                    ScriptSharpAttributes.implementAttribute) != null;
            }

            PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
            if (propertyDefinition != null)
            {
                if (propertyDefinition.Getter != null)
                {
                    return ScriptSharpAttributes.IsImplemented(propertyDefinition.Getter);
                }

                return ScriptSharpAttributes.IsImplemented(propertyDefinition.Setter);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified STR is caps name.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>
        /// <c>true</c> if the specified STR is caps name ; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCapsName(string str)
        {
            return str.ToUpperInvariant() == str;
        }

        internal static bool HasIgnoreNamespaceAttribute(TypeDefinition typeDefinition)
        {
            if (null !=
                ScriptSharpAttributes.GetAttribute(
                    typeDefinition.Attributes,
                    ScriptSharpAttributes.IgnoreNamespaceAttribute))
            {
                return true;
            }

            return false;
        }
    }
}
