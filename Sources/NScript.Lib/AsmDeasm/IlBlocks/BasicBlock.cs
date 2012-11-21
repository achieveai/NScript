using System;
using NScript.Lib.AsmDeasm.Ops;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class BasicBlock :  Block
    {
        public BasicBlock(BasicBlock[] childBlocks)
            : base(childBlocks) { }

        protected BasicBlock(BlockContext context)
            : base(context) { }

        protected BasicBlock(BasicBlock childBlock)
            : base(childBlock) { }

        public override void InsertInstruction(Instruction instruction)
        {
            if (instruction.Index < this.BlockStartIndex ||
                instruction.Index > this.BlockEndIndex)
            {
                throw new InvalidOperationException();
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                if (instruction.Index >= this.Children[i].BlockStartIndex &&
                    instruction.Index <= (this.Children[i].BlockEndIndex + 1))
                {
                    if (this.Children[i] is InstructionBlock)
                    {
                        var instructionBlock = new InstructionBlock(
                            this.Context,
                            instruction);

                        this.AddChildAt(
                            instructionBlock,
                            i + 1);

                    }
                    else
                    {
                        this.Children[i].InsertInstruction(instruction);
                    }
                    return;
                }
            }
        }

        public override void DeleteInstruction(int instructionIndex)
        {
            for (int i = 0; i < this.Children.Count-1; i++)
            {
                if (this.Children[i].BlockStartIndex == instructionIndex &&
                    this.Children[i] is InstructionBlock)
                {
                    this.Children[i].DeleteInstruction(instructionIndex);

                    if (this.Children.Count == 0)
                    {
                        this.Dissolve();
                    }

                    return;
                }
            }

            base.DeleteInstruction(instructionIndex);
        }

    }
}
