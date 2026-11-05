using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin.CodeGen;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// PR 149 P5: the one @foreach scoping rule the emitter and the plugin's method retention
    /// share. Behaviour through real templates is covered by the UI suite (AwbFuOuterItemHandler).
    /// </summary>
    [TestClass]
    public class EnclosingLoopsTests
    {
        private static readonly IReadOnlyList<(string Variable, string TypeName)> GroupThenTab =
            new List<(string, string)> { ("group", "Group"), ("tab", "Tab") };

        [DataTestMethod]
        [DataRow("tab.Pick", "row", 1)]
        [DataRow("group.Pick", "row", 0)]
        [DataRow("group.Child.Pick", "row", 0)]
        [DataRow("tab.Pick", "tab", -1)]      // the current loop's own variable is not enclosing
        [DataRow("Model.Pick", "row", -1)]
        [DataRow("other.Pick", "row", -1)]
        [DataRow("tab", "row", -1)]           // no member: not outer.M
        [DataRow("(e) => tab.Pick()", "row", -1)]
        public void IndexOfRoot_FindsTheEnclosingLoopThatDeclaresTheRoot(
            string expression, string currentLoopVariable, int expected)
        {
            EnclosingLoops.IndexOfRoot(GroupThenTab, expression, currentLoopVariable).Should().Be(expected);
        }

        [TestMethod]
        public void IndexOf_PrefersTheInnermostLoopAndToleratesNoLoops()
        {
            var shadowed = new List<(string, string)> { ("x", "Outer"), ("x", "Inner") };

            EnclosingLoops.IndexOf(shadowed, "x").Should().Be(1);
            EnclosingLoops.IndexOf(null, "x").Should().Be(-1);
            EnclosingLoops.IndexOfRoot(null, "x.M", null).Should().Be(-1);
        }

        [TestMethod]
        public void Enter_AddsTheLoopInnermostAndLeavesTheTemplateRootAlone()
        {
            EnclosingLoops.Enter(GroupThenTab, null, "Row").Should().BeSameAs(GroupThenTab);
            EnclosingLoops.Enter(null, "", "Row").Should().BeNull();

            var inner = EnclosingLoops.Enter(GroupThenTab, "row", "Row");

            inner.Should().Equal(("group", "Group"), ("tab", "Tab"), ("row", "Row"));
            GroupThenTab.Should().HaveCount(2, "the outer list is not changed");
        }
    }
}
