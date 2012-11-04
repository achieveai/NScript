//-----------------------------------------------------------------------
// <copyright file="CustomEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for CustomEvent
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class CustomEvent : ElementEvent
    {
        internal CustomEvent() { }

        [IntrinsicField]
        public object Data;
    }
}