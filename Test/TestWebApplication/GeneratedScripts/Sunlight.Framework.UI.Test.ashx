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
            var fileName = context.Request.PathInfo;
            if (string.IsNullOrEmpty(fileName))
            { fileName = context.Request.Params["f"].Substring(1); }

            if (!string.IsNullOrWhiteSpace(fileName))
            { fileName = fileName.Substring(1); }

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
                        var rv = fileLongNames[i];
                        if (Path.GetFileName(rv) == rv)
                        { return defaultFileToReturn; }

                        return rv;
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

/*
namespace NScript
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Text.RegularExpressions;
    using System.Configuration;

    /// <summary>
    /// Summary description for SrcMapper
    /// </summary>
    public class SrcMapper : IHttpHandler
    {
        static Regex regex = new Regex(
            "(\"|')(file:///)([^'\"]*)",
            RegexOptions.Singleline | RegexOptions.Compiled);
        static Regex srcRootFixer = new Regex(
            @"sourceRoot\s*:\s*""",
            RegexOptions.Singleline | RegexOptions.Compiled);
        static Regex jsonKey = new Regex(
            @"^(version|names|sourceRoot|sources|file|mappings)(\s*):",
            RegexOptions.Singleline | RegexOptions.Compiled);

        static string[] srcMapLookupDirs = (ConfigurationSettings.AppSettings["srcMapRoots"] ?? string.Empty).Split(new char[]{';'},StringSplitOptions.RemoveEmptyEntries);

        public void ProcessRequest(HttpContext context)
        {
            string fileName = null;
            var srcMapName = context.Request.Url.Query;
            if (srcMapName != null)
            {
                srcMapName = srcMapName = srcMapName.Substring(1);
            }

            int indexOfSlash = srcMapName.IndexOf('/');
            if (indexOfSlash > 0)
            {
                fileName = srcMapName.Substring(indexOfSlash + 1);
                srcMapName = srcMapName.Substring(0, indexOfSlash);
            }

            if (srcMapName != null)
            {
                srcMapName += ".js.srcmap";
            }

            var dirName = Path.GetDirectoryName(context.Request.PhysicalPath);
            if (fileName == null)
            {
                this.WriteSrcMap(
                    context,
                    srcMapName,
                    dirName);
                return;
            }
            else
            {
                if (fileName != null)
                {
                    var tuple = this.GetFileNames(
                        this.GetSourceMapContent(srcMapName, dirName));

                    if (tuple != null && tuple.Item1.ContainsKey(fileName))
                    {
                        fileName = tuple.Item1[fileName];
                    }
                }

                if (!File.Exists(fileName))
                {
                    if (File.Exists(Path.Combine(dirName, fileName)))
                    {
                        fileName = Path.Combine(dirName, fileName);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                        return;
                    }
                }

                context.Response.ContentType = "text/plain";
                context.Response.WriteFile(fileName);
            }
        }

        public string[] GetSourceMapContent(string srcMapFileName, string dirName)
        {
            string[] srcMapFileLines = null;
            if (File.Exists(srcMapFileName))
            {
                srcMapFileLines = File.ReadAllLines(srcMapFileName);
            }
            else if (File.Exists(Path.Combine(dirName, srcMapFileName)))
            {
                srcMapFileLines = File.ReadAllLines(Path.Combine(dirName, srcMapFileName));
            }
            else
            {
                foreach (var dir in SrcMapper.srcMapLookupDirs)
                {
                    dirName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, dir);
                    if (File.Exists(Path.Combine(dirName, srcMapFileName)))
                    {
                        srcMapFileLines = File.ReadAllLines(Path.Combine(dirName, srcMapFileName));
                        break;
                    }
                }
            }

            return srcMapFileLines;
        }

        private Tuple<Dictionary<string, string>, Dictionary<string, string>> GetFileNames(
            string[] srcMapFileLines)
        {
            if (srcMapFileLines == null)
            { return null; }

            Dictionary<string, string> fileMap = new Dictionary<string, string>();
            Dictionary<string, string> rFileMap = new Dictionary<string, string>();
            List<string> fullFileNames = new List<string>();
            foreach (var line in srcMapFileLines)
            {
                var match = SrcMapper.regex.Match(line);
                while (match.Success)
                {
                    string filePath = match.Groups[3].Value.ToLowerInvariant();
                    fullFileNames.Add(filePath);

                    match = match.NextMatch();
                }
            }

            fullFileNames.Sort();

            int maxMatch = Path.GetDirectoryName(fullFileNames[0]).Length + 1;
            for (int i = 1; i < fullFileNames.Count; i++)
            {
                int j = 0;
                string str1 = fullFileNames[i];
                string str2 = fullFileNames[i - 1];
                for (; j < str1.Length && j < str2.Length; j++)
                {
                    if (str1[j] != str2[j])
                    { break; }
                }

                maxMatch = Math.Min(maxMatch, j);
            }

            foreach (var fullFileName in fullFileNames)
            {
                string fileNameKey = fullFileName.Substring(maxMatch) + "_";

                rFileMap[fullFileName] = fileNameKey;
                fileMap[fileNameKey] = fullFileName;
            }

            return Tuple.Create(fileMap, rFileMap);
        }

        private void WriteSrcMap(
            HttpContext context,
            string srcMapFileName,
            string dirName)
        {
            string[] srcMapFileLines = this.GetSourceMapContent(srcMapFileName, dirName);

            if (srcMapFileLines == null)
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                return;
            }

            var tuple = this.GetFileNames(srcMapFileLines);
            context.Response.ContentType = "text/plain";

            for (int iLine = 0; iLine < srcMapFileLines.Length; iLine++)
            {
                string str = 
                    SrcMapper.regex.Replace(
                        srcMapFileLines[iLine],
                        (match) =>
                        {
                            return string.Format(
                                "{0}{1}",
                                match.Groups[1].Value,
                                tuple.Item2[match.Groups[3].Value.ToLowerInvariant()]);
                        });

                str = SrcMapper.srcRootFixer.Replace(
                    str,
                    (match)  => string.Format(
                        "sourceRoot: \"{0}",
                        context.Request.RawUrl));

                str = SrcMapper.jsonKey.Replace(
                    str,
                    (match) => string.Format("\"{0}\":", match.Groups[1].Value));

                srcMapFileLines[iLine] = str;
            }

            string finalContents = String.Join("\n", srcMapFileLines).Trim();
            if (finalContents.EndsWith(",\n}"))
            {
                finalContents = finalContents.Substring(0, finalContents.Length - 3) + "\n}";
            }
            else if (finalContents.EndsWith(",}"))
            {
                finalContents = finalContents.Substring(0, finalContents.Length - 2) + "}";
            }

            context.Response.Write(finalContents);
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
*/