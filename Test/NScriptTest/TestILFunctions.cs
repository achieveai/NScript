using System.Collections.Generic;
using NScript.Lib.ILParser;
using NScript.Lib.MetaData;

namespace NScriptTest
{
    class FunctionParts
    {
        public string Decleration { get; set; }
        public string[] Body { get; set; }
    }

    class TestILFunctions
    {
        public static ExecutionBlock GetExecutionBlock(FunctionParts functionParts)
        {
            var methodInfo = ILMethod.ParseFromHeader(functionParts.Decleration);
            var methodSignature = new MethodSignature(
                methodInfo.MethodName,
                new ClassSignature("Boo", new AssemblySignature("Assembly"), 0),
                (methodInfo.Attributes & MethodAttributes.Static) == MethodAttributes.Static,
                methodInfo.Arguments,
                new ClassSignature("Bar", new AssemblySignature("Assembly"), 0));

            return new ExecutionBlock(methodSignature, new List<string>(functionParts.Body));
        }

        #region SimpleIntReturnFunction
        public static readonly FunctionParts SimpleIntReturnFunction = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance int64 Func2(int64 i1, int64 i2) cil managed",
            Body = new[]
            {
                @"    .maxstack  1",
                @"    .locals init ([0] int64 CS$1$0000)",
                @"    .line 134,134 : 9,10 ''",
                @"    IL_0000:  nop",
                @"    .line 135,135 : 13,22 ''",
                @"    IL_0001:  ldc.i4.0",
                @"    IL_0002:  conv.i8",
                @"    IL_0003:  stloc.0",
                @"    IL_0004:  br.s       IL_0006",
                @"    .line 136,136 : 9,10 ''",
                @"    IL_0006:  ldloc.0",
                @"    IL_0007:  ret",
            },
        };
        #endregion

