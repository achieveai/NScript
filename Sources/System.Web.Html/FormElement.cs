//-----------------------------------------------------------------------
// <copyright file="FormElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for FormElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class FormElement : Element
    {
        private FormElement() { }

        [IntrinsicField]
        public readonly int Length;

        [IntrinsicField]
        public string AcceptCharset;

        [IntrinsicField]
        public string Action;

        [IntrinsicField]
        public NativeArray /*Element[]*/ Elements;

        [IntrinsicField]
        public string EncType;

        [IntrinsicField]
        public string Encoding;

        [IntrinsicField]
        public string Name;

        [IntrinsicField]
        public string Method;

        [IntrinsicField]
        public string Target;

        public extern void Reset();

        public extern void Submit();
    }
}