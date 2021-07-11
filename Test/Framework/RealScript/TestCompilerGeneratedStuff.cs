//-----------------------------------------------------------------------
// <copyright file="TestCompilerGeneratedStuff.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for TestCompilerGeneratedStuff
    /// </summary>
    public class TestCompilerGeneratedStuff
    {
        public int IntProperty
        {
            get;
            set;
        }

        public event Action<int> testEvent;

        public void AccessIntProperty(int value)
        {
            if (value < this.IntProperty)
            {
                this.IntProperty = value;
            }
        }

        public void TestEventCheck(Action<int> del)
        {
            if (this.IntProperty == 1)
            {
                this.testEvent -= del;
            }
            else
            {
                this.testEvent += del;
            }
        }

        public void TestCallEvent(int i)
        {
            if (this.testEvent != null)
            {
                this.testEvent(i);
            }
        }
    }
}