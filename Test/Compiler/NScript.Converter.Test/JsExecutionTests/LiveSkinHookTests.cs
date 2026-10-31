//-----------------------------------------------------------------------
// <copyright file="LiveSkinHookTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using System.Collections.Generic;
    using Microsoft.ClearScript.V8;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.JST;

    /// <summary>
    /// The live skin helpers (<see cref="LiveSkinHook"/>) run in V8 against a small model of
    /// skinned controls: an outer skin whose factory makes rows that show an inner skin.
    /// </summary>
    [TestClass]
    public class LiveSkinHookTests
    {
        // A control is {e: element, k: its instance}; an instance is {p: its control, kids}.
        // A skin is {f: factory, id}. set(c, v) is the Skin setter: the old instance goes,
        // and with it the rows it made, as SkinInstance.Dispose disposes its child controls.
        // The outer factory makes its rows before it returns, so a row is tracked before its outer.
        private const string Model = @"
var builds = {}, logs = [];
var console = { info: function(m) { logs.push(m); } };
var G = {};
function set(c, v) {
  if (c.k) { var old = c.k; c.k = null; old.kids.forEach(function(k) { set(k, null); }); }
  if (v) { var si = G.__nsTrack(v.f(c), v); si.p = c; c.k = si; }
}
function factory(tag, inner, rows) {
  return function(c) {
    builds[tag] = (builds[tag] || 0) + 1;
    var si = { tag: tag, kids: [] };
    for (var i = 0; i < (rows || 0); i++) { var k = { e: { parentNode: c.e } }; si.kids.push(k); set(k, inner); }
    return si;
  };
}
var inner = { f: factory('inner1'), id: 'i1' };
var outer = { f: factory('outer1', inner, 3), id: 'o1' };
var root = { e: { parentNode: { parentNode: null } } };
";

        [TestMethod]
        public void OuterAndInnerSaved_EachRowIsBuiltOnce_WithTheNewInnerSkin()
        {
            foreach (var innerFirst in new[] { false, true })
            {
                using var engine = Run(innerFirst
                    ? "G.__nsReskin(inner, factory('inner2'), 'i2', 'Inner'); G.__nsReskin(outer, factory('outer2', inner, 3), 'o2', 'Outer');"
                    : "G.__nsReskin(outer, factory('outer2', inner, 3), 'o2', 'Outer'); G.__nsReskin(inner, factory('inner2'), 'i2', 'Inner');");

                Assert.AreEqual("{\"outer2\":1,\"inner2\":3}", engine.Evaluate("JSON.stringify(builds)"), "inner first: " + innerFirst);
                Assert.AreEqual("inner2,inner2,inner2", engine.Evaluate("root.k.kids.map(function(k) { return k.k.tag; }).join()"));
                Assert.AreEqual(
                    "[nscript-hmr] reskinned 1 Outer (reset: focus, caret, scroll, unbound typed text)|"
                    + "[nscript-hmr] reskinned 3 Inner (reset: focus, caret, scroll, unbound typed text) (3 by an outer skin)",
                    engine.Evaluate(innerFirst ? "logs.reverse().join('|')" : "logs.join('|')"));

                // The next save finds the rows the flush made.
                engine.Execute("builds = {}; G.__nsReskin(inner, factory('inner3'), 'i3', 'Inner');" + Flush());
                Assert.AreEqual("{\"inner3\":3}", engine.Evaluate("JSON.stringify(builds)"));
            }
        }

        [TestMethod]
        public void InnerSaved_RebuildsEachOpenRow()
        {
            using var engine = Run("G.__nsReskin(inner, factory('inner2'), 'i2', 'Inner');");
            Assert.AreEqual("{\"inner2\":3}", engine.Evaluate("JSON.stringify(builds)"));
            Assert.AreEqual("[nscript-hmr] reskinned 3 Inner (reset: focus, caret, scroll, unbound typed text)", engine.Evaluate("logs.join('|')"));
            Assert.AreEqual("inner2,inner2,inner2", engine.Evaluate("root.k.kids.map(function(k) { return k.k.tag; }).join()"));
        }

        [TestMethod]
        public void SkinWithNoOpenControl_SaysSoAndBuildsNothing()
        {
            using var engine = Run("G.__nsReskin({ f: null, id: 'z' }, factory('other'), 'z2', 'Other');");
            Assert.AreEqual("{}", engine.Evaluate("JSON.stringify(builds)"));
            Assert.AreEqual("[nscript-hmr] reskinned 0 Other", engine.Evaluate("logs.join('|')"));
        }

        /// <summary>The page skins its root, then a patch runs <paramref name="patch"/> and the flush.</summary>
        private static V8ScriptEngine Run(string patch)
        {
            var engine = new V8ScriptEngine();
            engine.Execute(Model + Write(Helpers()) + "set(root, outer); builds = {};");
            engine.Execute(patch + Flush());
            return engine;
        }

        private static List<Statement> Helpers()
        {
            var root = new IdentifierScope(true);
            IIdentifier Name(string name) => SimpleIdentifier.CreateScopeIdentifier(root, name, true);
            return LiveSkinHook.CreateHelpers(
                root,
                Name("G"),
                Name("f"),
                Name("id"),
                Name("p"),
                Name("k"),
                Name("e"),
                new ScriptLiteralExpression(null, root, "set"));
        }

        private static string Flush()
            => Write(new List<Statement> { LiveSkinHook.CreateFlush(new IdentifierScope(true)) });

        private static string Write(List<Statement> statements)
            => ConverterTestHelpers.GetScriptString(statements) + "\n";
    }
}
