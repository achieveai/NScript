//-----------------------------------------------------------------------
// <copyright file="Lan8Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

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

        public static void IsPatternExpression()
        {
            var x = 90;
            var t = x is 900;
            var asdf = x is int z;

            if (x is 900)
            {
            }
            else if (x is int y)
            {

            }
        }
    }
}
