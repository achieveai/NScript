using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Cs2JsC.Lib.ILParser
{
    public class ILFileInfo
    {
        #region member variables
        private readonly AssemblyInfo assemblyInfo;

        private readonly List<AssemblyInfo> referencedAssemblies = new List<AssemblyInfo>();
        private readonly List<ClassInfo> classInfos = new List<ClassInfo>();
        #endregion

        #region constructors/Factories
        public ILFileInfo(
            string ilFileName,
            bool parseMetadataOnly)
        {
            var lines = ILFileInfo.ReadIlFiles(ilFileName);

            lines = ILFileInfo.FixLineInfos(lines);

            var scopes = Scope.CreateScopes(lines);

            foreach (var scope in scopes)
            {
                if (scope.ScopeType == ScopeType.Assembly)
                {
                    AssemblyInfo assemblyInfo = new AssemblyInfo(scope);

                    if (assemblyInfo.AssemblyParser.IsExtern)
                    {
                        this.referencedAssemblies.Add(assemblyInfo);
                        var nestedIlFilesInfo = new ILFileInfo(
                            ILFileInfo.GetAssemblyPath(
                                assemblyInfo.Assembly.Signature.Name,
                                ilFileName),
                            true);
                    }
                    else
                    {
                        this.assemblyInfo = assemblyInfo;

                        if (!parseMetadataOnly)
                        {
                            NameResolver.Instance.MainAssembly = this.assemblyInfo.Assembly;
                        }
                    }
                }
                else if (scope.ScopeType == ScopeType.Class)
                {
                    ClassInfo classInfo = new ClassInfo(
                        this.assemblyInfo,
                        null,
                        scope,
                        parseMetadataOnly);
                }

                if (this.assemblyInfo != null)
                {
                    NameResolver.Instance.CurrentAssembly = this.assemblyInfo.Assembly.Signature;
                }
            }

            NameResolver.Instance.AddAssembly(this.assemblyInfo.Assembly);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static List<string> FixLineInfos(IList<string> lines)
        {
            StringFragment lastFileName = null;

            for (int iLine = 0; iLine < lines.Count; iLine++)
            {
                var word = ParseUtils.GetNextWord(lines[iLine], 0);

                if (word == ".line")
                {
                    // Get the startPosition,
                    //
                    var startPosition = ParseUtils.GetNextWord(word);

                    // Get :
                    //
                    word = ParseUtils.GetNextWord(startPosition);

                    // Get endPosition,
                    //
                    var endPosition = ParseUtils.GetNextWord(word);

                    // Get FileInfo
                    //
                    var fileName = ParseUtils.GetNextWord(endPosition);

                    if (fileName == "''")
                    {
                        lines[iLine] = string.Format(
                            "{0}{1}",
                            fileName.ParentString.Substring(0, fileName.StartIndex),
                            lastFileName);
                    }
                }

                lines[iLine] = ILFileInfo.CleanComment(lines[iLine]);
            }

            List<string> newLines = new List<string>(lines.Count);

            for (int iLine = 0; iLine < lines.Count; iLine++)
            {
                var firstWord = ParseUtils.GetNextWord(lines[iLine], 0);

                if (StringFragment.IsNull(firstWord))
                {
                    continue;
                }

                if (firstWord[0] == '.' ||
                    (firstWord.StartsWith("IL_") && firstWord[firstWord.Length - 1] == ':') ||
                    firstWord[0] == '{' ||
                    firstWord[0] == '}')
                {
                    newLines.Add(lines[iLine]);
                }
                else if (newLines.Count > 0)
                {
                    newLines[newLines.Count - 1] = newLines[newLines.Count - 1] + " " + lines[iLine].Substring(firstWord.StartIndex);
                }
            }

            return newLines;
        }

        private static string CleanComment(string line)
        {
            char quoteChar = '\0';
            bool inQuote = false;

            for (int i = 0; i < line.Length - 1; i++)
            {
                if (line[i] == '"' ||
                    line[i] == '\'')
                {
                    if (inQuote && quoteChar == line[i])
                    {
                        inQuote = false;
                    }
                    else if (!inQuote)
                    {
                        inQuote = true;
                        quoteChar = line[i];
                    }
                }
                else if (line[i] == '/' && line[i + 1] == '/')
                {
                    return line.Substring(0, i);
                }
            }

            return line;
        }

        private static List<string> ReadIlFiles(string dllPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\ildasm.exe";
            process.StartInfo.Arguments = string.Format("{0} /Text /LINENUM", dllPath);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            string line = null;
            List<string> returnValue = new List<string>();
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                returnValue.Add(line);
            }

            return returnValue;
        }

        private static string GetAssemblyPath(string assemblyName, string currentAssembly)
        {
            string dirName = String.IsNullOrEmpty(System.IO.Path.GetDirectoryName(currentAssembly)) ?
                "." :
                System.IO.Path.GetDirectoryName(currentAssembly);
            return string.Format("{0}\\{1}.dll",
                dirName,
                assemblyName);
        }
        #endregion
    }
}
