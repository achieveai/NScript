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
    /// Definition for Attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AsyncTestFixtureAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for TestSetup Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestSetupAttribute : Attribute
    {
    }

    /// <summary>
    /// Definition for TestAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
    }
}
