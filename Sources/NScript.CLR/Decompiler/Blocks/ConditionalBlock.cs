namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class ConditionalBlock : BasicBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ConditionalBlock(BasicBlock[] blocks)
            : base(blocks)
        {
            if (this.IsConditionalWithSingleBranch())
            {
                this.AddConditionalTail();
            }

            if (this.IsConditionalTail())
            {
                this.FixConditionBranchTail();
            }

            var rightValueBlock = this.RightValueBlock as InstructionBlock;
            var leftValueBlock = this.LeftValuesBlock as InstructionBlock;

            if (leftValueBlock != null && rightValueBlock != null)
            {
                // Since all the constants have already been converted to LoadConstantInd32 with value
                // as argument.
                // if ((leftValueBlock.Instruction.OpCode == OpCode.LoadConstantInt32_0 &&
                //     rightValueBlock.Instruction.OpCode == OpCode.LoadConstantInt32_1) ||
                //     (leftValueBlock.Instruction.OpCode == OpCode.LoadConstantInt32_1 &&
                //     rightValueBlock.Instruction.OpCode == OpCode.LoadConstantInt32_0))
                if (leftValueBlock.Instruction.OpCode == OpCode.LoadConstantInt32 &&
                    rightValueBlock.Instruction.OpCode == OpCode.LoadConstantInt32 &&
                    (((int)leftValueBlock.Instruction.OpCodeArgument == 0
                        && (int)rightValueBlock.Instruction.OpCodeArgument == 1)
                    ||((int)leftValueBlock.Instruction.OpCodeArgument == 1
                        &&(int)rightValueBlock.Instruction.OpCodeArgument == 0)))
                {
                    this.IsConditionOnly = true;
                }
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public bool IsConditionOnly
        { get; private set; }

        public Block RightValueBlock
        {
            get
            {
                return this.Children[this.Children.Count - 1];
            }
        }

        public Block LeftValuesBlock
        {
            get
            {
                return this.Children[this.Children.Count - 3];
            }
        }
        #endregion

        #region public functions
        public static bool IsConditionalBlock(BasicBlock basicBlock)
        {
            if (basicBlock.Children.Count <= 4)
            { return false; }

            if (basicBlock.Children[basicBlock.Children.Count-1].Successors.Count != 1 &&
                basicBlock.Children[basicBlock.Children.Count-2].Successors.Count != 1)
            {
                return false;
            }

            if (!(basicBlock.Children[basicBlock.Children.Count-2] is InstructionBlock) ||
                basicBlock.Children[basicBlock.Children.Count-2].GetInstructionAt(0).OpCode != OpCode.Branch)
            {
                return false;
            }

            if (basicBlock.Children[basicBlock.Children.Count - 1].Successors[0] !=
                basicBlock.Children[basicBlock.Children.Count - 2].Successors[0])
            {
                return false;
            }

            return true;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions

        /// <summary>
        /// There are normally 2 kind of tails
        /// 1.
        /// IL_0025:  br.s       IL_0028
        ///     
        /// IL_0027:  ldc.i4.0
        /// IL_0028:  br.s       IL_002b
        /// IL_002a:  ldc.i4.1
        /// 
        /// IL_002b:  stloc.1
        /// 
        /// 2.
        /// IL_0025:  br.s       IL_002b
        ///     
        /// IL_002a:  ldc.i4.1
        /// 
        /// IL_002b:  stloc.1
        /// 
        /// In the above tails, 1st is used in more complex conditions and second one
        /// is used in somewhat simpler condition, e.g. X == a && Y == b.
        /// 
        /// In case of 1, tail block count will be 3, in 2, its going to be 1.
        /// </summary>
        /// <returns></returns>
        private bool IsConditionalTail()
        {
            // Here we assume that the tail is no longer single branch tail.
            // In that case, we take a look at the flow type of instruction before 4th last
            // block's first instruction. If that instruction is a branch instruction, then
            // we have a branch tail.
            //
            return this.Children[this.Children.Count - 3].GetInstructionAt(0).Previous.FlowType == FlowType.Branch;
        }

        private bool IsConditionalWithSingleBranch()
        {
            if (this.Children.Count < 5)
            { return false; }

            InstructionBlock block = this.Children[this.Children.Count - 3] as InstructionBlock;

            if (block != null)
            {
                if (block.Instruction.FlowType != FlowType.Branch)
                {
                    return false;
                }

                // In case of last condition in ConditionalValueBlock, the last branch
                // is preceded by check condition.
                //
                switch (block.Instruction.Previous.OpCode)
                {
                    case OpCode.CompareEq:
                    case OpCode.CompareGt:
                    case OpCode.CompareUGt:
                    case OpCode.CompareLt:
                    case OpCode.CompareULt:
                        break;
                    default:
                        return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// IL_0023:  ldarg.2
        /// IL_0024:  ldc.i4.3
        /// IL_0025:  ceq
        /// IL_0027:  ldc.i4.0
        /// IL_0028:  ceq
        /// IL_002a:  br.s       IL_002d
        /// 
        /// IL_002c:  ldc.i4.1
        /// IL_002d:  br.s       IL_0030
        /// IL_002f:  ldc.i4.0
        /// IL_0030:  stloc.3
        ///
        /// To:
        /// 
        /// IL_0023:  ldarg.2
        /// IL_0024:  ldc.i4.3
        /// IL_0025:  beq.s      IL_002f
        /// 
        /// IL_002c:  ldc.i4.1
        /// IL_002d:  br.s       IL_0030
        /// IL_002f:  ldc.i4.0
        /// IL_0030:  stloc.3
        /// 
        /// </summary>
        /// <returns></returns>
        private void FixConditionBranchTail()
        {
            int iBlock = this.Children.Count - 5;

            string branchLabel = this.Children[this.Children.Count - 1].GetInstructionAt(0).Label;

            bool firstBlockIsTrue = true;

            // To check the next load, we need to skip branch that follows current node.
            //
            if (this.Children[iBlock + 2].GetInstructionAt(0).OpCode == OpCode.LoadConstantInt32
                && (int)this.Children[iBlock + 2].GetInstructionAt(0).OpCodeArgument == 0
                && this.Children[iBlock + 2].InstructionCount == 1)
            {
                firstBlockIsTrue = false;
            }

            bool hasNotCheck = false;
            var newOp = ConditionalBlock.GetBranchCode(
                (BasicBlock)this.Children[iBlock],
                ref hasNotCheck);

            IlInstruction instructionToReplace;

            // The instruction that we need to replace is the check block that is working
            // on arguments.
            //
            if (hasNotCheck)
            {
                instructionToReplace = this.Children[iBlock]
                 .GetInstructionAt(this.Children[iBlock].InstructionCount - 3);
            }
            else
            {
                instructionToReplace = this.Children[iBlock]
                 .GetInstructionAt(this.Children[iBlock].InstructionCount - 1);
            }

            if (firstBlockIsTrue)
            {
                newOp = ConditionalBlock.GetNotBranch(newOp);
            }

            var newInstruction = new IlInstruction(
                null,
                newOp,
                instructionToReplace.Label,
                instructionToReplace.Index,
                branchLabel,
                instructionToReplace.Location);

            newInstruction.StackLevelBefore = instructionToReplace.StackLevelBefore;

            if (hasNotCheck)
            {
                // First we should dissolve Check instruction and it's stack.
                //
                this.Children[iBlock].Dissolve();

                // Now we can safely delete check instruction and load const.
                //
                this.DeleteInstruction(newInstruction.Index + 1);
                this.DeleteInstruction(newInstruction.Index + 1);
            }

            // Now let's delete the branch instruction as well.
            //
            this.DeleteInstruction(newInstruction.Index + 1);

            // Now we need to replace check instruction with new branch instruction. To
            // do this, we need to first dissolve check instruction's stack and then
            // delete check instruction and then we can add new instruction.
            //
            // Also notice that since we have deleted the branch instruction, the index of
            // check instruction would be iBlock + 1.
            //
            this.Children[iBlock].Dissolve();
            this.DeleteInstruction(instructionToReplace.Index);
            this.InsertInstruction(newInstruction);

            // Now let's recreate the dissolved block.
            //
            new StackedBlock(
                (InstructionBlock)this.Children[iBlock + 2],
                new[] {
                    (BasicBlock) this.Children[iBlock],
                    (BasicBlock) this.Children[iBlock + 1],
                });
        }

        public static OpCode GetNotBranch(
            OpCode opCode)
        {
            switch (opCode)
            {
                case OpCode.BranchEq:
                    return OpCode.BranchUNe;
                case OpCode.BranchUNe:
                    return OpCode.BranchEq;
                case OpCode.BranchGe:
                    return OpCode.BranchLt;
                case OpCode.BranchUGe:
                    return OpCode.BranchULt;
                case OpCode.BranchGt:
                    return OpCode.BranchLe;
                case OpCode.BranchUGt:
                    return OpCode.BranchULe;
                case OpCode.BranchLe:
                    return OpCode.BranchGt;
                case OpCode.BranchULe:
                    return OpCode.BranchUGt;
                case OpCode.BranchLt:
                    return OpCode.BranchGe;
                case OpCode.BranchULt:
                    return OpCode.BranchUGe;
                case OpCode.BranchTrue:
                    return OpCode.BranchFalse;
                case OpCode.BranchFalse:
                    return OpCode.BranchTrue;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static OpCode GetBranchCode(
            BasicBlock blockCollection,
            ref bool hasNotCheck)
        {
            if (blockCollection.GetInstructionAt(blockCollection.InstructionCount - 1).OpCode == OpCode.CompareEq &&
                (int)blockCollection.GetInstructionAt(blockCollection.InstructionCount - 2).OpCodeArgument == 0)
            {
                hasNotCheck = true;
                return ConditionalBlock.ConvertCheckToBranch(
                    blockCollection.GetInstructionAt(blockCollection.InstructionCount - 3).OpCode,
                    true);
            }

            hasNotCheck = false;
            return ConditionalBlock.ConvertCheckToBranch(
                blockCollection.GetInstructionAt(blockCollection.InstructionCount - 1).OpCode,
                false);
        }

        public static OpCode ConvertCheckToBranch(
            OpCode opCode,
            bool not)
        {
            switch (opCode)
            {
                case OpCode.CompareEq:
                    opCode = OpCode.BranchEq;
                    break;
                case OpCode.CompareGt:
                    opCode = OpCode.BranchGt;
                    break;
                case OpCode.CompareUGt:
                    opCode = OpCode.BranchUGt;
                    break;
                case OpCode.CompareLt:
                    opCode = OpCode.BranchLt;
                    break;
                case OpCode.CompareULt:
                    opCode = OpCode.BranchULt;
                    break;
                default:
                    throw new ArgumentException();
            }

            if (not)
            {
                opCode = ConditionalBlock.GetNotBranch(opCode);
            }

            return opCode;
        }

        private void AddConditionalTail()
        {
            int blockIndex = this.Children.Count - 2;

            var loadConstInstruction =
                this.Children[blockIndex].GetInstructionAt(0);

            var newConstInstruction = new IlInstruction(
                null,
                OpCode.LoadConstantInt32,
                loadConstInstruction.Previous.Label + "_ContitionalTailAdd1",
                loadConstInstruction.Index,
                (int)loadConstInstruction.OpCodeArgument == 0 ?
                    1 : 0,
                loadConstInstruction.Location)
                {
                    StackLevelBefore = loadConstInstruction.StackLevelBefore
                };

            var newBranchInstruction = new IlInstruction(
                null,
                OpCode.Branch,
                loadConstInstruction.Previous.Label + "_ContitionalTailAdd2",
                newConstInstruction.Index + 1,
                loadConstInstruction.Next.Label,
                loadConstInstruction.Location)
                {
                    StackLevelBefore = loadConstInstruction.StackLevelAfter
                };

            this.InsertInstruction(newConstInstruction);
            this.InsertInstruction(newBranchInstruction);
        }
        #endregion
    }
}
