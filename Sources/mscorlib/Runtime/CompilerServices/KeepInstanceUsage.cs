//-----------------------------------------------------------------------
// <copyright file="KeepInstanceUsage.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for KeepInstanceUsage
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class KeepInstanceUsageAttribute : Attribute
    {
    }
}