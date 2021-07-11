//-----------------------------------------------------------------------
// <copyright file="Factory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Factory
    /// </summary>
    [IgnoreGenericArguments]
    public class Factory<T> where T : class
    {
        Func<T> factory;

        public Factory(Func<T> factory)
        {
            ExceptionHelpers.ThrowOnArgumentNull(factory, "factory");
            this.factory = factory;
        }

        public T Create()
        { return this.factory(); }
    }
}
