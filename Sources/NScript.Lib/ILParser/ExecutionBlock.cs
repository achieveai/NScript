using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NScript.Lib.AsmDeasm;
using NScript.Lib.AsmDeasm.Ops;
using NScript.Lib.MetaData;
using NScript.PELoader.Reflection;

namespace NScript.Lib.ILParser
{
    class ExecutionBlock
    {
        #region member variables
        private readonly List<string> lines = new List<string>();
        private readonly List<ArgumentSignature> variableNames = new List<ArgumentSignature>();
        private readonly ReadOnlyCollection<ArgumentSignature> readonlyVariableNames;

        private ILDecompiler decompiler;
        private JSCompiler jsCompiler;
        #endregion

        #region constructors/Factories
        internal ExecutionBlock(
            MethodSignature methodSignature,
            List<string> lines)
        {
            this.MethodSignature = methodSignature;
            this.Arguments = methodSignature.Arguments;
            this.PreProcessLines(lines);
            this.readonlyVariableNames =
                new ReadOnlyCollection<ArgumentSignature>(this.variableNames);

            this.BuildInstructions();

            if (this.Instructions.Count > 0)
            {
                this.SetStackLevel(
                    this.Instructions[0],
                    0,
                    new HashSet<Instruction>());

                this.RemoveDoubleBranches();
                this.RemoveUselessBranches();
                this.RemoveTempVariables();
                //this.FixDupInstructions();
                this.RemoveNoOps();
            }
        }

