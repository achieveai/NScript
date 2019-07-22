namespace NScript.Csc.Lib.Test
{
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class TestResources
    {
        public const string nscriptGitPath = @"C:\repos\cs2jsc";
        public static readonly Dictionary<string, (string outName, string[] files, string directory, string[] refs, string keyFile, bool isDebug)>
            sources = new Dictionary<string, (string outName, string[] files, string directory, string[] refs, string keyFile, bool isDebug)>()
            {
                ["mscorlib"] = (
                    "mscorlib.dll",
                    new string[] {
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
                        @"Collections\Generic\Dictionary.cs",
                        @"Collections\Generic\EqualityComparer.cs",
                        @"Collections\Generic\ICollection.cs",
                        @"Collections\Generic\IDictionary.cs",
                        @"Collections\Generic\IEnumerable.cs",
                        @"Collections\Generic\IEqualityComparer.cs",
                        @"Collections\Generic\IList.cs",
                        @"Collections\Generic\INumberDictionary.cs",
                        @"Collections\Generic\IReadOnlyCollection.cs",
                        @"Collections\Generic\IReadOnlyList.cs",
                        @"Collections\Generic\IStringDictionary.cs",
                        @"Collections\Generic\KeyValuePair.cs",
                        @"Collections\Generic\List.cs",
                        @"Collections\Generic\NumberDictionary.cs",
                        @"Collections\Generic\Queue.cs",
                        @"Collections\Generic\Stack.cs",
                        @"Collections\Generic\StringDictionary.cs",
                        @"Collections\ICollection.cs",
                        @"Collections\IEnumerable.cs",
                        @"Collections\IEnumerator.cs",
                        @"Collections\IEqualityComparer.cs",
                        @"Collections\IList.cs",
                        @"Collections\ObjectModel\ReadOnlyCollection.cs",
                        @"Collections\Queue.cs",
                        @"Collections\Stack.cs",
                        @"CompareCallback.cs",
                        @"ComponentModel\BrowsableAttribute.cs",
                        @"ComponentModel\EditorBrowsableAttribute.cs",
                        @"ComponentModel\EditorBrowsableState.cs",
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
                        @"ErrorEvent.cs",
                        @"Event.cs",
                        @"EventArgs.cs",
                        @"EventBinder.cs",
                        @"EventHandler.cs",
                        @"EventTarget.cs",
                        @"Exception.cs",
                        @"FlagsAttribute.cs",
                        @"Func.cs",
                        @"Function.cs",
                        @"Guid.cs",
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
                        @"NativeArray.cs",
                        @"Nullable.cs",
                        @"Number.cs",
                        @"NumberFormatInfo.cs",
                        @"Object.cs",
                        @"ObsoleteAttribute.cs",
                        @"ParamArrayAttribute.cs",
                        @"Promise.cs",
                        @"Record.cs",
                        @"Reflection\Assembly.cs",
                        @"Reflection\AssemblyCompanyAttribute.cs",
                        @"Reflection\AssemblyConfigurationAttribute.cs",
                        @"Reflection\AssemblyContentType.cs",
                        @"Reflection\AssemblyCopyrightAttribute.cs",
                        @"Reflection\AssemblyCultureAttribute.cs",
                        @"Reflection\AssemblyDelaySignAttribute.cs",
                        @"Reflection\AssemblyDescriptionAttribute.cs",
                        @"Reflection\AssemblyFileVersionAttribute.cs",
                        @"Reflection\AssemblyHashAlgorithm.cs",
                        @"Reflection\AssemblyInformationalVersionAttribute.cs",
                        @"Reflection\AssemblyKeyFileAttribute.cs",
                        @"Reflection\AssemblyName.cs",
                        @"Reflection\AssemblyNameFlags.cs",
                        @"Reflection\AssemblyProductAttribute.cs",
                        @"Reflection\AssemblyTitleAttribute.cs",
                        @"Reflection\AssemblyTrademarkAttribute.cs",
                        @"Reflection\AssemblyVersionAttribute.cs",
                        @"Reflection\AssemblyVersionCompatibility.cs",
                        @"Reflection\Binder.cs",
                        @"Reflection\BindingFlags.cs",
                        @"Reflection\CallingConventions.cs",
                        @"Reflection\ConstructorInfo.cs",
                        @"Reflection\CustomAttributeData.cs",
                        @"Reflection\CustomAttributeNameArgument.cs",
                        @"Reflection\CustomAttributeTypedArgument.cs",
                        @"Reflection\DefaultMemberAttribute.cs",
                        @"Reflection\EventAttributes.cs",
                        @"Reflection\EventInfo.cs",
                        @"Reflection\ExceptionHandlingClause.cs",
                        @"Reflection\FieldAttribute.cs",
                        @"Reflection\FieldInfo.cs",
                        @"Reflection\ICustomAttributeProvider.cs",
                        @"Reflection\IReflectableType.cs",
                        @"Reflection\LocalVariableInfo.cs",
                        @"Reflection\MemberInfo.cs",
                        @"Reflection\MemberTypes.cs",
                        @"Reflection\MethodAttributes.cs",
                        @"Reflection\MethodBase.cs",
                        @"Reflection\MethodBody.cs",
                        @"Reflection\MethodImplAttributes.cs",
                        @"Reflection\MethodInfo.cs",
                        @"Reflection\Module.cs",
                        @"Reflection\ModuleHandle.cs",
                        @"Reflection\ParameterAttributes.cs",
                        @"Reflection\ParameterInfo.cs",
                        @"Reflection\ParameterModifier.cs",
                        @"Reflection\ProcessorArchitecture.cs",
                        @"Reflection\PropertyAttributes.cs",
                        @"Reflection\PropertyInfo.cs",
                        @"Reflection\RuntimeMethodHandle.cs",
                        @"Reflection\RuntimeModule.cs",
                        @"Reflection\StrongNameKeyPair.cs",
                        @"Reflection\TypedReference.cs",
                        @"Reflection\TypeInfo.cs",
                        @"Reflection\Version.cs",
                        @"RegularExpression.cs",
                        @"RuntimeFieldHandle.cs",
                        @"RuntimeTypeHandle.cs",
                        @"Runtime\CompilerServices\AlternateSignatureAttribute.cs",
                        @"Runtime\CompilerServices\AttachedPropertyAttribute.cs",
                        @"Runtime\CompilerServices\CompilerGeneratedAttribute.cs",
                        @"Runtime\CompilerServices\EntryPointAttribute.cs",
                        @"Runtime\CompilerServices\ExtensionAttribute.cs",
                        @"Runtime\CompilerServices\IgnoreGenericArgumentsAttribute.cs",
                        @"Runtime\CompilerServices\ImplementAttribute.cs",
                        @"Runtime\CompilerServices\IntrinsicFieldAttribute.cs",
                        @"Runtime\CompilerServices\GlobalMethodsAttribute.cs",
                        @"Runtime\CompilerServices\IgnoreNamespaceAttribute.cs",
                        @"Runtime\CompilerServices\ExtendedAttribute.cs",
                        @"Runtime\CompilerServices\IntrinsicOperator.cs",
                        @"Runtime\CompilerServices\IntrinsicPropertyAttribute.cs",
                        @"Runtime\CompilerServices\IsVolatile.cs",
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
                        @"Runtime\CompilerServices\Serializable.cs",
                        @"Runtime\CompilerServices\UsedAttribure.cs",
                        @"Runtime\InteropServices\ClassInterfaceAttribute.cs",
                        @"Runtime\InteropServices\ComDefaultInterfaceAttribute.cs",
                        @"Runtime\InteropServices\ComVisible.cs",
                        @"Runtime\InteropServices\InAttribute.cs",
                        @"Runtime\InteropServices\OutAttribute.cs",
                        @"Runtime\Versioning\TargetFrameworkAttribute.cs",
                        @"SByte.cs",
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
                        @"Xml\XmlText.cs"},
                    @"Sources\mscorlib",
                    new string[0],
                    @"Sources\mscorlib\mscorlibKey.snk",
                    true
                ),

                ["system.core"] = (
                    "system.core.dll",
                    new string[] {
                        @"Dynamic\BinaryOperationBinder.cs",
                        @"Dynamic\BindingRestrictions.cs",
                        @"Dynamic\CallInfo.cs",
                        @"Dynamic\ConvertBinder.cs",
                        @"Dynamic\CreateInstanceBinder.cs",
                        @"Dynamic\DeleteIndexBinder.cs",
                        @"Dynamic\DeleteMemberBinder.cs",
                        @"Dynamic\DynamicMetaObject.cs",
                        @"Dynamic\DynamicMetaObjectBinder.cs",
                        @"Dynamic\DynamicObject.cs",
                        @"Dynamic\ExpandoObject.cs",
                        @"Dynamic\GetIndexBinder.cs",
                        @"Dynamic\GetmemberBinder.cs",
                        @"Dynamic\IDynamicMetaObjectProvider.cs",
                        @"Dynamic\IInvokeOnGetBinder.cs",
                        @"Dynamic\InvokeBinder.cs",
                        @"Dynamic\InvokeMemberBinder.cs",
                        @"Dynamic\SetIndexBinder.cs",
                        @"Dynamic\SetMemberBinder.cs",
                        @"Dynamic\UnaryOperationBinder.cs",
                        @"Linq\Expressions\BinaryExpression.cs",
                        @"Linq\Expressions\BlockExpression.cs",
                        @"Linq\Expressions\CatchBlock.cs",
                        @"Linq\Expressions\ConditionalExpression.cs",
                        @"Linq\Expressions\ConstantExpression.cs",
                        @"Linq\Expressions\DebugInfoExpression.cs",
                        @"Linq\Expressions\DefaultExpression.cs",
                        @"Linq\Expressions\DynamicExpression.cs",
                        @"Linq\Expressions\DynamicExpressionVisitor.cs",
                        @"Linq\Expressions\ElementInit.cs",
                        @"Linq\Expressions\Expression.cs",
                        @"Linq\Expressions\ExpressionType.cs",
                        @"Linq\Expressions\ExpressionVisitor.cs",
                        @"Linq\Expressions\GotoExpression.cs",
                        @"Linq\Expressions\GotoExpressionKind.cs",
                        @"Linq\Expressions\IndexExpression.cs",
                        @"Linq\Expressions\InvocationExpression.cs",
                        @"Linq\Expressions\LabelExpression.cs",
                        @"Linq\Expressions\LabelTarget.cs",
                        @"Linq\Expressions\LambdaExpression.cs",
                        @"Linq\Expressions\ListInitExpression.cs",
                        @"Linq\Expressions\LoopExpression.cs",
                        @"Linq\Expressions\MemberAssignment.cs",
                        @"Linq\Expressions\MemberBinding.cs",
                        @"Linq\Expressions\MemberBindingType.cs",
                        @"Linq\Expressions\MemberExpression.cs",
                        @"Linq\Expressions\MemberInitExpression.cs",
                        @"Linq\Expressions\MemberListBinding.cs",
                        @"Linq\Expressions\MemberMemberBinding.cs",
                        @"Linq\Expressions\MethodCallExpression.cs",
                        @"Linq\Expressions\NewArrayExpression.cs",
                        @"Linq\Expressions\NewExpression.cs",
                        @"Linq\Expressions\ParameterExpression.cs",
                        @"Linq\Expressions\RuntimeVairablesExpression.cs",
                        @"Linq\Expressions\SwitchCase.cs",
                        @"Linq\Expressions\SwitchExpression.cs",
                        @"Linq\Expressions\SymbolDocumentInfo.cs",
                        @"Linq\Expressions\TryExpression.cs",
                        @"Linq\Expressions\TypeBinaryExpression.cs",
                        @"Linq\Expressions\UnaryExpression.cs",
                        @"Runtime\CompilerServices\CallSite.cs",
                        @"Runtime\CompilerServices\DynamicAttribute.cs",
                    },
                    @"Sources\System.Core",
                    new string[] {"mscorlib"},
                    @"Sources\mscorlib\mscorlibKey.snk",
                    true
                ),

                ["microsoft.csharp"] = (
                    "microsoft.csharp.dll",
                    new string[] {
                        @"RuntimeBinder\Binder.cs",
                        @"RuntimeBinder\CSharpArgumentInfo.cs",
                        @"RuntimeBinder\CSharpBinderFlags.cs",
                        @"RuntimeBinder\RuntimeBInderException.cs",
                        @"RuntimeBinder\RuntimeBinderInternalCompilerException.cs",
                    },
                    @"Sources\Microsoft.CSharp",
                    new string[] { "mscorlib", "system.core" },
                    @"Sources\mscorlib\mscorlibKey.snk",
                    true
                ),

                ["realScript"] = (
                    "realScript.dll",
                    new string[]
                    {
                        @"DelegateBlocks.cs",
                        @"DupInstructionBlocks.cs",
                        @"DynamicTest.cs",
                        @"ExceptionHandlerSamples.cs",
                        @"ForLoopBlocks.cs",
                        @"FuncRegressions.cs",
                        @"GenericCollections.cs",
                        @"GenericSamples.cs",
                        @"IfBlocks.cs",
                        @"BasicBlockTestFunctions.cs",
                        @"BasicStatements.cs",
                        @"EnumTypes.cs",
                        @"InlineComplexStatements.cs",
                        @"JsScriptImport.cs",
                        @"LoopTests.cs",
                        @"MathAlgorithms.cs",
                        @"NumberOperations.cs",
                        @"ScriptSharpCompat.cs",
                        @"SimpleInterfaceTest.cs",
                        @"NewLanguageFeatures.cs",
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
                    },
                    @"Test\RealScript",
                    new string[] { "mscorlib", "system.core", "microsoft.csharp" },
                    (string)null,
                    false
                ),

                ["realScript.debug"] = (
                    "realScript.debug.dll",
                    new string[]
                    {
                        @"DelegateBlocks.cs",
                        @"DupInstructionBlocks.cs",
                        @"DynamicTest.cs",
                        @"ExceptionHandlerSamples.cs",
                        @"ForLoopBlocks.cs",
                        @"FuncRegressions.cs",
                        @"GenericCollections.cs",
                        @"GenericSamples.cs",
                        @"IfBlocks.cs",
                        @"BasicBlockTestFunctions.cs",
                        @"BasicStatements.cs",
                        @"EnumTypes.cs",
                        @"InlineComplexStatements.cs",
                        @"JsScriptImport.cs",
                        @"LoopTests.cs",
                        @"MathAlgorithms.cs",
                        @"NumberOperations.cs",
                        @"ScriptSharpCompat.cs",
                        @"SimpleInterfaceTest.cs",
                        @"NewLanguageFeatures.cs",
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
                    },
                    @"Test\RealScript",
                    new string[] { "mscorlib", "system.core", "microsoft.csharp" },
                    (string)null,
                    true
                ),
            };

        public static readonly Dictionary<string, Dictionary<IMethodSymbol, MethodBody>> moduleMethodBodyMap
            = new Dictionary<string, Dictionary<IMethodSymbol, MethodBody>>();

        public static Dictionary<IMethodSymbol, MethodBody> MscorlibMethods
            => GetMethodMaps("mscorlib");

        public static Dictionary<IMethodSymbol, MethodBody> RealScriptMethods
            => GetMethodMaps("realScript");

        public static Dictionary<IMethodSymbol, MethodBody> RealScriptDebugMethods
            => GetMethodMaps("realScript.debug");

        public static void CompileAll()
        {
            var tmp = RealScriptDebugMethods;
            tmp = RealScriptMethods;
        }

        private static Dictionary<IMethodSymbol, MethodBody> GetMethodMaps(
            string moduleName)
        {
            if (moduleMethodBodyMap.TryGetValue(moduleName, out var rv))
            { return rv; }

            var resources = sources[moduleName];
            string runtimeMetadataVersion = null;
            if (resources.refs.Select(_ => GetMethodMaps(_)).Count() == 0)
            { runtimeMetadataVersion = "v4.100.0"; }

            var baseGitPath = typeof(TestResources)
                .Assembly
                .Location;
            baseGitPath =
                baseGitPath.Substring(
                    0,
                    baseGitPath.LastIndexOf(
                        typeof(TestResources)
                            .Assembly
                            .GetName()
                            .Name))
                + "..\\";

            baseGitPath = nscriptGitPath;

            var trees = resources
                .files
                .Select(_ =>
                    Path.Combine(
                        Path.GetFullPath(Path.Combine(baseGitPath,resources.directory)),
                        _))
                .Select(_ =>
                    CSharpSyntaxTree.ParseText(
                        File.ReadAllText(_),
                        CSharpParseOptions.Default,
                        _,
                        System.Text.Encoding.UTF8))
                .ToArray();

            var compilerOptions = new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: resources.isDebug
                    ? OptimizationLevel.Debug
                    : OptimizationLevel.Release,
                allowUnsafe: true,
                delaySign: resources.keyFile != null,
                cryptoKeyFile: null,
                platform: Platform.AnyCpu,
                generalDiagnosticOption: ReportDiagnostic.Warn,
                warningLevel: 4,
                concurrentBuild: false,
                specificDiagnosticOptions: new Dictionary<string, ReportDiagnostic>
                {
                    ["0824"] = ReportDiagnostic.Suppress,
                    ["0169"] = ReportDiagnostic.Suppress,
                    ["0649"] = ReportDiagnostic.Suppress,
                    ["0626"] = ReportDiagnostic.Suppress,
                    ["0414"] = ReportDiagnostic.Suppress,
                    ["0660"] = ReportDiagnostic.Suppress,
                    ["0661"] = ReportDiagnostic.Suppress,
                    ["3001"] = ReportDiagnostic.Suppress,
                    ["3002"] = ReportDiagnostic.Suppress,
                    ["1701"] = ReportDiagnostic.Suppress,
                    ["1701"] = ReportDiagnostic.Suppress,
                    ["2008"] = ReportDiagnostic.Suppress,
                });

            var compilation = CSharpCompilation.Create(
                Path.GetFileNameWithoutExtension(resources.outName),
                syntaxTrees: trees,
                options: compilerOptions,
                references: resources
                    .refs
                    .Select(_ => Path.Combine(Path.GetTempPath(), _ + ".dll"))
                    .Select(_ => MetadataReference.CreateFromFile(_))
                    .ToList());

            rv = SerializationHelper.ExpressionVisitMap(
                compilation,
                Path.GetTempPath(),
                resources.outName,
                runtimeMetadataVersion);

            moduleMethodBodyMap[moduleName] = rv;
            return rv;
        }
    }
}
