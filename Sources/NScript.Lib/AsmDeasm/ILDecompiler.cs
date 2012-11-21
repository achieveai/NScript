using System.Collections.Generic;
using NScript.Lib.AsmDeasm.IlBlocks;
using NScript.Lib.MetaData;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm
{
    class ILDecompiler
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ILDecompiler(ExecutionBlock executionBlock)
        {
            this.ArgumentList = executionBlock.Arguments;
            this.LocalVariables = executionBlock.Variables;
            this.ExecutionBlock = executionBlock;

            this.GenerateBasicBlocks();
            ILDecompiler.ProcessBlocks(this.RootBlock);

            //this.RootBlock.Write(
            //    Console.Out,
            //    "");
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ExecutionBlock ExecutionBlock
        { get; private set; }

        internal BlockContext Context
        { get; private set; }

        internal RootBlock RootBlock
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        internal IList<ArgumentSignature> ArgumentList
        { get; private set; }

        internal IList<ArgumentSignature> LocalVariables
        { get; private set; }
        #endregion

        #region private functions
        private void GenerateBasicBlocks()
        {
            this.RootBlock = RootBlock.Create(
                this.ArgumentList,
                this.LocalVariables,
                this.ExecutionBlock.Instructions,
                this.ExecutionBlock.LabelInstructionMap);

            this.Context = this.RootBlock.Context;
        }

        private static void ProcessBlocks(Block rootBlock)
        {
            CollapseConditionalBlocks(rootBlock);

            CreateWhileLoops(rootBlock);

            CreateIfElseBlocks(rootBlock);
        }

        private static void CollapseConditionalBlocks(Block rootBlock)
        {
            for (int i = 0; i < rootBlock.Children.Count; i++)
            {
                Block conditionalBlock = ConditionalStatement.CreateBlock(
                    rootBlock,
                    i);

                if (conditionalBlock == null)
                {
                    conditionalBlock = ConditionalValue.Create(
                        rootBlock,
                        i);
                }

                if (conditionalBlock == null)
                {
                    conditionalBlock = ConditionalValueAssignement.Create(
                        rootBlock,
                        i);
                }

                if (conditionalBlock != null)
                {
                    i = -1;
                }
            }
        }

        private static void CreateWhileLoops(Block rootBlock)
        {
            for (int i = 0; i < rootBlock.Children.Count; i++)
            {
                DoWhileBlock whileLoopBlock = WhileBlock.CreateBlock(rootBlock, i);

                if (whileLoopBlock == null)
                {
                    whileLoopBlock = DoWhileBlock.Create(rootBlock, i);
                }

                if (whileLoopBlock != null)
                {
                    ILDecompiler.ProcessBlocks(whileLoopBlock.Loop);

                    i = rootBlock.Children.IndexOf(whileLoopBlock);
                }
            }
        }

        private static void CreateIfElseBlocks(Block rootBlock)
        {
            for (int i = 0; i < rootBlock.Children.Count; i++)
            {
                var ifBlock = (IfElseBlock)IfElseBlock.Create(
                    rootBlock,
                    i);

                if (ifBlock != null)
                {
                    i = -1;
                    ILDecompiler.ProcessBlocks(ifBlock.IfBlock);

                    if (ifBlock.ElseBlock != null)
                    {
                        ILDecompiler.ProcessBlocks(ifBlock.ElseBlock);
                    }
                }
            }
        }
        #endregion
    }
}
