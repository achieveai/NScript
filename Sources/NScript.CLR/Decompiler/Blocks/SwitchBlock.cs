namespace NScript.CLR.Decompiler.Blocks
{
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    partial class SwitchBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public SwitchBlock(
            Block[] nestedBlocks)
            :base(nestedBlocks)
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Dictionary<int, Block> CaseBlocks
        {
            get;
            private set;
        }

        public List<int> IntCases
        {
            get;
            private set;
        }

        public List<string> StringCases
        {
            get;
            private set;
        }

        public Block Default
        {
            get;
            private set;
        }

        public List<Block> AllCases
        {
            get;
            private set;
        }

        public object LocalVarOrArg
        {
            get;
            private set;
        }

        public Block Switch
        {
            get;
            private set;
        }
        #endregion

        #region public functions
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            List<KeyValuePair<List<AST.LiteralExpression>, AST.Statement>> values =
                new List<KeyValuePair<List<AST.LiteralExpression>,AST.Statement>>();

            AST.Expression switchValue;
            StackedBlock switchValueStack = this.Switch as StackedBlock;

            if (switchValueStack != null
                && switchValueStack.RootBlock.Instruction.OpCode == OpCode.StoreLocal
                && switchValueStack.RootBlock.Instruction.OpCodeArgument.Equals(this.LocalVarOrArg))
            {
                switchValue = (AST.Expression) switchValueStack.GetDependent(0).ToAstNode(variableResolver);
            }
            else if (switchValueStack != null && switchValueStack.RootBlock.Instruction.OpCode == OpCode.Switch)
            {
                if (switchValueStack.GetDependent(0) is StackedBlock)
                {
                    // This is the case when we are adjusting the switch argument to account for
                    // our case block's offset from zero.
                    // In this case we only need to load children[0].children[0].
                    switchValue = (AST.Expression)switchValueStack.GetDependent(0).Children[0].ToAstNode(variableResolver);
                }
                else
                {
                    switchValue = (AST.Expression)switchValueStack.GetDependent(0).ToAstNode(variableResolver);
                }
            }
            else
            {
                switchValue = (AST.Expression)this.Switch.ToAstNode(variableResolver);
            }

            foreach (var caseBlock in this.AllCases)
            {
                List<AST.LiteralExpression> caseValue = new List<AST.LiteralExpression>();

                foreach (var kvPair in this.CaseBlocks)
                {
                    AST.LiteralExpression key;

                    if (kvPair.Value == caseBlock)
                    {
                        if (kvPair.Key == -1)
                        {
                            key = null;
                        }
                        else if (this.IntCases != null)
                        {
                            key = new AST.IntLiteral(
                                this.Context.ClrContext,
                                null,
                                this.IntCases[kvPair.Key]);
                        }
                        else
                        {
                            key = new AST.StringLiteral(
                                this.Context.ClrContext,
                                null,
                                this.StringCases[kvPair.Key]);
                        }

                        caseValue.Add(key);
                    }
                }

                var astCaseBlock =
                    AST.Statement.ToStatement(caseBlock.ToAstNode(variableResolver));

                if (!(astCaseBlock is AST.ReturnStatement)
                    && !(astCaseBlock is AST.BreakStatement))
                {
                    AST.ScopeBlock scopeBlock = astCaseBlock as AST.ScopeBlock;

                    if (scopeBlock == null)
                    {
                        scopeBlock = new ScopeBlock(
                            this.Context.ClrContext,
                            astCaseBlock.Location);
                        scopeBlock.AddStatement(astCaseBlock);
                        scopeBlock.AddStatement(
                            new AST.BreakStatement(
                                this.Context.ClrContext,
                                astCaseBlock.Location));

                        astCaseBlock = scopeBlock;
                    }
                    else if (!(scopeBlock.Statements[scopeBlock.Statements.Count - 1] is AST.ReturnStatement)
                             && !(scopeBlock.Statements[scopeBlock.Statements.Count - 1] is AST.BreakStatement))
                    {
                        scopeBlock.AddStatement(
                            new AST.BreakStatement(
                                this.Context.ClrContext,
                                astCaseBlock.Location));
                    }
                }
                else if (caseValue.Count == 1
                    && caseValue[0] == null)
                {
                    if (astCaseBlock is AST.BreakStatement ||
                        this.BlockEndIndex == this.Context.Instructions.Count - 1)
                    {
                        // In this case we are check if we really need to emit default: case block
                        // we can skip emitting default: block if
                        // 1. default block only contains break.
                        // 2. default block only contains return and switch block is last block in this function.
                        continue;
                    }
                }

                values.Add(
                    new KeyValuePair<List<AST.LiteralExpression>, AST.Statement>(
                        caseValue,
                        astCaseBlock));
            }

            return new AST.SwitchStatement(
                this.Context.ClrContext,
                this.ComputeLocation(),
                switchValue,
                values);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        /// <summary>
        /// Writes the head to stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="linePrefix">The line prefix.</param>
        protected override void WriteHeadToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine("{0} Switch-Block Begin", linePrefix);
            base.WriteHeadToStream(writer, linePrefix);
        }

        /// <summary>
        /// Writes the body to stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="linePrefix">The line prefix.</param>
        protected override void WriteBodyToStream(System.IO.TextWriter writer, string linePrefix)
        {
            string linePrefix2 = linePrefix + " .  ";
            writer.WriteLine("{0} Load local {1}", linePrefix, this.LocalVarOrArg);

            HashSet<Block> casesPrinted = new HashSet<Block>();
            foreach (var caseBlock in this.AllCases)
            {
                foreach (var kvPair in this.CaseBlocks)
                {
                    object key = null;
                    if (kvPair.Value == caseBlock)
                    {
                        if (kvPair.Key == -1)
                        {
                            key = "default";
                        }
                        else if (this.IntCases != null)
                        {
                            key = this.IntCases[kvPair.Key];
                        }
                        else
                        {
                            key = this.StringCases[kvPair.Key];
                        }

                        writer.WriteLine("{0} Case: {1}", linePrefix, key);
                    }
                }

                caseBlock.Write(writer, linePrefix2);
            }
        }

        /// <summary>
        /// Writes the tail to stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="linePrefix">The line prefix.</param>
        protected override void WriteTailToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine(
                "{0} Switch-Block End Id:{1}",
                linePrefix,
                this.Id);
        }

        private static SwitchBlock CreateLargeStringSwitch(
            Block parentBlock,
            int blockIndex)
        {
            return null;
        }

        private static SwitchBlock CreateSmallStringSwitch(
            Block parentBlock,
            int blockIndex)
        {
            return null;
        }

        /// <summary>
        /// Initializes the break statements.
        /// </summary>
        private void InitializeBreakStatements()
        {
            if (this.Successors.Count == 0)
            {
                return;
            }

            foreach (var caseBlock in this.AllCases)
            {
                for (int i = 0; i < caseBlock.Children.Count; i++)
                {
                    if (caseBlock.Children[i].Successors.Count == 1 &&
                        caseBlock.Children[i].Children.Count == 1 &&
                        caseBlock.Children[i].Successors[0] == this.Successors[0])
                    {
                        var breakStatement = new SpecialBranch(
                            (InstructionBlockCollection)caseBlock.Children[i],
                            true);
                    }
                }
            }
        }
        #endregion
    }
}
