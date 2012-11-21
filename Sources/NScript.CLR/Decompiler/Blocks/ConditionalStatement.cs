namespace NScript.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class ConditionalStatement : Block
    {
        #region member variables
        #endregion

        #region constructors/Factories
        internal ConditionalStatement(
            Block leftCondition,
            Block rightCondition)
            : base(new[] { leftCondition, rightCondition })
        {
            this.LeftCondition = leftCondition;
            this.RightCondition = rightCondition;
        }

        internal static ConditionalStatement CreateBlock(
            Block parentBlock,
            int currentBlockIndex)
        {
            if (ConditionalStatement.IsConditional(parentBlock.Children[currentBlockIndex]) &&
                currentBlockIndex < parentBlock.Children.Count - 1)
            {
                if (ConditionalStatement.IsConditional(parentBlock.Children[currentBlockIndex + 1]))
                {
                    // Let's try to combine both these conditionals. We can combine both of them
                    // if once combined, they will result in exactly 2 successors.
                    //
                    return ConditionalStatement.CombineConditionals(
                        parentBlock.Children[currentBlockIndex],
                        parentBlock.Children[currentBlockIndex + 1]);
                }
                
                if (ConditionalStatement.IsConditionalWithSingleBranch(
                    parentBlock.Children,
                    currentBlockIndex + 1))
                {
                    ConditionalStatement.FixConditionWithSingleBranch(
                        parentBlock,
                        currentBlockIndex + 1);

                    return ConditionalStatement.CombineConditionals(
                        parentBlock.Children[currentBlockIndex],
                        parentBlock.Children[currentBlockIndex + 1]);
                }
            }
            return null;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public bool IsDebug
        { get; private set; }

        public Block LeftCondition
        { get; private set; }

        public Block RightCondition
        { get; private set; }
        #endregion

        #region public functions
        /*
        public override IList<Ast.IAstNode> ToExpressions(
            ExecutionContext executionContext,
            Stack<Ast.Expression> expressionStack)
        {
            // We get two kind of expressions here.
            // 1. !A && B
            // 2. A || B
            //
            this.LeftCondition.ToExpressions(
                executionContext,
                expressionStack);

            this.RightCondition.ToExpressions(
                executionContext,
                expressionStack);

            var rightExpression = expressionStack.Pop();
            var leftExpression = expressionStack.Pop();

            bool isAnd = false;
            if (ConditionalStatement.GetTrueBranch(this.LeftCondition) != 
                ConditionalStatement.GetTrueBranch(this.RightCondition))
            {
                isAnd = true;
                leftExpression = BinaryExpression.NegateExpression(leftExpression);
            }

            var expression = new BinaryExpression(
                executionContext,
                isAnd ? BinaryOperator.BooleanAnd : BinaryOperator.BooleanOr,
                leftExpression, rightExpression);

            expressionStack.Push(expression);

            return new Expression[] { expression };
        }
         */

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>AST Node representing current block.</returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.Expression leftExpression = (AST.Expression)this.LeftCondition.ToAstNode(variableResolver);
            AST.Expression rightExpression = (AST.Expression)this.RightCondition.ToAstNode(variableResolver);

            bool isAnd = false;

            if (ConditionalStatement.GetFalseBranch(this.LeftCondition) ==
                ConditionalStatement.GetFalseBranch(this.RightCondition))
            {
                isAnd = true;
            }
            else
            {
                leftExpression = new AST.UnaryExpression(
                    this.Context.ClrContext,
                    leftExpression.Location,
                    leftExpression,
                    AST.UnaryOperator.LogicalNot);
            }

            return new AST.BinaryExpression(
                this.Context.ClrContext,
                this.ComputeLocation(),
                leftExpression,
                rightExpression,
                isAnd ? AST.BinaryOperator.LogicalAnd : AST.BinaryOperator.LogicalOr);
        }

        /// <summary>
        /// There are normally 2 kind of tails
        /// 1.
        /// IL_0025:  br.s       IL_0028
        ///     
        /// IL_0027:  ldc.i4.0
        /// IL_0028:  br.s       IL_002b
        /// IL_002a:  ldc.i4.1
        /// 
        /// IL_002b:  stloc.1
        /// 
        /// 2.
        /// IL_0025:  br.s       IL_002b
        ///     
        /// IL_002a:  ldc.i4.1
        /// 
        /// IL_002b:  stloc.1
        /// 
        /// In the above tails, 1st is used in more complex conditions and second one
        /// is used in somewhat simpler condition, e.g. X == a && Y == b.
        /// 
        /// In case of 1, tail block count will be 3, in 2, its going to be 1.
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="blockIndex"></param>
        /// <param name="blockCount"></param>
        /// <returns></returns>
        public static bool IsConditionalTail(
            IList<Block> blocks,
            int blockIndex,
            ref int blockCount)
        {
            var block = blocks[blockIndex];

            if (blockIndex >= blocks.Count - 1)
            {
                return false;
            }

            if (block.InstructionCount == 1 &&
                block.Successors.Count == 1 &&
                block.Successors[0] == blocks[blockIndex + 1] &&
                block.StackAfter == block.StackBefore + 1)
            {
                IlInstruction instruction = block.GetInstructionAt(0);

                if (instruction.OpCode != OpCode.LoadConstantInt32 ||
                    ((int)instruction.OpCodeArgument != 0 &&
                     (int)instruction.OpCodeArgument != 1))
                {
                    return false;
                }

                if (block.Successors[0].InstructionCount == 1 &&
                    block.Successors[0].Successors.Count == 1)
                {
                    instruction = block.Successors[0].GetInstructionAt(0);

                    if (instruction.OpCode == OpCode.Branch)
                    {
                        if (block.Successors[0].Successors[0] == blocks[blockIndex + 3])
                        {
                            if (ConditionalStatement.IsConditionalTail(
                                blocks,
                                blockIndex + 2,
                                ref blockCount))
                            {
                                blockCount = 3;
                                return true;
                            }
                        }
                    }
                }

                blockCount = 1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the false branch.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns>Branch for false condition (remember false condition is for the code, for IL it is true condition).</returns>
        public static Block GetFalseBranch(
            Block block)
        {
            if (block is ConditionalStatement)
            {
                return ConditionalStatement.GetFalseBranch(((ConditionalStatement)block).RightCondition);
            }
            
            return block.Successors[1];
        }

        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static ConditionalStatement CombineConditionals(
            Block block1,
            Block block2)
        {
            // Let's try to combine both these conditionals. We can combine both of them
            // if once combined, they will result in exactly 2 successors.
            //
            int common1stSuccessor = block1.Successors[0].Id;
            int secondSuccessor = -1;

            if (common1stSuccessor == block2.Id)
            {
                common1stSuccessor = block1.Successors[1].Id;
            }

            if (common1stSuccessor == block2.Successors[0].Id)
            {
                secondSuccessor = block2.Successors[1].Id;
            }
            else if (common1stSuccessor == block2.Successors[1].Id)
            {
                secondSuccessor = block2.Successors[0].Id;
            }
            else
            {
                return null;
            }

            return new ConditionalStatement(
                block1,
                block2);
        }

        public static bool IsConditional(
            Block block)
        {
            if (block.Successors.Count != 2)
            {
                return false;
            }

            if (block is ConditionalValueAssignmentBlock)
            {
                InstructionBlockCollection instructionsBlock = ((ConditionalValueAssignmentBlock)block).Assignement;

                if (instructionsBlock.GetInstructionAt(instructionsBlock.InstructionCount - 1)
                    .FlowType == FlowType.ConditionalBranch)
                {
                    return true;
                }
            }
            else if (block is ConditionalStatement)
            {
                return true;
            }
            else if (typeof(InstructionBlockCollection).IsInstanceOfType(block) ||
                typeof(Block).IsInstanceOfType(block))
            {
                if (block.GetInstructionAt(block.InstructionCount - 1)
                    .FlowType == FlowType.ConditionalBranch)
                {
                    return true;
                }
            }


            return false;
        }

        private static bool IsConditionalWithSingleBranch(
            IList<Block> blocks,
            int iBlock)
        {
            Block block = blocks[iBlock];

            // We need to have atleast 2 more blocks after this block
            // 1. loadConst, 2. Save or something
            //
            // Also there should be atleast 4 children.
            // 1. branch, 2. compare, 3-4, compare args.
            //
            if (iBlock >= blocks.Count - 3 ||
                block.InstructionCount < 4)
            {
                return false;
            }

            if (block is InstructionBlockCollection)
            {
                int branchIndex = block.InstructionCount - 1;

                if (block.GetInstructionAt(branchIndex).FlowType != FlowType.Branch)
                {
                    return false;
                }

                switch (block.GetInstructionAt(branchIndex - 1).OpCode)
                {
                    case OpCode.CompareEq:
                    case OpCode.CompareGt:
                    case OpCode.CompareUGt:
                    case OpCode.CompareLt:
                    case OpCode.CompareULt:
                        break;
                    default:
                        return false;
                }

                // Next one to the single branch conditional should be ConditionalTail
                //
                int tmp = 0;
                if (ConditionalStatement.IsConditionalTail(
                    blocks,
                    iBlock + 1,
                    ref tmp))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// IL_0023:  ldarg.2
        /// IL_0024:  ldc.i4.3
        /// IL_0025:  ceq
        /// IL_0027:  ldc.i4.0
        /// IL_0028:  ceq
        /// IL_002a:  br.s       IL_002d
        /// 
        /// IL_002c:  ldc.i4.1
        /// IL_002d:  br.s       IL_0030
        /// IL_002f:  ldc.i4.0
        /// IL_0030:  stloc.3
        ///
        /// To:
        /// 
        /// IL_0023:  ldarg.2
        /// IL_0024:  ldc.i4.3
        /// IL_0025:  beq.s      IL_002f
        /// 
        /// IL_002c:  ldc.i4.1
        /// IL_002d:  br.s       IL_0030
        /// IL_002f:  ldc.i4.0
        /// IL_0030:  stloc.3
        /// 
        /// </summary>
        /// <param name="blockCollection"></param>
        /// <returns></returns>
        private static void FixConditionWithSingleBranch(
            Block parentBlock,
            int iBlock)
        {
            int conditionalTailSize = 0;
            if (!ConditionalStatement.IsConditionalTail(
                parentBlock.Children,
                iBlock + 1,
                ref conditionalTailSize))
            {
                throw new InvalidOperationException("Can't fix single branch without conditional tail");
            }

            if (conditionalTailSize == 1)
            {
                ConditionalStatement.AddConditionalTail(
                    parentBlock,
                    iBlock + 1);
            }

            string branchLabel = parentBlock.Children[iBlock + 3].GetInstructionAt(0).Label;

            bool firstBlockIsTrue = true;
            if (parentBlock.Children[iBlock + 1].GetInstructionAt(0).OpCode == OpCode.LoadConstantInt32
                && (int)parentBlock.Children[iBlock + 1].GetInstructionAt(0).OpCodeArgument == 0)
            {
                firstBlockIsTrue = false;
            }

            bool hasNotCheck = false;
            var newOp = ConditionalStatement.GetBranchCode(
                (InstructionBlockCollection)parentBlock.Children[iBlock],
                ref hasNotCheck);

            IlInstruction instructionToReplace = null;
            if (hasNotCheck)
            {
                instructionToReplace = parentBlock.Children[iBlock]
                 .GetInstructionAt(parentBlock.Children[iBlock].InstructionCount - 4);
            }
            else
            {
                instructionToReplace = parentBlock.Children[iBlock]
                 .GetInstructionAt(parentBlock.Children[iBlock].InstructionCount - 2);
            }

            if (firstBlockIsTrue)
            {
                newOp = ConditionalBlock.GetNotBranch(newOp);
            }

            var newInstruction = new IlInstruction(
                null,
                newOp,
                instructionToReplace.Label,
                instructionToReplace.Index,
                branchLabel,
                instructionToReplace.Location);

            newInstruction.StackLevelBefore = instructionToReplace.StackLevelBefore;

            parentBlock.DeleteInstruction(newInstruction.Index + 1);
            if (hasNotCheck)
            {
                parentBlock.DeleteInstruction(newInstruction.Index + 1);
                parentBlock.DeleteInstruction(newInstruction.Index + 1);
            }

            parentBlock.Children[iBlock].DeleteInstruction(instructionToReplace.Index);
            parentBlock.Children[iBlock].InsertInstruction(newInstruction);
        }

        private static OpCode GetBranchCode(
            InstructionBlockCollection blockCollection,
            ref bool hasNotCheck)
        {
            if (blockCollection.GetInstructionAt(blockCollection.InstructionCount - 2).OpCode == OpCode.CompareEq
                && (int)blockCollection.GetInstructionAt(blockCollection.InstructionCount - 3).OpCodeArgument == 0)
            {
                hasNotCheck = true;
                return ConditionalBlock.ConvertCheckToBranch(
                    blockCollection.GetInstructionAt(blockCollection.InstructionCount - 4).OpCode,
                    true);
            }
            else
            {
                hasNotCheck = false;
                return ConditionalBlock.ConvertCheckToBranch(
                    blockCollection.GetInstructionAt(blockCollection.InstructionCount - 2).OpCode,
                    false);
            }
        }

        private static void AddConditionalTail(
            Block parentBlock,
            int blockIndex)
        {
            IlInstruction loadConstInstruction =
                parentBlock.Children[blockIndex].GetInstructionAt(0);

            IlInstruction newConstInstruction = new IlInstruction(
                null,
                OpCode.LoadConstantInt32,
                loadConstInstruction.Previous.Label + "_ContitionalTailAdd1",
                loadConstInstruction.Index,
                (int)loadConstInstruction.OpCodeArgument == 0 ?
                    1 : 0,
                loadConstInstruction.Location);

            newConstInstruction.StackLevelBefore = loadConstInstruction.StackLevelBefore;

            IlInstruction newBranchInstruction = new IlInstruction(
                null,
                OpCode.Branch,
                loadConstInstruction.Previous.Label + "_ContitionalTailAdd2",
                newConstInstruction.Index + 1,
                loadConstInstruction.Next.Label,
                loadConstInstruction.Location);

            newBranchInstruction.StackLevelBefore = loadConstInstruction.StackLevelAfter;

            parentBlock.InsertInstruction(newConstInstruction);
            parentBlock.InsertInstruction(newBranchInstruction);
        }
        #endregion
    }
}
