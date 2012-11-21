using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using NScript.Lib.AsmDeasm.Ops;
using System.IO;
using NScript.Lib.AsmDeasm.Ast;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.IlBlocks
{
    class Block
    {
        #region member variables
        private static int ids = 0;

        bool successorsDirty = true;
        bool predecessorsDirty = true;
        private readonly List<Block> successors = new List<Block>();
        private readonly List<Block> predecessors = new List<Block>();
        private readonly List<Block> children = new List<Block>();

        private readonly ReadOnlyCollection<Block> readonlySuccessors = null;
        private readonly ReadOnlyCollection<Block> readonlyPredecessors = null;

        readonly BlockContext context;
        #endregion

        #region constructors/Factories
        protected Block(BlockContext context)
        {
            this.context = context;

            this.Id = System.Threading.Interlocked.Increment(ref Block.ids);
            this.Children = new ReadOnlyCollection<Block>(this.children);
            this.readonlySuccessors = new ReadOnlyCollection<Block>(this.successors);
            this.readonlyPredecessors = new ReadOnlyCollection<Block>(this.predecessors);
        }

        public Block(Block[] childBlocks)
            : this(childBlocks[0].Context)
        {
            int minParentIndex = int.MaxValue;
            int maxParentIndex = int.MinValue;
            Block parentBlock = childBlocks[0].Parent;

            // Let's check if all the childBlocks have common parent.
            //
            for (int i = 0; i < childBlocks.Length; i++)
            {
                if (childBlocks[i].Parent != parentBlock)
                {
                    throw new InvalidOperationException();
                }

                int childIndex =
                    parentBlock.children.IndexOf(childBlocks[i]);

                minParentIndex = Math.Min(
                    childIndex,
                    minParentIndex);
                maxParentIndex = Math.Max(
                    childIndex,
                    maxParentIndex);
            }

            if (maxParentIndex - minParentIndex != childBlocks.Length - 1)
            {
                throw new InvalidOperationException("Can't add disjoint children");
            }

            this.Parent = childBlocks[0].Parent;

            for (int i = 0; i < childBlocks.Length; i++)
            {
                this.AddChild(childBlocks[i]);
            }
            parentBlock.children.RemoveRange(minParentIndex, childBlocks.Length);
            parentBlock.children.Insert(minParentIndex, this);
        }

        public Block(Block block)
            : this (block.Context)
        {
            this.AddChild(block);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public int Id
        { get; private set; }

        public Block Parent
        { get; private set; }

        public ReadOnlyCollection<Block> Children
        { get; private set; }

        public ReadOnlyCollection<Block> Successors
        {
            get
            {
                if (this.SuccessorsDirty)
                {
                    this.ComputeSuccessors();
                    this.successorsDirty = false;
                }

                return this.readonlySuccessors;
            }
        }

        public ReadOnlyCollection<Block> Predecessors
        {
            get
            {
                if (this.predecessorsDirty)
                {
                    this.ComputePredecessors();
                    this.predecessorsDirty = false;
                }

                return this.readonlyPredecessors;
            }
        }

        public virtual int StackBefore
        { get { return this.Children.Count > 0 ? this.Children[0].StackBefore : -1; } }

        public virtual int StackAfter
        { get { return this.Children.Count > 0 ? this.Children[this.Children.Count - 1].StackAfter : -1; } }

        public virtual int BlockStartIndex
        {
            get
            {
                if (this.children.Count > 0)
                {
                    return this.Children[0].BlockStartIndex;
                }

                return -1;
            }
        }

        public virtual int BlockEndIndex
        {
            get
            {
                if (this.Children.Count > 0)
                {
                    return this.Children[this.Children.Count - 1].BlockEndIndex;
                }

                return -1;
            }
        }

        public BlockContext Context
        { get { return this.context; } }

        public int InstructionCount
        { get { return this.BlockEndIndex - this.BlockStartIndex + 1; } }
        #endregion

        #region public functions
        public bool IsChild(Block block)
        {
            var parent = block.Parent;
            while (parent != null)
            {
                if (parent == this)
                {
                    return true;
                }
                parent = parent.Parent;
            }

            return false;
        }

        public void AddChildBlock(Block block)
        {
            this.AddChild(block);
        }

        public virtual void DeleteInstruction(int instructionIndex)
        {
            if (instructionIndex < this.BlockStartIndex ||
                instructionIndex > this.BlockEndIndex)
            {
                throw new ArgumentOutOfRangeException("instructionIndex");
            }

            foreach (var child in this.children)
            {
                if (child.BlockEndIndex >= instructionIndex &&
                    child.BlockStartIndex <= instructionIndex)
                {
                    child.DeleteInstruction(instructionIndex);
                    break;
                }
            }
        }

        public virtual void InsertInstruction(Instruction instruction)
        {
            if (instruction.Index < this.BlockStartIndex ||
                instruction.Index > this.BlockEndIndex)
            {
                throw new InvalidOperationException();
            }

            for (int i = 0; i < this.children.Count; i++)
            {
                if (instruction.Index >= this.children[i].BlockStartIndex &&
                    instruction.Index <= (this.children[i].BlockEndIndex + 1))
                {
                    this.children[i].InsertInstruction(instruction);
                    return;
                }
            }
        }

        /// <summary>
        /// Dissolve will dissolve this block and move all the children to the parent.
        /// The children can override this method in case they can't be dissolved, one
        /// such example is InstructionBlock.
        /// </summary>
        public virtual void Dissolve()
        {
            this.Parent.children.InsertRange(
                this.Parent.Children.IndexOf(this),
                this.children);

            for (int iChild = 0; iChild < this.children.Count; iChild++)
            {
                this.children[iChild].Parent = this.Parent;
                this.children[iChild].SetPredecessorDirty();
                this.children[iChild].SetSuccessorDirty();
            }

            this.Parent.children.Remove(this);
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat(
                "Id: {0}, Position: [{1:d3},{2:d3}] Stack: [{3}:{4}], Successors: ",
                this.Id,
                this.BlockStartIndex,
                this.BlockEndIndex,
                this.StackBefore,
                this.StackAfter);

            for (int i = 0; i < this.Successors.Count; i++)
            {
                strBuilder.Append(this.Successors[i].Id);
                strBuilder.Append(',');
            }

            if (this.Predecessors.Count > 0)
            {
                strBuilder.Append("Predecessor: ");
                for (int i = 0; i < this.Predecessors.Count; i++)
                {
                    strBuilder.Append(this.Predecessors[i].Id);
                    strBuilder.Append(',');
                }
            }

            return strBuilder.ToString();
        }

        public void Write(
            TextWriter writer,
            string linePrefix)
        {
            this.WriteHeadToStream(writer, linePrefix);
            this.WriteBodyToStream(writer, linePrefix);
            this.WriteTailToStream(writer, linePrefix);
        }

        public virtual Instruction GetInstructionAt(int instructionIndex)
        {
            return this.Context.Instructions[this.BlockStartIndex + instructionIndex];
        }

        public void SetSuccessorDirty()
        {
            this.successorsDirty = true;

            if (this.OnSuccessorDirty != null)
            {
                this.OnSuccessorDirty(this);
            }

            if (this.Parent != null)
            {
                this.Parent.SetSuccessorDirty();
            }

            this.successors.Clear();
        }

        public void SetPredecessorDirty()
        {
            this.predecessorsDirty = true;

            if (this.OnPredecessorDirty != null)
            {
                this.OnPredecessorDirty(this);
            }

            if (this.Parent != null)
            {
                if (this == this.Parent.Children[0])
                {
                    // This is only way when parent's predecessors could get
                    // dirty.
                    //
                    this.Parent.SetPredecessorDirty();
                }
            }
        }

        /// <summary>
        /// Checks if all the blocks between startIndex and endIndex (both included)
        /// result in a bigger block with only one successor.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public bool IsInNonBranchingRangeBlock(int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex > this.Children.Count - 1)
            {
                throw new ArgumentException();
            }

            var kvPair = this.GetRangePredecessorAndSuccessor(
                startIndex,
                endIndex);

            if (kvPair.HasValue &&
                kvPair.Value.Value.Count <= 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// A block can move to successor from any point within itself. But nothing can come in to anything
        /// else than first block.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public KeyValuePair<List<Block>, List<Block>>? GetRangePredecessorAndSuccessor(
            int startIndex,
            int endIndex)
        {
            if (startIndex < 0 || endIndex > this.Children.Count - 1)
            {
                throw new ArgumentException();
            }

            for (int index = startIndex + 1; index <= endIndex; index++)
            {
                foreach (var predecessor in this.Children[index].Predecessors)
                {
                    if (predecessor.BlockStartIndex > this.Children[endIndex].BlockStartIndex ||
                        predecessor.BlockStartIndex < this.Children[startIndex].BlockStartIndex)
                    {
                        return null;
                    }
                }
            }

            List<Block> successorBlocks = new List<Block>();

            for (int index = startIndex; index <= endIndex; index++)
            {
                foreach (var successor in this.Children[index].Successors)
                {
                    if (successor.BlockStartIndex > this.Children[endIndex].BlockStartIndex ||
                        successor.BlockStartIndex < this.Children[startIndex].BlockStartIndex)
                    {
                        if (!successorBlocks.Contains(successor))
                        {
                            successorBlocks.Add(successor);
                        }
                    }
                }
            }

            return new KeyValuePair<List<Block>,List<Block>>(
                new List<Block>(this.Children[startIndex].Predecessors),
                successorBlocks);
        }

        public virtual IList<IAstNode> ToExpressions(
            ExecutionContext executionContext,
            Stack<Expression> expressionStack)
        {
            if (executionContext == null) throw new ArgumentNullException("executionContext");

            List<IAstNode> returnValue = new List<IAstNode>();

            foreach (var block in this.Children)
            {
                var val = block.ToExpressions(
                    executionContext,
                    expressionStack);

                if (val != null && expressionStack.Count == 0)
                {
                    returnValue.AddRange(val);
                }
            }

            return returnValue;
        }

        public T[] CreateChildrenArray<T>(
            int startIndex,
            int endIndex) where T : Block
        {
            T[] returnValue = new T[endIndex-startIndex];

            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = (T)this.Children[i + startIndex];
            }

            return returnValue;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        internal bool SuccessorsDirty
        {
            get
            {
                return this.successorsDirty;
            }
        }

        internal bool PredecessorsDirty
        {
            get
            {
                return this.predecessorsDirty;
            }
        }

        private event Action<Block> OnPredecessorDirty;
        private event Action<Block> OnSuccessorDirty;
        #endregion

        #region private functions
        protected virtual void ComputeSuccessors()
        {
            this.successors.Clear();

            foreach (var child in this.Children)
            {
                foreach (var childSuccessor in child.Successors)
                {
                    this.AddAdjecent(childSuccessor, false);
                }
            }
        }

        protected virtual void ComputePredecessors()
        {
            this.predecessors.Clear();

            foreach (var child in this.Children)
            {
                foreach (var childSuccessor in child.Predecessors)
                {
                    this.AddAdjecent(childSuccessor, true);
                }
            }
        }

        protected void AddAdjecent(
            Block block,
            bool isPredecessor)
        {
            int thisDepth = this.BlockDepth();
            int blockDepth = block.BlockDepth();

            int commonAncestorDepth = Math.Min(thisDepth, blockDepth);
            Block tentativeThisCommonAncestor = this;
            Block tentativeBlockCommonAncestor = block;
            Block successorBlock = block;

            for (int i = blockDepth; i > commonAncestorDepth; i--)
            {
                successorBlock = tentativeBlockCommonAncestor;
                tentativeBlockCommonAncestor = tentativeBlockCommonAncestor.Parent;

                // Successors are always first ones in their parent list.
                //
                if (!isPredecessor)
                {
                    if (tentativeBlockCommonAncestor.Children[0] != successorBlock &&
                        tentativeBlockCommonAncestor != this)
                    {
                        throw new InvalidProgramException("successor should either be first element in parent or should share same parent");
                    }
                }
            }

            for (int i = thisDepth; i > commonAncestorDepth; i--)
            {
                tentativeThisCommonAncestor = tentativeThisCommonAncestor.Parent;
            }

            while (tentativeThisCommonAncestor != tentativeBlockCommonAncestor)
            {
                successorBlock = tentativeBlockCommonAncestor;
                tentativeBlockCommonAncestor = tentativeBlockCommonAncestor.Parent;
                tentativeThisCommonAncestor = tentativeThisCommonAncestor.Parent;

                // Successors are always first ones in their parent list.
                //
                if (!isPredecessor)
                {
                    if (tentativeBlockCommonAncestor.Children[0] != successorBlock &&
                        tentativeBlockCommonAncestor != tentativeThisCommonAncestor)
                    {
                        throw new InvalidProgramException("successor should either be first element in parent or should share same parent");
                    }
                }
            }

            if (this != tentativeThisCommonAncestor)
            {
                if (isPredecessor)
                {
                    if (!this.predecessors.Contains(successorBlock))
                    {
                        this.predecessors.Add(successorBlock);
                        successorBlock.OnSuccessorDirty += this.SuccessorGotDirty;
                    }
                }
                else
                {
                    if (!this.successors.Contains(successorBlock))
                    {
                        this.successors.Add(successorBlock);
                        successorBlock.OnPredecessorDirty += this.PredecessorGotDirty;
                    }
                }
            }
        }

        protected virtual bool IsContigious(Block block)
        {
            if (!Block.AreNeighbors(this, block) && this.BlockStartIndex >= 0)
            {
                return false;
            }

            return true;
        }

        protected virtual void AddChild(Block block)
        {
            if (!this.IsContigious(block))
            {
                throw new ArgumentException("block be contigous to currently added block");
            }

            this.children.Add(block);

            block.SetPredecessorDirty();
            block.SetSuccessorDirty();

            block.Parent = this;

            this.SetPredecessorDirty();
            this.SetSuccessorDirty();
        }

        protected void AddChildAt(Block child, int index)
        {
            child.Parent = this;

            this.children.Insert(
                index,
                child);

            this.SetPredecessorDirty();
            this.SetSuccessorDirty();
        }

        protected void RemoveChildAt(int index)
        {
            this.children[index].SetPredecessorDirty();
            this.children[index].SetSuccessorDirty();
            this.children.RemoveAt(index);
        }

        private int BlockDepth()
        {
            int depth = 0;
            var tmp = this.Parent;
            while (tmp != null)
            {
                depth++;
                tmp = tmp.Parent;
            }

            return depth;
        }

/*
        private void InsertInstruction(
            int insertionPoint,
            Instruction instruction)
        {
            for (int i = insertionPoint; i < this.context.Instructions.Count; i++)
            {
                this.context.Instructions[i].Index++;
            }

            this.context.LabelToInstruction.Add(
                instruction.Label,
                instruction);
            this.context.Instructions.Insert(insertionPoint, instruction);
            instruction.Index = insertionPoint;
        }
*/

        private static bool AreNeighbors(
            Block block1,
            Block block2)
        {
            return (block1.BlockEndIndex + 1) == block2.BlockStartIndex ||
                block1.BlockStartIndex == (block2.BlockEndIndex + 1);
        }

        private void PredecessorGotDirty(Block block)
        {
            block.OnPredecessorDirty -= this.PredecessorGotDirty;
            this.SetSuccessorDirty();
        }

        private void SuccessorGotDirty(Block block)
        {
            block.OnSuccessorDirty -= this.SuccessorGotDirty;
            this.SetPredecessorDirty();
        }

        protected virtual void WriteHeadToStream(
            TextWriter writer,
            string linePrefix)
        {
            writer.Write(
                "{0} Id: {1}, Pos: [{2:d3},{3:d3}] Stack: [{4}:{5}]",
                linePrefix,
                this.Id,
                this.BlockStartIndex,
                this.BlockEndIndex,
                this.StackBefore,
                this.StackAfter);

            if (this.Successors.Count > 0)
            {
                writer.Write(" Successors: {0}", this.Successors[0].Id);
                for (int i = 1; i < this.Successors.Count; i++)
                {
                    writer.Write(",{0}", this.Successors[i].Id);
                }
            }

            if (this.Predecessors.Count > 0)
            {
                writer.Write(" Predecessors: {0}", this.Predecessors[0].Id);
                for (int i = 1; i < this.Predecessors.Count; i++)
                {
                    writer.Write(",{0}", this.Predecessors[i].Id);
                }
            }

            writer.WriteLine("");
        }

        protected virtual void WriteTailToStream(
            TextWriter writer,
            string linePrefix)
        {
            writer.WriteLine(
                "{0} End Id:{1}",
                linePrefix,
                this.Id);
        }

        protected virtual void WriteBodyToStream(
            TextWriter writer,
            string linePrefix)
        {
            linePrefix += " .  ";
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (i != 0)
                {
                    writer.WriteLine(linePrefix);
                }
                this.Children[i].Write(writer, linePrefix);
            }
        }
        #endregion
    }
}
