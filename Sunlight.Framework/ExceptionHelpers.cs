//-----------------------------------------------------------------------
// <copyright file="ExceptionHelpers.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ExceptionHelpers
    /// </summary>
    public static class ExceptionHelpers
    {
        public static void ThrowOnOutOfRange(
            int value,
            int minValue,
            int maxValue,
            string argumentName)
        {
            if (value < minValue || value > maxValue)
            {
                throw new Exception("Out of range exception: " + argumentName);
            }
        }

        public static void ThrowOnArgumentNull(
            object value,
            string argumentName)
        {
            if (value == null)
            {
                throw new Exception("ArgumentNull: " + argumentName);
            }
        }
    }
}
