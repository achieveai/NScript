//-----------------------------------------------------------------------
// <copyright file="InternalPropertyRefernce.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Mono.Collections.Generic;

    /// <summary>
    /// Definition for InternalPropertyRefernce
    /// </summary>
    public class InternalPropertyReference : PropertyReference
    {
        private readonly MethodReference relatedMethod;
        private PropertyDefinition propertyDefinition;
        private Collection<ParameterDefinition> parameters;

        public InternalPropertyReference(
            MethodReference getter,
            MethodReference setter)
            : base(InternalPropertyReference.GetPropertyName(getter, setter),
                InternalPropertyReference.GetPropertyType(getter, setter))
        {
            base.DeclaringType = getter != null
                ? getter.DeclaringType
                : setter.DeclaringType;

            base.PropertyType = getter != null
                ? getter.ReturnType
                : setter.Parameters[0].ParameterType;

            if (getter != null)
            {
                this.parameters = getter.Parameters;
            }
            else
            {
                this.parameters = new Collection<ParameterDefinition>();

                for (int iParam = 1; iParam < setter.Parameters.Count; iParam++)
                {
                    this.parameters.Add(setter.Parameters[iParam]);
                }
            }

            this.relatedMethod = getter ?? setter;
            base.Name = relatedMethod.Resolve().GetPropertyDefinition().Name;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <returns>PropertyDefinition</returns>
        public override PropertyDefinition Resolve()
        {
            if (this.propertyDefinition == null)
            { this.propertyDefinition = this.relatedMethod.Resolve().GetPropertyDefinition(); }

            return this.propertyDefinition;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public override string FullName
        {
            get
            {
                return this.DeclaringType.FullName + "." + this.Resolve().Name;
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public override Mono.Collections.Generic.Collection<ParameterDefinition> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        private static TypeReference GetPropertyType(
            MethodReference getter,
            MethodReference setter)
        {
            if (getter != null)
            {
                return getter.ReturnType;
            }
            else
            {
                return setter.Parameters[0].ParameterType;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="getter">The getter.</param>
        /// <param name="setter">The setter.</param>
        /// <returns>Name of the property</returns>
        private static string GetPropertyName(
            MethodReference getter,
            MethodReference setter)
        {
            if (getter != null)
            {
                if (!getter.Resolve().IsGetter)
                {
                    throw new InvalidProgramException("getter is not a getter");
                }

                return getter.Resolve().GetPropertyDefinition().Name;
            }

            if (!setter.Resolve().IsSetter)
            {
                throw new InvalidProgramException("Setter is not a setter");
            }

            return setter.Resolve().GetPropertyDefinition().Name;
        }
    }
}
