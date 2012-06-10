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
    [Imported]
    [IgnoreNamespace]
    public sealed class MessageEvent : ElementEvent
    {
        private MessageEvent() { }

        [IntrinsicField]
        public readonly string Data;

        [IntrinsicField]
        public readonly string Origin;

        [IntrinsicField]
        public readonly Window Source;
    }
}