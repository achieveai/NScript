
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern sscorlib
{
  .publickeytoken = (8F C0 E3 AF 5A BC B6 C4 )                         // ....Z...
  .ver 0:5:5:0
}
.assembly RealScript
{
  .custom instance void [sscorlib]System.ScriptAssemblyAttribute::.ctor() = ( 01 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module RealScript.dll
// MVID: {9B8F1EF1-10A0-47AC-8CC1-063591CBE323}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x002A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class interface private abstract auto ansi RealScript.BaseInterface
{
  .method public hidebysig newslot abstract virtual 
          instance void  Bar() cil managed
  {
  } // end of method BaseInterface::Bar

} // end of class RealScript.BaseInterface

.class interface public abstract auto ansi RealScript.Foo
{
  .method public hidebysig newslot abstract virtual 
          instance int32  Foo() cil managed
  {
  } // end of method Foo::Foo

} // end of class RealScript.Foo

.class private auto ansi beforefieldinit RealScript.SimplImpl
       extends [sscorlib]System.Object
       implements RealScript.BaseInterface,
                  RealScript.Foo
{
  .method public hidebysig newslot virtual 
          instance void  Bar() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .language '{3F5162F8-07C6-11D3-9053-00C04FA302A1}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 14,14 : 35,36 'F:\\Projects\\work\\Cs2JsC\\RealScript\\TestInheritence.cs'
    IL_0000:  nop
    .line 14,14 : 37,38 ''
    IL_0001:  ret
  } // end of method SimplImpl::Bar

  .method private hidebysig newslot virtual final 
          instance int32  RealScript.Foo.Foo() cil managed
  {
    .override RealScript.Foo::Foo
    // Code size       7 (0x7)
    .maxstack  1
    .locals init ([0] int32 CS$1$0000)
    .line 19,19 : 9,10 ''
    IL_0000:  nop
    .line 20,20 : 13,22 ''
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    .line 21,21 : 9,10 ''
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method SimplImpl::RealScript.Foo.Foo

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [sscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SimplImpl::.ctor

} // end of class RealScript.SimplImpl

.class private auto ansi beforefieldinit RealScript.SimplImpl2
       extends RealScript.SimplImpl
{
  .method public hidebysig virtual instance void 
          Bar() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 28,28 : 36,37 ''
    IL_0000:  nop
    .line 28,28 : 38,39 ''
    IL_0001:  ret
  } // end of method SimplImpl2::Bar

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void RealScript.SimplImpl::.ctor()
    IL_0006:  ret
  } // end of method SimplImpl2::.ctor

} // end of class RealScript.SimplImpl2

.class private abstract auto ansi beforefieldinit RealScript.BaseClass
       extends [sscorlib]System.Object
       implements RealScript.BaseInterface
{
  .method public hidebysig newslot virtual 
          instance void  Foo() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 35,35 : 9,10 ''
    IL_0000:  nop
    .line 36,36 : 9,10 ''
    IL_0001:  ret
  } // end of method BaseClass::Foo

  .method public hidebysig newslot abstract virtual 
          instance void  Bar() cil managed
  {
  } // end of method BaseClass::Bar

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [sscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method BaseClass::.ctor

} // end of class RealScript.BaseClass

.class private auto ansi beforefieldinit RealScript.TestInheritence
       extends RealScript.BaseClass
{
  .method public hidebysig virtual instance void 
          Foo() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    .line 44,44 : 9,10 ''
    IL_0000:  nop
    .line 45,45 : 13,24 ''
    IL_0001:  ldarg.0
    IL_0002:  call       instance void RealScript.BaseClass::Foo()
    IL_0007:  nop
    .line 46,46 : 9,10 ''
    IL_0008:  ret
  } // end of method TestInheritence::Foo

  .method public hidebysig virtual instance void 
          Bar() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 49,49 : 9,10 ''
    IL_0000:  nop
    .line 50,50 : 9,10 ''
    IL_0001:  ret
  } // end of method TestInheritence::Bar

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void RealScript.BaseClass::.ctor()
    IL_0006:  ret
  } // end of method TestInheritence::.ctor

} // end of class RealScript.TestInheritence

.class private auto ansi beforefieldinit RealScript.TestInheritence2
       extends RealScript.BaseClass
{
  .method public hidebysig instance void 
          Foo() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 56,56 : 9,10 ''
    IL_0000:  nop
    .line 57,57 : 9,10 ''
    IL_0001:  ret
  } // end of method TestInheritence2::Foo

  .method public hidebysig virtual instance void 
          Bar() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 60,60 : 9,10 ''
    IL_0000:  nop
    .line 60,60 : 11,12 ''
    IL_0001:  ret
  } // end of method TestInheritence2::Bar

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void RealScript.BaseClass::.ctor()
    IL_0006:  ret
  } // end of method TestInheritence2::.ctor

} // end of class RealScript.TestInheritence2

