using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.CLR.AST;
using Mono.Cecil;

namespace NScript.CLR.Decompiler.Blocks
{
    internal class RootBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        private RootBlock(InstructionBlock firstBlock)
            : base(firstBlock)
        {
        }

        public static RootBlock Create(
            IList<IlInstruction> instructions,
            IDictionary<string, IlInstruction> labelToInstruction,
            MethodDefinition methodDefinition,
            ClrContext clrContext)
        {
            HashSet<string> blockStarts;
            List<InstructionBlock> instructionBlocks;

            var returnValue = RootBlock.CreateInternal(
                instructions,
                labelToInstruction,
                out blockStarts,
                out instructionBlocks,
                methodDefinition,
                clrContext);

            return returnValue;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public override void InsertInstruction(IlInstruction instruction)
        {
            if (instruction.Index < this.BlockStartIndex ||
                instruction.Index > (this.BlockEndIndex + 1))
            {
                throw new InvalidOperationException();
            }

            int insertionIndex = -1;
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i].BlockStartIndex == instruction.Index)
                {
                    insertionIndex = i;
                    break;
                }

                if (this.Children[i].BlockStartIndex < instruction.Index &&
                    this.Children[i].BlockEndIndex >= instruction.Index)
                {
                    this.Children[i].InsertInstruction(instruction);
                    return;
                }
            }

            var instructionBlock = new InstructionBlock(
                this.Context,
                instruction);

            var instructionBlockCollection = new InstructionBlockCollection(
                instructionBlock);

            this.AddChildAt(
                instructionBlockCollection,
                insertionIndex);
        }

        public void ProccessThroughPipeline(params Action<RootBlock>[] pipelineDelegates)
        {
            for (int i = 0; i < pipelineDelegates.Length; i++)
            {
                pipelineDelegates[i](this);
            }
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(
            VariableResolver variableResolver)
        {
            ParameterBlock returnValue = new ParameterBlock(
                this.Context.ClrContext,
                this.ComputeLocation());

            variableResolver.PushScope(returnValue);

            for (int childIndex = 0; childIndex < this.Children.Count; childIndex++)
            {
                Node node = this.Children[childIndex].ToAstNode(variableResolver);

                if (node is Expression)
                {
                    returnValue.AddStatement(
                        new ExpressionStatement(
                            (Expression)node));
                }
                else if (node is ReturnStatement)
                {
                    if (((ReturnStatement)node).ReturnExpression != null)
                    {
                        returnValue.AddStatement((Statement)node);
                    }

                    // We don't need to add any more statements, since return is the last
                    // and can't continue. Only possiblity of any further move is if we
                    // have goto, but we are not supporting it right now.
                    break;
                }
                else if (node != null)
                {
                    returnValue.AddStatement(
                        (Statement)node);
                }
            }

            variableResolver.PopScope();

            return returnValue;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        protected override bool IsContigious(Block block)
        {
            return true;
        }

        /// <summary>
        /// Creates the internal.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        /// <param name="labelToInstruction">The label to instruction.</param>
        /// <param name="blockStarts">The block starts.</param>
        /// <param name="instructionBlocks">The instruction blocks.</param>
        /// <param name="methodDefinition">The method definition.</param>
        /// <param name="clrContext">The CLR context.</param>
        /// <returns>Root block.</returns>
        private static RootBlock CreateInternal(
            IList<IlInstruction> instructions,
            IDictionary<string, IlInstruction> labelToInstruction,
            out HashSet<string> blockStarts,
            out List<InstructionBlock> instructionBlocks,
            MethodDefinition methodDefinition,
            ClrContext clrContext)
        {
            BlockContext context = new BlockContext(
                clrContext,
                methodDefinition,
                labelToInstruction,
                new Dictionary<int, Block>(),
                new Dictionary<IlInstruction, InstructionBlock>(),
                instructions,
                new List<Block>());

            blockStarts = new HashSet<string>();
            instructionBlocks = new List<InstructionBlock>();

            foreach (var instruction in instructions)
            {
                switch (instruction.FlowType)
                {
                    case FlowType.Branch:
                    case FlowType.Leave:
                    case FlowType.ConditionalBranch:
                        if (!blockStarts.Contains((string)instruction.OpCodeArgument))
                        {
                            blockStarts.Add((string)instruction.OpCodeArgument);
                        }
                        if (instruction.Next != null &&
                            !blockStarts.Contains(instruction.Next.Label))
                        {
                            blockStarts.Add(instruction.Next.Label);
                        }
                        break;
                    case FlowType.Return:
                        if (instruction.Next != null &&
                            !blockStarts.Contains(instruction.Next.Label))
                        {
                            blockStarts.Add(instruction.Next.Label);
                        }
                        break;
                    case FlowType.Switch:
                        foreach (var label in (string[])instruction.OpCodeArgument)
                        {
                            if (!blockStarts.Contains(label))
                            {
                                blockStarts.Add(label);
                            }
                        }
                        if (instruction.Next != null &&
                            !blockStarts.Contains(instruction.Next.Label))
                        {
                            blockStarts.Add(instruction.Next.Label);
                        }
                        break;
                    case FlowType.Unsuported:
                        throw new NotSupportedException();
                }

                if (instruction.FlowType == FlowType.ConditionalBranch)
                {
                    var previousInstruction = instruction.Previous;

                    while (previousInstruction != null &&
                        previousInstruction.StackLevelBefore != instruction.StackLevelAfter)
                    {
                        previousInstruction = previousInstruction.Previous;
                    }

                    if (previousInstruction != null)
                    {
                        blockStarts.Add(previousInstruction.Label);
                    }
                }

                if (instruction.StackLevelBefore == 0)
                {
                    if (!blockStarts.Contains(instruction.Label))
                    {
                        blockStarts.Add(instruction.Label);
                    }
                }

                instructionBlocks.Add(new InstructionBlock(
                    context,
                    instruction.Index));

                context.InstructionToBlock.Add(
                    instruction,
                    instructionBlocks[instructionBlocks.Count - 1]);

                context.IdToBlock.Add(
                    instructionBlocks[instructionBlocks.Count - 1].Id,
                    instructionBlocks[instructionBlocks.Count - 1]);
            }


            RootBlock returnValue = new RootBlock(instructionBlocks[0]);

            for (int i = 1; i < instructionBlocks.Count; i++)
            {
                returnValue.AddChild(instructionBlocks[i]);
            }

            return returnValue;
        }
        #endregion
    }
}
