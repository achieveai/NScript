//-----------------------------------------------------------------------
// <copyright file="EnumTypes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for EnumTypes
    /// </summary>
    [Flags]
    public enum FlagEnum
    {
        One = 1,
        Two = 2,
        Four = 4,
        Eight = 8
    }

    public enum NormalEnum
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }
}
