//-----------------------------------------------------------------------
// <copyright file="TargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for TargetBindingInfo
    /// </summary>
    public abstract class TargetBindingInfo
    {
        /// <summary>
        /// Specialised constructor for use only by derived classes.
        /// </summary>
        /// <param name="bindingType"> The type of the binding. </param>
        protected TargetBindingInfo(TypeReference bindingType)
        {
            this.BindingType = bindingType;
        }

        /// <summary>
        /// Gets a value indicating whether we can have dynamic binding.
        /// </summary>
        /// <value>
        /// true if we can have dynamic binding, false if not.
        /// </value>
        public virtual bool CanHaveDynamicBinding
        { get { return false; } }

        /// <summary>
        /// Gets or sets the type of the binding.
        /// </summary>
        /// <value>
        /// The type of the binding.
        /// </value>
        public TypeReference BindingType
        { get; private set; }

        /// <summary>
        /// Gets binding mode.
        /// </summary>
        /// <param name="specifiedMode"> The specified mode. </param>
        /// <param name="knownTypes">    List of types of the knowns. </param>
        /// <returns>
        /// The binding mode.
        /// </returns>
        internal virtual BindingMode GetBindingMode(
            BindingMode specifiedMode,
            KnownTemplateTypes knownTypes)
        {
            return specifiedMode == BindingMode.Default
                ? BindingMode.OneTime
                : specifiedMode;
        }

        /// <summary>
        /// Generates a getter setter.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <param name="isTwoWay">      true if this object is two way. </param>
        /// <returns>
        /// The getter setter.
        /// </returns>
        internal virtual
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            throw new NotImplementedException();
        }
    }
}
