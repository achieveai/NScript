//-----------------------------------------------------------------------
// <copyright file="SkinFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using Sunlight.Framework.UI.Helpers;
    using System;
    using System.Web.Html;

    /// <summary>
    /// Definition for SkinFactory
    /// </summary>
    public class Skin
    {
        Func<Skin, Document, SkinInstance> factoryMethod;
        Type skinableType;
        Type dataContextType;
        string id;

        public Skin(
            Type skinableType,
            Type dataContextType,
            Func<Skin, Document, SkinInstance> factoryMethod,
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
            return factoryMethod(this, Window.Instance.Document);
        }
    }
}
