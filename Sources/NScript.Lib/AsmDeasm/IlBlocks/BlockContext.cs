using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.AsmDeasm.Ops;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class BlockContext
    {
        public readonly IDictionary<string, Instruction> LabelToInstruction;
        public readonly IDictionary<int, Block> IdToBlock;
        public readonly IList<Instruction> Instructions;
        public readonly IList<Block> Blocks;
        public readonly IDictionary<Instruction, InstructionBlock> InstructionToBlock;

        public BlockContext(
            IDictionary<string, Instruction> labelToInstruction,
            IDictionary<int, Block> idToBlock,
            IDictionary<Instruction, InstructionBlock> instructionToBlock,
            IList<Instruction> instructions,
            IList<Block> blocks)
        {
            this.LabelToInstruction = labelToInstruction;
            this.IdToBlock = idToBlock;
            this.InstructionToBlock = instructionToBlock;
            this.Instructions = instructions;
            this.Blocks = blocks;
        }
    }
}
