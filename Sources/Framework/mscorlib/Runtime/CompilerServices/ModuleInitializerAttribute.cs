//-----------------------------------------------------------------------
// <copyright file="ModuleInitializerAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false), NonScriptable]
    public sealed class ModuleInitializerAttribute : Attribute
    {
    }
}
