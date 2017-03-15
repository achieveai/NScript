//-----------------------------------------------------------------------
// <copyright file="ErrorEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using Runtime.CompilerServices;
    using System;

    [IgnoreNamespace, ImportedType]
    public class ErrorEvent : Event
    {
        public extern ErrorEvent(
            string type);

        public extern ErrorEvent(
            string type,
            ErrorEventInit errorEventInitDict);

        [ScriptName("colno")]
        public extern int? ColumnNumber
        { get; }

        [ScriptName("lineno")]
        public extern int? LineNumber
        { get; }

        [ScriptName("message")]
        public extern string Message
        { get; }

        [ScriptName("filename")]
        public extern string Filename
        { get; }

        [ScriptName("filename")]
        public extern object Error
        { get; }

        public extern void InitErrorEvent(
            string typeArg,
            bool canBubbleArg,
            bool cancelableArg,
            string message,
            string fileNameArg,
            int lineNoArg);
    }

    [JsonType]
    public class ErrorEventInit
    {
        [ScriptName("message")]
        public extern string Message
        { get; set; }

        [ScriptName("filename")]
        public extern string FileName
        { get; set; }

        public extern object Error
        { get; set; }

        [ScriptName("lineno")]
        public extern int? LineNumber
        { get; set; }

        [ScriptName("colno")]
        public extern int? ColumnNumber
        { get; set; }
    }
}
