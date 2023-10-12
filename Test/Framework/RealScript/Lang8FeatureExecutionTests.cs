using System;
using System.Collections.Generic;

namespace RealScript
{
    public class Lang8FeatureExecutionTests
    {
        public static async void Main()
        {
            TestIsExpression();
            TestSwitchExpression();
            TestNullCoalescingAssignment();
            TestNullableReferenceTypes();
            await TestAsyncForEach();
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

            Console.WriteLine(WeekStr(DayOfWeek.Monday));
            Console.WriteLine(WeekStr(DayOfWeek.Tuesday));
            Console.WriteLine(WeekStr(DayOfWeek.Others));

            Console.WriteLine(DayOfWeekFromString(WeekStr(DayOfWeek.Monday)));
            Console.WriteLine(DayOfWeekFromString(WeekStr(DayOfWeek.Tuesday)));
            Console.WriteLine(DayOfWeekFromString(WeekStr(DayOfWeek.Others)));

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

            static string WeekStr(DayOfWeek day)
                => day switch
                {
                    DayOfWeek.Monday => "Monday",
                    DayOfWeek.Tuesday => "Tuesday",
                    DayOfWeek.Wednesday => "Wednesday",
                    _ => "Other Days"
                };

            static DayOfWeek DayOfWeekFromString(string str)
                => str switch
                {
                    "Monday" => DayOfWeek.Monday,
                    "Tuesday" => DayOfWeek.Tuesday,
                    "Wednesday" => DayOfWeek.Wednesday,
                    _ => DayOfWeek.Others
                };
        }

        private static void TestNullCoalescingAssignment()
        {
            BaseClass1 b = null;
            b ??= new BaseClass1 { X = 88 };

            Console.WriteLine(b.X);

            (int, int)? tupl = null;
            tupl ??= (3, 4);

            BaseClass2 b2 = new BaseClass2();
            b2.Derived ??= new Derived { X = 42 };

            Console.WriteLine(b2.Derived.X);

            Console.WriteLine(tupl.Value.Item1);
            Console.WriteLine(tupl.Value.Item2);
        }

        private static void TestNullableReferenceTypes()
        {
            BaseClass1? b = null;
            b = new BaseClass1();

            BaseClass2 b2 = null;
        }

        private static async Promise TestAsyncForEach()
        {
            await foreach (var item in GetIntsAsync())
            {
                Console.WriteLine(item);
            }

            foreach (var item in GetInts())
            {
                Console.WriteLine(item);
            }
        }

        private static async IAsyncEnumerable<int> GetIntsAsync()
        {
            var l = new System.Collections.Generic.List<int>() { 1, 2, 3 };

            foreach (var item in l)
            {
                yield return item;
                await Utilities.Delay(0);
            }
        }

        private static IEnumerable<int> GetInts()
        {
            yield return 1;
            yield return 2;
        }

        private class BaseClass1
        {
            public int X { get; set; }
        }

        private class BaseClass2
        {
            public Derived Derived { get; set; }
        }

        private class Derived : BaseClass1 { }

        private enum DayOfWeek
        {
            Monday,
            Tuesday,
            Wednesday,
            Others
        }
    }
}
