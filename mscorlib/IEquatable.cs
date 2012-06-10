//-----------------------------------------------------------------------
// <copyright file="IEquatable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System;

    /// <summary>
    /// Definition for IEquatable
    /// </summary>
    public interface IEquatable<T>
    {
        bool Equals(T other);
    }
}