.class public auto ansi beforefieldinit RealScript.TestGeneric`1<T>
       extends [sscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit TestSubClass<T>
         extends [sscorlib]System.Object
  {
    .method public hidebysig static !T  Foo<V>(!T bar,
                                               !T foo) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init ([0] !T CS$1$0000)
      .line 14,14 : 13,14 'F:\\Projects\\work\\Cs2JsC\\RealScript\\Class1.cs'
      IL_0000:  nop
      .line 15,15 : 17,28 ''
      IL_0001:  ldarg.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      .line 16,16 : 13,14 ''
      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method TestSubClass::Foo

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [sscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method TestSubClass::.ctor

  } // end of class TestSubClass

  .method public hidebysig instance !T  Foo(!T bar) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init ([0] !T CS$1$0000)
    .line 20,20 : 9,10 ''
    IL_0000:  nop
    .line 21,21 : 13,24 ''
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    .line 22,22 : 9,10 ''
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method TestGeneric`1::Foo

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [sscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestGeneric`1::.ctor

} // end of class RealScript.TestGeneric`1

.class public auto ansi beforefieldinit RealScript.TestGeneric
       extends [sscorlib]System.Object
{
  .method public hidebysig instance int32 
          Foo(int32 bar) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init ([0] int32 CS$1$0000)
    .line 27,27 : 33,34 ''
    IL_0000:  nop
    .line 27,27 : 35,46 ''
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    .line 27,27 : 47,48 ''
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method TestGeneric::Foo

  .method public hidebysig instance !!Too 
          Foo<valuetype .ctor ([sscorlib]System.ValueType) Too,class Uoo>(!!Too bar,
                                                                          !!Uoo var2) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init ([0] !!Too CS$1$0000)
    .line 29,29 : 9,10 ''
    IL_0000:  nop
    .line 29,29 : 11,22 ''
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    .line 29,29 : 23,24 ''
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method TestGeneric::Foo

  .method public hidebysig instance !!T  Foo<T,U>(!!U bar,
                                                  !!T foo) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init ([0] !!T CS$1$0000)
    .line 30,30 : 42,43 ''
    IL_0000:  nop
    .line 30,30 : 44,99 ''
    IL_0001:  ldarg.2
    IL_0002:  ldarg.2
    IL_0003:  call       !0 class RealScript.TestGeneric`1/TestSubClass<!!0>::Foo<bool>(!0,
                                                                                        !0)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    .line 30,30 : 100,101 ''
    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TestGeneric::Foo

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [sscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestGeneric::.ctor

} // end of class RealScript.TestGeneric

.class public auto ansi beforefieldinit RealScript.TempAttribute
       extends [sscorlib]System.Attribute
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [sscorlib]System.Attribute::.ctor()
    IL_0006:  ret
  } // end of method TempAttribute::.ctor

} // end of class RealScript.TempAttribute

.class interface public abstract auto ansi RealScript.FooBar
{
  .method public hidebysig newslot abstract virtual 
          instance void  Foo() cil managed
  {
  } // end of method FooBar::Foo

  .method public hidebysig newslot abstract virtual 
          instance int32  Bar(int32 foo) cil managed
  {
  } // end of method FooBar::Bar

} // end of class RealScript.FooBar

.class interface public abstract auto ansi RealScript.Fooo
       implements RealScript.Foo
{
  .method public hidebysig newslot abstract virtual 
          instance void  Fooo() cil managed
  {
  } // end of method Fooo::Fooo

} // end of class RealScript.Fooo

.class public auto ansi sealed RealScript.FooFunction
       extends [sscorlib]System.MulticastDelegate
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(object 'object',
                               native int 'method') runtime managed
  {
  } // end of method FooFunction::.ctor

  .method public hidebysig newslot virtual 
          instance int32  Invoke(int32 testc) runtime managed
  {
  } // end of method FooFunction::Invoke

} // end of class RealScript.FooFunction

.class public auto ansi beforefieldinit RealScript.Class1
       extends [sscorlib]System.Object
       implements [sscorlib]System.IDisposable,
                  RealScript.FooBar
{
  .custom instance void [sscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
  .field public int32 instanceFoo
  .field public static int32 staticBar
  .method public hidebysig instance bool 
          Func1(int32 i1,
                int32 i2) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  3
    .locals init ([0] int32 result,
             [1] bool CS$1$0000)
    .line 63,63 : 9,10 ''
    IL_0000:  nop
    .line 64,64 : 13,62 ''
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.0
    IL_0004:  call       instance class RealScript.FooFunction RealScript.Class1::GetFooFunction(bool)
    IL_0009:  ldarg.2
    IL_000a:  callvirt   instance int32 RealScript.FooFunction::Invoke(int32)
    IL_000f:  add
    IL_0010:  stloc.0
    .line 68,68 : 13,32 ''
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.s   10
    IL_0014:  cgt
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    .line 69,69 : 9,10 ''
    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method Class1::Func1

  .method public hidebysig instance void 
          FuncRef(int32& i1,
                  int32 i2,
                  int32 i3) cil managed
  {
    // Code size       81 (0x51)
    .maxstack  4
    .locals init ([0] bool CS$4$0000)
    .line 72,72 : 9,10 ''
    IL_0000:  nop
    .line 74,76 : 13,38 ''
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.3
    IL_0003:  bne.un.s   IL_001f

    IL_0005:  ldarg.3
    IL_0006:  ldc.i4.s   10
    IL_0008:  bne.un.s   IL_001f

    IL_000a:  ldarg.3
    IL_000b:  ldc.i4.2
    IL_000c:  beq.s      IL_002f

    IL_000e:  ldarg.2
    IL_000f:  ldarg.3
    IL_0010:  ldc.i4.s   10
    IL_0012:  beq.s      IL_001c

    IL_0014:  ldarg.3
    IL_0015:  ldc.i4.4
    IL_0016:  blt.s      IL_001c

    IL_0018:  ldc.i4.s   10
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.4
    IL_001d:  beq.s      IL_002f

    IL_001f:  ldarg.3
    IL_0020:  ldc.i4.4
    IL_0021:  bne.un.s   IL_002c

    IL_0023:  ldarg.2
    IL_0024:  ldc.i4.3
    IL_0025:  ceq
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.1
    IL_002d:  br.s       IL_0030

    IL_002f:  ldc.i4.0
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  brtrue.s   IL_0043

    .line 77,77 : 13,14 ''
    IL_0034:  nop
    .line 78,78 : 17,37 ''
    IL_0035:  ldarg.1
    IL_0036:  ldarg.0
    IL_0037:  ldarg.2
    IL_0038:  ldc.i4.s   15
    IL_003a:  call       instance int32 RealScript.Class1::Func2(int32,
                                                                 int32)
    IL_003f:  stind.i4
    .line 79,79 : 13,14 ''
    IL_0040:  nop
    IL_0041:  br.s       IL_0050

    .line 81,81 : 13,14 ''
    IL_0043:  nop
    .line 82,82 : 17,37 ''
    IL_0044:  ldarg.1
    IL_0045:  ldarg.0
    IL_0046:  ldarg.2
    IL_0047:  ldc.i4.s   25
    IL_0049:  call       instance int32 RealScript.Class1::Func2(int32,
                                                                 int32)
    IL_004e:  stind.i4
    .line 83,83 : 13,14 ''
    IL_004f:  nop
    .line 84,84 : 9,10 ''
    IL_0050:  ret
  } // end of method Class1::FuncRef

  .method public hidebysig instance int32 
          FuncIncrement(int32 i1,
                        int32 i2) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  6
    .locals init ([0] int32 CS$1$0000)
    .line 87,87 : 9,10 ''
    IL_0000:  nop
    .line 88,88 : 13,35 ''
    IL_0001:  ldarg.2
    IL_0002:  ldarg.0
    IL_0003:  ldarg.1
    IL_0004:  dup
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  starg.s    i1
    IL_0009:  ldc.i4.s   10
    IL_000b:  call       instance int32 RealScript.Class1::Func2(int32,
                                                                 int32)
    IL_0010:  add
    IL_0011:  starg.s    i2
    .line 90,90 : 13,35 ''
    IL_0013:  ldarg.2
    IL_0014:  ldarg.0
    IL_0015:  ldarg.1
    IL_0016:  ldarg.2
    IL_0017:  dup
    IL_0018:  ldc.i4.1
    IL_0019:  add
    IL_001a:  starg.s    i2
    IL_001c:  call       instance int32 RealScript.Class1::Func2(int32,
                                                                 int32)
    IL_0021:  add
    IL_0022:  starg.s    i2
    .line 92,92 : 13,23 ''
    IL_0024:  ldarg.2
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    .line 93,93 : 9,10 ''
    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method Class1::FuncIncrement

  .method public hidebysig instance int32 
          GetMore(int32 i) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000)
    .line 96,96 : 9,10 ''
    IL_0000:  nop
    .line 97,97 : 13,41 ''
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  ldfld      int32 RealScript.Class1::instanceFoo
    IL_0008:  add
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    .line 98,98 : 9,10 ''
    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method Class1::GetMore

  .method public hidebysig static int32  GetMoreStatic(int32 i) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000)
    .line 101,101 : 9,10 ''
    IL_0000:  nop
    .line 102,102 : 13,41 ''
    IL_0001:  ldarg.0
    IL_0002:  ldsfld     int32 RealScript.Class1::staticBar
    IL_0007:  add
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    .line 103,103 : 9,10 ''
    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method Class1::GetMoreStatic

  .method public hidebysig instance class RealScript.FooFunction 
          GetFooFunction(bool needsStatic) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  3
    .locals init ([0] class RealScript.FooFunction CS$1$0000,
             [1] bool CS$4$0001)
    .line 106,106 : 9,10 ''
    IL_0000:  nop
    .line 107,107 : 13,29 ''
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  brtrue.s   IL_0019

    .line 108,108 : 13,14 ''
    IL_0009:  nop
    .line 109,109 : 17,45 ''
    IL_000a:  ldnull
    IL_000b:  ldftn      int32 RealScript.Class1::GetMoreStatic(int32)
    IL_0011:  newobj     instance void RealScript.FooFunction::.ctor(object,
                                                                     native int)
    IL_0016:  stloc.0
    IL_0017:  br.s       IL_0029

    .line 112,112 : 13,14 ''
    IL_0019:  nop
    .line 113,113 : 17,33 ''
    IL_001a:  ldarg.0
    IL_001b:  ldftn      instance int32 RealScript.Class1::Bar(int32)
    IL_0021:  newobj     instance void RealScript.FooFunction::.ctor(object,
                                                                     native int)
    IL_0026:  stloc.0
    IL_0027:  br.s       IL_0029

    .line 115,115 : 9,10 ''
    IL_0029:  ldloc.0
    IL_002a:  ret
  } // end of method Class1::GetFooFunction

  .method public hidebysig specialname instance int32 
          get_Item(int32 tmp,
                   int32 foo) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000)
    .line 120,120 : 13,14 ''
    IL_0000:  nop
    .line 121,121 : 17,34 ''
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  add
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    .line 122,122 : 13,14 ''
    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method Class1::get_Item

  .method public hidebysig specialname instance void 
          set_Item(int32 tmp,
                   int32 foo,
                   int32 'value') cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    .line 125,125 : 13,14 ''
    IL_0000:  nop
    .line 126,126 : 17,40 ''
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  stfld      int32 RealScript.Class1::instanceFoo
    .line 127,127 : 17,40 ''
    IL_0008:  ldarg.2
    IL_0009:  stsfld     int32 RealScript.Class1::staticBar
    .line 128,128 : 13,14 ''
    IL_000e:  ret
  } // end of method Class1::set_Item

  .method public hidebysig instance void 
          Swap<T>(!!T& r1,
                  !!T& r2) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init ([0] !!T tmp)
    .line 132,132 : 9,10 ''
    IL_0000:  nop
    .line 133,133 : 13,24 ''
    IL_0001:  ldarg.1
    IL_0002:  ldobj      !!T
    IL_0007:  stloc.0
    .line 134,134 : 13,21 ''
    IL_0008:  ldarg.1
    IL_0009:  ldarg.2
    IL_000a:  ldobj      !!T
    IL_000f:  stobj      !!T
    .line 135,135 : 13,22 ''
    IL_0014:  ldarg.2
    IL_0015:  ldloc.0
    IL_0016:  stobj      !!T
    .line 136,136 : 9,10 ''
    IL_001b:  ret
  } // end of method Class1::Swap

  .method public hidebysig instance string 
          Print<valuetype .ctor ([sscorlib]System.ValueType) T,valuetype .ctor ([sscorlib]System.ValueType) U>(!!T i1,
                                                                                                               !!U i2) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init ([0] string CS$1$0000,
             [1] object[] CS$0$0001)
    .line 139,139 : 9,10 ''
    IL_0000:  nop
    .line 140,140 : 13,53 ''
    IL_0001:  ldstr      "{0}-{1}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.0
    IL_000f:  ldarg.1
    IL_0010:  box        !!T
    IL_0015:  stelem.ref
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  ldarg.2
    IL_0019:  box        !!U
    IL_001e:  stelem.ref
    IL_001f:  ldloc.1
    IL_0020:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    .line 141,141 : 9,10 ''
    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method Class1::Print

  .method public hidebysig instance string 
          Print2<(RealScript.Foo) T,(RealScript.FooBar) U>(!!T i1,
                                                           !!U i2) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init ([0] string CS$1$0000,
             [1] object[] CS$0$0001)
    .line 146,146 : 9,10 ''
    IL_0000:  nop
    .line 147,147 : 13,53 ''
    IL_0001:  ldstr      "{0}-{1}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.0
    IL_000f:  ldarg.1
    IL_0010:  box        !!T
    IL_0015:  stelem.ref
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  ldarg.2
    IL_0019:  box        !!U
    IL_001e:  stelem.ref
    IL_001f:  ldloc.1
    IL_0020:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    .line 148,148 : 9,10 ''
    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method Class1::Print2

  .method public hidebysig instance int64 
          Func2(int64 i1,
                int64 i2) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  1
    .locals init ([0] int64 CS$1$0000)
    .line 151,151 : 9,10 ''
    IL_0000:  nop
    .line 152,152 : 13,22 ''
    IL_0001:  ldc.i4.0
    IL_0002:  conv.i8
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    .line 153,153 : 9,10 ''
    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method Class1::Func2

  .method public hidebysig instance int32 
          Func2(int32 i1,
                int32 i2) cil managed
  {
    // Code size       112 (0x70)
    .maxstack  3
    .locals init ([0] bool b,
             [1] int32 returnValue,
             [2] int32 CS$1$0000,
             [3] bool CS$4$0001)
    .line 156,156 : 9,10 ''
    IL_0000:  nop
    .line 157,157 : 13,28 ''
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 158,158 : 13,33 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    .line 159,162 : 13,38 ''
    IL_0005:  ldarg.2
    IL_0006:  ldc.i4.3
    IL_0007:  bne.un.s   IL_001f

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.s   10
    IL_000c:  bne.un.s   IL_001f

    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.1
    IL_0010:  beq.s      IL_0017

    IL_0012:  ldarg.2
    IL_0013:  ldc.i4.s   9
    IL_0015:  bne.un.s   IL_001f

    IL_0017:  ldarg.1
    IL_0018:  ldc.i4.2
    IL_0019:  beq.s      IL_002f

    IL_001b:  ldarg.2
    IL_001c:  ldc.i4.3
    IL_001d:  beq.s      IL_002f

    IL_001f:  ldarg.1
    IL_0020:  ldc.i4.4
    IL_0021:  bne.un.s   IL_002c

    IL_0023:  ldarg.2
    IL_0024:  ldc.i4.3
    IL_0025:  ceq
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.1
    IL_002d:  br.s       IL_0030

    IL_002f:  ldc.i4.0
    IL_0030:  stloc.3
    IL_0031:  ldloc.3
    IL_0032:  brtrue.s   IL_003e

    .line 163,163 : 13,14 ''
    IL_0034:  nop
    .line 164,164 : 17,43 ''
    IL_0035:  ldarg.1
    IL_0036:  ldarg.2
    IL_0037:  ldc.i4.2
    IL_0038:  mul
    IL_0039:  add
    IL_003a:  stloc.1
    .line 165,165 : 13,14 ''
    IL_003b:  nop
    IL_003c:  br.s       IL_0066

    .line 166,166 : 18,39 ''
    IL_003e:  ldarg.1
    IL_003f:  ldc.i4.2
    IL_0040:  ble.s      IL_004b

    IL_0042:  ldarg.2
    IL_0043:  ldc.i4.3
    IL_0044:  clt
    IL_0046:  ldc.i4.0
    IL_0047:  ceq
    IL_0049:  br.s       IL_004c

    IL_004b:  ldc.i4.1
    IL_004c:  stloc.3
    IL_004d:  ldloc.3
    IL_004e:  brtrue.s   IL_0058

    .line 167,167 : 13,14 ''
    IL_0050:  nop
    .line 168,168 : 17,39 ''
    IL_0051:  ldarg.1
    IL_0052:  ldarg.2
    IL_0053:  add
    IL_0054:  stloc.1
    .line 169,169 : 13,14 ''
    IL_0055:  nop
    IL_0056:  br.s       IL_0066

    .line 170,170 : 18,24 ''
    IL_0058:  ldloc.0
    IL_0059:  ldc.i4.0
    IL_005a:  ceq
    IL_005c:  stloc.3
    IL_005d:  ldloc.3
    IL_005e:  brtrue.s   IL_0066

    .line 171,171 : 13,14 ''
    IL_0060:  nop
    .line 172,172 : 17,39 ''
    IL_0061:  ldarg.2
    IL_0062:  ldarg.1
    IL_0063:  sub
    IL_0064:  stloc.1
    .line 173,173 : 13,14 ''
    IL_0065:  nop
    .line 175,175 : 13,31 ''
    IL_0066:  ldloc.1
    IL_0067:  ldarg.2
    IL_0068:  add
    IL_0069:  stloc.1
    .line 177,177 : 13,32 ''
    IL_006a:  ldloc.1
    IL_006b:  stloc.2
    IL_006c:  br.s       IL_006e

    .line 178,178 : 9,10 ''
    IL_006e:  ldloc.2
    IL_006f:  ret
  } // end of method Class1::Func2

  .method public hidebysig instance int32 
          Foo1() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  5
    .locals init ([0] int32 i,
             [1] int32 j,
             [2] int32 k,
             [3] int32 CS$1$0000)
    .line 181,181 : 9,10 ''
    IL_0000:  nop
    .line 182,182 : 13,24 ''
    IL_0001:  ldc.i4.s   10
    IL_0003:  stloc.0
    .line 183,183 : 13,24 ''
    IL_0004:  ldc.i4.s   11
    IL_0006:  stloc.1
    .line 185,185 : 13,63 ''
    IL_0007:  ldarg.0
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 RealScript.Class1::instanceFoo
    IL_000e:  ldsfld     int32 RealScript.Class1::staticBar
    IL_0013:  call       instance int32 RealScript.Class1::Func2(int32,
                                                                 int32)
    IL_0018:  stloc.2
    .line 187,187 : 13,36 ''
    IL_0019:  ldarg.0
    IL_001a:  ldloca.s   k
    IL_001c:  ldloc.0
    IL_001d:  dup
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.0
    IL_0021:  ldloc.1
    IL_0022:  call       instance void RealScript.Class1::FuncRef(int32&,
                                                                  int32,
                                                                  int32)
    IL_0027:  nop
    .line 191,191 : 13,22 ''
    IL_0028:  ldloc.2
    IL_0029:  stloc.3
    IL_002a:  br.s       IL_002c

    .line 192,192 : 9,10 ''
    IL_002c:  ldloc.3
    IL_002d:  ret
  } // end of method Class1::Foo1

  .method public hidebysig instance void 
          Foo(int32 i) cil managed
  {
    // Code size       672 (0x2a0)
    .maxstack  4
    .locals init ([0] int32 j,
             [1] int32 k,
             [2] int32 l,
             [3] bool CS$4$0000,
             [4] object[] CS$0$0001)
    .line 195,195 : 9,10 ''
    IL_0000:  nop
    .line 196,196 : 13,39 ''
    IL_0001:  ldstr      "Starting"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    .line 198,198 : 18,28 ''
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_0062

    .line 199,199 : 13,14 ''
    IL_0016:  nop
    .line 200,200 : 17,29 ''
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.s   10
    IL_001a:  ceq
    IL_001c:  ldc.i4.0
    IL_001d:  ceq
    IL_001f:  stloc.3
    IL_0020:  ldloc.3
    IL_0021:  brtrue.s   IL_0045

    .line 201,201 : 17,18 ''
    IL_0023:  nop
    .line 202,202 : 21,45 ''
    IL_0024:  ldstr      "{0}"
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [sscorlib]System.Object
    IL_002f:  stloc.s    CS$0$0001
    IL_0031:  ldloc.s    CS$0$0001
    IL_0033:  ldc.i4.0
    IL_0034:  ldloc.0
    IL_0035:  box        [sscorlib]System.Int32
    IL_003a:  stelem.ref
    IL_003b:  ldloc.s    CS$0$0001
    IL_003d:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0042:  pop
    .line 203,203 : 21,27 ''
    IL_0043:  br.s       IL_006a

    .line 206,206 : 17,18 ''
    IL_0045:  nop
    .line 207,207 : 21,28 ''
    IL_0046:  ldloc.0
    IL_0047:  ldarg.1
    IL_0048:  add
    IL_0049:  stloc.0
    .line 208,208 : 17,18 ''
    IL_004a:  nop
    .line 209,209 : 13,14 ''
    IL_004b:  nop
    .line 198,198 : 36,77 ''
    IL_004c:  ldloc.0
    IL_004d:  ldarg.1
    IL_004e:  ldc.i4.s   10
    IL_0050:  beq.s      IL_005a

    IL_0052:  ldarg.1
    IL_0053:  ldc.i4.s   20
    IL_0055:  beq.s      IL_005a

    IL_0057:  ldc.i4.0
    IL_0058:  br.s       IL_005b

    IL_005a:  ldc.i4.1
    IL_005b:  add
    IL_005c:  stloc.0
    IL_005d:  ldarg.1
    IL_005e:  ldc.i4.1
    IL_005f:  sub
    IL_0060:  starg.s    i
    .line 198,198 : 29,34 ''
    IL_0062:  ldloc.0
    IL_0063:  ldarg.1
    IL_0064:  clt
    IL_0066:  stloc.3
    IL_0067:  ldloc.3
    IL_0068:  brtrue.s   IL_0016

    .line 211,211 : 13,37 ''
    IL_006a:  ldstr      "Ending"
    IL_006f:  ldc.i4.0
    IL_0070:  newarr     [sscorlib]System.Object
    IL_0075:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_007a:  pop
    .line 213,213 : 13,23 ''
    IL_007b:  ldc.i4.0
    IL_007c:  stloc.1
    IL_007d:  br.s       IL_00c7

    .line 215,215 : 13,14 ''
    IL_007f:  nop
    .line 216,216 : 17,29 ''
    IL_0080:  ldloc.1
    IL_0081:  ldc.i4.s   10
    IL_0083:  ceq
    IL_0085:  ldc.i4.0
    IL_0086:  ceq
    IL_0088:  stloc.3
    IL_0089:  ldloc.3
    IL_008a:  brtrue.s   IL_00af

    .line 217,217 : 17,18 ''
    IL_008c:  nop
    .line 218,218 : 21,45 ''
    IL_008d:  ldstr      "{0}"
    IL_0092:  ldc.i4.1
    IL_0093:  newarr     [sscorlib]System.Object
    IL_0098:  stloc.s    CS$0$0001
    IL_009a:  ldloc.s    CS$0$0001
    IL_009c:  ldc.i4.0
    IL_009d:  ldloc.1
    IL_009e:  box        [sscorlib]System.Int32
    IL_00a3:  stelem.ref
    IL_00a4:  ldloc.s    CS$0$0001
    IL_00a6:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00ab:  pop
    .line 219,219 : 17,18 ''
    IL_00ac:  nop
    IL_00ad:  br.s       IL_00b5

    .line 221,221 : 17,18 ''
    IL_00af:  nop
    .line 222,222 : 21,28 ''
    IL_00b0:  ldloc.1
    IL_00b1:  ldarg.1
    IL_00b2:  add
    IL_00b3:  stloc.1
    .line 223,223 : 17,18 ''
    IL_00b4:  nop
    .line 225,225 : 17,38 ''
    IL_00b5:  ldloc.1
    IL_00b6:  ldarg.1
    IL_00b7:  ldc.i4.s   10
    IL_00b9:  beq.s      IL_00be

    IL_00bb:  ldc.i4.2
    IL_00bc:  br.s       IL_00bf

    IL_00be:  ldc.i4.1
    IL_00bf:  add
    IL_00c0:  stloc.1
    .line 226,226 : 17,24 ''
    IL_00c1:  ldarg.1
    IL_00c2:  ldc.i4.1
    IL_00c3:  sub
    IL_00c4:  starg.s    i
    .line 227,227 : 13,14 ''
    IL_00c6:  nop
    .line 214,214 : 13,31 ''
    IL_00c7:  ldloc.1
    IL_00c8:  ldarg.1
    IL_00c9:  ldc.i4.s   10
    IL_00cb:  add
    IL_00cc:  clt
    IL_00ce:  stloc.3
    IL_00cf:  ldloc.3
    IL_00d0:  brtrue.s   IL_007f

    .line 229,229 : 13,44 ''
    IL_00d2:  ldstr      "{0}"
    IL_00d7:  ldc.i4.1
    IL_00d8:  newarr     [sscorlib]System.Object
    IL_00dd:  stloc.s    CS$0$0001
    IL_00df:  ldloc.s    CS$0$0001
    IL_00e1:  ldc.i4.0
    IL_00e2:  ldstr      "Ending"
    IL_00e7:  stelem.ref
    IL_00e8:  ldloc.s    CS$0$0001
    IL_00ea:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00ef:  pop
    .line 230,230 : 13,46 ''
    IL_00f0:  ldstr      "{0}"
    IL_00f5:  ldc.i4.1
    IL_00f6:  newarr     [sscorlib]System.Object
    IL_00fb:  stloc.s    CS$0$0001
    IL_00fd:  ldloc.s    CS$0$0001
    IL_00ff:  ldc.i4.0
    IL_0100:  ldstr      "Starting"
    IL_0105:  stelem.ref
    IL_0106:  ldloc.s    CS$0$0001
    IL_0108:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_010d:  pop
    .line 232,232 : 18,28 ''
    IL_010e:  ldc.i4.0
    IL_010f:  stloc.0
    IL_0110:  br         IL_01ac

    .line 233,233 : 13,14 ''
    IL_0115:  nop
    .line 234,234 : 17,29 ''
    IL_0116:  ldloc.0
    IL_0117:  ldc.i4.s   10
    IL_0119:  ceq
    IL_011b:  ldc.i4.0
    IL_011c:  ceq
    IL_011e:  stloc.3
    IL_011f:  ldloc.3
    IL_0120:  brtrue.s   IL_0176

    .line 235,235 : 17,18 ''
    IL_0122:  nop
    .line 236,236 : 21,33 ''
    IL_0123:  ldarg.1
    IL_0124:  ldc.i4.s   10
    IL_0126:  ceq
    IL_0128:  ldc.i4.0
    IL_0129:  ceq
    IL_012b:  stloc.3
    IL_012c:  ldloc.3
    IL_012d:  brtrue.s   IL_0151

    .line 237,237 : 21,22 ''
    IL_012f:  nop
    .line 238,238 : 25,49 ''
    IL_0130:  ldstr      "{0}"
    IL_0135:  ldc.i4.1
    IL_0136:  newarr     [sscorlib]System.Object
    IL_013b:  stloc.s    CS$0$0001
    IL_013d:  ldloc.s    CS$0$0001
    IL_013f:  ldc.i4.0
    IL_0140:  ldloc.0
    IL_0141:  box        [sscorlib]System.Int32
    IL_0146:  stelem.ref
    IL_0147:  ldloc.s    CS$0$0001
    IL_0149:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_014e:  pop
    .line 239,239 : 25,31 ''
    IL_014f:  br.s       IL_01b7

    .line 242,242 : 21,22 ''
    IL_0151:  nop
    .line 243,243 : 25,50 ''
    IL_0152:  ldstr      "{0}"
    IL_0157:  ldc.i4.1
    IL_0158:  newarr     [sscorlib]System.Object
    IL_015d:  stloc.s    CS$0$0001
    IL_015f:  ldloc.s    CS$0$0001
    IL_0161:  ldc.i4.0
    IL_0162:  ldc.i4.s   10
    IL_0164:  box        [sscorlib]System.Int32
    IL_0169:  stelem.ref
    IL_016a:  ldloc.s    CS$0$0001
    IL_016c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0171:  pop
    .line 244,244 : 21,22 ''
    IL_0172:  nop
    .line 245,245 : 17,18 ''
    IL_0173:  nop
    IL_0174:  br.s       IL_017c

    .line 247,247 : 17,18 ''
    IL_0176:  nop
    .line 248,248 : 21,28 ''
    IL_0177:  ldloc.0
    IL_0178:  ldarg.1
    IL_0179:  add
    IL_017a:  stloc.0
    .line 249,249 : 17,18 ''
    IL_017b:  nop
    .line 250,250 : 17,45 ''
    IL_017c:  ldstr      "{0}"
    IL_0181:  ldc.i4.1
    IL_0182:  newarr     [sscorlib]System.Object
    IL_0187:  stloc.s    CS$0$0001
    IL_0189:  ldloc.s    CS$0$0001
    IL_018b:  ldc.i4.0
    IL_018c:  ldstr      "foo"
    IL_0191:  stelem.ref
    IL_0192:  ldloc.s    CS$0$0001
    IL_0194:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0199:  pop
    .line 251,251 : 13,14 ''
    IL_019a:  nop
    .line 232,232 : 36,64 ''
    IL_019b:  ldloc.0
    IL_019c:  ldarg.1
    IL_019d:  ldc.i4.s   10
    IL_019f:  beq.s      IL_01a4

    IL_01a1:  ldc.i4.2
    IL_01a2:  br.s       IL_01a5

    IL_01a4:  ldc.i4.1
    IL_01a5:  add
    IL_01a6:  stloc.0
    IL_01a7:  ldarg.1
    IL_01a8:  ldc.i4.1
    IL_01a9:  sub
    IL_01aa:  starg.s    i
    .line 232,232 : 29,34 ''
    IL_01ac:  ldloc.0
    IL_01ad:  ldarg.1
    IL_01ae:  clt
    IL_01b0:  stloc.3
    IL_01b1:  ldloc.3
    IL_01b2:  brtrue     IL_0115

    .line 253,253 : 13,44 ''
    IL_01b7:  ldstr      "{0}"
    IL_01bc:  ldc.i4.1
    IL_01bd:  newarr     [sscorlib]System.Object
    IL_01c2:  stloc.s    CS$0$0001
    IL_01c4:  ldloc.s    CS$0$0001
    IL_01c6:  ldc.i4.0
    IL_01c7:  ldstr      "Ending"
    IL_01cc:  stelem.ref
    IL_01cd:  ldloc.s    CS$0$0001
    IL_01cf:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_01d4:  pop
    .line 255,255 : 13,23 ''
    IL_01d5:  ldc.i4.0
    IL_01d6:  stloc.2
    IL_01d7:  br         IL_0273

    .line 257,257 : 13,14 ''
    IL_01dc:  nop
    .line 258,258 : 17,29 ''
    IL_01dd:  ldloc.2
    IL_01de:  ldc.i4.s   10
    IL_01e0:  ceq
    IL_01e2:  ldc.i4.0
    IL_01e3:  ceq
    IL_01e5:  stloc.3
    IL_01e6:  ldloc.3
    IL_01e7:  brtrue.s   IL_023d

    .line 259,259 : 17,18 ''
    IL_01e9:  nop
    .line 260,260 : 21,33 ''
    IL_01ea:  ldarg.1
    IL_01eb:  ldc.i4.s   10
    IL_01ed:  ceq
    IL_01ef:  ldc.i4.0
    IL_01f0:  ceq
    IL_01f2:  stloc.3
    IL_01f3:  ldloc.3
    IL_01f4:  brtrue.s   IL_0218

    .line 261,261 : 21,22 ''
    IL_01f6:  nop
    .line 262,262 : 25,49 ''
    IL_01f7:  ldstr      "{0}"
    IL_01fc:  ldc.i4.1
    IL_01fd:  newarr     [sscorlib]System.Object
    IL_0202:  stloc.s    CS$0$0001
    IL_0204:  ldloc.s    CS$0$0001
    IL_0206:  ldc.i4.0
    IL_0207:  ldloc.2
    IL_0208:  box        [sscorlib]System.Int32
    IL_020d:  stelem.ref
    IL_020e:  ldloc.s    CS$0$0001
    IL_0210:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0215:  pop
    .line 263,263 : 25,34 ''
    IL_0216:  br.s       IL_0273

    .line 266,266 : 21,22 ''
    IL_0218:  nop
    .line 267,267 : 25,50 ''
    IL_0219:  ldstr      "{0}"
    IL_021e:  ldc.i4.1
    IL_021f:  newarr     [sscorlib]System.Object
    IL_0224:  stloc.s    CS$0$0001
    IL_0226:  ldloc.s    CS$0$0001
    IL_0228:  ldc.i4.0
    IL_0229:  ldc.i4.s   10
    IL_022b:  box        [sscorlib]System.Int32
    IL_0230:  stelem.ref
    IL_0231:  ldloc.s    CS$0$0001
    IL_0233:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0238:  pop
    .line 268,268 : 21,22 ''
    IL_0239:  nop
    .line 269,269 : 17,18 ''
    IL_023a:  nop
    IL_023b:  br.s       IL_0243

    .line 271,271 : 17,18 ''
    IL_023d:  nop
    .line 272,272 : 21,28 ''
    IL_023e:  ldloc.2
    IL_023f:  ldarg.1
    IL_0240:  add
    IL_0241:  stloc.2
    .line 273,273 : 17,18 ''
    IL_0242:  nop
    .line 275,275 : 17,45 ''
    IL_0243:  ldstr      "{0}"
    IL_0248:  ldc.i4.1
    IL_0249:  newarr     [sscorlib]System.Object
    IL_024e:  stloc.s    CS$0$0001
    IL_0250:  ldloc.s    CS$0$0001
    IL_0252:  ldc.i4.0
    IL_0253:  ldstr      "foo"
    IL_0258:  stelem.ref
    IL_0259:  ldloc.s    CS$0$0001
    IL_025b:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0260:  pop
    .line 277,277 : 17,38 ''
    IL_0261:  ldloc.2
    IL_0262:  ldarg.1
    IL_0263:  ldc.i4.s   10
    IL_0265:  beq.s      IL_026a

    IL_0267:  ldc.i4.2
    IL_0268:  br.s       IL_026b

    IL_026a:  ldc.i4.1
    IL_026b:  add
    IL_026c:  stloc.2
    .line 278,278 : 17,24 ''
    IL_026d:  ldarg.1
    IL_026e:  ldc.i4.1
    IL_026f:  sub
    IL_0270:  starg.s    i
    .line 279,279 : 13,14 ''
    IL_0272:  nop
    .line 256,256 : 13,31 ''
    IL_0273:  ldloc.2
    IL_0274:  ldarg.1
    IL_0275:  ldc.i4.s   10
    IL_0277:  add
    IL_0278:  clt
    IL_027a:  stloc.3
    IL_027b:  ldloc.3
    IL_027c:  brtrue     IL_01dc

    .line 281,281 : 13,44 ''
    IL_0281:  ldstr      "{0}"
    IL_0286:  ldc.i4.1
    IL_0287:  newarr     [sscorlib]System.Object
    IL_028c:  stloc.s    CS$0$0001
    IL_028e:  ldloc.s    CS$0$0001
    IL_0290:  ldc.i4.0
    IL_0291:  ldstr      "Ending"
    IL_0296:  stelem.ref
    IL_0297:  ldloc.s    CS$0$0001
    IL_0299:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_029e:  pop
    .line 282,282 : 9,10 ''
    IL_029f:  ret
  } // end of method Class1::Foo

  .method public hidebysig instance void 
          FooWhileTest(int32 i) cil managed
  {
    // Code size       222 (0xde)
    .maxstack  5
    .locals init ([0] object[] CS$0$0000,
             [1] bool CS$4$0001)
    .line 285,285 : 9,10 ''
    IL_0000:  nop
    IL_0001:  br.s       IL_0027

    .line 287,287 : 13,14 ''
    IL_0003:  nop
    .line 288,288 : 17,41 ''
    IL_0004:  ldstr      "{0}"
    IL_0009:  ldc.i4.1
    IL_000a:  newarr     [sscorlib]System.Object
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.1
    IL_0013:  box        [sscorlib]System.Int32
    IL_0018:  stelem.ref
    IL_0019:  ldloc.0
    IL_001a:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_001f:  pop
    .line 289,289 : 17,25 ''
    IL_0020:  ldarg.1
    IL_0021:  ldc.i4.s   10
    IL_0023:  sub
    IL_0024:  starg.s    i
    .line 290,290 : 13,14 ''
    IL_0026:  nop
    .line 286,286 : 13,27 ''
    IL_0027:  ldarg.1
    IL_0028:  ldc.i4.s   10
    IL_002a:  cgt
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  brtrue.s   IL_0003

    IL_0030:  br.s       IL_0058

    .line 292,292 : 13,14 ''
    IL_0032:  nop
    .line 293,293 : 17,46 ''
    IL_0033:  ldstr      "{0}"
    IL_0038:  ldc.i4.1
    IL_0039:  newarr     [sscorlib]System.Object
    IL_003e:  stloc.0
    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  ldarg.1
    IL_0042:  ldc.i4.s   10
    IL_0044:  add
    IL_0045:  box        [sscorlib]System.Int32
    IL_004a:  stelem.ref
    IL_004b:  ldloc.0
    IL_004c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0051:  pop
    .line 294,294 : 17,24 ''
    IL_0052:  ldarg.1
    IL_0053:  ldc.i4.2
    IL_0054:  sub
    IL_0055:  starg.s    i
    .line 295,295 : 13,14 ''
    IL_0057:  nop
    .line 291,291 : 13,26 ''
    IL_0058:  ldarg.1
    IL_0059:  ldc.i4.4
    IL_005a:  cgt
    IL_005c:  stloc.1
    IL_005d:  ldloc.1
    IL_005e:  brtrue.s   IL_0032

    .line 298,298 : 13,14 ''
    IL_0060:  nop
    .line 299,299 : 17,45 ''
    IL_0061:  ldstr      "{0}"
    IL_0066:  ldc.i4.1
    IL_0067:  newarr     [sscorlib]System.Object
    IL_006c:  stloc.0
    IL_006d:  ldloc.0
    IL_006e:  ldc.i4.0
    IL_006f:  ldarg.1
    IL_0070:  ldc.i4.1
    IL_0071:  sub
    IL_0072:  box        [sscorlib]System.Int32
    IL_0077:  stelem.ref
    IL_0078:  ldloc.0
    IL_0079:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_007e:  pop
    .line 300,300 : 17,28 ''
    IL_007f:  ldarg.1
    IL_0080:  ldc.i4.3
    IL_0081:  ceq
    IL_0083:  ldc.i4.0
    IL_0084:  ceq
    IL_0086:  stloc.1
    IL_0087:  ldloc.1
    IL_0088:  brtrue.s   IL_0092

    .line 301,301 : 17,18 ''
    IL_008a:  nop
    .line 302,302 : 21,28 ''
    IL_008b:  ldarg.1
    IL_008c:  ldc.i4.1
    IL_008d:  add
    IL_008e:  starg.s    i
    .line 303,303 : 21,30 ''
    IL_0090:  br.s       IL_0098

    .line 305,305 : 17,24 ''
    IL_0092:  ldarg.1
    IL_0093:  ldc.i4.2
    IL_0094:  add
    IL_0095:  starg.s    i
    .line 306,306 : 13,14 ''
    IL_0097:  nop
    .line 306,306 : 15,30 ''
    IL_0098:  ldarg.1
    IL_0099:  ldc.i4.s   10
    IL_009b:  clt
    IL_009d:  stloc.1
    IL_009e:  ldloc.1
    IL_009f:  brtrue.s   IL_0060

    .line 308,308 : 13,24 ''
    IL_00a1:  ldarg.1
    IL_00a2:  ldc.i4.2
    IL_00a3:  ceq
    IL_00a5:  ldc.i4.0
    IL_00a6:  ceq
    IL_00a8:  stloc.1
    IL_00a9:  ldloc.1
    IL_00aa:  brtrue.s   IL_00dd

    .line 309,309 : 13,14 ''
    IL_00ac:  nop
    IL_00ad:  br.s       IL_00d3

    .line 311,311 : 17,18 ''
    IL_00af:  nop
    .line 312,312 : 21,45 ''
    IL_00b0:  ldstr      "{0}"
    IL_00b5:  ldc.i4.1
    IL_00b6:  newarr     [sscorlib]System.Object
    IL_00bb:  stloc.0
    IL_00bc:  ldloc.0
    IL_00bd:  ldc.i4.0
    IL_00be:  ldarg.1
    IL_00bf:  box        [sscorlib]System.Int32
    IL_00c4:  stelem.ref
    IL_00c5:  ldloc.0
    IL_00c6:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00cb:  pop
    .line 313,313 : 21,29 ''
    IL_00cc:  ldarg.1
    IL_00cd:  ldc.i4.s   10
    IL_00cf:  add
    IL_00d0:  starg.s    i
    .line 314,314 : 17,18 ''
    IL_00d2:  nop
    .line 310,310 : 17,31 ''
    IL_00d3:  ldarg.1
    IL_00d4:  ldc.i4.s   40
    IL_00d6:  clt
    IL_00d8:  stloc.1
    IL_00d9:  ldloc.1
    IL_00da:  brtrue.s   IL_00af

    .line 315,315 : 13,14 ''
    IL_00dc:  nop
    .line 316,316 : 9,10 ''
    IL_00dd:  ret
  } // end of method Class1::FooWhileTest

  .method public hidebysig instance void 
          SimpleWhileLoop(int32 i) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  4
    .locals init ([0] object[] CS$0$0000,
             [1] bool CS$4$0001)
    .line 319,319 : 9,10 ''
    IL_0000:  nop
    .line 320,320 : 13,34 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    IL_0012:  br.s       IL_0037

    .line 323,323 : 13,14 ''
    IL_0014:  nop
    .line 324,324 : 17,41 ''
    IL_0015:  ldstr      "{0}"
    IL_001a:  ldc.i4.1
    IL_001b:  newarr     [sscorlib]System.Object
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldarg.1
    IL_0024:  box        [sscorlib]System.Int32
    IL_0029:  stelem.ref
    IL_002a:  ldloc.0
    IL_002b:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0030:  pop
    .line 326,326 : 17,21 ''
    IL_0031:  ldarg.1
    IL_0032:  ldc.i4.1
    IL_0033:  sub
    IL_0034:  starg.s    i
    .line 327,327 : 13,14 ''
    IL_0036:  nop
    .line 322,322 : 13,26 ''
    IL_0037:  ldarg.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  cgt
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  brtrue.s   IL_0014

    .line 329,329 : 13,34 ''
    IL_0040:  ldstr      "tmp"
    IL_0045:  ldc.i4.0
    IL_0046:  newarr     [sscorlib]System.Object
    IL_004b:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0050:  pop
    .line 330,330 : 9,10 ''
    IL_0051:  ret
  } // end of method Class1::SimpleWhileLoop

  .method public hidebysig instance void 
          SimpleDoWhileLoop(int32 i) cil managed
  {
    // Code size       80 (0x50)
    .maxstack  4
    .locals init ([0] object[] CS$0$0000,
             [1] bool CS$4$0001)
    .line 333,333 : 9,10 ''
    IL_0000:  nop
    .line 334,334 : 13,34 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    .line 337,337 : 13,14 ''
    IL_0012:  nop
    .line 338,338 : 17,41 ''
    IL_0013:  ldstr      "{0}"
    IL_0018:  ldc.i4.1
    IL_0019:  newarr     [sscorlib]System.Object
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.0
    IL_0021:  ldarg.1
    IL_0022:  box        [sscorlib]System.Int32
    IL_0027:  stelem.ref
    IL_0028:  ldloc.0
    IL_0029:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_002e:  pop
    .line 340,340 : 17,21 ''
    IL_002f:  ldarg.1
    IL_0030:  ldc.i4.1
    IL_0031:  sub
    IL_0032:  starg.s    i
    .line 341,341 : 13,14 ''
    IL_0034:  nop
    .line 341,341 : 15,30 ''
    IL_0035:  ldarg.1
    IL_0036:  ldc.i4.s   10
    IL_0038:  cgt
    IL_003a:  stloc.1
    IL_003b:  ldloc.1
    IL_003c:  brtrue.s   IL_0012

    .line 343,343 : 13,34 ''
    IL_003e:  ldstr      "tmp"
    IL_0043:  ldc.i4.0
    IL_0044:  newarr     [sscorlib]System.Object
    IL_0049:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_004e:  pop
    .line 344,344 : 9,10 ''
    IL_004f:  ret
  } // end of method Class1::SimpleDoWhileLoop

  .method public hidebysig instance void 
          SimpleForLoop(int32 i) cil managed
  {
    // Code size       88 (0x58)
    .maxstack  4
    .locals init ([0] int32 j,
             [1] object[] CS$0$0000,
             [2] bool CS$4$0001)
    .line 347,347 : 9,10 ''
    IL_0000:  nop
    .line 348,348 : 13,34 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    .line 350,350 : 18,28 ''
    IL_0012:  ldarg.1
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_003d

    .line 351,351 : 13,14 ''
    IL_0016:  nop
    .line 352,352 : 17,41 ''
    IL_0017:  ldstr      "{0}"
    IL_001c:  ldc.i4.1
    IL_001d:  newarr     [sscorlib]System.Object
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldarg.1
    IL_0026:  box        [sscorlib]System.Int32
    IL_002b:  stelem.ref
    IL_002c:  ldloc.1
    IL_002d:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0032:  pop
    .line 354,354 : 17,21 ''
    IL_0033:  ldarg.1
    IL_0034:  ldc.i4.1
    IL_0035:  sub
    IL_0036:  starg.s    i
    .line 355,355 : 13,14 ''
    IL_0038:  nop
    .line 350,350 : 37,40 ''
    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.1
    IL_003b:  sub
    IL_003c:  stloc.0
    .line 350,350 : 29,35 ''
    IL_003d:  ldloc.0
    IL_003e:  ldc.i4.s   10
    IL_0040:  cgt
    IL_0042:  stloc.2
    IL_0043:  ldloc.2
    IL_0044:  brtrue.s   IL_0016

    .line 357,357 : 13,34 ''
    IL_0046:  ldstr      "tmp"
    IL_004b:  ldc.i4.0
    IL_004c:  newarr     [sscorlib]System.Object
    IL_0051:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0056:  pop
    .line 358,358 : 9,10 ''
    IL_0057:  ret
  } // end of method Class1::SimpleForLoop

  .method public hidebysig instance void 
          NestedWhileDoWhile(int32 i) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  2
    .locals init ([0] bool CS$4$0000)
    .line 361,361 : 9,10 ''
    IL_0000:  nop
    .line 362,362 : 13,34 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    IL_0012:  br.s       IL_002c

    .line 365,365 : 13,14 ''
    IL_0014:  nop
    .line 367,367 : 17,18 ''
    IL_0015:  nop
    .line 368,368 : 21,25 ''
    IL_0016:  ldarg.1
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  starg.s    i
    .line 369,369 : 17,18 ''
    IL_001b:  nop
    .line 369,369 : 19,36 ''
    IL_001c:  ldarg.1
    IL_001d:  ldc.i4.2
    IL_001e:  rem
    IL_001f:  ldc.i4.0
    IL_0020:  ceq
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  brtrue.s   IL_0015

    .line 371,371 : 17,21 ''
    IL_0026:  ldarg.1
    IL_0027:  ldc.i4.1
    IL_0028:  sub
    IL_0029:  starg.s    i
    .line 372,372 : 13,14 ''
    IL_002b:  nop
    .line 364,364 : 13,27 ''
    IL_002c:  ldarg.1
    IL_002d:  ldc.i4.s   10
    IL_002f:  cgt
    IL_0031:  stloc.0
    IL_0032:  ldloc.0
    IL_0033:  brtrue.s   IL_0014

    .line 374,374 : 13,34 ''
    IL_0035:  ldstr      "tmp"
    IL_003a:  ldc.i4.0
    IL_003b:  newarr     [sscorlib]System.Object
    IL_0040:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0045:  pop
    .line 375,375 : 9,10 ''
    IL_0046:  ret
  } // end of method Class1::NestedWhileDoWhile

  .method public hidebysig instance void 
          LastWhileBlock(int32 i) cil managed
  {
    // Code size       65 (0x41)
    .maxstack  4
    .locals init ([0] object[] CS$0$0000,
             [1] bool CS$4$0001)
    .line 378,378 : 9,10 ''
    IL_0000:  nop
    .line 379,379 : 13,34 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    IL_0012:  br.s       IL_0037

    .line 382,382 : 13,14 ''
    IL_0014:  nop
    .line 383,383 : 17,41 ''
    IL_0015:  ldstr      "{0}"
    IL_001a:  ldc.i4.1
    IL_001b:  newarr     [sscorlib]System.Object
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldarg.1
    IL_0024:  box        [sscorlib]System.Int32
    IL_0029:  stelem.ref
    IL_002a:  ldloc.0
    IL_002b:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0030:  pop
    .line 385,385 : 17,21 ''
    IL_0031:  ldarg.1
    IL_0032:  ldc.i4.1
    IL_0033:  sub
    IL_0034:  starg.s    i
    .line 386,386 : 13,14 ''
    IL_0036:  nop
    .line 381,381 : 13,27 ''
    IL_0037:  ldarg.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  cgt
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  brtrue.s   IL_0014

    .line 387,387 : 9,10 ''
    IL_0040:  ret
  } // end of method Class1::LastWhileBlock

  .method public hidebysig instance int32 
          WhileLoopBreak(int32 i,
                         int32 j) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000,
             [1] bool CS$4$0001)
    .line 390,390 : 9,10 ''
    IL_0000:  nop
    .line 391,391 : 13,20 ''
    IL_0001:  ldarg.2
    IL_0002:  ldarg.1
    IL_0003:  add
    IL_0004:  starg.s    j
    .line 394,394 : 13,14 ''
    IL_0006:  nop
    .line 395,395 : 17,24 ''
    IL_0007:  ldarg.1
    IL_0008:  ldarg.2
    IL_0009:  add
    IL_000a:  starg.s    i
    .line 397,397 : 17,33 ''
    IL_000c:  ldarg.1
    IL_000d:  ldc.i4.s   10
    IL_000f:  rem
    IL_0010:  ldc.i4.0
    IL_0011:  ceq
    IL_0013:  ldc.i4.0
    IL_0014:  ceq
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  brtrue.s   IL_0030

    .line 398,398 : 17,18 ''
    IL_001a:  nop
    .line 399,399 : 21,25 ''
    IL_001b:  ldarg.2
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  starg.s    j
    .line 401,401 : 21,33 ''
    IL_0020:  ldarg.2
    IL_0021:  ldc.i4.s   10
    IL_0023:  ceq
    IL_0025:  ldc.i4.0
    IL_0026:  ceq
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  brtrue.s   IL_002f

    .line 402,402 : 21,22 ''
    IL_002c:  nop
    .line 403,403 : 25,31 ''
    IL_002d:  br.s       IL_003a

    .line 405,405 : 17,18 ''
    IL_002f:  nop
    .line 407,407 : 13,14 ''
    IL_0030:  nop
    .line 407,407 : 15,31 ''
    IL_0031:  ldarg.1
    IL_0032:  ldc.i4.s   100
    IL_0034:  clt
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  brtrue.s   IL_0006

    .line 409,409 : 13,22 ''
    IL_003a:  ldarg.2
    IL_003b:  stloc.0
    IL_003c:  br.s       IL_003e

    .line 410,410 : 9,10 ''
    IL_003e:  ldloc.0
    IL_003f:  ret
  } // end of method Class1::WhileLoopBreak

  .method public hidebysig instance int32 
          WhileLoopContinue(int32 i,
                            int32 j) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000,
             [1] bool CS$4$0001)
    .line 413,413 : 9,10 ''
    IL_0000:  nop
    .line 414,414 : 13,20 ''
    IL_0001:  ldarg.2
    IL_0002:  ldarg.1
    IL_0003:  add
    IL_0004:  starg.s    j
    .line 417,417 : 13,14 ''
    IL_0006:  nop
    .line 418,418 : 17,24 ''
    IL_0007:  ldarg.1
    IL_0008:  ldarg.2
    IL_0009:  add
    IL_000a:  starg.s    i
    .line 420,420 : 17,33 ''
    IL_000c:  ldarg.1
    IL_000d:  ldc.i4.s   10
    IL_000f:  rem
    IL_0010:  ldc.i4.0
    IL_0011:  ceq
    IL_0013:  ldc.i4.0
    IL_0014:  ceq
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  brtrue.s   IL_0035

    .line 421,421 : 17,18 ''
    IL_001a:  nop
    .line 422,422 : 21,25 ''
    IL_001b:  ldarg.2
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  starg.s    j
    .line 424,424 : 21,33 ''
    IL_0020:  ldarg.2
    IL_0021:  ldc.i4.s   10
    IL_0023:  ceq
    IL_0025:  ldc.i4.0
    IL_0026:  ceq
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  brtrue.s   IL_002f

    .line 425,425 : 21,22 ''
    IL_002c:  nop
    .line 426,426 : 25,34 ''
    IL_002d:  br.s       IL_003b

    .line 429,429 : 21,25 ''
    IL_002f:  ldarg.1
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  starg.s    i
    .line 430,430 : 17,18 ''
    IL_0034:  nop
    .line 432,432 : 17,21 ''
    IL_0035:  ldarg.2
    IL_0036:  ldc.i4.1
    IL_0037:  sub
    IL_0038:  starg.s    j
    .line 434,434 : 13,14 ''
    IL_003a:  nop
    .line 434,434 : 15,31 ''
    IL_003b:  ldarg.1
    IL_003c:  ldc.i4.s   100
    IL_003e:  clt
    IL_0040:  stloc.1
    IL_0041:  ldloc.1
    IL_0042:  brtrue.s   IL_0006

    .line 436,436 : 13,22 ''
    IL_0044:  ldarg.2
    IL_0045:  stloc.0
    IL_0046:  br.s       IL_0048

    .line 437,437 : 9,10 ''
    IL_0048:  ldloc.0
    IL_0049:  ret
  } // end of method Class1::WhileLoopContinue

  .method public hidebysig instance int32 
          ForLoopBreak(int32 i,
                       int32 j) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000,
             [1] bool CS$4$0001)
    .line 440,440 : 9,10 ''
    IL_0000:  nop
    .line 441,441 : 13,20 ''
    IL_0001:  ldarg.2
    IL_0002:  ldarg.1
    IL_0003:  add
    IL_0004:  starg.s    j
    IL_0006:  br.s       IL_0038

    .line 444,444 : 13,14 ''
    IL_0008:  nop
    .line 445,445 : 17,24 ''
    IL_0009:  ldarg.1
    IL_000a:  ldarg.2
    IL_000b:  add
    IL_000c:  starg.s    i
    .line 447,447 : 17,33 ''
    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.s   10
    IL_0011:  rem
    IL_0012:  ldc.i4.0
    IL_0013:  ceq
    IL_0015:  ldc.i4.0
    IL_0016:  ceq
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  brtrue.s   IL_0032

    .line 448,448 : 17,18 ''
    IL_001c:  nop
    .line 449,449 : 21,25 ''
    IL_001d:  ldarg.2
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  starg.s    j
    .line 451,451 : 21,33 ''
    IL_0022:  ldarg.2
    IL_0023:  ldc.i4.s   10
    IL_0025:  ceq
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  brtrue.s   IL_0031

    .line 452,452 : 21,22 ''
    IL_002e:  nop
    .line 453,453 : 25,31 ''
    IL_002f:  br.s       IL_0041

    .line 455,455 : 17,18 ''
    IL_0031:  nop
    .line 456,456 : 13,14 ''
    IL_0032:  nop
    .line 443,443 : 27,30 ''
    IL_0033:  ldarg.1
    IL_0034:  ldc.i4.1
    IL_0035:  add
    IL_0036:  starg.s    i
    .line 443,443 : 18,25 ''
    IL_0038:  ldarg.1
    IL_0039:  ldc.i4.s   100
    IL_003b:  clt
    IL_003d:  stloc.1
    IL_003e:  ldloc.1
    IL_003f:  brtrue.s   IL_0008

    .line 458,458 : 13,22 ''
    IL_0041:  ldarg.2
    IL_0042:  stloc.0
    IL_0043:  br.s       IL_0045

    .line 459,459 : 9,10 ''
    IL_0045:  ldloc.0
    IL_0046:  ret
  } // end of method Class1::ForLoopBreak

  .method public hidebysig instance int32 
          ForLoopContinue(int32 i,
                          int32 j) cil managed
  {
    // Code size       81 (0x51)
    .maxstack  2
    .locals init ([0] int32 CS$1$0000,
             [1] bool CS$4$0001)
    .line 462,462 : 9,10 ''
    IL_0000:  nop
    .line 463,463 : 13,20 ''
    IL_0001:  ldarg.2
    IL_0002:  ldarg.1
    IL_0003:  add
    IL_0004:  starg.s    j
    IL_0006:  br.s       IL_0042

    .line 466,466 : 13,14 ''
    IL_0008:  nop
    .line 467,467 : 17,24 ''
    IL_0009:  ldarg.1
    IL_000a:  ldarg.2
    IL_000b:  add
    IL_000c:  starg.s    i
    .line 469,469 : 17,33 ''
    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.s   10
    IL_0011:  rem
    IL_0012:  ldc.i4.0
    IL_0013:  ceq
    IL_0015:  ldc.i4.0
    IL_0016:  ceq
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  brtrue.s   IL_0037

    .line 470,470 : 17,18 ''
    IL_001c:  nop
    .line 471,471 : 21,25 ''
    IL_001d:  ldarg.2
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  starg.s    j
    .line 473,473 : 21,33 ''
    IL_0022:  ldarg.2
    IL_0023:  ldc.i4.s   10
    IL_0025:  ceq
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  brtrue.s   IL_0031

    .line 474,474 : 21,22 ''
    IL_002e:  nop
    .line 475,475 : 25,34 ''
    IL_002f:  br.s       IL_003d

    .line 478,478 : 21,25 ''
    IL_0031:  ldarg.1
    IL_0032:  ldc.i4.1
    IL_0033:  add
    IL_0034:  starg.s    i
    .line 479,479 : 17,18 ''
    IL_0036:  nop
    .line 481,481 : 17,21 ''
    IL_0037:  ldarg.2
    IL_0038:  ldc.i4.1
    IL_0039:  sub
    IL_003a:  starg.s    j
    .line 482,482 : 13,14 ''
    IL_003c:  nop
    .line 465,465 : 27,30 ''
    IL_003d:  ldarg.1
    IL_003e:  ldc.i4.1
    IL_003f:  add
    IL_0040:  starg.s    i
    .line 465,465 : 18,25 ''
    IL_0042:  ldarg.1
    IL_0043:  ldc.i4.s   100
    IL_0045:  clt
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  brtrue.s   IL_0008

    .line 484,484 : 13,22 ''
    IL_004b:  ldarg.2
    IL_004c:  stloc.0
    IL_004d:  br.s       IL_004f

    .line 485,485 : 9,10 ''
    IL_004f:  ldloc.0
    IL_0050:  ret
  } // end of method Class1::ForLoopContinue

  .method public hidebysig instance void 
          SimpleIfTest(int32 i) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  2
    .locals init ([0] string returnValue,
             [1] bool CS$4$0000)
    .line 488,488 : 9,10 ''
    IL_0000:  nop
    .line 489,489 : 13,40 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  stloc.0
    .line 491,491 : 13,25 ''
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.s   10
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  brtrue.s   IL_001b

    .line 492,492 : 13,14 ''
    IL_0013:  nop
    .line 493,493 : 17,45 ''
    IL_0014:  ldstr      "tmpFoo"
    IL_0019:  stloc.0
    .line 494,494 : 13,14 ''
    IL_001a:  nop
    .line 496,496 : 13,47 ''
    IL_001b:  ldloc.0
    IL_001c:  ldstr      "bar"
    IL_0021:  call       string [sscorlib]System.String::Concat(string,
                                                                string)
    IL_0026:  stloc.0
    .line 498,498 : 13,37 ''
    IL_0027:  ldarg.0
    IL_0028:  ldloc.0
    IL_0029:  callvirt   instance int32 [sscorlib]System.String::get_Length()
    IL_002e:  call       instance void RealScript.Class1::Foo(int32)
    IL_0033:  nop
    .line 499,499 : 9,10 ''
    IL_0034:  ret
  } // end of method Class1::SimpleIfTest

  .method public hidebysig instance void 
          SimpleIfElseTest(int32 i) cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init ([0] string returnValue,
             [1] bool CS$4$0000)
    .line 502,502 : 9,10 ''
    IL_0000:  nop
    .line 503,503 : 13,40 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  stloc.0
    .line 505,505 : 13,25 ''
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.s   10
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  brtrue.s   IL_001d

    .line 506,506 : 13,14 ''
    IL_0013:  nop
    .line 507,507 : 17,45 ''
    IL_0014:  ldstr      "tmpFoo"
    IL_0019:  stloc.0
    .line 508,508 : 13,14 ''
    IL_001a:  nop
    IL_001b:  br.s       IL_0025

    .line 510,510 : 13,14 ''
    IL_001d:  nop
    .line 511,511 : 17,45 ''
    IL_001e:  ldstr      "tmpbar"
    IL_0023:  stloc.0
    .line 512,512 : 13,14 ''
    IL_0024:  nop
    .line 514,514 : 13,47 ''
    IL_0025:  ldloc.0
    IL_0026:  ldstr      "bar"
    IL_002b:  call       string [sscorlib]System.String::Concat(string,
                                                                string)
    IL_0030:  stloc.0
    .line 516,516 : 13,37 ''
    IL_0031:  ldarg.0
    IL_0032:  ldloc.0
    IL_0033:  callvirt   instance int32 [sscorlib]System.String::get_Length()
    IL_0038:  call       instance void RealScript.Class1::Foo(int32)
    IL_003d:  nop
    .line 517,517 : 9,10 ''
    IL_003e:  ret
  } // end of method Class1::SimpleIfElseTest

  .method public hidebysig instance int32 
          SimpleIfElseIfTest(int32 i) cil managed
  {
    // Code size       76 (0x4c)
    .maxstack  2
    .locals init ([0] string returnValue,
             [1] int32 CS$1$0000,
             [2] bool CS$4$0001)
    .line 520,520 : 9,10 ''
    IL_0000:  nop
    .line 521,521 : 13,40 ''
    IL_0001:  ldstr      "tmp"
    IL_0006:  stloc.0
    .line 523,523 : 13,25 ''
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.s   10
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  brtrue.s   IL_001d

    .line 524,524 : 13,14 ''
    IL_0013:  nop
    .line 525,525 : 17,45 ''
    IL_0014:  ldstr      "tmpFoo"
    IL_0019:  stloc.0
    .line 526,526 : 13,14 ''
    IL_001a:  nop
    IL_001b:  br.s       IL_0041

    .line 527,527 : 18,30 ''
    IL_001d:  ldarg.1
    IL_001e:  ldc.i4.s   20
    IL_0020:  ceq
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  stloc.2
    IL_0026:  ldloc.2
    IL_0027:  brtrue.s   IL_0033

    .line 528,528 : 13,14 ''
    IL_0029:  nop
    .line 529,529 : 17,45 ''
    IL_002a:  ldstr      "tmpbar"
    IL_002f:  stloc.0
    .line 530,530 : 13,14 ''
    IL_0030:  nop
    IL_0031:  br.s       IL_0041

    .line 532,532 : 13,14 ''
    IL_0033:  nop
    .line 533,533 : 17,51 ''
    IL_0034:  ldloc.0
    IL_0035:  ldstr      "bar"
    IL_003a:  call       string [sscorlib]System.String::Concat(string,
                                                                string)
    IL_003f:  stloc.0
    .line 534,534 : 13,14 ''
    IL_0040:  nop
    .line 536,536 : 13,39 ''
    IL_0041:  ldloc.0
    IL_0042:  callvirt   instance int32 [sscorlib]System.String::get_Length()
    IL_0047:  stloc.1
    IL_0048:  br.s       IL_004a

    .line 537,537 : 9,10 ''
    IL_004a:  ldloc.1
    IL_004b:  ret
  } // end of method Class1::SimpleIfElseIfTest

  .method public hidebysig instance int32 
          NestedIfElseTest(int32 i) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init ([0] int32 returnValue,
             [1] int32 CS$1$0000,
             [2] bool CS$4$0001)
    .line 540,540 : 9,10 ''
    IL_0000:  nop
    .line 541,541 : 13,33 ''
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 543,543 : 13,24 ''
    IL_0003:  ldarg.1
    IL_0004:  ldc.i4.s   10
    IL_0006:  cgt
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.2
    IL_000c:  ldloc.2
    IL_000d:  brtrue.s   IL_0032

    .line 544,544 : 13,14 ''
    IL_000f:  nop
    .line 545,545 : 17,35 ''
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.s   10
    IL_0013:  add
    IL_0014:  stloc.0
    .line 547,547 : 17,29 ''
    IL_0015:  ldarg.1
    IL_0016:  ldc.i4.s   15
    IL_0018:  ceq
    IL_001a:  ldc.i4.0
    IL_001b:  ceq
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  brtrue.s   IL_0029

    .line 548,548 : 17,18 ''
    IL_0021:  nop
    .line 549,549 : 21,38 ''
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.5
    IL_0024:  add
    IL_0025:  stloc.0
    .line 550,550 : 17,18 ''
    IL_0026:  nop
    IL_0027:  br.s       IL_002f

    .line 552,552 : 17,18 ''
    IL_0029:  nop
    .line 553,553 : 21,38 ''
    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.2
    IL_002c:  add
    IL_002d:  stloc.0
    .line 554,554 : 17,18 ''
    IL_002e:  nop
    .line 555,555 : 13,14 ''
    IL_002f:  nop
    IL_0030:  br.s       IL_0037

    .line 557,557 : 13,14 ''
    IL_0032:  nop
    .line 558,558 : 17,34 ''
    IL_0033:  ldc.i4.s   11
    IL_0035:  stloc.0
    .line 559,559 : 13,14 ''
    IL_0036:  nop
    .line 561,561 : 13,32 ''
    IL_0037:  ldloc.0
    IL_0038:  stloc.1
    IL_0039:  br.s       IL_003b

    .line 562,562 : 9,10 ''
    IL_003b:  ldloc.1
    IL_003c:  ret
  } // end of method Class1::NestedIfElseTest

  .method public hidebysig instance void 
          FooSwitch(int32 i) cil managed
  {
    // Code size       266 (0x10a)
    .maxstack  4
    .locals init ([0] int32 CS$4$0000,
             [1] object[] CS$0$0001)
    .line 565,565 : 9,10 ''
    IL_0000:  nop
    .line 566,566 : 13,33 ''
    IL_0001:  ldstr      "{}"
    IL_0006:  ldc.i4.0
    IL_0007:  newarr     [sscorlib]System.Object
    IL_000c:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0011:  pop
    .line 568,568 : 13,23 ''
    IL_0012:  ldarg.1
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.s   10
    IL_0017:  bgt.s      IL_0027

    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  beq.s      IL_007c

    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.s   10
    IL_0020:  beq.s      IL_0069

    IL_0022:  br         IL_00db

    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.s   16
    IL_002a:  beq.s      IL_007c

    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.s   32
    IL_002f:  beq.s      IL_007c

    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.s   100
    IL_0034:  sub
    IL_0035:  switch     ( 
                          IL_0053,
                          IL_008f,
                          IL_00b5,
                          IL_00c8,
                          IL_00a2)
    IL_004e:  br         IL_00db

    .line 571,571 : 21,49 ''
    IL_0053:  ldstr      "OneHundred"
    IL_0058:  ldc.i4.0
    IL_0059:  newarr     [sscorlib]System.Object
    IL_005e:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0063:  pop
    .line 572,572 : 21,27 ''
    IL_0064:  br         IL_00ee

    .line 574,574 : 21,42 ''
    IL_0069:  ldstr      "Ten"
    IL_006e:  ldc.i4.0
    IL_006f:  newarr     [sscorlib]System.Object
    IL_0074:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0079:  pop
    .line 575,575 : 21,27 ''
    IL_007a:  br.s       IL_00ee

    .line 579,579 : 21,51 ''
    IL_007c:  ldstr      "Power Of Two"
    IL_0081:  ldc.i4.0
    IL_0082:  newarr     [sscorlib]System.Object
    IL_0087:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_008c:  pop
    .line 580,580 : 21,27 ''
    IL_008d:  br.s       IL_00ee

    .line 582,582 : 21,53 ''
    IL_008f:  ldstr      "Contigous Test"
    IL_0094:  ldc.i4.0
    IL_0095:  newarr     [sscorlib]System.Object
    IL_009a:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_009f:  pop
    .line 583,583 : 21,27 ''
    IL_00a0:  br.s       IL_00ee

    .line 585,585 : 21,55 ''
    IL_00a2:  ldstr      "Contigous Test 2"
    IL_00a7:  ldc.i4.0
    IL_00a8:  newarr     [sscorlib]System.Object
    IL_00ad:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00b2:  pop
    .line 586,586 : 21,27 ''
    IL_00b3:  br.s       IL_00ee

    .line 588,588 : 21,55 ''
    IL_00b5:  ldstr      "Contigous Test 3"
    IL_00ba:  ldc.i4.0
    IL_00bb:  newarr     [sscorlib]System.Object
    IL_00c0:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00c5:  pop
    .line 589,589 : 21,27 ''
    IL_00c6:  br.s       IL_00ee

    .line 591,591 : 21,55 ''
    IL_00c8:  ldstr      "Contigous Test 4"
    IL_00cd:  ldc.i4.0
    IL_00ce:  newarr     [sscorlib]System.Object
    IL_00d3:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00d8:  pop
    .line 592,592 : 21,27 ''
    IL_00d9:  br.s       IL_00ee

    .line 594,594 : 21,46 ''
    IL_00db:  ldstr      "Default"
    IL_00e0:  ldc.i4.0
    IL_00e1:  newarr     [sscorlib]System.Object
    IL_00e6:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_00eb:  pop
    .line 595,595 : 21,27 ''
    IL_00ec:  br.s       IL_00ee

    .line 672,672 : 13,63 ''
    IL_00ee:  ldstr      "{0}"
    IL_00f3:  ldc.i4.1
    IL_00f4:  newarr     [sscorlib]System.Object
    IL_00f9:  stloc.1
    IL_00fa:  ldloc.1
    IL_00fb:  ldc.i4.0
    IL_00fc:  ldstr      "done with NoDefault stuff"
    IL_0101:  stelem.ref
    IL_0102:  ldloc.1
    IL_0103:  call       string [sscorlib]System.String::Format(string,
                                                                object[])
    IL_0108:  pop
    .line 689,689 : 9,10 ''
    IL_0109:  ret
  } // end of method Class1::FooSwitch

  .method public hidebysig newslot virtual final 
          instance void  Dispose() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 694,694 : 9,10 ''
    IL_0000:  nop
    .line 695,695 : 9,10 ''
    IL_0001:  ret
  } // end of method Class1::Dispose

  .method public hidebysig newslot virtual final 
          instance void  Foo() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    .line 702,702 : 9,10 ''
    IL_0000:  nop
    .line 703,703 : 9,10 ''
    IL_0001:  ret
  } // end of method Class1::Foo

  .method public hidebysig newslot virtual final 
          instance int32  Bar(int32 foo) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init ([0] int32 CS$1$0000)
    .line 706,706 : 9,10 ''
    IL_0000:  nop
    .line 707,707 : 13,24 ''
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    .line 708,708 : 9,10 ''
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method Class1::Bar

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    .line 59,59 : 9,37 ''
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   10
    IL_0003:  stfld      int32 RealScript.Class1::instanceFoo
    IL_0008:  ldarg.0
    IL_0009:  call       instance void [sscorlib]System.Object::.ctor()
    IL_000e:  nop
    IL_000f:  ret
  } // end of method Class1::.ctor

  .property instance int32 Item(int32,
                                int32)
  {
    .get instance int32 RealScript.Class1::get_Item(int32,
                                                    int32)
    .set instance void RealScript.Class1::set_Item(int32,
                                                   int32,
                                                   int32)
  } // end of property Class1::Item
} // end of class RealScript.Class1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file F:\Projects\work\Cs2JsC\RealScript\RealScript.res
