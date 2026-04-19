//-----------------------------------------------------------------------
// <copyright file="InternalsVisibleToAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Specifies that types that are ordinarily visible only within the current
    /// assembly are visible to another specified assembly. The Roslyn compiler
    /// recognizes this well-known attribute during semantic analysis to grant
    /// <c>internal</c>-visibility access to the named friend assembly.
    /// </summary>
    /// <remarks>
    /// NonScriptable because the attribute has no runtime representation in the
    /// emitted JavaScript — it only affects C# compilation semantics.
    /// </remarks>
    [NonScriptable]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class InternalsVisibleToAttribute : Attribute
    {
        private readonly string assemblyName;

        public InternalsVisibleToAttribute(string assemblyName)
        {
            this.assemblyName = assemblyName;
        }

        public string AssemblyName
        {
            get { return this.assemblyName; }
        }

        public bool AllInternalsVisible { get; set; }
    }
}
