using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    class SerialBlock : BasicBlock
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public SerialBlock(BasicBlock[] startBlock)
            : base(startBlock)
        { }
        #endregion

        // ****************** Public  Stuff *****************************
        #region dependency properties
        #endregion

        #region properties
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region event handlers
        #endregion

        #region private functions
        protected override void AddChild(Block block)
        {
            if (block is BasicBlock)
            {
                base.AddChild(block);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        #endregion
    }
}
