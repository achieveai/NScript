//-----------------------------------------------------------------------
// <copyright file="Attributes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SunlightUnit
{
    using System;

    /// <summary>
    /// Definition for Attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TestFixtureAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for Test suite setup Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestSetupAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for Test case setup Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestCaseSetupAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for TestAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for Test Suite Teardown Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestTearDownAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for Test Case Teardown Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestCaseTearDownAttribute : Attribute
    {
    }
}
