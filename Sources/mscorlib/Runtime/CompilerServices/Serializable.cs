//-----------------------------------------------------------------------
// <copyright file="Serializable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <filterpriority>1</filterpriority>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    public sealed class SerializableAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.SerializableAttribute" /> class.</summary>
        public SerializableAttribute()
        {
        }
    }
}
