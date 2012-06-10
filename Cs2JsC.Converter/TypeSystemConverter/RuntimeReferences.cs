//-----------------------------------------------------------------------
// <copyright file="RuntimeReferences.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.PELoader.Reflection;
    using Cs2JsC.PELoader.Helpers;

    /// <summary>
    /// Definition for RuntimeReferences
    /// </summary>
    public class RuntimeReferences
    {
        /// <summary>
        /// Backing field for FunctionType.
        /// </summary>
        private static TypeReference functionType;

        /// <summary>
        /// Backing field for DelegateType.
        /// </summary>
        private static TypeReference delegateType;

        /// <summary>
        /// Backing field for FlagsAttributeType.
        /// </summary>
        private static TypeReference flagsAttributeType;

        /// <summary>
        /// Backing field for NativeArrayType.
        /// </summary>
        private static TypeReference nativeArrayType;

        /// <summary>
        /// Backing field for CreateDelegate
        /// </summary>
        private static MethodReference createDelegate;

        /// <summary>
        /// Backing field for CreateGenericDelegate.
        /// </summary>
        private static MethodReference createGenericDelegate;

        /// <summary>
        /// Backing field for CreateGenericInstanceDelegate.
        /// </summary>
        private static MethodReference createGenericInstanceDelegate;

        /// <summary>
        /// Backing field for StaticInstanceCreateDelegate
        /// </summary>
        private static MethodReference staticInstanceCreateDelegate;

        /// <summary>
        /// Backing field for ToStringMethod.
        /// </summary>
        private static MethodReference toStringMethod;

        /// <summary>
        /// Backing field for boxMethod
        /// </summary>
        private static MethodReference boxMethod;

        /// <summary>
        /// Backing field for UnboxMethod.
        /// </summary>
        private static MethodReference unboxMethod;

        /// <summary>
        /// Backing field for ValueBoxMethod
        /// </summary>
        private static MethodReference valueBoxMethod;

        /// <summary>
        /// Backing field for ValueUnboxMethod.
        /// </summary>
        private static MethodReference valueUnboxMethod;

        /// <summary>
        /// Backing field for RegisterReferenceTypeMethod.
        /// </summary>
        private static MethodReference registerRefTypeMethod;

        /// <summary>
        /// Backing field for RegisterStructTypeMethod.
        /// </summary>
        private static MethodReference registerStructTypeMethod;

        /// <summary>
        /// Backing field for RegisterEnumMethod.
        /// </summary>
        private static MethodReference registerEnumMethod;

        /// <summary>
        /// Backing field for RegisterInterfaceMethod.
        /// </summary>
        private static MethodReference registerInterfaceMethod;

        /// <summary>
        /// Backing field for GetDefaultMethod.
        /// </summary>
        private static MethodReference getDefaultValueMethod;

        /// <summary>
        /// Backing field for IsInstanceOf.
        /// </summary>
        private static MethodReference isInstanceOfMethod;

        /// <summary>
        /// Backing field for CastTypeMethod.
        /// </summary>
        private static MethodReference castTypeMethod;

        /// <summary>
        /// Backing field for AsTypeMehtod.
        /// </summary>
        private static MethodReference asTypeMethod;

        /// <summary>
        /// Backing field for EnumStrToValueMap.
        /// </summary>
        private static FieldReference enumStrToValueMapField;

        /// <summary>
        /// Backing field for Boxed value field.
        /// </summary>
        private static FieldReference boxedValueField;

        /// <summary>
        /// Backing field for PrototypeField.
        /// </summary>
        private static FieldReference prototypeField;

        /// <summary>
        /// Backing field for constructor field.
        /// </summary>
        private static FieldReference constructorField;

        /// <summary>
        /// Backing field for TypeIdField.
        /// </summary>
        private static FieldReference typeIdField;

        /// <summary>
        /// Backing field for TypeNameField.
        /// </summary>
        private static FieldReference typeNameField;

        /// <summary>
        /// Backing field for BaseTypeField.
        /// </summary>
        private static FieldReference baseTypeField;

        /// <summary>
        /// Backing field for InheritField.
        /// </summary>
        private static FieldReference inheritsField;

        /// <summary>
        /// Empty array for reuse.
        /// </summary>
        private static readonly TypeReferenceBase[] emptyArray = new TypeReferenceBase[0];

        /// <summary>
        /// Gets the type of the function.
        /// </summary>
        /// <value>
        /// The type of the function.
        /// </value>
        public static TypeReference FunctionType
        {
            get
            {
                if (RuntimeReferences.functionType == null)
                {
                    RuntimeReferences.functionType = new TypeReference(
                            Tuple.Create(
                                "mscorlib",
                                "System.Function"));

                }

                return RuntimeReferences.functionType;
            }
        }

        /// <summary>
        /// Gets the type of the delegate.
        /// </summary>
        /// <value>
        /// The type of the delegate.
        /// </value>
        public static TypeReference DelegateType
        {
            get
            {
                if (RuntimeReferences.delegateType == null)
                {
                    RuntimeReferences.delegateType = new TypeReference(
                        Tuple.Create(
                            "mscorlib",
                            "System.Delegate"));

                }

                return RuntimeReferences.delegateType;
            }
        }

        /// <summary>
        /// Gets the type of the flags attribute.
        /// </summary>
        /// <value>
        /// The type of the flags attribute.
        /// </value>
        public static TypeReference FlagsAttributeType
        {
            get
            {
                if (RuntimeReferences.flagsAttributeType == null)
                {
                    RuntimeReferences.flagsAttributeType = new TypeReference(
                        Tuple.Create(
                            "mscorlib",
                            "System.FlagsAttribute"));

                }

                return RuntimeReferences.flagsAttributeType;
            }
        }

        /// <summary>
        /// Gets or sets the native array signature.
        /// </summary>
        /// <value>
        /// The native array signature.
        /// </value>
        public static TypeReference NativeArrayType
        {
            get
            {
                if (RuntimeReferences.nativeArrayType == null)
                {
                    RuntimeReferences.nativeArrayType = new TypeReference(
                            Tuple.Create(
                                "mscorlib",
                                "System.NativeArray"));
                }

                return RuntimeReferences.nativeArrayType;
            }
        }

        /// <summary>
        /// Gets the create delegate.
        /// </summary>
        public static MethodReference CreateDelegate
        {
            get
            {
                if (RuntimeReferences.createDelegate == null)
                {
                    RuntimeReferences.createDelegate = new MethodReference(
                        "Create",
                        true,
                        false,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            KnownTypeSignatures.ObjectTypeSignature,
                        },
                        RuntimeReferences.DelegateType,
                        RuntimeReferences.DelegateType);
                }

                return RuntimeReferences.createDelegate;
            }
        }

        /// <summary>
        /// Gets the create generic delegate.
        /// </summary>
        public static MethodReference CreateGenericDelegate
        {
            get
            {
                if (RuntimeReferences.createGenericDelegate == null)
                {
                    RuntimeReferences.createGenericDelegate = new MethodReference(
                        "CreateGeneric",
                        true,
                        false,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            KnownTypeSignatures.ObjectTypeSignature,
                            RuntimeReferences.FunctionType,
                            RuntimeReferences.NativeArrayType
                        },
                        RuntimeReferences.DelegateType,
                        RuntimeReferences.DelegateType);
                }

                return RuntimeReferences.createGenericDelegate;
            }
        }

        /// <summary>
        /// Gets the create generic instance delegate.
        /// </summary>
        public static MethodReference CreateGenericInstanceDelegate
        {
            get
            {
                if (RuntimeReferences.createGenericInstanceDelegate == null)
                {
                    RuntimeReferences.createGenericInstanceDelegate = new MethodReference(
                        "CreateInstanceGeneric",
                        true,
                        false,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            KnownTypeSignatures.ObjectTypeSignature,
                            RuntimeReferences.NativeArrayType
                        },
                        RuntimeReferences.DelegateType,
                        RuntimeReferences.DelegateType);
                }

                return RuntimeReferences.createGenericInstanceDelegate;
            }
        }

        /// <summary>
        /// Gets the static instance create delegate.
        /// </summary>
        public static MethodReference StaticInstanceCreateDelegate
        {
            get
            {
                if (RuntimeReferences.staticInstanceCreateDelegate == null)
                {
                    RuntimeReferences.staticInstanceCreateDelegate = new MethodReference(
                        "StaticInstanceCreate",
                        true,
                        false,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            KnownTypeSignatures.ObjectTypeSignature,
                            RuntimeReferences.FunctionType
                        },
                        RuntimeReferences.DelegateType,
                        RuntimeReferences.DelegateType);
                }

                return RuntimeReferences.staticInstanceCreateDelegate;
            }
        }

        /// <summary>
        /// Gets the box method.
        /// </summary>
        public static MethodReference BoxMethod
        {
            get
            {
                if (RuntimeReferences.boxMethod == null)
                {
                    RuntimeReferences.boxMethod = new MethodReference(
                        "Box",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        KnownTypeSignatures.ObjectTypeSignature,
                        KnownTypeSignatures.TypeTypeSignature);

                }

                return RuntimeReferences.boxMethod;
            }
        }

        /// <summary>
        /// Gets the unbox method.
        /// </summary>
        public static MethodReference UnboxMethod
        {
            get
            {
                if (RuntimeReferences.unboxMethod == null)
                {
                    RuntimeReferences.unboxMethod = new MethodReference(
                        "Unbox",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        KnownTypeSignatures.ObjectTypeSignature,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.unboxMethod;
            }
        }

        /// <summary>
        /// Gets the value box method.
        /// </summary>
        public static MethodReference ValueBoxMethod
        {
            get
            {
                if (RuntimeReferences.valueBoxMethod == null)
                {
                    RuntimeReferences.valueBoxMethod = new MethodReference(
                        "Box",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        KnownTypeSignatures.ObjectTypeSignature,
                        KnownTypeSignatures.ValueTypeSignature);
                }

                return RuntimeReferences.valueBoxMethod;
            }
        }

        /// <summary>
        /// Gets the value unbox method.
        /// </summary>
        public static MethodReference ValueUnboxMethod
        {
            get
            {
                if (RuntimeReferences.valueUnboxMethod == null)
                {
                    RuntimeReferences.valueUnboxMethod = new MethodReference(
                        "Unbox",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        KnownTypeSignatures.ObjectTypeSignature,
                        KnownTypeSignatures.ValueTypeSignature);
                }

                return RuntimeReferences.valueUnboxMethod;
            }
        }

        /// <summary>
        /// Gets the get default method.
        /// </summary>
        public static MethodReference GetDefaultMethod
        {
            get
            {
                if (RuntimeReferences.getDefaultValueMethod == null)
                {
                    RuntimeReferences.getDefaultValueMethod = new MethodReference(
                        "GetDefaultValue",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        RuntimeReferences.emptyArray,
                        KnownTypeSignatures.ObjectTypeSignature,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.getDefaultValueMethod;
            }
        }

        /// <summary>
        /// Gets the register inteface.
        /// </summary>
        public static MethodReference RegisterIntefaceMethod
        {
            get
            {
                if (RuntimeReferences.registerInterfaceMethod == null)
                {
                    RuntimeReferences.registerInterfaceMethod = new MethodReference(
                        "RegisterInterface",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.registerInterfaceMethod;
            }
        }

        /// <summary>
        /// Gets the register enum.
        /// </summary>
        public static MethodReference RegisterEnumMethod
        {
            get
            {
                if (RuntimeReferences.registerEnumMethod == null)
                {
                    RuntimeReferences.registerEnumMethod = new MethodReference(
                        "RegisterEnum",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            KnownTypeSignatures.BoolTypeSignature,
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.registerEnumMethod;
            }
        }

        /// <summary>
        /// Gets the register reference type method.
        /// </summary>
        public static MethodReference RegisterReferenceTypeMethod
        {
            get
            {
                if (RuntimeReferences.registerRefTypeMethod == null)
                {
                    RuntimeReferences.registerRefTypeMethod = new MethodReference(
                        "RegisterReferenceType",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            KnownTypeSignatures.TypeTypeSignature,
                            new TypeReference(
                                KnownTypeSignatures.ArraySignature,
                                KnownTypeSignatures.TypeTypeSignature)
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.registerRefTypeMethod;
            }
        }

        /// <summary>
        /// Gets the register struct type method.
        /// </summary>
        public static MethodReference RegisterStructTypeMethod
        {
            get
            {
                if (RuntimeReferences.registerStructTypeMethod == null)
                {
                    RuntimeReferences.registerStructTypeMethod = new MethodReference(
                        "RegisterStructType",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.StringTypeSignature,
                            new TypeReference(
                                KnownTypeSignatures.ArraySignature,
                                KnownTypeSignatures.TypeTypeSignature)
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.registerStructTypeMethod;
            }
        }

        /// <summary>
        /// Gets to string method.
        /// </summary>
        public static MethodReference ToStringMethod
        {
            get
            {
                if (RuntimeReferences.toStringMethod == null)
                {
                    RuntimeReferences.toStringMethod = new MethodReference(
                        "ToString",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        RuntimeReferences.emptyArray,
                        null,
                        KnownTypeSignatures.ObjectTypeSignature);
                }

                return RuntimeReferences.toStringMethod;
            }
        }

        /// <summary>
        /// Gets the System.Type::IsInstanceOf method.
        /// </summary>
        public static MethodReference IsInstanceOfMethod
        {
            get
            {
                if (RuntimeReferences.isInstanceOfMethod == null)
                {
                    RuntimeReferences.isInstanceOfMethod = new MethodReference(
                        "IsInstanceOfType",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.isInstanceOfMethod;
            }
        }

        /// <summary>
        /// Gets the System.Type::CastType method.
        /// </summary>
        public static MethodReference CastTypeMethod
        {
            get
            {
                if (RuntimeReferences.castTypeMethod == null)
                {
                    RuntimeReferences.castTypeMethod = new MethodReference(
                        "CastType",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.castTypeMethod;
            }
        }

        /// <summary>
        /// Gets System.Type::AsType method.
        /// </summary>
        public static MethodReference AsTypeMethod
        {
            get
            {
                if (RuntimeReferences.asTypeMethod == null)
                {
                    RuntimeReferences.asTypeMethod = new MethodReference(
                        "AsType",
                        false,
                        true,
                        RuntimeReferences.emptyArray,
                        new TypeReferenceBase[]
                        {
                            KnownTypeSignatures.ObjectTypeSignature
                        },
                        null,
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.asTypeMethod;
            }
        }

        /// <summary>
        /// Gets the enum STR to value map field.
        /// </summary>
        public static FieldReference EnumStrToValueMapField
        {
            get
            {
                if (RuntimeReferences.enumStrToValueMapField == null)
                {
                    RuntimeReferences.enumStrToValueMapField = new FieldReference(
                        "enumStrToValueMap",
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.enumStrToValueMapField;
            }
        }

        /// <summary>
        /// Gets the boxed value field.
        /// </summary>
        public static FieldReference BoxedValueField
        {
            get
            {
                if (RuntimeReferences.boxedValueField == null)
                {
                    RuntimeReferences.boxedValueField = new FieldReference(
                        "boxedValue",
                        KnownTypeSignatures.ValueTypeSignature);
                }

                return RuntimeReferences.boxedValueField;
            }
        }

        /// <summary>
        /// Gets the prototype field.
        /// </summary>
        public static FieldReference PrototypeField
        {
            get
            {
                if (RuntimeReferences.prototypeField == null)
                {
                    RuntimeReferences.prototypeField = new FieldReference(
                        "Prototype",
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.prototypeField;
            }
        }

        /// <summary>
        /// Gets the constructor field.
        /// </summary>
        public static FieldReference ConstructorField
        {
            get
            {
                if (RuntimeReferences.constructorField == null)
                {
                    RuntimeReferences.constructorField = new FieldReference(
                        "Constructor",
                        KnownTypeSignatures.ObjectTypeSignature);
                }

                return RuntimeReferences.constructorField;
            }
        }

        /// <summary>
        /// Gets the type id field.
        /// </summary>
        public static FieldReference TypeIdField
        {
            get
            {
                if (RuntimeReferences.typeIdField == null)
                {
                    RuntimeReferences.typeIdField = new FieldReference(
                        "TypeId",
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.typeIdField;
            }
        }

        /// <summary>
        /// Gets the type name field.
        /// </summary>
        public static FieldReference TypeNameField
        {
            get
            {
                if (RuntimeReferences.typeNameField == null)
                {
                    RuntimeReferences.typeNameField = new FieldReference(
                        "FullName",
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.typeNameField;
            }
        }

        /// <summary>
        /// Gets the base type field.
        /// </summary>
        public static FieldReference BaseTypeField
        {
            get
            {
                if (RuntimeReferences.baseTypeField == null)
                {
                    RuntimeReferences.baseTypeField = new FieldReference(
                        "BaseType",
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.baseTypeField;
            }
        }

        /// <summary>
        /// Gets the inherits field.
        /// </summary>
        public static FieldReference InheritsField
        {
            get
            {
                if (RuntimeReferences.inheritsField == null)
                {
                    RuntimeReferences.inheritsField = new FieldReference(
                        "InheritsField",
                        KnownTypeSignatures.TypeTypeSignature);
                }

                return RuntimeReferences.inheritsField;
            }
        }
    }
}
