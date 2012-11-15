namespace Cs2JsC.CLR.Test
{
    using System;
    using Mono.Cecil;
    using System.IO;

    public static class TestAssemblyLoader
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        public static ClrContext Context
        {
            get;
            private set;
        }

        public static DllBuilder DllBuilder
        {
            get;
            private set;
        }

        public static ClrContext McsContext
        {
            get { return TestAssemblyLoader.DllBuilder.ClrContext; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loaded; otherwise, <c>false</c>.
        /// </value>
        public static bool IsLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads the assemblies.
        /// </summary>
        public static void LoadAssemblies()
        {
            if (TestAssemblyLoader.IsLoaded)
            {
                return;
            }

            TestAssemblyLoader.IsLoaded = true;

            TestAssemblyLoader.Context = new ClrContext();
            TestAssemblyLoader.Context.LoadAssembly(System.IO.Path.GetFullPath(@"mscorlib.dll"));
            TestAssemblyLoader.Context.LoadAssembly(System.IO.Path.GetFullPath(@"realScript.dll"));
            TestAssemblyLoader.Context.LoadAssembly(System.IO.Path.GetFullPath(@"realScript.Debug.dll"));

            TestAssemblyLoader.DllBuilder = new DllBuilder();
            TestAssemblyLoader.LoadMcsCorlibAssemblies();
            TestAssemblyLoader.LoadMcsAssemblies();
        }

        /// <summary>
        /// Gets the method definition.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <returns>Method definition</returns>
        public static MethodDefinition GetMethodDefinition(
            string className,
            string methodName,
            bool isDebug,
            bool isMcs = false)
        {
            TypeReference typeReference = TestAssemblyLoader.GetTypeReference(
                className,
                isDebug,
                isMcs);

            TypeDefinition typeDefinition = typeReference.Resolve();

            return TestAssemblyLoader.GetMethodDefinition(
                typeDefinition,
                methodName);
        }

        /// <summary>
        /// Gets the method definition.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <returns>
        /// Method definition
        /// </returns>
        public static MethodDefinition GetMethodDefinition(
            TypeDefinition typeDefinition,
            string methodName)
        {
            for (int methodIndex = 0; methodIndex < typeDefinition.Methods.Count; methodIndex++)
            {
                if (typeDefinition.Methods[methodIndex].Name == methodName)
                {
                    return typeDefinition.Methods[methodIndex];
                }
            }

            throw new ArgumentException(
                string.Format(
                    "Method name {0} not found in class {1}",
                    methodName,
                    typeDefinition.Name));
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <returns>Method Reference</returns>
        public static MethodReference GetMethodReference(
            string className,
            string methodName,
            bool isDebug,
            bool isMcs = false)
        {
            return TestAssemblyLoader.GetMethodDefinition(
                    className,
                    methodName,
                    isDebug,
                    isMcs);
        }

        /// <summary>
        /// Gets the type reference.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <returns>Type Reference.</returns>
        public static TypeReference GetTypeReference(
            string className,
            bool isDebug,
            bool isMcs = false)
        {
            if (className.StartsWith("System"))
            {
                return TestAssemblyLoader.GetTypeReference(
                    isMcs ? TestAssemblyLoader.McsContext : TestAssemblyLoader.Context,
                    className,
                    "mscorlib");
            }
            else
            {
                return TestAssemblyLoader.GetTypeReference(
                    isMcs ? TestAssemblyLoader.McsContext : TestAssemblyLoader.Context,
                    "RealScript." + className,
                    isDebug ? "RealScript.Debug" : "RealScript");
            }
        }

        private static TypeReference GetTypeReference(
            ClrContext clrContext,
            string className,
            string assemblyName)
        {
            string assemblyExtension = System.IO.Path.GetExtension(assemblyName);
            if (!string.IsNullOrEmpty(assemblyExtension))
            {
                assemblyExtension = assemblyExtension.ToLowerInvariant();
                if (assemblyExtension == ".dll"
                    || assemblyExtension == ".exe")
                {
                    assemblyName = System.IO.Path.GetFileNameWithoutExtension(assemblyName);
                }
            }

            return clrContext.GetTypeDefinition(
                    Tuple.Create(assemblyName, className));
        }

        private static void LoadMcsCorlibAssemblies()
        {
            string[] mscorlibFileNames = new string[]

            #region fileNames

            {
                @"Activator.cs",
                @"AssemblyInfo.cs",
                @"Action.cs",
                @"Arguments.cs",
                @"Array.cs",
                @"Attribute.cs",
                @"AttributeTargets.cs",
                @"AttributeUsageAttribute.cs",
                @"Boolean.cs",
                @"Byte.cs",
                @"Callback.cs",
                @"Char.cs",
                @"CLSCompliantAttribute.cs",
                @"CodeDom\Compiler\GeneratedCodeAttribute.cs",
                @"Collections\ArrayAggregator.cs",
                @"Collections\ArrayCallback.cs",
                @"Collections\ArrayFilterCallback.cs",
                @"Collections\ArrayGrouping.cs",
                @"Collections\ArrayItemAggregator.cs",
                @"Collections\ArrayItemCallback.cs",
                @"Collections\ArrayItemFilterCallback.cs",
                @"Collections\ArrayItemKeyGenerator.cs",
                @"Collections\ArrayItemMapCallback.cs",
                @"Collections\ArrayList.cs",
                @"Collections\ArrayMapCallback.cs",
                @"Collections\Dictionary.cs",
                @"Collections\DictionaryEntry.cs",
                @"Collections\Generic\ICollection.cs",
                @"Collections\Generic\IDictionary.cs",
                @"Collections\Generic\IEnumerable.cs",
                @"Collections\Generic\IList.cs",
                @"Collections\Generic\INumberDictionary.cs",
                @"Collections\Generic\IStringDictionary.cs",
                @"Collections\Generic\KeyValuePair.cs",
                @"Collections\Generic\List.cs",
                @"Collections\Generic\NumberDictionary.cs",
                @"Collections\Generic\StringDictionary.cs",
                @"Collections\IEnumerable.cs",
                @"Collections\IEnumerator.cs",
                @"Collections\Queue.cs",
                @"Collections\Stack.cs",
                @"CompareCallback.cs",
                @"ComponentModel\BrowsableAttribute.cs",
                @"ComponentModel\CancelEventArgs.cs",
                @"ComponentModel\CancelEventHandler.cs",
                @"ComponentModel\CollectionChangedAction.cs",
                @"ComponentModel\CollectionChangedEventArgs.cs",
                @"ComponentModel\CollectionChangedEventHandler.cs",
                @"ComponentModel\EditorBrowsableAttribute.cs",
                @"ComponentModel\EditorBrowsableState.cs",
                @"ComponentModel\INotifyCollectionChanged.cs",
                @"ComponentModel\INotifyPropertyChanged.cs",
                @"ComponentModel\PropertyChangedEventArgs.cs",
                @"ComponentModel\PropertyChangedEventHandler.cs",
                @"ContextualCallback.cs",
                @"CultureInfo.cs",
                @"DateFormatInfo.cs",
                @"DateTime.cs",
                @"Decimal.cs",
                @"Delegate.cs",
                @"Diagnostics\CodeAnalysis\SuppressMessageAttribute.cs",
                @"Diagnostics\ConditionalAttribute.cs",
                @"Diagnostics\Debug.cs",
                @"Double.cs",
                @"Enum.cs",
                @"EventArgs.cs",
                @"EventBinder.cs",
                @"EventHandler.cs",
                @"Exception.cs",
                @"FlagsAttribute.cs",
                @"Func.cs",
                @"Function.cs",
                @"IDisposable.cs",
                @"IEquatable.cs",
                @"Int16.cs",
                @"Int32.cs",
                @"Int64.cs",
                @"InternalArrayImpl.cs",
                @"IntPtr.cs",
                @"JavaScriptStaticHelpers.cs",
                @"MarshalByRefObject.cs",
                @"Math.cs",
                @"MulticastDelegate.cs",
                @"Nullable.cs",
                @"NativeArray.cs",
                @"Net\ReadyState.cs",
                @"Net\XmlHttpRequest.cs",
                @"Number.cs",
                @"NumberFormatInfo.cs",
                @"Object.cs",
                @"ObsoleteAttribute.cs",
                @"ParamArrayAttribute.cs",
                @"Record.cs",
                @"Reflection\AssemblyCompanyAttribute.cs",
                @"Reflection\AssemblyConfigurationAttribute.cs",
                @"Reflection\AssemblyCopyrightAttribute.cs",
                @"Reflection\AssemblyCultureAttribute.cs",
                @"Reflection\AssemblyDelaySignAttribute.cs",
                @"Reflection\AssemblyDescriptionAttribute.cs",
                @"Reflection\AssemblyFileVersionAttribute.cs",
                @"Reflection\AssemblyInformationalVersionAttribute.cs",
                @"Reflection\AssemblyKeyFileAttribute.cs",
                @"Reflection\AssemblyProductAttribute.cs",
                @"Reflection\AssemblyTitleAttribute.cs",
                @"Reflection\AssemblyTrademarkAttribute.cs",
                @"Reflection\AssemblyVersionAttribute.cs",
                @"Reflection\DefaultMemberAttribute.cs",
                @"RegularExpression.cs",
                @"RuntimeFieldHandle.cs",
                @"RuntimeTypeHandle.cs",
                @"Runtime\CompilerServices\AlternateSignatureAttribute.cs",
                @"Runtime\CompilerServices\AttachedPropertyAttribute.cs",
                @"Runtime\CompilerServices\CompilerGeneratedAttribute.cs",
                @"Runtime\CompilerServices\ExtensionAttribute.cs",
                @"Runtime\CompilerServices\EntryPointAttribute.cs",
                @"Runtime\CompilerServices\IgnoreGenericArgumentsAttribute.cs",
                @"Runtime\CompilerServices\ImplementAttribute.cs",
                @"Runtime\CompilerServices\IntrinsicFieldAttribute.cs",
                @"Runtime\CompilerServices\GlobalMethodsAttribute.cs",
                @"Runtime\CompilerServices\IgnoreNamespaceAttribute.cs",
                @"Runtime\CompilerServices\ExtendedAttribute.cs",
                @"Runtime\CompilerServices\IntrinsicPropertyAttribute.cs",
                @"Runtime\CompilerServices\KeepInstanceUsage.cs",
                @"Runtime\CompilerServices\MakeStaticUsage.cs",
                @"Runtime\CompilerServices\MixinAttribute.cs",
                @"Runtime\CompilerServices\NamedValuesAttribute.cs",
                @"Runtime\CompilerServices\NonScriptableAttribute.cs",
                @"Runtime\CompilerServices\NumericValuesAttribute.cs",
                @"Runtime\CompilerServices\PreserveCaseAttribute.cs",
                @"Runtime\CompilerServices\PreserveNameAttribute.cs",
                @"Runtime\CompilerServices\PsudoTypeAttribute.cs",
                @"Runtime\CompilerServices\RequiresAssemblyAttribute.cs",
                @"Runtime\CompilerServices\ResourcesAttribute.cs",
                @"Runtime\CompilerServices\RuntimeHelpers.cs",
                @"Runtime\CompilerServices\ScriptAliasAttribute.cs",
                @"Runtime\CompilerServices\ScriptAssemblyAttribute.cs",
                @"Runtime\CompilerServices\ScriptAttribute.cs",
                @"Runtime\CompilerServices\ScriptNameAttribute.cs",
                @"Runtime\CompilerServices\ScriptNamespaceAttribute.cs",
                @"Runtime\CompilerServices\ScriptQualifierAttribute.cs",
                @"Runtime\CompilerServices\ScriptSkipAttribute.cs",
                @"Runtime\CompilerServices\UsedAttribure.cs",
                @"Runtime\InteropServices\OutAttribute.cs",
                @"Runtime\Versioning\TargetFrameworkAttribute.cs",
                @"SByte.cs",
                @"Script.cs",
                @"Serialization\Json.cs",
                @"Serialization\JsonParseCallback.cs",
                @"Serialization\JsonStringifyCallback.cs",
                @"Single.cs",
                @"String.cs",
                @"StringBuilder.cs",
                @"StringReplaceCallback.cs",
                @"Testing\Assert.cs",
                @"Testing\TestClass.cs",
                @"Testing\TestEngine.cs",
                @"Threading\Interlocked.cs",
                @"Type.cs",
                @"UInt16.cs",
                @"UInt32.cs",
                @"UInt64.cs",
                @"UIntPtr.cs",
                @"ValueType.cs",
                @"Void.cs",
                @"Xml\XmlAttribute.cs",
                @"Xml\XmlDocument.cs",
                @"Xml\XmlDocumentParser.cs",
                @"Xml\XmlNamedNodeMap.cs",
                @"Xml\XmlNode.cs",
                @"Xml\XmlNodeList.cs",
                @"Xml\XmlNodeType.cs",
                @"Xml\XmlText.cs"
            };

            #endregion fileNames

            TestAssemblyLoader.DllBuilder.Build(
                "mscorlib.dll",
                Path.GetFullPath(@"..\..\..\..\Sources\mscorlib"),
                mscorlibFileNames,
                null,
                false,
                "mscorlibKey.snk");
        }

        private static void LoadMcsAssemblies()
        {
            string[] references = new string[]
            {
                @"mscorlib.dll"
            };

            string[] fileNames = new string[]
            {
                @"DelegateBlocks.cs",
                @"DupInstructionBlocks.cs",
                @"ExceptionHandlerSamples.cs",
                @"ForLoopBlocks.cs",
                @"FuncRegressions.cs",
                @"GenericCollections.cs",
                @"GenericSamples.cs",
                @"IfBlocks.cs",
                @"BasicBlockTestFunctions.cs",
                @"BasicStatements.cs",
                @"InlineComplexStatements.cs",
                @"JsScriptImport.cs",
                @"LoopTests.cs",
                @"MathAlgorithms.cs",
                @"NumberOperations.cs",
                @"ScriptSharpCompat.cs",
                @"SimpleInterfaceTest.cs",
                @"SimpleTypes.cs",
                @"StructClass.cs",
                @"SwitchTest.cs",
                @"TestArithmetics.cs",
                @"TestControlFlow.cs",
                @"TestDelegates.cs",
                @"TestGeneric.cs",
                @"TestInheritence.cs",
                @"TestInitializers.cs",
                @"TestPsudoType.cs",
                @"TestReferenceClass.cs",
                @"TestCompilerGeneratedStuff.cs",
                @"WhileLoopBlocks.cs",
                @"YieldReturnTests.cs",
                @"Properties\AssemblyInfo.cs",
                @"Class1.cs",
                @"ConstructorTests.cs",
                @"NullableTests.cs"
            };

            TestAssemblyLoader.DllBuilder.Build(
                "RealScript.dll",
                Path.GetFullPath(@"..\..\..\RealScript"),
                fileNames,
                references,
                false);

            TestAssemblyLoader.DllBuilder.Build(
                "RealScript.Debug.dll",
                Path.GetFullPath(@"..\..\..\RealScript"),
                fileNames,
                references,
                true);
        }
    }
}