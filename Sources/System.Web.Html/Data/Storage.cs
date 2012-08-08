//-----------------------------------------------------------------------
// <copyright file="Storage.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    [Imported]
    [IgnoreNamespace]
    public sealed class Storage
    {
        private Storage() { }

        [IntrinsicField]
        public int Length;

        [IntrinsicProperty]
        public extern object this[string key]
        {
            get;
            set;
        }

        public extern void Clear();

        public extern object GetItem(string key);

        [ScriptName("key")]
        public extern string GetKey(int index);

        public extern void RemoveItem(string key);

        public extern void SetItem(string key, object value);
    }
}