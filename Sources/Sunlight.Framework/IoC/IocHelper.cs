//-----------------------------------------------------------------------
// <copyright file="IocHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.IoC
{
    using System;

    /// <summary>
    /// Definition for IocHelper
    /// </summary>
    public class IocHelper
    {
        Action isSingleton;
        Action<Type> registerAs;

        internal IocHelper(
            Action isSingleton,
            Action<Type> registerAs)
        {
            this.registerAs = registerAs;
            this.isSingleton = isSingleton;
        }

        public IocHelper As<T>() where T : class
        {
            this.registerAs(typeof(T));
            return this;
        }

        public IocHelper IsSingleton()
        {
            this.isSingleton();
            return this;
        }
    }
}
