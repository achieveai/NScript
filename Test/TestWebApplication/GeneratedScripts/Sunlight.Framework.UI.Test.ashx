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
            var fileName = context.Request.Params["f"].Substring(1);
            context.Response.ContentType = "text";

            var dirName = Path.GetDirectoryName(context.Request.PhysicalPath);
            var myName = Path.GetFileNameWithoutExtension(context.Request.PhysicalPath);
            var path = GetFileToOpen(
                dirName,
                myName,
                fileName);

            using (StreamReader reader = new StreamReader(path))
            {
                context.Response.Write(reader.ReadToEnd());
            }
        }

        public string GetFileToOpen(
            string directory,
            string myName,
            string fileName)
        {
            string lines = File.ReadAllText(
                Path.Combine(directory, myName + ".map"));
            string defaultFileToReturn = Path.Combine(directory, myName + ".js");
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
                    if (fileNames[i].StartsWith(myName) && fileNames[i] == myName + ".js")
                    {
                        return defaultFileToReturn;
                    }
                    else
                    {
                        return fileLongNames[i];
                    }
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