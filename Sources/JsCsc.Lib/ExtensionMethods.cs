//-----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using NScript.Utils;
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

        public static string GetStrLoc(this Mono.CSharp.Expression expression)
        {
            if (expression.Location.IsNull)
            {
                return null;
            }

            return string.Format("{0}:{1}-{2}:{3}",
                expression.Location.Row,
                expression.Location.Column,
                expression.EndLocation.Row,
                expression.EndLocation.Column);
        }

        public static string GetStrLoc(this Mono.CSharp.Statement statement)
        {
            if (statement.Location.IsNull)
            {
                return null;
            }

            return string.Format("{0}:{1}-{2}:{3}",
                statement.Location.Row,
                statement.Location.Column,
                statement.EndLocation.Row,
                statement.EndLocation.Column);
        }

        public static string GetStrLoc(this Mono.CSharp.Block block)
        {
            if (block.Location.IsNull)
            {
                return null;
            }

            return string.Format("{0}:{1}-{2}:{3}",
                block.Location.Row,
                block.Location.Column,
                block.EndLocation.Row,
                block.EndLocation.Column);
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