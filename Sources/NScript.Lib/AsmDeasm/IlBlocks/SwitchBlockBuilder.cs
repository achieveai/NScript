using System;
using System.Collections.Generic;
using System.Linq;
using NScript.Lib.AsmDeasm.Ops;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    partial class SwitchBlock
    {
        internal class SwitchBlockBuilder
        {
            public static void Process(Block parentBlock)
            {
                for (int i = 0; i < parentBlock.Children.Count; i++)
                {
                    if (parentBlock.Children[i] is BasicBlock)
                    {
                        SwitchBlockBuilder.CreateIntSwitchStatement(
                            parentBlock,
                            i);
                    }
                }

                for (int iChild = 0; iChild < parentBlock.Children.Count; iChild++)
                {
                    if (parentBlock.Children[iChild].GetType() ==
                        typeof(SwitchBlock))
                    {
                        foreach (var subBlock in ((SwitchBlock)parentBlock.Children[iChild]).AllCases)
                        {
                            SwitchBlockBuilder.Process(subBlock);
                        }
                    }
                    else if (!(parentBlock.Children[iChild] is BasicBlock))
                    {
                        SwitchBlockBuilder.Process(parentBlock.Children[iChild]);
                    }
                }
            }

            private static SwitchBlock CreateIntSwitchStatement(
                Block parentBlock,
                int blockIndex)
            {
                // We need at least 4 blocks.
                //
                if (blockIndex >= parentBlock.Children.Count - 4)
                {
                    return null;
                }

                Block currentBlock = parentBlock.Children[blockIndex];
                if (!(currentBlock is StackedBlock))
                {
                    return null;
                }

                // Switch statement will always store the switch value in temporary variable.
                // Here we depend on that fact.
                //
                if (currentBlock.GetInstructionAt(currentBlock.InstructionCount - 1).CodeOp.Code != IlOpCode.StoreLocal)
                {
                    return null;
                }

                int localIndex = (int)currentBlock.GetInstructionAt(currentBlock.Children.Count - 1).OpCodeArgument;
                int switchEndIndex = blockIndex + 1;
                int endIndex = 0;
                List<int> bodyBlockBoundaries;

                // first block should use local variable.
                //
                if (!(parentBlock.Children[blockIndex + 1] is StackedBlock) ||
                    SwitchBlockBuilder.GetLocalVarIndexFromHeader(
                        (StackedBlock)parentBlock.Children[blockIndex + 1]) !=
                    localIndex)
                {
                    return null;
                }

                if (SwitchBlockBuilder.GetSwitchBlock(
                    parentBlock,
                    localIndex,
                    blockIndex,
                    ref switchEndIndex,
                    ref endIndex,
                    out bodyBlockBoundaries))
                {
                    List<Block> caseBlocks = new List<Block>();
                    List<Block> subBlocks = new List<Block>();
                    for (int i = bodyBlockBoundaries.Count - 1; i > 0; i--)
                    {
                        for (int j = bodyBlockBoundaries[i - 1]; j < bodyBlockBoundaries[i]; j++)
                        {
                            subBlocks.Add(parentBlock.Children[j]);
                        }

                        // All the subBlocks should contain something.
                        //
                        if (subBlocks.Count == 0)
                        {
                            return null;
                        }

                        caseBlocks.Insert(0, new Block(subBlocks.ToArray()));
                        subBlocks.Clear();
                    }

                    Dictionary<int, Block> caseValueMapping = new Dictionary<int, Block>();

                    for (int i = blockIndex; i <= switchEndIndex; i++)
                    {
                        subBlocks.Add(parentBlock.Children[i]);

                        SwitchBlockBuilder.AddIntCaseValue(
                            caseValueMapping,
                            parentBlock.Children[i] as StackedBlock);
                    }

                    subBlocks.AddRange(caseBlocks);

                    HashSet<Block> tmpCaseBlocks = new HashSet<Block>(caseBlocks);
                    // Now let's create case block mappings and default block mapping
                    //
                    List<int> caseValues = new List<int>(caseValueMapping.Keys);
                    Dictionary<int, Block> caseBlockMappings = new Dictionary<int, Block>();

                    for (int i = 0; i < caseValues.Count; i++)
                    {
                        caseBlockMappings.Add(
                            i,
                            caseValueMapping[caseValues[i]]);

                        tmpCaseBlocks.Remove(caseValueMapping[caseValues[i]]);
                    }

                    if (tmpCaseBlocks.Count == 1)
                    {
                        caseBlockMappings.Add(-1, tmpCaseBlocks.First());
                    }
                    else if (tmpCaseBlocks.Count != 0)
                    {
                        throw new InvalidProgramException();
                    }

                    var returnValue = new SwitchBlock(subBlocks.ToArray())
                        {
                            IntCases = caseValues,
                            CaseBlocks = caseBlockMappings,
                            LocalVariableIndex = localIndex,
                            AllCases = caseBlocks,
                            Switch = subBlocks[0]
                        };

                    returnValue.InitializeBreakStatements();

                    return returnValue;
                }

                return null;
            }

            private static bool GetSwitchBlock(
                Block parentBlock,
                int localVariableIndex,
                int switchStartIndex,
                ref int switchEndIndex,
                ref int endIndex,
                out List<int> bodyBlockBoundaries)
            {
                bodyBlockBoundaries = null;

                if (switchEndIndex >= parentBlock.Children.Count - 1)
                {
                    return false;
                }

                if (!(parentBlock.Children[switchEndIndex] is StackedBlock) &&
                    !((parentBlock.Children[switchEndIndex] is InstructionBlock) &&
                      parentBlock.Children[switchEndIndex].GetInstructionAt(0).CodeOp.Code == IlOpCode.Branch))
                {
                    return false;
                }

                int tmpSwitchEndIndex = switchEndIndex + 1;
                BasicBlock currentBlock = (BasicBlock)parentBlock.Children[switchEndIndex];

                if (SwitchBlockBuilder.IsIntHeaderPart(
                    currentBlock,
                    localVariableIndex))
                {
                    if (GetSwitchBlock(
                        parentBlock,
                        localVariableIndex,
                        switchStartIndex,
                        ref tmpSwitchEndIndex,
                        ref endIndex,
                        out bodyBlockBoundaries))
                    {
                        switchEndIndex = tmpSwitchEndIndex;
                        return true;
                    }

                    int tmpEndIndex = SwitchBlockBuilder.GetBodyEndIndex(
                        parentBlock,
                        switchStartIndex,
                        tmpSwitchEndIndex,
                        out bodyBlockBoundaries);

                    if (tmpEndIndex > tmpSwitchEndIndex)
                    {
                        endIndex = tmpEndIndex;
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Scans for all the body blocks, and returns the endIndex for switch block.
            /// </summary>
            /// <param name="parentBlock"></param>
            /// <param name="bodyStartIndex"></param>
            /// <param name="bodyBlockBoundaries"></param>
            /// <returns></returns>
            private static int GetBodyEndIndex(
                Block parentBlock,
                int switchStartIndex,
                int bodyStartIndex,
                out List<int> bodyBlockBoundaries)
            {
                bodyBlockBoundaries = null;
                HashSet<Block> switchBlockBodiesHash = new HashSet<Block>();
                HashSet<Block> switchBlockHeaders = new HashSet<Block>();

                for (int i = switchStartIndex; i < bodyStartIndex; i++)
                {
                    switchBlockHeaders.Add(parentBlock.Children[i]);

                    foreach (var successor in parentBlock.Children[i].Successors)
                    {
                        if (!switchBlockBodiesHash.Contains(successor))
                        {
                            switchBlockBodiesHash.Add(successor);
                        }
                    }
                }

                foreach (var switchBlockHeader in switchBlockHeaders)
                {
                    switchBlockBodiesHash.Remove(switchBlockHeader);
                }

                List<int> switchBodyIndexes = SwitchBlockBuilder.GetBlockIndexes(
                    parentBlock,
                    switchBlockBodiesHash);

                if (switchBodyIndexes.Count < 2)
                {
                    return -1;
                }

                // We can't have more than one blocks that are outside
                // of this parent.
                //
                if (switchBodyIndexes[switchBodyIndexes.Count - 1] == -1 &&
                    switchBodyIndexes[switchBodyIndexes.Count - 2] == -1)
                {
                    return -1;
                }

                // Now let's check contiguity of these blocks.
                // Every block should be Contiguous.
                //
                for (int i = 0; i < switchBodyIndexes.Count - 1; i++)
                {
                    if (!parentBlock.IsInNonBranchingRangeBlock(
                        switchBodyIndexes[i],
                        switchBodyIndexes[i + 1] - 1))
                    {
                        return -1;
                    }
                }

                // Now that we have indexes, there are two cases
                // 1. last index belongs to default block.
                // 2. last index belongs to following block.
                //
                // If it is case 1: then if we check at all the previous blocks, they would lead to
                // following block. Now there is a case where all the blocks returns, we have nothing
                // to work with here. Also in this case the last really is the default block.
                //
                // Else in case 1:, if we follow all the previous blocks, they would lead to last index
                //
                var farthestIndex = SwitchBlockBuilder.GetFarthestSuccessorInRange(
                    parentBlock,
                    bodyStartIndex,
                    switchBodyIndexes[switchBodyIndexes.Count - 1] == -1 ?
                        parentBlock.Children.Count - 1 :
                        switchBodyIndexes[switchBodyIndexes.Count - 1] - 1);

                if (farthestIndex == switchBodyIndexes[switchBodyIndexes.Count - 1])
                {
                    bodyBlockBoundaries = switchBodyIndexes;

                    // This is case 2.
                    //
                    if (farthestIndex == -1)
                    {
                        return parentBlock.Children.Count - 1;
                    }

                    return farthestIndex - 1;
                }
                
                if (switchBodyIndexes[switchBodyIndexes.Count - 1] == -1)
                {
                    // Since this is not case 2. this can't happen.
                    //
                    return -1;
                }
                
                if (switchBodyIndexes[switchBodyIndexes.Count - 1] > farthestIndex &&
                    farthestIndex >= 0)
                {
                    // This is the case where all the previous but last blocks returns.
                    // In this case our last block is last block.
                    //
                    bodyBlockBoundaries = switchBodyIndexes;
                    bodyBlockBoundaries.Add(switchBodyIndexes[switchBodyIndexes.Count - 1] + 1);

                    return bodyBlockBoundaries[bodyBlockBoundaries.Count - 1];
                }

                // This is case 1.
                // Let's make sure this is really the case. If this is the case then scanning from
                // lastIndex to farthestIndex -1 should result in farthestIndex.
                // OR
                // lastIndex through farthestIndex should be contiguous block.
                //
                // We haven't checked the contiguity of last Block.
                // Let's check this before returning the value.
                //
                if (parentBlock.IsInNonBranchingRangeBlock(
                    switchBodyIndexes[switchBodyIndexes.Count - 1],
                    farthestIndex == -1 ? parentBlock.Children.Count - 1 : farthestIndex - 1))
                {
                    bodyBlockBoundaries = switchBodyIndexes;
                    bodyBlockBoundaries.Add(farthestIndex == -1 ? parentBlock.Children.Count : farthestIndex);

                    return farthestIndex == -1 ?
                        parentBlock.Children.Count - 1 :
                        farthestIndex - 1;
                }

                // Looks like we did not meet both Case 1 and Case 2 described above.
                // This means we can't find endIndex for body.
                //
                return -1;
            }


            private static List<int> GetBlockIndexes(
                Block parentBlock,
                IEnumerable<Block> blocks)
            {
                List<Block> blocksList = new List<Block>(blocks);

                // Now let's sort these blocks.
                //
                blocksList.Sort((l, r) => l.BlockStartIndex - r.BlockStartIndex);

                List<int> blockIndexes = new List<int>();

                int i, j;
                for (i = 0, j = 0; i < parentBlock.Children.Count && j < blocksList.Count; i++)
                {
                    if (parentBlock.Children[i] == blocksList[j])
                    {
                        blockIndexes.Add(i);
                        j++;
                    }
                }

                for (i = j; i < blocksList.Count; i++)
                {
                    blockIndexes.Add(-1);
                }

                return blockIndexes;
            }

            private static void AddIntCaseValue(
                Dictionary<int, Block> mapping,
                StackedBlock header)
            {
                if (header == null) return;

                if (header.Children.Count == 3)
                {
                    SwitchBlockBuilder.AddIntBranchCaseValue(
                        mapping,
                        header);
                }
                else if (header.Children.Count == 2 &&
                    header.InstructionCount == 4)
                {
                    SwitchBlockBuilder.AddIntSwitchCaseValue(
                        mapping,
                        header);
                }
            }

            /// <summary>
            /// Check for following pattern and return the index of local variable used.
            /// 
            /// P1:
            /// IL_000d:  ldloc.0
            /// IL_000e:  ldc.i4.s   10
            /// IL_0010:  bgt.un.s   IL_001d
            /// 
            /// P2:
            /// IL_0012:  ldloc.0
            /// IL_0013:  ldc.i4.2
            /// IL_0014:  beq.s      IL_005e
            /// 
            /// P3:
            /// IL_0016:  ldloc.0
            /// IL_0017:  ldc.i4.s   10
            /// IL_0019:  beq.s      IL_0052
            /// 
            /// </summary>
            /// <param name="header"></param>
            /// <returns></returns>
            private static int GetLocalVarIndexFromBranchHeader(
                StackedBlock header)
            {
                if (header.Children.Count != 3)
                {
                    return -1;
                }

                if (header.GetInstructionAt(0).CodeOp.Code != Ops.IlOpCode.LoadLocal)
                {
                    return -1;
                }

                if (!SwitchBlockBuilder.IsGoodIntLoadStatement(header.GetInstructionAt(1)))
                {
                    return -1;
                }

                if (header.GetInstructionAt(2).CodeOp.Flow != FlowType.ConditionalBranch)
                {
                    return -1;
                }

                return (int)header.GetInstructionAt(0).OpCodeArgument;
            }

            private static bool IsIntHeaderPart(
                BasicBlock headerBlock,
                int variableIndex)
            {
                if (headerBlock is InstructionBlock &&
                    headerBlock.GetInstructionAt(0).CodeOp.Code == IlOpCode.Branch)
                {
                    return true;
                }

                return SwitchBlockBuilder.GetLocalVarIndexFromHeader(headerBlock as StackedBlock) == variableIndex;
            }

            private static int GetLocalVarIndexFromHeader(
                StackedBlock headerBlock)
            {
                if (headerBlock == null)
                {
                    return -1;
                }

                if (headerBlock.Children.Count == 3)
                {
                    return SwitchBlockBuilder.GetLocalVarIndexFromBranchHeader(headerBlock);
                }
                
                if (headerBlock.Children.Count == 2 &&
                    headerBlock.InstructionCount == 4)
                {
                    return SwitchBlockBuilder.GetLocalVarIndexFromSwitchHeader(headerBlock);
                }

                return -1;
            }

            private static void AddIntBranchCaseValue(
                Dictionary<int, Block> mapping,
                StackedBlock header)
            {
                var ilCode = header.GetInstructionAt(2).CodeOp.Code;

                if (ilCode == IlOpCode.BranchIfEqual)
                {
                    // Conditional branch will have 0th successor as next successor and 1st for branch.
                    //
                    mapping.Add(
                        (int)header.GetInstructionAt(1).OpCodeArgument,
                        header.Children[2].Successors[1]);
                }
            }

            /// <summary>
            /// Check for following pattern.
            /// 
            /// IL_0027:  ldloc.0
            /// IL_0028:  ldc.i4.s   100
            /// IL_002a:  sub
            /// IL_002b:  switch     ( 
            ///                       IL_0046,
            ///                       IL_006a,
            ///                       IL_0082,
            ///                       IL_008e,
            ///                       IL_0076)
            ///                       
            /// 
            /// IL_0044:  br.s       IL_009a
            /// </summary>
            /// <param name="header"></param>
            /// <returns></returns>
            private static int GetLocalVarIndexFromSwitchHeader(
                StackedBlock header)
            {
                if (header.Children.Count != 2)
                {
                    return -1;
                }

                if (header.GetInstructionAt(3).CodeOp.Code != Ops.IlOpCode.Switch)
                {
                    return -1;
                }

                if (!SwitchBlockBuilder.IsGoodIntLoadStatement(header.GetInstructionAt(1)))
                {
                    return -1;
                }

                if (header.GetInstructionAt(2).CodeOp.Code != Ops.IlOpCode.Sub)
                {
                    return -1;
                }

                return (int)header.GetInstructionAt(0).OpCodeArgument;
            }

            /// <summary>
            /// Adds switch case value and the target blocks to the mapping provided.
            /// </summary>
            /// <param name="mapping"></param>
            /// <param name="header"></param>
            private static void AddIntSwitchCaseValue(
                Dictionary<int, Block> mapping,
                StackedBlock header)
            {
                int value = (int)header.GetInstructionAt(1).OpCodeArgument;

                for (int i = 1; i < header.Successors.Count; i++)
                {
                    // Switch branch will have 0th successor as next block and 1st and following as
                    // branch targets.
                    //
                    mapping.Add(
                        value + i - 1,
                        header.Successors[i]);
                }
            }

            /// <summary>
            /// Checks only for load of constant ints.
            /// </summary>
            /// <param name="instruction"></param>
            /// <returns></returns>
            private static bool IsGoodIntLoadStatement(
                Instruction instruction)
            {
                switch (instruction.CodeOp.Code)
                {
                    case Ops.IlOpCode.LoadConstantInt:
                    case Ops.IlOpCode.LoadConstantLong:
                        return true;
                    default:
                        return false;
                }
            }

            /// <summary>
            /// This function scans all the blocks between start and end and returns the farthest successor of all.
            /// </summary>
            /// <param name="parentBlock"></param>
            /// <param name="scanStart"></param>
            /// <param name="scanEnd"></param>
            /// <returns></returns>
            private static int GetFarthestSuccessorInRange(
                Block parentBlock,
                int scanStart,
                int scanEnd)
            {
                Block lastBlock = null;

                for (int i = scanStart; i <= scanEnd; i++)
                {
                    foreach (var successor in parentBlock.Children[i].Successors)
                    {
                        if (lastBlock == null ||
                            successor.BlockStartIndex > lastBlock.BlockStartIndex)
                        {
                            lastBlock = successor;
                        }
                    }
                }

                return parentBlock.Children.IndexOf(lastBlock);
            }
        }
    }
}
