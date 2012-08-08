//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerBlockBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil.Cil;

    /// <summary>
    /// Definition for ExceptionHandlerBlockBuilder
    /// </summary>
    internal static class ExceptionHandlerBlockBuilder
    {
        /// <summary>
        /// Processes the specified root block.
        /// </summary>
        /// <param name="rootBlock">The root block.</param>
        public static void Process(RootBlock rootBlock)
        {
            foreach (var handler in rootBlock.Context.MethodDefinition.Body.ExceptionHandlers)
            {
                if (!ExceptionHandlerBlockBuilder.ProcessInternal(
                    rootBlock,
                    handler))
                {
                    throw new InvalidProgramException("Missed creation of an exception handler block");
                }
            }
        }

        /// <summary>
        /// Processes the internal.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>true if block was created</returns>
        private static bool ProcessInternal(
            Block block,
            ExceptionHandler handler)
        {
            ExceptionHandlerBlock exceptionHandlerBlock = block as ExceptionHandlerBlock;
            if (exceptionHandlerBlock != null)
            {
                if (ExceptionHandlerBlockBuilder.ProcessInternal(
                    exceptionHandlerBlock.TryBlock,
                    handler))
                {
                    return true;
                }

                if (ExceptionHandlerBlockBuilder.ProcessInternal(
                    exceptionHandlerBlock.HandlerBlock,
                    handler))
                {
                    return true;
                }

                return false;
            }

            if (block is BasicBlock)
            {
                return false;
            }

            string tryStartLabel = handler.TryStart.Offset.ToString(),
                tryEndLabel = handler.TryEnd.Offset.ToString(),
                handlerStartLabel = handler.HandlerStart.Offset.ToString(),
                handlerEndLabel = handler.HandlerEnd.Offset.ToString();

            int tryStart = -1, tryEnd = -1, handlerStart = -1, handlerEnd = -1;

            for (int blockIndex = 0; blockIndex < block.Children.Count; blockIndex++)
            {
                Block childBlock = block.Children[blockIndex];

                if (ExceptionHandlerBlockBuilder.ProcessInternal(childBlock, handler))
                {
                    return true;
                }

                IlInstruction fistInstruction = childBlock.GetInstructionAt(0);

                if (fistInstruction.Label == tryStartLabel)
                {
                    tryStart = blockIndex;
                }
                else if (fistInstruction.Label == tryEndLabel)
                {
                    // Last block in tryBlock is block before EndLabel.
                    // EndLabel is not included in tryBlock.
                    tryEnd = blockIndex - 1;
                }

                if (fistInstruction.Label == handlerStartLabel)
                {
                    handlerStart = blockIndex;
                }
                else if (fistInstruction.Label == handlerEndLabel)
                {
                    // Last block in handlerBlock is block before EndLabel.
                    // EndLabel is not included in handlerBlock.
                    handlerEnd = blockIndex - 1;
                }
            }

            if (tryStart != -1
                && handlerEnd != -1)
            {
                if (tryEnd == -1
                    || handlerStart == -1
                    || handlerStart != tryEnd + 1)
                {
                    throw new InvalidOperationException();
                }

                Block handlerBlock =
                    new Block(
                        block.CreateChildrenArray<Block>(
                            handlerStart,
                            handlerEnd + 1));

                Block tryBlock =
                    new Block(
                        block.CreateChildrenArray<Block>(
                        tryStart,
                        handlerStart));

                new ExceptionHandlerBlock(
                    handler.HandlerType == ExceptionHandlerType.Catch,
                    handler.CatchType,
                    tryBlock,
                    handlerBlock);

                return true;
            }

            return false;
        }
    }
}
