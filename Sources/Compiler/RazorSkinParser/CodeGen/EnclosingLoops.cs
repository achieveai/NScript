using System;
using System.Collections.Generic;
using System.Linq;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// The @foreach loops around a loop body, outermost first, as (loop variable, item type
    /// name). One scoping rule for the emitter (GraphDescriptorJSTEmitter) and the method
    /// retention in RazorTemplatingPlugin, so the code that calls <c>outer.M</c> and the code
    /// that keeps <c>M</c> agree on which loop <c>outer</c> names.
    /// </summary>
    internal static class EnclosingLoops
    {
        /// <summary>
        /// The loops an inner loop body sees: <paramref name="loops"/> plus the loop whose body
        /// it is. Unchanged when <paramref name="variable"/> is empty (the template root).
        /// </summary>
        internal static IReadOnlyList<(string Variable, string TypeName)> Enter(
            IReadOnlyList<(string Variable, string TypeName)> loops, string variable, string typeName)
        {
            if (string.IsNullOrEmpty(variable)) return loops;
            return (loops ?? Array.Empty<(string, string)>()).Append((variable, typeName)).ToList();
        }

        /// <summary>The innermost loop that declares <paramref name="variable"/>, or -1.</summary>
        internal static int IndexOf(IReadOnlyList<(string Variable, string TypeName)> loops, string variable)
        {
            for (int i = (loops?.Count ?? 0) - 1; i >= 0; i--)
                if (loops[i].Variable == variable) return i;
            return -1;
        }

        /// <summary>
        /// The innermost enclosing loop whose variable is the root of <paramref name="expression"/>
        /// (<c>outer</c> in <c>outer.M</c>), or -1. The current loop's own variable
        /// (<paramref name="currentLoopVariable"/>) is not an enclosing loop.
        /// </summary>
        internal static int IndexOfRoot(IReadOnlyList<(string Variable, string TypeName)> loops,
            string expression, string currentLoopVariable)
        {
            var dot = expression?.IndexOf('.') ?? -1;
            if (dot <= 0) return -1;
            var root = expression.Substring(0, dot);
            return root == currentLoopVariable ? -1 : IndexOf(loops, root);
        }
    }
}
