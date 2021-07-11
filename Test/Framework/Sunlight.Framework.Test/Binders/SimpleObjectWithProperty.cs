//-----------------------------------------------------------------------
// <copyright file="SimpleObjectWithProperty.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Binders
{
    using System;

    /// <summary>
    /// Definition for SimpleObjectWithProperty
    /// </summary>
    public class SimpleObjectWithProperty
    {
        public string StringProp
        {
            get;
            set;
        }

        public int IntProp
        {
            get;
            set;
        }

        public SimpleObjectWithProperty SelfProp
        {
            get;
            set;
        }

        public SimpleNotifiableClass NotifiableProp
        {
            get;
            set;
        }
    }
}
