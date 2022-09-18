//-----------------------------------------------------------------------
// <copyright file="Lang7Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for Lang7Features
    /// </summary>
    public static class Lang7Features
    {
        public static void TestStringInterpolation()
        {
            var i = 12;
            var x = $"{i} asd";
        }

        public static (int a, int b) TestTupleReturn(int x, int y)
        {
            return (x, y);
        }

        public static int TestTupleUnfolding(System.ValueTuple<int, int> tup)
        {
            var (x, y) = tup;

            return x + y;
        }

        public static int TestThrowExpression(int? num)
        {
            int rv = num ?? throw new ArgumentNullException("num");
            return rv;
        }

        public static int DefaultLiteralExpression(int? num)
        {
            return num ?? default;
        }

        public static int SumPositiveNumbers(IEnumerable<object> sequence)
        {
            int sum = 0;
            foreach (var i in sequence)
            {
                switch (i)
                {
                    case 0:
                    case 10:
                        break;
                    case IEnumerable<int> childSequence:
                        {
                            foreach (var item in childSequence)
                                sum += (item > 0) ? item : 0;
                            break;
                        }
                    case Number n when n > 0:
                        sum += n;
                        break;
                    case null:
                        throw new ArgumentException("Null found in sequence");
                    default:
                        throw new InvalidProgramException("Unrecognized type");
                }
            }
            return sum;
        }

        public static void TestSwitchPatternMatching_1()
        {
            int [] l = { 1, 2, 3, 90 };
            object age = 90;
            string ageBlock = null;
            switch ((int)age)
            {
                case 50:
                    ageBlock = "the big five-oh";
                    break;
                case var testAge when l.Contains(testAge):
                    ageBlock = "octogenarian";
                    break;
                case var testAge when ((testAge >= 90) & (testAge <= 99)):
                    ageBlock = "nonagenarian";
                    break;
                case var testAge when (testAge >= 100):
                    ageBlock = "centenarian";
                    break;
                default:
                    ageBlock = "just old";
                    break;
            }
        }

        public static void TestSwitchPatternMatching_2()
        {
            object o = "asdf";
            switch (o)
            {
                case var obj when obj.Equals(12):
                    break;
                case string obj when obj.Length < 3:
                    break;
                case string _:
                    break;
                default:
                    throw new InvalidProgramException();
            }
        }
    }
}
