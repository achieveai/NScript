//-----------------------------------------------------------------------
// <copyright file="MessageEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for MessageEvent
    /// </summary>
    [IgnoreNamespace]
    public sealed class MessageEvent : ElementEvent
    {
        private extern MessageEvent();

        public extern string Data
        { get; }

        public extern string Origin
        { get; }

        public extern Window Source
        { get; }
    }
}