namespace Cs2JsC.CLR.Decompiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using Cs2JsC.Utils;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    /// <summary>
    /// IlInstruction class.
    /// </summary>
    public class IlInstruction
    {
        /// <summary>
        /// Backing field for OpCode.
        /// </summary>
        private readonly OpCode opCode;

        /// <summary>
        /// Backing field for number of pushes.
        /// </summary>
        private readonly StackPushCode pushes;

        /// <summary>
        /// Backing field for number of pops.
        /// </summary>
        private readonly StackPopCode pops;

        /// <summary>
        /// Backing field for argumentType.
        /// </summary>
        private readonly OpArgumentType argumentType;

        /// <summary>
        /// Backing field for flowType.
        /// </summary>
        private readonly FlowType flowType;

        /// <summary>
        /// Backing field for label.
        /// </summary>
        private readonly string label;

        /// <summary>
        /// Backing field for fileLocation.
        /// </summary>
        private readonly Location fileLocation;

        /// <summary>
        /// Backing field for OpCode argument.
        /// </summary>
        private object opCodeArgument;

        /// <summary>
        /// Backing field for nextInstruction.
        /// </summary>
        private IlInstruction nextInstruction;

        /// <summary>
        /// Backing field for previousInstruction.
        /// </summary>
        private IlInstruction previousInstruction;

        /// <summary>
        /// Backing field for index.
        /// </summary>
        private int index;

        /// <summary>
        /// Backing field for stackBefore.
        /// </summary>
        private int stackBefore = -1;

        /// <summary>
        /// Backing field for stackDelta.
        /// </summary>
        private int stackDelta = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="IlInstruction"/> class.
        /// </summary>
        /// <param name="previousInstruction">The previous instruction.</param>
        /// <param name="opCode">The op code.</param>
        /// <param name="label">The label.</param>
        /// <param name="index">The index.</param>
        /// <param name="opCodeArgument">The op code argument.</param>
        /// <param name="location">The location.</param>
        public IlInstruction(
            IlInstruction previousInstruction,
            OpCode opCode,
            string label,
            int index,
            object opCodeArgument,
            Location location)
        {
            this.opCode = opCode;
            this.label = label;
            this.index = index;
            this.opCodeArgument = opCodeArgument;
            this.fileLocation = location;
            this.previousInstruction = previousInstruction;

            if (this.previousInstruction != null)
            {
                this.previousInstruction.nextInstruction = this;
            }

            this.pushes = OpCodeHelper.GetPushes(this.opCode);
            this.pops = OpCodeHelper.GetPops(this.opCode);
            this.argumentType = OpCodeHelper.GetArgumentType(this.opCode);
            this.flowType = OpCodeHelper.GetFlowType(this.opCode);
        }

        /// <summary>
        /// Gets the op code.
        /// </summary>
        /// <value>The op code.</value>
        public OpCode OpCode
        {
            get
            {
                return this.opCode;
            }
        }

        /// <summary>
        /// Gets or sets the next.
        /// </summary>
        /// <value>The next.</value>
        public IlInstruction Next
        {
            get
            {
                return this.nextInstruction;
            }

            internal set
            {
                this.nextInstruction = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous.
        /// </summary>
        /// <value>The previous.</value>
        public IlInstruction Previous
        {
            get
            {
                return this.previousInstruction;
            }

            internal set
            {
                this.previousInstruction = value;
            }
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label
        {
            get
            {
                return this.label;
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get
            {
                return this.index;
            }

            internal set
            {
                this.index = value;
            }
        }

        /// <summary>
        /// Gets the op code argument.
        /// </summary>
        /// <value>The op code argument.</value>
        public object OpCodeArgument
        {
            get
            {
                return this.opCodeArgument;
            }
        }

        /// <summary>
        /// Gets or sets the stack before.
        /// </summary>
        /// <value>The stack before.</value>
        public int StackLevelBefore
        {
            get { return this.stackBefore; }
            set
            {
                if (value == this.stackBefore)
                {
                    return;
                }

                if (this.stackBefore >= 0)
                {
                    throw new InvalidOperationException("StackBefore already set");
                }

                if (value < 0)
                {
                    throw new ArgumentException("stackBefore can't be less than 0");
                }

                this.stackBefore = value;
                this.CalculateStackDelta();
            }
        }

        /// <summary>
        /// Gets the stack after.
        /// </summary>
        /// <value>The stack after.</value>
        public int StackLevelAfter
        {
            get
            {
                return this.stackDelta == int.MinValue ?
                    0 :
                    this.stackDelta + this.stackBefore;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is temp variable store.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is temp variable store; otherwise, <c>false</c>.
        /// </value>
        public bool IsTempVariableStore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of the flow.
        /// </summary>
        /// <value>The type of the flow.</value>
        public FlowType FlowType
        {
            get
            {
                return this.flowType;
            }
        }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        /// <value>The type of the artument.</value>
        public OpArgumentType ArgumentType
        {
            get
            {
                return this.argumentType;
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location Location
        {
            get
            {
                return this.fileLocation;
            }
        }

        /// <summary>
        /// Simplifies the op code and argument.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <param name="argument">The argument.</param>
        public static void SimplifyOpCodeAndArgument(
            MethodDefinition methodDefinition,
            ref OpCode opCode,
            ref object argument)
        {
            switch (opCode)
            {
                case OpCode.LoadArgument_0:
                    opCode = OpCode.LoadArgument;
                    argument = IlInstruction.GetParameter(
                            methodDefinition,
                            0);
                    break;
                case OpCode.LoadArgument_1:
                    opCode = OpCode.LoadArgument;
                    argument = IlInstruction.GetParameter(
                            methodDefinition,
                            1);
                    break;
                case OpCode.LoadArgument_2:
                    opCode = OpCode.LoadArgument;
                    argument = IlInstruction.GetParameter(
                            methodDefinition,
                            2);
                    break;
                case OpCode.LoadArgument_3:
                    opCode = OpCode.LoadArgument;
                    argument = IlInstruction.GetParameter(
                            methodDefinition,
                            3);
                    break;
                case OpCode.LoadLocal_s:
                    opCode = OpCode.LoadLocal;
                    break;
                case OpCode.LoadLocal_0:
                    opCode = OpCode.LoadLocal;
                    argument = methodDefinition.Body.Variables[0];
                    break;
                case OpCode.LoadLocal_1:
                    opCode = OpCode.LoadLocal;
                    argument = methodDefinition.Body.Variables[1];
                    break;
                case OpCode.LoadLocal_2:
                    opCode = OpCode.LoadLocal;
                    argument = methodDefinition.Body.Variables[2];
                    break;
                case OpCode.LoadLocal_3:
                    opCode = OpCode.LoadLocal;
                    argument = methodDefinition.Body.Variables[3];
                    break;
                case OpCode.StoreLocal_0:
                    opCode = OpCode.StoreLocal;
                    argument = methodDefinition.Body.Variables[0];
                    break;
                case OpCode.StoreLocal_1:
                    opCode = OpCode.StoreLocal;
                    argument = methodDefinition.Body.Variables[1];
                    break;
                case OpCode.StoreLocal_2:
                    opCode = OpCode.StoreLocal;
                    argument = methodDefinition.Body.Variables[2];
                    break;
                case OpCode.StoreLocal_3:
                    opCode = OpCode.StoreLocal;
                    argument = methodDefinition.Body.Variables[3];
                    break;
                case OpCode.StoreLocal_s:
                    opCode = OpCode.StoreLocal;
                    break;
                case OpCode.LoadArgument_s:
                    opCode = OpCode.LoadArgument;
                    break;
                case OpCode.LoadArgumentAddress_s:
                    opCode = OpCode.LoadArgumentAddress;
                    break;
                case OpCode.StoreArgument_s:
                    opCode = OpCode.StoreArgument;
                    break;
                case OpCode.LoadConstantInt32_m1:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)-1;
                    break;
                case OpCode.LoadConstantInt32_0:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)0;
                    break;
                case OpCode.LoadConstantInt32_1:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)1;
                    break;
                case OpCode.LoadConstantInt32_2:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)2;
                    break;
                case OpCode.LoadConstantInt32_3:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)3;
                    break;
                case OpCode.LoadConstantInt32_4:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)4;
                    break;
                case OpCode.LoadConstantInt32_5:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)5;
                    break;
                case OpCode.LoadConstantInt32_6:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)6;
                    break;
                case OpCode.LoadConstantInt32_7:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)7;
                    break;
                case OpCode.LoadConstantInt32_8:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)8;
                    break;
                case OpCode.LoadConstantInt32_s:
                    opCode = OpCode.LoadConstantInt32;
                    argument = (int)(sbyte)argument;
                    break;
                case OpCode.Branch:
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
                case OpCode.Leave:
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchEq_s:
                    opCode = OpCode.BranchEq;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchGe_s:
                    opCode = OpCode.BranchGe;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchUGe_s:
                    opCode = OpCode.BranchUGe;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchGt_s:
                    opCode = OpCode.BranchGt;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchUGt_s:
                    opCode = OpCode.BranchUGt;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchLe_s:
                    opCode = OpCode.BranchLe;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchULe_s:
                    opCode = OpCode.BranchULe;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchLt_s:
                    opCode = OpCode.BranchLt;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchULt_s:
                    opCode = OpCode.BranchULt;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchUNe_s:
                    opCode = OpCode.BranchUNe;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchFalse_s:
                    opCode = OpCode.BranchFalse;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.BranchTrue_s:
                    opCode = OpCode.BranchTrue;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.Branch_s:
                    opCode = OpCode.Branch;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.Leave_s:
                    opCode = OpCode.Leave;
                    argument = ((Instruction)argument).Offset.ToString();
                    break;
                case OpCode.Switch:
                    {
                        IList<Instruction> offsets = (IList<Instruction>)argument;
                        string[] strOffsets = new string[offsets.Count];

                        for (int offsetIndex = 0; offsetIndex < offsets.Count; offsetIndex++)
                        {
                            strOffsets[offsetIndex] = offsets[offsetIndex].Offset.ToString();
                        }

                        argument = strOffsets;
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}:[{1}:{2}] {3}: {4}",
                this.Label,
                this.StackLevelBefore,
                this.StackLevelAfter,
                this.OpCode,
                this.OpCodeArgument);
        }

        /// <summary>
        /// Resets the stack before.
        /// </summary>
        /// <param name="stackLevel">The stack level.</param>
        internal void ResetStackBefore(int stackLevel)
        {
            if (stackLevel < 0)
            {
                throw new ArgumentException("stackLevel");
            }
            this.stackBefore = stackLevel;
            this.CalculateStackDelta();
        }

        /// <summary>
        /// Gets the pushes.
        /// </summary>
        /// <returns></returns>
        public int GetPushes()
        {
            MethodReference methodReference;
            MethodDefinition methodDefinition;

            switch (this.pushes)
            {
                case StackPushCode.Push0:
                    return 0;
                case StackPushCode.Push1:
                    return 1;
                case StackPushCode.Push2:
                    return 2;
                case StackPushCode.PushVar:
                    methodReference = (MethodReference)this.OpCodeArgument;
                    methodDefinition = methodReference.Resolve();

                    // The returnValue only matters if current operation is not a new Object
                    // operation.
                    //
                    return (methodDefinition.ReturnType == null || methodDefinition.ReturnType.MetadataType == MetadataType.Void)
                        && (this.opCode != OpCode.Newobj) ? 0 : 1;
            }

            return 0;
        }

        /// <summary>
        /// Gets the pops.
        /// </summary>
        /// <returns></returns>
        public int GetPops()
        {
            MethodReference methodReference;
            MethodDefinition methodDefinition;

            switch (this.pops)
            {
                case StackPopCode.Pop0:
                    return 0;
                case StackPopCode.Pop1:
                    return 1;
                case StackPopCode.Pop2:
                    return 2;
                case StackPopCode.Pop3:
                    return 3;
                case StackPopCode.PopVar:
                    methodReference = (MethodReference)this.opCodeArgument;
                    methodDefinition = methodReference.Resolve();

                    // Incase we are constructing new object, here we will create new object and pass it
                    // to constructor, so we need to take that fact into account as well
                    //
                    return methodDefinition.Parameters.Count +
                        (methodDefinition.HasThis ? 1 : 0) - (this.OpCode == OpCode.Newobj ? 1 : 0);
                case StackPopCode.PopAll:
                    return this.stackBefore >= 0 ? this.stackBefore : int.MinValue;
            }

            return 0;
        }

        /// <summary>
        /// Sets the argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public void SetArgument(object argument)
        {
            this.opCodeArgument = argument;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <param name="argIndex">Index of the arg.</param>
        /// <returns>Parameter Definition at given argumentIndex (from IL).</returns>
        private static ParameterDefinition GetParameter(
            MethodDefinition methodDefinition,
            int argIndex)
        {
            if (methodDefinition.HasThis)
            {
                if (argIndex == 0)
                {
                    return methodDefinition.Body.ThisParameter;
                }

                argIndex--;
            }

            return methodDefinition.Parameters[argIndex];
        }

        /// <summary>
        /// Calculates the stack delta.
        /// </summary>
        private void CalculateStackDelta()
        {
            this.stackDelta = 0 - this.GetPops();

            if (this.stackDelta > int.MinValue)
            {
                this.stackDelta += this.GetPushes();
            }
        }
    }
}
