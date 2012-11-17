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
    public class TestPsudoType
    {
        private string tmpStr;

        public int? TempI
        { get; set; }

        public extern int TempJ
        { get; }

        public extern int[] TheArray
        { get; set; }

        public extern event Action<int, float> TestEvent;

        public void WorkOnString()
        {
            if (this.tmpStr == null)
            {
                this.tmpStr = "";
            }
            else
            {
                this.tmpStr = this.tmpStr + this.tmpStr.Length;
            }
        }
    }
}
