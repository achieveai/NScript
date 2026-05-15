namespace NScript.Csc.Lib.Test
{
    using System.IO;
    using JsCsc.Lib;
    using JsCsc.Lib.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;

    /// <summary>
    /// Pins the wire-level round-trip of the Roslyn-synthesised method-reference
    /// shapes that previously NRE'd the in-test <c>BondToAst</c> pipeline
    /// (issue #61):
    ///
    /// * <see cref="MethodSpecSer.IsInitOnly"/> survives ProtoBuf round-trip.
    /// * <see cref="MemberReferenceDeserializer.StripSignatureWrappers"/> peels
    ///   every <c>modreq</c> / <c>modopt</c> / by-ref / pointer wrapper —
    ///   covers the relaxed-signature fallback used when Cecil's strict
    ///   resolver returns null.
    /// * <see cref="MemberReferenceDeserializer.RelaxedTypeMatches"/> ignores
    ///   wrapper shape but still distinguishes element types.
    ///
    /// End-to-end coverage for the full deserializer path comes from
    /// <c>Lang9RecordTests.cs</c> and <c>Lang11RequiredTests.cs</c> being
    /// re-listed in <see cref="TestResources"/>; this file pins the small
    /// units a future regression would surface in isolation first.
    /// </summary>
    [TestClass]
    public class MemberReferenceRoundTripTests
    {
        [TestMethod]
        public void MethodSpecSer_IsInitOnly_RoundTripsTrue()
        {
            var original = new MethodSpecSer
            {
                Name = "set_Theme",
                IsInitOnly = true,
            };

            var clone = RoundTrip(original);

            Assert.AreEqual("set_Theme", clone.Name);
            Assert.IsTrue(clone.IsInitOnly);
        }

        [TestMethod]
        public void MethodSpecSer_IsInitOnly_DefaultsFalse()
        {
            var original = new MethodSpecSer
            {
                Name = "RegularMethod",
            };

            var clone = RoundTrip(original);

            Assert.IsFalse(clone.IsInitOnly);
        }

        [TestMethod]
        public void StripSignatureWrappers_PlainType_ReturnsSame()
        {
            var module = NewTransientModule();
            var voidRef = module.TypeSystem.Void;

            var result = MemberReferenceDeserializer.StripSignatureWrappers(voidRef);

            Assert.AreSame(voidRef, result);
        }

        [TestMethod]
        public void StripSignatureWrappers_RequiredModifier_PeelsToElement()
        {
            var module = NewTransientModule();
            var voidRef = module.TypeSystem.Void;
            var isExternalInit = NewMarkerType(module, "System.Runtime.CompilerServices", "IsExternalInit");

            var wrapped = new RequiredModifierType(isExternalInit, voidRef);

            var result = MemberReferenceDeserializer.StripSignatureWrappers(wrapped);

            Assert.AreSame(voidRef, result);
        }

        [TestMethod]
        public void StripSignatureWrappers_OptionalModifier_PeelsToElement()
        {
            var module = NewTransientModule();
            var intRef = module.TypeSystem.Int32;
            var marker = NewMarkerType(module, "Test.Modopts", "OptionalMarker");

            var wrapped = new OptionalModifierType(marker, intRef);

            var result = MemberReferenceDeserializer.StripSignatureWrappers(wrapped);

            Assert.AreSame(intRef, result);
        }

        [TestMethod]
        public void StripSignatureWrappers_ByReference_PeelsToElement()
        {
            var module = NewTransientModule();
            var intRef = module.TypeSystem.Int32;

            var wrapped = new ByReferenceType(intRef);

            var result = MemberReferenceDeserializer.StripSignatureWrappers(wrapped);

            Assert.AreSame(intRef, result);
        }

        [TestMethod]
        public void StripSignatureWrappers_Pointer_PeelsToElement()
        {
            var module = NewTransientModule();
            var intRef = module.TypeSystem.Int32;

            var wrapped = new PointerType(intRef);

            var result = MemberReferenceDeserializer.StripSignatureWrappers(wrapped);

            Assert.AreSame(intRef, result);
        }

        [TestMethod]
        public void StripSignatureWrappers_NestedWrappers_FullyPeel()
        {
            // Roslyn's init-setter signatures stack modreq on top of plain
            // types; nested wrapper handling guards against future modreq
            // shapes (e.g. ref-readonly returns) without code change.
            var module = NewTransientModule();
            var voidRef = module.TypeSystem.Void;
            var markerA = NewMarkerType(module, "Test", "MarkerA");
            var markerB = NewMarkerType(module, "Test", "MarkerB");

            var wrapped = new RequiredModifierType(
                markerA,
                new OptionalModifierType(markerB, voidRef));

            var result = MemberReferenceDeserializer.StripSignatureWrappers(wrapped);

            Assert.AreSame(voidRef, result);
        }

        [TestMethod]
        public void RelaxedTypeMatches_PlainVoidAndModReqVoid_AreEqual()
        {
            // The original NRE path: a freshly deserialized MethodReference
            // has plain `void` while the on-disk MethodDefinition has
            // `void modreq(IsExternalInit)`. The relaxed matcher must treat
            // them as equal for fallback resolution to succeed.
            var module = NewTransientModule();
            var voidRef = module.TypeSystem.Void;
            var isExternalInit = NewMarkerType(module, "System.Runtime.CompilerServices", "IsExternalInit");

            var plain = voidRef;
            var withModReq = new RequiredModifierType(isExternalInit, voidRef);

            Assert.IsTrue(MemberReferenceDeserializer.RelaxedTypeMatches(plain, withModReq));
            Assert.IsTrue(MemberReferenceDeserializer.RelaxedTypeMatches(withModReq, plain));
        }

        [TestMethod]
        public void RelaxedTypeMatches_DifferentElementTypes_AreNotEqual()
        {
            // Wrapper-stripping must not produce false positives across
            // genuinely different element types — only identical element
            // FullNames after stripping should match.
            var module = NewTransientModule();
            var intRef = module.TypeSystem.Int32;
            var stringRef = module.TypeSystem.String;

            Assert.IsFalse(MemberReferenceDeserializer.RelaxedTypeMatches(intRef, stringRef));
            Assert.IsFalse(
                MemberReferenceDeserializer.RelaxedTypeMatches(
                    new ByReferenceType(intRef),
                    new ByReferenceType(stringRef)));
        }

        [TestMethod]
        public void RelaxedTypeMatches_ByRefAndPlain_AreEqual()
        {
            // `ref int` / `out int` parameters strip down to `int` — the
            // ParameterAttributes flow on the deserialized MethodReference
            // independently carries the by-ref bit, so the relaxed matcher
            // intentionally collapses them.
            var module = NewTransientModule();
            var intRef = module.TypeSystem.Int32;

            Assert.IsTrue(
                MemberReferenceDeserializer.RelaxedTypeMatches(
                    intRef,
                    new ByReferenceType(intRef)));
        }

        private static T RoundTrip<T>(T value)
        {
            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, value);
            ms.Position = 0;
            return ProtoBuf.Serializer.Deserialize<T>(ms);
        }

        private static ModuleDefinition NewTransientModule()
        {
            // Empty in-memory module backing the TypeSystem.* primitives
            // used by the wrapper tests. We never write the module out, so
            // the missing core-library reference is fine.
            return ModuleDefinition.CreateModule(
                "Test.Transient",
                new ModuleParameters
                {
                    Kind = ModuleKind.Dll,
                });
        }

        private static TypeDefinition NewMarkerType(
            ModuleDefinition module,
            string @namespace,
            string name)
        {
            var typeDef = new TypeDefinition(
                @namespace,
                name,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
                module.TypeSystem.Object);

            module.Types.Add(typeDef);
            return typeDef;
        }
    }
}
