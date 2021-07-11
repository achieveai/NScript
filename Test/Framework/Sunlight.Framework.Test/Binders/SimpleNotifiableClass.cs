//-----------------------------------------------------------------------
// <copyright file="SimpleNotifiableClass.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Binders
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for SimpleNotifiableClass
    /// </summary>
    public class SimpleNotifiableClass : ObservableObject
    {
        private string strField;
        private int intField;
        private SimpleNotifiableClass selfField;
        private SimpleObjectWithProperty objField;

        public string StrProp
        {
            get { return this.strField; }
            set
            {
                if (this.strField != value)
                {
                    this.strField = value;
                    this.FirePropertyChanged("StrProp");
                }
            }
        }

        public int IntProp
        {
            get { return this.intField; }
            set
            {
                if (this.intField != value)
                {
                    this.intField = value;
                    this.FirePropertyChanged("IntProp");
                }
            }
        }

        public SimpleNotifiableClass SelfProp
        {
            get { return this.selfField; }
            set
            {
                if (this.selfField != value)
                {
                    this.selfField = value;
                    this.FirePropertyChanged("SelfProp");
                }
            }
        }

        public SimpleObjectWithProperty ObjProp
        {
            get { return this.objField; }
            set
            {
                if (this.objField != value)
                {
                    this.objField = value;
                    this.FirePropertyChanged("ObjProp");
                }
            }
        }
    }
}
