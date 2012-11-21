using System;
using System.Collections.Generic;
using NScript.Lib.MetaData;
using NScript.Lib.ILParser;

namespace NScript.Lib.AsmDeasm.Ops
{
    public class Instruction
    {
        #region member variables
        private int stackBefore = -1;
        private int stackDelta = 0;
        #endregion

        #region constructors/Factories
        public Instruction(
            Instruction previousInstruction,
            OpCodeWrapper opCode,
            string label,
            int index,
            object opCodeArgument,
            SourceCodeBlock codeBlock)
        {
            this.CodeOp = opCode;
            this.Label = label;
            this.Index = index;
            this.Previous = previousInstruction;
            this.OpCodeArgument = opCodeArgument;
            this.SourceCode = codeBlock;
            this.CalculateStackDelta();

            if (this.Previous != null)
            {
                this.Previous.Next = this;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public OpCodeWrapper CodeOp
        { get; private set; }

        public Instruction Next
        { get; internal set; }

        public Instruction Previous
        { get; internal set; }

        public string Label
        { get; private set; }

        public int Index
        { get; internal set; }

        public object OpCodeArgument
        { get; private set; }

        public SourceCodeBlock SourceCode
        { get; private set; }

        public int StackBefore
        {
            get { return this.stackBefore; }
            set
            {
                if (value == this.stackBefore)
                {
                    return;
                }

                if (stackBefore >= 0)
                {
                    throw new InvalidOperationException("StackBefore already set");
                }

                if (value < 0)
                {
                    throw new ArgumentException("stackBefore can't be less than 0");
                }

                this.stackBefore = value;
            }
        }

        public int StackAfter
        {
            get
            {
                return this.stackDelta == int.MinValue ?
                    0 :
                    this.stackDelta + this.stackBefore;
            }
        }

        public bool IsTempVariableStore
        {
            get;
            set;
        }
        #endregion

        #region public functions
        public override string ToString()
        {
            return string.Format(
                "{0}:[{1}:{2}] {3}: {4}",
                this.Label,
                this.StackBefore,
                this.StackAfter,
                this.CodeOp,
                this.OpCodeArgument);
        }

        public static object InitializeOpCodeArgument(
            IlOpCode code,
            string opCodeArgumentString,
            bool isStaticFunction,
            IList<ArgumentSignature> argumentList,
            IList<ArgumentSignature> variableList)
        {
            switch (code)
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
                case IlOpCode.Branch:
                case IlOpCode.BranchIfFalse:
                case IlOpCode.BranchIfTrue:
                    return opCodeArgumentString;
                case IlOpCode.NewObject:
                case IlOpCode.Call:
                case IlOpCode.Calli:
                case IlOpCode.Callvirt:
                case IlOpCode.LoadMethodPointer:
                    return ParseUtils.ParseMethodSignature((StringFragment)opCodeArgumentString);
                case IlOpCode.LoadArgument:
                case IlOpCode.LoadArgumentAddress:
                case IlOpCode.StoreArgument:
                    return Instruction.GetArgumentId(
                        opCodeArgumentString,
                        argumentList,
                        isStaticFunction);
                case IlOpCode.LoadConstantLong:
                    return long.Parse(opCodeArgumentString);
                case IlOpCode.LoadConstantFloat:
                    return float.Parse(opCodeArgumentString);
                case IlOpCode.LoadConstantDouble:
                    return double.Parse(opCodeArgumentString);
                case IlOpCode.LoadConstantInt:
                    int intRetVal;
                    return int.TryParse(opCodeArgumentString, out intRetVal)
                               ? intRetVal
                               : int.Parse(opCodeArgumentString.Substring(2),
                                           System.Globalization.NumberStyles.HexNumber);
                case IlOpCode.LoadLocal:
                case IlOpCode.LoadLocalAddress:
                case IlOpCode.StoreLocal:
                    return Instruction.GetLocalId(
                        opCodeArgumentString,
                        variableList);
                case IlOpCode.StoreField:
                case IlOpCode.LoadField:
                case IlOpCode.LoadFieldAddress:
                    return ParseUtils.ParseFieldSignature(opCodeArgumentString, false);
                case IlOpCode.StoreStaticField:
                case IlOpCode.LoadStaticField:
                case IlOpCode.LoadStaticFieldAddress:
                    return ParseUtils.ParseFieldSignature(opCodeArgumentString, true);
                case IlOpCode.LoadString:
                    return opCodeArgumentString;
                case IlOpCode.Switch:
                    opCodeArgumentString = opCodeArgumentString.Trim(' ', '(', ')', '\t');
                    var returnValue = opCodeArgumentString.Split(',');
                    for (int i = 0; i < returnValue.Length; i++)
                    {
                        returnValue[i] = returnValue[i].Trim();
                    }
                    return returnValue;
#if false
                case IlOpCode.Add:
               case IlOpCode.AddOvfSigned:
                case IlOpCode.AddOvfUnsigned:
                case IlOpCode.And:
                case IlOpCode.Arglist:
                case IlOpCode.Break:
                case IlOpCode.CheckEquals:
                case IlOpCode.CheckGreater:
                case IlOpCode.CheckGreaterUnsigned:
                case IlOpCode.Ckfinite:
                case IlOpCode.CheckLesser:
                case IlOpCode.CheckLesserUnsigned:
                case IlOpCode.ConvToByte:
                case IlOpCode.ConvToShort:
                case IlOpCode.ConvToInt:
                case IlOpCode.ConvToLong:
                case IlOpCode.ConvToFloat:
                case IlOpCode.ConvToDouble:
                case IlOpCode.ConvToUnsignedByte:
                case IlOpCode.ConvToUnsignedShort:
                case IlOpCode.ConvToUnsignedInt:
                case IlOpCode.ConvToUnsignedLong:
                case IlOpCode.ConvOvfToByte:
                case IlOpCode.ConvOvfToShort:
                case IlOpCode.ConvOvfToInt:
                case IlOpCode.ConvOvfToLong:
                case IlOpCode.ConvOvfToUnsignedByte:
                case IlOpCode.ConvOvfToUnsignedShort:
                case IlOpCode.ConvOvfToUnsignedInt:
                case IlOpCode.ConvOvfToUnsignedLong:
                case IlOpCode.CopyBulk:
                case IlOpCode.Div:
                case IlOpCode.DivUnsigned:
                case IlOpCode.Dup:
                case IlOpCode.Endfilter:
                case IlOpCode.Endfinally:
                case IlOpCode.InitBulk:
                case IlOpCode.Jmp:
                case IlOpCode.LoadArgument0:
                case IlOpCode.LoadArgument1:
                case IlOpCode.LoadArgument2:
                case IlOpCode.LoadArgument3:
                case IlOpCode.LoadConstantInt0:
                case IlOpCode.LoadConstantInt1:
                case IlOpCode.LoadConstantInt2:
                case IlOpCode.LoadConstantInt3:
                case IlOpCode.LoadConstantInt4:
                case IlOpCode.LoadConstantInt5:
                case IlOpCode.LoadConstantInt6:
                case IlOpCode.LoadConstantInt7:
                case IlOpCode.LoadConstantInt8:
                case IlOpCode.LoadConstantIntNeg1:
                case IlOpCode.LoadIndirect:
                case IlOpCode.LoadArrayLength:
                case IlOpCode.LoadObject:
                case IlOpCode.LoadLocal0:
                case IlOpCode.LoadLocal1:
                case IlOpCode.LoadLocal2:
                case IlOpCode.LoadLocal3:
                case IlOpCode.LoadLocalAddress0:
                case IlOpCode.LoadLocalAddress1:
                case IlOpCode.LoadLocalAddress2:
                case IlOpCode.LoadLocalAddress3:
                case IlOpCode.LoadNull:
                case IlOpCode.Leave: // UnSuported
                case IlOpCode.Localalloc:
                case IlOpCode.Mul:
                case IlOpCode.MulOvf:
                case IlOpCode.Neg:
                case IlOpCode.Nop:
                case IlOpCode.Not:
                case IlOpCode.Or:
                case IlOpCode.Pop:
                case IlOpCode.Remainder:
                case IlOpCode.RemainderUnsigned:
                case IlOpCode.Return:
                case IlOpCode.ShiftLeft:
                case IlOpCode.ShiftRight:
                case IlOpCode.ShiftRightUnsigned:
                case IlOpCode.StoreIndirect:
                case IlOpCode.StoreLocal0:
                case IlOpCode.StoreLocal1:
                case IlOpCode.StoreLocal2:
                case IlOpCode.StoreLocal3:
                case IlOpCode.Sub:
                case IlOpCode.SubOvf:
                case IlOpCode.Xor:
                case IlOpCode.Box:
                case IlOpCode.Castclass:
                case IlOpCode.Cpobj:
                case IlOpCode.Initobj:
                case IlOpCode.Isinst:
                case IlOpCode.LoadArrayElement:
                case IlOpCode.LoadArrayElementByte:
                case IlOpCode.LoadArrayElementShort:
                case IlOpCode.LoadArrayElementInt:
                case IlOpCode.LoadArrayElementLong:
                case IlOpCode.LoadArrayElementUnsignedByte:
                case IlOpCode.LoadArrayElementUnsignedShort:
                case IlOpCode.LoadArrayElementUnsignedInt:
                case IlOpCode.LoadArrayElementUnsignedLong:
                case IlOpCode.LoadArrayElementFloat:
                case IlOpCode.LoadArrayElementDouble:
                case IlOpCode.LoadArrayElementObject:
                case IlOpCode.LoadArrayElementAddress:
                case IlOpCode.LoadToken:
                case IlOpCode.LoadVirtualFunction:
                case IlOpCode.MakeReferenceAny:
                case IlOpCode.NewArray:
                case IlOpCode.Refanytype:
                case IlOpCode.Refanyval:
                case IlOpCode.Rethrow:
                case IlOpCode.Sizeof:
                case IlOpCode.StoreArrayElement:
                case IlOpCode.StoreByteArrayElement:
                case IlOpCode.StoreShortArrayElement:
                case IlOpCode.StoreIntArrayElement:
                case IlOpCode.StoreLongArrayElement:
                case IlOpCode.StoreFloatArrayElement:
                case IlOpCode.StoreDoubleArrayElement:
                case IlOpCode.StoreObjectArrayElement:
                case IlOpCode.StoreObject:
                case IlOpCode.Throw:
                case IlOpCode.Unbox:
                case IlOpCode.UnboxAny:
  
    #endif
                default:
                    return opCodeArgumentString;
            }
        }

        public int GetPushes()
        {
            MethodSignature tmpSig;

            switch (this.CodeOp.PushCode)
            {
                case StackPushCode.Push0:
                    return 0;
                case StackPushCode.Push1:
                    return 1;
                case StackPushCode.Push2:
                    return 2;
                case StackPushCode.PushVar:
                    tmpSig = (MethodSignature)this.OpCodeArgument;
                    // The returnValue only matters if current operation is not a new Object
                    // operation.
                    //
                    return tmpSig.Type == null && (this.CodeOp.Code != IlOpCode.NewObject) ? 0 : 1;
            }

            return 0;
        }

        public int GetPops()
        {
            MethodSignature tmpSig;

            switch (this.CodeOp.PopCode)
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
                    tmpSig = (MethodSignature)this.OpCodeArgument;
                    // Incase we are constructing new object, here we will create new object and pass it
                    // to constructor, so we need to take that fact into account as well
                    //
                    return tmpSig.Arguments.Count +
                        (tmpSig.IsStatic? 0 : 1) - (this.CodeOp.Code == IlOpCode.NewObject ? 1 : 0);
                case StackPopCode.PopAll:
                    return this.stackBefore >= 0 ? this.stackBefore : int.MinValue;
            }

            return 0;
        }

