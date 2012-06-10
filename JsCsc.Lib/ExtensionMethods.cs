//-----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using Cs2JsC.Utils;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Definition for ExtensionMethods
    /// </summary>
    public static class ExtensionMethods
    {
        public static Location GetCsLocation(this Mono.CSharp.Location loc)
        {
            if (loc.File == 0)
            {
                return null;
            }

            return new Location(loc.NameFullPath, loc.Row, loc.Column);
        }

        public static string GetStrLoc(this Mono.CSharp.Location loc)
        {
            if (loc.Row > 0)
            {
                return string.Format("{0}:{1}", loc.Row, loc.Column);
            }

            return null;
        }

        public static T Val<T>(this JObject jObject, string propertyName)
        {
            var value = jObject[propertyName];
            if (value == null)
            {
                return default(T);
            }

            return value.Value<T>();
        }
    }
}