//-----------------------------------------------------------------------
// <copyright file="ScriptElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ScriptElement
    /// </summary>
    [IgnoreNamespace, ScriptName("HTMLScriptElement")]
    public sealed class ScriptElement : Element
    {
        private extern ScriptElement();

        public extern string Src
        { get; set; }

        public extern string Type
        { get; set; }

        public extern string ReadyState
        { get; set; }

        public extern bool? Async
        { get; set; }

        public extern bool? Defer
        { get; set; }

        public extern event Action<ScriptElement, ElementEvent> OnLoad;
    }
}