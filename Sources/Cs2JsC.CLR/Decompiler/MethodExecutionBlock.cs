namespace Cs2JsC.CLR.Decompiler
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.CLR.Decompiler.Blocks;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Cs2JsC.Utils;

    public class MethodExecutionBlock
    {
        private readonly ClrContext clrContext;
        private readonly MethodDefinition methodDefinition;
        private readonly IList<ParameterDefinition> arguments;
        private readonly List<VariableDefinition> variables;
        private readonly MethodBody implementation;
        private readonly List<IlInstruction> instructions;
        private readonly Dictionary<Instruction, Location> locations =
            new Dictionary<Instruction, Location>();

        private readonly List<TypeReference> typeReferences =
            new List<TypeReference>();
        private readonly List<MemberReference> memberReferences =
            new List<MemberReference>();
        private readonly Dictionary<string, IlInstruction> labelInstructionMap =
            new Dictionary<string,IlInstruction>();
        private readonly Dictionary<string, List<IlInstruction>> branchTargets =
            new Dictionary<string, List<IlInstruction>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodExecutionBlock"/> class.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        public MethodExecutionBlock(
            ClrContext context,
            MethodDefinition methodDefinition)
        {
            this.clrContext = context;
            this.methodDefinition = methodDefinition;
            this.arguments = methodDefinition.Parameters;
            this.implementation = methodDefinition.Body;
            this.variables = new List<VariableDefinition>(this.implementation.Variables);

            this.instructions = new List<IlInstruction>();

            // Add all the type references for local variables.
            foreach (VariableDefinition localVariableDefinition in this.variables)
            {
                this.typeReferences.Add(localVariableDefinition.VariableType);
            }

            // Build instructions.
            this.BuildInstructions();

            if (this.Instructions.Count > 0)
            {
                HashSet<IlInstruction> visited = new HashSet<IlInstruction>();
                this.SetStackLevel(
                    this.Instructions[0],
                    0,
                    visited);
                this.SetExceptionHandlersStackLevel(visited);

                HashSet<string> exceptionHandlerPointOfInterest =
                    new HashSet<string>();

                foreach (var exceptionHandler in this.implementation.ExceptionHandlers)
                {
                    string tS = exceptionHandler.TryStart.Offset.ToString();
                    string tE = exceptionHandler.TryEnd.Offset.ToString();
                    string hS = exceptionHandler.HandlerStart.Offset.ToString();
                    string hE = exceptionHandler.HandlerEnd.Offset.ToString();

                    if (!exceptionHandlerPointOfInterest.Contains(tS))
                    {
                        exceptionHandlerPointOfInterest.Add(tS);
                    }
                    if (!exceptionHandlerPointOfInterest.Contains(tE))
                    {
                        exceptionHandlerPointOfInterest.Add(tE);
                    }

                    // There is no way we can have multiple handlers starting at same location.
                    exceptionHandlerPointOfInterest.Add(hS);
                    exceptionHandlerPointOfInterest.Add(hE);
                }

                this.FixDefaultValueInits();
                this.FixLoadStructAddressForVirtualCall();
                this.FixStructConstructorCalls();
                this.SkipDoubleBranches();
                this.RemoveUnusedBranches(exceptionHandlerPointOfInterest);
                this.RemoveTempVariables();
                this.RemoveNoOps(exceptionHandlerPointOfInterest);
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ClrContext Context
        {
            get { return this.clrContext; }
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public MethodDefinition Method
        {
            get
            {
                return this.methodDefinition;
            }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<ParameterDefinition> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>The variables.</value>
        public IList<VariableDefinition> Variables
        {
            get
            {
                return this.variables;
            }
        }

        /// <summary>
        /// Gets the instructions.
        /// </summary>
        /// <value>The instructions.</value>
        internal List<IlInstruction> Instructions
        {
            get
            {
                return this.instructions;
            }
        }

        /// <summary>
        /// Gets the label instruction map.
        /// </summary>
        /// <value>The label instruction map.</value>
        internal Dictionary<string, IlInstruction> LabelInstructionMap
        {
            get
            {
                return this.labelInstructionMap;
            }
        }

        /// <summary>
        /// Gets the branch targets.
        /// </summary>
        /// <value>The branch targets.</value>
        internal Dictionary<string, List<IlInstruction>> BranchTargets
        {
            get
            {
                return this.branchTargets;
            }
        }

        /// <summary>
        /// Gets the root block.
        /// </summary>
        /// <returns>RootBlock after block building.</returns>
        internal RootBlock GetRootBlock()
        {
            RootBlock rootBlock = RootBlock.Create(
                this.Instructions,
                this.LabelInstructionMap,
                this.methodDefinition,
                this.clrContext);

            rootBlock.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                InlineAssignmentBlockBuilder.Process,
                PostfixBlockBuilder.Process,
                OpAssignmentBlockBuilder.Process,
                NullConditionalBlockBuilder.Process,
                InlineObjectArrayCreateBlockBuilder.Process,
                InlinePropertyInitializerBuilder.Process,
                ExceptionHandlerBlockBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                DoWhileLoopBuilder.ProcessInfiniteLoop,
                IfElseBlockBuilder.Process,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues,
                JumpBlockBuilder.ProcessRemainingGotos,
                IfElseBlockBuilder.ReProcess,
                InlineDelegateBuilder.Process,
                ForEachBlockBuilder.Process);

            return rootBlock;
        }

        /// <summary>
        /// Toes the AST.
        /// </summary>
        /// <returns>Top level block</returns>
        public TopLevelBlock ToAST()
        {
            TopLevelBlock topBlock = new TopLevelBlock(
                this.methodDefinition);

            VariableResolver variableResolver = new VariableResolver(
                this,
                topBlock);

            RootBlock rootBlock = this.GetRootBlock();
            topBlock.RootBlock = (ParameterBlock)rootBlock.ToAstNode(variableResolver);
            topBlock.RootBlock.ProcessThroughPipeline(new AST.Processors.Processor(topBlock));

            if (rootBlock.Context.InlineDelegateInfo != null)
            {
                topBlock.InlineDelegateClass = rootBlock.Context.InlineDelegateInfo.ToAst(variableResolver);
            }

            return topBlock;
        }

        /// <summary>
        /// Maps the locations.
        /// </summary>
        private void MapLocations()
        {
            Location lastLocation = null;

            foreach (var instruction in this.implementation.Instructions)
            {
                if (instruction.SequencePoint != null)
                {
                    SequencePoint sequencePoint = instruction.SequencePoint;
                    lastLocation = 
                        new Location(
                            sequencePoint.Document.Url,
                            sequencePoint.StartLine,
                            sequencePoint.StartColumn,
                            sequencePoint.EndLine,
                            sequencePoint.EndColumn);
                }

                this.locations.Add(
                    instruction,
                    lastLocation);
            }
        }

        /// <summary>
        /// Builds the instructions.
        /// </summary>
        private void BuildInstructions()
        {
            IList<Instruction> instructionInfos = this.implementation.Instructions;
            int dupCopies = 0;

            this.MapLocations();

            for (int instructionIndex = 0; instructionIndex < instructionInfos.Count; instructionIndex++)
            {
                Instruction instructionInfo = instructionInfos[instructionIndex];
                OpCode opCode = OpCodeHelper.GetOpCode(instructionInfo.OpCode.Code);
                object argument = instructionInfo.Operand;

                IlInstruction.SimplifyOpCodeAndArgument(
                        this.methodDefinition,
                        ref opCode,
                        ref argument);
                string label = instructionInfo.Offset.ToString();

                for (int handlerIndex = 0; handlerIndex < this.implementation.ExceptionHandlers.Count; handlerIndex++)
                {
                    if (this.implementation.ExceptionHandlers[handlerIndex].HandlerType == ExceptionHandlerType.Catch
                        && this.implementation.ExceptionHandlers[handlerIndex].HandlerStart == instructionInfo)
                    {
                        // This is entry point for catch block, let's insert CatchEntry instruction.
                        IlInstruction catchEntry = new IlInstruction(
                            this.instructions[instructionIndex + dupCopies - 1],
                            OpCode.CatchEntry,
                            instructionInfo.Offset.ToString(),
                            instructionIndex + dupCopies,
                            null,
                            this.locations[instructionInfo]);

                        dupCopies++;
                        label = instructionInfo.Offset.ToString() + "_catchEntryfix";

                        this.instructions.Add(catchEntry);
                        this.labelInstructionMap.Add(catchEntry.Label, catchEntry);
                        this.AccumilateReferences(catchEntry);
                    }
                }

                IlInstruction instruction = new IlInstruction(
                    this.instructions.Count > 0
                        ? this.instructions[instructionIndex + dupCopies - 1]
                        : null,
                    opCode,
                    label,
                    instructionIndex + dupCopies,
                    argument,
                    this.locations[instructionInfo]);

                this.instructions.Add(instruction);
                this.labelInstructionMap.Add(instruction.Label, instruction);
                this.AccumilateReferences(instruction);

                // This is fix for Dup instruction so that our codeflow can analyze dup instructions
                // just fine.
                if (opCode == OpCode.Dup)
                {
                    dupCopies++;
                    instruction = new IlInstruction(
                        instruction,
                        OpCode.DupCopy,
                        instruction.Label + "_copy",
                        instructionIndex + dupCopies,
                        instruction,
                        this.locations[instructionInfos[instructionIndex]]);

                    this.instructions.Add(instruction);
                    this.labelInstructionMap.Add(instruction.Label, instruction);
                    this.AccumilateReferences(instruction);
                }

                switch (opCode)
                {
                    case OpCode.BranchEq:
                    case OpCode.BranchGe:
                    case OpCode.BranchUGe:
                    case OpCode.BranchGt:
                    case OpCode.BranchUGt:
                    case OpCode.BranchLe:
                    case OpCode.BranchULe:
                    case OpCode.BranchLt:
                    case OpCode.BranchULt:
                    case OpCode.BranchUNe:
                    case OpCode.BranchFalse:
                    case OpCode.BranchTrue:
                    case OpCode.Branch:
                    case OpCode.Leave:
                        if (!this.branchTargets.ContainsKey((string)argument))
                        {
                            this.branchTargets.Add(
                                (string)argument,
                                new List<IlInstruction>());
                        }

                        this.branchTargets[(string)argument].Add(instruction);
                        break;
                    case OpCode.Switch:
                        {
                            string[] strOffsets = (string[])argument;

                            for (int offsetIndex = 0; offsetIndex < strOffsets.Length; offsetIndex++)
                            {
                                if (!this.branchTargets.ContainsKey(strOffsets[offsetIndex]))
                                {
                                    this.branchTargets.Add(
                                        strOffsets[offsetIndex],
                                        new List<IlInstruction>());
                                }

                                this.branchTargets[strOffsets[offsetIndex]].Add(instruction);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the stack level.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <param name="stackLevel">The stack level.</param>
        /// <param name="visited">The visited.</param>
        private void SetStackLevel(
            IlInstruction instruction,
            int stackLevel,
            HashSet<IlInstruction> visited)
        {
            if (instruction == null || visited.Contains(instruction))
            {
                return;
            }

            visited.Add(instruction);

            instruction.StackLevelBefore = stackLevel;

            switch (instruction.FlowType)
            {
                case FlowType.Branch:
                case FlowType.Leave:
                    this.SetStackLevel(
                        this.labelInstructionMap[(string)instruction.OpCodeArgument],
                        instruction.StackLevelAfter,
                        visited);
                    break;
                case FlowType.ConditionalBranch:
                    this.SetStackLevel(
                        this.labelInstructionMap[(string)instruction.OpCodeArgument],
                        instruction.StackLevelAfter,
                        visited);
                    this.SetStackLevel(
                        instruction.Next,
                        instruction.StackLevelAfter,
                        visited);
                    break;
                case FlowType.Throw:
                case FlowType.MethodCall:
                case FlowType.Next:
                    this.SetStackLevel(
                        instruction.Next,
                        instruction.StackLevelAfter,
                        visited);
                    break;
                case FlowType.Switch:
                    this.SetStackLevel(
                        instruction.Next,
                        instruction.StackLevelAfter,
                        visited);
                    foreach (var label in (string[])instruction.OpCodeArgument)
                    {
                        this.SetStackLevel(
                            this.labelInstructionMap[label],
                            instruction.StackLevelAfter,
                            visited);
                    }
                    break;
                case FlowType.Return:
                    break;
                case FlowType.Unsuported:
                    throw new NotSupportedException();
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the exception handlers stack level.
        /// </summary>
        /// <param name="visited">The visited.</param>
        private void SetExceptionHandlersStackLevel(
            HashSet<IlInstruction> visited)
        {
            foreach (var handler in this.implementation.ExceptionHandlers)
            {
                // For catch block, we have exception already sitting on the stack.
                this.SetStackLevel(
                    this.labelInstructionMap[handler.HandlerStart.Offset.ToString()],
                    0,
                    visited);
            }
        }

        /// <summary>
        /// Skips the double branches.
        /// </summary>
        private void SkipDoubleBranches()
        {
            foreach (IlInstruction instruction in this.instructions)
            {
                if (instruction.FlowType == FlowType.Branch ||
                    instruction.FlowType == FlowType.ConditionalBranch)
                {
                    string branchTargetLabel = (string)instruction.OpCodeArgument;

                    if (this.labelInstructionMap[branchTargetLabel].OpCode == OpCode.Branch)
                    {
                        this.branchTargets[branchTargetLabel].Remove(instruction);

                        if (this.branchTargets[branchTargetLabel].Count == 0)
                        {
                            this.branchTargets.Remove(branchTargetLabel);
                        }

                        instruction.SetArgument(
                            this.labelInstructionMap[branchTargetLabel]
                                .OpCodeArgument);

                        branchTargetLabel = (string)instruction.OpCodeArgument;

                        this.branchTargets[branchTargetLabel].Add(instruction);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the unused branches.
        /// </summary>
        /// <param name="cantRemove">The cant remove.</param>
        private void RemoveUnusedBranches(HashSet<string> cantRemove)
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                if (this.Instructions[i].OpCode == OpCode.Branch &&
                    this.Instructions[i + 1].Label == (string)this.Instructions[i].OpCodeArgument &&
                    this.BranchTargets[this.Instructions[i+1].Label].Count == 1 &&
                    !this.BranchTargets.ContainsKey(this.Instructions[i].Label) &&
                    !cantRemove.Contains(this.Instructions[i].Label))
                {
                    this.DeleteInstructionAt(i);
                    this.BranchTargets.Remove(this.Instructions[i].Label);
                }
            }
        }

        /// <summary>
        /// Accumilates the references.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void AccumilateReferences(IlInstruction instruction)
        {
            switch (instruction.ArgumentType)
            {
                case OpArgumentType.Field:
                    this.memberReferences.Add((MemberReference)instruction.OpCodeArgument);
                    break;
                case OpArgumentType.Method:
                    this.memberReferences.Add((MemberReference)instruction.OpCodeArgument);
                    break;
                case OpArgumentType.ObjectType:
                    this.typeReferences.Add((TypeReference)instruction.OpCodeArgument);
                    break;
            }
        }

        /// <summary>
        /// Deletes the instruction at.
        /// </summary>
        /// <param name="index">The index.</param>
        private void DeleteInstructionAt(int index)
        {
            if (this.Instructions[index].Next != null)
            {
                this.Instructions[index].Next.Previous = this.Instructions[index].Previous;
            }

            if (this.Instructions[index].Previous != null)
            {
                this.Instructions[index].Previous.Next = this.Instructions[index].Next;
            }

            this.LabelInstructionMap.Remove(this.Instructions[index].Label);
            this.Instructions.RemoveAt(index);

            for (; index < this.Instructions.Count; ++index)
            {
                this.Instructions[index].Index--;
            }
        }

        /// <summary>
        /// Inserts the instruction at.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <param name="index">The index.</param>
        private void InsertInstructionAt(int index, IlInstruction instruction)
        {
            this.instructions.Insert(index, instruction);
            if (index > 0
                && instruction.Previous != this.instructions[index - 1])
            {
                instruction.Previous = this.instructions[index - 1];
                instruction.Previous.Next = instruction;
            }

            if (this.instructions.Count > index + 1)
            {
                instruction.Next = this.instructions[index + 1];
                this.instructions[index + 1].Previous = instruction;
            }

            this.LabelInstructionMap.Add(
                instruction.Label,
                instruction);

            index += 1;
            for (; index < this.Instructions.Count; ++index)
            {
                this.Instructions[index].Index++;
            }
        }

        /// <summary>
        /// Removes the temp variables.
        /// </summary>
        private void RemoveTempVariables()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                List<IlInstruction> branchTargets;
                if (this.Instructions[i].OpCode == OpCode.StoreLocal &&
                    this.Instructions[i + 1].OpCode == OpCode.LoadLocal &&
                    this.Instructions[i].OpCodeArgument.Equals(this.Instructions[i + 1].OpCodeArgument) &&
                    this.instructions[i].StackLevelAfter == 0)
                {
                    if (!this.IsVariableUsedWithoutChange(
                            (VariableDefinition)this.Instructions[i].OpCodeArgument,
                            i + 2,
                            new HashSet<int>()) &&
                        !this.BranchTargets.TryGetValue(
                            this.Instructions[i + 1].Label,
                            out branchTargets))
                    {
                        this.FixBranchSources(
                            this.Instructions[i].Label,
                            this.Instructions[i + 2].Label);

                        this.DeleteInstructionAt(i + 1);
                        this.DeleteInstructionAt(i--);
                    }
                }
                else if ((this.instructions[i].OpCode == OpCode.LoadConstantInt32
                        || this.instructions[i].OpCode == OpCode.LoadConstantInt32)
                    && this.Instructions[i + 1].OpCode == OpCode.StoreLocal
                    && this.instructions[i].StackLevelAfter == 1)
                {
                    // Here we delete any useless temp variable introduced by debug build.
                    // the case here is loadconst xxy, storelocal xx
                    if (!this.IsVariableUsedWithoutChange(
                            (VariableDefinition)this.Instructions[i + 1].OpCodeArgument,
                            i + 2,
                            new HashSet<int>()) &&
                        !this.BranchTargets.TryGetValue(
                            this.Instructions[i + 1].Label,
                            out branchTargets))
                    {
                        this.FixBranchSources(
                            this.Instructions[i].Label,
                            this.Instructions[i + 2].Label);

                        this.DeleteInstructionAt(i + 1);
                        this.DeleteInstructionAt(i--);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether given variable is used without ever chaning again.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="instructionsVisisted">The instructions visisted.</param>
        /// <returns>
        /// <c>true</c> if the specified variable index is variable used without change ; otherwise, <c>false</c>.
        /// </returns>
        private bool IsVariableUsedWithoutChange(
            VariableDefinition variable,
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

                switch (this.Instructions[iInstruction].OpCode)
                {
                    case OpCode.BranchEq:
                    case OpCode.BranchGe:
                    case OpCode.BranchUGe:
                    case OpCode.BranchGt:
                    case OpCode.BranchUGt:
                    case OpCode.BranchLe:
                    case OpCode.BranchULe:
                    case OpCode.BranchLt:
                    case OpCode.BranchULt:
                    case OpCode.BranchUNe:
                    case OpCode.BranchFalse:
                    case OpCode.BranchTrue:
                        if (this.IsVariableUsedWithoutChange(
                            variable,
                            this.LabelInstructionMap[(string)this.Instructions[iInstruction].OpCodeArgument].Index,
                            instructionsVisisted))
                        {
                            return true;
                        }
                        break;
                    case OpCode.Branch:
                        return this.IsVariableUsedWithoutChange(
                            variable,
                            this.LabelInstructionMap[(string)this.Instructions[iInstruction].OpCodeArgument].Index,
                            instructionsVisisted);
                    case OpCode.LoadLocal:
                    case OpCode.LoadLocalAddress:
                        if (this.Instructions[iInstruction].OpCodeArgument.Equals(variable))
                        {
                            return true;
                        }
                        break;
                    case OpCode.StoreLocal:
                        if (this.Instructions[iInstruction].OpCodeArgument.Equals(variable))
                        {
                            return false;
                        }
                        break;
                    case OpCode.Return:
                        return false;
                    default:
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Fixes the default value inits.
        /// Here we essentiall convert
        /// IL_00X0: ldloca xy
        /// IL_00X1: initobj XYZObj
        /// 
        /// to
        /// 
        /// IL_00X0: getDefaultValue XYZObj
        /// IL_00X1: stloc xy
        /// 
        /// By doing the above we make it simpler for us to work through the code flow.
        /// </summary>
        private void FixDefaultValueInits()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                OpCode reciprocatingOpCode = OpCode.LoadNull;

                switch (this.Instructions[i].OpCode)
                {
                    case OpCode.LoadLocalAddress:
                    case OpCode.LoadLocalAddress_s:
                        reciprocatingOpCode = OpCode.StoreLocal;
                        break;
                    case OpCode.LoadArgumentAddress:
                    case OpCode.LoadArgumentAddress_s:
                        reciprocatingOpCode = OpCode.StoreArgument;
                        break;
                    case OpCode.LoadStaticFieldAddress:
                        reciprocatingOpCode = OpCode.StoreStaticField;
                        break;
                    case OpCode.LoadFieldAddress:
                        reciprocatingOpCode = OpCode.StoreField;
                        break;
                }

                if (reciprocatingOpCode == OpCode.LoadNull)
                { continue; }

                if (this.Instructions[i + 1].OpCode != OpCode.Initobj)
                { continue; }

                IlInstruction nextInstruction = i < this.instructions.Count - 2
                    ? this.instructions[i + 2]
                    : null;

                IlInstruction defaultValueInstruction =
                    new IlInstruction(
                        this.Instructions[i].Previous,
                        OpCode.LoadDefaultValue,
                        this.Instructions[i].Label,
                        this.Instructions[i].Index,
                        this.Instructions[i + 1].OpCodeArgument,
                        this.Instructions[i].Location);

                IlInstruction setValueInstruction =
                    new IlInstruction(
                        defaultValueInstruction,
                        reciprocatingOpCode,
                        this.Instructions[i + 1].Label,
                        this.Instructions[i + 1].Index,
                        this.Instructions[i].OpCodeArgument,
                        this.Instructions[i].Location);

                defaultValueInstruction.StackLevelBefore = this.Instructions[i].StackLevelBefore;
                setValueInstruction.StackLevelBefore = this.Instructions[i + 1].StackLevelBefore;
                this.Instructions[i] = defaultValueInstruction;
                this.instructions[i + 1] = setValueInstruction;
                this.LabelInstructionMap[defaultValueInstruction.Label] = defaultValueInstruction;
                this.LabelInstructionMap[setValueInstruction.Label] = setValueInstruction;

                if (nextInstruction != null)
                {
                    nextInstruction.Previous = setValueInstruction;
                    setValueInstruction.Next = nextInstruction;
                }
            }
        }

        /// <summary>
        /// Fixes the load struct address for virtual call.
        /// 
        /// Here we convert LoadAddress instruction which is followed by constraint
        /// instruction to simple load instruction.
        /// 
        /// We also fix call to instance functions of struct types. In those cases again
        /// we have loadAddress instruction followed by call to instance method.
        /// </summary>
        private void FixLoadStructAddressForVirtualCall()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                OpCode replacementOpCode = OpCode.LoadNull;

                switch (this.Instructions[i].OpCode)
                {
                    case OpCode.LoadLocalAddress:
                    case OpCode.LoadLocalAddress_s:
                        replacementOpCode = OpCode.LoadLocal;
                        break;
                    case OpCode.LoadArgumentAddress:
                    case OpCode.LoadArgumentAddress_s:
                        replacementOpCode = OpCode.LoadArgument;
                        break;
                    case OpCode.LoadStaticFieldAddress:
                        replacementOpCode = OpCode.LoadStaticField;
                        break;
                    case OpCode.LoadFieldAddress:
                        replacementOpCode = OpCode.LoadField;
                        break;
                }

                if (replacementOpCode == OpCode.LoadNull)
                { continue; }

                if (this.Instructions[i + 1].OpCode != OpCode.Constrained)
                {
                    if (this.Instructions[i + 1].OpCode != OpCode.Call
                        || !((MethodReference)this.Instructions[i + 1].OpCodeArgument).DeclaringType.Resolve().IsValueType)
                    { continue; }
                }

                IlInstruction nextInstruction = this.instructions[i + 1];

                IlInstruction loadInstruction =
                    new IlInstruction(
                        this.Instructions[i].Previous,
                        replacementOpCode,
                        this.Instructions[i].Label,
                        this.Instructions[i].Index,
                        this.Instructions[i].OpCodeArgument,
                        this.Instructions[i].Location);

                loadInstruction.StackLevelBefore = this.Instructions[i].StackLevelBefore;
                this.Instructions[i] = loadInstruction;
                this.LabelInstructionMap[loadInstruction.Label] = loadInstruction;

                if (nextInstruction != null)
                {
                    nextInstruction.Previous = loadInstruction;
                    loadInstruction.Next = nextInstruction;
                }
            }
        }

        /// <summary>
        /// Fixes the struct constructor calls.
        /// </summary>
        private void FixStructConstructorCalls()
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                if (this.instructions[i].OpCode != OpCode.Call)
                { continue; }

                MethodReference methodReference = (MethodReference)this.instructions[i].OpCodeArgument;
                MethodDefinition methodDefinition = methodReference.Resolve();
                if (methodDefinition.Name != ".ctor" || !methodDefinition.DeclaringType.IsValueType)
                { continue; }

                OpCode? setOpCode = null;
                int j;
                // Here we will be changing constructor of struct types from taking address of target
                // as argument to using newObj and store instruction.
                for (j = i - 1; j >= 0; j--)
                {
                    if (this.instructions[j].StackLevelBefore != this.instructions[i].StackLevelAfter)
                    { continue; }

                    setOpCode = MethodExecutionBlock.GetReciprocatingSetOpCode(this.instructions[j].OpCode);
                    break;
                }

                if (setOpCode == null)
                { continue; }

                IlInstruction methodCallInstruction = this.instructions[i];
                IlInstruction loadAddressInstruction = this.instructions[j];

                // First let's remove loadAddressOpCode.
                this.DeleteInstructionAt(i);
                this.DeleteInstructionAt(j);

                this.InsertInstructionAt(
                    i - 1,
                    new IlInstruction(
                        this.instructions[i-2],
                        OpCode.Newobj,
                        methodCallInstruction.Label + "_newObj",
                        i-1,
                        methodCallInstruction.OpCodeArgument,
                        methodCallInstruction.Location));

                this.InsertInstructionAt(
                    i,
                    new IlInstruction(
                        this.instructions[i - 1],
                        setOpCode.Value,
                        loadAddressInstruction.Label + "_setInstruction",
                        i,
                        loadAddressInstruction.OpCodeArgument,
                        loadAddressInstruction.Location));

                for (int k = j; k < i - 1; k++)
                {
                    this.instructions[k].ResetStackBefore(
                        this.instructions[k].StackLevelBefore - 1);
                }

                this.instructions[i - 1].StackLevelBefore = this.instructions[i - 2].StackLevelAfter;
                this.instructions[i].StackLevelBefore = this.instructions[i - 1].StackLevelAfter;
            }
        }

        /// <summary>
        /// Removes the no ops.
        /// </summary>
        /// <param name="cantRemove">The cant remove.</param>
        private void RemoveNoOps(HashSet<string> cantRemove)
        {
            for (int i = 0; i < this.Instructions.Count - 1; i++)
            {
                if (this.Instructions[i].OpCode == OpCode.Nop &&
                    !cantRemove.Contains(this.Instructions[i].Label))
                {
                    this.FixBranchSources(
                        this.Instructions[i].Label,
                        this.Instructions[i + 1].Label);

                    this.DeleteInstructionAt(i);
                }
            }
        }

        /// <summary>
        /// Fixes the branch sources.
        /// </summary>
        /// <param name="branchSources">The branch sources.</param>
        /// <param name="oldTarget">The old target.</param>
        /// <param name="newTarget">The new target.</param>
        private void FixBranchSources(
            string oldTarget,
            string newTarget)
        {
            List<IlInstruction> branchSources;

            if (this.BranchTargets.TryGetValue(
                oldTarget,
                out branchSources))
            {
                foreach (var branch in branchSources)
                {
                    if (branch.FlowType == FlowType.Switch)
                    {
                        string[] arguments = (string[])branch.OpCodeArgument;
                        for (int iArg = 0; iArg < arguments.Length; iArg++)
                        {
                            if (arguments[iArg] == oldTarget)
                            {
                                arguments[iArg] = newTarget;
                            }
                        }
                    }
                    else
                    {
                        branch.SetArgument(newTarget);
                    }
                }

                this.BranchTargets.Remove(oldTarget);

                List<IlInstruction> newBranchSources;

                if (!this.BranchTargets.TryGetValue(
                    newTarget,
                    out newBranchSources))
                {
                    this.BranchTargets.Add(newTarget, branchSources);
                }
                else
                {
                    newBranchSources.AddRange(branchSources);
                }
            }
        }

        /// <summary>
        /// Gets the reciprocating set op code.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <returns>Null if not a address load opCode</returns>
        private static OpCode? GetReciprocatingSetOpCode(OpCode opCode)
        {
            switch (opCode)
            {
                case OpCode.LoadLocalAddress:
                case OpCode.LoadLocalAddress_s:
                    return OpCode.StoreLocal;
                case OpCode.LoadArgumentAddress:
                case OpCode.LoadArgumentAddress_s:
                    return OpCode.StoreArgument;
                case OpCode.LoadStaticFieldAddress:
                    return OpCode.StoreStaticField;
                case OpCode.LoadFieldAddress:
                    return OpCode.StoreField;
            }

            return null;
        }
    }
}
