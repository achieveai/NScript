//-----------------------------------------------------------------------
// <copyright file="ConverterKnownReferences.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter
{
    using System;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for ConverterKnownReferences
    /// </summary>
    public class ConverterKnownReferences
    {
        public const string CollectionsStr = "System.Collections";
        private readonly ClrContext clrContext;
        private ModuleDefinition msCorlibModule;

        private TypeReference dictionaryEntry;
        private TypeReference ienumerableReference;
        private TypeReference ienumeratorReference;
        private TypeReference ignoreNamespaceAttribute;
        private TypeReference implementAttribute;
        private TypeReference importedAttribute;
        private TypeReference intrinsicPropertyAttribute;
        private TypeReference intrinsicFieldAttribute;
        private TypeReference nonScriptableAttribute;
        private TypeReference numericValueAttribute;
        private TypeReference preserveCaseAttribute;
        private TypeReference scriptAliasAttribute;
        private TypeReference scriptNamespaceAttribute;
        private TypeReference makeStaticUsageAttribute;
        private TypeReference psudoTypeAttribute;
        private TypeReference keepInstanceUsageAttribute;
        private TypeReference scriptAttribute;
        private TypeReference preserveNameAttribute;
        private TypeReference globalMethodsAttribute;
        private TypeReference ignoreGenericArgumentsAttribute;
        private TypeReference entryPointAttribute;

        /// <summary>
        /// Backing field for ScriptNameAttribute
        /// </summary>
        private TypeReference scriptNameAttribute;

        /// <summary>
        /// Backing field for NativeArrayType.
        /// </summary>
        private TypeReference nativeArrayType;

        /// <summary>
        /// Backing field for ArrayImplGeneric.
        /// </summary>
        private TypeReference arrayImpl;

        /// <summary>
        /// Class to replace Array type.
        /// </summary>
        private TypeReference arrayImplBase;

        /// <summary>
        /// Backing field for CreateDelegate
        /// </summary>
        private MethodReference createDelegate;

        /// <summary>
        /// Backing field for CreateDelegateFromFunction
        /// </summary>
        private MethodReference createDelegateFromFunction;

        /// <summary>
        /// Backing field for StaticInstanceCreateDelegate
        /// </summary>
        private MethodReference staticInstanceCreateDelegate;

        /// <summary>
        /// Backing field for CreateGenericDelegate.
        /// </summary>
        private MethodReference createGenericDelegate;

        /// <summary>
        /// Backing field for CreateGenericInstanceDelegate.
        /// </summary>
        private MethodReference createGenericInstanceDelegate;

        /// <summary>
        /// Backing Field for DelegateCombineMethod.
        /// </summary>
        private MethodReference delegateCombineMethod;

        /// <summary>
        /// Backing Field for DelegateRemoveMethod.
        /// </summary>
        private MethodReference delegateRemoveMethod;
        /// <summary>
        /// Backing field for boxMethod
        /// </summary>
        private MethodReference boxMethod;

        /// <summary>
        /// Backing field for UnboxMethod.
        /// </summary>
        private MethodReference unboxMethod;

        /// <summary>
        /// Backing field for ValueBoxMethod
        /// </summary>
        private MethodReference valueBoxMethod;

        /// <summary>
        /// Backing field for ValueUnboxMethod.
        /// </summary>
        private MethodReference valueUnboxMethod;

        /// <summary>
        /// Backing field for RegisterReferenceTypeMethod.
        /// </summary>
        private MethodReference registerRefTypeMethod;

        /// <summary>
        /// Backing field for RegisterStructTypeMethod.
        /// </summary>
        private MethodReference registerStructTypeMethod;

        /// <summary>
        /// Backing field for RegisterEnumMethod.
        /// </summary>
        private MethodReference registerEnumMethod;

        /// <summary>
        /// Backing field for RegisterInterfaceMethod.
        /// </summary>
        private MethodReference registerInterfaceMethod;

        /// <summary>
        /// Backing field for GetDefaultMethod.
        /// </summary>
        private MethodReference getDefaultValueMethod;

        /// <summary>
        /// Backing field for ToStringMethod.
        /// </summary>
        private MethodReference toStringMethod;

        /// <summary>
        /// Backing field for EnumToStringMethod.
        /// </summary>
        private MethodReference enumToStringMethod;

        /// <summary>
        /// Backing field for GetEnumeratorIEnumerbleMethod
        /// </summary>
        private MethodReference getEnumeratorIEnumerableMethod;

        /// <summary>
        /// Backing field for GetCurrentIEnumeratorMethod
        /// </summary>
        private MethodReference getCurrentIEnumeratorMethod;

        /// <summary>
        /// Backing field for EnumStrToValueMap.
        /// </summary>
        private FieldReference enumStrToValueMapField;

        /// <summary>
        /// Backing field for Boxed value field.
        /// </summary>
        private FieldReference boxedValueField;

        /// <summary>
        /// Backing field for PrototypeField.
        /// </summary>
        private FieldReference prototypeField;

        /// <summary>
        /// Backing field for constructor field.
        /// </summary>
        private FieldReference constructorField;

        /// <summary>
        /// Backing field for TypeIdField.
        /// </summary>
        private FieldReference typeIdField;

        /// <summary>
        /// Backing field for TypeNameField.
        /// </summary>
        private FieldReference typeNameField;

        /// <summary>
        /// Backing field for BaseTypeField.
        /// </summary>
        private FieldReference baseTypeField;

        /// <summary>
        /// Backing field for GenericClosureField;
        /// </summary>
        private FieldReference genericClosureField;

        /// <summary>
        /// Backing field for GenericParametersField.
        /// </summary>
        private FieldReference genericParametersField;

        /// <summary>
        /// Backing field for DictEntryKey.
        /// </summary>
        private PropertyReference dictEntryKey;

        /// <summary>
        /// Backing field for DictEntryValue.
        /// </summary>
        private PropertyReference dictEntryValue;

        /// <summary>
        /// Backing field for ArrayAccessor.
        /// </summary>
        private PropertyReference arrayAccessor;

        /// <summary>
        /// Backing field for NullableValueProperty.
        /// </summary>
        private PropertyReference nullableValueProperty;

        /// <summary>
        /// Backing field for MoveNextEnumeratorMethod.
        /// </summary>
        private MethodReference moveNextEnumeratorMethod;

        /// <summary>
        /// Backing field for ArrayImplNativeArrayCtor.
        /// </summary>
        private MethodReference arrayImplNativeArrayCtor;

        /// <summary>
        /// Backing field for ArrayImplLengthCtor.
        /// </summary>
        private MethodReference arrayImplLengthCtor;

        /// <summary>
        /// Backing store for ItemGetter.
        /// </summary>
        private MethodReference arrayItemGetter;

        /// <summary>
        /// Backing field for IsInstanceOfMethod.
        /// </summary>
        private MethodReference isInstanceOfMethod;

        /// <summary>
        /// Backing field for CastTypeMethod.
        /// </summary>
        private MethodReference castTypeMethod;

        /// <summary>
        /// Backing field for AsTypeMethod.
        /// </summary>
        private MethodReference asTypeMethod;

        private MethodReference arrayCloneMethod;

        private MethodReference arrayImplCloneMethod;

        private MethodReference arrayContainsMethod;

        private MethodReference arrayImplContainsMethod;

        private MethodReference arrayIndexOf1Method;

        private MethodReference arrayImplIndexOf1Method;

        private MethodReference arrayIndexOf2Method;

        private MethodReference arrayImplIndexOf2Method;

        private MethodReference arrayReverseMethod;

        private MethodReference arrayImplReverseMethod;

        private MethodReference arrayGetEnumeratorMethod;

        private MethodReference arrayImplGetEnumeratorMethod;

        private MethodReference arrayLengthGetter;

        private MethodReference arrayImplLengthGetter;

        private MethodReference typeDefaultConstructor;

        private MethodReference nullableBoxMethod;

        private MethodReference nullableUnboxMethod;

        public ConverterKnownReferences(ClrContext clrContext)
        {
            this.clrContext = clrContext;
        }

        /// <summary>
        /// Gets the I enumerator.
        /// </summary>
        public TypeReference IEnumerable
        {
            get
            {
                if (this.ienumerableReference == null)
                {
                    this.ienumerableReference = this.GetTypeReference(
                        ConverterKnownReferences.CollectionsStr,
                        "IEnumerable");
                }

                return ienumerableReference;
            }
        }

        /// <summary>
        /// Gets the I enumerator.
        /// </summary>
        public TypeReference IEnumerator
        {
            get
            {
                if (this.ienumeratorReference == null)
                {
                    this.ienumeratorReference = this.GetTypeReference(
                        ConverterKnownReferences.CollectionsStr,
                        "IEnumerator");
                }

                return ienumeratorReference;
            }
        }

        /// <summary>
        /// Gets the dictionary entry.
        /// </summary>
        public TypeReference DictionaryEntry
        {
            get
            {
                if (this.dictionaryEntry == null)
                {
                    this.dictionaryEntry = this.GetTypeReference(
                        ConverterKnownReferences.CollectionsStr,
                        "DictionaryEntry");
                }

                return this.dictionaryEntry;
            }
        }

        /// <summary>
        /// Gets the native array.
        /// </summary>
        public TypeReference NativeArray
        {
            get
            {
                if (this.nativeArrayType == null)
                {
                    this.nativeArrayType = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "NativeArray");
                }

                return this.nativeArrayType;
            }
        }

        /// <summary>
        /// Gets the ignore namespace attribute reference.
        /// </summary>
        public TypeReference IgnoreNamespaceAttribute
        {
            get
            {
                if (this.ignoreNamespaceAttribute == null)
                {
                    this.ignoreNamespaceAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "IgnoreNamespaceAttribute");
                }

                return this.ignoreNamespaceAttribute;
            }
        }

        /// <summary>
        /// Gets the implement attribute reference.
        /// </summary>
        public TypeReference ImplementAttribute
        {
            get
            {
                if (this.implementAttribute == null)
                {
                    this.implementAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ImplementAttribute");
                }

                return this.implementAttribute;
            }
        }

        /// <summary>
        /// Gets the imported attribute reference.
        /// </summary>
        public TypeReference ImportedAttribute
        {
            get
            {
                if (this.importedAttribute == null)
                {
                    this.importedAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ImportedAttribute");
                }

                return this.importedAttribute;
            }
        }

        /// <summary>
        /// Gets the intrinsic property attribute reference.
        /// </summary>
        public TypeReference IntrinsicPropertyAttribute
        {
            get
            {
                if (this.intrinsicPropertyAttribute == null)
                {
                    this.intrinsicPropertyAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "IntrinsicPropertyAttribute");
                }

                return this.intrinsicPropertyAttribute;
            }
        }

        /// <summary>
        /// Gets the intrinsic field attribute reference.
        /// </summary>
        public TypeReference IntrinsicFieldAttribute
        {
            get
            {
                if (this.intrinsicFieldAttribute == null)
                {
                    this.intrinsicFieldAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "IntrinsicFieldAttribute");
                }

                return this.intrinsicFieldAttribute;
            }
        }

        /// <summary>
        /// Gets the non scriptable attribute reference.
        /// </summary>
        public TypeReference NonScriptableAttribute
        {
            get
            {
                if (this.nonScriptableAttribute == null)
                {
                    this.nonScriptableAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "NonScriptableAttribute");
                }

                return this.nonScriptableAttribute;
            }
        }

        /// <summary>
        /// Gets the numeric value attribute reference.
        /// </summary>
        public TypeReference NumericValueAttribute
        {
            get
            {
                if (this.numericValueAttribute == null)
                {
                    this.numericValueAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "NumericValueAttribute");
                }

                return this.numericValueAttribute;
            }
        }

        /// <summary>
        /// Gets the preserve case attribute reference.
        /// </summary>
        public TypeReference PreserveCaseAttribute
        {
            get
            {
                if (this.preserveCaseAttribute == null)
                {
                    this.preserveCaseAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "PreserveCaseAttribute");
                }

                return this.preserveCaseAttribute;
            }
        }

        /// <summary>
        /// Gets the preserve name attribute reference.
        /// </summary>
        public TypeReference PreserveNameAttribute
        {
            get
            {
                if (this.preserveNameAttribute == null)
                {
                    this.preserveNameAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "PreserveNameAttribute");
                }

                return this.preserveNameAttribute;
            }
        }

        /// <summary>
        /// Gets the script alias attribute reference.
        /// </summary>
        public TypeReference ScriptAliasAttribute
        {
            get
            {
                if (this.scriptAliasAttribute == null)
                {
                    this.scriptAliasAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ScriptAliasAttribute");
                }

                return this.scriptAliasAttribute;
            }
        }

        /// <summary>
        /// Gets the script namespace attribute reference.
        /// </summary>
        public TypeReference ScriptNamespaceAttribute
        {
            get
            {
                if (this.scriptNamespaceAttribute == null)
                {
                    this.scriptNamespaceAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ScriptNamespaceAttribute");
                }

                return this.scriptNamespaceAttribute;
            }
        }

        /// <summary>
        /// Gets the script name attribute reference.
        /// </summary>
        public TypeReference ScriptNameAttribute
        {
            get
            {
                if (this.scriptNameAttribute == null)
                {
                    this.scriptNameAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ScriptNameAttribute");
                }

                return this.scriptNameAttribute;
            }
        }

        /// <summary>
        /// Gets the script attribute reference.
        /// </summary>
        public TypeReference ScriptAttribute
        {
            get
            {
                if (this.scriptAttribute == null)
                {
                    this.scriptAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ScriptAttribute");
                }

                return this.scriptAttribute;
            }
        }

        /// <summary>
        /// Gets the make static attribute reference.
        /// </summary>
        public TypeReference MakeStaticUsageAttribute
        {
            get
            {
                if (this.makeStaticUsageAttribute == null)
                {
                    this.makeStaticUsageAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "MakeStaticUsageAttribute");
                }

                return this.makeStaticUsageAttribute;
            }
        }

        /// <summary>
        /// Gets the psudo type attribute.
        /// </summary>
        public TypeReference PsudoTypeAttribute
        {
            get
            {
                if (this.psudoTypeAttribute == null)
                {
                    this.psudoTypeAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "PsudoTypeAttribute");
                }

                return this.makeStaticUsageAttribute;
            }
        }

        /// <summary>
        /// Gets the keep instance usage attribute.
        /// </summary>
        public TypeReference KeepInstanceUsageAttribute
        {
            get
            {
                if (this.keepInstanceUsageAttribute == null)
                {
                    this.keepInstanceUsageAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "KeepInstanceUsageAttribute");
                }

                return this.keepInstanceUsageAttribute;
            }
        }

        /// <summary>
        /// Gets the global methods attribute.
        /// This attribute is used to point that all the methods are in global scope.
        /// </summary>
        /// <value>The global methods attribute.</value>
        public TypeReference GlobalMethodsAttribute
        {
            get
            {
                if (this.globalMethodsAttribute == null)
                {
                    this.globalMethodsAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "GlobalMethodsAttribute");
                }

                return this.globalMethodsAttribute;
            }
        }

        /// <summary>
        /// Gets the ignore generic arguments attribute.
        /// </summary>
        public TypeReference IgnoreGenericArgumentsAttribute
        {
            get
            {
                if (this.ignoreGenericArgumentsAttribute == null)
                {
                    this.ignoreGenericArgumentsAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "IgnoreGenericArgumentsAttribute");
                }

                return this.ignoreGenericArgumentsAttribute;
            }
        }

        /// <summary>
        /// Gets the entry point attribute.
        /// </summary>
        public TypeReference EntryPointAttribute
        {
            get
            {
                if (this.entryPointAttribute == null)
                {
                    this.entryPointAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "EntryPointAttribute");
                }

                return this.entryPointAttribute;
            }
        }

        /// <summary>
        /// Gets the array impl.
        /// </summary>
        public TypeReference ArrayImplGeneric
        {
            get
            {
                if (this.arrayImpl == null)
                {
                    this.arrayImpl = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "ArrayG`1");
                }

                return this.arrayImpl;
            }
        }

        /// <summary>
        /// Gets the array impl.
        /// </summary>
        public TypeReference ArrayImplBase
        {
            get
            {
                if (this.arrayImplBase == null)
                {
                    this.arrayImplBase = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "ArrayImpl");
                }

                return this.arrayImplBase;
            }
        }

        /// <summary>
        /// Gets the get current I enumerator method.
        /// </summary>
        public MethodReference GetEnumeratorIEnumerableMethod
        {
            get
            {
                if (this.getEnumeratorIEnumerableMethod == null)
                {
                    this.getEnumeratorIEnumerableMethod = this.GetMethodReference(
                        "GetEnumerator",
                        this.IEnumerator,
                        this.IEnumerable);
                }

                return this.getEnumeratorIEnumerableMethod;
            }
        }

        /// <summary>
        /// Gets the get current I enumerator method.
        /// </summary>
        public MethodReference GetCurrentIEnumeratorMethod
        {
            get
            {
                if (this.getCurrentIEnumeratorMethod == null)
                {
                    this.getCurrentIEnumeratorMethod = this.GetMethodReference(
                        "get_Current",
                        this.ClrReferences.Object,
                        this.IEnumerator);
                }

                return this.getCurrentIEnumeratorMethod;
            }
        }

        /// <summary>
        /// Gets the move next enumerator method.
        /// </summary>
        public MethodReference MoveNextEnumeratorMethod
        {
            get
            {
                if (this.moveNextEnumeratorMethod == null)
                {
                    this.moveNextEnumeratorMethod = this.GetMethodReference(
                        "MoveNext",
                        this.ClrReferences.Boolean,
                        this.IEnumerator);
                }
                return this.moveNextEnumeratorMethod;
            }
        }

        /// <summary>
        /// Gets the create delegate.
        /// </summary>
        public MethodReference CreateDelegate
        {
            get
            {
                if (this.createDelegate == null)
                {
                    this.createDelegate = this.GetMethodReference(
                        "Create",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.String,
                        this.ClrReferences.Object);
                }

                return this.createDelegate;
            }
        }

        /// <summary>
        /// Gets the create delegate.
        /// </summary>
        public MethodReference CreateDelegateFromFunction
        {
            get
            {
                if (this.createDelegateFromFunction == null)
                {
                    this.createDelegate = this.GetMethodReference(
                        "Create",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.FunctionType);
                }

                return this.createDelegateFromFunction;
            }
        }

        /// <summary>
        /// Gets the static instance create delegate.
        /// </summary>
        public MethodReference StaticInstanceCreateDelegate
        {
            get
            {
                if (this.staticInstanceCreateDelegate == null)
                {
                    this.staticInstanceCreateDelegate = this.GetMethodReference(
                        "StaticInstanceCreate",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.String,
                        this.ClrReferences.Object,
                        this.ClrReferences.FunctionType);
                }

                return this.staticInstanceCreateDelegate;
            }
        }

        /// <summary>
        /// Gets the create generic delegate.
        /// </summary>
        public MethodReference CreateGenericDelegate
        {
            get
            {
                if (this.createGenericDelegate == null)
                {
                    this.createGenericDelegate = this.GetMethodReference(
                        "CreateGeneric",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.String,
                        this.ClrReferences.Object,
                        this.ClrReferences.FunctionType,
                        this.NativeArray);
                }

                return this.createGenericDelegate;
            }
        }

        /// <summary>
        /// Gets the create generic instance delegate.
        /// </summary>
        public MethodReference CreateGenericInstanceDelegate
        {
            get
            {
                if (this.createGenericInstanceDelegate == null)
                {
                    this.createGenericInstanceDelegate = this.GetMethodReference(
                        "CreateInstanceGeneric",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.String,
                        this.ClrReferences.Object,
                        this.NativeArray);
                }

                return this.createGenericInstanceDelegate;
            }
        }

        /// <summary>
        /// Gets the delegate combine method.
        /// </summary>
        public MethodReference DelegateCombineMethod
        {
            get
            {
                if (this.delegateCombineMethod == null)
                {
                    this.delegateCombineMethod = this.GetMethodReference(
                        "Combine",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType);
                }

                return this.delegateCombineMethod;
            }
        }

        /// <summary>
        /// Gets the delegate remove method.
        /// </summary>
        public MethodReference DelegateRemoveMethod
        {
            get
            {
                if (this.delegateRemoveMethod == null)
                {
                    this.delegateRemoveMethod = this.GetMethodReference(
                        "Remove",
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType,
                        this.ClrReferences.DelegateType);
                }

                return this.delegateRemoveMethod;
            }
        }

        /// <summary>
        /// Gets the array impl native array ctor.
        /// </summary>
        public MethodReference ArrayImplNativeArrayCtor
        {
            get
            {
                if (this.arrayImplNativeArrayCtor == null)
                {
                    this.arrayImplNativeArrayCtor = this.GetMethodReference(
                        ".ctor",
                        this.ClrReferences.Void,
                        this.ArrayImplGeneric,
                        this.NativeArray);
                }

                return this.arrayImplNativeArrayCtor;
            }
        }

        /// <summary>
        /// Gets the array impl native array ctor.
        /// </summary>
        public MethodReference ArrayImplLengthCtor
        {
            get
            {
                if (this.arrayImplLengthCtor == null)
                {
                    this.arrayImplLengthCtor = this.GetMethodReference(
                        ".ctor",
                        this.ClrReferences.Void,
                        this.ArrayImplGeneric,
                        this.ClrReferences.Int32);
                }

                return this.arrayImplLengthCtor;
            }
        }

        /// <summary>
        /// Gets the array clone method.
        /// </summary>
        public MethodReference ArrayCloneMethod
        {
            get
            {
                if (this.arrayCloneMethod == null)
                {
                    this.arrayCloneMethod = this.GetMethodReference(
                        "Clone",
                        this.ClrReferences.SystemArray,
                        this.ClrReferences.SystemArray);
                }

                return this.arrayCloneMethod;
            }
        }

        /// <summary>
        /// Gets the array contains method.
        /// </summary>
        public MethodReference ArrayContainsMethod
        {
            get
            {
                if (this.arrayContainsMethod == null)
                {
                    this.arrayContainsMethod = this.GetMethodReference(
                        "Contains",
                        this.ClrReferences.Boolean,
                        this.ClrReferences.SystemArray,
                        this.ClrReferences.Object);
                }

                return this.arrayContainsMethod;
            }
        }

        /// <summary>
        /// Gets the array index of1 method.
        /// </summary>
        public MethodReference ArrayIndexOf1Method
        {
            get
            {
                if (this.arrayIndexOf1Method == null)
                {
                    this.arrayIndexOf1Method = this.GetMethodReference(
                        "IndexOf",
                        this.ClrReferences.Int32,
                        this.ClrReferences.SystemArray,
                        this.ClrReferences.Object);
                }

                return this.arrayIndexOf1Method;
            }
        }

        /// <summary>
        /// Gets the array index of2 method.
        /// </summary>
        public MethodReference ArrayIndexOf2Method
        {
            get
            {
                if (this.arrayIndexOf2Method == null)
                {
                    this.arrayIndexOf2Method = this.GetMethodReference(
                        "IndexOf",
                        this.ClrReferences.Int32,
                        this.ClrReferences.SystemArray,
                        this.ClrReferences.Object,
                        this.ClrReferences.Int32);
                }

                return this.arrayIndexOf2Method;
            }
        }

        /// <summary>
        /// Gets the array reverse method.
        /// </summary>
        public MethodReference ArrayReverseMethod
        {
            get
            {
                if (this.arrayReverseMethod == null)
                {
                    this.arrayReverseMethod = this.GetMethodReference(
                        "Reverse",
                        this.ClrReferences.Void,
                        this.ClrReferences.SystemArray);
                }

                return this.arrayReverseMethod;
            }
        }

        /// <summary>
        /// Gets the array reverse method.
        /// </summary>
        public MethodReference ArrayGetEnumeratorMethod
        {
            get
            {
                if (this.arrayGetEnumeratorMethod == null)
                {
                    this.arrayGetEnumeratorMethod = this.GetMethodReference(
                        "GetEnumerator",
                        this.IEnumerator,
                        this.ClrReferences.SystemArray);
                }

                return this.arrayGetEnumeratorMethod;
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
                        this.ClrReferences.Int32,
                        this.ClrReferences.SystemArray);
                }

                return this.arrayLengthGetter;
            }
        }

        /// <summary>
        /// Gets the array length property.
        /// </summary>
        public PropertyReference ArrayLengthProperty
        {
            get { return this.arrayLengthGetter.Resolve().GetPropertyDefinition(); }
        }

        /// <summary>
        /// Gets the array impl clone method.
        /// </summary>
        public MethodReference ArrayImplCloneMethod
        {
            get
            {
                if (this.arrayImplCloneMethod == null)
                {
                    this.arrayImplCloneMethod = this.GetMethodReference(
                        "Clone",
                        this.ArrayImplBase,
                        this.ArrayImplBase);
                }

                return this.arrayImplCloneMethod;
            }
        }

        /// <summary>
        /// Gets the array impl contains method.
        /// </summary>
        public MethodReference ArrayImplContainsMethod
        {
            get
            {
                if (this.arrayImplContainsMethod == null)
                {
                    this.arrayImplContainsMethod = this.GetMethodReference(
                        "Contains",
                        this.ClrReferences.Boolean,
                        this.ArrayImplBase,
                        this.ClrReferences.Object);
                }

                return this.arrayImplContainsMethod;
            }
        }

        /// <summary>
        /// Gets the array impl index of1 method.
        /// </summary>
        public MethodReference ArrayImplIndexOf1Method
        {
            get
            {
                if (this.arrayImplIndexOf1Method == null)
                {
                    this.arrayImplIndexOf1Method = this.GetMethodReference(
                        "IndexOf",
                        this.ClrReferences.Int32,
                        this.ArrayImplBase,
                        this.ClrReferences.Object);
                }

                return this.arrayImplIndexOf1Method;
            }
        }

        /// <summary>
        /// Gets the array impl index of2 method.
        /// </summary>
        public MethodReference ArrayImplIndexOf2Method
        {
            get
            {
                if (this.arrayImplIndexOf2Method == null)
                {
                    this.arrayImplIndexOf2Method = this.GetMethodReference(
                        "IndexOf",
                        this.ClrReferences.Int32,
                        this.ArrayImplBase,
                        this.ClrReferences.Object,
                        this.ClrReferences.Int32);
                }

                return this.arrayImplIndexOf2Method;
            }
        }

        /// <summary>
        /// Gets the array impl reverse method.
        /// </summary>
        public MethodReference ArrayImplReverseMethod
        {
            get
            {
                if (this.arrayImplReverseMethod == null)
                {
                    this.arrayImplReverseMethod = this.GetMethodReference(
                        "Reverse",
                        this.ClrReferences.Void,
                        this.ArrayImplBase);
                }

                return this.arrayImplReverseMethod;
            }
        }

        /// <summary>
        /// Gets the array impl get enumerator method.
        /// </summary>
        public MethodReference ArrayImplGetEnumeratorMethod
        {
            get
            {
                if (this.arrayImplGetEnumeratorMethod == null)
                {
                    this.arrayImplGetEnumeratorMethod = this.GetMethodReference(
                        "GetEnumerator",
                        this.ClrReferences.Void,
                        this.ClrReferences.SystemArray);
                }

                return this.arrayImplGetEnumeratorMethod;
            }
        }

        /// <summary>
        /// Gets the array impl length getter.
        /// </summary>
        public MethodReference ArrayImplLengthGetter
        {
            get
            {
                if (this.arrayImplLengthGetter == null)
                {
                    this.arrayImplLengthGetter = this.GetMethodReference(
                        "get_Length",
                        this.ClrReferences.Int32,
                        this.ArrayImplBase);
                }

                return this.arrayImplLengthGetter;
            }
        }

        /// <summary>
        /// Gets the array impl length property.
        /// </summary>
        public PropertyReference ArrayImplLengthProperty
        {
            get { return this.arrayImplLengthGetter.Resolve().GetPropertyDefinition(); }
        }

        /// <summary>
        /// Gets the box method.
        /// </summary>
        public MethodReference BoxMethod
        {
            get
            {
                if (this.boxMethod == null)
                {
                    this.boxMethod = this.GetMethodReference(
                        "Box",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.boxMethod;
            }
        }

        /// <summary>
        /// Gets the unbox method.
        /// </summary>
        public MethodReference UnboxMethod
        {
            get
            {
                if (this.unboxMethod == null)
                {
                    this.unboxMethod = this.GetMethodReference(
                        "Unbox",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.unboxMethod;
            }
        }

        /// <summary>
        /// Gets the value box method.
        /// </summary>
        public MethodReference ValueBoxMethod
        {
            get
            {
                if (this.valueBoxMethod == null)
                {
                    this.valueBoxMethod = this.GetMethodReference(
                        "Box",
                        this.ClrReferences.Object,
                        this.ClrReferences.ValueType,
                        this.ClrReferences.Object);
                }

                return this.valueBoxMethod;
            }
        }

        /// <summary>
        /// Gets the value unbox method.
        /// </summary>
        public MethodReference ValueUnboxMethod
        {
            get
            {
                if (this.valueUnboxMethod == null)
                {
                    this.valueUnboxMethod = this.GetMethodReference(
                        "Unbox",
                        this.ClrReferences.Object,
                        this.ClrReferences.ValueType,
                        this.ClrReferences.Object);
                }

                return this.valueUnboxMethod;
            }
        }

        /// <summary>
        /// Gets the get default method.
        /// </summary>
        public MethodReference GetDefaultMethod
        {
            get
            {
                if (this.getDefaultValueMethod == null)
                {
                    this.getDefaultValueMethod = this.GetMethodReference(
                        "GetDefaultValue",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType);
                }

                return this.getDefaultValueMethod;
            }
        }

        /// <summary>
        /// Gets the get default constructor method.
        /// </summary>
        public MethodReference GetDefaultConstructorMethod
        {
            get
            {
                if (this.typeDefaultConstructor == null)
                {
                    this.typeDefaultConstructor = this.GetMethodReference(
                        "DefaultConstructor",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType);
                }

                return this.typeDefaultConstructor;
            }
        }

        /// <summary>
        /// Gets the nullable box method.
        /// </summary>
        public MethodReference NullableBoxMethod
        {
            get
            {
                if (this.nullableBoxMethod == null)
                {
                    var arg = new GenericInstanceType(this.ClrReferences.NullableType);
                    arg.GenericArguments.Add(this.ClrReferences.NullableType.GenericParameters[0]);

                    this.nullableBoxMethod = this.GetMethodReference(
                        "Box",
                        this.ClrReferences.Object,
                        this.ClrReferences.NullableType,
                        arg).Resolve();
                }

                return this.nullableBoxMethod;
            }
        }

        /// <summary>
        /// Gets the nullable unbox method.
        /// </summary>
        public MethodReference NullableUnboxMethod
        {
            get
            {
                if (this.nullableUnboxMethod == null)
                {
                    var rv = new GenericInstanceType(this.ClrReferences.NullableType);
                    rv.GenericArguments.Add(this.ClrReferences.NullableType.GenericParameters[0]);

                    this.nullableUnboxMethod = this.GetMethodReference(
                        "Unbox",
                        rv,
                        this.ClrReferences.NullableType,
                        this.ClrReferences.Object).Resolve();
                }

                return this.nullableUnboxMethod;
            }
        }

        /// <summary>
        /// Gets the nullable value property.
        /// </summary>
        public PropertyReference NullableValueProperty
        {
            get
            {
                if (this.nullableValueProperty == null)
                {
                    this.nullableValueProperty = new InternalPropertyReference(
                        this.GetMethodReference(
                            "get_Value",
                            this.ClrReferences.NullableType.GenericParameters[0],
                            this.ClrReferences.NullableType),
                        null);
                }

                return this.nullableValueProperty;
            }
        }

        /// <summary>
        /// Gets the array accessor.
        /// </summary>
        /// <param name="arrayElementType">Type of the array element.</param>
        /// <returns>
        /// Array element accessor property.
        /// </returns>
        public MethodReference NullableValuePropertyGetter(TypeReference elementType)
        {
            MethodDefinition methodDefinition = this.NullableValueProperty.Resolve().GetMethod;
            GenericInstanceType instanceType = new GenericInstanceType(methodDefinition.DeclaringType);
            instanceType.GenericArguments.Add(elementType);
            MethodReference getValueMethod = new MethodReference(
                methodDefinition.Name,
                methodDefinition.ReturnType,
                instanceType);

            getValueMethod.HasThis = methodDefinition.HasThis;

            return getValueMethod;
        }

        /// <summary>
        /// Gets the register inteface.
        /// </summary>
        public MethodReference RegisterIntefaceMethod
        {
            get
            {
                if (this.registerInterfaceMethod == null)
                {
                    this.registerInterfaceMethod = this.GetMethodReference(
                        "RegisterInterface",
                        this.ClrReferences.Void,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.String);
                }

                return this.registerInterfaceMethod;
            }
        }

        /// <summary>
        /// Gets the register enum.
        /// </summary>
        public MethodReference RegisterEnumMethod
        {
            get
            {
                if (this.registerEnumMethod == null)
                {
                    this.registerEnumMethod = this.GetMethodReference(
                        "RegisterEnum",
                        this.ClrReferences.Void,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.String,
                        this.ClrReferences.Boolean);
                }

                return this.registerEnumMethod;
            }
        }

        /// <summary>
        /// Gets the register reference type method.
        /// </summary>
        public MethodReference RegisterReferenceTypeMethod
        {
            get
            {
                if (this.registerRefTypeMethod == null)
                {
                    this.registerRefTypeMethod = this.GetMethodReference(
                        "RegisterReferenceType",
                        this.ClrReferences.Void,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.String,
                        this.ClrReferences.TypeType,
                        new ArrayType(this.ClrReferences.TypeType));
                }

                return this.registerRefTypeMethod;
            }
        }

        /// <summary>
        /// Gets the register struct type method.
        /// </summary>
        public MethodReference RegisterStructTypeMethod
        {
            get
            {
                if (this.registerStructTypeMethod == null)
                {
                    this.registerStructTypeMethod = this.GetMethodReference(
                        "RegisterStructType",
                        this.ClrReferences.Void,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.String,
                        new ArrayType(this.ClrReferences.TypeType));
                }

                return this.registerStructTypeMethod;
            }
        }

        /// <summary>
        /// Gets to string method.
        /// </summary>
        public MethodReference ToStringMethod
        {
            get
            {
                if (this.toStringMethod == null)
                {
                    this.toStringMethod = this.GetMethodReference(
                        "ToString",
                        this.ClrReferences.String,
                        this.ClrReferences.Object);
                }

                return this.toStringMethod;
            }
        }

        /// <summary>
        /// Gets Enum's toString.
        /// </summary>
        public MethodReference EnumToStringMethod
        {
            get
            {
                if (this.enumToStringMethod == null)
                {
                    this.enumToStringMethod = this.GetMethodReference(
                        "ToString",
                        this.ClrReferences.String,
                        this.ClrReferences.Enum,
                        this.ClrReferences.Enum,
                        this.ClrReferences.Int32);
                }

                return this.enumToStringMethod;
            }
        }

        /// <summary>
        /// Gets the array item getter.
        /// </summary>
        public MethodReference ArrayItemGetter
        {
            get
            {
                if (this.arrayItemGetter == null)
                {
                    this.arrayItemGetter = this.GetMethodReference(
                        "get_Item",
                        this.ArrayImplGeneric.GenericParameters[0],
                        this.ArrayImplGeneric,
                        this.ClrReferences.Int32);
                }

                return this.arrayItemGetter;
            }
        }

        /// <summary>
        /// Gets the is instance of method.
        /// </summary>
        public MethodReference IsInstanceOfMethod
        {
            get
            {
                if (this.isInstanceOfMethod == null)
                {
                    this.isInstanceOfMethod = this.GetMethodReference(
                        "IsInstanceOfType",
                        this.ClrReferences.Boolean,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.isInstanceOfMethod;
            }
        }

        /// <summary>
        /// Gets the cast type method.
        /// </summary>
        public MethodReference CastTypeMethod
        {
            get
            {
                if (this.castTypeMethod == null)
                {
                    this.castTypeMethod = this.GetMethodReference(
                        "CastType",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.castTypeMethod;
            }
        }

        /// <summary>
        /// Gets as type method.
        /// </summary>
        public MethodReference AsTypeMethod
        {
            get
            {
                if (this.asTypeMethod == null)
                {
                    this.asTypeMethod = this.GetMethodReference(
                        "AsType",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.asTypeMethod;
            }
        }

        /// <summary>
        /// Gets the enum STR to value map field.
        /// </summary>
        public FieldReference EnumStrToValueMapField
        {
            get
            {
                if (this.enumStrToValueMapField == null)
                {
                    this.enumStrToValueMapField = new FieldReference(
                        "enumStrToValueMap",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType);
                }

                return this.enumStrToValueMapField;
            }
        }

        /// <summary>
        /// Gets the boxed value field.
        /// </summary>
        public FieldReference BoxedValueField
        {
            get
            {
                if (this.boxedValueField == null)
                {
                    this.boxedValueField = new FieldReference(
                        "boxedValue",
                        this.ClrReferences.Object,
                        this.ClrReferences.ValueType);
                }

                return this.boxedValueField;
            }
        }

        /// <summary>
        /// Gets the prototype field.
        /// </summary>
        public FieldReference PrototypeField
        {
            get
            {
                if (this.prototypeField == null)
                {
                    this.prototypeField = new FieldReference(
                        "Prototype",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType);
                }

                return this.prototypeField;
            }
        }

        /// <summary>
        /// Gets the constructor field.
        /// </summary>
        public FieldReference ConstructorField
        {
            get
            {
                if (this.constructorField == null)
                {
                    this.constructorField = new FieldReference(
                        "Constructor",
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.constructorField;
            }
        }

        /// <summary>
        /// Gets the type id field.
        /// </summary>
        public FieldReference TypeIdField
        {
            get
            {
                if (this.typeIdField == null)
                {
                    this.typeIdField = new FieldReference(
                        "TypeId",
                        this.ClrReferences.String,
                        this.ClrReferences.TypeType);
                }

                return this.typeIdField;
            }
        }

        /// <summary>
        /// Gets the type name field.
        /// </summary>
        public FieldReference TypeNameField
        {
            get
            {
                if (this.typeNameField == null)
                {
                    this.typeNameField = new FieldReference(
                        "FullName",
                        this.ClrReferences.String,
                        this.ClrReferences.TypeType);
                }

                return this.typeNameField;
            }
        }

        /// <summary>
        /// Gets the base type field.
        /// </summary>
        public FieldReference BaseTypeField
        {
            get
            {
                if (this.baseTypeField == null)
                {
                    this.baseTypeField = new FieldReference(
                        "BaseType",
                        this.ClrReferences.TypeType,
                        this.ClrReferences.TypeType);
                }

                return this.baseTypeField;
            }
        }

        /// <summary>
        /// Gets the generic parameters field.
        /// </summary>
        public FieldReference GenericClosureField
        {
            get
            {
                if (this.genericClosureField == null)
                {
                    this.genericClosureField = new FieldReference(
                        "genericClosure",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType);
                }

                return this.genericClosureField;
            }
        }

        /// <summary>
        /// Gets the generic parameters field.
        /// </summary>
        public FieldReference GenericParametersField
        {
            get
            {
                if (this.genericParametersField == null)
                {
                    this.genericParametersField = new FieldReference(
                        "genericParameters",
                        new ArrayType(this.ClrReferences.TypeType),
                        this.ClrReferences.TypeType);
                }

                return this.genericParametersField;
            }
        }

        /// <summary>
        /// Gets the dict entry key.
        /// </summary>
        public PropertyReference DictEntryKey
        {
            get
            {
                if (this.dictEntryKey == null)
                {
                    this.dictEntryKey = this.GetMethodReference(
                        "get_Key",
                        this.ClrReferences.String,
                        this.DictionaryEntry).Resolve().GetPropertyDefinition();
                }

                return this.dictEntryKey;
            }
        }

        /// <summary>
        /// Gets the dict entry value.
        /// </summary>
        public PropertyReference DictEntryValue
        {
            get
            {
                if (this.dictEntryValue == null)
                {
                    this.dictEntryValue = this.GetMethodReference(
                        "get_Value",
                        this.ClrReferences.Object,
                        this.DictionaryEntry).Resolve().GetPropertyDefinition();
                }

                return this.dictEntryValue;
            }
        }

        /// <summary>
        /// Gets the array accessor.
        /// </summary>
        public PropertyReference ArrayAccessor
        {
            get
            {
                if (this.arrayAccessor == null)
                {
                    this.arrayAccessor =
                        this.ArrayItemGetter.Resolve().GetPropertyDefinition();
                }

                return this.arrayAccessor;
            }
        }

        /// <summary>
        /// Gets the array int arg ctor.
        /// </summary>
        /// <param name="arrayElementType">Type of the array element.</param>
        /// <returns>Generic ArrayG&lt;arrayElementType&gt;</returns>
        public MethodReference GetArrayIntArgCtor(TypeReference arrayElementType)
        {
            MethodDefinition methodDefinition = this.ArrayImplLengthCtor.Resolve();
            GenericInstanceType instanceType = new GenericInstanceType(methodDefinition.DeclaringType);
            instanceType.GenericArguments.Add(arrayElementType);
            MethodReference ctorMethod = new MethodReference(
                methodDefinition.Name,
                methodDefinition.ReturnType,
                instanceType);
            ctorMethod.Parameters.Add(methodDefinition.Parameters[0]);
            ctorMethod.HasThis = true;

            return ctorMethod;
        }

        /// <summary>
        /// Gets the array native array arg ctor.
        /// </summary>
        /// <param name="arrayElementType">Type of the array element.</param>
        /// <returns>
        /// Method reference with GenericInstanceType DeclaringType.
        /// </returns>
        public MethodReference GetArrayNativeArrayArgCtor(TypeReference arrayElementType)
        {
            MethodDefinition methodDefinition = this.ArrayImplNativeArrayCtor.Resolve();
            GenericInstanceType instanceType = new GenericInstanceType(methodDefinition.DeclaringType);
            instanceType.GenericArguments.Add(arrayElementType);
            MethodReference ctorMethod = new MethodReference(
                methodDefinition.Name,
                methodDefinition.ReturnType,
                instanceType);
            ctorMethod.Parameters.Add(methodDefinition.Parameters[0]);
            ctorMethod.HasThis = true;

            return ctorMethod;
        }

        /// <summary>
        /// Gets the array accessor.
        /// </summary>
        /// <param name="arrayElementType">Type of the array element.</param>
        /// <returns>
        /// Array element accessor property.
        /// </returns>
        public PropertyReference GetArrayAccessor(TypeReference arrayElementType)
        {
            MethodDefinition methodDefinition = this.ArrayItemGetter.Resolve();
            GenericInstanceType instanceType = new GenericInstanceType(methodDefinition.DeclaringType);
            instanceType.GenericArguments.Add(arrayElementType);
            MethodReference getItemMethod = new MethodReference(
                methodDefinition.Name,
                methodDefinition.ReturnType,
                instanceType);
            getItemMethod.Parameters.Add(methodDefinition.Parameters[0]);
            getItemMethod.HasThis = true;

            return new InternalPropertyReference(
                getItemMethod,
                null);
        }

        /// <summary>
        /// Gets the CLR references.
        /// </summary>
        private ClrKnownReferences ClrReferences
        { get { return this.clrContext.KnownReferences; } }

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

            methodReference.HasThis = methodReference.Resolve().HasThis;

            return methodReference;
        }
    }
}