using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.JST.Visitors;
using NScript.Utils;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Phase 2 coverage: literal <see cref="Location"/> propagation, statement-level
    /// dispatcher wrapping, and <see cref="CatchHandler"/> location wiring.
    /// These tests construct real JST nodes (not just EnterLocation/LeaveLocation
    /// calls on the writer) and drive them through <see cref="JSWriter.Write(Node)"/>
    /// so the dispatcher-level wrapping added in Phase 2 is exercised end-to-end.
    /// </summary>
    [TestClass]
    public class Phase2MappingTests
    {
        /// <summary>
        /// Phase 2a: a <see cref="NumberLiteralExpression"/> constructed with a Location
        /// must produce a mapping pointing at that source line when written through
        /// the dispatcher. Prior to Phase 2 the literal ctor dropped Location on the
        /// floor by passing null to the Expression base.
        /// </summary>
        [TestMethod]
        public void Write_NumberLiteralWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Program.cs", 7, 12, 7, 14);

            var literal = new NumberLiteralExpression(scope, 42, location);
            writer.Write(literal);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Program.cs", sourceLine: 6),
                "Expected Program.cs line 7 (zero-indexed 6) in map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2a: a string literal with a Location must also surface in the map.
        /// </summary>
        [TestMethod]
        public void Write_StringLiteralWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Strings.cs", 3, 0, 3, 5);

            var literal = new StringLiteralExpression(scope, "hi", location);
            writer.Write(literal);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Strings.cs", sourceLine: 2),
                "Expected Strings.cs line 3 (zero-indexed 2) in map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2a regression: a literal constructed without a Location must not
        /// cause mappings to be emitted for an unrelated source. The null-location
        /// default path is exercised by every synthetic compiler-generated literal
        /// (default values, runtime-scope constants), so it must stay clean.
        /// </summary>
        [TestMethod]
        public void Write_LiteralWithoutLocation_DoesNotLeakIntoMap()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);

            var literal = new NumberLiteralExpression(scope, 7);
            writer.Write(literal);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            foreach (var src in decoded.Sources)
            {
                StringAssert.EndsWith(src, "out.js",
                    "Unexpected source file appeared in map from a null-location literal: " + src);
            }
        }

        /// <summary>
        /// Phase 2b: a Statement with a Location must be wrapped by the dispatcher
        /// so the child expression tokens inherit the statement's source line.
        /// Built from a <see cref="ReturnStatement"/> containing an expressionless
        /// return so we don't need to construct an IIdentifier.
        /// </summary>
        [TestMethod]
        public void Write_StatementWithLocation_DispatcherWrapsIt()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Control.cs", 15, 4, 15, 11);

            var stmt = new ReturnStatement(location, scope, null);
            writer.Write(stmt);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Control.cs", sourceLine: 14),
                "Statement Location should be wrapped by the dispatcher. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2b regression: a Statement constructed with null Location must NOT
        /// emit a mapping — the dispatcher only enters a location when one is present.
        /// Guards against accidentally wrapping synthetic compiler-generated statements
        /// (e.g., enumerator setup in ForLoopConverter) with a garbage source.
        /// </summary>
        [TestMethod]
        public void Write_StatementWithoutLocation_NoMappingEmitted()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);

            var stmt = new ReturnStatement(null, scope, null);
            writer.Write(stmt);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            foreach (var src in decoded.Sources)
            {
                StringAssert.EndsWith(src, "out.js",
                    "A statement with null Location leaked an unexpected source into the map: " + src);
            }
        }

        /// <summary>
        /// Phase 2b: an <see cref="ExpressionStatement"/> with a Location should map
        /// its semicolon/syntax to the statement's source line. This also exercises
        /// the interaction between dispatcher-level wrapping and the manual
        /// EnterLocation call inside ExpressionStatement.Write — double-push is
        /// harmless because both pushes carry the same Location.
        /// </summary>
        [TestMethod]
        public void Write_ExpressionStatementWithLocation_MapsStatement()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var stmtLoc = new Location("Prog.cs", 20, 0, 20, 10);
            var exprLoc = new Location("Prog.cs", 20, 2, 20, 4);

            var inner = new NumberLiteralExpression(scope, 99, exprLoc);
            var stmt = new ExpressionStatement(stmtLoc, scope, inner);
            writer.Write(stmt);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Prog.cs", sourceLine: 19),
                "Expected Prog.cs line 20 in map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2c: the <see cref="CatchHandler"/> ctor accepted in Phase 2 must
        /// store the Location on the node so the dispatcher can pick it up.
        /// A pure unit check — construct the node, verify Location round-trips.
        /// </summary>
        [TestMethod]
        public void CatchHandler_LocationParameter_StoredOnNode()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Try.cs", 42, 0, 42, 5);
            var emptyBlock = new ScopeBlock(null, scope, new List<Statement>());

            var handler = new CatchHandler(scope, catchIdentifier: null, catchBlock: emptyBlock, location: location);

            Assert.IsNotNull(handler.Location, "CatchHandler should expose the Location it was constructed with.");
            Assert.AreEqual("Try.cs", handler.Location.FileName);
            Assert.AreEqual(42, handler.Location.StartLine);
        }

        /// <summary>
        /// Phase 2c: the Location passed to CatchHandler must reach the source map
        /// when the handler is written through the dispatcher. Pairs with the unit
        /// check above to confirm end-to-end wiring, not just parameter plumbing.
        /// </summary>
        [TestMethod]
        public void Write_CatchHandlerWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("TryCatch.cs", 30, 4, 30, 9);
            var emptyBlock = new ScopeBlock(null, scope, new List<Statement>());

            var handler = new CatchHandler(scope, catchIdentifier: null, catchBlock: emptyBlock, location: location);
            writer.Write(handler);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("TryCatch.cs", sourceLine: 29),
                "CatchHandler Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2c: the Location on <see cref="TryCatchFinalyBlock"/> must reach the
        /// map when the block is written through the dispatcher. Mirrors the
        /// CatchHandler end-to-end test so both new-Location parameters are covered.
        /// </summary>
        [TestMethod]
        public void Write_TryCatchFinalyBlockWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("TryBlock.cs", 50, 4, 50, 7);
            var tryBody = new ScopeBlock(null, scope, new List<Statement>());

            var block = new TryCatchFinalyBlock(
                scope,
                tryBody,
                catchHandler: null,
                finallyStatement: null,
                location: location);
            writer.Write(block);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("TryBlock.cs", sourceLine: 49),
                "TryCatchFinalyBlock Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2d: variable-declaration blocks (`int x = 1, y = 2;` in C# → a
        /// `var`-block in JS) must carry a Location so debuggers can step into
        /// the declaration line. This test covers <see cref="VarInitializerStatement"/>
        /// (the hoisted-declaration form used by NScript for `var x, y, z;`
        /// emissions) going through the dispatcher.
        /// </summary>
        [TestMethod]
        public void Write_VarInitializerStatementWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Vars.cs", 12, 0, 12, 20);
            var ident = new IdentifierExpression(
                SimpleIdentifier.CreateScopeIdentifier(scope, "x", true),
                scope);

            var stmt = new VarInitializerStatement(
                location,
                scope,
                new List<Expression> { ident });
            writer.Write(stmt);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Vars.cs", sourceLine: 11),
                "VarInitializerStatement Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2d: the plain <see cref="InitializerStatement"/> (used by
        /// <c>BondToAst.ParseVariableInitializers</c> for C# multi-variable
        /// declarations like `int x = 1, y = 2;`) must also propagate Location.
        /// Pairs with the Roslyn-side fix that now sets `Location` on
        /// <c>VariableBlockDeclaration</c> in <c>BoundAstToAstBase</c>.
        /// </summary>
        [TestMethod]
        public void Write_InitializerStatementWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Multi.cs", 25, 0, 25, 30);
            var ident = new IdentifierExpression(
                SimpleIdentifier.CreateScopeIdentifier(scope, "y", true),
                scope);

            var stmt = new InitializerStatement(
                location,
                scope,
                new List<Expression> { ident });
            writer.Write(stmt);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Multi.cs", sourceLine: 24),
                "InitializerStatement Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2a: <see cref="BooleanLiteralExpression"/> must carry a Location.
        /// Parallel to <c>Write_NumberLiteralWithLocation_MapsToSource</c> for
        /// the other primitive literal types added in Phase 2.
        /// </summary>
        [TestMethod]
        public void Write_BooleanLiteralWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Bool.cs", 4, 10, 4, 14);

            var literal = new BooleanLiteralExpression(scope, true, location);
            writer.Write(literal);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Bool.cs", sourceLine: 3),
                "BooleanLiteral Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2a: <see cref="DoubleLiteralExpression"/> must carry a Location.
        /// </summary>
        [TestMethod]
        public void Write_DoubleLiteralWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Dbl.cs", 6, 0, 6, 5);

            var literal = new DoubleLiteralExpression(scope, 3.14, location);
            writer.Write(literal);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Dbl.cs", sourceLine: 5),
                "DoubleLiteral Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2a: <see cref="NullLiteralExpression"/> must carry a Location.
        /// </summary>
        [TestMethod]
        public void Write_NullLiteralWithLocation_MapsToSource()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var scope = new IdentifierScope(isExecutionScope: true);
            var location = new Location("Null.cs", 9, 2, 9, 6);

            var literal = new NullLiteralExpression(scope, location);
            writer.Write(literal);

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMapView.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Null.cs", sourceLine: 8),
                "NullLiteral Location should surface in map. Map:\n" + map.ToString());
        }

        /// <summary>
        /// Phase 2 regression guard: <see cref="TransformerVisitorExtension"/>
        /// clones nodes during optimizer passes (e.g., <c>ProxyFixer</c>,
        /// <c>MethodNameRemover</c>). If the clone drops Location, a release build
        /// with `-optimize` loses all literal / catch-handler mappings silently.
        /// This test runs every literal through the transformer's default visit
        /// (via <see cref="IdentityTransformer"/>) and asserts Location survives.
        /// </summary>
        [TestMethod]
        public void Transformer_ClonesLiteralsAndCatch_PreservesLocation()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            ITransformerVisitor transformer = new IdentityTransformer();

            var numLoc = new Location("N.cs", 1, 0, 1, 1);
            var num = (NumberLiteralExpression)transformer.VisitNumberLiteralExpression(
                new NumberLiteralExpression(scope, 1, numLoc));
            Assert.AreSame(numLoc, num.Location, "NumberLiteral Location must survive transformer clone.");

            var strLoc = new Location("S.cs", 2, 0, 2, 1);
            var str = (StringLiteralExpression)transformer.VisitStringLiteralExpression(
                new StringLiteralExpression(scope, "hi", strLoc));
            Assert.AreSame(strLoc, str.Location, "StringLiteral Location must survive transformer clone.");

            var boolLoc = new Location("B.cs", 3, 0, 3, 1);
            var b = (BooleanLiteralExpression)transformer.VisitBooleanLiteralExpression(
                new BooleanLiteralExpression(scope, false, boolLoc));
            Assert.AreSame(boolLoc, b.Location, "BooleanLiteral Location must survive transformer clone.");

            var dblLoc = new Location("D.cs", 4, 0, 4, 1);
            var d = (DoubleLiteralExpression)transformer.VisitDoubleLiteralExpression(
                new DoubleLiteralExpression(scope, 2.5, dblLoc));
            Assert.AreSame(dblLoc, d.Location, "DoubleLiteral Location must survive transformer clone.");

            var nullLoc = new Location("Z.cs", 5, 0, 5, 1);
            var n = (NullLiteralExpression)transformer.VisitNullLiteralExpression(
                new NullLiteralExpression(scope, nullLoc));
            Assert.AreSame(nullLoc, n.Location, "NullLiteral Location must survive transformer clone.");

            var catchLoc = new Location("C.cs", 6, 0, 6, 1);
            var emptyBlock = new ScopeBlock(null, scope, new List<Statement>());
            var catchHandler = new CatchHandler(scope, null, emptyBlock, catchLoc);
            var clonedCatch = (CatchHandler)transformer.VisitCatchHandler(catchHandler);
            Assert.AreSame(catchLoc, clonedCatch.Location, "CatchHandler Location must survive transformer clone.");

            var tryLoc = new Location("T.cs", 7, 0, 7, 1);
            var tryBlock = new TryCatchFinalyBlock(scope, emptyBlock, null, null, tryLoc);
            var clonedTry = (TryCatchFinalyBlock)transformer.VisitTryCatchFinallyBlock(tryBlock);
            Assert.AreSame(tryLoc, clonedTry.Location, "TryCatchFinalyBlock Location must survive transformer clone.");
        }

        /// <summary>
        /// Identity transformer: accepts the <see cref="ITransformerVisitor"/>
        /// default implementations (which delegate to the extension methods).
        /// Used to exercise the clone paths without customizing any visit.
        /// </summary>
        private sealed class IdentityTransformer : ITransformerVisitor
        {
        }

        /// <summary>
        /// Lightweight duplicate of the DecodedMap parser from
        /// <see cref="JSWriterIntegrationTests"/> — kept local so Phase 2 tests can
        /// evolve independently without touching the Phase 1 harness.
        /// </summary>
        private sealed class DecodedMapView
        {
            public List<string> Sources { get; } = new List<string>();

            public List<Mapping> Mappings { get; } = new List<Mapping>();

            public bool HasMappingFor(string sourceFileSuffix, int sourceLine)
            {
                foreach (var m in this.Mappings)
                {
                    if (m.SourceFileIndex >= 0
                        && m.SourceFileIndex < this.Sources.Count
                        && this.Sources[m.SourceFileIndex].EndsWith(sourceFileSuffix, System.StringComparison.Ordinal)
                        && m.SourceLine == sourceLine)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static DecodedMapView Parse(string json)
            {
                var result = new DecodedMapView();

                const string sourcesLongMarker = "\"sourcesLong\": [\"";
                int srcStart = json.IndexOf(sourcesLongMarker, System.StringComparison.Ordinal);
                if (srcStart >= 0)
                {
                    srcStart += sourcesLongMarker.Length;
                    int srcEnd = json.IndexOf("\"]", srcStart, System.StringComparison.Ordinal);
                    Assert.IsTrue(
                        srcEnd > srcStart,
                        "sourcesLong array was not terminated with '\"]' — source-map JSON format may have changed.");
                    string inside = json.Substring(srcStart, srcEnd - srcStart);
                    foreach (string s in inside.Split(new[] { "\",\n\t\t\"" }, System.StringSplitOptions.None))
                    {
                        result.Sources.Add(s);
                    }
                }

                const string mappingsMarker = "\"mappings\": \"";
                int mapStart = json.IndexOf(mappingsMarker, System.StringComparison.Ordinal);
                if (mapStart < 0) return result;
                mapStart += mappingsMarker.Length;
                int mapEnd = json.IndexOf('"', mapStart);
                Assert.IsTrue(
                    mapEnd > mapStart,
                    "mappings field was not terminated — source-map JSON format may have changed.");
                string mappings = json.Substring(mapStart, mapEnd - mapStart);

                int sourceLine = 0;
                int sourceCol = 0;
                int sourceFileIndex = 0;
                int nameIndex = 0;
                int generatedLine = 0;

                foreach (string line in mappings.Split(';'))
                {
                    int generatedCol = 0;
                    foreach (string segment in line.Split(','))
                    {
                        if (segment.Length == 0) continue;

                        int pos = 0;
                        int genColDelta = Base64VLQDecoder.Decode(segment, ref pos);
                        generatedCol += genColDelta;

                        var mapping = new Mapping
                        {
                            GeneratedLine = generatedLine,
                            GeneratedColumn = generatedCol,
                            SourceFileIndex = -1,
                        };

                        if (pos < segment.Length)
                        {
                            sourceFileIndex += Base64VLQDecoder.Decode(segment, ref pos);
                            sourceLine += Base64VLQDecoder.Decode(segment, ref pos);
                            sourceCol += Base64VLQDecoder.Decode(segment, ref pos);

                            mapping.SourceFileIndex = sourceFileIndex;
                            mapping.SourceLine = sourceLine;
                            mapping.SourceColumn = sourceCol;

                            if (pos < segment.Length)
                            {
                                nameIndex += Base64VLQDecoder.Decode(segment, ref pos);
                                mapping.NameIndex = nameIndex;
                            }
                            else
                            {
                                mapping.NameIndex = -1;
                            }
                        }

                        result.Mappings.Add(mapping);
                    }
                    generatedLine++;
                }

                return result;
            }
        }

        private sealed class Mapping
        {
            public int GeneratedLine;
            public int GeneratedColumn;
            public int SourceFileIndex = -1;
            public int SourceLine;
            public int SourceColumn;
            public int NameIndex = -1;
        }
    }
}
