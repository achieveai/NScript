//-----------------------------------------------------------------------
// <copyright file="ConverterInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    public enum ConverterArgType
    {
        Boolean,
        Integer,
        Float,
        Enum,
        String,
        StaticPropInfo
    }

    /// <summary>
    /// Definition for ConverterInfo
    /// </summary>
    public abstract class ConverterInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="arguments"> The arguments. </param>
        /// <param name="fromType">  Type of from. </param>
        /// <param name="toType">    Type of to. </param>
        protected ConverterInfo(
            List<Tuple<ConverterArgType, object>> arguments,
            TypeReference fromType,
            TypeReference toType)
        {
            this.Arguments = arguments;
            this.ToType = toType;
            this.FromType = fromType;
        }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public List<Tuple<ConverterArgType, object>> Arguments
        { get; private set; }

        /// <summary>
        /// Gets or sets the type of from.
        /// </summary>
        /// <value>
        /// The type of from.
        /// </value>
        public TypeReference FromType
        { get; private set; }

        /// <summary>
        /// Gets or sets the type of to.
        /// </summary>
        /// <value>
        /// The type of to.
        /// </value>
        public TypeReference ToType
        { get; private set; }

        /// <summary>
        /// Gets to and from methods.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <returns>
        /// to and from methods.
        /// </returns>
        public virtual Tuple<IIdentifier, IIdentifier> GetToAndFromMethods(
            CodeGenerator codeGenerator)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeConverterInfo : ConverterInfo
    {
        public TypeConverterInfo(
            TypeReference converterType,
            List<Tuple<ConverterArgType, object>> arguments,
            TypeReference fromType,
            TypeReference toType)
            :base (arguments, fromType, toType)
        {
            this.ConverterType = converterType;
        }
    
        public  TypeReference ConverterType
        { get; private set; }
    }

    public class DelegateConverterInfo : ConverterInfo
    {
        public DelegateConverterInfo(
            MethodReference methodReference,
            List<Tuple<ConverterArgType, object>> arguments,
            TypeReference fromType,
            TypeReference toType)
            : base(arguments, fromType, toType)
        {
            this.MethodReference = methodReference;
        }

        public MethodReference MethodReference { get; set; }
    }
}
