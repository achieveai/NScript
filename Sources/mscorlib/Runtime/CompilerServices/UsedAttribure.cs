//-----------------------------------------------------------------------
// <copyright file="UsedAttribure.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for UsedAttribure
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited=true, AllowMultiple=false), NonScriptable]
    public class UsedAttribure : Attribute
    {
    }
}
