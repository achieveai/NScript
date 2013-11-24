//-----------------------------------------------------------------------
// <copyright file="TestViewModels.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Observables;
    using System;
    using System.Web.Html;

    /// <summary>
    /// Definition for TestViewModels
    /// </summary>
    public class TestViewModelA : ObservableObject
    {
        private string str1;
        private bool bool1;
        private TestViewModelA testVMA;
        public int PropInt1
        {
            get
            {
                return this.int1;
            }
            set
            {
                if (this.int1 != value)
                {
                    this.int1 = value;
                    base.FirePropertyChanged("PropInt1");
                }
            }
        }

        public string PropStr1
        {
            get
            {
                return this.str1;
            }
            set
            {
                if (this.str1 != value)
                {
                    this.str1 = value;
                    base.FirePropertyChanged("PropStr1");
                }
            }
        }

        public bool PropBool1
        {
            get
            {
                return this.bool1;
            }
            set
            {
                if (this.bool1 != value)
                {
                    this.bool1 = value;
                    base.FirePropertyChanged("PropBool1");
                }
            }
        }

        public TestInterface TestIface
        {
            get;
            set;
        }

        public TestViewModelA TestVMA
        {
            get
            {
                return this.testVMA;
            }
            set
            {
                if (this.testVMA != value)
                {
                    this.testVMA = value;
                    base.FirePropertyChanged("TestVMA");
                }
            }
        }

        public void DomEvent1(Element elem, ElementEvent evt)
        { }

        public void DomEvent2()
        { }

        public int int1 { get; set; }

        public static object PropStr1Getter(object obj)
        {
            return ((TestViewModelA)obj).PropStr1;
        }

        public static void PropStr1Setter(object obj, object val)
        {
            ((TestViewModelA)obj).PropStr1 = (string)val;
        }

        public static object PropInt1Getter(object obj)
        {
            return ((TestViewModelA)obj).PropInt1;
        }

        public static void PropInt1Setter(object obj, object val)
        {
            ((TestViewModelA)obj).PropInt1 = (int)val;
        }

        public static object PropBool1Getter(object obj)
        {
            return ((TestViewModelA)obj).PropBool1;
        }

        public static void PropBool1Setter(object obj, object val)
        {
            ((TestViewModelA)obj).PropBool1 = (bool)val;
        }

        public static object TestVMAGetter(object obj)
        {
            return ((TestViewModelA)obj).TestVMA;
        }

        public static void TestVMASetter(object obj, object val)
        {
            ((TestViewModelA)obj).TestVMA = (TestViewModelA)val;
        }
    }

    public class TestViewModelB : TestViewModelA, TestInterface
    {
        public static TestViewModelB StaticProp
        { get; set; }

        public static string StaticStrProp
        { get; set; }

        public int PropInt2
        { get; set; }

        public string PropStr2
        { get; set; }

        public string PropStrB
        { get; set; }
    }

    public class TestViewModelC : TestViewModelA
    {
        public int PropInt3
        { get; set; }

        public string PropStr3
        { get; set; }
    }

    public interface TestInterface
    {
        int PropInt2
        { get; set; }

        string PropStr2
        { get; set; }
    }
}
