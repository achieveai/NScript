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
        public int PropInt1
        { get; set; }

        public string PropStr1
        { get; set; }

        public bool PropBool1
        { get; set; }

        public TestInterface TestIface
        { get; set; }

        public TestViewModelA TestVMA
        { get; set; }

        public void DomEvent1(Element elem, ElementEvent evt)
        { }

        public void DomEvent2()
        { }
    }

    public class TestViewModelB : TestViewModelA, TestInterface
    {
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
