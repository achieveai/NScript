using System;
using NScript.Lib.AsmDeasm.Ops;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class InstructionBlockCollection : Block
    {
        #region member variables
        public InstructionBlockCollection(InstructionBlock block)
            : base(block)
        { }

        public InstructionBlockCollection(InstructionBlock[] blocks)
            : base(blocks) { }
        #endregion

        #region constructors/Factories
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public override Instruction GetInstructionAt(int i)
        {
            return ((InstructionBlock)this.Children[i]).Instruction;
        }

        public override void InsertInstruction(NScript.Lib.AsmDeasm.Ops.Instruction instruction)
        {
            if (instruction.Index < this.BlockStartIndex ||
                instruction.Index > (this.BlockEndIndex + 1))
            {
                throw new InvalidOperationException();
            }

            var instructionBlock = new InstructionBlock(
                this.Context,
                instruction);

            this.AddChildAt(
                instructionBlock,
                instruction.Index - this.BlockStartIndex);
        }

        public override void DeleteInstruction(int instructionIndex)
        {
            if (instructionIndex < this.BlockStartIndex ||
                instructionIndex > this.BlockEndIndex)
            {
                throw new ArgumentOutOfRangeException("instructionIndex");
            }

            if (this.Children.Count == 1)
            {
                throw new InvalidOperationException("Can't empty InstructionCollection");
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                var child = this.Children[i];
                if (child.BlockEndIndex >= instructionIndex &&
                    child.BlockStartIndex <= instructionIndex)
                {
                    child.DeleteInstruction(instructionIndex);
                    break;
                }
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        protected override void AddChild(Block block)
        {
            InstructionBlock instructionBlock = block as InstructionBlock;
            if (instructionBlock == null)
            {
                throw new ArgumentException("Block should be instruction block");
            }

            InstructionBlock lastBlock = this.Children.Count > 0 ?
                ((InstructionBlock)this.Children[this.Children.Count - 1]) : null;

            if (lastBlock != null &&
                (lastBlock.Instruction.Next != instructionBlock.Instruction ||
                lastBlock.Instruction.CodeOp.Flow == Ops.FlowType.Branch ||
                lastBlock.Instruction.CodeOp.Flow == Ops.FlowType.ConditionalBranch ||
                lastBlock.Instruction.CodeOp.Flow == Ops.FlowType.Return))
            {
                throw new ArgumentException("Blocks should be part of contigous strip");
            }

            base.AddChild(block);
        }

        #endregion
    }
}
