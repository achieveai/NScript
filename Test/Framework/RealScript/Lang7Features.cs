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
                        break;
                    case IEnumerable<int> childSequence:
                    {
                        foreach(var item in childSequence)
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
    }
}
