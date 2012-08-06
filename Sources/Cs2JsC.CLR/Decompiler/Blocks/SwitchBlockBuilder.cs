namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;
    using Mono.Cecil;

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

                StackedBlock currentBlock = parentBlock.Children[blockIndex] as StackedBlock;
                if (currentBlock == null)
                {
                    return null;
                }

                // Switch statement will always store the switch value in temporary variable.
                // Here we depend on that fact.
                //
                if (currentBlock.GetInstructionAt(currentBlock.InstructionCount - 1).OpCode != OpCode.StoreLocal
                    && currentBlock.RootBlock.Instruction.OpCode != OpCode.Switch)
                {
                    return null;
                }

                object localOrArg = null;
                int switchEndIndex = blockIndex;
                int endIndex = 0;
                List<int> bodyBlockBoundaries;
                if (currentBlock.RootBlock.Instruction.OpCode != OpCode.Switch)
                {
                    localOrArg = (VariableDefinition)currentBlock.GetInstructionAt(currentBlock.InstructionCount - 1)
                        .OpCodeArgument;
                    switchEndIndex = blockIndex + 1;
                }
                else
                {
                    localOrArg = SwitchBlockBuilder.GetLocalOrArgFromSwitchHeader(currentBlock);
                }

                StackedBlock firstSwitchBlock = parentBlock.Children[switchEndIndex] as StackedBlock;

                // first block should use local variable.
                //
                if (firstSwitchBlock == null
                    || (SwitchBlockBuilder.GetLocalVarIndexFromHeader(firstSwitchBlock) != localOrArg
                        && firstSwitchBlock != currentBlock))
                {
                    return null;
                }

                if (SwitchBlockBuilder.GetSwitchBlock(
                    parentBlock,
                    localOrArg,
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
                            localOrArg == null,
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
                            LocalVarOrArg = localOrArg,
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
                object localOrArg,
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
                      parentBlock.Children[switchEndIndex].GetInstructionAt(0).OpCode == OpCode.Branch))
                {
                    return false;
                }

                int tmpSwitchEndIndex = switchEndIndex + 1;
                BasicBlock currentBlock = (BasicBlock)parentBlock.Children[switchEndIndex];

                if (SwitchBlockBuilder.IsIntHeaderPart(
                    currentBlock,
                    localOrArg))
                {
                    // if LocalVariable is null, this means that we are analyzing switch block and only switch block
                    // so there is no point in recursing and trying to find any more switch case blocks.
                    //
                    // if localVariableIndex is something else, then it makes sense to look for more switch case blocks.
                    if (localOrArg != null
                        && SwitchBlockBuilder.GetSwitchBlock(
                            parentBlock,
                            localOrArg,
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

                /*
                // Now let's check contiguity of these blocks.
                // Every block should be Contiguous.
                //
                // we are removing this check , because we want to support some goto:s as well.
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
                 */

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
                var nearestIndex = SwitchBlockBuilder.GetNearestSuccessorInRange(
                    parentBlock,
                    bodyStartIndex,
                    switchBodyIndexes[switchBodyIndexes.Count - 1] == -1
                        ? parentBlock.Children.Count - 1
                        : switchBodyIndexes[switchBodyIndexes.Count - 2],
                    switchBodyIndexes[switchBodyIndexes.Count - 1]);

                if (nearestIndex == switchBodyIndexes[switchBodyIndexes.Count - 1])
                {
                    bodyBlockBoundaries = switchBodyIndexes;

                    // This is case 2.
                    //
                    if (nearestIndex == -1)
                    {
                        return parentBlock.Children.Count - 1;
                    }

                    return nearestIndex - 1;
                }
                
                if (switchBodyIndexes[switchBodyIndexes.Count - 1] == -1)
                {
                    // Since this is not case 2. this can't happen.
                    //
                    return -1;
                }
                
                if (switchBodyIndexes[switchBodyIndexes.Count - 1] > nearestIndex &&
                    nearestIndex >= 0)
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
                bodyBlockBoundaries = switchBodyIndexes;
                bodyBlockBoundaries.Add(nearestIndex == -1 ? parentBlock.Children.Count : nearestIndex);

                return nearestIndex == -1 ?
                    parentBlock.Children.Count - 1 :
                    nearestIndex - 1;
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
                bool switchOnlyBlock,
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
                else if (header.Children.Count == 2 && header.InstructionCount == 4 && !switchOnlyBlock)
                {
                    SwitchBlockBuilder.AddIntSwitchCaseValue(
                        mapping,
                        header);
                }
                else if (switchOnlyBlock && header.GetInstructionAt(header.InstructionCount - 1).OpCode == OpCode.Switch)
                {
                    SwitchBlockBuilder.AddZeroBaseSwitchCaseValue(
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
            private static VariableDefinition GetLocalVarIndexFromBranchHeader(
                StackedBlock header)
            {
                if (header.Children.Count != 3)
                {
                    return null;
                }

                if (header.GetInstructionAt(0).OpCode != OpCode.LoadLocal)
                {
                    return null;
                }

                if (!SwitchBlockBuilder.IsGoodIntLoadStatement(header.GetInstructionAt(1)))
                {
                    return null;
                }

                if (header.GetInstructionAt(2).FlowType != FlowType.ConditionalBranch)
                {
                    return null;
                }

                return (VariableDefinition)header.GetInstructionAt(0).OpCodeArgument;
            }

            private static bool IsIntHeaderPart(
                BasicBlock headerBlock,
                object localOrArg)
            {
                if (headerBlock is InstructionBlock &&
                    headerBlock.GetInstructionAt(0).OpCode == OpCode.Branch)
                {
                    return true;
                }

                return SwitchBlockBuilder.GetLocalVarIndexFromHeader(headerBlock as StackedBlock) == localOrArg;
            }

            private static object GetLocalVarIndexFromHeader(
                StackedBlock headerBlock)
            {
                if (headerBlock == null)
                {
                    return null;
                }

                // This is simple comparision function
                // lbl:   ldloc x
                // lbl+1: ldconst y
                // lbl+2: branchXX target
                if (headerBlock.Children.Count == 3)
                {
                    return SwitchBlockBuilder.GetLocalVarIndexFromBranchHeader(headerBlock);
                }
                
                if (headerBlock.Children.Count == 2 &&
                    headerBlock.InstructionCount == 4)
                {
                    return SwitchBlockBuilder.GetLocalOrArgFromSwitchHeader(headerBlock);
                }

                return null;
            }

            private static void AddIntBranchCaseValue(
                Dictionary<int, Block> mapping,
                StackedBlock header)
            {
                var ilCode = header.GetInstructionAt(2).OpCode;

                if (ilCode == OpCode.BranchEq)
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
            private static object GetLocalOrArgFromSwitchHeader(
                StackedBlock header)
            {
                if (header.Children.Count != 2)
                {
                    return null;
                }

                if (header.GetInstructionAt(3).OpCode != OpCode.Switch)
                {
                    return null;
                }

                if (!SwitchBlockBuilder.IsGoodIntLoadStatement(header.GetInstructionAt(1)))
                {
                    return null;
                }

                if (header.GetInstructionAt(2).OpCode != OpCode.Sub)
                {
                    return null;
                }

                return header.GetInstructionAt(0).OpCodeArgument;
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
                int value = header.InstructionCount == 2
                    ? 0
                    : (int)header.GetInstructionAt(1).OpCodeArgument;

                // There may be couple of case blocks that are merged. In those cases we need to
                // map their case label to correct successor block.
                string[] labels = (string[])header.RootBlock.Instruction.OpCodeArgument;

                for (int i = 0; i < labels.Length; i++)
                {
                    var instruction = header.Context.LabelToInstruction[labels[i]];
                    for (int j = 0; j < header.Successors.Count; j++)
                    {
                        if (instruction.Index >= header.Successors[j].BlockStartIndex
                            && instruction.Index <= header.Successors[j].BlockEndIndex)
                        {
                            // Switch branch will have 0th successor as next block and 1st and following as
                            // branch targets.
                            //
                            mapping.Add(
                                value + i,
                                header.Successors[j]);
                        }
                    }
                }
            }

            /// <summary>
            /// Adds the zero base switch case value.
            /// </summary>
            /// <param name="mapping">The mapping.</param>
            /// <param name="header">The header.</param>
            private static void AddZeroBaseSwitchCaseValue(
                Dictionary<int, Block> mapping,
                StackedBlock header)
            {
                for (int i = 1; i < header.Successors.Count; i++)
                {
                    // Switch branch will have 0th successor as next block and 1st and following as
                    // branch targets.
                    //
                    mapping.Add(
                        i - 1,
                        header.Successors[i]);
                }
            }

            /// <summary>
            /// Checks only for load of constant ints.
            /// </summary>
            /// <param name="instruction"></param>
            /// <returns></returns>
            private static bool IsGoodIntLoadStatement(
                IlInstruction instruction)
            {
                switch (instruction.OpCode)
                {
                    case OpCode.LoadConstantInt32:
                    case OpCode.LoadConstantInt64:
                        return true;
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Gets the nearest successor in range.
            /// </summary>
            /// <param name="parentBlock">The parent block.</param>
            /// <param name="scanStart">The scan start.</param>
            /// <param name="scanEnd">The scan end.</param>
            /// <returns>nearest successor in given range</returns>
            private static int GetNearestSuccessorInRange(
                Block parentBlock,
                int scanStart,
                int scanEnd,
                int searchStart)
            {
                Block firstBlock = null;
                int startInstructionIndex =
                    parentBlock.Children[searchStart < 0 ? parentBlock.Children.Count - 1 : searchStart].BlockStartIndex;

                for (int i = scanStart; i <= scanEnd; i++)
                {
                    foreach (var successor in parentBlock.Children[i].Successors)
                    {
                        if (successor.BlockStartIndex < startInstructionIndex)
                        {
                            continue;
                        }

                        if (firstBlock == null ||
                            successor.BlockStartIndex < firstBlock.BlockStartIndex)
                        {
                            firstBlock = successor;
                        }
                    }
                }

                return parentBlock.Children.IndexOf(firstBlock);
            }
        }
    }
}
