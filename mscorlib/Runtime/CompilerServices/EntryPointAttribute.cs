//-----------------------------------------------------------------------
// <copyright file="EntryPointAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for EntryPointAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false), NonScriptable]
    public class EntryPointAttribute : Attribute
    {
    }
}