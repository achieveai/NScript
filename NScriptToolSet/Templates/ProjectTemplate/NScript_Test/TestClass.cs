//-----------------------------------------------------------------------
// <copyright file="TestClass.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace $safeprojectname$
{
    using System;
    using SunlightUnit;

    /// <summary>
    /// Definition for TestClass
    /// </summary>
    [TestFixture]
    public class TestClass
    {
        [TestSetup]
        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        /// </summary>
        public static void Setup()
        {
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void Test()
        {
            Assert.IsTrue(true, "true should be true");
        }
    }
}

