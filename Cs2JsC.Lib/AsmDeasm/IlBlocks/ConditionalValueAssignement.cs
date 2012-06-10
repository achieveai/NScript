using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    class ConditionalValueAssignement : Block
    {
        #region member variables
        private ConditionalValueAssignement(
            ConditionalValue value,
            InstructionBlockCollection assignement)
            : base(new Block[]{value, assignement})
        {
            this.Value = value;
            this.Assignement = assignement;
        }

        public static ConditionalValueAssignement Create(
            Block parentBlock,
            int blockIndex)
        {
            if (parentBlock.Children[blockIndex] is ConditionalValue)
            {
                ConditionalValue value = (ConditionalValue)parentBlock.Children[blockIndex];

                if (value.Successors[0].StackBefore == value.StackAfter &&
                    value.Successors[0] == parentBlock.Children[blockIndex + 1])
                {
                    return new ConditionalValueAssignement(
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
        public ConditionalValue Value
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
