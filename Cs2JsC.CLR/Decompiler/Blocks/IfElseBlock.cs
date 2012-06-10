namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class IfElseBlock : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        internal IfElseBlock(
            Block condition,
            Block ifBlock,
            Block elseBlock)
            : base(new Block[]{condition, ifBlock, elseBlock})
        {
        }

        internal IfElseBlock(
            Block condition,
            Block ifBlock)
            : base(new Block[]{condition, ifBlock})
        {
        }

        public static Block Create(
            Block parentBlock,
            int blockIndex)
        {
            // If statement will provide two next locations.
            // 1. If block
            // 2. Else block, or Continuation point if there is no else block.
            //
            // To check if second block is continuation block, we need to check if
            // it is If block's successor. Now there is yet another set of problems.
            // 
            // We can exit these blocks by
            // 1. natural flow, This is most common.
            // 2. Return statement, next common.
            // 3. break and continue statement.
            //
            // Natural flow is pretty simple to follow, it will lead to continuation block.
            // Return will have no successors.
            // Break will have block next to parent loop block.
            // Continue will either be start of conditional block (in while case) or some n blocks before
            // end which are increment blocks in "for" block.
            //

            if (!ConditionalStatement.IsConditional(parentBlock.Children[blockIndex]) ||
                parentBlock.Children[blockIndex].StackBefore > 0 ||
                parentBlock.Children[blockIndex].StackAfter > 0)
            {
                return null;
            }

            int[] indexes = new int[] {
                parentBlock.Children.IndexOf(parentBlock.Children[blockIndex].Successors[0]),
                parentBlock.Children.IndexOf(parentBlock.Children[blockIndex].Successors[1])};

            int ifBlockIndex =indexes[0];
            int secondBlockIndex = indexes[1];

            Block firstIfBlock = parentBlock.Children[blockIndex].Successors[0];
            Block continuationBlock = parentBlock.Children[blockIndex].Successors[1];

            if (secondBlockIndex < ifBlockIndex &&
                secondBlockIndex != -1)
            {
                int tmp = ifBlockIndex;
                ifBlockIndex = secondBlockIndex;
                secondBlockIndex = tmp;

                var tmpBlock = firstIfBlock;
                firstIfBlock = continuationBlock;
                continuationBlock = tmpBlock;
            }

            if (ifBlockIndex < blockIndex)
            {
                return null;
            }

            List<Block> rangeSuccessors = IfElseBlock.GetIfElseBlocksSuccessors(
                parentBlock,
                blockIndex,
                ifBlockIndex,
                secondBlockIndex);

            // The secondBlockIndex is in the list of successors for ifBlock.
            // This means that there is no else block.
            //
            if (rangeSuccessors.Count == 0 ||
                rangeSuccessors[0] == continuationBlock)
            {
                // TODO: We need to check for break and continue statements, but we don't really care
                // about them right now.
                //

                secondBlockIndex = secondBlockIndex == -1 ? parentBlock.Children.Count : secondBlockIndex;

                List<Block> ifBlockMembers = new List<Block>();
                for (int i = ifBlockIndex; i < secondBlockIndex; i++)
                {
                    ifBlockMembers.Add(parentBlock.Children[i]);
                }

                if (ifBlockMembers.Count == 0)
                {
                    throw new NotSupportedException();
                }

                Block ifBlock = new Block(ifBlockMembers.ToArray());

                return new IfElseBlock(
                    parentBlock.Children[blockIndex],
                    ifBlock);
            }
            else
            {
                int continousBlockIndex = parentBlock.Children.IndexOf(rangeSuccessors[0]);

                if (continousBlockIndex < secondBlockIndex && continousBlockIndex != -1)
                {
                    return null;
                }

                rangeSuccessors = IfElseBlock.GetIfElseBlocksSuccessors(
                    parentBlock,
                    blockIndex,
                    secondBlockIndex,
                    continousBlockIndex);

                if (rangeSuccessors.Count != 0 && parentBlock.Children.IndexOf(rangeSuccessors[0]) != continousBlockIndex)
                {
                    return null;
                }

                int ifBlockEnd = secondBlockIndex == -1 ? parentBlock.Children.Count : secondBlockIndex;

                List<Block> ifBlockMembers = new List<Block>();
                for (int i = ifBlockIndex; i < ifBlockEnd; i++)
                {
                    ifBlockMembers.Add(parentBlock.Children[i]);
                }

                if (ifBlockMembers.Count == 0)
                {
                    throw new NotSupportedException();
                }

                List<Block> elseBlockMembers = new List<Block>();
                int elseBlockEnd = continousBlockIndex == -1 ? parentBlock.Children.Count : continousBlockIndex;

                for (int i = secondBlockIndex; i < elseBlockEnd; i++)
                {
                    elseBlockMembers.Add(parentBlock.Children[i]);
                }

                Block ifBlock = new Block(ifBlockMembers.ToArray());
                Block elseBlock = new Block(elseBlockMembers.ToArray());

                return new IfElseBlock(
                    parentBlock.Children[blockIndex],
                    ifBlock,
                    elseBlock);
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Block Condition
        {
            get { return this.Children[0]; }
        }

        public Block IfBlock
        {
            get { return this.Children[1]; }
        }

        public Block ElseBlock
        {
            get
            {
                if (this.Children.Count == 3)
                {
                    return this.Children[2];
                }

                return null;
            }
        }
        #endregion

        #region public functions
        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.Expression condition = (AST.Expression)this.Condition.ToAstNode(variableResolver);
            if (this.StackAfter == this.StackBefore + 1)
            {
                // This is the case when if statement may be representing conditional expression or complex condition.
                AST.Expression leftExpression = (AST.Expression)this.IfBlock.Children[0].ToAstNode(variableResolver);
                AST.Expression rightExpression = null;

                // It is possible that else clause may also terminate with break instruction.
                if (this.ElseBlock.GetType() == typeof(Block) &&
                    this.ElseBlock.Children.Count == 2 &&
                    this.ElseBlock.Children[1].GetInstructionAt(0).FlowType == FlowType.Branch)
                {
                    rightExpression = (AST.Expression)this.ElseBlock.Children[0].ToAstNode(variableResolver);
                }
                else
                {
                    AST.Node node = this.ElseBlock.ToAstNode(variableResolver);
                    if (node is AST.ExpressionStatement)
                    {
                        rightExpression = ((AST.ExpressionStatement)node).Expression;
                    }
                    else if (node is AST.Expression)
                    {
                        rightExpression = (AST.Expression)node;
                    }
                    else
                    {
                        throw new InvalidOperationException("Don't know how to decompile this block");
                    }
                }

                return new AST.ConditionalOperatorExpression(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    condition,
                    leftExpression,
                    rightExpression);
            }
            else
            {
                AST.ScopeBlock trueStatements = null;
                AST.ScopeBlock falseStatements = null;

                AST.Statement trueBlock = AST.Statement.ToStatement(this.IfBlock.ToAstNode(variableResolver));
                if (trueBlock is AST.ScopeBlock)
                {
                    trueStatements = (AST.ScopeBlock)trueBlock;
                }
                else
                {
                    trueStatements = new AST.ScopeBlock(this.Context.ClrContext, trueBlock.Location);
                    trueStatements.AddStatement(trueBlock);
                }

                if (this.ElseBlock != null)
                {
                    AST.Statement falseScope = AST.Statement.ToStatement(this.ElseBlock.ToAstNode(variableResolver));

                    if (falseScope is AST.ScopeBlock)
                    {
                        falseStatements = (AST.ScopeBlock)falseScope;
                    }
                    else
                    {
                        falseStatements = new AST.ScopeBlock(falseScope.Context, falseScope.Location);
                        falseStatements.AddStatement(falseScope);
                    }
                }

                return new AST.IfBlockStatement(
                    this.Context.ClrContext,
                    this.ComputeLocation(),
                    condition,
                    trueStatements,
                    falseStatements);
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions\
        private static bool IsBreakStatement(
            Block currentBlock,
            Block block)
        {
            Block parent = currentBlock.Parent;

            while (parent != null)
            {
                if (parent is DoWhileBlock)
                {
                    return parent.Successors[0] == block;
                }
                parent = parent.Parent;
            }

            return false;
        }

        private static bool IsContinueStatement(
            Block currentBlock,
            Block block)
        {
            Block parent = currentBlock.Parent;

            while (parent != null)
            {
                if (parent is DoWhileBlock)
                {
                    var whileBlock = (DoWhileBlock)parent;
                    return whileBlock.Condition == block;
                }

                parent = parent.Parent;
            }

            return false;
        }

        private static List<Block> GetIfElseBlocksSuccessors(
            Block parentBlock,
            int ifBlockIndex,
            int startIndex,
            int endIndex)
        {
            var kvPair = parentBlock.GetRangePredecessorAndSuccessor(
                startIndex,
                (endIndex == -1 ? parentBlock.Children.Count : endIndex) - 1);

            if (!kvPair.HasValue)
            {
                return null;
            }

            var psPair = kvPair.Value;

            // IfBlock should only have one and only one predecessor and that predecessor is ConditionalStatement
            //
            if (psPair.Key.Count != 1 &&
                psPair.Key[0] != parentBlock.Children[ifBlockIndex])
            {
                return null;
            }

            for (int i = psPair.Value.Count - 1; i >= 0; i--)
            {
                if (IfElseBlock.IsBreakStatement(
                        parentBlock.Children[ifBlockIndex],
                        psPair.Value[i]) ||
                    IfElseBlock.IsContinueStatement(
                        parentBlock.Children[ifBlockIndex],
                        psPair.Value[i]))
                {
                    psPair.Value.RemoveAt(i);
                }
            }

            if (psPair.Value.Count > 1)
            {
                return null;
            }

            return psPair.Value;
        }

        protected override void WriteHeadToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine("{0} If-Block Begin", linePrefix);
            base.WriteHeadToStream(writer, linePrefix);
        }

        protected override void WriteBodyToStream(System.IO.TextWriter writer, string linePrefix)
        {
            string linePrefix2 = linePrefix + " .  ";
            writer.WriteLine("{0} Condition", linePrefix);
            this.Condition.Write(writer, linePrefix2);
            writer.WriteLine("{0} IfBlock", linePrefix);
            this.IfBlock.Write(writer, linePrefix2);

            if (this.ElseBlock != null)
            {
                writer.WriteLine("{0} ElseBlock", linePrefix);
                this.ElseBlock.Write(writer, linePrefix2);
            }
        }

        protected override void WriteTailToStream(System.IO.TextWriter writer, string linePrefix)
        {
            writer.WriteLine(
                "{0} If-Block End Id:{1}",
                linePrefix,
                this.Id);
        }
        #endregion
    }
}
