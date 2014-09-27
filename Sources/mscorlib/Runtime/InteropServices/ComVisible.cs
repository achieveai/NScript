//-----------------------------------------------------------------------
// <copyright file="ComVisible.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>Controls accessibility of an individual managed type or member, or of all types within an assembly, to COM.</summary>
    public sealed class ComVisibleAttribute : Attribute
    {
        internal bool _val;
        /// <summary>Gets a value that indicates whether the COM type is visible.</summary>
        /// <returns>true if the type is visible; otherwise, false. The default value is true.</returns>
        public bool Value
        {
            get
            {
                return this._val;
            }
        }
        /// <summary>Initializes a new instance of the ComVisibleAttribute class.</summary>
        /// <param name="visibility">true to indicate that the type is visible to COM; otherwise, false. The default is true. </param>
        public ComVisibleAttribute(bool visibility)
        {
            this._val = visibility;
        }
    }}
