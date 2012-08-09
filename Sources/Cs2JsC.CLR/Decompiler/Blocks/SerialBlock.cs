namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

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
        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            if (this.Children.Count == 1)
            {
                return this.Children[0].ToAstNode(variableResolver);
            }
            else
            {
                if (this.Children.Count == 2)
                {
                    IlInstruction il1 = this.Children[0].GetInstructionAt(this.Children[0].InstructionCount - 1);
                    IlInstruction il2 = this.Children[1].GetInstructionAt(0);
                    OpCode loadOpCode = il2.OpCode;
                    OpCode storeOpCode = il1.OpCode;

                    if (loadOpCode == OpCode.LoadLocal && storeOpCode == OpCode.StoreLocal
                        && il2.OpCodeArgument.Equals(il1.OpCodeArgument))
                    {
                        var range = this.Context.GetRangeContaining((VariableDefinition)il2.OpCodeArgument, il2);
                        if (range.Item2 == il2)
                        {
                            // This is the case where we were using this variable for temporary purpose.
                            // Let's move child1 into child2.
                            Block child1 = this.Children[1];
                            while (child1.Children.Count != 0)
                            {
                                child1 = child1.Children[0];
                            }

                            child1 = child1.Parent;
                            child1.DeleteInstruction(child1.BlockStartIndex);
                            child1.AddChildAt(this.Children[0], 0);
                            this.RemoveChildAt(0);
                            child1.Children[0].Dissolve();
                            child1.DeleteInstruction(child1.Children[1].BlockStartIndex);

                            return this.Children[0].ToAstNode(variableResolver);
                        }
                    }
                }
                throw new NotImplementedException();
            }
        }
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
