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
            : base(arguments, fromType, toType)
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
            List<Tuple<ConverterArgType, object>> arguments)
            : base(arguments, methodReference.Parameters[0].ParameterType, methodReference.ReturnType)
        {
            this.MethodReference = methodReference;
        }

        public MethodReference MethodReference { get; private set; }

        public override Tuple<IIdentifier, IIdentifier> GetToAndFromMethods(CodeGenerator codeGenerator)
        {
            return new Tuple<IIdentifier, IIdentifier>(
                codeGenerator.GetToConverterIdentifier(this),
                null);
        }
    }

    public class ConverterInfoComparer : IEqualityComparer<ConverterInfo>
    {
        ListEqualityComparer<Tuple<ConverterArgType, object>> argComparer =
            new ListEqualityComparer<Tuple<ConverterArgType, object>>(ArgumentComparer.Instance);

        public bool Equals(ConverterInfo x, ConverterInfo y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null
                || y == null)
            {
                return false;
            }

            var xType = x.GetType();
            if (xType != y.GetType())
            {
                return false;
            }

            if (xType == typeof(DelegateConverterInfo))
            {
                var xDCI = (DelegateConverterInfo)x;
                var yDCI = (DelegateConverterInfo)y;

                if (!NScript.CLR.MemberReferenceComparer.Instance.Equals(
                        xDCI.MethodReference,
                        yDCI.MethodReference))
                {
                    return false;
                }

                return this.argComparer.Equals(
                    xDCI.Arguments,
                    yDCI.Arguments);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public int GetHashCode(ConverterInfo obj)
        {
            if (obj == null)
            {
                return 0;
            }

            DelegateConverterInfo dci = obj as DelegateConverterInfo;
            if (dci != null)
            {
                int rv = NScript.CLR.MemberReferenceComparer.Instance.GetHashCode(dci.MethodReference);
                if (dci.Arguments != null)
                foreach (var arg in dci.Arguments)
                {
                    rv ^= ArgumentComparer.Instance.GetHashCode(arg);
                }

                return rv;
            }

            return -1;
        }
    }

    public class ArgumentComparer : IEqualityComparer<Tuple<ConverterArgType, object>>
    {
        static ListEqualityComparer<MemberReference> propComparer =
            new ListEqualityComparer<MemberReference>(NScript.CLR.MemberReferenceComparer.Instance);

        static private ArgumentComparer instance;

        public static ArgumentComparer Instance
        {
            get
            {
                if (ArgumentComparer.instance == null)
                {
                    ArgumentComparer.instance = new ArgumentComparer();
                }

                return ArgumentComparer.instance;
            }
        }

        public bool Equals(
            Tuple<ConverterArgType, object> x,
            Tuple<ConverterArgType, object> y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null
                || y == null)
            {
                return false;
            }

            if (x.Item1 != y.Item1)
            {
                return false;
            }

            switch (x.Item1)
            {
                case ConverterArgType.Boolean:
                    return (Boolean)x.Item2 == (Boolean)y.Item2;
                case ConverterArgType.Integer:
                    return (Int32)x.Item2 == (Int32)y.Item2;
                case ConverterArgType.Float:
                    return (Double)x.Item2 == (Double)y.Item2;
                case ConverterArgType.Enum:
                    return Convert.ToInt32(x.Item2) == Convert.ToInt32(y.Item2);
                case ConverterArgType.String:
                    return (string)x.Item2 == (string)y.Item2;
                case ConverterArgType.StaticPropInfo:
                    return propComparer.Equals((List<MemberReference>)x.Item2, (List<MemberReference>)y.Item2);
                default:
                    return false;
            }
        }

        public int GetHashCode(Tuple<ConverterArgType, object> obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj.Item1 != ConverterArgType.StaticPropInfo)
            {
                return obj.Item2.GetHashCode() ^ (int)obj.Item1;
            }

            return propComparer.GetHashCode((List<MemberReference>)obj.Item2) ^ (int)obj.Item2;
        }
    }
}
