//-----------------------------------------------------------------------
// <copyright file="PsudoTypeAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for ImportedTypeAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Type), NonScriptable]
    public class ImportedTypeAttribute : Attribute
    {
    }

    /// <summary>
    /// Attribute for json type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Type), NonScriptable]
    public class JsonTypeAttribute : Attribute
    {
    }
}