        public ExecutionBlock(MethodInfo methodInfo, List<string> lines)
            : this(methodInfo.Method.Signature, lines)
        {
            this.Method = methodInfo;
            NameResolver.Instance.AddMethodCompiler(
                this.Method.Method.Signature,
                this.Compiler);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public MethodInfo Method
        { get; private set; }

        public MethodSignature MethodSignature
        { get; private set; }

        public ReadOnlyCollection<ArgumentSignature> Variables
        { get { return this.readonlyVariableNames; } }

        public IList<ArgumentSignature> Arguments
        { get; private set; }

        public IList<Instruction> Instructions
        { get; private set; }

        public IDictionary<string, Instruction> LabelInstructionMap
        { get; private set; }

        public ILDecompiler Decompiler
        {
            get
            {
                if (this.decompiler == null && this.Instructions.Count > 0)
                {
                    this.decompiler = new ILDecompiler(this);
                }

                return this.decompiler;
            }
        }

        public JSCompiler Compiler
        {
            get
            {
                if (this.jsCompiler == null && this.Decompiler != null)
                {
                    var executionContext = new ExecutionContext(
                        this.MethodSignature,
                        this.Arguments,
                        this.Variables);

                    this.jsCompiler = new JSCompiler(
                        executionContext,
                        this.Decompiler.RootBlock);
                }

                return this.jsCompiler;
            }
        }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        private List<string> IlLines
        {
            get { return this.lines; }
        }

        private Dictionary<string, List<Instruction>> BranchTargets
        { get; set; }
        #endregion

        #region private functions
        private void ProcessLocalNames(string localDecls)
        {
            var word = ParseUtils.GetNextWord(localDecls, 0);

            if (word != ".locals")
            {
                throw new ArgumentException(
                    string.Format(
                        "{0} is not locals init string",
                        localDecls));
            }

            word = ParseUtils.GetNextWord(word);

            if (word != "init")
            {
                return;
            }

            var initBlock = GetInitBlock(word);

            word = ParseUtils.GetNextWord(initBlock, 0);

            do
            {
                // In most cases the init block contains locals as [x] int foo, ...
                // but if there is only one local, then the format is int foo
                //
                if (word[0] == '[')
                {
                    word = ParseUtils.GetNextWord(word);
                }
                string type = ILMethod.GetReturnType(ref word);
                string name = (string)(word);

                if (name.EndsWith(","))
                {
                    name = name.Substring(0, name.Length - 1);
                }

                this.variableNames.Add(
                    new ArgumentSignature(
                        type,
                        name,
                        this.variableNames.Count,
                        false,
                        false));
            } while (!StringFragment.IsNull(word = ParseUtils.GetNextWord(word)));
        }

        private static StringFragment GetInitBlock(StringFragment word)
        {
            int initBlockStartIndex = -1;
            for (int i = word.StartIndex; i < word.EffectiveParentLength; i++)
            {
                if (initBlockStartIndex == -1 && word.ParentString[i] == '(')
                {
                    initBlockStartIndex = i;
                }
                if (initBlockStartIndex >= 0 && word.ParentString[i] == ')')
                {
                    return new StringFragment(
                        word.ParentString,
                        i,
                        initBlockStartIndex + 1,
                        i - initBlockStartIndex - 1);
                }
            }

            return null;
        }

        private void PreProcessLines(IList<string> inputLines)
        {
            string currentLine = null;
            for (int iLine = 0; iLine < inputLines.Count; iLine++)
            {
                var word = ParseUtils.GetNextWord(inputLines[iLine], 0);

                if (StringFragment.IsNull(word))
                {
                    continue;
                }

                if (word.StartsWith("//") ||
                    word.StartsWith(".") ||
                    (word.StartsWith("IL_") && word[word.Length-1] == ':'))
                {
                    if (currentLine != null)
                    {
                        this.AddLine(currentLine);
                    }
                    currentLine = inputLines[iLine];
                }
                else
                {
                    if (currentLine != null)
                        currentLine = string.Format(
                            "{0} {1}",
                            currentLine.TrimEnd(),
                            inputLines[iLine].TrimStart());
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                this.AddLine(currentLine);
            }
        }

        private void AddLine(string line)
        {
            this.lines.Add(line);

            if (this.variableNames.Count == 0 &&
                ParseUtils.GetNextWord(line, 0).StartsWith(".locals"))
            {
                this.ProcessLocalNames(line);
            }
        }

        private void SetStackLevel(
            Instruction instruction,
            int stackLevel,
            HashSet<Instruction> visited)
        {
            if (instruction == null ||
                visited.Contains(instruction))
            {
                return;
            }

            visited.Add(instruction);

            instruction.StackBefore = stackLevel;

            switch (instruction.CodeOp.Flow)
            {
                case FlowType.Branch:
                    this.SetStackLevel(
                        this.LabelInstructionMap[(string)instruction.OpCodeArgument],
                        instruction.StackAfter,
                        visited);
                    break;
                case FlowType.ConditionalBranch:
                    this.SetStackLevel(
                        this.LabelInstructionMap[(string)instruction.OpCodeArgument],
                        instruction.StackAfter,
                        visited);
                    this.SetStackLevel(
                        instruction.Next,
                        instruction.StackAfter,
                        visited);
                    break;
                case FlowType.Throw:
                case FlowType.MethodCall:
                case FlowType.Next:
                    this.SetStackLevel(
                        instruction.Next,
                        instruction.StackAfter,
                        visited);
                    break;
                case FlowType.Switch:
                    this.SetStackLevel(
                        instruction.Next,
                        instruction.StackAfter,
                        visited);
                    foreach (var label in (string[])instruction.OpCodeArgument)
                    {
                        this.SetStackLevel(
                            this.LabelInstructionMap[label],
                            instruction.StackAfter,
                            visited);
                    }
                    break;
                case FlowType.Return:
                    break;
                case FlowType.Unsuported:
                    break;
                default:
                    break;
            }
        }

        private void BuildInstructions()
        {
            Dictionary<string, Instruction> instructionMap = new Dictionary<string, Instruction>();
            Dictionary<string, List<Instruction>> branchTargets = new Dictionary<string, List<Instruction>>();
            List<Instruction> instructions = new List<Instruction>();
            SourceCodeBlock lastCodeBlock = null;

            for (int iLine = 0; iLine < this.IlLines.Count; iLine++)
            {
                string label, opCodeStr, opCodeArg;

                if (!ExecutionBlock.BreakInstruction(
                    this.IlLines[iLine],
                    out label,
                    out opCodeStr,
                    out opCodeArg))
                {
                    if (ParseUtils.GetNextWord(this.IlLines[iLine], 0) == ".line")
                    {
                        lastCodeBlock = new SourceCodeBlock(
                            this.IlLines[iLine]);
                    }

                    continue;
                }

                var opCode = OpCodeWrapper.GetOpCode(opCodeStr);

                var instruction = new Instruction(
                    instructions.Count > 0 ? instructions[instructions.Count-1] : null,
                    opCode,
                    label,
                    instructions.Count,
                    Instruction.InitializeOpCodeArgument(
                        opCode.Code,
                        opCodeArg,
                        this.MethodSignature.IsStatic,
                        this.Arguments,
                        this.Variables),
                    lastCodeBlock);

                instruction = SimplifyInstruction(instruction);

                instructions.Add(instruction);
                instructionMap.Add(instruction.Label, instruction);

                switch (instruction.CodeOp.Code)
                {
                    case IlOpCode.BranchIfEqual:
                    case IlOpCode.BranchIfGreaterOrEqual:
                    case IlOpCode.BranchIfGreaterOrEqualUnsigned:
                    case IlOpCode.BranchIfGreater:
                    case IlOpCode.BranchIfGreaterUnsigned:
                    case IlOpCode.BranchIfLessOrEqual:
                    case IlOpCode.BranchIfLessOrEqualUnsigned:
                    case IlOpCode.BranchIfLessThan:
                    case IlOpCode.BranchIfLessThanUnsigned:
                    case IlOpCode.BranchIfNotEqualsUnsigned:
                    case IlOpCode.BranchIfFalse:
                    case IlOpCode.BranchIfTrue:
                    case IlOpCode.Branch:
                        if (!branchTargets.ContainsKey((string)instruction.OpCodeArgument))
                        {
                            branchTargets.Add((string)instruction.OpCodeArgument, new List<Instruction>());
                        }

                        branchTargets[(string)instruction.OpCodeArgument].Add(instruction);
                        break;
                }
            }

            this.Instructions = instructions;
            this.LabelInstructionMap = instructionMap;
            this.BranchTargets = branchTargets;
        }

        private static bool BreakInstruction(
            string instruction,
            out string label,
            out string opCode,
            out string opCodeArg)
        {
            label = null;
            opCode = null;
            opCodeArg = null;

            var word = ParseUtils.GetNextWord(instruction, 0);

            if (!word.StartsWith("IL_"))
            {
                return false;
            }

            label = ((string)word).Substring(0, word.Length-1);

            word = ParseUtils.GetNextWord(word);

            opCode = (string)word;

            word = ParseUtils.GetNextWord(word);

            opCodeArg =
                StringFragment.IsNull(word) ?
                null : instruction.Substring(word.StartIndex);

            return true;
        }

        private static Instruction SimplifyInstruction(
            Instruction instruction)
        {
            switch (instruction.CodeOp.Code)
            {
                case IlOpCode.LoadArgument0:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadArgument,
                        instruction.Label,
                        instruction.Index,
                        0,
                        instruction.SourceCode);
                case IlOpCode.LoadArgument1:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadArgument,
                        instruction.Label,
                        instruction.Index,
                        1,
                        instruction.SourceCode);
                case IlOpCode.LoadArgument2:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadArgument,
                        instruction.Label,
                        instruction.Index,
                        2,
                        instruction.SourceCode);
                case IlOpCode.LoadArgument3:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadArgument,
                        instruction.Label,
                        instruction.Index,
                        3,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt0:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        0,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt1:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        1,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt2:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        2,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt3:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        3,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt4:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        4,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt5:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        5,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt6:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        6,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt7:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        7,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantInt8:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        8,
                        instruction.SourceCode);
                case IlOpCode.LoadConstantIntNeg1:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadInt,
                        instruction.Label,
                        instruction.Index,
                        -1,
                        instruction.SourceCode);
                case IlOpCode.LoadLocal0:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocal,
                        instruction.Label,
                        instruction.Index,
                        0,
                        instruction.SourceCode);
                case IlOpCode.LoadLocal1:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocal,
                        instruction.Label,
                        instruction.Index,
                        1,
                        instruction.SourceCode);
                case IlOpCode.LoadLocal2:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocal,
                        instruction.Label,
                        instruction.Index,
                        2,
                        instruction.SourceCode);
                case IlOpCode.LoadLocal3:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocal,
                        instruction.Label,
                        instruction.Index,
                        3,
                        instruction.SourceCode);
                case IlOpCode.LoadLocalAddress0:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocalByRef,
                        instruction.Label,
                        instruction.Index,
                        0,
                        instruction.SourceCode);
                case IlOpCode.LoadLocalAddress1:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocalByRef,
                        instruction.Label,
                        instruction.Index,
                        1,
                        instruction.SourceCode);
                case IlOpCode.LoadLocalAddress2:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocalByRef,
                        instruction.Label,
                        instruction.Index,
                        2,
                        instruction.SourceCode);
                case IlOpCode.LoadLocalAddress3:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.LoadLocalByRef,
                        instruction.Label,
                        instruction.Index,
                        3,
                        instruction.SourceCode);
                case IlOpCode.StoreLocal0:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.StoreLocal,
                        instruction.Label,
                        instruction.Index,
                        0,
                        instruction.SourceCode);
                case IlOpCode.StoreLocal1:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.StoreLocal,
                        instruction.Label,
                        instruction.Index,
                        1,
                        instruction.SourceCode);
                case IlOpCode.StoreLocal2:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.StoreLocal,
                        instruction.Label,
                        instruction.Index,
                        2,
                        instruction.SourceCode);
                case IlOpCode.StoreLocal3:
                    return new Instruction(
                        instruction.Previous,
                        OpCodes.StoreLocal,
                        instruction.Label,
                        instruction.Index,
                        3,
                        instruction.SourceCode);
                default:
                    return instruction;
            }
        }

        private void RemoveDoubleBranches()
        {
            foreach (var instruction in this.Instructions)
            {
                if (instruction.CodeOp.Flow == FlowType.Branch ||
                    instruction.CodeOp.Flow == FlowType.ConditionalBranch)
                {
                    string branchTargetLabel = (string) instruction.OpCodeArgument;
                    if (this.LabelInstructionMap[branchTargetLabel].CodeOp.Code == IlOpCode.Branch)
                    {
                        // Let's also fix the branchTargets.
                        //
                        this.BranchTargets[branchTargetLabel].Remove(instruction);

                        if (this.BranchTargets[branchTargetLabel].Count == 0)
                        {
                            this.BranchTargets.Remove(branchTargetLabel);
                        }

                        instruction.SetArgument(
                            this.LabelInstructionMap[branchTargetLabel].OpCodeArgument);

                        branchTargetLabel = (string)instruction.OpCodeArgument;

                        this.BranchTargets[branchTargetLabel].Add(instruction);
                    }
                }
            }
        }

        private void RemoveUselessBranches()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                if (this.Instructions[i].CodeOp.Code == IlOpCode.Branch &&
                    this.Instructions[i + 1].Label == (string)this.Instructions[i].OpCodeArgument &&
                    this.BranchTargets[this.Instructions[i+1].Label].Count == 1 &&
                    !this.BranchTargets.ContainsKey(this.Instructions[i].Label))
                {
                    this.DeleteInstructionAt(i);
                    this.BranchTargets.Remove(this.Instructions[i].Label);
                }
            }
        }

        private void RemoveTempVariables()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                if (this.Instructions[i].CodeOp.Code == IlOpCode.StoreLocal &&
                    this.Instructions[i + 1].CodeOp.Code == IlOpCode.LoadLocal &&
                    (int)this.Instructions[i].OpCodeArgument == (int)this.Instructions[i + 1].OpCodeArgument)
                {
                    List<Instruction> branchTargets;
                    if (!this.IsVariableUsedWithoutChange((int)this.Instructions[i].OpCodeArgument, i + 2, new HashSet<int>()) &&
                        !this.BranchTargets.TryGetValue(
                            this.Instructions[i + 1].Label,
                            out branchTargets))
                    {
                        if (this.BranchTargets.TryGetValue(
                            this.Instructions[i].Label,
                            out branchTargets))
                        {
                            List<Instruction> newBranchTargets;
                            if (!this.BranchTargets.TryGetValue(
                                this.Instructions[i + 1].Label,
                                out newBranchTargets))
                            {
                                this.BranchTargets[this.Instructions[i + 1].Label] =
                                    branchTargets;
                            }
                            else
                            {
                                newBranchTargets.AddRange(branchTargets);
                            }

                            foreach (var branchTarget in branchTargets)
                            {
                                branchTarget.SetArgument(
                                    this.Instructions[i + 2].Label);
                            }
                        }

                        this.DeleteInstructionAt(i + 1);
                        this.DeleteInstructionAt(i);
                    }
                }
            }
        }

        private void FixDupInstruction(
            int index)
        {
            // Dup is equivalent to store, load, load.
            // For this purpose, we allocate new temp variable which then will be used for double loads
            //
            int tmpVariableIndex = this.AllocateTempVariable();

            Instruction storeInstruction = new Instruction(
                this.Instructions[index - 1],
                OpCodes.StoreLocal,
                string.Format("{0}_1", this.Instructions[index].Label),
                index,
                tmpVariableIndex,
                this.Instructions[index].SourceCode);

            storeInstruction.StackBefore = this.Instructions[index].StackBefore;

            Instruction loadInstruciton1 = new Instruction(
                storeInstruction,
                OpCodes.LoadLocal,
                string.Format("{0}_2", this.Instructions[index].Label),
                index + 1,
                tmpVariableIndex,
                storeInstruction.SourceCode);

            loadInstruciton1.StackBefore = storeInstruction.StackAfter;

            Instruction loadInstruciton2 = new Instruction(
                storeInstruction,
                OpCodes.LoadLocal,
                string.Format("{0}_3", this.Instructions[index].Label),
                index + 2,
                tmpVariableIndex,
                storeInstruction.SourceCode);

            loadInstruciton2.StackBefore = loadInstruciton1.StackAfter;

            for (int i = index + 1; i < this.Instructions.Count; i++)
            {
                this.Instructions[i].Index += 2;
            }

            this.Instructions[index].Next.Previous = loadInstruciton2;
            loadInstruciton2.Next = this.Instructions[index].Next;

            this.LabelInstructionMap[storeInstruction.Label] = storeInstruction;
            this.LabelInstructionMap[loadInstruciton1.Label] = loadInstruciton1;
            this.LabelInstructionMap[loadInstruciton2.Label] = loadInstruciton2;

            this.Instructions[index] = storeInstruction;
            this.Instructions.Insert(index + 1, loadInstruciton1);
            this.Instructions.Insert(index + 2, loadInstruciton2);
        }

        private void FixDupInstructions()
        {
            for (int iInstruction = 0; iInstruction < this.Instructions.Count; iInstruction++)
            {
                if (this.Instructions[iInstruction].CodeOp.Code == IlOpCode.Dup)
                {
                    this.FixDupInstruction(iInstruction);
                }
            }
        }

        private int AllocateTempVariable()
        {
            this.variableNames.Add(
                new ArgumentSignature(
                    "object",
                    string.Format("tmp${0}", this.variableNames.Count),
                    this.variableNames.Count,
                    false,
                    false));

            return this.variableNames.Count - 1;
        }

        private bool IsVariableUsedWithoutChange(
            int variableIndex,
            int startIndex,
            HashSet<int> instructionsVisisted)
        {
            if (instructionsVisisted.Contains(startIndex))
            {
                return false;
            }

            for (int iInstruction = startIndex; iInstruction < this.Instructions.Count && !instructionsVisisted.Contains(iInstruction); iInstruction++)
            {
                instructionsVisisted.Add(iInstruction);

                switch (this.Instructions[iInstruction].CodeOp.Code)
                {
                    case IlOpCode.BranchIfEqual:
                    case IlOpCode.BranchIfGreaterOrEqual:
                    case IlOpCode.BranchIfGreaterOrEqualUnsigned:
                    case IlOpCode.BranchIfGreater:
                    case IlOpCode.BranchIfGreaterUnsigned:
                    case IlOpCode.BranchIfLessOrEqual:
                    case IlOpCode.BranchIfLessOrEqualUnsigned:
                    case IlOpCode.BranchIfLessThan:
                    case IlOpCode.BranchIfLessThanUnsigned:
                    case IlOpCode.BranchIfNotEqualsUnsigned:
                    case IlOpCode.BranchIfFalse:
                    case IlOpCode.BranchIfTrue:
                        if (this.IsVariableUsedWithoutChange(
                            variableIndex,
                            this.LabelInstructionMap[(string)this.Instructions[iInstruction].OpCodeArgument].Index,
                            instructionsVisisted))
                        {
                            return true;
                        }
                        break;
                    case IlOpCode.Branch:
                        return this.IsVariableUsedWithoutChange(
                            variableIndex,
                            this.LabelInstructionMap[(string)this.Instructions[iInstruction].OpCodeArgument].Index,
                            instructionsVisisted);
                    case IlOpCode.LoadLocal:
                    case IlOpCode.LoadLocal0:
                    case IlOpCode.LoadLocal1:
                    case IlOpCode.LoadLocal2:
                    case IlOpCode.LoadLocal3:
                    case IlOpCode.LoadLocalAddress:
                    case IlOpCode.LoadLocalAddress0:
                    case IlOpCode.LoadLocalAddress1:
                    case IlOpCode.LoadLocalAddress2:
                    case IlOpCode.LoadLocalAddress3:
                        if ((int)this.Instructions[iInstruction].OpCodeArgument == variableIndex)
                        {
                            return true;
                        }
                        break;
                    case IlOpCode.StoreLocal:
                    case IlOpCode.StoreLocal0:
                    case IlOpCode.StoreLocal1:
                    case IlOpCode.StoreLocal2:
                    case IlOpCode.StoreLocal3:
                        if ((int)this.Instructions[iInstruction].OpCodeArgument == variableIndex)
                        {
                            return false;
                        }
                        break;
                    case IlOpCode.Return:
                        return false;
                    default:
                        break;
                }
            }
            return false;
        }

        private void DeleteInstructionAt(int i)
        {
            if (this.Instructions[i].Next != null)
            {
                this.Instructions[i].Next.Previous = this.Instructions[i].Previous;
            }

            if (this.Instructions[i].Previous != null)
            {
                this.Instructions[i].Previous.Next = this.Instructions[i].Next;
            }

            this.LabelInstructionMap.Remove(this.Instructions[i].Label);
            this.Instructions.RemoveAt(i);

            for (; i < this.Instructions.Count; ++i)
            {
                this.Instructions[i].Index--;
            }
        }

        private void RemoveNoOps()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                if (this.Instructions[i].CodeOp.Code == IlOpCode.Nop ||
                    this.Instructions[i].CodeOp.Code == IlOpCode.Box)
                {
                    List<Instruction> branchSources;

                    if (this.BranchTargets.TryGetValue(this.Instructions[i].Label, out branchSources))
                    {
                        foreach (var branch in branchSources)
                        {
                            branch.SetArgument(this.Instructions[i + 1].Label);
                        }
                    }

                    this.DeleteInstructionAt(i);
                }
            }
        }
        #endregion
    }
}
