namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
    class ForBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ForBlock(
            Block fallinBlock,
            Block loopBlock,
            Block incrementBlock,
            Block conditionBlock)
            : base(new []{fallinBlock, loopBlock, incrementBlock, conditionBlock})
        {
            this.FallinBlock = fallinBlock;
            this.LoopBlock = loopBlock;
            this.IncrementBlock = incrementBlock;
            this.ConditionBlock = conditionBlock;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block ConditionBlock { get; set; }

        public Block IncrementBlock { get; set; }

        public Block LoopBlock { get; set; }

        public Block FallinBlock { get; set; }
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
