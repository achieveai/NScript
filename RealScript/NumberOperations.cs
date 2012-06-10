//-----------------------------------------------------------------------
// <copyright file="NumberOperations.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for NumberOperations
    /// </summary>
    public static class NumberOperations
    {
        public static int Div(int x, int y)
        {
            return x / y;
        }

        public static int DoubleDivide(int x, double y)
        {
            return (int)(x / y);
        }

        public static int Tan(int angle)
        {
            return (int)Math.Tan(angle);
        }

        public static int PassDoubleAsIntArg(double doub)
        {
            return NumberOperations.Tan((int)doub);
        }
    }
}
