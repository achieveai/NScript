namespace NScript.Csc.Lib.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Regression coverage for WI-94: an <c>ErrorTypeSymbol</c> reaching
    /// <c>SymbolSerializer.Serialize(TypeSymbol)</c> via the METHOD SIGNATURE
    /// path (param/return type), not the method body. WI-93's
    /// <c>HasErrors</c> gate in <c>SerializationHelper.InjectIntoCompilation</c>
    /// is body-only, so it does not fire when a sibling method's body binds
    /// cleanly but its signature references an undefined type. The fix is a
    /// dedicated <c>ErrorType</c> arm in <c>Serialize(TypeSymbol)</c> that
    /// returns a placeholder <c>TypeSpecSer</c> instead of throwing
    /// <c>NotSupportedException</c>.
    /// </summary>
    [TestClass]
    public class ErrorTypeLeakTests
    {
        [TestMethod]
        public void SignatureLevel_UnknownParameterType_DoesNotThrow()
        {
            // The method body is empty (no body-level binding errors), but the
            // parameter type `NotDefinedType` is unresolved. Roslyn binds the
            // parameter to MissingMetadataTypeSymbol+TopLevel (Kind=ErrorType).
            // SerializationHelper's HasErrors gate sees bodyHasErrors=False and
            // does NOT skip the callback; SymbolSerializer.GetMethodSpecId then
            // serializes the signature and hits the ErrorType arm.
            const string source = @"
namespace Repro
{
    public class B2
    {
        public void DoSomething(NotDefinedType x) { }
    }
}";
            var outcome = Pipeline.Invoke(source, "SigParamRepro");

            Assert.IsNull(
                outcome.ThrownException,
                "Signature-level ErrorType must not crash the serializer. WI-94 regression. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(
                outcome.Success,
                "Emit must report failure when a parameter type fails to bind.");
            Assert.IsTrue(
                outcome.Diagnostics.Any(d =>
                    d.Severity == DiagnosticSeverity.Error
                    && (d.Id == "CS0246" || d.Id == "CS0234" || d.Id == "CS0117")),
                "Expected a Roslyn type-resolution diagnostic. Actual: "
                    + string.Join(", ", outcome.Diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
        }

        [TestMethod]
        public void SignatureLevel_UnknownReturnType_DoesNotThrow()
        {
            // Mirror of the parameter case for the return-type path. Roslyn
            // serializes return type before parameters; both flow through the
            // same Serialize(TypeSymbol) entry.
            const string source = @"
namespace Repro
{
    public class B2
    {
        public NotDefinedType GetSomething() { return default(NotDefinedType); }
    }
}";
            var outcome = Pipeline.Invoke(source, "SigReturnRepro");

            Assert.IsNull(
                outcome.ThrownException,
                "Signature-level ErrorType (return type) must not crash. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(outcome.Success);
            Assert.IsTrue(
                outcome.Diagnostics.Any(d =>
                    d.Severity == DiagnosticSeverity.Error
                    && (d.Id == "CS0246" || d.Id == "CS0234")),
                "Expected CS0246/CS0234 for unresolved return type. Actual: "
                    + string.Join(", ", outcome.Diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
        }

        [TestMethod]
        public void SignatureLevel_MixedWithHealthySibling_CapturesHealthyOnly()
        {
            // Pair an unresolved-signature method with a healthy sibling. The
            // healthy method must still be captured (HasErrors gate is
            // per-callback) — the broken signature must not poison the rest of
            // the compilation.
            const string source = @"
namespace Repro
{
    public class B2
    {
        public void Broken(NotDefinedType x) { }
        public int Healthy(int y) { return y + 1; }
    }
}";
            var outcome = Pipeline.Invoke(source, "MixedSigRepro");

            Assert.IsNull(
                outcome.ThrownException,
                "Mixed compilation with sig-level ErrorType must not crash. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(outcome.Success);
            Assert.IsTrue(
                outcome.MethodCount >= 1,
                "Healthy sibling method body must still be captured. Captured: "
                    + outcome.MethodCount);
        }

        [TestMethod]
        public void SignatureLevel_GenericArgumentIsErrorType_DoesNotThrow()
        {
            // The unresolved type appears as a generic argument inside a
            // resolved generic (List<T>). NamedType is serialized normally;
            // its TypeArguments recurse through GetTypeSpecSer → the ErrorType
            // arm fires for the inner argument.
            const string source = @"
namespace Repro
{
    public class B2
    {
        public void Take(System.Collections.Generic.List<NotDefinedType> xs) { }
    }
}";
            var outcome = Pipeline.Invoke(source, "GenericArgRepro");

            Assert.IsNull(
                outcome.ThrownException,
                "Generic argument that is an ErrorType must not crash. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(outcome.Success);
        }

        [TestMethod]
        public void SignatureLevel_ArrayOfErrorType_DoesNotThrow()
        {
            // ArrayTypeSymbol whose ElementType is an ErrorType — the array
            // arm of Serialize recurses on the element, hitting the ErrorType
            // arm transitively.
            const string source = @"
namespace Repro
{
    public class B2
    {
        public void Take(NotDefinedType[] xs) { }
    }
}";
            var outcome = Pipeline.Invoke(source, "ArrayErrorRepro");

            Assert.IsNull(
                outcome.ThrownException,
                "Array element ErrorType must not crash. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(outcome.Success);
        }

        [TestMethod]
        public void TitleScenario_DeclareThenAssignInTry_DoesNotThrow()
        {
            // The issue title names "declare-then-assign-in-try local". WI-93's
            // body-level HasErrors gate already covers this body-only shape
            // when the bound expression carries errors. Keep a regression test
            // here so the title scenario itself is locked in.
            const string source = @"
namespace Repro
{
    public class B3
    {
        public void Send(string s)
        {
            UnknownType local;
            try
            {
                local = new UnknownType();
            }
            catch
            {
                local = null;
            }
        }
    }
}";
            var outcome = Pipeline.Invoke(source, "DeclareAssignTryRepro");

            Assert.IsNull(
                outcome.ThrownException,
                "Body-level declare-then-assign-in-try with ErrorType must not crash. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(outcome.Success);
        }

        private static class Pipeline
        {
            public sealed class Outcome
            {
                public bool Success { get; init; }
                public System.Collections.Immutable.ImmutableArray<Diagnostic> Diagnostics { get; init; }
                    = System.Collections.Immutable.ImmutableArray<Diagnostic>.Empty;
                public int MethodCount { get; init; }
                public Exception ThrownException { get; init; }
            }

            public static Outcome Invoke(string source, string assemblyName)
            {
                var tree = CSharpSyntaxTree.ParseText(source, path: assemblyName + ".cs");
                var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                var compilation = CSharpCompilation.Create(
                    assemblyName,
                    syntaxTrees: new[] { tree },
                    references: new[] { mscorlib },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                try
                {
                    var (resources, rv) = SerializationHelper.InjectIntoCompilation(compilation);
                    using var output = new MemoryStream();
                    var result = compilation.Emit(output, manifestResources: resources);
                    return new Outcome
                    {
                        Success = result.Success,
                        Diagnostics = result.Diagnostics,
                        MethodCount = rv.Count,
                    };
                }
                catch (Exception ex)
                {
                    return new Outcome { ThrownException = ex };
                }
            }
        }
    }
}
