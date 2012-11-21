namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using OpCode = Decompiler.OpCode;

    internal class BlockContext
    {
        public readonly ClrContext ClrContext;
        public readonly IDictionary<string, IlInstruction> LabelToInstruction;
        public readonly IDictionary<int, Block> IdToBlock;
        public readonly IList<IlInstruction> Instructions;
        public readonly IList<Block> Blocks;
        public readonly IDictionary<IlInstruction, InstructionBlock> InstructionToBlock;
        public readonly Scope ExecutionScope;
        public readonly MethodDefinition MethodDefinition;
        private readonly Dictionary<VariableDefinition, List<Tuple<IlInstruction, IlInstruction>>> variableRange;
        private InlineDelegateInfo inlineDelegateInfo = null;
        private int allocatedIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockContext"/> class.
        /// </summary>
        /// <param name="clrContext">The CLR context.</param>
        /// <param name="methodDefinition">The method definition.</param>
        /// <param name="labelToInstruction">The label to instruction.</param>
        /// <param name="idToBlock">The id to block.</param>
        /// <param name="instructionToBlock">The instruction to block.</param>
        /// <param name="instructions">The instructions.</param>
        /// <param name="blocks">The blocks.</param>
        public BlockContext(
            ClrContext clrContext,
            MethodDefinition methodDefinition,
            IDictionary<string, IlInstruction> labelToInstruction,
            IDictionary<int, Block> idToBlock,
            IDictionary<IlInstruction, InstructionBlock> instructionToBlock,
            IList<IlInstruction> instructions,
            IList<Block> blocks)
        {
            this.ClrContext = clrContext;
            this.MethodDefinition = methodDefinition;
            this.ExecutionScope = methodDefinition != null
                ? methodDefinition.Body.Scope
                : null;
            this.LabelToInstruction = labelToInstruction;
            this.IdToBlock = idToBlock;
            this.InstructionToBlock = instructionToBlock;
            this.Instructions = instructions;
            this.Blocks = blocks;

            for (int i = 0; i < this.Blocks.Count; i++)
            {
                this.allocatedIds = Math.Max(this.Blocks[i].Id + 1, this.allocatedIds);
            }

            int variableCount = 0;
            if (this.ExecutionScope != null)
            {
                variableCount = this.ExecutionScope.Variables.Count;
            }
            else
            {
                foreach (var instruction in instructions)
                {
                    switch (instruction.OpCode)
                    {
                        case OpCode.LoadLocal:
                        case OpCode.StoreLocal:
                        case OpCode.LoadLocalAddress:
                        case OpCode.LoadLocalAddress_s:
                        case OpCode.StoreLocal_s:
                            variableCount = Math.Max(
                                variableCount,
                                1 + ((VariableDefinition)instruction.OpCodeArgument).Index);
                            break;
                    }
                }
            }

            this.variableRange = this.GetUsageRanges(variableCount);
        }

        /// <summary>
        /// Gets or sets the inline delegate info.
        /// </summary>
        /// <value>
        /// The inline delegate info.
        /// </value>
        public InlineDelegateInfo InlineDelegateInfo
        {
            get { return this.inlineDelegateInfo; }
            set { this.inlineDelegateInfo = value; }
        }

        /// <summary>
        /// Gets the range containing.
        /// </summary>
        /// <param name="variable">Index of the variable.</param>
        /// <param name="instruction">The instruction.</param>
        /// <returns>VariableRange if one is found, else null.</returns>
        public Tuple<IlInstruction, IlInstruction> GetRangeContaining(
            VariableDefinition variable,
            IlInstruction instruction)
        {
            foreach (Tuple<IlInstruction, IlInstruction> range in this.variableRange[variable])
            {
                if (range.Item1.Index <= instruction.Index
                    && range.Item2.Index >= instruction.Index)
                {
                    return range;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the next id.
        /// </summary>
        /// <returns>newly allocated id</returns>
        public int GetNextId()
        {
            return System.Threading.Interlocked.Increment(ref this.allocatedIds);
        }

        /// <summary>
        /// Gets the usage ranges.
        /// </summary>
        /// <param name="variableCount">The variable count.</param>
        /// <returns></returns>
        private Dictionary<VariableDefinition, List<Tuple<IlInstruction, IlInstruction>>> GetUsageRanges(int variableCount)
        {
            Dictionary<VariableDefinition, List<Tuple<IlInstruction, IlInstruction>>> returnValue =
                new Dictionary<VariableDefinition, List<Tuple<IlInstruction, IlInstruction>>>();
            HashSet<int> visited = new HashSet<int>();
            int[] lastScanedIndex = new int[variableCount];

            for (int variableIndex = 0; variableIndex < this.MethodDefinition.Body.Variables.Count; variableIndex++)
            {
                returnValue[this.MethodDefinition.Body.Variables[variableIndex]] = new List<Tuple<IlInstruction, IlInstruction>>();
            }

            for (int iInstruction = 0; iInstruction < Instructions.Count; iInstruction++)
            {
                var instruction = this.Instructions[iInstruction];

                switch (instruction.OpCode)
                {
                    case OpCode.LoadLocal:
                    case OpCode.StoreLocal:
                    case OpCode.LoadLocalAddress:
                    case OpCode.LoadLocalAddress_s:
                    case OpCode.StoreLocal_s:
                        {
                            VariableDefinition local = (VariableDefinition)instruction.OpCodeArgument;

                            if (lastScanedIndex[local.Index] < instruction.Index)
                            {
                                IlInstruction lastUseIndex = this.GetLastUsage(
                                    local,
                                    iInstruction + 1,
                                    visited);

                                lastUseIndex = lastUseIndex ?? instruction;

                                returnValue[local].Add(
                                    new Tuple<IlInstruction, IlInstruction>(
                                        instruction,
                                        lastUseIndex));

                                lastScanedIndex[local.Index] = lastUseIndex.Index;
                                visited.Clear();
                            }
                        }
                        break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the last usage.
        /// </summary>
        /// <param name="variable">Index of the variable.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="visited">The visited.</param>
        /// <returns></returns>
        private IlInstruction GetLastUsage(
            VariableDefinition variable,
            int startIndex,
            HashSet<int> visited)
        {
            IlInstruction returnValue = null;
            IlInstruction temp;
            if (visited.Contains(startIndex))
            {
                return returnValue;
            }

            for (int iInstruction = startIndex; iInstruction < this.Instructions.Count && !visited.Contains(iInstruction); iInstruction++)
            {
                visited.Add(iInstruction);

                switch (this.Instructions[iInstruction].OpCode)
                {
                    case OpCode.BranchEq:
                    case OpCode.BranchGe:
                    case OpCode.BranchUGe:
                    case OpCode.BranchGt:
                    case OpCode.BranchUGt:
                    case OpCode.BranchLe:
                    case OpCode.BranchULe:
                    case OpCode.BranchLt:
                    case OpCode.BranchULt:
                    case OpCode.BranchUNe:
                    case OpCode.BranchFalse:
                    case OpCode.BranchTrue:
                        temp = this.GetLastUsage(
                            variable,
                            this.LabelToInstruction[(string)this.Instructions[iInstruction].OpCodeArgument].Index,
                            visited);

                        if (temp != null)
                        {
                            if (temp.Index < this.Instructions[iInstruction].Index)
                            {
                                returnValue = this.Instructions[iInstruction];
                            }
                            else
                            {
                                returnValue = temp;
                            }
                        }

                        break;
                    case OpCode.Branch:
                        temp = this.GetLastUsage(
                            variable,
                            this.LabelToInstruction[(string)this.Instructions[iInstruction].OpCodeArgument].Index,
                            visited);

                        if (temp != null)
                        {
                            if (temp.Index < this.Instructions[iInstruction].Index)
                            {
                                returnValue = this.Instructions[iInstruction];
                            }
                            else
                            {
                                returnValue = temp;
                            }
                        }

                        return returnValue;
                    case OpCode.LoadLocal:
                    case OpCode.LoadLocalAddress:
                        if (this.Instructions[iInstruction].OpCodeArgument.Equals(variable))
                        {
                            returnValue = returnValue == null
                                ? this.Instructions[iInstruction]
                                : returnValue.Index < this.Instructions[iInstruction].Index
                                    ? this.Instructions[iInstruction]
                                    : returnValue;
                        }
                        break;
                    case OpCode.StoreLocal:
                        if (this.Instructions[iInstruction].OpCodeArgument.Equals(variable))
                        {
                            return returnValue;
                        }
                        break;
                    case OpCode.Return:
                        return returnValue;
                    default:
                        break;
                }
            }

            return returnValue;
        }
    }
}