//-----------------------------------------------------------------------
// <copyright file="NullConditionalBlockBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for NullConditionalBlockBuilder
    /// </summary>
    internal static class NullConditionalBlockBuilder
    {
        /// <summary>
        /// Processes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        public static void Process(Block block)
        {
            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                NullConditionalBlockBuilder.Process(block.Children[childIndex]);
            }

            for (int childIndex = 0; childIndex < block.Children.Count; childIndex++)
            {
                NullConditionalBlockBuilder.Create(block, childIndex);
            }
        }

        /// <summary>
        /// Creates the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="childIndex">Index of the child.</param>
        private static void Create(
            Block block,
            int childIndex)
        {
            // Expression: dest.Property = trc1.Property ?? trc1;
            // is reduced to blocks below.
            // <BlockInfo id="1" type="RootBlock">
            //   <BlockInfo id="2" type="StackedBlock">
            //     <BlockInfo id="3" type="InstructionBlock" label="1" opcode="LoadArgument" argument="1"/>
            //     <BlockInfo id="4" type="SerialBlock"> -----> this is root of NullConditionBlock.
            //       <BlockInfo id="5" type="StackedBlock">
            //         <BlockInfo id="6" type="SerialBlock">
            //           <BlockInfo id="6" type="StackedBlock">
            //             <BlockInfo id="7" type="StackedBlock">
            //               <BlockInfo id="8" type="InstructionBlock" label="2" opcode="LoadArgument" argument="0"/>
            //               <BlockInfo id="9" type="InstructionBlock" label="3" opcode="CallVirtual" argument="NScript.PELoader.Reflection.MethodReference"/>
            //             </BlockInfo>
            //             <BlockInfo id="10" type="InstructionBlock" label="8" opcode="Dup" argument=""/>
            //           </BlockInfo>
            //           <BlockInfo id="11" type="StackedBlock">
            //             <BlockInfo id="12" type="InstructionBlock" label="8_copy" opcode="DupCopy" argument="8:[2:2] Dup: "/>
            //             <BlockInfo id="13" type="InstructionBlock" label="9" opcode="BranchTrue" argument="13"/>
            //           </BlockInfo>
            //         </BlockInfo>
            //         <BlockInfo id="14" type="InstructionBlock" label="11" opcode="Pop" argument=""/>
            //       </BlockInfo>
            //       <BlockInfo id="15" type="InstructionBlock" label="12" opcode="LoadArgument" argument="0"/>
            //     </BlockInfo>
            //     <BlockInfo id="16" type="InstructionBlock" label="13" opcode="CallVirtual" argument="NScript.PELoader.Reflection.MethodReference"/>
            //   </BlockInfo>
            //   <BlockInfo id="17" type="InstructionBlock" label="19" opcode="Return" argument=""/>
            // </BlockInfo>
            SerialBlock serialBlock = block.Children[childIndex] as SerialBlock;

            if (serialBlock == null
                || serialBlock.Children.Count != 2)
            {
                return;
            }

            StackedBlock stackedBlock = serialBlock.Children[0] as StackedBlock;
            if (stackedBlock == null
                || stackedBlock.RootBlock.Instruction.OpCode != OpCode.Pop
                || stackedBlock.DependentCount() != 1)
            {
                return;
            }

            SerialBlock serialBlock2 = stackedBlock.GetDependent(0) as SerialBlock;
            if (serialBlock2 == null
                || serialBlock2.Children.Count != 2)
            {
                return;
            }

            StackedBlock dupCopyBranch = serialBlock2.Children[1] as StackedBlock;

            if (dupCopyBranch == null
                || dupCopyBranch.RootBlock.Instruction.OpCode != OpCode.BranchTrue
                || dupCopyBranch.Children.Count != 2
                || dupCopyBranch.GetInstructionAt(0).OpCode != OpCode.DupCopy)
            {
                return;
            }

            StackedBlock dupBlock = serialBlock2.Children[0] as StackedBlock;

            if (dupBlock == null
                || dupBlock.RootBlock.Instruction.OpCode != OpCode.Dup)
            {
                return;
            }

            new NullConditionalBlock(
                (BasicBlock)dupBlock.GetDependent(0),
                (BasicBlock)serialBlock.Children[1],
                serialBlock);
        }
    }
}
