using System.Collections.Generic;

namespace RealScript
{
    public class BasicStatements
    {
        public static char ReturnChar()
        {
            return 't';
        }

        public static int ReturnInt()
        {
            return 10;
        }

        public static double ReturnDouble()
        {
            return 10.0;
        }

        public static long ReturnLong()
        {
            return 10L;
        }

        public static long ReturnLargeLong()
        {
            return 0x109A889910L;
        }

        public static float ReturnSingle()
        {
            return 10.0f;
        }

        public static uint ReturnUInt()
        {
            return 10;
        }

        public static uint ReturnLargeUInt()
        {
            return 0xC089A310;
        }

        public static int AddIntArgs(int n1, int n2)
        {
            return n1 + n2;
        }

        public static int AddIntArgToConst(int n1)
        {
            return n1 + 10;
        }

        public static double IntDivideInts(int i, int j)
        {
            return (double)i / (double)j;
        }

        public static int ReturnMethodCall()
        {
            return BasicStatements.ReturnInt();
        }

        public static int ReturnMethodCallWith1Arg()
        {
            return BasicStatements.AddIntArgToConst(10);
        }

        public static TestReferenceClass ReturnNewObject()
        {
            return new TestReferenceClass();
        }

        public static TestReferenceClass ReturnNewObjectWithArg(int n1)
        {
            return new TestReferenceClass(n1);
        }

        public static int[] ReturnIntArray()
        {
            return new int[10];
        }

        public static TestReferenceClass[] ReturnObjectArray()
        {
            return new TestReferenceClass[10];
        }

        public static int ReturnArrayElement(int[] arr, int index)
        {
            return arr[index];
        }

        public static void SetArrayElement(int[] arr, int index, int value)
        {
            arr[index] = value;
        }

        public static bool CheckType(object obj)
        {
            return obj is TestReferenceClass;
        }

        public static TestReferenceClass CastType(object obj)
        {
            return (TestReferenceClass)obj;
        }

        public static TestReferenceClass AsType(object obj)
        {
            return obj as TestReferenceClass;
        }

        public static bool TestSimpleCompare(int i)
        {
            return i == 10;
        }

        public static bool TestSimpleAndComparision(int i)
        {
            return i >= 10 && i <= 14;
        }

        public static bool TestFourPartCondition(int i)
        {
            return (i >= 10 || BasicStatements.TestSimpleAndComparision(i)) && i <= 14 && !BasicStatements.TestSimpleCompare(i);
        }

        public static int TestSimpleConditional(int i)
        {
            return i >= 10 && i <= 14 ? 14 : -1;
        }

        public static TestReferenceClass[] TestInlineArray()
        {
            return new TestReferenceClass[]
            {
                new TestReferenceClass(1),
                new TestReferenceClass(2),
                new TestReferenceClass(3),
                new TestReferenceClass(4),
                new TestReferenceClass(5),
            };
        }

        public static int SetVariableSimple(int i)
        {
            int rv = i + 10;
            BasicStatements.TestSimpleAndComparision(rv);

            return rv;
        }

        public static TestReferenceClass ReturnProperty(TestReferenceClass obj)
        {
            return obj.Property;
        }

        public static void SetProperty(TestReferenceClass obj, int v)
        {
            obj.IntProperty = v;
        }

        public static int TestComplexCondition(int i1, int i2)
        {
            return (i2 == 3 && i1 == 10) &&
                   (i1 == 1 || i2 == 9) &&
                   (i1 == 2 || i2 == 3) ||
                   (i1 == 4 && i2 == 3)
                       ? 10
                       : 14;
        }

        public static int AccessRefArgument(int i1, ref int testOut)
        {
            testOut = i1 + 10 + testOut;

            return i1;
        }

        public static int PassVariableByRef(int i)
        {
            int j = 10;
            return AccessRefArgument(i, ref j) + j;
        }

        public static void PassObjectFieldByRef(TestReferenceClass cl)
        {
            AccessRefArgument(10, ref cl.intField);
        }

        public static void PassArrayElementByRef(int[] intArray)
        {
            AccessRefArgument(10, ref intArray[10]);
        }

        public static IntFunction AddDelegates(IntFunction d1, IntFunction d2)
        {
            return d1 + d2;
        }

        public static void AddAssignDelegates(IntFunction d1, IntFunction d2)
        {
            d1 += d2;
        }

        public static IntFunction SubDelegates(IntFunction d1, IntFunction d2)
        {
            return d1 - d2;
        }

        public static void SubAssignDelegates(IntFunction d1, IntFunction d2)
        {
            d1 -= d2;
        }

        public static void IndexerSet(StringDictionary<int> dict, int num)
        {
            dict["0"] = num;
        }

        public static int IndexerGet(StringDictionary<int> dict)
        {
            return dict["0"];
        }

        public static FlagEnum? RegressionFlagsOrAssign(FlagEnum? en)
        {
            en |= FlagEnum.Two;
            return en;
        }

        public static FlagEnum? RegressionFlagsOr(FlagEnum? en)
        {
            return en | FlagEnum.Two;
        }

        public static FlagEnum? RegressionFlagsOrWithNullable(FlagEnum? en, FlagEnum? other)
        {
            return en | other;
        }

        public static void RegressionFlagsOrAssign2(FlagsEnumOrOperatorRegression obj)
        {
            obj.FlagEnum |= FlagEnum.Two;
        }

        public static void RegressionFlagsOr2(FlagsEnumOrOperatorRegression obj)
        {
            obj.FlagEnum =  obj.FlagEnum | FlagEnum.Two;
        }

        public void TestRefs(int a, int b)
        {
            TestControlFlow.Swap(ref a, ref b);
        }
    }
}