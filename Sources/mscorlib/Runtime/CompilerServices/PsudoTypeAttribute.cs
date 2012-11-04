//-----------------------------------------------------------------------
// <copyright file="PsudoTypeAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for PsudoTypeAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Type), NonScriptable]
    public class PsudoTypeAttribute : ExtendedAttribute
    {
    }
}
