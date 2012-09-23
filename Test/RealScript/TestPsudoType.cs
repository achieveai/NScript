//-----------------------------------------------------------------------
// <copyright file="TestPsudoType.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TestPsudoType
    /// </summary>
    [PsudoType]
    public class TestPsudoType
    {
        [IntrinsicField]
        public readonly int TempI;

        [IntrinsicField]
        public readonly int TempJ;
    }
}
