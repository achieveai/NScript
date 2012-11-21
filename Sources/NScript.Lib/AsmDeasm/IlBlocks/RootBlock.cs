using System;
using System.Collections.Generic;
using NScript.Lib.AsmDeasm.Ops;
using NScript.Lib.MetaData;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class RootBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        private RootBlock(InstructionBlock firstBlock)
            : base(firstBlock)
        {
        }

        public static RootBlock Create(
            IList<ArgumentSignature> arguments,
            IList<ArgumentSignature> locals,
            IList<Instruction> instructions,
            IDictionary<string, Instruction> labelToInstruction)
        {
            HashSet<string> blockStarts;
            List<InstructionBlock> instructionBlocks;
            var returnValue = RootBlock.CreateInternal(instructions,
                labelToInstruction,
                out blockStarts,
                out instructionBlocks);

            BlockContext context = returnValue.Context;
            IDictionary<int, Block> idBlockMap = context.IdToBlock;

            // Now let's group all the instructionBlocks into InstructionBlockCollection.
            //
            List<InstructionBlock> blockCollection = new List<InstructionBlock>();
            for (int i = 0; i <= instructionBlocks.Count; i++)
            {
                var block = i < instructionBlocks.Count ? instructionBlocks[i] : null;

                if (block == null ||
                    (blockStarts.Contains(block.Instruction.Label) && blockCollection.Count > 0))
                {
                    var currentBlockCollection = new InstructionBlockCollection(blockCollection.ToArray());
                    blockCollection.Clear();

                    idBlockMap.Add(currentBlockCollection.Id, currentBlockCollection);
                }

                if (block != null)
                {
                    blockCollection.Add(block);
                }
            }

            return returnValue;
        }

        public static RootBlock Create2(
            IList<ArgumentSignature> arguments,
            IList<ArgumentSignature> locals,
            IList<Instruction> instructions,
            IDictionary<string, Instruction> labelToInstruction)
        {
            HashSet<string> blockStarts;
            List<InstructionBlock> instructionBlocks;

            var returnValue = RootBlock.CreateInternal(instructions,
                labelToInstruction,
                out blockStarts,
                out instructionBlocks);

            return returnValue;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public override void InsertInstruction(Instruction instruction)
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
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        protected override bool IsContigious(Block block)
        {
            return true;
        }

        private static RootBlock CreateInternal(
            IList<Instruction> instructions,
            IDictionary<string, Instruction> labelToInstruction,
            out HashSet<string> blockStarts,
            out List<InstructionBlock> instructionBlocks)
        {
            BlockContext context = new BlockContext(
                labelToInstruction,
                new Dictionary<int, Block>(),
                new Dictionary<Instruction, InstructionBlock>(),
                instructions,
                new List<Block>());

            blockStarts = new HashSet<string>();
            instructionBlocks = new List<InstructionBlock>();

            foreach (var instruction in instructions)
            {
                switch (instruction.CodeOp.Flow)
                {
                    case FlowType.Branch:
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

                if (instruction.CodeOp.Flow == FlowType.ConditionalBranch)
                {
                    var previousInstruction = instruction.Previous;

                    while (previousInstruction != null &&
                        previousInstruction.StackBefore != instruction.StackAfter)
                    {
                        previousInstruction = previousInstruction.Previous;
                    }

                    if (previousInstruction != null)
                    {
                        blockStarts.Add(previousInstruction.Label);
                    }
                }

                if (instruction.StackBefore == 0)
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
