//-----------------------------------------------------------------------
// <copyright file="ClrKnownReferences.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR
{
    using System;
    using System.Collections.Generic;
using Mono.Cecil;

    /// <summary>
    /// Definition for ClrKnownReferences
    /// </summary>
    public class ClrKnownReferences
    {
        public const string MSCorlibStr = "mscorlib";
        public const string SystemStr = "System";
        public const string CompilerServicesStr = "System.Runtime.CompilerServices";

        private readonly ClrContext clrContext;

        /// <summary>
        /// The type references.
        /// </summary>
        private Dictionary<string, TypeReference> typeReferences;

        /// <summary>
        /// The method references.
        /// </summary>
        private Dictionary<string, MethodReference> methodReferences;

        /// <summary>
        /// Backing field for the ModuleDefinition.
        /// </summary>
        private ModuleDefinition msCorlibModule;

        /// <summary>
        /// Backing field for VoidReference.
        /// </summary>
        private TypeReference voidReference;

        /// <summary>
        /// Backing field for BooleanReference
        /// </summary>
        private TypeReference booleanReference;

        /// <summary>
        /// Backing field for ByteReference.
        /// </summary>
        private TypeReference byteReference;

        /// <summary>
        /// Backign field for SByteReference
        /// </summary>
        private TypeReference sbyteReference;

        /// <summary>
        /// Backing field for UShort.
        /// </summary>
        private TypeReference ushortReference;

        /// <summary>
        /// Backing field for Short.
        /// </summary>
        private TypeReference shortReference;

        /// <summary>
        /// Backing field for UInt32.
        /// </summary>
        private TypeReference uint32Reference;

        /// <summary>
        /// Backing field for Int32.
        /// </summary>
        private TypeReference int32Reference;

        /// <summary>
        /// Backing field for UInt64.
        /// </summary>
        private TypeReference uint64Reference;

        /// <summary>
        /// Backing field for Int64.
        /// </summary>
        private TypeReference int64Reference;

        /// <summary>
        /// Backing field for IntPtr.
        /// </summary>
        private TypeReference intPtrReference;

        /// <summary>
        /// Backing field for UIntPtr.
        /// </summary>
        private TypeReference uintPtrReference;

        /// <summary>
        /// Backing field for SingleREference.
        /// </summary>
        private TypeReference singleReference;

        /// <summary>
        /// Backing field for Double.
        /// </summary>
        private TypeReference doubleReference;

        /// <summary>
        /// Backing field for Char.
        /// </summary>
        private TypeReference charReference;

        /// <summary>
        /// Backing field for String.
        /// </summary>
        private TypeReference stringReference;

        /// <summary>
        /// Backing field for Object.
        /// </summary>
        private TypeReference objectReference;

        /// <summary>
        /// Backing field for TypeType.
        /// </summary>
        private TypeReference typeTypeReference;

        /// <summary>
        /// Backing field for SystemArray.
        /// </summary>
        private TypeReference systemArrayReference;

        /// <summary>
        /// Backing field for Enum.
        /// </summary>
        private TypeReference enumReference;

        /// <summary>
        /// Backing field for MulticastDelegate.
        /// </summary>
        private TypeReference multicastDelegateReference;

        /// <summary>
        /// Backing field for ValueTypeREference.
        /// </summary>
        private TypeReference valueTypeReference;

        /// <summary>
        /// Backing field for Exception.
        /// </summary>
        private TypeReference exceptionReference;

        /// <summary>
        /// Backing field for CompilerGenerateAttributeReference.
        /// </summary>
        private TypeReference compilerGeneratedAttributeReference;

        /// <summary>
        /// Backing field for FunctionType.
        /// </summary>
        private TypeReference functionType;

        /// <summary>
        /// Backing field for DelegateType.
        /// </summary>
        private TypeReference delegateType;

        /// <summary>
        /// Backing field for FlagsAttributeType.
        /// </summary>
        private TypeReference flagsAttributeType;

        /// <summary>
        /// Backign field for RuntimeHelper
        /// </summary>
        private TypeReference runtimeHelperReference;

        /// <summary>
        /// Backing field for RuntimeFieldHandle
        /// </summary>
        private TypeReference runtimeFieldHandle;

        /// <summary>
        /// Backing field for RuntimeTypeHandle.
        /// </summary>
        private TypeReference runtimeTypeHandle;

        private TypeReference nullableType;

        private MethodReference initializeArrayReference;
        private MethodReference arrayLengthGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrKnownReferences"/> class.
        /// </summary>
        /// <param name="clrContext">The converter.</param>
        public ClrKnownReferences(ClrContext clrContext)
        {
            this.clrContext = clrContext;
        }

        /// <summary>
        /// Gets the type references.
        /// </summary>
        /// <value>
        /// The type references.
        /// </value>
        public Dictionary<string, TypeReference> TypeReferences
        { get { return this.typeReferences; } }

        /// <summary>
        /// Gets the method references.
        /// </summary>
        /// <value>
        /// The method references.
        /// </value>
        public Dictionary<string, MethodReference> MethodReferences
        {get { return this.methodReferences; } }

        /// <summary>
        /// Gets the void reference.
        /// </summary>
        public TypeReference Void
        {
            get
            {
                if (this.voidReference == null)
                {
                    this.voidReference =
                        this.MSCorlibModule.TypeSystem.Void;
                }

                return this.voidReference;
            }
        }

        /// <summary>
        /// Gets the boolean reference.
        /// </summary>
        public TypeReference Boolean
        {
            get
            {
                if (this.booleanReference == null)
                {
                    this.booleanReference =
                        this.MSCorlibModule.TypeSystem.Boolean;

                    this.booleanReference.IsValueType = true;
                }

                return this.booleanReference;
            }
        }

        /// <summary>
        /// Gets the byte reference.
        /// </summary>
        public TypeReference Byte
        {
            get
            {
                if (this.byteReference == null)
                {
                    this.byteReference =
                        this.MSCorlibModule.TypeSystem.Byte;

                    this.byteReference.IsValueType = true;
                }

                return this.byteReference;
            }
        }

        /// <summary>
        /// Gets the S byte reference.
        /// </summary>
        public TypeReference SByte
        {
            get
            {
                if (this.sbyteReference == null)
                {
                    this.sbyteReference =
                        this.MSCorlibModule.TypeSystem.SByte;

                    this.sbyteReference.IsValueType = true;
                }

                return this.sbyteReference;
            }
        }

        /// <summary>
        /// Gets the U short reference.
        /// </summary>
        public TypeReference UShort
        {
            get
            {
                if (this.ushortReference == null)
                {
                    this.ushortReference =
                        this.MSCorlibModule.TypeSystem.UInt16;

                    this.ushortReference.IsValueType = true;
                }

                return this.ushortReference;
            }
        }

        /// <summary>
        /// Gets the short reference.
        /// </summary>
        public TypeReference Short
        {
            get
            {
                if (this.shortReference == null)
                {
                    this.shortReference =
                        this.MSCorlibModule.TypeSystem.Int16;

                    this.shortReference.IsValueType = true;
                }

                return this.shortReference;
            }
        }

        /// <summary>
        /// Gets the U int32 reference.
        /// </summary>
        public TypeReference UInt32
        {
            get
            {
                if (this.uint32Reference == null)
                {
                    this.uint32Reference =
                        this.MSCorlibModule.TypeSystem.UInt32;

                    this.uint32Reference.IsValueType = true;
                }

                return this.uint32Reference;
            }
        }

        /// <summary>
        /// Gets the int32 reference.
        /// </summary>
        public TypeReference Int32
        {
            get
            {
                if (this.int32Reference == null)
                {
                    this.int32Reference =
                        this.MSCorlibModule.TypeSystem.Int32;

                    this.int32Reference.IsValueType = true;
                }

                return this.int32Reference;
            }
        }

        /// <summary>
        /// Gets the U int64 reference.
        /// </summary>
        public TypeReference UInt64
        {
            get
            {
                if (this.uint64Reference == null)
                {
                    this.uint64Reference =
                        this.MSCorlibModule.TypeSystem.UInt64;

                    this.uint64Reference.IsValueType = true;
                }

                return this.uint64Reference;
            }
        }

        /// <summary>
        /// Gets the int64 reference.
        /// </summary>
        public TypeReference Int64
        {
            get
            {
                if (this.int64Reference == null)
                {
                    this.int64Reference =
                        this.MSCorlibModule.TypeSystem.Int64;

                    this.int64Reference.IsValueType = true;
                }

                return this.int64Reference;
            }
        }

        /// <summary>
        /// Gets the U int PTR reference.
        /// </summary>
        public TypeReference UIntPtr
        {
            get
            {
                if (this.uintPtrReference == null)
                {
                    this.uintPtrReference =
                        this.MSCorlibModule.TypeSystem.UIntPtr;

                    this.uintPtrReference.IsValueType = true;
                }

                return this.uintPtrReference;
            }
        }

        /// <summary>
        /// Gets the int PTR reference.
        /// </summary>
        public TypeReference IntPtr
        {
            get
            {
                if (this.intPtrReference == null)
                {
                    this.intPtrReference =
                        this.MSCorlibModule.TypeSystem.IntPtr;

                    this.intPtrReference.IsValueType = true;
                }

                return this.intPtrReference;
            }
        }

        /// <summary>
        /// Gets the single reference.
        /// </summary>
        public TypeReference Single
        {
            get
            {
                if (this.singleReference == null)
                {
                    this.singleReference =
                        this.MSCorlibModule.TypeSystem.Single;

                    this.singleReference.IsValueType = true;
                }

                return this.singleReference;
            }
        }

        /// <summary>
        /// Gets the double reference.
        /// </summary>
        public TypeReference Double
        {
            get
            {
                if (this.doubleReference == null)
                {
                    this.doubleReference =
                        this.MSCorlibModule.TypeSystem.Double;

                    this.doubleReference.IsValueType = true;
                }

                return this.doubleReference;
            }
        }

        /// <summary>
        /// Gets the char reference.
        /// </summary>
        public TypeReference Char
        {
            get
            {
                if (this.charReference == null)
                {
                    this.charReference =
                        this.MSCorlibModule.TypeSystem.Char;

                    this.charReference.IsValueType = true;
                }

                return this.charReference;
            }
        }

        /// <summary>
        /// Gets the string reference.
        /// </summary>
        public TypeReference String
        {
            get
            {
                if (this.stringReference == null)
                {
                    this.stringReference =
                        this.MSCorlibModule.TypeSystem.String;
                }

                return this.stringReference;
            }
        }

        /// <summary>
        /// Gets the object reference.
        /// </summary>
        public TypeReference Object
        {
            get
            {
                if (this.objectReference == null)
                {
                    this.objectReference =
                        this.MSCorlibModule.TypeSystem.Object;
                }

                return this.objectReference;
            }
        }

        /// <summary>
        /// Gets the type type reference.
        /// </summary>
        public TypeReference TypeType
        {
            get
            {
                if (this.typeTypeReference == null)
                {
                    this.typeTypeReference =
                        this.GetTypeReference(
                            ClrKnownReferences.SystemStr,
                            "Type");
                }

                return this.typeTypeReference;
            }
        }

        /// <summary>
        /// Gets the enum reference.
        /// </summary>
        public TypeReference Enum
        {
            get
            {
                if (this.enumReference == null)
                {
                    this.enumReference = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "Enum");

                    this.enumReference.IsValueType = true;
                }

                return this.enumReference;
            }
        }

        /// <summary>
        /// Gets the system array reference.
        /// </summary>
        public TypeReference SystemArray
        {
            get
            {
                if (this.systemArrayReference == null)
                {
                    this.systemArrayReference =
                        this.GetTypeReference(
                            ClrKnownReferences.SystemStr,
                            "Array");
                }

                return this.systemArrayReference;
            }
        }

        /// <summary>
        /// Gets the multicast delegate reference.
        /// </summary>
        public TypeReference MulticastDelegate
        {
            get
            {
                if (this.multicastDelegateReference == null)
                {
                    this.multicastDelegateReference = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "MulticastDelegate");
                }

                return this.multicastDelegateReference;
            }
        }

        /// <summary>
        /// Gets the value type reference.
        /// </summary>
        public TypeReference ValueType
        {
            get
            {
                if (this.valueTypeReference == null)
                {
                    this.valueTypeReference = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "ValueType");
                }

                return this.valueTypeReference;
            }
        }

        /// <summary>
        /// Gets the exception reference.
        /// </summary>
        public TypeReference Exception
        {
            get
            {
                if (this.exceptionReference == null)
                {
                    this.exceptionReference = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "Exception");
                }

                return this.exceptionReference;
            }
        }

        /// <summary>
        /// Gets the compiler generated attribute reference.
        /// </summary>
        public TypeReference CompilerGeneratedAttribute
        {
            get
            {
                if (this.compilerGeneratedAttributeReference == null)
                {
                    this.compilerGeneratedAttributeReference = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "CompilerGeneratedAttribute");
                }

                return this.compilerGeneratedAttributeReference;
            }
        }

        /// <summary>
        /// Gets the type of the function.
        /// </summary>
        /// <value>
        /// The type of the function.
        /// </value>
        public TypeReference FunctionType
        {
            get
            {
                if (this.functionType == null)
                {
                    this.functionType = this.GetTypeReference(
                                ClrKnownReferences.SystemStr,
                                "Function");
                }

                return this.functionType;
            }
        }

        /// <summary>
        /// Gets the type of the delegate.
        /// </summary>
        /// <value>
        /// The type of the delegate.
        /// </value>
        public TypeReference DelegateType
        {
            get
            {
                if (this.delegateType == null)
                {
                    this.delegateType = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "Delegate");
                }

                return this.delegateType;
            }
        }

        /// <summary>
        /// Gets the type of the flags attribute.
        /// </summary>
        /// <value>
        /// The type of the flags attribute.
        /// </value>
        public TypeReference FlagsAttributeType
        {
            get
            {
                if (this.flagsAttributeType == null)
                {
                    this.flagsAttributeType = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "FlagsAttribute");
                }

                return this.flagsAttributeType;
            }
        }

        /// <summary>
        /// Gets the runtime helper reference.
        /// </summary>
        public TypeReference RuntimeHelper
        {
            get
            {
                if (this.runtimeHelperReference == null)
                {
                    this.runtimeHelperReference = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "RuntimeHelpers");
                }

                return this.runtimeHelperReference;
            }
        }

        /// <summary>
        /// Gets the runtime field handle.
        /// </summary>
        public TypeReference RuntimeFieldHandle
        {
            get
            {
                if (this.runtimeFieldHandle == null)
                {
                    this.runtimeFieldHandle = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "RuntimeFieldHandle");
                }

                return this.runtimeFieldHandle;
            }
        }

        /// <summary>
        /// Gets the runtime type handle.
        /// </summary>
        public TypeReference RuntimeTypeHandle
        {
            get
            {
                if (this.runtimeTypeHandle == null)
                {
                    this.runtimeTypeHandle = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "RuntimeTypeHandle");
                }

                return this.runtimeTypeHandle;
            }
        }

        /// <summary>
        /// Gets the NullableType.
        /// </summary>
        /// <value>
        /// The NullableType.
        /// </value>
        public TypeReference NullableType
        {
            get
            {
                if (this.nullableType == null)
                {
                    this.nullableType = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "Nullable`1");
                }

                return this.nullableType;
            }
        }

        /// <summary>
        /// Gets the initialize array reference.
        /// </summary>
        public MethodReference InitializeArray
        {
            get
            {
                if (this.initializeArrayReference == null)
                {
                    this.initializeArrayReference = this.GetMethodReference(
                        "InitializeArray",
                        this.Void,
                        this.RuntimeHelper,
                        this.SystemArray,
                        this.RuntimeFieldHandle);
                }

                return this.initializeArrayReference;
            }
        }

        /// <summary>
        /// Gets the array length getter.
        /// </summary>
        public MethodReference ArrayLengthGetter
        {
            get
            {
                if (this.arrayLengthGetter == null)
                {
                    this.arrayLengthGetter = this.GetMethodReference(
                        "get_Length",
                        this.Int32,
                        this.SystemArray);
                }

                return this.arrayLengthGetter;
            }
        }

        /// <summary>
        /// Gets a context for the colour.
        /// </summary>
        /// <value>
        /// The colour context.
        /// </value>
        public ClrContext ClrContext
        {
            get
            {
                return this.clrContext;
            }
        }

        /// <summary>
        /// Gets the MS corlib module.
        /// </summary>
        private ModuleDefinition MSCorlibModule
        {
            get
            {
                if (this.msCorlibModule == null)
                {
                    if (!this.clrContext.TryGetModuleDefinition(
                        ClrKnownReferences.MSCorlibStr,
                        out this.msCorlibModule))
                    {
                        throw new ApplicationException("MSCorlib not yet initialized");
                    }
                }

                return this.msCorlibModule;
            }
        }

        /// <summary>
        /// Gets the type reference.
        /// </summary>
        /// <param name="typeNamespace">The type namespace.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Type reference inside mscorlib.</returns>
        private TypeReference GetTypeReference(
            string typeNamespace,
            string typeName)
        {
            return new TypeReference(
                typeNamespace,
                typeName,
                this.MSCorlibModule,
                this.MSCorlibModule.TypeSystem.Corlib).Resolve();
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>MethodReference</returns>
        private MethodReference GetMethodReference(
            string methodName,
            TypeReference returnType,
            TypeReference declaringType,
            params TypeReference[] arguments)
        {
            MethodReference methodReference = new MethodReference(
                methodName,
                returnType,
                declaringType);

            foreach (var argument in arguments)
            {
                methodReference.Parameters.Add(
                    new ParameterDefinition(argument));
            }

            return methodReference;
        }
    }
}
