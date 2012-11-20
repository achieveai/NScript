using System;
namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class ConditionalStatementBuilder
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
                // For to conditional child to combine, their combination should result in
                // exactly 2 successors.
                //
                if ((block.Children[index].Successors[0] == block.Children[index + 1] &&
                    (block.Children[index].Successors[1] == block.Children[index + 1].Successors[0] ||
                    block.Children[index].Successors[1] == block.Children[index + 1].Successors[1])) ||
                    (block.Children[index].Successors[1] == block.Children[index + 1] &&
                    (block.Children[index].Successors[0] == block.Children[index + 1].Successors[0] ||
                    block.Children[index].Successors[0] == block.Children[index + 1].Successors[1])))
                {
                    return new ConditionalStatement(
                        block.Children[index],
                        block.Children[index + 1]);
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether given block is conditional child or not.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns>
        /// <c>true</c> if the specified block is conditional child; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsConditionalChild(
            Block block)
        {
            var stackedBlock = block as StackedBlock;

            if (stackedBlock != null &&
                stackedBlock.RootBlock.Instruction.CodeOp.Flow == Ops.FlowType.ConditionalBranch)
            {
                return true;
            }

            return block is ConditionalStatement;
        }
    }
}
