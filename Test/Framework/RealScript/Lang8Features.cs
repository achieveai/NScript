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
    }
}
