//-----------------------------------------------------------------------
// <copyright file="Lazy.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;

    /// <summary>
    /// Definition for Lazy
    /// </summary>
    public class Lazy<T> where T : class
    {
        private Func<T> factory;
        private T value;
        private Action createdCallbacks;

        public Lazy(Func<T> factory)
        {
            ExceptionHelpers.ThrowOnArgumentNull(factory, "factory");
            this.factory = factory;
        }

        public event Action OnCreated
        {
            add
            {
                if (this.factory == null)
                {
                    value();
                    return;
                }

                if (this.createdCallbacks == null)
                {
                    this.createdCallbacks = value;
                }
                else
                {
                    this.createdCallbacks += value;
                }
            }

            remove
            {
                if (this.createdCallbacks != null)
                {
                    this.createdCallbacks -= value;
                }
            }
        }

        public bool IsValueCreated
        { get { return this.factory == null; } }

        public T Value
        {
            get
            {
                if (this.factory != null)
                {
                    this.value = this.factory();
                    this.factory = null;

                    if (this.createdCallbacks != null)
                    {
                        this.createdCallbacks();
                        this.createdCallbacks = null;
                    }
                }

                return this.value;
            }
        }
    }
}
