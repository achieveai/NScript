
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Test
{
  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 29 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B   // ..).NETFramework
                                                                                                        2C 56 65 72 73 69 6F 6E 3D 76 34 2E 30 2C 50 72   // ,Version=v4.0,Pr
                                                                                                        6F 66 69 6C 65 3D 43 6C 69 65 6E 74 01 00 54 0E   // ofile=Client..T.
                                                                                                        14 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61   // .FrameworkDispla
                                                                                                        79 4E 61 6D 65 1F 2E 4E 45 54 20 46 72 61 6D 65   // yName..NET Frame
                                                                                                        77 6F 72 6B 20 34 20 43 6C 69 65 6E 74 20 50 72   // work 4 Client Pr
                                                                                                        6F 66 69 6C 65 )                                  // ofile
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 04 54 65 73 74 00 00 )                      // ...Test..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 09 4D 69 63 72 6F 73 6F 66 74 00 00 )       // ...Microsoft..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 04 54 65 73 74 00 00 )                      // ...Test..
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 1B 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // ...Copyright .. 
                                                                                                  4D 69 63 72 6F 73 6F 66 74 20 32 30 31 30 00 00 ) // Microsoft 2010..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 35 61 36 31 38 30 33 64 2D 65 33 39 61   // ..$5a61803d-e39a
                                                                                                  2D 34 64 62 31 2D 39 61 62 61 2D 39 37 33 31 62   // -4db1-9aba-9731b
                                                                                                  30 63 30 37 63 37 64 00 00 )                      // 0c07c7d..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module Test.exe
