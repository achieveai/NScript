//-----------------------------------------------------------------------
// <copyright file="CustomJsonDeserializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Utils
{
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// Definition for CustomJsonDeserializer
    /// </summary>
    public class CustomJsonDeserializer
    {
        const string typeProp = "_t";
        JsonTextReader reader;

        public CustomJsonDeserializer(TextReader reader)
        {
            this.reader = new JsonTextReader(reader);
        }
    }
}