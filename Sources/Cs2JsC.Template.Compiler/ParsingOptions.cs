using System.Collections.Generic;
using System.IO;
using System;
namespace Cs2JsC.Template.Compiler
{
    public class ParsingOptions
    {
        public string JsFileName { get; set; }
        public string CssFileName { get; set; }

        private List<string> referenceDlls = new List<string>();
        public List<string> ReferenceDlls
        {
            get { return this.referenceDlls; }
        }

        private List<string> templateFiles = new List<string>();
        public List<string> TemplateFiles
        {
            get { return this.templateFiles; }
        }
        
        private List<string> globalStyleNames = new List<string>();
        public List<string> GlobalStyleNames
        {
            get { return this.globalStyleNames; }
        }

        public ParsingOptions()
        {
        }
        
        public void AddGlobalStyleNames(string styleNames)
        {
            string[] styles = styleNames.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string style in styles)
            {
                this.globalStyleNames.Add(style);
            }
        }

        public List<ErrorInfo> AddTemplateFile(string htmlFile)
        {
            return this.GetFiles(htmlFile, this.templateFiles, FileType.Template);
        }

        public List<ErrorInfo> AddReferenceDll(string referenceDll)
        {
            return this.GetFiles(referenceDll, this.referenceDlls, FileType.Reference);
        }

        private List<ErrorInfo> GetFiles(string fileNameOrWildcard, List<string> listToAddTo, FileType fileType)
        {
            List<ErrorInfo> fileErrors = new List<ErrorInfo>();

            string[] fileSplits = fileNameOrWildcard.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
            string fileName;
            string[] files;

            foreach (var singleFileName in fileSplits)
            {
                var dirPath = Path.GetDirectoryName(singleFileName);

                if (dirPath == string.Empty)
                {
                    dirPath = "./";
                }

                // If the folder doesn't exist, ignore it
                if(Directory.Exists(dirPath))
                {
                    fileName = Path.GetFileName(singleFileName);
                    files = Directory.GetFiles(dirPath, fileName);

                    if (files.Length == 0)
                    {
                        // ignore wildcards
                        if (!fileName.Contains("*"))
                        {
                            fileErrors.Add(new ErrorInfo(fileName, fileType, ErrorType.NotFound));
                        }
                    }
                    else
                    {
                        for (int iFile = 0; iFile < files.Length; ++iFile)
                        {
                            listToAddTo.Add(files[iFile]);
                        }
                    }
                }
            }

            return fileErrors;
        }
    }
}
