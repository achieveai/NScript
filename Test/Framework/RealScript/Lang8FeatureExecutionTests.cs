using System;

namespace RealScript
{
    public class Lang8FeatureExecutionTests
    {
        public static void Main()
        {
            TestIsExpression();
            TestSwitchExpression();
            TestNullableReferenceTypes();
        }

        private static void TestIsExpression()
        {
            if (MethodReturningInt() is var x)
            {
                Console.WriteLine(x.ToString());
            }

            if (new Derived() is BaseClass1)
            {
                Console.WriteLine("Derived is BaseClass");
            }

            if (new BaseClass2() as object is BaseClass1)
            {
                Console.WriteLine("BaseClass2 is BaseClass1");
            }


            static int MethodReturningInt() => 0;
        }

        private static void TestSwitchExpression()
        {
            Console.WriteLine(NumberConverter(1));
            Console.WriteLine(NumberConverter(2));
            Console.WriteLine(NumberConverter(4));
            Console.WriteLine(NumberConverter(100));

            Console.WriteLine(GetOperation(DayOfWeek.Monday)(23, 45));
            Console.WriteLine(GetOperation(DayOfWeek.Tuesday)(23, 45));
            Console.WriteLine(GetOperation(DayOfWeek.Wednesday)(23, 45));

            static string NumberConverter(int num)
            {
                return num switch
                {
                    1 => "one",
                    2 => "two",
                    _ => "gt5"
                };
            }

            static Func<int, int, int> GetOperation(DayOfWeek day)
            {
                return day switch
                {
                    DayOfWeek.Monday => (l, r) => l * r,
                    DayOfWeek.Tuesday => (l, r) => l + r,
                    _ => (l, r) => l - r
                };
            }
        }

        private static void TestNullableReferenceTypes()
        {
            BaseClass1? b = null;
            b = new BaseClass1();

            BaseClass2 b2 = null;
        }

        private class BaseClass1
        {
            public int X { get; set; }
        }

        private class BaseClass2 { }

        private class Derived : BaseClass1 { }

        private enum DayOfWeek
        {
            Monday,
            Tuesday,
            Wednesday,
        }
    }
}
