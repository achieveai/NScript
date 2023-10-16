//-----------------------------------------------------------------------
// <copyright file="Lan8Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections.Generic;

    /// <summary>
    /// Definition for Lan8Features
    /// </summary>
    public class Lang8Features
    {
        public static void NullCoalescingAssignment()
        {
            (int, int)? tupl = null;
            tupl ??= (1, 2);

            _ = (tupl ??= (3, 4)).Item1;
        }

        public static void IsVarPattern()
        {
            var isNonZero = GetInstance() is var x && x.X != 0;

            static BaseClass GetInstance() =>
                new BaseClass { X = 90 };
        }

        public static void IsConstantPattern()
        {
            var x = 90;
            var t = x is 900;
        }

        public static void IsDeclarationPattern()
        {
            BaseClass b = new SubClass();

            var isZero = b is SubClass sb && sb.X == 0;

            var isSubClass = b is SubClass;
        }

        public static void SwitchExpression()
        {
            var x = 90;
            var y = (x) switch
            {
                100 => 200,
                90 => 200,
                _ => 900
            };

            var str = x switch
            {
                1 => "1",
                2 => "2",
                _ => "10"
            };
        }

        public static async void TestAsyncForEach()
        {
            await foreach (var item in GetIntsAsync())
            {
                Console.WriteLine(item);
            }
        }

        public static async IAsyncEnumerable<int> GetIntsAsync()
        {
            await Utilities.Delay(0);
            yield return 1200;
        }

        private class BaseClass
        {
            public int X { get; set; }
        }

        private class SubClass : BaseClass
        {
            public new int X { get; set; }
        }
    }
}
