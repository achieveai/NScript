//-----------------------------------------------------------------------
// <copyright file="BoundProperty.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.DataBinders
{
    using System.Collections.Generic;
    using NScript.PELoader.Reflection;
    using NScript.Template.Compiler.Attributes;

    /// <summary>
    /// Definition for BoundProperty
    /// </summary>
    public class BoundProperty
    {
        /// <summary>
        /// Backing field for PropertyReference.
        /// </summary>
        private readonly PropertyReference propertyReference;

        /// <summary>
        /// Backing field for FieldReference
        /// </summary>
        private readonly FieldReference fieldReference;

        /// <summary>
        /// Backing field for PropertyType.
        /// </summary>
        private readonly TypeReference propertyType;

        /// <summary>
        /// Backing field for ControlType.
        /// </summary>
        private readonly TypeReference controlType;

        /// <summary>
        /// Backing field for PropertyName.
        /// </summary>
        private readonly string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundProperty"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyReference">The property info.</param>
        private BoundProperty(
            TypeReference controlType,
            string propertyName,
            PropertyReference propertyReference)
        {
            this.controlType = controlType;
            this.propertyName = propertyName;
            this.propertyReference = propertyReference;
            this.propertyType =
                this.propertyReference.PropertyDefinition.PropertyType.ApplyGenericArguments(
                    this.propertyReference.Parent.Parameters,
                    null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundProperty"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="fieldReference">The field info.</param>
        /// <param name="typeReference">The type reference info.</param>
        private BoundProperty(
            TypeReference controlType,
            string propertyName,
            FieldReference fieldReference,
            TypeReference typeReference)
        {
            this.controlType = controlType;
            this.propertyName = propertyName;
            this.fieldReference = fieldReference;
            this.propertyType = typeReference;
        }

        /// <summary>
        /// Gets the type of the control.
        /// </summary>
        /// <value>The type of the control.</value>
        public TypeReference ControlType
        {
            get
            {
                return this.controlType;
            }
        }

        /// <summary>
        /// Gets the type of the parent.
        /// </summary>
        /// <value>The type of the parent.</value>
        public TypeReference ParentType
        {
            get
            {
                return this.propertyReference != null
                    ? this.propertyReference.Parent
                    : this.fieldReference.Parent;
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public TypeReference PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <value>The property info.</value>
        public PropertyReference PropertyReference
        {
            get
            {
                return this.propertyReference;
            }
        }

        /// <summary>
        /// Gets the field info.
        /// </summary>
        /// <value>The field info.</value>
        public FieldReference FieldReference
        {
            get
            {
                return this.fieldReference;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>
        /// Creates the specified resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="controlTypeReference">The control type info.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="strictDefinition">if set to <c>true</c> [strict definition].</param>
        /// <returns>A bound property if it is resolved null otherwise.</returns>
        public static BoundProperty Create(
            TypeInfoResolver resolver,
            TypeReference controlTypeReference,
            AttributeInfo attribute,
            bool strictDefinition)
        {
            string propertyName = strictDefinition
                ? attribute.LocalName
                : attribute.LocalName.Substring("data-".Length);

            if (!string.IsNullOrEmpty(attribute.AttributeNS))
            {
                propertyName = string.Format("{0}.{1}",
                    attribute.AttributeNS,
                    attribute.LocalName);
            }

            PropertyReference propertyReference = null;
            FieldReference fieldReference = null;

            if (string.IsNullOrEmpty(attribute.AttributeNS))
            {
                propertyReference = resolver.ResolveProperty(
                    controlTypeReference,
                    propertyName);
            }

            if (propertyReference != null)
            {
                return new BoundProperty(
                        controlTypeReference,
                        propertyName,
                        propertyReference);
            }

            fieldReference = resolver.ResolveStaticField(
                propertyName);

            if (fieldReference != null)
            {
                AttachedPropertyAttribute attrib = 
                    AttributeBase.GetAttribute<AttachedPropertyAttribute>(fieldReference.FieldDefinition.Attributes);

                if (attrib != null)
                {
                    return new BoundProperty(
                        controlTypeReference,
                        propertyName,
                        fieldReference,
                        attrib.TypeReference);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the specified control type info.
        /// </summary>
        /// <param name="controlTypeReference">The control type info.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A bound property if it is resolved null otherwise.</returns>
        public static BoundProperty Create(
            TypeReference controlTypeReference,
            string propertyName)
        {
            var propertyReference = TypeManager.ResolveProperty(controlTypeReference, propertyName);

            if (propertyReference != null)
            {
                return new BoundProperty(controlTypeReference, propertyName, propertyReference);
            }
            else
            {
                return null;
            }
        }
    }
}
