namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;

    /// <summary>
    /// Slice 2, Inc 1: dev-mode stable names (design section 3). Names come from identity and
    /// metadata only, so a body edit renames nothing outside its function, and the NSDEV guards
    /// turn any collision into a build error instead of a silently shadowed name.
    /// </summary>
    [TestClass]
    public class DevNamingTests
    {
        private static readonly ModuleDefinition Module =
            ModuleDefinition.CreateModule("DevNamingFixture", ModuleKind.Dll);

        private static TypeDefinition Type(string ns, string name, TypeDefinition declaringType = null)
        {
            var type = new TypeDefinition(ns, name, TypeAttributes.Public | TypeAttributes.Class, Module.TypeSystem.Object);
            if (declaringType != null)
            {
                declaringType.NestedTypes.Add(type);
            }
            else
            {
                Module.Types.Add(type);
            }

            return type;
        }

        private static MethodDefinition Method(TypeDefinition owner, string name, params TypeReference[] parameters)
        {
            var method = new MethodDefinition(name, MethodAttributes.Public, Module.TypeSystem.Void);
            foreach (var parameter in parameters)
            {
                method.Parameters.Add(new ParameterDefinition(parameter));
            }

            owner.Methods.Add(method);
            return method;
        }

        private static SimpleIdentifier Root(IdentifierScope scope, string suggested, string stable)
        {
            var ident = SimpleIdentifier.CreateScopeIdentifier(scope, suggested, false);
            ident.StableName = stable;
            return ident;
        }

        private static void Use(SimpleIdentifier ident, IdentifierScope at, int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                ident.AddUsage(at);
            }
        }

        [TestMethod]
        public void A1_TypeFormCarriesMarkerSoGlobalTypesCannotShadowBrowserGlobals()
        {
            var date = Type(string.Empty, "Date");
            var nested = Type("A", "B");

            Assert.AreEqual("Date$", DevNames.TypeForm(date));
            Assert.AreEqual("A_B$", DevNames.TypeForm(nested));
            Assert.AreNotEqual(DevNames.M(nested.FullName), DevNames.TypeForm(nested), "A type must not take the mangled namespace name A.B.");

            // A user type `Date` next to the enforced browser global `Date` in one bundle.
            var root = new IdentifierScope(true);
            var global = SimpleIdentifier.CreateScopeIdentifier(root, "Date", true);
            var userType = Root(root, "Date", DevNames.TypeForm(date));

            var namer = IdentifierScope.DevStableNamer.NameExecutionTree(root);

            Assert.AreEqual(0, namer.Errors.Count, string.Join("; ", namer.Errors));
            Assert.AreEqual("Date", global.GetName());
            Assert.AreEqual("Date$", userType.GetName());
        }

        [TestMethod]
        public void NestedTypesWithTheSameNameGetDifferentForms()
        {
            var list = Type("System.Collections.Generic", "List`1");
            var dictionary = Type("System.Collections.Generic", "Dictionary`2");
            var listEnumerator = Type(string.Empty, "Enumerator", list);
            var dictionaryEnumerator = Type(string.Empty, "Enumerator", dictionary);

            Assert.AreEqual("System_Collections_Generic_List_11_9Enumerator$", DevNames.TypeForm(listEnumerator));
            Assert.AreEqual("System_Collections_Generic_Dictionary_12_9Enumerator$", DevNames.TypeForm(dictionaryEnumerator));
        }

        [TestMethod]
        public void InternalTypesOfTheSameNameInTwoAssembliesGetDifferentForms()
        {
            // A source file linked into two projects gives both assemblies the same internal type.
            TypeDefinition InternalShim(string assembly)
            {
                var module = ModuleDefinition.CreateModule(assembly, ModuleKind.Dll);
                var type = new TypeDefinition("Shared", "Shim", TypeAttributes.NotPublic | TypeAttributes.Class, module.TypeSystem.Object);
                module.Types.Add(type);
                var nested = new TypeDefinition(string.Empty, "Closure", TypeAttributes.NestedPrivate | TypeAttributes.Class, module.TypeSystem.Object);
                type.NestedTypes.Add(nested);
                return type;
            }

            var first = InternalShim("LibA");
            var second = InternalShim("LibB");

            Assert.AreEqual("LibA$$asm$Shared_Shim$", DevNames.TypeForm(first));
            Assert.AreEqual("LibB$$asm$Shared_Shim$", DevNames.TypeForm(second));
            Assert.AreEqual("LibA$$asm$Shared_Shim_9Closure$", DevNames.TypeForm(first.NestedTypes[0]));

            // Public types, and private types nested in them, keep the short form.
            var list = Type("Pub", "List");
            var enumerator = new TypeDefinition(string.Empty, "Enumerator", TypeAttributes.NestedPrivate | TypeAttributes.Class, Module.TypeSystem.Object);
            list.NestedTypes.Add(enumerator);
            Assert.AreEqual("Pub_List$", DevNames.TypeForm(list));
            Assert.AreEqual("Pub_List_9Enumerator$", DevNames.TypeForm(enumerator));
        }

        /// <summary>
        /// F-G: a legal .NET assembly name may start with a digit (<c>123.App</c>); the form of
        /// its internal types must still be a JS identifier, and differ from <c>App123</c>'s.
        /// </summary>
        [TestMethod]
        public void InternalTypeOfADigitLeadingAssembly_IsAJsIdentifier()
        {
            TypeDefinition Internal(string assembly)
            {
                var module = ModuleDefinition.CreateModule(assembly, ModuleKind.Dll);
                var type = new TypeDefinition(string.Empty, "Program", TypeAttributes.NotPublic | TypeAttributes.Class, module.TypeSystem.Object);
                module.Types.Add(type);
                return type;
            }

            var form = DevNames.TypeForm(Internal("123.App"));

            Assert.AreEqual("$123_App$$asm$Program$", form);
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(form, @"^[A-Za-z_$][A-Za-z0-9_$]*$"), form);
            Assert.AreEqual("App123$$asm$Program$", DevNames.TypeForm(Internal("App123")), "a letter-leading name keeps its form");
        }

        [TestMethod]
        public void ManglingIsInjectiveOnSeparatorsAndNeverEmitsDollar()
        {
            var inputs = new[] { "a.b", "a_b", "a/b", "a`b", "a__b", "a._b", "a-b", "a<b>" };
            var outputs = inputs.Select(DevNames.M).ToList();

            CollectionAssert.AllItemsAreUnique(outputs, string.Join(", ", outputs));
            Assert.IsFalse(outputs.Any(o => o.Contains("$")), string.Join(", ", outputs));
        }

        [TestMethod]
        public void KindTagsSeparateMembersWithTheSameNameInOneType()
        {
            // IL allows a field and a method with one name; a virtual slot and its
            // implementation always share the method name in one scope.
            var names = new[]
            {
                DevNames.Member("Title", 1, DevNames.Kind.Field),
                DevNames.Member("Title", 1, DevNames.Kind.InstanceMethod),
                DevNames.Member("Title", 1, DevNames.Kind.VirtualSlot),
                DevNames.Member("Title", 1, DevNames.Kind.Property),
            };
            CollectionAssert.AllItemsAreUnique(names, string.Join(", ", names));
            Assert.AreEqual("Title$1p", names[3]);

            var scope = new IdentifierScope(false);
            var child = new IdentifierScope(scope, "Child_Inst");
            foreach (var name in names)
            {
                Root(child, "Title", name);
            }

            var namer = IdentifierScope.DevStableNamer.NameTypeTree(scope);
            Assert.AreEqual(0, namer.Errors.Count, string.Join("; ", namer.Errors));
        }

        [TestMethod]
        public void OverloadSignatureComesFromMetadataOnly()
        {
            var owner = Type("Fixture", "Overloads");
            var addInt = Method(owner, "Add", Module.TypeSystem.Int32);
            var addString = Method(owner, "Add", Module.TypeSystem.String);
            var single = Method(owner, "Clear");

            Assert.IsNull(DevNames.MethodSig(single), "A single method of a name gets no sig.");
            Assert.AreEqual("Fixture_Overloads$Add$System_Int32$", DevNames.StaticMember(owner, addInt.Name, DevNames.MethodSig(addInt)));
            Assert.AreEqual("Fixture_Overloads$Add$System_String$", DevNames.StaticMember(owner, addString.Name, DevNames.MethodSig(addString)));
        }

        [TestMethod]
        public void DevTypeIdsAreStableShapedAndDistinct()
        {
            var a = Type("Fixture", "IdA");
            var b = Type("Fixture", "IdB");

            Assert.AreEqual(DevNames.TypeId(a), DevNames.TypeId(a));
            Assert.AreNotEqual(DevNames.TypeId(a), DevNames.TypeId(b));
            StringAssert.Matches(DevNames.TypeId(a), new System.Text.RegularExpressions.Regex("^k[0-9A-Za-z]{9}$"));
        }

        /// <summary>
        /// Builds a global scope with two functions. <paramref name="hotUses"/> is how often F2
        /// uses root identifier X: a frequency shift that renames other functions' identifiers
        /// under the usage-ordered minified namer.
        /// </summary>
        private static (IdentifierScope root, SimpleIdentifier x, SimpleIdentifier y, SimpleIdentifier f1Local, SimpleIdentifier f1Param)
            LocalityFixture(int hotUses)
        {
            var root = new IdentifierScope(true);
            var x = Root(root, "X", "Fixture_X$");
            var y = Root(root, "Y", "Fixture_Y$");

            var f1 = new IdentifierScope(root, new[] { "item" }, false);
            var f1Local = SimpleIdentifier.CreateScopeIdentifier(f1, "count", false);
            Use(y, f1, 5);
            Use(f1Local, f1, 2);
            Use(f1.ParameterIdentifiers[0], f1);

            var f2 = new IdentifierScope(root, new[] { "value" }, false);
            Use(x, f2, hotUses);
            return (root, x, y, f1Local, f1.ParameterIdentifiers[0]);
        }

        [TestMethod]
        public void Locality_FrequencyShiftInOneFunctionRenamesNothingElse()
        {
            string[] DevNamesOf(int hotUses)
            {
                var (root, x, y, local, param) = LocalityFixture(hotUses);
                var namer = IdentifierScope.DevStableNamer.NameExecutionTree(root);
                Assert.AreEqual(0, namer.Errors.Count, string.Join("; ", namer.Errors));
                return new[] { x.GetName(), y.GetName(), local.GetName(), param.GetName() };
            }

            string[] MinifiedNamesOf(int hotUses)
            {
                var (root, x, y, local, param) = LocalityFixture(hotUses);
                IdentifierScope.IdentifierMinifiedNamer.MinifyNames(root, false);
                return new[] { x.GetName(), y.GetName(), local.GetName(), param.GetName() };
            }

            // The edit must matter: the old namer renames F1's references when only F2 changed.
            CollectionAssert.AreNotEqual(MinifiedNamesOf(1), MinifiedNamesOf(30), "Fixture no longer shifts the old namer; the locality check proves nothing.");

            var before = DevNamesOf(1);
            CollectionAssert.AreEqual(before, DevNamesOf(30));
            CollectionAssert.AreEqual(new[] { "Fixture_X$", "Fixture_Y$", "count", "item" }, before);
        }

        [TestMethod]
        public void Locals_AvoidReservedWordsFreeIdentifiersAndEnclosingNames()
        {
            var root = new IdentifierScope(true);
            var window = SimpleIdentifier.CreateScopeIdentifier(root, "window", true);
            var outer = new IdentifierScope(root, new[] { "default" }, false);
            var shadow = SimpleIdentifier.CreateScopeIdentifier(outer, "window", false);
            var name = SimpleIdentifier.CreateScopeIdentifier(outer, "name", false);
            var inner = new IdentifierScope(outer, new[] { "name" }, false);
            Use(window, outer);
            Use(name, outer);

            var namer = IdentifierScope.DevStableNamer.NameExecutionTree(root);

            Assert.AreEqual(0, namer.Errors.Count, string.Join("; ", namer.Errors));
            Assert.AreEqual("default_2", outer.ParameterIdentifiers[0].GetName());
            Assert.AreEqual("window_2", shadow.GetName());
            Assert.AreEqual("name", name.GetName());
            Assert.AreEqual("name_2", inner.ParameterIdentifiers[0].GetName(), "A closure parameter must not shadow its enclosing local.");
        }

        [TestMethod]
        public void Guards_RootCollisionIsNsdev001()
        {
            var root = new IdentifierScope(true);
            Root(root, "A", "Same$");
            Root(root, "B", "Same$");

            var namer = IdentifierScope.DevStableNamer.NameExecutionTree(root);

            Assert.AreEqual(1, namer.Errors.Count);
            StringAssert.StartsWith(namer.Errors[0], "NSDEV001");
        }

        [TestMethod]
        public void Guards_LookupChainCollisionIsNsdev001ButImportedOverridesAreAllowed()
        {
            var objectScope = new IdentifierScope(false);
            var baseScope = new IdentifierScope(objectScope, "Base_Inst");
            var derived = new IdentifierScope(baseScope, "Derived_Inst");
            var sibling = new IdentifierScope(objectScope, "Sibling_Inst");
            Root(baseScope, "Foo", "Foo$1m");
            Root(derived, "Foo", "Foo$1m");
            Root(sibling, "Foo", "Foo$1m");
            SimpleIdentifier.CreateScopeIdentifier(baseScope, "toString", true);
            SimpleIdentifier.CreateScopeIdentifier(derived, "toString", true);

            var namer = IdentifierScope.DevStableNamer.NameTypeTree(objectScope);

            Assert.AreEqual(1, namer.Errors.Count, string.Join("; ", namer.Errors));
            StringAssert.StartsWith(namer.Errors[0], "NSDEV001");
            StringAssert.Contains(namer.Errors[0], "Derived_Inst");
        }

        [TestMethod]
        public void Guards_InterfaceSlotLookalikeIsNsdev001()
        {
            var objectScope = new IdentifierScope(false);
            Root(objectScope, "Foo", "Foo$0m$X_kAbCdEfGh1");

            var namer = IdentifierScope.DevStableNamer.NameTypeTree(objectScope);

            Assert.AreEqual(1, namer.Errors.Count, string.Join("; ", namer.Errors));
            StringAssert.StartsWith(namer.Errors[0], "NSDEV001");
        }

        [TestMethod]
        public void Guards_UnnamedRootIdentifierIsNsdev002_AndPluginNamesFallBackPerName()
        {
            var root = new IdentifierScope(true);
            var first = SimpleIdentifier.CreateScopeIdentifier(root, "tmplStore", false);
            var second = SimpleIdentifier.CreateScopeIdentifier(root, "tmplStore", false);
            SimpleIdentifier.CreateScopeIdentifier(root, string.Empty, false);

            var namer = IdentifierScope.DevStableNamer.NameExecutionTree(root);

            Assert.AreEqual("tmplStore$$g", first.GetName());
            Assert.AreEqual("tmplStore$$g2", second.GetName());
            Assert.IsTrue(namer.Errors.Any(e => e.StartsWith("NSDEV002", StringComparison.Ordinal)), string.Join("; ", namer.Errors));
            Assert.AreEqual(3, namer.Fallbacks.Count);
        }

        /// <summary>Roots every static parameterless <c>Main</c> of the RealScript fixture.</summary>
        private sealed class AllMainsPlugin : IRuntimeConverterPlugin
        {
            private ClrContext clrContext;

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
                => this.clrContext = clrContext;

            public void ParseArgs(IList<Tuple<string, string>> args) { }

            public List<MethodReference> GetMethodsToEmitPass1()
                => this.clrContext.GetTypeDefinitions()
                    .Where(t => t.Namespace == "RealScript")
                    .SelectMany(t => t.Methods)
                    .Where(m => m.IsStatic && m.Name == "Main" && !m.HasParameters)
                    .Cast<MethodReference>()
                    .ToList();

            public List<MethodReference> GetMethodsToEmitPassN() => null;

            public List<Statement> GetPreJavascript() => null;

            public List<Statement> GetPostJavascript() => null;
        }

        [TestMethod]
        [TestCategory("Integration")] // 17 s fixture setup; only end-to-end dev-mode run over a real script fixture.
        public void DevModeBuild_FixtureHasNoNamingErrorsAndUsesStableNames()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var outJs = Path.Combine(temp, "devnaming_" + Guid.NewGuid().ToString("N") + ".js");
            try
            {
                Builder.ResetProcessState();
                var builder = new Builder(
                    outJs,
                    1,
                    Path.Combine(temp, "realScript.dll"),
                    new[]
                    {
                        Path.Combine(temp, "mscorlib.dll"),
                        Path.Combine(temp, "system.core.dll"),
                        Path.Combine(temp, "microsoft.csharp.dll"),
                    },
                    new IConverterPlugin[] { new AllMainsPlugin() },
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);

                Assert.IsTrue(builder.Execute(), "Dev-mode build failed; NSDEV errors are printed above.");
                var js = File.ReadAllText(outJs);
                StringAssert.Contains(js, "System_Object$", "Root type names must use the dev type form.");
                StringAssert.Contains(js, "$$ptyp", "The reusable prototype variable must use its stable name.");
            }
            finally
            {
                foreach (var file in new[] { outJs, Path.ChangeExtension(outJs, ".map"), Path.ChangeExtension(outJs, ".ashx") })
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("Integration")] // Needs the real mscorlib fixture for a ConverterContext (shared ~10 s setup).
        public void D1_MembersDifferingOnlyInCaseGetDifferentStableNames()
        {
            // D1 (tester-m2): Go() and go(), or property Size and method get_size(), are valid C#.
            // Their JS member names are equal after the first letter is lowered, so the stable
            // name must come from the metadata name or NSDEV001 rejects the build.
            TestAssemblyLoader.LoadAssemblies();
            var context = new ConverterContext(TestAssemblyLoader.Context) { DevMode = true };
            var known = TestAssemblyLoader.Context.KnownReferences;

            // Types come from the fixture mscorlib; the in-memory module's would resolve to the host's.
            var owner = new TypeDefinition("Fixture", "CaseMembers", TypeAttributes.Public | TypeAttributes.Class, known.Object.Resolve());
            Module.Types.Add(owner);
            var upper = new MethodDefinition("Go", MethodAttributes.Public, known.Void.Resolve());
            var lower = new MethodDefinition("go", MethodAttributes.Public, known.Void.Resolve());
            owner.Methods.Add(upper);
            owner.Methods.Add(lower);
            var ctor = new MethodDefinition(
                ".ctor",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
                known.Void.Resolve());
            owner.Methods.Add(ctor);
            foreach (var method in new[] { upper, lower, ctor })
            {
                method.Body.GetILProcessor().Emit(Mono.Cecil.Cil.OpCodes.Ret); // a body: not extern
            }
            var sizeField = new FieldDefinition("Size", FieldAttributes.Public, known.Int32.Resolve());
            var lowerField = new FieldDefinition("size", FieldAttributes.Public, known.Int32.Resolve());
            owner.Fields.Add(sizeField);
            owner.Fields.Add(lowerField);

            var root = new IdentifierScope(false);
            var manager = new TypeScopeManager(context, owner, root, new IdentifierScope(root, "Statics"));
            var names = new[]
            {
                ((SimpleIdentifier)manager.ResolveMethod(upper)).StableName,
                ((SimpleIdentifier)manager.ResolveMethod(lower)).StableName,
                ((SimpleIdentifier)manager.ResolveField(sizeField)).StableName,
                ((SimpleIdentifier)manager.ResolveField(lowerField)).StableName,
            };

            CollectionAssert.AllItemsAreUnique(names, string.Join(", ", names));
            var namer = IdentifierScope.DevStableNamer.NameTypeTree(root);
            Assert.AreEqual(0, namer.Errors.Count, string.Join("; ", namer.Errors));
        }
    }
}
