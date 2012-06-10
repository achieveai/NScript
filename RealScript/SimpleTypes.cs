//-----------------------------------------------------------------------
// <copyright file="SimpleTypes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for SimpleTypes
    /// </summary>
    public static class SimpleStaticType
    {
        public static int intField;
        public static int GetField()
        {
            return SimpleStaticType.intField;
        }
    }

    /// <summary>
    /// Definition for simple instance type.
    /// </summary>
    public class SimpleInstanceType
    {
        public int intField;
        public int GetField()
        {
            return this.intField;
        }
    }

    /// <summary>
    /// Definition for simple instance type with multiple constructors.
    /// </summary>
    public class MultipleConstructorsType
    {
        public int intField;
        public double doubleField;

        public MultipleConstructorsType(double dbl)
        {
            this.doubleField = dbl;
        }

        public MultipleConstructorsType(int i)
        {
            this.intField = i;
        }
    }

    /// <summary>
    /// Definition for class with same named instance and static methods.
    /// </summary>
    public class SameNameInstanceAndStaticMethod
    {
        public int intFiled;

        public static int GetInt(SameNameInstanceAndStaticMethod c)
        {
            return c.intFiled;
        }

        public int GetInt()
        {
            return this.intFiled;
        }
    }

    /// <summary>
    /// Definition of base class to test basic virtual mapping.
    /// </summary>
    public class VirtualBase
    {
        public virtual int GetInt(int i)
        {
            return i * 2;
        }
    }

    /// <summary>
    /// Definition of class to test basic virtual mapping.
    /// </summary>
    public class VirtualOverride : VirtualBase
    {
        public override int GetInt(int i)
        {
            return i / 2;
        }
    }

    /// <summary>
    /// Test enum creation.
    /// </summary>
    public enum SimpleEnumType
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    /// <summary>
    /// Test class that uses enum.
    /// </summary>
    public class EnumUsingClass
    {
        public const SimpleEnumType TestValue =
            SimpleEnumType.Two;

        public SimpleEnumType TestValue2 =
            SimpleEnumType.One;

        public string GetStr(SimpleEnumType en)
        {
            return en.ToString();
        }

        public string GetStrVoid()
        {
            return this.GetStr(this.TestValue2);
        }
    }

    /// <summary>
    /// Class to test out static constructor.
    /// </summary>
    public static class StaticConstructorType
    {
        private static int tempValue;

        static StaticConstructorType()
        {
            StaticConstructorType.tempValue =
                Class1.GetMoreStatic(10);
        }

        public static int Value
        {
            get { return StaticConstructorType.tempValue; }
        }
    }
}