        #region StringFromat3ArgCall
        public static readonly FunctionParts StringFormat3ArgCall = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance int64 Func2(int64 i1, int64 i2) cil managed",
            Body = new[]
            {
                @"    .maxstack  4",
                @"    .locals init ([0] string CS$1$0000, [1] object[] CS$0$0001)",
                @"    .line 122,122 : 9,10 ''",
                @"    IL_0000:  nop",
                @"    .line 123,123 : 13,53 ''",
                @"    IL_0001:  ldstr      ""{0}-{1}""",
                @"      IL_0006:  ldc.i4.2",
                @"          IL_0007:  newarr     [sscorlib]System.Object",
                @"      IL_000c:  stloc.1",
                @"      IL_000d:  ldloc.1",
                @"          IL_000e:  ldc.i4.0",
                @"              IL_000f:  ldarg.1",
                @"                  IL_0010:  box        !!T",
                @"                  IL_0015:  stelem.ref",
                @"      IL_0016:  ldloc.1",
                @"          IL_0017:  ldc.i4.1",
                @"              IL_0018:  ldarg.2",
                @"                  IL_0019:  box        !!U",
                @"                  IL_001e:  stelem.ref",
                @"      IL_001f:  ldloc.1",
                @"          IL_0020:  call       string [sscorlib]System.String::Format(string, object[])",
                @"      IL_0025:  stloc.0",
                @"    IL_0026:  br.s       IL_0028",
                @"    .line 124,124 : 9,10 ''",
                @"    IL_0028:  ldloc.0",
                @"    IL_0029:  ret",
            },
        };
        #endregion

        #region SimpleConditionalArgument
        public static readonly FunctionParts SimpleConditionalArgument = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void FuncRef(int32& i1, int32 i2, int32 i3) cil managed",
            Body = new[]
            {
                @"    .maxstack  8",
                @"    .line 72,72 : 9,10 ''",
                @"    IL_0000:  nop",
                @"    .line 74,74 : 13,48 ''",
                @"    IL_0001:  ldarg.1",
                @"      IL_0002:  ldarg.0",
                @"          IL_0003:  ldarg.2",
                @"              IL_0004:  ldarg.2",
                @"                  IL_0005:  ldc.i4.s   10",
                @"                      IL_0007:  beq.s      IL_000d",
                @"              IL_0009:  ldc.i4.s   25",
                @"                  IL_000b:  br.s       IL_000f",
                @"              IL_000d:  ldc.i4.s   14",
                @"                  IL_000f:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                @"          IL_0014:  stind.i4",
                @"    .line 75,75 : 9,10 ''",
                @"    IL_0015:  ret",
            },
        };
        #endregion

        #region Complex1ConditionalArgument
        public static readonly FunctionParts Complex1ConditionalArgument = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void FuncRef(int32& i1, int32 i2, int32 i3) cil managed",
            Body = new[]
            {
                @"    .maxstack  8",
                @"    .line 72,72 : 9,10 ''",
                @"    IL_0000:  nop",
                @"    .line 74,78 : 13,49 ''",
                @"    IL_0001:  ldarg.1",
                @"    IL_0002:  ldarg.0",
                @"    IL_0003:  ldarg.2",
                @"    IL_0004:  ldarg.2",
                @"    IL_0005:  ldc.i4.3",
                @"    IL_0006:  bne.un.s   IL_0021",
                @"",
                @"    IL_0008:  ldarg.1",
                @"    IL_0009:  ldind.i4",
                @"    IL_000a:  ldc.i4.s   10",
                @"    IL_000c:  bne.un.s   IL_0021",
                @"",
                @"    IL_000e:  ldarg.1",
                @"    IL_000f:  ldind.i4",
                @"    IL_0010:  ldc.i4.1",
                @"    IL_0011:  beq.s      IL_0018",
                @"",
                @"    IL_0013:  ldarg.2",
                @"    IL_0014:  ldc.i4.s   9",
                @"    IL_0016:  bne.un.s   IL_0021",
                @"",
                @"    IL_0018:  ldarg.1",
                @"    IL_0019:  ldind.i4",
                @"    IL_001a:  ldc.i4.2",
                @"    IL_001b:  beq.s      IL_002e",
                @"",
                @"    IL_001d:  ldarg.2",
                @"    IL_001e:  ldc.i4.3",
                @"    IL_001f:  beq.s      IL_002e",
                @"",
                @"    IL_0021:  ldarg.1",
                @"    IL_0022:  ldind.i4",
                @"    IL_0023:  ldc.i4.4",
                @"    IL_0024:  bne.un.s   IL_002a",
                @"",
                @"    IL_0026:  ldarg.2",
                @"    IL_0027:  ldc.i4.3",
                @"    IL_0028:  beq.s      IL_002e",
                @"",
                @"    IL_002a:  ldc.i4.s   25",
                @"    IL_002c:  br.s       IL_0030",
                @"",
                @"    IL_002e:  ldc.i4.s   14",
                @"    IL_0030:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                @"    IL_0035:  stind.i4",
                @"    .line 79,79 : 9,10 ''",
                @"    IL_0036:  ret",
            },
        };
        #endregion

        #region Complex2ConditionalArgument
        public static readonly FunctionParts Complex2ConditionalArgument = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void FuncRef(int32& i1, int32 i2, int32 i3) cil managed",
            Body = new[]
            {
                @"    .maxstack  8",
                @"    IL_0000:  nop",
                @"    IL_0001:  ldarg.1",
                @"      IL_0002:  ldarg.0",
                @"          IL_0003:  ldarg.2",
                @"              IL_0004:  ldarg.2",
                @"                  IL_0005:  ldc.i4.3",
                @"                      IL_0006:  bne.un.s   IL_0022",
                @"",
                @"              IL_0008:  ldarg.3",
                @"                  IL_0009:  ldc.i4.s   10",
                @"                      IL_000b:  bne.un.s   IL_0022",
                @"",
                @"              IL_000d:  ldarg.3",
                @"                  IL_000e:  ldc.i4.2",
                @"                      IL_000f:  beq.s      IL_002e",
                @"",
                @"              IL_0011:  ldarg.2",
                @"                  IL_0012:  ldarg.3",
                @"                      IL_0013:  ldc.i4.s   10",
                @"                          IL_0015:  beq.s      IL_001f",
                @"",
                @"                  IL_0017:  ldarg.3",
                @"                      IL_0018:  ldc.i4.4",
                @"                          IL_0019:  blt.s      IL_001f",
                @"",
                @"                  IL_001b:  ldc.i4.s   10",
                @"                      IL_001d:  br.s       IL_0020",
                @"",
                @"                  IL_001f:  ldc.i4.4",
                @"                      IL_0020:  beq.s      IL_002e",
                @"",
                @"              IL_0022:  ldarg.3",
                @"                  IL_0023:  ldc.i4.4",
                @"                      IL_0024:  bne.un.s   IL_002a",
                @"",
                @"              IL_0026:  ldarg.2",
                @"                  IL_0027:  ldc.i4.3",
                @"                      IL_0028:  beq.s      IL_002e",
                @"",
                @"              IL_002a:  ldc.i4.s   25",
                @"                  IL_002c:  br.s       IL_0030",
                @"",
                @"              IL_002e:  ldc.i4.s   14",
                @"                  IL_0030:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                @"          IL_0035:  stind.i4",
                @"    IL_0036:  ret",
            },
        };
        #endregion

        #region Complex2IfCondition
        /// <summary>
        /// public void FuncRef(ref int i1, int i2, int i3)
        /// {
        ///     if ((i2 == 3 && i3 == 10) &&
        ///         (i3 == 2 || i2 == (i3 == 10 || i3 &lt; 4 ? 4 : 10)) ||
        ///         (i3 == 4 && i2 == 3))
        ///     {
        ///         i1 = Func2 (i2, 15);
        ///     }
        ///     else
        ///     {
        ///         i1 = Func2 (i2, 25);
        ///     }
        /// }
        /// </summary>
        public static readonly FunctionParts Complex2IfCondition = new FunctionParts
        {
            Decleration =
                @"  .method public hidebysig instance void FuncRef(int32& i1, int32 i2, int32 i3) cil managed",
            Body = new[]
            {
                @"    .maxstack  4",
                @"    .locals init (bool V_0)"
                ,
                @"    IL_0000:  nop",
                @"    IL_0001:  ldarg.2",
                @"      IL_0002:  ldc.i4.3",
                @"          IL_0003:  bne.un.s   IL_001f"
                ,
                @"",
                @"    IL_0005:  ldarg.3",
                @"      IL_0006:  ldc.i4.s   10"
                ,
                @"          IL_0008:  bne.un.s   IL_001f"
                ,
                @"",
                @"    IL_000a:  ldarg.3",
                @"      IL_000b:  ldc.i4.2",
                @"          IL_000c:  beq.s      IL_002f"
                ,
                @"",
                @"    IL_000e:  ldarg.2",
                @"      IL_000f:  ldarg.3",
                @"          IL_0010:  ldc.i4.s   10"
                ,
                @"              IL_0012:  beq.s      IL_001c"
                ,
                @"",
                @"      IL_0014:  ldarg.3",
                @"          IL_0015:  ldc.i4.4"
                ,
                @"              IL_0016:  blt.s      IL_001c"
                ,
                @"",
                @"      IL_0018:  ldc.i4.s   10"
                ,
                @"          IL_001a:  br.s       IL_001d"
                ,
                @"",
                @"      IL_001c:  ldc.i4.4",
                @"          IL_001d:  beq.s      IL_002f"
                ,
                @"",
                @"    IL_001f:  ldarg.3",
                @"      IL_0020:  ldc.i4.4",
                @"          IL_0021:  bne.un.s   IL_002c"
                ,
                @"",
                @"    IL_0023:  ldarg.2",
                @"      IL_0024:  ldc.i4.3",
                @"          IL_0025:  ceq",
                @"      IL_0027:  ldc.i4.0",
                @"          IL_0028:  ceq",
                @"      IL_002a:  br.s       IL_002d"
                ,
                @"",
                @"    IL_002c:  ldc.i4.1",
                @"      IL_002d:  br.s       IL_0030"
                ,
                @"",
                @"    IL_002f:  ldc.i4.0",
                @"      IL_0030:  stloc.0",
                @"    IL_0031:  ldloc.0",
                @"      IL_0032:  brtrue.s   IL_0043"
                ,
                @"",
                @"    IL_0034:  nop",
                @"    IL_0035:  ldarg.1",
                @"      IL_0036:  ldarg.0",
                @"          IL_0037:  ldarg.2"
                ,
                @"              IL_0038:  ldc.i4.s   15"
                ,
                @"                  IL_003a:  call       instance int32 RealScript.Class1::Func2(int32, int32)"
                ,
                @"          IL_003f:  stind.i4"
                ,
                @"    IL_0040:  nop",
                @"    IL_0041:  br.s       IL_0050"
                ,
                @"",
                @"    IL_0043:  nop",
                @"    IL_0044:  ldarg.1",
                @"      IL_0045:  ldarg.0",
                @"          IL_0046:  ldarg.2"
                ,
                @"              IL_0047:  ldc.i4.s   25"
                ,
                @"                  IL_0049:  call       instance int32 RealScript.Class1::Func2(int32, int32)"
                ,
                @"          IL_004e:  stind.i4"
                ,
                @"    IL_004f:  nop",
                @"    IL_0050:  ret",
            },
        };
        #endregion

        #region ComplexIfConditionOptimized1

        /// <summary>
        /// public int Func2(int i1, int i2)
        /// {
        ///     bool b = false;
        ///     int returnValue = 0;
        ///     if ((i2 == 3 && i1 == 10) &&
        ///         (i1 == 1 || i2 == 9) &&
        ///         (i1 == 2 || i2 == 3) ||
        ///         (i1 == 4 && i2 == 3))
        ///     {
        ///         returnValue = i1 + i2 * 2;
        ///     }
        ///     else if (i1 &gt; 2 && i2 &lt; 3)
        ///     {
        ///         returnValue = i1 + i2;
        ///     }
        ///     else if (b)
        ///     {
        ///         returnValue = i2 - i1;
        ///     }
        /// 
        ///     returnValue += i2;
        /// 
        ///     return returnValue;
        /// }
        /// </summary>
        public static readonly FunctionParts ComplexIfConditionOptimized1 = new FunctionParts
        {
            Decleration = @".method public hidebysig instance void Func2(int32 i1, int32 i2) cil managed",
            Body = new []
            {
                @".maxstack  3",
                @".locals init ([0] bool b, [1] int32 returnValue)",
                @".line 148,148 : 13,28 ''",
                @"IL_0000:  ldc.i4.0",
                @"IL_0001:  stloc.0",
                @".line 149,149 : 13,33 ''",
                @"IL_0002:  ldc.i4.0",
                @"IL_0003:  stloc.1",
                @".line 150,153 : 13,38 ''",
                @"  IL_0004:  ldarg.2",
                @"  IL_0005:  ldc.i4.3",
                @"IL_0006:  bne.un.s   IL_001e",
                @"",
                @"  IL_0008:  ldarg.1",
                @"  IL_0009:  ldc.i4.s   10",
                @"IL_000b:  bne.un.s   IL_001e",
                @"",
                @"  IL_000d:  ldarg.1",
                @"  IL_000e:  ldc.i4.1",
                @"IL_000f:  beq.s      IL_0016",
                @"",
                @"  IL_0011:  ldarg.2",
                @"  IL_0012:  ldc.i4.s   9",
                @"IL_0014:  bne.un.s   IL_001e",
                @"",
                @"  IL_0016:  ldarg.1",
                @"  IL_0017:  ldc.i4.2",
                @"IL_0018:  beq.s      IL_0026",
                @"",
                @"  IL_001a:  ldarg.2",
                @"  IL_001b:  ldc.i4.3",
                @"IL_001c:  beq.s      IL_0026",
                @"",
                @"  IL_001e:  ldarg.1",
                @"  IL_001f:  ldc.i4.4",
                @"IL_0020:  bne.un.s   IL_002e",
                @"",
                @"  IL_0022:  ldarg.2",
                @"  IL_0023:  ldc.i4.3",
                @"IL_0024:  bne.un.s   IL_002e",
                @"",
                @".line 155,155 : 17,43 ''",
                @"      IL_0026:  ldarg.1",
                @"          IL_0027:  ldarg.2",
                @"          IL_0028:  ldc.i4.2",
                @"      IL_0029:  mul",
                @"  IL_002a:  add",
                @"IL_002b:  stloc.1",
                @"IL_002c:  br.s       IL_0043",
                @"",
                @".line 157,157 : 18,39 ''",
                @"  IL_002e:  ldarg.1",
                @"  IL_002f:  ldc.i4.2",
                @"IL_0030:  ble.s      IL_003c",
                @"",
                @"  IL_0032:  ldarg.2",
                @"  IL_0033:  ldc.i4.3",
                @"IL_0034:  bge.s      IL_003c",
                @"",
                @".line 159,159 : 17,39 ''",
                @"      IL_0036:  ldarg.1",
                @"      IL_0037:  ldarg.2",
                @"  IL_0038:  add",
                @"IL_0039:  stloc.1",
                @"IL_003a:  br.s       IL_0043",
                @"",
                @".line 161,161 : 18,24 ''",
                @"  IL_003c:  ldloc.0",
                @"IL_003d:  brfalse.s  IL_0043",
                @"",
                @".line 163,163 : 17,39 ''",
                @"      IL_003f:  ldarg.2",
                @"      IL_0040:  ldarg.1",
                @"  IL_0041:  sub",
                @"IL_0042:  stloc.1",
                @".line 166,166 : 13,31 ''",
                @"      IL_0043:  ldloc.1",
                @"      IL_0044:  ldarg.2",
                @"  IL_0045:  add",
                @"IL_0046:  stloc.1",
                @".line 168,168 : 13,32 ''",
                @"  IL_0047:  ldloc.1",
                @"IL_0048:  ret",
            }
        };
        #endregion

        #region ComplexIfConditionOptimized2
        /// <summary>
        /// public void FuncRef(ref int i1, int i2, int i3)
        /// {
        ///     if ((i2 == 3 && i3 == 10) &&
        ///         (i3 == 2 || i2 == (i3 == 10 || i3 &lt; 4 ? 4 : 10)) ||
        ///         (i3 == 4 && i2 == 3))
        ///     {
        ///         i1 = Func2 (i2, 15);
        ///     }
        ///     else
        ///     {
        ///         i1 = Func2 (i2, 25);
        ///     }
        /// }
        /// </summary>
        public static readonly FunctionParts ComplexIfConditionOptimized2 = new FunctionParts
        {
            Decleration = 
                @"  .method public hidebysig instance void FuncRef(int32& i1, int32 i2, int32 i3) cil managed",
            Body = new[]
            {
                @".maxstack  8",
                @".line 74,76 : 13,38 ''",
                @"  IL_0000:  ldarg.2",
                @"  IL_0001:  ldc.i4.3",
                @"IL_0002:  bne.un.s   IL_001e",
                @"",
                @"  IL_0004:  ldarg.3",
                @"  IL_0005:  ldc.i4.s   10",
                @"IL_0007:  bne.un.s   IL_001e",
                @"",
                @"  IL_0009:  ldarg.3",
                @"  IL_000a:  ldc.i4.2",
                @"IL_000b:  beq.s      IL_0026",
                @"",
                @"  IL_000d:  ldarg.2",
                @"      IL_000e:  ldarg.3",
                @"      IL_000f:  ldc.i4.s   10",
                @"  IL_0011:  beq.s      IL_001b",
                @"",
                @"      IL_0013:  ldarg.3",
                @"      IL_0014:  ldc.i4.4",
                @"  IL_0015:  blt.s      IL_001b",
                @"",
                @"  IL_0017:  ldc.i4.s   10",
                @"  IL_0019:  br.s       IL_001c",
                @"",
                @"  IL_001b:  ldc.i4.4",
                @"IL_001c:  beq.s      IL_0026",
                @"",
                @"  IL_001e:  ldarg.3",
                @"  IL_001f:  ldc.i4.4",
                @"IL_0020:  bne.un.s   IL_0032",
                @"",
                @"  IL_0022:  ldarg.2",
                @"  IL_0023:  ldc.i4.3",
                @"IL_0024:  bne.un.s   IL_0032",
                @"",
                @".line 78,78 : 17,37 ''",
                @"  IL_0026:  ldarg.1",
                @"      IL_0027:  ldarg.0",
                @"      IL_0028:  ldarg.2",
                @"      IL_0029:  ldc.i4.s   15",
                @"  IL_002b:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                @"IL_0030:  stind.i4",
                @"IL_0031:  ret",
                @"",
                @".line 82,82 : 17,37 ''",
                @"  IL_0032:  ldarg.1",
                @"      IL_0033:  ldarg.0",
                @"      IL_0034:  ldarg.2",
                @"      IL_0035:  ldc.i4.s   25",
                @"  IL_0037:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                @"IL_003c:  stind.i4",
                @".line 84,84 : 9,10 ''",
                @"IL_003d:  ret",
            }
        };
        #endregion

        #region IncrementOnStack
        /// <summary>
        ///        public int FuncIncrement(int i1, int i2)
        ///        {
        ///            i2 += Func2(i1++, 10);
        ///
        ///            i2 += Func2(i1, i2++);
        ///
        ///            return i2;
        ///        }
        /// </summary>
        public static readonly FunctionParts IncrementOnStack =
            new FunctionParts
            {
                Decleration = @".method public hidebysig instance int32 FuncIncrement(int32 i1, int32 i2) cil managed",
                Body =
                    new string[]
                    {
                        @".maxstack  8",
                        @".line 88,88 : 13,35 ''",
                        @"      IL_0000:  ldarg.2",
                        @"          IL_0001:  ldarg.0",
                        @"              IL_0002:  ldarg.1",
                        @"          IL_0003:  dup",
                        @"                  IL_0004:  ldc.i4.1",
                        @"              IL_0005:  add",
                        @"          IL_0006:  starg.s    i1",
                        @"          IL_0008:  ldc.i4.s   10",
                        @"      IL_000a:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                        @"  IL_000f:  add",
                        @"IL_0010:  starg.s    i2",
                        @".line 90,90 : 13,35 ''",
                        @"      IL_0012:  ldarg.2",
                        @"          IL_0013:  ldarg.0",
                        @"          IL_0014:  ldarg.1",
                        @"                  IL_0015:  ldarg.2",
                        @"              IL_0016:  dup",
                        @"                  IL_0017:  ldc.i4.1",
                        @"              IL_0018:  add",
                        @"          IL_0019:  starg.s    i2",
                        @"      IL_001b:  call       instance int32 RealScript.Class1::Func2(int32, int32)",
                        @"  IL_0020:  add",
                        @"IL_0021:  starg.s    i2",
                        @".line 92,92 : 13,23 ''",
                        @"IL_0023:  ldarg.2",
                        @"IL_0024:  ret",
                    },
            };
        #endregion

        #region SimpleSwitchFunction
        /// <summary>
        /// public void FooSwitch(int i)
        /// {
        ///     string.Format("{}");
        /// 
        ///     switch (i)
        ///     {
        ///         case 100:
        ///             string.Format("OneHundred");
        ///             break;
        ///         case 10:
        ///             string.Format("Ten");
        ///             break;
        ///         case 2:
        ///         case 16:
        ///         case 32:
        ///             string.Format("Power Of Two");
        ///             break;
        ///         case 101:
        ///             string.Format("Contigous Test");
        ///             break;
        ///         case 104:
        ///             string.Format("Contigous Test 2");
        ///             break;
        ///         case 102:
        ///             string.Format("Contigous Test 3");
        ///             break;
        ///         case 103:
        ///             string.Format("Contigous Test 4");
        ///             break;
        ///         default:
        ///             string.Format("Default");
        ///             break;
        ///     }
        /// 
        ///     string.Format("{0}", "done with NoDefault stuff");
        /// }
        /// </summary>
        public static readonly FunctionParts SimpleSwitchFunction = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void FooSwitch(int32 i) cil managed",
            Body = new[]
            {
                @".maxstack  4",
                @".locals init ([0] int32 CS$4$0000, [1] object[] CS$0$0001)",
                @".line 305,305 : 9,10 ''",
                @"IL_0000:  nop",
                @".line 306,306 : 13,33 ''",
                @"IL_0001:  ldstr      ""{}""",
                @"  IL_0006:  ldc.i4.0",
                @"      IL_0007:  newarr     [sscorlib]System.Object",
                @"  IL_000c:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_0011:  pop",
                @".line 308,308 : 13,23 ''",
                @"IL_0012:  ldarg.1",
                @"  IL_0013:  stloc.0",
                @"IL_0014:  ldloc.0",
                @"  IL_0015:  ldc.i4.s   10",
                @"      IL_0017:  bgt.s      IL_0027",
                @"",
                @"IL_0019:  ldloc.0",
                @"  IL_001a:  ldc.i4.2",
                @"      IL_001b:  beq.s      IL_007c",
                @"",
                @"IL_001d:  ldloc.0",
                @"  IL_001e:  ldc.i4.s   10",
                @"      IL_0020:  beq.s      IL_0069",
                @"",
                @"IL_0022:  br         IL_00db",
                @"",
                @"IL_0027:  ldloc.0",
                @"  IL_0028:  ldc.i4.s   16",
                @"      IL_002a:  beq.s      IL_007c",
                @"",
                @"IL_002c:  ldloc.0",
                @"  IL_002d:  ldc.i4.s   32",
                @"      IL_002f:  beq.s      IL_007c",
                @"",
                @"IL_0031:  ldloc.0",
                @"  IL_0032:  ldc.i4.s   100",
                @"      IL_0034:  sub",
                @"  IL_0035:  switch     ( IL_0053, IL_008f, IL_00b5, IL_00c8, IL_00a2)",
                @"IL_004e:  br         IL_00db",
                @"",
                @".line 311,311 : 21,49 ''",
                @"IL_0053:  ldstr      ""OneHundred""",
                @"  IL_0058:  ldc.i4.0",
                @"      IL_0059:  newarr     [sscorlib]System.Object",
                @"  IL_005e:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_0063:  pop",
                @".line 312,312 : 21,27 ''",
                @"IL_0064:  br         IL_00ee",
                @"",
                @".line 314,314 : 21,42 ''",
                @"IL_0069:  ldstr      ""Ten""",
                @"  IL_006e:  ldc.i4.0",
                @"      IL_006f:  newarr     [sscorlib]System.Object",
                @"  IL_0074:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_0079:  pop",
                @".line 315,315 : 21,27 ''",
                @"IL_007a:  br.s       IL_00ee",
                @"",
                @".line 319,319 : 21,51 ''",
                @"IL_007c:  ldstr      ""Power Of Two""",
                @"  IL_0081:  ldc.i4.0",
                @"      IL_0082:  newarr     [sscorlib]System.Object",
                @"  IL_0087:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_008c:  pop",
                @".line 320,320 : 21,27 ''",
                @"IL_008d:  br.s       IL_00ee",
                @"",
                @".line 322,322 : 21,53 ''",
                @"IL_008f:  ldstr      ""Contigous Test""",
                @"  IL_0094:  ldc.i4.0",
                @"      IL_0095:  newarr     [sscorlib]System.Object",
                @"  IL_009a:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_009f:  pop",
                @".line 323,323 : 21,27 ''",
                @"IL_00a0:  br.s       IL_00ee",
                @"",
                @".line 325,325 : 21,55 ''",
                @"IL_00a2:  ldstr      ""Contigous Test 2""",
                @"  IL_00a7:  ldc.i4.0",
                @"      IL_00a8:  newarr     [sscorlib]System.Object",
                @"  IL_00ad:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_00b2:  pop",
                @".line 326,326 : 21,27 ''",
                @"IL_00b3:  br.s       IL_00ee",
                @"",
                @".line 328,328 : 21,55 ''",
                @"IL_00b5:  ldstr      ""Contigous Test 3""",
                @"  IL_00ba:  ldc.i4.0",
                @"      IL_00bb:  newarr     [sscorlib]System.Object",
                @"  IL_00c0:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_00c5:  pop",
                @".line 329,329 : 21,27 ''",
                @"IL_00c6:  br.s       IL_00ee",
                @"",
                @".line 331,331 : 21,55 ''",
                @"IL_00c8:  ldstr      ""Contigous Test 4""",
                @"  IL_00cd:  ldc.i4.0",
                @"      IL_00ce:  newarr     [sscorlib]System.Object",
                @"  IL_00d3:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_00d8:  pop",
                @".line 332,332 : 21,27 ''",
                @"IL_00d9:  br.s       IL_00ee",
                @"",
                @".line 334,334 : 21,46 ''",
                @"IL_00db:  ldstr      ""Default""",
                @"  IL_00e0:  ldc.i4.0",
                @"      IL_00e1:  newarr     [sscorlib]System.Object",
                @"  IL_00e6:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_00eb:  pop",
                @".line 335,335 : 21,27 ''",
                @"IL_00ec:  br.s       IL_00ee",
                @"",
                @".line 412,412 : 13,63 ''",
                @"IL_00ee:  ldstr      ""{0}""",
                @"  IL_00f3:  ldc.i4.1",
                @"      IL_00f4:  newarr     [sscorlib]System.Object",
                @"  IL_00f9:  stloc.1",
                @"  IL_00fa:  ldloc.1",
                @"      IL_00fb:  ldc.i4.0",
                @"          IL_00fc:  ldstr      ""done with NoDefault stuff""",
                @"  IL_0101:  stelem.ref",
                @"  IL_0102:  ldloc.1",
                @"      IL_0103:  call       string [sscorlib]System.String::Format(string, object[])",
                @"  IL_0108:  pop",
                @".line 429,429 : 9,10 ''",
                @"IL_0109:  ret",
            },
        };
        #endregion

        #region simpleDoWhileLoop
        public static readonly FunctionParts SimpleDoWhileLoop = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void SimpleDoWhileLoop(int32 i) cil managed",
            Body = new[]
            {
                @".maxstack  4",
                @".locals init ([0] object[] CS$0$0000, [1] bool CS$4$0001)",
                @".line 324,324 : 9,10 ''",
                @"IL_0000:  nop",
                @".line 325,325 : 13,34 ''",
                @"      IL_0001:  ldstr      ""tmp""",
                @"          IL_0006:  ldc.i4.0",
                @"      IL_0007:  newarr     [sscorlib]System.Object",
                @"  IL_000c:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0011:  pop",
                @".line 328,328 : 13,14 ''",
                @"IL_0012:  nop",
                @".line 329,329 : 17,41 ''",
                @"      IL_0013:  ldstr      ""{0}""",
                @"              IL_0018:  ldc.i4.1",
                @"          IL_0019:  newarr     [sscorlib]System.Object",
                @"      IL_001e:  stloc.0",
                @"          IL_001f:  ldloc.0",
                @"          IL_0020:  ldc.i4.0",
                @"          IL_0021:  ldarg.1",
                @"          IL_0022:  box        [sscorlib]System.Int32",
                @"      IL_0027:  stelem.ref",
                @"      IL_0028:  ldloc.0",
                @"  IL_0029:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_002e:  pop",
                @".line 331,331 : 17,21 ''",
                @"      IL_002f:  ldarg.1",
                @"      IL_0030:  ldc.i4.1",
                @"  IL_0031:  sub",
                @"IL_0032:  starg.s    i",
                @".line 332,332 : 13,14 ''",
                @"IL_0034:  nop",
                @".line 332,332 : 15,30 ''",
                @"    IL_0035:  ldarg.1",
                @"    IL_0036:  ldc.i4.s   10",
                @"  IL_0038:  cgt",
                @"IL_003a:  stloc.1",
                @"IL_003b:  ldloc.1",
                @"IL_003c:  brtrue.s   IL_0012",
                @"",
                @".line 334,334 : 13,34 ''",
                @"      IL_003e:  ldstr      ""tmp""",
                @"          IL_0043:  ldc.i4.0",
                @"      IL_0044:  newarr     [sscorlib]System.Object",
                @"  IL_0049:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_004e:  pop",
                @".line 335,335 : 9,10 ''",
                @"IL_004f:  ret",
            }
        };
        #endregion

        #region SimpleWhileLoop
        public static readonly FunctionParts SimpleWhileLoop = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void SimpleWhileLoop(int32 i) cil managed",
            Body = new[]
            {
                @".maxstack  4",
                @".locals init ([0] object[] CS$0$0000, [1] bool CS$4$0001)",
                @".line 324,324 : 9,10 ''",
                @"IL_0000:  nop",
                @".line 325,325 : 13,34 ''",
                @"      IL_0001:  ldstr      ""tmp""",
                @"          IL_0006:  ldc.i4.0",
                @"      IL_0007:  newarr     [sscorlib]System.Object",
                @"  IL_000c:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0011:  pop",
                @".line 328,328 : 13,14 ''",
                @"IL_0012:  br  IL_0035",
                @"IL_0100:  nop",
                @".line 329,329 : 17,41 ''",
                @"      IL_0013:  ldstr      ""{0}""",
                @"              IL_0018:  ldc.i4.1",
                @"          IL_0019:  newarr     [sscorlib]System.Object",
                @"      IL_001e:  stloc.0",
                @"          IL_001f:  ldloc.0",
                @"          IL_0020:  ldc.i4.0",
                @"          IL_0021:  ldarg.1",
                @"          IL_0022:  box        [sscorlib]System.Int32",
                @"      IL_0027:  stelem.ref",
                @"      IL_0028:  ldloc.0",
                @"  IL_0029:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_002e:  pop",
                @".line 331,331 : 17,21 ''",
                @"      IL_002f:  ldarg.1",
                @"      IL_0030:  ldc.i4.1",
                @"  IL_0031:  sub",
                @"IL_0032:  starg.s    i",
                @".line 332,332 : 13,14 ''",
                @"IL_0034:  nop",
                @".line 332,332 : 15,30 ''",
                @"    IL_0035:  ldarg.1",
                @"    IL_0036:  ldc.i4.s   10",
                @"  IL_0038:  cgt",
                @"IL_003a:  stloc.1",
                @"IL_003b:  ldloc.1",
                @"IL_003c:  brtrue.s   IL_0100",
                @"",
                @".line 334,334 : 13,34 ''",
                @"      IL_003e:  ldstr      ""tmp""",
                @"          IL_0043:  ldc.i4.0",
                @"      IL_0044:  newarr     [sscorlib]System.Object",
                @"  IL_0049:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_004e:  pop",
                @".line 335,335 : 9,10 ''",
                @"IL_004f:  ret",
            }
        };
        #endregion

        #region SimpleForLoop
        public static readonly FunctionParts SimpleForLoop = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void SimpleForLoop(int32 i) cil managed",
            Body = new[]
            {
                @".maxstack  4",
                @".locals init ([0] int32 j, [1] object[] CS$0$0000, [2] bool CS$4$0001)",
                @".line 338,338 : 9,10 ''",
                @"IL_0000:  nop",
                @".line 339,339 : 13,34 ''",
                @"      IL_0001:  ldstr      ""tmp""",
                @"          IL_0006:  ldc.i4.0",
                @"      IL_0007:  newarr     [sscorlib]System.Object",
                @"  IL_000c:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0011:  pop",
                @".line 341,341 : 18,28 ''",
                @"  IL_0012:  ldarg.1",
                @"IL_0013:  stloc.0",
                @"IL_0014:  br.s       IL_003d",
                @"",
                @".line 342,342 : 13,14 ''",
                @"IL_0016:  nop",
                @".line 343,343 : 17,41 ''",
                @"      IL_0017:  ldstr      ""{0}""",
                @"          IL_001c:  ldc.i4.1",
                @"          IL_001d:  newarr     [sscorlib]System.Object",
                @"      IL_0022:  stloc.1",
                @"          IL_0023:  ldloc.1",
                @"          IL_0024:  ldc.i4.0",
                @"              IL_0025:  ldarg.1",
                @"          IL_0026:  box        [sscorlib]System.Int32",
                @"      IL_002b:  stelem.ref",
                @"      IL_002c:  ldloc.1",
                @"  IL_002d:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0032:  pop",
                @".line 345,345 : 17,21 ''",
                @"      IL_0033:  ldarg.1",
                @"      IL_0034:  ldc.i4.1",
                @"  IL_0035:  sub",
                @"IL_0036:  starg.s    i",
                @".line 346,346 : 13,14 ''",
                @"IL_0038:  nop",
                @".line 341,341 : 37,40 ''",
                @"      IL_0039:  ldloc.0",
                @"      IL_003a:  ldc.i4.1",
                @"  IL_003b:  sub",
                @"IL_003c:  stloc.0",
                @".line 341,341 : 29,35 ''",
                @"    IL_003d:  ldloc.0",
                @"    IL_003e:  ldc.i4.s   10",
                @"  IL_0040:  cgt",
                @"IL_0042:  stloc.2",
                @"  IL_0043:  ldloc.2",
                @"IL_0044:  brtrue.s   IL_0016",
                @"",
                @".line 348,348 : 13,34 ''",
                @"      IL_0046:  ldstr      ""tmp""",
                @"          IL_004b:  ldc.i4.0",
                @"      IL_004c:  newarr     [sscorlib]System.Object",
                @"  IL_0051:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0056:  pop",
                @".line 349,349 : 9,10 ''",
                @"IL_0057:  ret",
            }
        };
        #endregion

        #region NestedWhileDoWhileLoop1
        public static readonly FunctionParts NestedWhileDoWhileLoop1 = new FunctionParts
        {
            Decleration =
                @"  .method public hidebysig instance void FuncRef(int32 i) cil managed",
            Body = new string[]
            {
                @".maxstack  2",
                @".locals init ([0] bool CS$4$0000)",
                @".line 352,352 : 9,10 ''",
                @"IL_0000:  nop",
                @".line 353,353 : 13,34 ''",
                @"      IL_0001:  ldstr      ""tmp""",
                @"          IL_0006:  ldc.i4.0",
                @"      IL_0007:  newarr     [sscorlib]System.Object",
                @"  IL_000c:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0011:  pop",
                @"IL_0012:  br.s       IL_002c",
                @"",
                @".line 356,356 : 13,14 ''",
                @"IL_0014:  nop",
                @".line 358,358 : 17,18 ''",
                @"IL_0015:  nop",
                @".line 359,359 : 21,25 ''",
                @"      IL_0016:  ldarg.1",
                @"      IL_0017:  ldc.i4.1",
                @"  IL_0018:  add",
                @"IL_0019:  starg.s    i",
                @".line 360,360 : 17,18 ''",
                @"IL_001b:  nop",
                @".line 360,360 : 19,36 ''",
                @"          IL_001c:  ldarg.1",
                @"          IL_001d:  ldc.i4.2",
                @"      IL_001e:  rem",
                @"      IL_001f:  ldc.i4.0",
                @"  IL_0020:  ceq",
                @"IL_0022:  stloc.0",
                @"IL_0023:  ldloc.0",
                @"IL_0024:  brtrue.s   IL_0015",
                @"",
                @".line 362,362 : 17,21 ''",
                @"      IL_0026:  ldarg.1",
                @"      IL_0027:  ldc.i4.1",
                @"  IL_0028:  sub",
                @"IL_0029:  starg.s    i",
                @".line 363,363 : 13,14 ''",
                @"IL_002b:  nop",
                @".line 355,355 : 13,27 ''",
                @"      IL_002c:  ldarg.1",
                @"      IL_002d:  ldc.i4.s   10",
                @"  IL_002f:  cgt",
                @"IL_0031:  stloc.0",
                @"IL_0032:  ldloc.0",
                @"IL_0033:  brtrue.s   IL_0014",
                @"",
                @".line 365,365 : 13,34 ''",
                @"      IL_0035:  ldstr      ""tmp""",
                @"          IL_003a:  ldc.i4.0",
                @"      IL_003b:  newarr     [sscorlib]System.Object",
                @"  IL_0040:  call       string [sscorlib]System.String::Format(string, object[])",
                @"IL_0045:  pop",
                @".line 366,366 : 9,10 ''",
                @"IL_0046:  ret",

            },
        };
        #endregion

        #region LastDoWhileBlock

        public static readonly FunctionParts LastDoWhileBlock = new FunctionParts
        {
            Decleration = @"  .method public hidebysig instance void FuncRef(int32 i) cil managed",
            Body = new string[]
            {
                @".maxstack  4",
                @".locals init ([0] object[] CS$0$0000)",
                @".line 370,370 : 13,34 ''",
                @"IL_0000:  ldstr      ""tmp""",
                @"IL_0005:  ldc.i4.0",
                @"IL_0006:  newarr     [sscorlib]System.Object",
                @"IL_000b:  call       string [sscorlib]System.String::Format(string,",
                @"                        object[])",
                @"IL_0010:  pop",
                @".line 374,374 : 17,41 ''",
                @"IL_0011:  ldstr      ""{0}""",
                @"IL_0016:  ldc.i4.1",
                @"IL_0017:  newarr     [sscorlib]System.Object",
                @"IL_001c:  stloc.0",
                @"IL_001d:  ldloc.0",
                @"IL_001e:  ldc.i4.0",
                @"IL_001f:  ldarg.1",
                @"IL_0020:  box        [sscorlib]System.Int32",
                @"IL_0025:  stelem.ref",
                @"IL_0026:  ldloc.0",
                @"IL_0027:  call       string [sscorlib]System.String::Format(string,",
                @"                        object[])",
                @"IL_002c:  pop",
                @".line 376,376 : 17,21 ''",
                @"IL_002d:  ldarg.1",
                @"IL_002e:  ldc.i4.1",
                @"IL_002f:  sub",
                @"IL_0030:  starg.s    i",
                @".line 377,377 : 15,30 ''",
                @"IL_0032:  ldarg.1",
                @"IL_0033:  ldc.i4.s   10",
                @"IL_0035:  bgt.s      IL_0011",
                @"",
                @".line 378,378 : 9,10 ''",
                @"IL_0037:  ret",
            }
        };
        #endregion

        #region SimpleIfTest
        /// <summary>
        /// public void SimpleIfTest(int i)
        /// {
        ///     string returnValue = "tmp";
        ///     
        ///     if (i == 10)
        ///     {
        ///         returnValue = "tmp" + "Foo";
        ///     }
        ///
        ///     returnValue = returnValue + "bar";
        ///
        ///     Foo(returnValue.Length);
        /// }
        /// </summary>
        public static readonly FunctionParts SimpleIfTest = new FunctionParts
        {
            Decleration =
                @".method public hidebysig instance void SimpleIfTest(int32 i) cil managed",
            Body = new string[]
            {
                @".maxstack  2",
                @".locals init ([0] string returnValue)",
                @".line 391,391 : 13,40 ''",
                @"  IL_0000:  ldstr      ""tmp""",
                @"IL_0005:  stloc.0",

                @".line 393,393 : 13,25 ''",
                @"  IL_0006:  ldarg.1",
                @"  IL_0007:  ldc.i4.s   10",
                @"IL_0009:  bne.un.s   IL_0011",
                @"",
                @".line 395,395 : 17,45 ''",
                @"  IL_000b:  ldstr      ""tmpFoo""",
                @"IL_0010:  stloc.0",
                @".line 398,398 : 13,47 ''",
                @"      IL_0011:  ldloc.0",
                @"      IL_0012:  ldstr      ""bar""",
                @"  IL_0017:  call       string [sscorlib]System.String::Concat(string, string)",
                @"IL_001c:  stloc.0",
                @".line 400,400 : 13,37 ''",
                @"      IL_001d:  ldarg.0",
                @"      IL_001e:  ldloc.0",
                @"  IL_001f:  callvirt   instance int32 [sscorlib]System.String::get_Length()",
                @"IL_0024:  call       instance void RealScript.Class1::Foo(int32)",
                @".line 401,401 : 9,10 ''",
                @"IL_0029:  ret",
            }
        };
        #endregion

        #region SimpleIfElseTest
        /// <summary>
        /// public void SimpleIfElseTest(int i)
        /// {
        ///     string returnValue = "tmp";
        ///
        ///     if (i == 10)
        ///     {
        ///         returnValue = "tmp" + "Foo";
        ///     }
        ///     else
        ///     {
        ///         returnValue = "tmp" + "bar";
        ///     }
        ///
        ///     returnValue = returnValue + "bar";
        ///
        ///     Foo(returnValue.Length);
        /// }
        /// </summary>
        public static readonly FunctionParts SimpleIfElseTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance void SimpleIfElseTest(int32 i) cil managed",
            Body = new string[]
            {
                @".maxstack  2",
                @".locals init ([0] string returnValue)",
                @".line 405,405 : 13,40 ''",
                @"  IL_0000:  ldstr      ""tmp""",
                @"IL_0005:  stloc.0",
                @".line 407,407 : 13,25 ''",
                @"  IL_0006:  ldarg.1",
                @"  IL_0007:  ldc.i4.s   10",
                @"IL_0009:  bne.un.s   IL_0013",
                @"",
                @".line 409,409 : 17,45 ''",
                @"  IL_000b:  ldstr      ""tmpFoo""",
                @"IL_0010:  stloc.0",
                @"IL_0011:  br.s       IL_0019",
                @"",
                @".line 413,413 : 17,45 ''",
                @"  IL_0013:  ldstr      ""tmpbar""",
                @"IL_0018:  stloc.0",
                @".line 416,416 : 13,47 ''",
                @"      IL_0019:  ldloc.0",
                @"      IL_001a:  ldstr      ""bar""",
                @"  IL_001f:  call       string [sscorlib]System.String::Concat(string, string)",
                @"IL_0024:  stloc.0",
                @".line 418,418 : 13,37 ''",
                @"      IL_0025:  ldarg.0",
                @"      IL_0026:  ldloc.0",
                @"  IL_0027:  callvirt   instance int32 [sscorlib]System.String::get_Length()",
                @"IL_002c:  call       instance void RealScript.Class1::Foo(int32)",
                @".line 419,419 : 9,10 ''",
                @"IL_0031:  ret",
            },
        };
        #endregion

        #region SimpleIfElseIfTest
        /// <summary>
        /// public int SimpleIfElseIfTest(int i)
        /// {
        ///     string returnValue = "tmp";
        ///
        ///     if (i == 10)
        ///     {
        ///         returnValue = "tmp" + "Foo";
        ///     }
        ///     else if (i == 20)
        ///     {
        ///         returnValue = "tmp" + "bar";
        ///     }
        ///     else
        ///     {
        ///         returnValue = returnValue + "bar";
        ///     }
        ///
        ///     return returnValue.Length;
        /// }
        /// </summary>
        public static readonly FunctionParts SimpleIfElseIfTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 SimpleIfElseIfTest(int32 i) cil managed",
            Body = new string[]
            {
                @".maxstack  2",
                @".locals init ([0] string returnValue)",
                @".line 423,423 : 13,40 ''",
                @"  IL_0000:  ldstr      ""tmp""",
                @"IL_0005:  stloc.0",
                @".line 425,425 : 13,25 ''",
                @"  IL_0006:  ldarg.1",
                @"  IL_0007:  ldc.i4.s   10",
                @"IL_0009:  bne.un.s   IL_0013",
                @"",
                @".line 427,427 : 17,45 ''",
                @"  IL_000b:  ldstr      ""tmpFoo""",
                @"IL_0010:  stloc.0",
                @"IL_0011:  br.s       IL_002c",
                @"",
                @".line 429,429 : 18,30 ''",
                @"  IL_0013:  ldarg.1",
                @"  IL_0014:  ldc.i4.s   20",
                @"IL_0016:  bne.un.s   IL_0020",
                @"",
                @".line 431,431 : 17,45 ''",
                @"  IL_0018:  ldstr      ""tmpbar""",
                @"IL_001d:  stloc.0",
                @"IL_001e:  br.s       IL_002c",
                @"",
                @".line 435,435 : 17,51 ''",
                @"      IL_0020:  ldloc.0",
                @"      IL_0021:  ldstr      ""bar""",
                @"  IL_0026:  call       string [sscorlib]System.String::Concat(string, string)",
                @"IL_002b:  stloc.0",
                @".line 438,438 : 13,39 ''",
                @"      IL_002c:  ldloc.0",
                @"  IL_002d:  callvirt   instance int32 [sscorlib]System.String::get_Length()",
                @"IL_0032:  ret",
            },
        };
        #endregion

        #region NestedIfElseTest
        /// <summary>
        /// public int NestedIfElseTest(int i)
        /// {
        ///     int returnValue = 0;
        ///
        ///     if (i > 10)
        ///     {
        ///         returnValue += 10;
        ///
        ///         if (i == 15)
        ///         {
        ///             returnValue += 5;
        ///         }
        ///         else
        ///         {
        ///             returnValue += 2;
        ///         }
        ///     }
        ///     else
        ///     {
        ///         returnValue = 11;
        ///     }
        ///
        ///     return returnValue;
        /// }
        /// </summary>
        public static readonly FunctionParts NestedIfElseTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 NestedIfElseTest(int32 i) cil managed",
            Body = new string[]
            {
                @".maxstack  2",
                @".locals init ([0] int32 returnValue)",
                @".line 443,443 : 13,33 ''",
                @"  IL_0000:  ldc.i4.0",
                @"IL_0001:  stloc.0",
                @".line 445,445 : 13,24 ''",
                @"  IL_0002:  ldarg.1",
                @"  IL_0003:  ldc.i4.s   10",
                @"IL_0005:  ble.s      IL_001d",
                @"",
                @".line 447,447 : 17,35 ''",
                @"      IL_0007:  ldloc.0",
                @"      IL_0008:  ldc.i4.s   10",
                @"  IL_000a:  add",
                @"IL_000b:  stloc.0",
                @".line 449,449 : 17,29 ''",
                @"  IL_000c:  ldarg.1",
                @"  IL_000d:  ldc.i4.s   15",
                @"IL_000f:  bne.un.s   IL_0017",
                @"",
                @".line 451,451 : 21,38 ''",
                @"      IL_0011:  ldloc.0",
                @"      IL_0012:  ldc.i4.5",
                @"  IL_0013:  add",
                @"IL_0014:  stloc.0",
                @"IL_0015:  br.s       IL_0020",
                @"",
                @".line 455,455 : 21,38 ''",
                @"      IL_0017:  ldloc.0",
                @"      IL_0018:  ldc.i4.2",
                @"  IL_0019:  add",
                @"IL_001a:  stloc.0",
                @"IL_001b:  br.s       IL_0020",
                @"",
                @".line 460,460 : 17,34 ''",
                @"  IL_001d:  ldc.i4.s   11",
                @"IL_001f:  stloc.0",
                @".line 463,463 : 13,32 ''",
                @"  IL_0020:  ldloc.0",
                @"IL_0021:  ret",
            },
        };
        #endregion

        #region WhileLoopBreakTest
        /// <summary>
        /// public int WhileLoopBreak(int i, int j)
        /// {
        ///     j += i;
        ///
        ///     do
        ///     {
        ///         i += j;
        ///
        ///         if (i % 10 == 0)
        ///         {
        ///             j++;
        ///
        ///             if (j == 10)
        ///             {
        ///                 break;
        ///             }
        ///         }
        ///
        ///     } while (i &lt; 100);
        ///
        ///     return j;
        /// }
        /// </summary>
        public static readonly FunctionParts WhileLoopBreakTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 WhileLoopBreak(int32 i, int32 j) cil managed",
            Body = new string[]
            {
                @".maxstack  8",
                @".line 391,391 : 13,20 ''",
                @"      IL_0000:  ldarg.2",
                @"      IL_0001:  ldarg.1",
                @"  IL_0002:  add",
                @"IL_0003:  starg.s    j",
                @".line 395,395 : 17,24 ''",
                @"      IL_0005:  ldarg.1",
                @"      IL_0006:  ldarg.2",
                @"  IL_0007:  add",
                @"IL_0008:  starg.s    i",
                @".line 397,397 : 17,33 ''",
                @"      IL_000a:  ldarg.1",
                @"      IL_000b:  ldc.i4.s   10",
                @"  IL_000d:  rem",
                @"IL_000e:  brtrue.s   IL_001a",
                @"",
                @".line 399,399 : 21,25 ''",
                @"      IL_0010:  ldarg.2",
                @"      IL_0011:  ldc.i4.1",
                @"  IL_0012:  add",
                @"IL_0013:  starg.s    j",
                @".line 401,401 : 21,33 ''",
                @"  IL_0015:  ldarg.2",
                @"  IL_0016:  ldc.i4.s   10",
                @"IL_0018:  beq.s      IL_001f",
                @"",
                @".line 407,407 : 15,31 ''",
                @"  IL_001a:  ldarg.1",
                @"  IL_001b:  ldc.i4.s   100",
                @"IL_001d:  blt.s      IL_0005",
                @"",
                @".line 409,409 : 13,22 ''",
                @"  IL_001f:  ldarg.2",
                @"IL_0020:  ret",
            },
        };
        #endregion

        #region WhileLoopContinueToIfTest
        /// <summary>
        /// public int WhileLoopContinueToIfTest(int i, int j)
        /// {
        ///     j += i;
        /// 
        ///     do
        ///     {
        ///         i += j;
        /// 
        ///         if (i % 10 == 0)
        ///         {
        ///             j++;
        /// 
        ///             if (j == 10)
        ///             {
        ///                 continue;
        ///             }
        /// 
        ///             i++;
        ///         }
        /// 
        ///     } while (i &lt; 100);
        /// 
        ///     return j;
        /// }
        /// </summary>
        public static readonly FunctionParts WhileLoopContinueToIfTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 WhileLoopContinueToIfTest(int32 i, int32 j) cil managed",
            Body = new string[]
            {
                @".maxstack  8",
                @".line 414,414 : 13,20 ''",
                @"      IL_0000:  ldarg.2",
                @"      IL_0001:  ldarg.1",
                @"  IL_0002:  add",
                @"IL_0003:  starg.s    j",
                @".line 418,418 : 17,24 ''",
                @"      IL_0005:  ldarg.1",
                @"      IL_0006:  ldarg.2",
                @"  IL_0007:  add",
                @"IL_0008:  starg.s    i",
                @".line 420,420 : 17,33 ''",
                @"      IL_000a:  ldarg.1",
                @"      IL_000b:  ldc.i4.s   10",
                @"  IL_000d:  rem",
                @"IL_000e:  brtrue.s   IL_001f",
                @"",
                @".line 422,422 : 21,25 ''",
                @"      IL_0010:  ldarg.2",
                @"      IL_0011:  ldc.i4.1",
                @"  IL_0012:  add",
                @"IL_0013:  starg.s    j",
                @".line 424,424 : 21,33 ''",
                @"  IL_0015:  ldarg.2",
                @"  IL_0016:  ldc.i4.s   10",
                @"IL_0018:  beq.s      IL_001f",
                @"",
                @".line 429,429 : 21,25 ''",
                @"      IL_001a:  ldarg.1",
                @"      IL_001b:  ldc.i4.1",
                @"  IL_001c:  add",
                @"IL_001d:  starg.s    i",
                @".line 432,432 : 15,31 ''",
                @"  IL_001f:  ldarg.1",
                @"  IL_0020:  ldc.i4.s   100",
                @"IL_0022:  blt.s      IL_0005",
                @"",
                @".line 434,434 : 13,22 ''",
                @"  IL_0024:  ldarg.2",
                @"IL_0025:  ret",
            },
        };
        #endregion

        #region WhileLoopContinue
        /// <summary>
        /// public int WhileLoopContinueToIfTest(int i, int j)
        /// {
        ///     j += i;
        /// 
        ///     do
        ///     {
        ///         i += j;
        /// 
        ///         if (i % 10 == 0)
        ///         {
        ///             j++;
        /// 
        ///             if (j == 10)
        ///             {
        ///                 continue;
        ///             }
        /// 
        ///             i++;
        ///         }
        ///         j++;
        /// 
        ///     } while (i &lt; 100);
        /// 
        ///     return j;
        /// }
        /// </summary>
        public static readonly FunctionParts WhileLoopContinueTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 WhileLoopContinue(int32 i, int32 j) cil managed",
            Body = new string[]
            {
                @".maxstack  8",
                @".line 414,414 : 13,20 ''",
                @"      IL_0000:  ldarg.2",
                @"      IL_0001:  ldarg.1",
                @"  IL_0002:  add",
                @"IL_0003:  starg.s    j",
                @".line 418,418 : 17,24 ''",
                @"      IL_0005:  ldarg.1",
                @"      IL_0006:  ldarg.2",
                @"  IL_0007:  add",
                @"IL_0008:  starg.s    i",
                @".line 420,420 : 17,33 ''",
                @"      IL_000a:  ldarg.1",
                @"      IL_000b:  ldc.i4.s   10",
                @"  IL_000d:  rem",
                @"IL_000e:  brtrue.s   IL_001f",
                @"",
                @".line 422,422 : 21,25 ''",
                @"      IL_0010:  ldarg.2",
                @"      IL_0011:  ldc.i4.1",
                @"  IL_0012:  add",
                @"IL_0013:  starg.s    j",
                @".line 424,424 : 21,33 ''",
                @"  IL_0015:  ldarg.2",
                @"  IL_0016:  ldc.i4.s   10",
                @"IL_0018:  beq.s      IL_0024",
                @"",
                @".line 429,429 : 21,25 ''",
                @"      IL_001a:  ldarg.1",
                @"      IL_001b:  ldc.i4.1",
                @"  IL_001c:  add",
                @"IL_001d:  starg.s    i",
                @".line 432,432 : 17,21 ''",
                @"      IL_001f:  ldarg.2",
                @"      IL_0020:  ldc.i4.1",
                @"  IL_0021:  sub",
                @"IL_0022:  starg.s    j",
                @".line 434,434 : 15,31 ''",
                @"  IL_0024:  ldarg.1",
                @"  IL_0025:  ldc.i4.s   100",
                @"IL_0027:  blt.s      IL_0005",
                @"",
                @".line 436,436 : 13,22 ''",
                @"  IL_0029:  ldarg.2",
                @"IL_002a:  ret",
            },
        };
        #endregion

        #region ForLoopBreakTest
        /// <summary>
        /// public int ForLoopBreak(int i, int j)
        /// {
        ///     j += i;
        ///
        ///     for(;i &lt; 100; i++)
        ///     {
        ///         i += j;
        ///
        ///         if (i % 10 == 0)
        ///         {
        ///             j++;
        ///
        ///             if (j == 10)
        ///             {
        ///                 break;
        ///             }
        ///         }
        ///     }
        ///
        ///     return j;
        /// }
        /// </summary>
        public static readonly FunctionParts ForLoopBreakTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 ForLoopBreak(int32 i, int32 j) cil managed",
            Body = new string[]
            {
                @".maxstack  8",
                @".line 441,441 : 13,20 ''",
                @"      IL_0000:  ldarg.2",
                @"      IL_0001:  ldarg.1",
                @"  IL_0002:  add",
                @"  IL_0003:  starg.s    j",
                @"IL_0005:  br.s       IL_0021",
                @"",
                @".line 445,445 : 17,24 ''",
                @"      IL_0007:  ldarg.1",
                @"      IL_0008:  ldarg.2",
                @"  IL_0009:  add",
                @"IL_000a:  starg.s    i",
                @".line 447,447 : 17,33 ''",
                @"      IL_000c:  ldarg.1",
                @"      IL_000d:  ldc.i4.s   10",
                @"  IL_000f:  rem",
                @"IL_0010:  brtrue.s   IL_001c",
                @"",
                @".line 449,449 : 21,25 ''",
                @"      IL_0012:  ldarg.2",
                @"      IL_0013:  ldc.i4.1",
                @"  IL_0014:  add",
                @"IL_0015:  starg.s    j",
                @".line 451,451 : 21,33 ''",
                @"  IL_0017:  ldarg.2",
                @"  IL_0018:  ldc.i4.s   10",
                @"IL_001a:  beq.s      IL_0026",
                @"",
                @".line 443,443 : 27,30 ''",
                @"      IL_001c:  ldarg.1",
                @"      IL_001d:  ldc.i4.1",
                @"  IL_001e:  add",
                @"IL_001f:  starg.s    i",
                @".line 443,443 : 18,25 ''",
                @"  IL_0021:  ldarg.1",
                @"  IL_0022:  ldc.i4.s   100",
                @"IL_0024:  blt.s      IL_0007",
                @"",
                @".line 458,458 : 13,22 ''",
                @"  IL_0026:  ldarg.2",
                @"IL_0027:  ret",
            },
        };
        #endregion

        #region ForLoopContinueTest
        /// <summary>
        /// public int ForLoopContinue(int i, int j)
        /// {
        ///     j += i;
        /// 
        ///     for(;i &lt; 100; i++)
        ///     {
        ///         i += j;
        /// 
        ///         if (i % 10 == 0)
        ///         {
        ///             j++;
        /// 
        ///             if (j == 10)
        ///             {
        ///                 continue;
        ///             }
        /// 
        ///             i++;
        ///         }
        /// 
        ///         j--;
        ///     }
        /// 
        ///     return j;
        /// }
        /// </summary>
        public static readonly FunctionParts ForLoopContinueTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 ForLoopContinue(int32 i, int32 j) cil managed",
            Body = new string[]
            {
                @".maxstack  8",
                @".line 463,463 : 13,20 ''",
                @"      IL_0000:  ldarg.2",
                @"      IL_0001:  ldarg.1",
                @"  IL_0002:  add",
                @"IL_0003:  starg.s    j",
                @"IL_0005:  br.s       IL_002b",
                @"",
                @".line 467,467 : 17,24 ''",
                @"      IL_0007:  ldarg.1",
                @"      IL_0008:  ldarg.2",
                @"  IL_0009:  add",
                @"IL_000a:  starg.s    i",
                @".line 469,469 : 17,33 ''",
                @"      IL_000c:  ldarg.1",
                @"      IL_000d:  ldc.i4.s   10",
                @"  IL_000f:  rem",
                @"IL_0010:  brtrue.s   IL_0021",
                @"",
                @".line 471,471 : 21,25 ''",
                @"      IL_0012:  ldarg.2",
                @"      IL_0013:  ldc.i4.1",
                @"  IL_0014:  add",
                @"IL_0015:  starg.s    j",
                @".line 473,473 : 21,33 ''",
                @"  IL_0017:  ldarg.2",
                @"  IL_0018:  ldc.i4.s   10",
                @"IL_001a:  beq.s      IL_0026",
                @"",
                @".line 478,478 : 21,25 ''",
                @"      IL_001c:  ldarg.1",
                @"      IL_001d:  ldc.i4.1",
                @"  IL_001e:  add",
                @"IL_001f:  starg.s    i",
                @".line 481,481 : 17,21 ''",
                @"      IL_0021:  ldarg.2",
                @"      IL_0022:  ldc.i4.1",
                @"  IL_0023:  sub",
                @"IL_0024:  starg.s    j",
                @".line 465,465 : 27,30 ''",
                @"      IL_0026:  ldarg.1",
                @"      IL_0027:  ldc.i4.1",
                @"  IL_0028:  add",
                @"IL_0029:  starg.s    i",
                @".line 465,465 : 18,25 ''",
                @"  IL_002b:  ldarg.1",
                @"  IL_002c:  ldc.i4.s   100",
                @"IL_002e:  blt.s      IL_0007",
                @"",
                @".line 484,484 : 13,22 ''",
                @"  IL_0030:  ldarg.2",
                @"IL_0031:  ret",
            },
        };
        #endregion

        #region ForLoopContinueWoLineNumTest
        /// <summary>
        /// public int ForLoopContinue(int i, int j)
        /// {
        ///     j += i;
        /// 
        ///     for(;i &lt; 100; i++)
        ///     {
        ///         i += j;
        /// 
        ///         if (i % 10 == 0)
        ///         {
        ///             j++;
        /// 
        ///             if (j == 10)
        ///             {
        ///                 continue;
        ///             }
        /// 
        ///             i++;
        ///         }
        /// 
        ///         j--;
        ///     }
        /// 
        ///     return j;
        /// }
        /// </summary>
        public static readonly FunctionParts ForLoopContinueWoLineNumTest = new FunctionParts
        {
            Decleration = @".method public hidebysig instance int32 ForLoopContinue(int32 i, int32 j) cil managed",
            Body = new string[]
            {
                @".maxstack  8",
                @"      IL_0000:  ldarg.2",
                @"      IL_0001:  ldarg.1",
                @"  IL_0002:  add",
                @"IL_0003:  starg.s    j",
                @"IL_0005:  br.s       IL_002b",
                @"",
                @"      IL_0007:  ldarg.1",
                @"      IL_0008:  ldarg.2",
                @"  IL_0009:  add",
                @"IL_000a:  starg.s    i",
                @"      IL_000c:  ldarg.1",
                @"      IL_000d:  ldc.i4.s   10",
                @"  IL_000f:  rem",
                @"IL_0010:  brtrue.s   IL_0021",
                @"",
                @"      IL_0012:  ldarg.2",
                @"      IL_0013:  ldc.i4.1",
                @"  IL_0014:  add",
                @"IL_0015:  starg.s    j",
                @"  IL_0017:  ldarg.2",
                @"  IL_0018:  ldc.i4.s   10",
                @"IL_001a:  beq.s      IL_0026",
                @"",
                @"      IL_001c:  ldarg.1",
                @"      IL_001d:  ldc.i4.1",
                @"  IL_001e:  add",
                @"IL_001f:  starg.s    i",
                @"      IL_0021:  ldarg.2",
                @"      IL_0022:  ldc.i4.1",
                @"  IL_0023:  sub",
                @"IL_0024:  starg.s    j",
                @"      IL_0026:  ldarg.1",
                @"      IL_0027:  ldc.i4.1",
                @"  IL_0028:  add",
                @"IL_0029:  starg.s    i",
                @"  IL_002b:  ldarg.1",
                @"  IL_002c:  ldc.i4.s   100",
                @"IL_002e:  blt.s      IL_0007",
                @"",
                @"  IL_0030:  ldarg.2",
                @"IL_0031:  ret",
            },
        };
        #endregion

#if Temp
        #region Template
        public static readonly FunctionParts Template = new FunctionParts
        {
            Decleration = @"",
            Body = new string[]
            {
            },
        };
        #endregion
#endif
    }
}