// MVID: {BAD80474-B67B-4643-83FF-F0F8D4BD7B3A}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x006A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit Test.NullableTests
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Main() cil managed
  {
    .entrypoint
    // Code size       26 (0x1a)
    .maxstack  8
    .language '{3F5162F8-07C6-11D3-9053-00C04FA302A1}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
// Source File 'c:\Users\Gautam\Documents\Visual Studio 2012\Projects\cs2jsc\Test\Class1.cs' not found
    .line 8,8 : 9,10 'c:\\Users\\Gautam\\Documents\\Visual Studio 2012\\Projects\\cs2jsc\\Test\\Class1.cs'
    IL_0000:  nop
    .line 9,9 : 13,41 ''
    IL_0001:  call       void Test.NullableTests::TestNonNull()
    IL_0006:  nop
    .line 10,10 : 13,38 ''
    IL_0007:  call       void Test.NullableTests::TestNull()
    IL_000c:  nop
    .line 11,11 : 13,52 ''
    IL_000d:  call       void Test.NullableTests::TestLiftingNonNullArgs()
    IL_0012:  nop
    .line 12,12 : 13,49 ''
    IL_0013:  call       void Test.NullableTests::TestLiftingNullArgs()
    IL_0018:  nop
    .line 13,13 : 9,10 ''
    IL_0019:  ret
  } // end of method NullableTests::Main

  .method private hidebysig static void  TestNonNull() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init ([0] valuetype [mscorlib]System.Nullable`1<int32> n)
    .line 16,16 : 9,10 ''
    IL_0000:  nop
    .line 17,17 : 13,51 ''
    IL_0001:  ldstr      "Testing non-null"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    .line 18,18 : 13,30 ''
    IL_000c:  ldloca.s   n
    IL_000e:  ldc.i4.s   42
    IL_0010:  call       instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0015:  nop
    .line 19,19 : 13,28 ''
    IL_0016:  ldloc.0
    IL_0017:  call       void Test.NullableTests::LogNullable<int32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_001c:  nop
    .line 20,20 : 9,10 ''
    IL_001d:  ret
  } // end of method NullableTests::TestNonNull

  .method private hidebysig static void  TestNull() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  1
    .locals init ([0] valuetype [mscorlib]System.Nullable`1<int32> n)
    .line 23,23 : 9,10 ''
    IL_0000:  nop
    .line 24,24 : 13,47 ''
    IL_0001:  ldstr      "Testing null"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    .line 25,25 : 13,35 ''
    IL_000c:  ldloca.s   n
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    .line 26,26 : 13,28 ''
    IL_0014:  ldloc.0
    IL_0015:  call       void Test.NullableTests::LogNullable<int32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_001a:  nop
    .line 27,27 : 9,10 ''
    IL_001b:  ret
  } // end of method NullableTests::TestNull

  .method private hidebysig static void  TestLiftingNonNullArgs() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  2
    .locals init ([0] valuetype [mscorlib]System.Nullable`1<int32> n1,
             [1] valuetype [mscorlib]System.Nullable`1<int32> n2,
             [2] valuetype [mscorlib]System.Nullable`1<int32> CS$0$0000,
             [3] valuetype [mscorlib]System.Nullable`1<int32> CS$0$0001,
             [4] valuetype [mscorlib]System.Nullable`1<int32> CS$0$0002)
    .line 30,30 : 9,10 ''
    IL_0000:  nop
    .line 31,31 : 13,69 ''
    IL_0001:  ldstr      "Testing lifting with non-null args"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    .line 32,32 : 13,30 ''
    IL_000c:  ldloca.s   n1
    IL_000e:  ldc.i4.1
    IL_000f:  call       instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0014:  nop
    .line 33,33 : 13,30 ''
    IL_0015:  ldloca.s   n2
    IL_0017:  ldc.i4.2
    IL_0018:  call       instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001d:  nop
    .line 34,34 : 13,34 ''
    IL_001e:  ldloc.0
    IL_001f:  stloc.2
    IL_0020:  ldloc.1
    IL_0021:  stloc.3
    IL_0022:  ldloca.s   CS$0$0000
    IL_0024:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0029:  ldloca.s   CS$0$0001
    IL_002b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0030:  and
    IL_0031:  brtrue.s   IL_003f

    IL_0033:  ldloca.s   CS$0$0002
    IL_0035:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_003b:  ldloc.s    CS$0$0002
    IL_003d:  br.s       IL_0053

    IL_003f:  ldloca.s   CS$0$0000
    IL_0041:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0046:  ldloca.s   CS$0$0001
    IL_0048:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_004d:  add
    IL_004e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0053:  nop
    IL_0054:  call       void Test.NullableTests::LogNullable<int32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0059:  nop
    .line 35,35 : 9,10 ''
    IL_005a:  ret
  } // end of method NullableTests::TestLiftingNonNullArgs

  .method private hidebysig static void  TestLiftingNullArgs() cil managed
  {
    // Code size       147 (0x93)
    .maxstack  2
    .locals init ([0] valuetype [mscorlib]System.Nullable`1<int32> n1,
             [1] valuetype [mscorlib]System.Nullable`1<int32> n2,
             [2] valuetype [mscorlib]System.Nullable`1<int32> n3,
             [3] valuetype [mscorlib]System.Nullable`1<int32> CS$0$0000,
             [4] valuetype [mscorlib]System.Nullable`1<int32> CS$0$0001,
             [5] valuetype [mscorlib]System.Nullable`1<int32> CS$0$0002)
    .line 38,38 : 9,10 ''
    IL_0000:  nop
    .line 39,39 : 13,65 ''
    IL_0001:  ldstr      "Testing lifting with null args"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    .line 40,40 : 13,30 ''
    IL_000c:  ldloca.s   n1
    IL_000e:  ldc.i4.1
    IL_000f:  call       instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0014:  nop
    .line 41,41 : 13,36 ''
    IL_0015:  ldloca.s   n2
    IL_0017:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    .line 42,42 : 13,30 ''
    IL_001d:  ldloc.0
    IL_001e:  stloc.3
    IL_001f:  ldloc.1
    IL_0020:  stloc.s    CS$0$0001
    IL_0022:  ldloca.s   CS$0$0000
    IL_0024:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0029:  ldloca.s   CS$0$0001
    IL_002b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0030:  and
    IL_0031:  brtrue.s   IL_003f

    IL_0033:  ldloca.s   CS$0$0002
    IL_0035:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_003b:  ldloc.s    CS$0$0002
    IL_003d:  br.s       IL_0053

    IL_003f:  ldloca.s   CS$0$0000
    IL_0041:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0046:  ldloca.s   CS$0$0001
    IL_0048:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_004d:  add
    IL_004e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0053:  nop
    IL_0054:  stloc.2
    .line 43,43 : 13,34 ''
    IL_0055:  ldloc.0
    IL_0056:  stloc.3
    IL_0057:  ldloc.1
    IL_0058:  stloc.s    CS$0$0001
    IL_005a:  ldloca.s   CS$0$0000
    IL_005c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0061:  ldloca.s   CS$0$0001
    IL_0063:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0068:  and
    IL_0069:  brtrue.s   IL_0077

    IL_006b:  ldloca.s   CS$0$0002
    IL_006d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0073:  ldloc.s    CS$0$0002
    IL_0075:  br.s       IL_008b

    IL_0077:  ldloca.s   CS$0$0000
    IL_0079:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007e:  ldloca.s   CS$0$0001
    IL_0080:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0085:  add
    IL_0086:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_008b:  nop
    IL_008c:  call       void Test.NullableTests::LogNullable<int32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0091:  nop
    .line 44,44 : 9,10 ''
    IL_0092:  ret
  } // end of method NullableTests::TestLiftingNullArgs

  .method private hidebysig static void  LogNullable<valuetype .ctor ([mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T> nt) cil managed
  {
    // Code size       365 (0x16d)
    .maxstack  2
    .locals init ([0] object o,
             [1] valuetype [mscorlib]System.Nullable`1<!!T> nt2,
             [2] !!T t1,
             [3] class [mscorlib]System.Exception e,
             [4] !!T t2,
             [5] bool CS$4$0000,
             [6] !!T CS$0$0001,
             [7] valuetype [mscorlib]System.Nullable`1<!!T> CS$0$0002)
    .line 47,47 : 9,10 ''
    IL_0000:  nop
    .line 48,48 : 13,29 ''
    IL_0001:  ldarga.s   nt
    IL_0003:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.s    CS$4$0000
    IL_000d:  ldloc.s    CS$4$0000
    IL_000f:  brtrue.s   IL_0039

    .line 49,49 : 17,80 ''
    IL_0011:  ldstr      "Nullable has value "
    IL_0016:  ldarga.s   nt
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
    IL_001d:  stloc.s    CS$0$0001
    IL_001f:  ldloca.s   CS$0$0001
    IL_0021:  constrained. !!T
    IL_0027:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_002c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0036:  nop
    IL_0037:  br.s       IL_0044

    .line 51,51 : 17,60 ''
    IL_0039:  ldstr      "Nullable has no value"
    IL_003e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0043:  nop
    .line 52,52 : 13,28 ''
    IL_0044:  ldarga.s   nt
    IL_0046:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_004b:  stloc.s    CS$4$0000
    IL_004d:  ldloc.s    CS$4$0000
    IL_004f:  brtrue.s   IL_005c

    .line 53,53 : 17,59 ''
    IL_0051:  ldstr      "Nullable equals null"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  nop
    .line 54,54 : 13,32 ''
    IL_005c:  ldarg.0
    IL_005d:  box        valuetype [mscorlib]System.Nullable`1<!!T>
    IL_0062:  stloc.0
    .line 55,55 : 13,27 ''
    IL_0063:  ldloc.0
    IL_0064:  ldnull
    IL_0065:  ceq
    IL_0067:  ldc.i4.0
    IL_0068:  ceq
    IL_006a:  stloc.s    CS$4$0000
    IL_006c:  ldloc.s    CS$4$0000
    IL_006e:  brtrue.s   IL_007d

    .line 56,56 : 17,68 ''
    IL_0070:  ldstr      "Nullable casts to null object"
    IL_0075:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007a:  nop
    IL_007b:  br.s       IL_0093

    .line 58,58 : 17,79 ''
    IL_007d:  ldstr      "Nullable casts to object "
    IL_0082:  ldloc.0
    IL_0083:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0088:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_008d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0092:  nop
    .line 59,59 : 13,38 ''
    IL_0093:  ldloc.0
    IL_0094:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!T>
    IL_0099:  stloc.1
    .line 60,60 : 13,29 ''
    IL_009a:  ldarga.s   nt
    IL_009c:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_00a1:  ldc.i4.0
    IL_00a2:  ceq
    IL_00a4:  stloc.s    CS$4$0000
    IL_00a6:  ldloc.s    CS$4$0000
    IL_00a8:  brtrue.s   IL_00d2

    .line 61,61 : 17,87 ''
    IL_00aa:  ldstr      "Recast nullable has value "
    IL_00af:  ldarga.s   nt
    IL_00b1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
    IL_00b6:  stloc.s    CS$0$0001
    IL_00b8:  ldloca.s   CS$0$0001
    IL_00ba:  constrained. !!T
    IL_00c0:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00c5:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_00ca:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00cf:  nop
    IL_00d0:  br.s       IL_00dd

    .line 63,63 : 17,67 ''
    IL_00d2:  ldstr      "Recast nullable has no value"
    IL_00d7:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00dc:  nop
    .line 65,65 : 13,14 ''
    .try
    {
      IL_00dd:  nop
      .line 66,66 : 17,32 ''
      IL_00de:  ldarga.s   nt
      IL_00e0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::get_Value()
      IL_00e5:  stloc.2
      .line 67,67 : 17,90 ''
      IL_00e6:  ldstr      "Nullable casts to underlying value "
      IL_00eb:  ldloca.s   t1
      IL_00ed:  constrained. !!T
      IL_00f3:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_00f8:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_00fd:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0102:  nop
      .line 68,68 : 13,14 ''
      IL_0103:  nop
      IL_0104:  leave.s    IL_012c

      .line 69,69 : 13,32 ''
    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0106:  stloc.3
      .line 70,70 : 13,14 ''
      IL_0107:  nop
      .line 71,71 : 17,81 ''
      IL_0108:  ldstr      "Nullable fails casting to underlying value"
      IL_010d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0112:  nop
      .line 72,72 : 17,70 ''
      IL_0113:  ldstr      "Exception Message: "
      IL_0118:  ldloc.3
      IL_0119:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_011e:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0123:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0128:  nop
      .line 73,73 : 13,14 ''
      IL_0129:  nop
      IL_012a:  leave.s    IL_012c

    }  // end handler
    IL_012c:  nop
    .line 74,74 : 13,39 ''
    IL_012d:  ldarg.0
    IL_012e:  stloc.s    CS$0$0002
    IL_0130:  ldloca.s   CS$0$0002
    IL_0132:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_0137:  brtrue.s   IL_0145

    IL_0139:  ldloca.s   CS$0$0001
    IL_013b:  initobj    !!T
    IL_0141:  ldloc.s    CS$0$0001
    IL_0143:  br.s       IL_014c

    IL_0145:  ldloca.s   CS$0$0002
    IL_0147:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::GetValueOrDefault()
    IL_014c:  nop
    IL_014d:  stloc.s    t2
    .line 75,75 : 13,71 ''
    IL_014f:  ldstr      "Value or default is "
    IL_0154:  ldloca.s   t2
    IL_0156:  constrained. !!T
    IL_015c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0161:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0166:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_016b:  nop
    .line 76,76 : 9,10 ''
    IL_016c:  ret
  } // end of method NullableTests::LogNullable

} // end of class Test.NullableTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file E:\Users\Gautam\Documents\Visual Studio 2012\Projects\cs2jsc\Test\Foo.res
