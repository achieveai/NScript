//-----------------------------------------------------------------------
// <copyright file="FormData.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for FormData.
    /// </summary>
    [IgnoreNamespace]
    public class FormData
    {
        /// <summary>
        /// Gets the form data.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern FormData();

        /// <summary>
        /// Gets the form data.
        /// </summary>
        /// <param name="formElement"> The form element. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern FormData(FormElement formElement);

        /// <summary>
        /// Appends a name.
        /// </summary>
        /// <param name="name">     The name. </param>
        /// <param name="data">     The data. </param>
        /// <param name="fileName"> (optional) filename of the file. </param>
        public extern void Append(string name, File data, string fileName = null);

        /// <summary>
        /// Appends a name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="data"> The data. </param>
        public extern void Append(string name, string data);

        /// <summary>
        /// Send this message.
        /// </summary>
        /// <param name="url"> URL of the document. </param>
        /// <param name="cb">  The callback(dataString, statusCode, succeeded) </param>
        public void Send(
            string url,
            Action<string, short, bool> cb,
            string acceptType = "text/*",
            string[] headerPair = null)
        {
            var request = new XMLHttpRequest();
            request.Open("POST", url, true);
            request.SetRequestHeader("Accept", acceptType);
            if (headerPair != null)
            {
                for (int iHeader = 0; iHeader < headerPair.Length - 1; iHeader+=2)
                {
                    request.SetRequestHeader(headerPair[iHeader], headerPair[iHeader + 1]);
                }
            }

            if (cb != null)
            {
                request.OnLoad +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        cb(request.ResponseText, request.Status, false);
                    };

                request.OnError +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        cb(null, request.Status, true);
                    };
            }

            request.Send(this);
        }
    }
}
