using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.CLR.Decompiler.Blocks
{
    class ConditionalValueAssignmentBlock : Block
    {
        #region member variables
        private ConditionalValueAssignmentBlock(
            ConditionalValueBlock value,
            InstructionBlockCollection assignement)
            : base(new Block[]{value, assignement})
        {
            this.Value = value;
            this.Assignement = assignement;
        }

        public static ConditionalValueAssignmentBlock Create(
            Block parentBlock,
            int blockIndex)
        {
            if (parentBlock.Children[blockIndex] is ConditionalValueBlock)
            {
                ConditionalValueBlock value = (ConditionalValueBlock)parentBlock.Children[blockIndex];

                if (value.Successors[0].StackBefore == value.StackAfter &&
                    value.Successors[0] == parentBlock.Children[blockIndex + 1])
                {
                    return new ConditionalValueAssignmentBlock(
                        value,
                        (InstructionBlockCollection)value.Successors[0]);
                }
            }

            return null;
        }
        #endregion

        #region constructors/Factories
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ConditionalValueBlock Value
        { get; private set; }

        public InstructionBlockCollection Assignement
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
