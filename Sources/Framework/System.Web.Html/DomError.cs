//-----------------------------------------------------------------------
// <copyright file="DomError.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;

    /// <summary>
    /// Definition for DomError
    /// </summary>
    public class DomError
    {
        internal extern DomError();

        public extern string Name
        { get; }
    }

    public class DomException : DomError
    {
        public extern string Message
        { get; }
    }

    public static class DomExtentions
    {
        public static string ExceptionMessage(this DomError domError)
        {
            DomException domException = (DomException)domError;
            if (object.IsNullOrUndefined(domError))
            { return null; }

            if (!string.IsNullOrUndefined(domException.Message))
            { return domException.Message; }

            return domError.Name;
        }
    }
}
