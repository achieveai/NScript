<%@ WebHandler Language="C#" Class="NScript.SrcMapper" %>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NScript
{
    /// <summary>
    /// Summary description for SrcMapper
    /// </summary>
    public class SrcMapper : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var jsFileName = context.Request.Params["js"];
            var fileName = context.Request.Params["fname"].Substring(1);
            context.Response.ContentType = "text";

            var path = GetFileToOpen(
                Path.GetDirectoryName(context.Request.PhysicalApplicationPath),
                jsFileName,
                fileName);

            if (path == jsFileName)
            {
                path = Path.Combine(
                    Path.GetDirectoryName(context.Request.PhysicalApplicationPath),
                    jsFileName);
            }

            using (StreamReader reader = new StreamReader(path))
            {
                context.Response.Write(reader.ReadToEnd());
            }
        }

        public string GetFileToOpen(
            string directory,
            string jsFileName,
            string fileName)
        {
            string lines = File.ReadAllText(
                Path.Combine(directory, jsFileName + ".map"));
            string defaultFileToReturn = Path.Combine(directory, jsFileName);
            const string sourcesMarker = "\"sources\": [";
            const string sourcesLongMarker = "\"sourcesLong\": [";
            const string endMarker = "\"],";
            char[] seperators = new char[] {',', '"', '\n', '\t'};

            int sourcesIndex = lines.IndexOf(sourcesMarker);
            if (sourcesIndex < 0)
            {
                return defaultFileToReturn;
            }

            int sourcesEndIndex = lines.IndexOf(endMarker, sourcesIndex + 1);
            if (sourcesEndIndex++ < 0)
            {
                return defaultFileToReturn;
            }

            int sourcesLongIndex = lines.IndexOf(sourcesLongMarker, sourcesEndIndex + 1);
            int sourcesLongEndIndex = lines.IndexOf(endMarker, sourcesEndIndex + 1);
            if (sourcesLongEndIndex < 0 || sourcesLongIndex < 0)
            {
                return defaultFileToReturn;
            }

            string[] fileNames = lines.Substring(sourcesIndex + sourcesMarker.Length, sourcesEndIndex - sourcesIndex - sourcesMarker.Length).Split(seperators, StringSplitOptions.RemoveEmptyEntries);
            string[] fileLongNames = lines.Substring(sourcesLongIndex + sourcesLongMarker.Length, sourcesLongEndIndex - sourcesLongIndex - sourcesLongMarker.Length).Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < fileNames.Length && i < fileLongNames.Length; i++)
            {
                if (fileNames[i] == fileName)
                {
                    return fileLongNames[i];
                }
            }

            return defaultFileToReturn;
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}