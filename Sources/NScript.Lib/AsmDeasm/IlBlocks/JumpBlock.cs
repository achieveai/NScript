using System;
namespace NScript.Lib.AsmDeasm.IlBlocks
{
    internal enum JumpType
    {
        Break,
        Continue,
        Goto // This is not used for timeBeing.
    }

    class JumpBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        internal JumpBlock(
            BasicBlock branchBlock,
            JumpType jumpType)
            :base(new Block[]{branchBlock})
        {
            var codeOp = branchBlock.GetInstructionAt(
                branchBlock.BlockEndIndex - branchBlock.BlockStartIndex).CodeOp;

            if (codeOp.Flow != Ops.FlowType.Branch &&
                codeOp.Flow != Ops.FlowType.ConditionalBranch)
            {
                throw new ArgumentException("branchBlock");
            }
            this.JumpType = jumpType;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public JumpType JumpType
        { get; private set; }

        public bool IsConditional
        { get { return this.Children[0] is StackedBlock; } }
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
