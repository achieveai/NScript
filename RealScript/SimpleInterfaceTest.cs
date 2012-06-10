//-----------------------------------------------------------------------
// <copyright file="SimpleInterfaceTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for SimpleInterfaceTest
    /// </summary>
    public interface SimpleInterface
    {
        int GetInt(int i);
    }

    /// <summary>
    /// Overrides simpleInterface, but not using virtual method.
    /// </summary>
    public class InheritInterface : SimpleInterface
    {
        public virtual int GetInt(int i)
        {
            return i + 2;
        }
    }

    public class SecondOrderInterfaceInherit : InheritInterface
    {
        public override int GetInt(int i)
        {
            return i + 3;
        }
    }

    /// <summary>
    /// Testing if we get proper overrides in double layered interfaces.
    /// </summary>
    public interface SimpleInheritedInterface : SimpleInterface
    {
        string GetStr(int i);
    }

    /// <summary>
    /// Check if we can map both the function to proper interfaces.
    /// </summary>
    public class InheritDerivedInterface : SimpleInheritedInterface
    {
        public int GetInt(int i)
        {
            return i + 2;
        }

        public virtual string GetStr(int i)
        {
            return i.ToString();
        }
    }
}