        public void SetArgument(object arg)
        {
            this.OpCodeArgument = arg;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static object GetArgumentId(
            string opCodeArgumentString,
            IList<ArgumentSignature> arguments,
            bool isStaticFunction)
        {
            int argumentIndex;

            if (int.TryParse(opCodeArgumentString, out argumentIndex))
            {
                return argumentIndex;
            }

            for (int i = 0; i < arguments.Count; i++)
            {
                if (opCodeArgumentString == arguments[i].Name)
                {
                    return isStaticFunction ? i : i + 1;
                }
            }

            throw new InvalidOperationException("Can't resolve argumentId");
        }

        private static object GetLocalId(
            string opCodeArgumentString,
            IList<ArgumentSignature> variables)
        {
            int argumentIndex;

            if (int.TryParse(opCodeArgumentString, out argumentIndex))
            {
                return argumentIndex;
            }

            for (int i = 0; i < variables.Count; i++)
            {
                if (opCodeArgumentString == variables[i].Name)
                {
                    return i;
                }
            }

            throw new InvalidOperationException("Can't resolve argumentId");
        }

        private void CalculateStackDelta()
        {
            this.stackDelta = 0 - this.GetPops();

            if (this.stackDelta > int.MinValue)
            {
                this.stackDelta += this.GetPushes();
            }
        }
        #endregion
    }
}
