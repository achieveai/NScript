//-----------------------------------------------------------------------
// <copyright file="SkinFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using System;

    /// <summary>
    /// Definition for SkinFactory
    /// </summary>
    public class SkinFactory
    {
        Func<SkinFactory, SkinInstance> factoryMethod;
        Type skinableType;
        Type dataContextType;
        string id;

        public SkinFactory(
            Type skinableType,
            Type dataContextType,
            Func<SkinFactory, SkinInstance> factoryMethod,
            string id)
        {
            this.factoryMethod = factoryMethod;
            this.skinableType = skinableType;
            this.dataContextType = dataContextType;
            this.id = id;
        }

        public Type SkinableType
        {
            get { return this.skinableType; }
        }

        public Type DataContextType
        {
            get { return this.dataContextType; }
        }

        public SkinInstance CreateInstance()
        {
            return factoryMethod(this);
        }
    }
}
