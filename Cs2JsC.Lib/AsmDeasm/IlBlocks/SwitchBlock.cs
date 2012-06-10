using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.AsmDeasm.Ops;
using Cs2JsC.Lib.ILParser;

namespace Cs2JsC.Lib.AsmDeasm.IlBlocks
{
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
        { get; private set; }

        public List<int> IntCases
        { get; private set; }

        public List<string> StringCases
        { get; private set; }

        public Block Default
        { get; private set; }

        public List<Block> AllCases
        { get; private set; }

        public int LocalVariableIndex
        { get; private set; }

        public Block Switch { get; private set; }
        #endregion

        #region public functions
        public override IList<Ast.IAstNode> ToExpressions(
            ExecutionContext executionContext,
            Stack<Ast.Expression> expressionStack)
        {
            List<KeyValuePair<List<object>, IList<Ast.IAstNode>>> values =
                new List<KeyValuePair<List<object>,IList<Ast.IAstNode>>>();

            var switchStatement = this.Switch.ToExpressions(
                executionContext,
                new Stack<Ast.Expression>());

            foreach (var caseBlock in this.AllCases)
            {
                List<object> caseValue = new List<object>();

                foreach (var kvPair in this.CaseBlocks)
                {
                    object key = null;
                    if (kvPair.Value == caseBlock)
                    {
                        if (kvPair.Key == -1)
                        {
                            key = null;
                        }
                        else if (this.IntCases != null)
                        {
                            key = this.IntCases[kvPair.Key];
                        }
                        else
                        {
                            key = this.StringCases[kvPair.Key];
                        }

                        caseValue.Add(key);
                    }
                }

                values.Add(
                    new KeyValuePair<List<object>, IList<Ast.IAstNode>>(
                        caseValue,
                        caseBlock.ToExpressions(executionContext, new Stack<Ast.Expression>())));
            }

            return new Ast.IAstNode[]{
                new Ast.SwitchExpression(
                ((Ast.AssignementExpression)switchStatement[0]).Value,
                values)};
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
            writer.WriteLine("{0} Load local {1}", linePrefix, this.LocalVariableIndex);

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
