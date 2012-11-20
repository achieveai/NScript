using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class StackedBlock : BasicBlock
    {
        #region member variables

        #endregion

        #region constructors/Factories
        public StackedBlock(InstructionBlock rootBlock, params BasicBlock[] blocks)
            : base(StackedBlock.MergeArray(blocks, rootBlock))
        {
            this.DependentBlocks = blocks;
            this.RootBlock = rootBlock;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public InstructionBlock RootBlock
        { get; private set; }

        public BasicBlock[] DependentBlocks
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static BasicBlock[] MergeArray(BasicBlock[] array1, BasicBlock block)
        {
            BasicBlock[] array2 = new BasicBlock[array1.Length + 1];
            array1.CopyTo(array2, 0);
            array2[array1.Length] = block;

            return array2;
        }
        #endregion
    }
}
