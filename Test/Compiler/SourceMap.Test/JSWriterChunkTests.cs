using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.Utils;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Slice 2, Inc 2: a chunk-flagged function is rendered on its own at depth 0 and spliced
    /// back with re-indentation and rebased map segments. The output must equal rendering the
    /// same function in place, byte for byte, for the .js and the .map (design section 6).
    /// </summary>
    [TestClass]
    public class JSWriterChunkTests
    {
        private const string File = "Chunks.cs";

        private static Location At(int line, int column, int length = 4)
            => new Location(File, line, column, line, column + length);

        private static IdentifierExpression Use(SimpleIdentifier identifier, IdentifierScope scope, Location location = null)
            => new IdentifierExpression(identifier, scope, location);

        private static ExpressionStatement Assign(IdentifierScope scope, Expression target, Expression value, Location location)
            => new ExpressionStatement(
                location,
                scope,
                new BinaryExpression(location, scope, BinaryOperator.Assignment, target, value));

        private static FunctionExpression Function(
            IdentifierScope outer,
            string name,
            Location location,
            System.Func<IdentifierScope, SimpleIdentifier, List<Statement>> body)
        {
            var inner = new IdentifierScope(outer, new List<string> { "arg" }, false);
            var nameIdentifier = name == null
                ? null
                : SimpleIdentifier.CreateScopeIdentifier(outer, name, false);
            var function = new FunctionExpression(location, outer, inner, inner.ParameterIdentifiers, nameIdentifier);
            function.AddStatements(body(inner, (SimpleIdentifier)inner.ParameterIdentifiers[0]));
            return function;
        }

        private static List<Statement> Returns(IdentifierScope scope, Expression value, Location location)
            => new List<Statement> { new ReturnStatement(location, scope, value) };

        /// <summary>
        /// Functions at depth 0, 1 and 2; after '=', ':', '(' and 'return'; one empty; one without
        /// a location; one nested in another; and one with a raw-newline script literal.
        /// </summary>
        private static (List<Statement> program, List<FunctionExpression> functions, IdentifierScope root) Build()
        {
            var root = new IdentifierScope(isExecutionScope: true);
            var functions = new List<FunctionExpression>();
            var program = new List<Statement>();
            var x = SimpleIdentifier.CreateScopeIdentifier(root, "counterValue", false);
            var call = SimpleIdentifier.CreateScopeIdentifier(root, "log", true);

            // f1 = function f1(arg) { var total; if (arg) { total = arg + 1; } return total; };
            var f1 = Function(root, "firstFunction", At(10, 5), (scope, arg) =>
            {
                var total = SimpleIdentifier.CreateScopeIdentifier(scope, "total", false);
                var thenBlock = new ScopeBlock(At(12, 9), scope, new List<Statement>
                {
                    Assign(
                        scope,
                        Use(total, scope, At(13, 13)),
                        new BinaryExpression(At(13, 21), scope, BinaryOperator.Plus, Use(arg, scope, At(13, 21)), new NumberLiteralExpression(scope, 1, At(13, 27))),
                        At(13, 13, 16)),
                });
                return new List<Statement>
                {
                    new IfBlockStatement(At(12, 9), scope, Use(arg, scope, At(12, 13)), thenBlock, null),
                    new ReturnStatement(At(15, 9), scope, Use(total, scope, At(15, 16))),
                };
            });
            functions.Add(f1);
            program.Add(Assign(root, Use(x, root, At(9, 1)), f1, At(9, 1, 30)));

            // x = { m: function(arg) { return arg; }, n: function() {} , o: <no location> };
            var obj = new InlineObjectInitializer(At(20, 5), root);
            var m = Function(root, null, At(21, 12), (scope, arg) => Returns(scope, Use(arg, scope, At(21, 30)), At(21, 23)));
            var n = Function(root, null, At(22, 12), (scope, arg) => new List<Statement>());
            var o = Function(root, null, null, (scope, arg) => Returns(scope, Use(arg, scope), null));
            functions.AddRange(new[] { m, n, o });
            obj.AddInitializer("m", m);
            obj.AddInitializer("n", n);
            obj.AddInitializer("o", o);
            program.Add(Assign(root, Use(x, root, At(20, 1)), obj, At(20, 1, 40)));

            // x = function rawFunction(arg) { <raw literal with a CRLF> };  -- inside the if
            // block (depth 1), where splicing would indent the literal's second line.
            var raw = Function(root, "rawFunction", At(40, 5), (scope, arg) => new List<Statement>
            {
                new ExpressionStatement(At(41, 9), scope, new ScriptLiteralExpression(At(41, 9), scope, "first();\r\nsecond()")),
            });
            functions.Add(raw);

            // if (x) { log(function(arg) { return function(arg) { return arg; }; }, 1); x = raw; }
            var outer = Function(root, null, At(31, 13), (scope, arg) =>
            {
                var nested = Function(scope, null, At(32, 20), (nestedScope, nestedArg) =>
                    Returns(nestedScope, Use(nestedArg, nestedScope, At(32, 40)), At(32, 33)));
                functions.Add(nested);
                return Returns(scope, nested, At(32, 13));
            });
            functions.Add(outer);
            program.Add(new IfBlockStatement(
                At(30, 1),
                root,
                Use(x, root, At(30, 5)),
                new ScopeBlock(At(30, 8), root, new List<Statement>
                {
                    new ExpressionStatement(
                        At(31, 5),
                        root,
                        new MethodCallExpression(At(31, 5), root, Use(call, root, At(31, 5)), outer, new NumberLiteralExpression(root, 1, At(33, 8)))),
                    Assign(root, Use(x, root, At(40, 1)), raw, At(40, 1, 30)),
                }),
                null));

            return (program, functions, root);
        }

        private static (string js, string map, IReadOnlyList<JSWriter.ChunkInfo> chunks, int fallbacks) Write(bool chunks)
        {
            var (program, functions, root) = Build();
            IdentifierScope.IdentifierMinifiedNamer.MinifyNames(root, false);
            foreach (var function in functions)
            {
                function.IsChunk = chunks;
            }

            var writer = new JSWriter(true, false);
            foreach (var statement in program)
            {
                writer.Write(statement);
            }

            using var text = new StringWriter();
            var map = writer.WriteWithMap(text, "out.js");
            return (text.ToString(), map.ToString(), writer.Chunks, writer.ChunkFallbacks);
        }

        [TestMethod]
        public void Chunks_GiveTheSameJsAndMapAsInPlaceRendering()
        {
            var inPlace = Write(chunks: false);
            var chunked = Write(chunks: true);

            Assert.AreEqual(0, inPlace.chunks.Count);
            Assert.AreEqual(inPlace.js, chunked.js);
            Assert.AreEqual(inPlace.map, chunked.map);

            // The nested function renders inside its parent chunk, and the raw-newline function
            // falls back to in-place writing, so 5 of the 7 are chunks.
            Assert.AreEqual(5, chunked.chunks.Count, string.Join(", ", chunked.chunks.Select(c => c.Name)));
            Assert.AreEqual(1, chunked.fallbacks, "Only the raw-newline function falls back.");
            Assert.IsFalse(chunked.chunks.Any(c => c.Name == "rawFunction"));
            Assert.IsTrue(chunked.chunks.Any(c => c.LineCount > 1), "Expected multi-line chunks.");
        }

        [TestMethod]
        public void Chunks_SpliceAtEveryDepth()
        {
            var chunked = Write(chunks: true);

            // depth 0 (f1), depth 1 (object members), depth 1 inside if (log argument).
            StringAssert.Contains(chunked.js, "\r\n    return ");
            StringAssert.Contains(chunked.js, "\r\n      return ");
            Assert.IsTrue(chunked.chunks.Select(c => c.StartLine).Distinct().Count() > 3);
        }
    }
}
