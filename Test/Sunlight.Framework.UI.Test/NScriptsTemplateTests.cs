//-----------------------------------------------------------------------
// <copyright file="NScriptsTemplateTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;

    /// <summary>
    /// Definition for NScriptsTemplateTests
    /// </summary>
    [TestFixture]
    public class NScriptsTemplateTests
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
            Assert.NotEqual(null, NScriptsTemplatesClass.TestTemplate1, "Template should not be null");
            Assert.IsTrue(true, "true should be true");
        }
    }
}

