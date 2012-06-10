//-----------------------------------------------------------------------
// <copyright file="AttachedProperty.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;

    /// <summary>
    /// Definition for AttachedProperty
    /// </summary>
    public class AttachedProperty<T>
    {
        private string name;

        internal AttachedProperty(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}
