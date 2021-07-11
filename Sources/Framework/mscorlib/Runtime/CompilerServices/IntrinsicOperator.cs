//-----------------------------------------------------------------------
// <copyright file="IntrinsicOperator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for IntrinsicOperator
    /// </summary>
    [Extended, NonScriptable, AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=false)]
    public sealed class IntrinsicOperatorAttribute : Attribute
    {
    }
}
