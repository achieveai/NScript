//-----------------------------------------------------------------------
// <copyright file="ImplementAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for ImplementAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field), Extended, NonScriptable]
    public class ImplementAttribute : Attribute
    {
    }
}
