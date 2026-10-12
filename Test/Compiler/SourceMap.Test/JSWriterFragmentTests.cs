using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.Utils;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Dev patch emit: a fragment written into an already written script, before a type's
    /// marker, must give the script and map that rendering the fragment's statements there in
    /// the first place gives. The map includes the mappings of the script into itself.
    /// </summary>
    [TestClass]
    public class JSWriterFragmentTests
    {
        private const string File = "Fragment.cs";

        private static Location At(int line, int column, int length = 4)
            => new Location(File, line, column, line, column + length);

        /// <summary><c>counterValue = function name(arg) { if (arg) { arg = 1; } return arg; };</c> over source lines from <paramref name="line"/>.</summary>
        private static Statement AssignFunction(IdentifierScope root, SimpleIdentifier target, string name, int line)
        {
            var inner = new IdentifierScope(root, new List<string> { "arg" }, false);
            var arg = (SimpleIdentifier)inner.ParameterIdentifiers[0];
            var function = new FunctionExpression(
                At(line, 5),
                root,
                inner,
                inner.ParameterIdentifiers,
                SimpleIdentifier.CreateScopeIdentifier(root, name, false));
            function.AddStatements(new List<Statement>
            {
                new IfBlockStatement(
                    At(line + 1, 9),
                    inner,
                    new IdentifierExpression(arg, inner, At(line + 1, 13)),
                    new ScopeBlock(At(line + 1, 18), inner, new List<Statement>
                    {
                        new ExpressionStatement(
                            At(line + 2, 13),
                            inner,
                            new BinaryExpression(
                                At(line + 2, 13),
                                inner,
                                BinaryOperator.Assignment,
                                new IdentifierExpression(arg, inner, At(line + 2, 13)),
                                new NumberLiteralExpression(inner, 1, At(line + 2, 19)))),
                    }),
                    null),
                new ReturnStatement(At(line + 4, 9), inner, new IdentifierExpression(arg, inner, At(line + 4, 16))),
            });
            return new ExpressionStatement(
                At(line, 1),
                root,
                new BinaryExpression(At(line, 1), root, BinaryOperator.Assignment, new IdentifierExpression(target, root, At(line, 1)), function));
        }

        /// <summary>Two typed sections, each ending at its marker, and a fragment for each.</summary>
        private static (List<Statement> program, Statement firstPatch, Statement secondPatch) Build()
        {
            var root = new IdentifierScope(isExecutionScope: true);
            var target = SimpleIdentifier.CreateScopeIdentifier(root, "counterValue", false);
            var program = new List<Statement>
            {
                AssignFunction(root, target, "firstFunction", 10),
                new MarkerStatement(root, MarkerStatement.ForType("Demo.First")),
                AssignFunction(root, target, "secondFunction", 30),
                new MarkerStatement(root, MarkerStatement.ForType("Demo.Second")),
                AssignFunction(root, target, "lastFunction", 70),
            };
            var firstPatch = AssignFunction(root, target, "firstPatched", 50);
            var secondPatch = AssignFunction(root, target, "secondPatched", 60);
            IdentifierScope.IdentifierMinifiedNamer.MinifyNames(root, false);
            return (program, firstPatch, secondPatch);
        }

        private static (string js, OwaSourceMapper.SourceMap map) Write(IEnumerable<Statement> statements)
        {
            var writer = new JSWriter(true, false);
            foreach (var statement in statements)
            {
                writer.Write(statement);
            }

            using var text = new StringWriter();
            var map = writer.WriteWithMap(text, "out.js");
            return (text.ToString(), map);
        }

        /// <summary>Writes <paramref name="patch"/> before <paramref name="marker"/>'s line as <c>DevPatchSession</c> does.</summary>
        private static string Patch(string js, OwaSourceMapper.SourceMap map, string marker, Statement patch)
        {
            int offset = js.IndexOf("\r\n" + MarkerStatement.ForType(marker), System.StringComparison.Ordinal);
            Assert.IsTrue(offset > 0, marker);
            int line = js.Substring(0, offset + 2).Split('\n').Length - 1;
            var writer = new JSWriter(true, false);
            writer.Write(patch);
            return js.Insert(offset, writer.WriteFragment(map, "out.js", line));
        }

        /// <summary>A statement whose text breaks a line where the writer does not.</summary>
        private sealed class RawLineBreakStatement : EmptyStatement
        {
            public RawLineBreakStatement(IdentifierScope scope)
                : base(null, scope)
            {
            }

            public override void Write(JSWriter writer)
                => writer.WriteNewLine().WriteIdentifier("/*a\r\nb*/");
        }

        [TestMethod]
        public void Fragment_WithARawLineBreak_IsRefused()
        {
            // Its line count, and the map below it, would be wrong: the patch emits in full.
            var root = new IdentifierScope(isExecutionScope: true);
            var writer = new JSWriter(true, false);
            writer.Write(new RawLineBreakStatement(root));
            Assert.ThrowsException<System.InvalidOperationException>(
                () => writer.WriteFragment(new OwaSourceMapper.SourceMap(), "out.js", 1));
        }

        /// <summary>A marked line <c>pageCss = "<paramref name="css"/>";</c> between two functions.</summary>
        private static List<Statement> MarkedLiteral(string css)
        {
            var root = new IdentifierScope(isExecutionScope: true);
            var target = SimpleIdentifier.CreateScopeIdentifier(root, "counterValue", false);
            var pageCss = SimpleIdentifier.CreateScopeIdentifier(root, "pageCss", false);
            var program = new List<Statement>
            {
                AssignFunction(root, target, "firstFunction", 10),
                new MarkerStatement(root, MarkerStatement.ForLiteral("Demo.Css")),
                new ExpressionStatement(
                    At(20, 1),
                    root,
                    new BinaryExpression(
                        At(20, 1),
                        root,
                        BinaryOperator.Assignment,
                        new IdentifierExpression(pageCss, root, At(20, 1)),
                        new StringLiteralExpression(root, css, At(20, 11)))),
                AssignFunction(root, target, "lastFunction", 30),
            };
            IdentifierScope.IdentifierMinifiedNamer.MinifyNames(root, false);
            return program;
        }

        [TestMethod]
        public void MarkedLiteral_WrittenAgainInPlace_GivesTheScriptAndMapOfWritingItSo()
        {
            const string changed = ".a { color: \"red\"; }\r\n.b { margin: 0; }\n";
            var expected = Write(MarkedLiteral(changed));
            var (js, map) = Write(MarkedLiteral(".a { color: blue; }"));

            Assert.AreEqual(expected.js, MarkerStatement.ReplaceLiteral(js, map, "out.js", MarkerStatement.ForLiteral("Demo.Css"), changed));
            Assert.AreEqual(expected.map.ToString(), map.ToString(), "the map's columns after the literal move with it");
            Assert.IsNull(MarkerStatement.ReplaceLiteral(js, map, "out.js", MarkerStatement.ForLiteral("Demo.Other"), changed));
        }

        [TestMethod]
        public void Fragments_GiveTheScriptAndMapOfRenderingThemInPlace()
        {
            var inPlace = Build();
            var expected = Write(new[]
            {
                inPlace.program[0], inPlace.firstPatch, inPlace.program[1],
                inPlace.program[2], inPlace.secondPatch, inPlace.program[3],
                inPlace.program[4],
            });

            var patched = Build();
            var (js, map) = Write(patched.program);
            js = Patch(js, map, "Demo.Second", patched.secondPatch);
            js = Patch(js, map, "Demo.First", patched.firstPatch);

            Assert.AreEqual(expected.js, js);
            Assert.AreEqual(expected.map.ToString(), map.ToString());
        }
    }
}
