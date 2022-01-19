//-----------------------------------------------------------------------
// <copyright file="LazyAsync.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for LazyAsync
    /// </summary>
    public class LazyAsync<T> : Lazy<Promise<T>>
    {
        public LazyAsync(Promise<T> promise)
            : base (() => promise)
        {
        }

        public LazyAsync(Func<Promise<T>> factory)
            : base (factory)
        {
        }

        public TaskAwaiter<T> GetAwaiter()
            => Value.GetAwaiter();
    }
}
