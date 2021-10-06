//-----------------------------------------------------------------------
// <copyright file="ConverterKnownReferences.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using System;
    using NScript.CLR;
    using NScript.CLR.AST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for ConverterKnownReferences.
    /// </summary>
    public class ConverterKnownReferences
    {
        /// <summary>
        /// The collections string.
        /// </summary>
        public const string CollectionsStr = "System.Collections";

        /// <summary>
        /// The collections string.
        /// </summary>
        public const string GenericCollectionsStr = "System.Collections.Generic";

        /// <summary>
        /// Context for the colour.
        /// </summary>
        private readonly ClrContext clrContext;

        /// <summary>
        /// The milliseconds corlib module.
        /// </summary>
        private ModuleDefinition msCorlibModule;

        /// <summary>
        /// The dictionary entry.
        /// </summary>
        private TypeReference dictionaryEntry;

        /// <summary>
        /// The ienumerable reference.
        /// </summary>
        private TypeReference ienumerableReference;

        /// <summary>
        /// The ienumerator reference.
        /// </summary>
        private TypeReference ienumeratorReference;

        /// <summary>
        /// The ignore namespace attribute.
        /// </summary>
        private TypeReference ignoreNamespaceAttribute;

        /// <summary>
        /// The implement attribute.
        /// </summary>
        private TypeReference implementAttribute;

        /// <summary>
        /// The extended attribute.
        /// </summary>
        private TypeReference extendedAttribute;

        /// <summary>
        /// The intrinsic property attribute.
        /// </summary>
        private TypeReference intrinsicPropertyAttribute;

        /// <summary>
        /// The intrinsic field attribute.
        /// </summary>
        private TypeReference intrinsicFieldAttribute;

        /// <summary>
        /// The intrinsic operator attribute.
        /// </summary>
        private TypeReference intrinsicOperatorAttribute;

        /// <summary>
        /// The non scriptable attribute.
        /// </summary>
        private TypeReference nonScriptableAttribute;

        /// <summary>
        /// The numeric value attribute.
        /// </summary>
        private TypeReference numericValueAttribute;

        /// <summary>
        /// The preserve case attribute.
        /// </summary>
        private TypeReference preserveCaseAttribute;

        /// <summary>
        /// The script alias attribute.
        /// </summary>
        private TypeReference scriptAliasAttribute;

        /// <summary>
        /// The script namespace attribute.
        /// </summary>
        private TypeReference scriptNamespaceAttribute;

        /// <summary>
        /// The make static usage attribute.
        /// </summary>
        private TypeReference makeStaticUsageAttribute;

        /// <summary>
        /// The list generic.
        /// </summary>
        private TypeReference listGeneric;

        /// <summary>
        /// The psudo type attribute.
        /// </summary>
        private TypeReference psudoTypeAttribute;

        /// <summary>
        /// The imported type attribute.
        /// </summary>
        private TypeReference importedTypeAttribute;

        /// <summary>
        /// The json type attribute.
        /// </summary>
        private TypeReference jsonTypeAttribute;

        /// <summary>
        /// The keep instance usage attribute.
        /// </summary>
        private TypeReference keepInstanceUsageAttribute;

        /// <summary>
        /// The script attribute.
        /// </summary>
        private TypeReference scriptAttribute;

        /// <summary>
        /// The preserve name attribute.
        /// </summary>
        private TypeReference preserveNameAttribute;

        /// <summary>
        /// The global methods attribute.
        /// </summary>
        private TypeReference globalMethodsAttribute;

        /// <summary>
        /// The ignore generic arguments attribute.
        /// </summary>
        private TypeReference ignoreGenericArgumentsAttribute;

        /// <summary>
        /// The entry point attribute.
        /// </summary>
        private TypeReference entryPointAttribute;

        /// <summary>
        /// The event binder.
        /// </summary>
        private TypeReference eventBinder;

        /// <summary>
        /// Backing field for ScriptNameAttribute.
        /// </summary>
        private TypeReference scriptNameAttribute;

        private TypeReference generatorWrapperType;

        private TypeReference nativeGeneratorType;

        /// <summary>
        /// Backing field for NativeArrayType.
        /// </summary>
        private TypeReference nativeArrayType;

        /// <summary>
        /// The native array type generic.
        /// </summary>
        private TypeReference nativeArrayTypeGeneric;

        /// <summary>
        /// Backing field for ArrayImplGeneric.
        /// </summary>
        private TypeReference arrayImpl;

        /// <summary>
        /// Class to replace Array type.
        /// </summary>
        private TypeReference arrayImplBase;

        /// <summary>
        /// Backing field for CreateDelegate.
        /// </summary>
        private MethodReference createDelegate;

        /// <summary>
        /// Backing field for CreateDelegateFromFunction.
        /// </summary>
        private MethodReference createDelegateFromFunction;

        /// <summary>
        /// Backing field for StaticInstanceCreateDelegate.
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
        /// Backing field for boxMethod.
        /// </summary>
        private MethodReference boxMethod;

        /// <summary>
        /// The nullable box method.
        /// </summary>
        private MethodReference typeNullableBoxMethod;

        /// <summary>
        /// Backing field for UnboxMethod.
        /// </summary>
        private MethodReference unboxMethod;

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
        /// The get default value method static.
        /// </summary>
        private MethodReference getDefaultValueMethodStatic;

        /// <summary>
        /// Backing field for ToStringMethod.
        /// </summary>
        private MethodReference toStringMethod;

        /// <summary>
        /// Backing field for EnumToStringMethod.
        /// </summary>
        private MethodReference enumToStringMethod;

        /// <summary>
        /// Backing field for GetEnumeratorIEnumerbleMethod.
        /// </summary>
        private MethodReference getEnumeratorIEnumerableMethod;

        /// <summary>
        /// Backing field for GetCurrentIEnumeratorMethod.
        /// </summary>
        private MethodReference getCurrentIEnumeratorMethod;

        /// <summary>
        /// Backing field for MoveNextEnumeratorMethod.
        /// </summary>
        private MethodReference moveNextEnumeratorMethod;

        private MethodReference generatorWrapperCtor;

        private MethodReference nativeGeneratorCtor;

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
        /// The list native array constructor.
        /// </summary>
        private MethodReference listNativeArrayCtor;

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

        /// <summary>
        /// The array clone method.
        /// </summary>
        private MethodReference arrayCloneMethod;

        /// <summary>
        /// The array implementation clone method.
        /// </summary>
        private MethodReference arrayImplCloneMethod;

        /// <summary>
        /// The array contains method.
        /// </summary>
        private MethodReference arrayContainsMethod;

        /// <summary>
        /// The array implementation contains method.
        /// </summary>
        private MethodReference arrayImplContainsMethod;

        /// <summary>
        /// The array index of 1 method.
        /// </summary>
        private MethodReference arrayIndexOf1Method;

        /// <summary>
        /// The array implementation index of 1 method.
        /// </summary>
        private MethodReference arrayImplIndexOf1Method;

        /// <summary>
        /// The array index of 2 method.
        /// </summary>
        private MethodReference arrayIndexOf2Method;

        /// <summary>
        /// The array implementation index of 2 method.
        /// </summary>
        private MethodReference arrayImplIndexOf2Method;

        /// <summary>
        /// The array reverse method.
        /// </summary>
        private MethodReference arrayReverseMethod;

        /// <summary>
        /// The array implementation reverse method.
        /// </summary>
        private MethodReference arrayImplReverseMethod;

        /// <summary>
        /// The array get enumerator method.
        /// </summary>
        private MethodReference arrayGetEnumeratorMethod;

        /// <summary>
        /// The array implementation get enumerator method.
        /// </summary>
        private MethodReference arrayImplGetEnumeratorMethod;

        /// <summary>
        /// The array length getter.
        /// </summary>
        private MethodReference arrayLengthGetter;

        /// <summary>
        /// The array implementation length getter.
        /// </summary>
        private MethodReference arrayImplLengthGetter;

        /// <summary>
        /// The array get value.
        /// </summary>
        private MethodReference arrayGetValue;

        /// <summary>
        /// The array implementation get value.
        /// </summary>
        private MethodReference arrayImplGetValue;

        /// <summary>
        /// The array set value.
        /// </summary>
        private MethodReference arraySetValue;

        /// <summary>
        /// The array implementation set value.
        /// </summary>
        private MethodReference arrayImplSetValue;

        /// <summary>
        /// The type default constructor.
        /// </summary>
        private MethodReference typeDefaultConstructor;

        /// <summary>
        /// The type get default constructor.
        /// </summary>
        private MethodReference typeGetDefaultConstructor;

        /// <summary>
        /// The nullable box method.
        /// </summary>
        private MethodReference nullableBoxMethod;

        /// <summary>
        /// The nullable unbox method.
        /// </summary>
        private MethodReference nullableUnboxMethod;

        /// <summary>
        /// The add event method.
        /// </summary>
        private MethodReference addEventMethod;

        /// <summary>
        /// The remove event method.
        /// </summary>
        private MethodReference removeEventMethod;

        /// <summary>
        /// Array of get native array froms.
        /// </summary>
        private MethodReference getNativeArrayFromArray;

        /// <summary>
        /// List of get native array froms.
        /// </summary>
        private MethodReference getNativeArrayFromList;

        /// <summary>
        /// The get new imported extension.
        /// </summary>
        private MethodReference getNewImportedExtension;

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
        /// The imported extension field.
        /// </summary>
        private FieldReference importedExtensionField;

        /// <summary>
        /// Backing field for EnumStrToValueMap.
        /// </summary>
        private FieldReference enumStrToValueMapField;

        /// <summary>
        /// Backing field for Boxed value field.
        /// </summary>
        private FieldReference boxedValueField;

        /// <summary>
        /// Backing field for isNullable field.
        /// </summary>
        private FieldReference isNullableField;

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
        /// Constructor.
        /// </summary>
        /// <param name="clrContext"> Context for the colour. </param>
        public ConverterKnownReferences(ClrContext clrContext)
        {
            this.clrContext = clrContext;
        }

        /// <summary>
        /// Gets the I enumerator.
        /// </summary>
        /// <value>
        /// The i enumerable.
        /// </value>
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
        /// <value>
        /// The i enumerator.
        /// </value>
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
        /// <value>
        /// The dictionary entry.
        /// </value>
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

        public TypeReference GeneratorWrapper
        {
            get
            {
                if (this.generatorWrapperType != null)
                {
                    return this.generatorWrapperType;
                }
                return this.generatorWrapperType = GetTypeReference(
                    ClrKnownReferences.SystemStr,
                    "GeneratorWrapper");
            }
        }

        public TypeReference NativeGenerator
        {
            get
            {
                if(this.nativeGeneratorType != null)
                {
                    return this.nativeGeneratorType;
                }

                return this.nativeGeneratorType = this.GetTypeReference(
                    ClrKnownReferences.SystemStr,
                    "NativeGenerator");
            }
        }
        /// <summary>
        /// Gets the native array.
        /// </summary>
        /// <value>
        /// An Array of natives.
        /// </value>
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
        /// Gets the native array.
        /// </summary>
        /// <value>
        /// An Array of natives.
        /// </value>
        public TypeReference NativeArrayGeneric
        {
            get
            {
                if (this.nativeArrayTypeGeneric == null)
                {
                    this.nativeArrayTypeGeneric = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "NativeArray`1");
                }

                return this.nativeArrayTypeGeneric;
            }
        }

        /// <summary>
        /// Gets the ignore namespace attribute reference.
        /// </summary>
        /// <value>
        /// The ignore namespace attribute.
        /// </value>
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
        /// <value>
        /// The implement attribute.
        /// </value>
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
        /// Gets the extended attribute reference.
        /// </summary>
        /// <value>
        /// The extended attribute.
        /// </value>
        public TypeReference ExtendedAttribute
        {
            get
            {
                if (this.extendedAttribute == null)
                {
                    this.extendedAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ExtendedAttribute");
                }

                return this.extendedAttribute;
            }
        }

        /// <summary>
        /// Gets the intrinsic property attribute reference.
        /// </summary>
        /// <value>
        /// The intrinsic property attribute.
        /// </value>
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
        /// <value>
        /// The intrinsic field attribute.
        /// </value>
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
        /// Gets the intrinsic operator attribute reference.
        /// </summary>
        /// <value>
        /// The intrinsic operator attribute.
        /// </value>
        public TypeReference IntrinsicOperatorAttribute
        {
            get
            {
                if (this.intrinsicOperatorAttribute == null)
                {
                    this.intrinsicOperatorAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "IntrinsicOperatorAttribute");
                }

                return this.intrinsicOperatorAttribute;
            }
        }

        /// <summary>
        /// Gets the non scriptable attribute reference.
        /// </summary>
        /// <value>
        /// The non scriptable attribute.
        /// </value>
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
        /// <value>
        /// The total number of eric value attribute.
        /// </value>
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
        /// <value>
        /// The preserve case attribute.
        /// </value>
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
        /// <value>
        /// The preserve name attribute.
        /// </value>
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
        /// <value>
        /// The script alias attribute.
        /// </value>
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
        /// <value>
        /// The script namespace attribute.
        /// </value>
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
        /// <value>
        /// The script name attribute.
        /// </value>
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
        /// <value>
        /// The script attribute.
        /// </value>
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
        /// <value>
        /// The make static usage attribute.
        /// </value>
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
        /// <value>
        /// The psudo type attribute.
        /// </value>
        public TypeReference ImportedTypeAttribute
        {
            get
            {
                if (this.importedTypeAttribute == null)
                {
                    this.importedTypeAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "ImportedTypeAttribute");
                }

                return this.importedTypeAttribute;
            }
        }

        /// <summary>
        /// Gets the psudo type attribute.
        /// </summary>
        /// <value>
        /// The psudo type attribute.
        /// </value>
        public TypeReference PsudoTypeAttribute
        {
            get
            {
                if (this.psudoTypeAttribute == null)
                {
                    this.psudoTypeAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "PseudoInterfaceTypeAttribute");
                }

                return this.psudoTypeAttribute;
            }
        }

        /// <summary>
        /// Gets the json type attribute.
        /// </summary>
        /// <value>
        /// The json type attribute.
        /// </value>
        public TypeReference JsonTypeAttribute
        {
            get
            {
                if (this.jsonTypeAttribute == null)
                {
                    this.jsonTypeAttribute = this.GetTypeReference(
                        ClrKnownReferences.CompilerServicesStr,
                        "JsonTypeAttribute");
                }

                return this.jsonTypeAttribute;
            }
        }

        /// <summary>
        /// Gets the keep instance usage attribute.
        /// </summary>
        /// <value>
        /// The keep instance usage attribute.
        /// </value>
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
        /// Gets the global methods attribute. This attribute is used to point that all the methods are
        /// in global scope.
        /// </summary>
        /// <value>
        /// The global methods attribute.
        /// </value>
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
        /// <value>
        /// The ignore generic arguments attribute.
        /// </value>
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
        /// <value>
        /// The entry point attribute.
        /// </value>
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
        /// <value>
        /// The array implementation generic.
        /// </value>
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
        /// <value>
        /// The array implementation base.
        /// </value>
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
        /// Gets the list generic.
        /// </summary>
        /// <value>
        /// The list generic.
        /// </value>
        public TypeReference ListGeneric
        {
            get
            {
                if (this.listGeneric == null)
                {
                    this.listGeneric = this.GetTypeReference(
                        ConverterKnownReferences.GenericCollectionsStr,
                        "List`1");
                }

                return this.listGeneric;
            }
        }

        /// <summary>
        /// Gets the get current I enumerator method.
        /// </summary>
        /// <value>
        /// The get enumerator i enumerable method.
        /// </value>
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
        /// <value>
        /// The get current i enumerator method.
        /// </value>
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
        /// <value>
        /// The move next enumerator method.
        /// </value>
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
        /// <value>
        /// The create delegate.
        /// </value>
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
        /// <value>
        /// The create delegate from function.
        /// </value>
        public MethodReference CreateDelegateFromFunction
        {
            get
            {
                if (this.createDelegateFromFunction == null)
                {
                    this.createDelegateFromFunction = this.GetMethodReference(
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
        /// <value>
        /// The static instance create delegate.
        /// </value>
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
        /// <value>
        /// The create generic delegate.
        /// </value>
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
        /// <value>
        /// The create generic instance delegate.
        /// </value>
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
        /// <value>
        /// The delegate combine method.
        /// </value>
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
        /// <value>
        /// The delegate remove method.
        /// </value>
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

        public MethodReference NativeGeneratorCtor
        {
            get
            {
                if (this.nativeGeneratorCtor != null)
                {
                    return this.nativeGeneratorCtor;
                }

                return this.nativeGeneratorCtor = this.GetMethodReference(
                    ".ctor",
                    this.ClrReferences.Void,
                    this.NativeGenerator);
            }
        }

        public MethodReference GeneratorWrapperCtor
        {
            get
            {
                if (this.generatorWrapperCtor != null)
                {
                    return this.generatorWrapperCtor;
                }
                return this.generatorWrapperCtor = this.GetMethodReference(
                    ".ctor",
                    ClrReferences.Void,
                    GeneratorWrapper,
                    ClrReferences.Object);
            }
        }

        /// <summary>
        /// Gets the array impl native array ctor.
        /// </summary>
        /// <value>
        /// The array implementation native array constructor.
        /// </value>
        public MethodReference ArrayImplNativeArrayCtor
        {
            get
            {
                if (this.arrayImplNativeArrayCtor == null)
                {
                    GenericInstanceType nativeArray = new GenericInstanceType(this.NativeArrayGeneric);
                    nativeArray.GenericArguments.Add(new GenericParameter(0, GenericParameterType.Type, this.NativeArray.Module));
                    this.arrayImplNativeArrayCtor = this.GetMethodReference(
                        ".ctor",
                        this.ClrReferences.Void,
                        this.ArrayImplGeneric,
                        nativeArray);
                }

                return this.arrayImplNativeArrayCtor;
            }
        }

        public MethodReference ListNativeArrayCtor
        {
            get
            {
                if (this.listNativeArrayCtor == null)
                {
                    var argType = new GenericInstanceType(this.NativeArrayGeneric.Resolve());
                    argType.GenericArguments.Add(
                        new GenericParameter(0, GenericParameterType.Type, this.ListGeneric.Module));

                    this.listNativeArrayCtor = this.GetMethodReference(
                        ".ctor",
                        this.ClrReferences.Void,
                        this.ListGeneric,
                        argType);
                }

                return this.listNativeArrayCtor;
            }
        }

        /// <summary>
        /// Gets an array of get native array froms.
        /// </summary>
        /// <value>
        /// An Array of get native array froms.
        /// </value>
        public MethodReference GetNativeArrayFromArray
        {
            get
            {
                if (this.getNativeArrayFromArray == null)
                {
                    this.getNativeArrayFromArray = new MethodReference(
                        "GetNativeArray",
                        this.NativeArray,
                        this.NativeArray);

                    this.getNativeArrayFromArray.GenericParameters.Add(new GenericParameter("T", this.getNativeArrayFromArray));
                    ArrayType arrayType = new ArrayType(new GenericParameter(0, GenericParameterType.Method, this.NativeArray.Module));
                    this.getNativeArrayFromArray.Parameters.Add(new ParameterDefinition(arrayType));
                }

                return this.getNativeArrayFromArray;
            }
        }

        /// <summary>
        /// Gets a list of get native array froms.
        /// </summary>
        /// <value>
        /// A List of get native array froms.
        /// </value>
        public MethodReference GetNativeArrayFromList
        {
            get
            {
                if (this.getNativeArrayFromList == null)
                {
                    this.getNativeArrayFromList = new MethodReference(
                        "GetNativeArray",
                        this.NativeArray,
                        this.NativeArray);

                    this.getNativeArrayFromList.GenericParameters.Add(new GenericParameter("T", this.getNativeArrayFromList));
                    GenericInstanceType listType = new GenericInstanceType(this.ListGeneric);
                    listType.GenericArguments.Add(new GenericParameter(0, GenericParameterType.Method, this.NativeArray.Module));
                    this.getNativeArrayFromList.Parameters.Add(new ParameterDefinition(listType));
                }

                return this.getNativeArrayFromList;
            }
        }

        /// <summary>
        /// Gets the array impl native array ctor.
        /// </summary>
        /// <value>
        /// The array implementation length constructor.
        /// </value>
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
        /// <value>
        /// The array clone method.
        /// </value>
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
        /// <value>
        /// The array contains method.
        /// </value>
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
        /// <value>
        /// The array index of 1 method.
        /// </value>
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
        /// <value>
        /// The array index of 2 method.
        /// </value>
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
        /// <value>
        /// The array reverse method.
        /// </value>
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
        /// <value>
        /// The array get enumerator method.
        /// </value>
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
        /// <value>
        /// The array length getter.
        /// </value>
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
        /// <value>
        /// The array length property.
        /// </value>
        public PropertyReference ArrayLengthProperty
        {
            get
            {
                return this.ArrayLengthGetter.Resolve().GetPropertyDefinition();
            }
        }

        /// <summary>
        /// Gets the array get value.
        /// </summary>
        /// <value>
        /// The array get value.
        /// </value>
        public MethodReference ArrayGetValue
        {
            get
            {
                if (this.arrayGetValue == null)
                {
                    this.arrayGetValue = this.GetMethodReference(
                        "GetValue",
                        this.ClrReferences.Object,
                        this.ClrReferences.SystemArray,
                        this.ClrReferences.Int32);
                }

                return this.arrayGetValue;
            }
        }

        /// <summary>
        /// Gets the array set value.
        /// </summary>
        /// <value>
        /// The array set value.
        /// </value>
        public MethodReference ArraySetValue
        {
            get
            {
                if (this.arraySetValue == null)
                {
                    this.arraySetValue = this.GetMethodReference(
                        "SetValue",
                        this.ClrReferences.Void,
                        this.ClrReferences.SystemArray,
                        this.ClrReferences.Int32,
                        this.ClrReferences.Object);
                }

                return this.arraySetValue;
            }
        }

        /// <summary>
        /// Gets the array impl clone method.
        /// </summary>
        /// <value>
        /// The array implementation clone method.
        /// </value>
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
        /// <value>
        /// The array implementation contains method.
        /// </value>
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
        /// <value>
        /// The array implementation index of 1 method.
        /// </value>
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
        /// <value>
        /// The array implementation index of 2 method.
        /// </value>
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
        /// <value>
        /// The array implementation reverse method.
        /// </value>
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
        /// <value>
        /// The array implementation get enumerator method.
        /// </value>
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
        /// <value>
        /// The array implementation length getter.
        /// </value>
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
        /// <value>
        /// The array implementation length property.
        /// </value>
        public PropertyReference ArrayImplLengthProperty
        {
            get { return this.ArrayImplLengthGetter.Resolve().GetPropertyDefinition(); }
        }

        /// <summary>
        /// Gets the array get value.
        /// </summary>
        /// <value>
        /// The array get value.
        /// </value>
        public MethodReference ArrayImplGetValue
        {
            get
            {
                if (this.arrayImplGetValue == null)
                {
                    this.arrayImplGetValue = this.GetMethodReference(
                        "GetValue",
                        this.ClrReferences.Object,
                        this.ArrayImplBase,
                        this.ClrReferences.Int32);
                }

                return this.arrayImplGetValue;
            }
        }

        /// <summary>
        /// Gets the array set value.
        /// </summary>
        /// <value>
        /// The array set value.
        /// </value>
        public MethodReference ArrayImplSetValue
        {
            get
            {
                if (this.arrayImplSetValue == null)
                {
                    this.arrayImplSetValue = this.GetMethodReference(
                        "SetValue",
                        this.ClrReferences.Void,
                        this.ArrayImplBase,
                        this.ClrReferences.Int32,
                        this.ClrReferences.Object);
                }

                return this.arrayImplSetValue;
            }
        }

        /// <summary>
        /// Gets the box method.
        /// </summary>
        /// <value>
        /// The box method.
        /// </value>
        public MethodReference BoxMethod
        {
            get
            {
                if (this.boxMethod == null)
                {
                    this.boxMethod = this.GetMethodReference(
                        "BoxTypeInstance",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.boxMethod;
            }
        }

        /// <summary>
        /// Gets the type nullable box method.
        /// </summary>
        /// <value>
        /// The type nullable box method.
        /// </value>
        public MethodReference TypeNullableBoxMethod
        {
            get
            {
                if (this.typeNullableBoxMethod == null)
                {
                    this.typeNullableBoxMethod = this.GetMethodReference(
                        "NullableBox",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.typeNullableBoxMethod;
            }
        }

        /// <summary>
        /// Gets the unbox method.
        /// </summary>
        /// <value>
        /// The unbox method.
        /// </value>
        public MethodReference UnboxMethod
        {
            get
            {
                if (this.unboxMethod == null)
                {
                    this.unboxMethod = this.GetMethodReference(
                        "UnBoxTypeInstance",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Object);
                }

                return this.unboxMethod;
            }
        }

        /// <summary>
        /// Gets the get default method.
        /// </summary>
        /// <value>
        /// The get default method.
        /// </value>
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
        /// Gets the get default method.
        /// </summary>
        /// <value>
        /// The get default method.
        /// </value>
        public MethodReference GetDefaultMethodStatic
        {
            get
            {
                if (this.getDefaultValueMethodStatic == null)
                {
                    this.getDefaultValueMethodStatic = this.GetMethodReference(
                        "GetDefaultValueStatic",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.TypeType);
                }

                return this.getDefaultValueMethodStatic;
            }
        }

        /// <summary>
        /// Gets the get default constructor method.
        /// </summary>
        /// <value>
        /// The get default constructor method.
        /// </value>
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
        /// Gets the get default constructor method.
        /// </summary>
        /// <value>
        /// The get default constructor method.
        /// </value>
        public MethodReference GetGetDefaultConstructorMethod
        {
            get
            {
                if (this.typeGetDefaultConstructor == null)
                {
                    this.typeGetDefaultConstructor = this.GetMethodReference(
                        "GetDefaultConstructor",
                        this.ClrReferences.Object,
                        this.ClrReferences.TypeType,
                        this.ClrReferences.TypeType);
                }

                return this.typeGetDefaultConstructor;
            }
        }

        /// <summary>
        /// Gets the get new imported extension method.
        /// </summary>
        /// <value>
        /// The get new imported extension method.
        /// </value>
        public MethodReference GetNewImportedExtensionMethod
        {
            get
            {
                if (this.getNewImportedExtension == null)
                {
                    this.getNewImportedExtension = this.GetMethodReference(
                        "GetNewImportedExtension",
                        this.ClrReferences.Object,
                        this.ClrReferences.Object);
                }

                return this.getNewImportedExtension;
            }
        }

        /// <summary>
        /// Gets the nullable box method.
        /// </summary>
        /// <value>
        /// The nullable box method.
        /// </value>
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
        /// <value>
        /// The nullable unbox method.
        /// </value>
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
        /// <value>
        /// The nullable value property.
        /// </value>
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
        /// <param name="elementType"> Type of the array element. </param>
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
        /// <value>
        /// The register inteface method.
        /// </value>
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
        /// <value>
        /// The register enum method.
        /// </value>
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
        /// <value>
        /// The register reference type method.
        /// </value>
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
        /// <value>
        /// The register structure type method.
        /// </value>
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
        /// <value>
        /// to string method.
        /// </value>
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
        /// <value>
        /// The enum to string method.
        /// </value>
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
                        this.ClrReferences.TypeType,
                        this.ClrReferences.Int32);
                }

                return this.enumToStringMethod;
            }
        }

        /// <summary>
        /// Gets the array item getter.
        /// </summary>
        /// <value>
        /// The array item getter.
        /// </value>
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
        /// <value>
        /// The is instance of method.
        /// </value>
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
        /// <value>
        /// The cast type method.
        /// </value>
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
        /// <value>
        /// as type method.
        /// </value>
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
        /// <value>
        /// The enum string to value map field.
        /// </value>
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
        /// <value>
        /// The boxed value field.
        /// </value>
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
        /// Gets the boxed value field.
        /// </summary>
        /// <value>
        /// The boxed value field.
        /// </value>
        public FieldReference IsNullableField
        {
            get
            {
                if (this.isNullableField == null)
                {
                    this.isNullableField = new FieldReference(
                        "IsNullable",
                        this.ClrReferences.Boolean,
                        this.ClrReferences.TypeType);
                }

                return this.isNullableField;
            }
        }

        /// <summary>
        /// Gets the prototype field.
        /// </summary>
        /// <value>
        /// The prototype field.
        /// </value>
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
        /// <value>
        /// The constructor field.
        /// </value>
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
        /// <value>
        /// The type identifier field.
        /// </value>
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
        /// <value>
        /// The type name field.
        /// </value>
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
        /// <value>
        /// The base type field.
        /// </value>
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
        /// <value>
        /// The generic closure field.
        /// </value>
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
        /// <value>
        /// The generic parameters field.
        /// </value>
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
        /// <value>
        /// The dictionary entry key.
        /// </value>
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
        /// <value>
        /// The dictionary entry value.
        /// </value>
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
        /// <value>
        /// The array accessor.
        /// </value>
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
        /// Gets the event binder.
        /// </summary>
        /// <value>
        /// The event binder.
        /// </value>
        public TypeReference EventBinder
        {
            get
            {
                if (this.eventBinder == null)
                {
                    this.eventBinder = this.GetTypeReference(
                        ClrKnownReferences.SystemStr,
                        "EventBinder");
                }

                return this.eventBinder;
            }
        }

        /// <summary>
        /// Gets the add event method.
        /// </summary>
        /// <value>
        /// The add event method.
        /// </value>
        public MethodReference AddEventMethod
        {
            get
            {
                if (this.addEventMethod == null)
                {
                    foreach (var method in this.EventBinder.Resolve().Methods)
                    {
                        if (method.Name == "AddEvent"
                            && method.IsStatic)
                        {
                            this.addEventMethod = method;
                            break;
                        }
                    }
                }

                return this.addEventMethod;
            }
        }

        /// <summary>
        /// Gets the remove event method.
        /// </summary>
        /// <value>
        /// The remove event method.
        /// </value>
        public MethodReference RemoveEventMethod
        {
            get
            {
                if (this.removeEventMethod == null)
                {
                    foreach (var method in this.EventBinder.Resolve().Methods)
                    {
                        if (method.Name == "RemoveEvent"
                            && method.IsStatic)
                        {
                            this.removeEventMethod = method;
                            break;
                        }
                    }
                }

                return this.removeEventMethod;
            }
        }

        /// <summary>
        /// Gets the imported extension field.
        /// </summary>
        /// <value>
        /// The imported extension field.
        /// </value>
        public FieldReference ImportedExtensionField
        {
            get
            {
                if (this.importedExtensionField == null)
                {
                    this.importedExtensionField = new FieldReference(
                        "importedExtension",
                        this.ClrReferences.Object,
                        this.ClrReferences.Object);
                }

                return this.importedExtensionField;
            }
        }

        /// <summary>
        /// Gets the array int arg ctor.
        /// </summary>
        /// <param name="arrayElementType"> Type of the array element. </param>
        /// <returns>
        /// Generic ArrayG&lt;arrayElementType&gt;
        /// </returns>
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
        /// <param name="arrayElementType"> Type of the array element. </param>
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
        /// Gets the array native array arg ctor.
        /// </summary>
        /// <param name="listElementType"> Type of the list element. </param>
        /// <returns>
        /// Method reference with GenericInstanceType DeclaringType.
        /// </returns>
        public MethodReference GetListNativeArrayArgCtor(TypeReference listElementType)
        {
            MethodDefinition methodDefinition = this.ListNativeArrayCtor.Resolve();
            GenericInstanceType instanceType = new GenericInstanceType(methodDefinition.DeclaringType);
            instanceType.GenericArguments.Add(listElementType);
            MethodReference ctorMethod = new MethodReference(
                methodDefinition.Name,
                methodDefinition.ReturnType,
                instanceType);
            ctorMethod.Parameters.Add(methodDefinition.Parameters[0]);
            ctorMethod.HasThis = true;

            return ctorMethod;
        }

        /// <summary>
        /// Gets a native array from array method.
        /// </summary>
        /// <param name="arrayElementType"> Type of the array element. </param>
        /// <returns>
        /// The native array from array method.
        /// </returns>
        public MethodReference GetNativeArrayFromArrayMethod(TypeReference arrayElementType)
        {
            GenericInstanceMethod method = new GenericInstanceMethod(this.GetNativeArrayFromArray);
            method.GenericArguments.Add(arrayElementType);
            return method;
        }

        /// <summary>
        /// Gets a native array from list method.
        /// </summary>
        /// <param name="listElementType"> Type of the list element. </param>
        /// <returns>
        /// The native array from list method.
        /// </returns>
        public MethodReference GetNativeArrayFromListMethod(TypeReference listElementType)
        {
            GenericInstanceMethod method = new GenericInstanceMethod(this.GetNativeArrayFromList);
            method.GenericArguments.Add(listElementType);
            return method;
        }

        /// <summary>
        /// Gets the array accessor.
        /// </summary>
        /// <param name="arrayElementType"> Type of the array element. </param>
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
        /// Fix array type.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        /// .
        /// </returns>
        public TypeReference FixArrayType(TypeReference type)
        {
            TypeReference fixedType = null;
            if (type.IsArray)
            {
                ArrayType arrayType = type as ArrayType;
                var genericInstanceType = new GenericInstanceType(this.ArrayImplGeneric);
                TypeReference tmpType = this.FixArrayType(arrayType.ElementType);
                genericInstanceType.GenericArguments.Add(tmpType ?? arrayType.ElementType);

                fixedType = genericInstanceType;
            }
            else if (type.IsGenericInstance)
            {
                GenericInstanceType genericInstanceType = type as GenericInstanceType;
                var genericArgs = genericInstanceType.GenericArguments;
                bool changed = false;
                for (int iGeneric = 0; iGeneric < genericArgs.Count; iGeneric++)
                {
                    TypeReference tmpType;
                    if ((tmpType = this.FixArrayType(genericArgs[iGeneric])) != null)
                    {
                        if (!changed)
                        {
                            genericInstanceType = new GenericInstanceType(genericInstanceType.ElementType);
                            for (int j = 0; j < iGeneric; j++)
                            {
                                genericInstanceType.GenericArguments.Add(genericArgs[j]);
                            }
                        }

                        changed = true;
                    }

                    if (changed)
                    {
                        genericInstanceType.GenericArguments.Add(tmpType ?? genericArgs[iGeneric]);
                    }
                }

                if (changed)
                {
                    fixedType = genericInstanceType;
                }
            }

            return fixedType;
        }

        /// <summary>
        /// Gets the CLR references.
        /// </summary>
        /// <value>
        /// The colour references.
        /// </value>
        private ClrKnownReferences ClrReferences
        { get { return this.clrContext.KnownReferences; } }

        /// <summary>
        /// Gets the MS corlib module.
        /// </summary>
        /// <value>
        /// The milliseconds corlib module.
        /// </value>
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
        /// <param name="typeNamespace"> The type namespace. </param>
        /// <param name="typeName">      Name of the type. </param>
        /// <returns>
        /// Type reference inside mscorlib.
        /// </returns>
        private TypeReference GetTypeReference(
            string typeNamespace,
            string typeName)
        {
            return new TypeReference(
                typeNamespace,
                typeName,
                this.MSCorlibModule,
                this.MSCorlibModule.TypeSystem.CoreLibrary).Resolve();
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <param name="methodName">    Name of the method. </param>
        /// <param name="returnType">    Type of the return. </param>
        /// <param name="declaringType"> Type of the declaring. </param>
        /// <param name="arguments">     The arguments. </param>
        /// <returns>
        /// MethodReference.
        /// </returns>
        public MethodReference GetMethodReference(
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