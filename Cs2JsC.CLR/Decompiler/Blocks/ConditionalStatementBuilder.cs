using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    internal class ConditionalStatementBuilder
    {
        public static void Process(
            Block block)
        {
            for (int i = 0; i < block.Children.Count; i++)
            {
                ConditionalStatementBuilder.Process(block.Children[i]);
            }

            for (int i = 0; i < block.Children.Count; i++)
            {
                if (ConditionalStatementBuilder.Create(
                    block,
                    i) != null)
                {
                    // If we just created new ConditionalStatement, move back i by
                    // 2 so that we can retry to combine previous statement and this
                    // one.
                    //
                    i -= 2;

                    // if i == 0, then next iteration will have i == -1, so let's
                    // pickup i to be -1 or greater.
                    //
                    i = Math.Max(-1, i);
                }
            }
        }

        private static ConditionalStatement Create(
            Block block,
            int index)
        {
            // Currently it only makes sense for us to create conditional statements
            // within conditional blocks.
            //
            if (index >= block.Children.Count-1)
            { return null; }

            if (ConditionalStatementBuilder.IsConditionalChild(block.Children[index]) &&
                ConditionalStatementBuilder.IsConditionalChild(block.Children[index + 1]))
            {
                Block b1 = block.Children[index];
                Block b2 = block.Children[index + 1];
                Block b1s1 = b1.LogicalSuccessors[0];
                Block b1s2 = b1.LogicalSuccessors[1];
                Block b2s1 = b2.LogicalSuccessors[0];
                Block b2s2 = b2.LogicalSuccessors[1];
                // For to conditional child to combine, their combination should result in
                // exactly 2 successors.
                //
                if ((b1s1 == b2 && (b1s2 == b2s1 || b1s2 == b2s2))
                    || (b1s2 == b2 && (b1s1 == b2s1 || b1s1 == b2s2)))
                {
                    return new ConditionalStatement(
                        block.Children[index],
                        block.Children[index + 1]);
                }
            }

            return null;
        }

        internal static bool IsConditionalChild(
            Block block)
        {
            var stackedBlock = block as StackedBlock;

            if (stackedBlock != null &&
                stackedBlock.RootBlock.Instruction.FlowType == FlowType.ConditionalBranch)
            {
                return true;
            }

            return block is ConditionalStatement;
        }
    }
}
