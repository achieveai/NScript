using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.Ops
{
    public static class OpCodes
    {
        public static readonly OpCodeWrapper Add =
            new OpCodeWrapper(
                @"add",
                IlOpCode.Add,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper AddOverflow =
            new OpCodeWrapper(
                @"add\.ovf",
                IlOpCode.AddOvfSigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper AddOverflowUnsigned =
            new OpCodeWrapper(
                @"add\.ovf\.un",
                IlOpCode.AddOvfUnsigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper And =
            new OpCodeWrapper(
                @"and",
                IlOpCode.And,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper ArgumentList =
            new OpCodeWrapper(
                @"arglist",
                IlOpCode.Arglist,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper BranchIfEqual =
            new OpCodeWrapper(
                @"beq(\.s)?",
                IlOpCode.BranchIfEqual,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfGreaterOrEqual =
            new OpCodeWrapper(
                @"bge(\.s)?",
                IlOpCode.BranchIfGreaterOrEqual,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfGreaterOrEqualUnsigned =
            new OpCodeWrapper(
                @"bge\.un(\.s)?",
                IlOpCode.BranchIfGreaterOrEqualUnsigned,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfGreater =
            new OpCodeWrapper(
                @"bgt(\.s)?",
                IlOpCode.BranchIfGreater,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfGreaterUnsigned =
            new OpCodeWrapper(
                @"bgt\.un(\.s)?",
                IlOpCode.BranchIfGreaterUnsigned,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfLesserOrEqual =
            new OpCodeWrapper(
                @"ble(\.s)?",
                IlOpCode.BranchIfLessOrEqual,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfLesserOrEqualUnsigned =
            new OpCodeWrapper(
                @"ble\.un(\.s)?",
                IlOpCode.BranchIfLessOrEqualUnsigned,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfLesser =
            new OpCodeWrapper(
                @"blt(\.s)?",
                IlOpCode.BranchIfLessThan,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfLesserUnsigned =
            new OpCodeWrapper(
                @"blt\.un(\.s)?",
                IlOpCode.BranchIfLessThanUnsigned,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper BranchIfNotEqual =
            new OpCodeWrapper(
                @"bne\.un(\.s)?",
                IlOpCode.BranchIfNotEqualsUnsigned,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper Branch =
            new OpCodeWrapper(
                @"br(\.s)?",
                IlOpCode.Branch,
                FlowType.Branch,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper Break =
            new OpCodeWrapper(
                @"break",
                IlOpCode.Break,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper BranchIfFalse =
            new OpCodeWrapper(
                @"brfalse(\.s)?",
                IlOpCode.BranchIfFalse,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper BranchIfTrue =
            new OpCodeWrapper(
                @"brtrue(\.s)?",
                IlOpCode.BranchIfTrue,
                FlowType.ConditionalBranch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Call =
            new OpCodeWrapper(
                @"call",
                IlOpCode.Call,
                FlowType.MethodCall,
                OpArgumentType.Method,
                StackPushCode.PushVar,
                StackPopCode.PopVar);

        public static readonly OpCodeWrapper CallIndirect =
            new OpCodeWrapper(
                @"calli",
                IlOpCode.Calli,
                FlowType.MethodCall,
                OpArgumentType.Method,
                StackPushCode.PushVar,
                StackPopCode.PopVar);

        public static readonly OpCodeWrapper CheckEquals =
            new OpCodeWrapper(
                @"ceq",
                IlOpCode.CheckEquals,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper CheckGreater =
            new OpCodeWrapper(
                @"cgt",
                IlOpCode.CheckGreater,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper CheckGreaterUnsigned =
            new OpCodeWrapper(
                @"cgt\.un",
                IlOpCode.CheckGreaterUnsigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper CheckFinite =
            new OpCodeWrapper(
                @"ckfinite",
                IlOpCode.Ckfinite,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper CheckLesser =
            new OpCodeWrapper(
                @"clt",
                IlOpCode.CheckLesser,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper CheckLesserUnsigned =
            new OpCodeWrapper(
                @"clt\.un",
                IlOpCode.CheckLesserUnsigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper ConvertToByte =
            new OpCodeWrapper(
                @"conv\.i1",
                IlOpCode.ConvToByte,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToShort =
            new OpCodeWrapper(
                @"conv\.i2",
                IlOpCode.ConvToShort,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToInt =
            new OpCodeWrapper(
                @"conv\.i(4)?",
                IlOpCode.ConvToInt,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToLong =
            new OpCodeWrapper(
                @"conv\.i8",
                IlOpCode.ConvToLong,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToFloat =
            new OpCodeWrapper(
                @"conv\.r4",
                IlOpCode.ConvToFloat,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToDouble =
            new OpCodeWrapper(
                @"conv\.r8",
                IlOpCode.ConvToDouble,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedByte =
            new OpCodeWrapper(
                @"conv\.u1",
                IlOpCode.ConvToUnsignedByte,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedShort =
            new OpCodeWrapper(
                @"conv\.u2",
                IlOpCode.ConvToUnsignedShort,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedInt =
            new OpCodeWrapper(
                @"conv\.u(4)?",
                IlOpCode.ConvToUnsignedInt,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedLong =
            new OpCodeWrapper(
                @"conv\.u8",
                IlOpCode.ConvToUnsignedLong,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToByteWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf\.i1",
                IlOpCode.ConvOvfToByte,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToShortWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf\.i2",
                IlOpCode.ConvOvfToShort,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToIntWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf.i(4)?",
                IlOpCode.ConvOvfToInt,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToLongWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf\.i8",
                IlOpCode.ConvOvfToLong,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedByteWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf\.u1",
                IlOpCode.ConvOvfToUnsignedByte,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedShortWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf\.u2",
                IlOpCode.ConvOvfToUnsignedShort,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedIntWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf.u(4)?",
                IlOpCode.ConvOvfToUnsignedInt,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ConvertToUnsignedLongWithOverflow =
            new OpCodeWrapper(
                @"conv\.ovf\.u8",
                IlOpCode.ConvOvfToUnsignedLong,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper CopyBulk =
            new OpCodeWrapper(
                @"cpblk",
                IlOpCode.CopyBulk,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper Divide =
            new OpCodeWrapper(
                @"div",
                IlOpCode.Div,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper DivideUnsigned =
            new OpCodeWrapper(
                @"div\.un",
                IlOpCode.DivUnsigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper Duplicate =
            new OpCodeWrapper(
                @"dup",
                IlOpCode.Dup,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push2,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper EndFilter =
            new OpCodeWrapper(
                @"endfilter",
                IlOpCode.Endfilter,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper EndFinally =
            new OpCodeWrapper(
                @"endfinally",
                IlOpCode.Endfinally,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper InitBulk =
            new OpCodeWrapper(
                @"initblk",
                IlOpCode.InitBulk,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper Jump =
            new OpCodeWrapper(
                @"jmp",
                IlOpCode.Jmp,
                FlowType.MethodCall,
                OpArgumentType.Method,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadArgument =
            new OpCodeWrapper(
                @"ldarg(\.s)?",
                IlOpCode.LoadArgument,
                FlowType.Next,
                OpArgumentType.ArgumentId,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadArgument0 =
            new OpCodeWrapper(
                @"ldarg\.0",
                IlOpCode.LoadArgument0,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadArgument1 =
            new OpCodeWrapper(
                @"ldarg\.1",
                IlOpCode.LoadArgument1,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadArgument2 =
            new OpCodeWrapper(
                @"ldarg\.2",
                IlOpCode.LoadArgument2,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadArgument3 =
            new OpCodeWrapper(
                @"ldarg\.3",
                IlOpCode.LoadArgument3,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadArgumentByRef =
            new OpCodeWrapper(
                @"ldarga(\.s)?",
                IlOpCode.LoadArgumentAddress,
                FlowType.Next,
                OpArgumentType.ArgumentId,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadInt =
            new OpCodeWrapper(
                @"ldc\.i4(\.s)?",
                IlOpCode.LoadConstantInt,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue0 =
            new OpCodeWrapper(
                @"ldc\.i4\.0",
                IlOpCode.LoadConstantInt0,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue1 =
            new OpCodeWrapper(
                @"ldc\.i4\.1",
                IlOpCode.LoadConstantInt1,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue2 =
            new OpCodeWrapper(
                @"ldc\.i4\.2",
                IlOpCode.LoadConstantInt2,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue3 =
            new OpCodeWrapper(
                @"ldc\.i4\.3",
                IlOpCode.LoadConstantInt3,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue4 =
            new OpCodeWrapper(
                @"ldc\.i4\.4",
                IlOpCode.LoadConstantInt4,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue5 =
            new OpCodeWrapper(
                @"ldc\.i4\.5",
                IlOpCode.LoadConstantInt5,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue6 =
            new OpCodeWrapper(
                @"ldc\.i4\.6",
                IlOpCode.LoadConstantInt6,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue7 =
            new OpCodeWrapper(
                @"ldc\.i4\.7",
                IlOpCode.LoadConstantInt7,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValue8 =
            new OpCodeWrapper(
                @"ldc\.i4\.8",
                IlOpCode.LoadConstantInt8,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIntValueNeg1 =
            new OpCodeWrapper(
                @"ldc\.i4\.m1",
                IlOpCode.LoadConstantIntNeg1,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLong =
            new OpCodeWrapper(
                @"ldc\.i8(\.s)?",
                IlOpCode.LoadConstantLong,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadFloat =
            new OpCodeWrapper(
                @"ldc\.r4(\.s)?",
                IlOpCode.LoadConstantFloat,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadDouble =
            new OpCodeWrapper(
                @"ldc\.r8(\.s)?",
                IlOpCode.LoadConstantDouble,
                FlowType.Next,
                OpArgumentType.Constant,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadMethodPointer =
            new OpCodeWrapper(
                @"ldftn",
                IlOpCode.LoadMethodPointer,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadIndirect =
            new OpCodeWrapper(
                @"ldind(\.[iuref1248]*)?",
                IlOpCode.LoadIndirect,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper LoadLocal =
            new OpCodeWrapper(
                @"ldloc(\.s)?",
                IlOpCode.LoadLocal,
                FlowType.Next,
                OpArgumentType.LocalVariableId,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal0 =
            new OpCodeWrapper(
                @"ldloc\.0",
                IlOpCode.LoadLocal0,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal1 =
            new OpCodeWrapper(
                @"ldloc\.1",
                IlOpCode.LoadLocal1,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal2 =
            new OpCodeWrapper(
                @"ldloc\.2",
                IlOpCode.LoadLocal2,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal3 =
            new OpCodeWrapper(
                @"ldloc\.3",
                IlOpCode.LoadLocal3,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocalByRef =
            new OpCodeWrapper(
                @"ldloca(\.s)?",
                IlOpCode.LoadLocalAddress,
                FlowType.Next,
                OpArgumentType.LocalVariableId,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal0ByRef =
            new OpCodeWrapper(
                @"ldloca\.0",
                IlOpCode.LoadLocalAddress0,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal1ByRef =
            new OpCodeWrapper(
                @"ldloca\.1",
                IlOpCode.LoadLocalAddress1,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal2ByRef =
            new OpCodeWrapper(
                @"ldloca\.2",
                IlOpCode.LoadLocalAddress2,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadLocal3ByRef =
            new OpCodeWrapper(
                @"ldloca\.3",
                IlOpCode.LoadLocalAddress3,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadNull =
            new OpCodeWrapper(
                @"ldnull",
                IlOpCode.LoadNull,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper Leave =
            new OpCodeWrapper(
                @"leave(\.s)?",
                IlOpCode.Leave,
                FlowType.Branch,
                OpArgumentType.BranchTarget,
                StackPushCode.Push0,
                StackPopCode.PopAll);

        public static readonly OpCodeWrapper Localloc =
            new OpCodeWrapper(
                @"localloc",
                IlOpCode.Localalloc,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Multiply =
            new OpCodeWrapper(
                @"mul",
                IlOpCode.Mul,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper MultiplyWithOverflow =
            new OpCodeWrapper(
                @"mul\.ovf(\.un)?",
                IlOpCode.MulOvf,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper Negate =
            new OpCodeWrapper(
                @"neg",
                IlOpCode.Neg,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper NoOp =
            new OpCodeWrapper(
                @"nop",
                IlOpCode.Nop,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper Not =
            new OpCodeWrapper(
                @"not",
                IlOpCode.Not,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Or =
            new OpCodeWrapper(
                @"or",
                IlOpCode.Or,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Pop =
            new OpCodeWrapper(
                @"pop",
                IlOpCode.Pop,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Remainder =
            new OpCodeWrapper(
                @"rem",
                IlOpCode.Remainder,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper RemainderUnsigned =
            new OpCodeWrapper(
                @"rem\.un",
                IlOpCode.RemainderUnsigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper Return =
            new OpCodeWrapper(
                @"ret",
                IlOpCode.Return,
                FlowType.Return,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.PopAll);

        public static readonly OpCodeWrapper ShiftLeft =
            new OpCodeWrapper(
                @"shl",
                IlOpCode.ShiftLeft,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper ShiftRight =
            new OpCodeWrapper(
                @"shr",
                IlOpCode.ShiftRight,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper ShiftRightUnsigned =
            new OpCodeWrapper(
                @"shr\.un",
                IlOpCode.ShiftRightUnsigned,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper StoreArgument =
            new OpCodeWrapper(
                @"starg(\.s)?",
                IlOpCode.StoreArgument,
                FlowType.Next,
                OpArgumentType.ArgumentId,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper StoreIndirect =
            new OpCodeWrapper(
                @"stind(\.[iuref1248]*)?",
                IlOpCode.StoreIndirect,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper StoreLocal =
            new OpCodeWrapper(
                @"stloc(\.s)?",
                IlOpCode.StoreLocal,
                FlowType.Next,
                OpArgumentType.LocalVariableId,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper StoreLocal0 =
            new OpCodeWrapper(
                @"stloc\.0",
                IlOpCode.StoreLocal0,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper StoreLocal1 =
            new OpCodeWrapper(
                @"stloc\.1",
                IlOpCode.StoreLocal1,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper StoreLocal2 =
            new OpCodeWrapper(
                @"stloc\.2",
                IlOpCode.StoreLocal2,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper StoreLocal3 =
            new OpCodeWrapper(
                @"stloc\.3",
                IlOpCode.StoreLocal3,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Substract =
            new OpCodeWrapper(
                @"sub",
                IlOpCode.Sub,
                FlowType.Next,
                OpArgumentType.LocalVariableId,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper SubstractWithOverflow =
            new OpCodeWrapper(
                @"sub\.ovf(\.un)?",
                IlOpCode.SubOvf,
                FlowType.Next,
                OpArgumentType.LocalVariableId,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper Switch =
            new OpCodeWrapper(
                @"switch",
                IlOpCode.Switch,
                FlowType.Switch,
                OpArgumentType.SwitchTargets,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Xor =
            new OpCodeWrapper(
                @"xor",
                IlOpCode.Xor,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper Box =
            new OpCodeWrapper(
                @"box",
                IlOpCode.Box,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper CallVirtual =
            new OpCodeWrapper(
                @"callvirt",
                IlOpCode.Callvirt,
                FlowType.MethodCall,
                OpArgumentType.None,
                StackPushCode.PushVar,
                StackPopCode.PopVar);

        public static readonly OpCodeWrapper CastClass =
            new OpCodeWrapper(
                @"castclass",
                IlOpCode.Castclass,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper CopyObject =
            new OpCodeWrapper(
                @"cpobj",
                IlOpCode.Cpobj,
                FlowType.Unsuported,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper InitObject =
            new OpCodeWrapper(
                @"initobj",
                IlOpCode.Initobj,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper IsInstance =
            new OpCodeWrapper(
                @"isinst",
                IlOpCode.Isinst,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        //public static readonly OpCode InitObject =
        //    new OpCode(
        //        @"initobj",
        //        IlOpCode.Initobj,
        //        FlowType.Next,
        //        OpArgumentType.ObjectType,
        //        StackPushCode.Push0,
        //        StackPopCode.Pop1);

        public static readonly OpCodeWrapper LoadArrayElement =
            new OpCodeWrapper(
                @"ldelem",
                IlOpCode.LoadArrayElement,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadByteArrayElement =
            new OpCodeWrapper(
                @"ldelem\.i1",
                IlOpCode.LoadArrayElementByte,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadShortArrayElement =
            new OpCodeWrapper(
                @"ldelem\.i2",
                IlOpCode.LoadArrayElementShort,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadIntArrayElement =
            new OpCodeWrapper(
                @"ldelem\.i(4)?",
                IlOpCode.LoadArrayElementInt,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadLongArrayElement =
            new OpCodeWrapper(
                @"ldelem\.i8",
                IlOpCode.LoadArrayElementLong,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadUnsignedByteArrayElement =
            new OpCodeWrapper(
                @"ldelem\.u1",
                IlOpCode.LoadArrayElementUnsignedByte,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadUnsignedShortArrayElement =
            new OpCodeWrapper(
                @"ldelem\.u2",
                IlOpCode.LoadArrayElementUnsignedShort,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadUnsignedIntArrayElement =
            new OpCodeWrapper(
                @"ldelem\.u4",
                IlOpCode.LoadArrayElementUnsignedInt,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadUnsignedLongArrayElement =
            new OpCodeWrapper(
                @"ldelem\.u8",
                IlOpCode.LoadArrayElementUnsignedLong,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadFloatArrayElement =
            new OpCodeWrapper(
                @"ldelem\.r4",
                IlOpCode.LoadArrayElementFloat,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadDoubleArrayElement =
            new OpCodeWrapper(
                @"ldelem\.r8",
                IlOpCode.LoadArrayElementDouble,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadObjectArrayElement =
            new OpCodeWrapper(
                @"ldelem\.ref",
                IlOpCode.LoadArrayElementObject,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadArrayElementAddress =
            new OpCodeWrapper(
                @"ldelema",
                IlOpCode.LoadArrayElementAddress,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper LoadField =
            new OpCodeWrapper(
                @"ldfld",
                IlOpCode.LoadField,
                FlowType.Next,
                OpArgumentType.Field,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper LoadFieldAddress =
            new OpCodeWrapper(
                @"ldflda",
                IlOpCode.LoadFieldAddress,
                FlowType.Next,
                OpArgumentType.Field,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper LoadLength =
            new OpCodeWrapper(
                @"ldlen",
                IlOpCode.LoadArrayLength,
                FlowType.Next,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper LoadObject =
            new OpCodeWrapper(
                @"ldobj",
                IlOpCode.LoadObject,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper LoadStaticField =
            new OpCodeWrapper(
                @"ldsfld",
                IlOpCode.LoadStaticField,
                FlowType.Next,
                OpArgumentType.Field,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadStaticFieldAddress =
            new OpCodeWrapper(
                @"ldsflda",
                IlOpCode.LoadStaticFieldAddress,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadString =
            new OpCodeWrapper(
                @"ldstr",
                IlOpCode.LoadString,
                FlowType.Next,
                OpArgumentType.ConstantString,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadToken =
            new OpCodeWrapper(
                @"ldtoken",
                IlOpCode.LoadToken,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper LoadVirtualFunction =
            new OpCodeWrapper(
                @"ldvirtftn",
                IlOpCode.LoadVirtualFunction,
                FlowType.Next,
                OpArgumentType.Method,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper MakeReferenceAny =
            new OpCodeWrapper(
                @"mkrefany",
                IlOpCode.MakeReferenceAny,
                FlowType.Unsuported,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper NewArray =
            new OpCodeWrapper(
                @"newarr",
                IlOpCode.NewArray,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper NewObject =
            new OpCodeWrapper(
                @"newobj",
                IlOpCode.NewObject,
                FlowType.Next,
                OpArgumentType.Method,
                StackPushCode.Push1,
                StackPopCode.PopVar);

        public static readonly OpCodeWrapper ReferAnyType =
            new OpCodeWrapper(
                @"refanytype",
                IlOpCode.Refanytype,
                FlowType.Unsuported,
                OpArgumentType.None,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper ReferAnyValue =
            new OpCodeWrapper(
                @"refanyval",
                IlOpCode.Refanyval,
                FlowType.Unsuported,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Rethrow =
            new OpCodeWrapper(
                @"rethrow",
                IlOpCode.Rethrow,
                FlowType.Throw,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop0);

        public static readonly OpCodeWrapper SizeOf =
            new OpCodeWrapper(
                @"sizeof",
                IlOpCode.Sizeof,
                FlowType.Unsuported,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper StoreArrayElement =
            new OpCodeWrapper(
                @"stelem",
                IlOpCode.StoreArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreByteArrayElement =
            new OpCodeWrapper(
                @"stelem\.i1",
                IlOpCode.StoreByteArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        //public static readonly OpCode StoreByteArrayElement =
        //    new OpCode(
        //        @"stelem\.i1",
        //        IlOpCode.StoreByteArrayElement,
        //        FlowType.Next,
        //        OpArgumentType.ObjectType,
        //        StackPushCode.Push0,
        //        StackPopCode.Pop2);

        public static readonly OpCodeWrapper StoreShortArrayElement =
            new OpCodeWrapper(
                @"stelem\.i2",
                IlOpCode.StoreShortArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreIntArrayElement =
            new OpCodeWrapper(
                @"stelem\.i(4)?",
                IlOpCode.StoreIntArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreLongArrayElement =
            new OpCodeWrapper(
                @"stelem\.i8",
                IlOpCode.StoreLongArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreFloatArrayElement =
            new OpCodeWrapper(
                @"stelem\.r4",
                IlOpCode.StoreFloatArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreDoubleArrayElement =
            new OpCodeWrapper(
                @"stelem\.r8",
                IlOpCode.StoreDoubleArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreObjectArrayElement =
            new OpCodeWrapper(
                @"stelem\.ref",
                IlOpCode.StoreObjectArrayElement,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop3);

        public static readonly OpCodeWrapper StoreField =
            new OpCodeWrapper(
                @"stfld",
                IlOpCode.StoreField,
                FlowType.Next,
                OpArgumentType.Field,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper StoreObject =
            new OpCodeWrapper(
                @"stobj",
                IlOpCode.StoreObject,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push0,
                StackPopCode.Pop2);

        public static readonly OpCodeWrapper StoreStaticField =
            new OpCodeWrapper(
                @"stsfld",
                IlOpCode.StoreStaticField,
                FlowType.Next,
                OpArgumentType.Field,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper Throw =
            new OpCodeWrapper(
                @"throw",
                IlOpCode.Throw,
                FlowType.Throw,
                OpArgumentType.None,
                StackPushCode.Push0,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper UnBox =
            new OpCodeWrapper(
                @"unbox",
                IlOpCode.Unbox,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);

        public static readonly OpCodeWrapper UnBoxAny =
            new OpCodeWrapper(
                @"unbox\.any",
                IlOpCode.UnboxAny,
                FlowType.Next,
                OpArgumentType.ObjectType,
                StackPushCode.Push1,
                StackPopCode.Pop1);
    }
}
