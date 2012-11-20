using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.AsmDeasm.Ops
{
    public enum IlOpCode
    {
        // add numeric values	33
        Add,
        // add integer values with overflow check	34
        AddOvfSigned,
        // add integer values with overflow check	34
        AddOvfUnsigned,
        // bitwise AND	35
        And,
        // get argument list	36
        Arglist,
        // branch on equal	37
        BranchIfEqual,
        // branch on greater than or equal to	38
        BranchIfGreaterOrEqual,
        // branch on greater than or equal to, unsigned or unordered	39
        BranchIfGreaterOrEqualUnsigned,
        // branch on greater than	40
        BranchIfGreater,
        // branch on greater than, unsigned or unordered	41
        BranchIfGreaterUnsigned,
        // branch on less than or equal to	42
        BranchIfLessOrEqual,
        // branch on less than or equal to, unsigned or unordered	43
        BranchIfLessOrEqualUnsigned,
        // branch on less than	44
        BranchIfLessThan,
        // branch on less than, unsigned or unordered	45
        BranchIfLessThanUnsigned,
        // branch on not equal or unordered	46
        BranchIfNotEqualsUnsigned,
        // unconditional branch	47
        Branch,
        // breakpoint instruction	48
        Break,
        // branch on false, null, or zero	49
        BranchIfFalse,
        // branch on non-false or non-null	50
        BranchIfTrue,
        // call a method	51
        Call,
        // indirect method call	53
        Calli,
        // compare equal	54
        CheckEquals,
        // compare greater than	55
        CheckGreater,
        // compare greater than, unsigned or unordered	56
        CheckGreaterUnsigned,
        // check for a finite real number	57
        Ckfinite,
        // compare less than	58
        CheckLesser,
        // compare less than, unsigned or unordered	59
        CheckLesserUnsigned,
        //– data conversion	60
        ConvToByte,
        //– data conversion	60
        ConvToShort,
        //– data conversion	60
        ConvToInt,
        //– data conversion	60
        ConvToLong,
        //– data conversion	60
        ConvToFloat,
        //– data conversion	60
        ConvToDouble,
        //– data conversion	60
        ConvToUnsignedByte,
        //– data conversion	60
        ConvToUnsignedShort,
        //– data conversion	60
        ConvToUnsignedInt,
        //– data conversion	60
        ConvToUnsignedLong,
        //– data conversion with overflow detection	61
        ConvOvfToByte,
        //– data conversion with overflow detection	61
        ConvOvfToShort,
        //– data conversion with overflow detection	61
        ConvOvfToInt,
        //– data conversion with overflow detection	61
        ConvOvfToLong,
        //– data conversion with overflow detection	61
        ConvOvfToUnsignedByte,
        //– data conversion with overflow detection	61
        ConvOvfToUnsignedShort,
        //– data conversion with overflow detection	61
        ConvOvfToUnsignedInt,
        //– data conversion with overflow detection	61
        ConvOvfToUnsignedLong,
        // copy data from memory to memory	63
        CopyBulk,
        // divide values	64
        Div,
        // divide integer values, unsigned	65
        DivUnsigned,
        // duplicate the top value of the stack	66
        Dup,
        // end exception handling filter clause	67
        Endfilter,
        // end the finally or fault clause of an exception block	68
        Endfinally,
        // initialize a block of memory to a value	69
        InitBulk,
        // jump to method	70
        Jmp,
        // load argument onto the stack	71
        LoadArgument,
        // load argument onto the stack	71
        LoadArgument0,
        // load argument onto the stack	71
        LoadArgument1,
        // load argument onto the stack	71
        LoadArgument2,
        // load argument onto the stack	71
        LoadArgument3,
        // load an argument address	72
        LoadArgumentAddress,
        // load numeric constant	73
        LoadConstantInt,
        // load numeric constant	73
        LoadConstantInt0,
        // load numeric constant	73
        LoadConstantInt1,
        // load numeric constant	73
        LoadConstantInt2,
        // load numeric constant	73
        LoadConstantInt3,
        // load numeric constant	73
        LoadConstantInt4,
        // load numeric constant	73
        LoadConstantInt5,
        // load numeric constant	73
        LoadConstantInt6,
        // load numeric constant	73
        LoadConstantInt7,
        // load numeric constant	73
        LoadConstantInt8,
        // load numeric constant	73
        LoadConstantIntNeg1,
        // load numeric constant	73
        LoadConstantLong,
        // load numeric constant	73
        LoadConstantFloat,
        // load numeric constant	73
        LoadConstantDouble,
        // load method pointer	74
        LoadMethodPointer,
        // load value indirect onto the stack	75
        LoadIndirect,
        // load local variable onto the stack	77
        LoadLocal,
        // load local variable onto the stack	77
        LoadLocal0,
        // load local variable onto the stack	77
        LoadLocal1,
        // load local variable onto the stack	77
        LoadLocal2,
        // load local variable onto the stack	77
        LoadLocal3,
        // load local variable address	78
        LoadLocalAddress,
        // load local variable address	78
        LoadLocalAddress0,
        // load local variable address	78
        LoadLocalAddress1,
        // load local variable address	78
        LoadLocalAddress2,
        // load local variable address	78
        LoadLocalAddress3,
        // load a null pointer	79
        LoadNull,
        // exit a protected region of code	80
        Leave,
        // allocate space in the local dynamic memory pool	81
        Localalloc,
        // multiply values	82
        Mul,
        // multiply integer values with overflow check	83
        MulOvf,
        // negate	84
        Neg,
        // no operation	85
        Nop,
        // bitwise complement	86
        Not,
        // bitwise OR	87
        Or,
        // remove the top element of the stack	88
        Pop,
        // compute remainder	89
        Remainder,
        // compute integer remainder, unsigned	91
        RemainderUnsigned,
        // return from method	92
        Return,
        // shift integer left	93
        ShiftLeft,
        // shift integer right	94
        ShiftRight,
        // shift integer right, unsigned	95
        ShiftRightUnsigned,
        // store a value in an argument slot	96
        StoreArgument,
        // store value indirect from stack	97
        StoreIndirect,
        // pop value from stack to local variable	98
        StoreLocal,
        // subtract numeric values	99
        StoreLocal0,
        // subtract numeric values	99
        StoreLocal1,
        // subtract numeric values	99
        StoreLocal2,
        // subtract numeric values	99
        StoreLocal3,
        // subtract numeric values	99
        Sub,
        // subtract integer values, checking for overflow	100
        SubOvf,
        // table switch based on value	101
        Switch,
        // bitwise XOR	102
        Xor,
        // convert a boxable value to its boxed form	104
        Box,
        // call a method associated, at runtime, with an object	105
        Callvirt,
        // cast an object to a class	106
        Castclass,
        // copy a value from one address to another	107
        Cpobj,
        // initialize the value at an address	108
        Initobj,
        // test if an object is an instance of a class or interface	109
        Isinst,
        // load element from array	110
        LoadArrayElement,
        // load an element of an array	111
        LoadArrayElementByte,
        // load an element of an array	111
        LoadArrayElementShort,
        // load an element of an array	111
        LoadArrayElementInt,
        // load an element of an array	111
        LoadArrayElementLong,
        // load an element of an array	111
        LoadArrayElementUnsignedByte,
        // load an element of an array	111
        LoadArrayElementUnsignedShort,
        // load an element of an array	111
        LoadArrayElementUnsignedInt,
        // load an element of an array	111
        LoadArrayElementUnsignedLong,
        // load an element of an array	111
        LoadArrayElementFloat,
        // load an element of an array	111
        LoadArrayElementDouble,
        // load an element of an array	111
        LoadArrayElementObject,
        // load address of an element of an array	113
        LoadArrayElementAddress,
        // load field of an object	114
        LoadField,
        // load field address	115
        LoadFieldAddress,
        // load the length of an array	116
        LoadArrayLength,
        // copy a value from an address to the stack	117
        LoadObject,
        // load static field of a class	118
        LoadStaticField,
        // load static field address	119
        LoadStaticFieldAddress,
        // load a literal string	120
        LoadString,
        // load the runtime representation of a metadata token	121
        LoadToken,
        // load a virtual method pointer	122
        LoadVirtualFunction,
        // push a typed reference on the stack	123
        MakeReferenceAny,
        // create a zero-based, one-dimensional array	124
        NewArray,
        // create a new object	125
        NewObject,
        // load the type out of a typed reference	126
        Refanytype,
        // load the address out of a typed reference	127
        Refanyval,
        // rethrow the current exception	128
        Rethrow,
        // load the size, in bytes,of a type	129
        Sizeof,
        // store element to array	130
        StoreArrayElement,
        // store an element of an array	131
        StoreByteArrayElement,
        // store an element of an array	131
        StoreShortArrayElement,
        // store an element of an array	131
        StoreIntArrayElement,
        // store an element of an array	131
        StoreLongArrayElement,
        // store an element of an array	131
        StoreFloatArrayElement,
        // store an element of an array	131
        StoreDoubleArrayElement,
        // store an element of an array	131
        StoreObjectArrayElement,
        // store into a field of an object	132
        StoreField,
        // store a value at an address	133
        StoreObject,
        // store a static field of a class	134
        StoreStaticField,
        // throw an exception	135
        Throw,
        // convert boxed value type to its raw form	136
        Unbox,
        // convert boxed type to value	137
        UnboxAny,
    }
}
