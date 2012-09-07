using OwaSourceMapper.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace OwaSourceMapper
{
    public struct SourceMapping
    {
        public int SourceColumn;
        public int SourceLine;
        public int TargetColumn;
        public int TargetLine;
        public int SourceFileIndex;
        public int SourceNameIndex;

        public SourceMapping(int sLine, int sCol, int tLine, int tCol, int sFileIndex, int sNameIndex = -1)
        {
            this.SourceLine = sLine;
            this.SourceColumn = sCol;
            this.TargetLine = tLine;
            this.TargetColumn = tCol;
            this.SourceFileIndex = sFileIndex;
            this.SourceNameIndex = sNameIndex;
        }

        public static SourceMapping DefaultMapping
        {
            get { return new SourceMapping(0, 0, 0, 0, -1, -1); }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", this.SourceLine, this.SourceColumn, this.TargetLine, this.TargetColumn);
        }

        public string ToStringRelative(SourceMapping previousMapping)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceFileIndex));
            sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceColumn - previousMapping.SourceColumn));
            sb.Append(Base64VLQ.ConvertToBase64VLQ(this.TargetLine - previousMapping.TargetLine));
            sb.Append(Base64VLQ.ConvertToBase64VLQ(this.TargetColumn - previousMapping.TargetColumn));

            if (this.SourceNameIndex != -1)
            {
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceNameIndex));
            }

            return sb.ToString();
        }
    }

    public class SourceMap
    {
        private SortedList<Tuple<int, int>, SourceMapping> mappings;
        private List<string> files = new List<string>();
        private List<string> names = new List<string>();
        private int version = 3;

        public SourceMap()
        {
            IComparer<Tuple<int, int>> comparer = Comparer<Tuple<int, int>>.Create(MappingComparison);
            mappings = new SortedList<Tuple<int, int>, SourceMapping>();
        }

        public string File { get; set; }

        public string SourceRoot { get; set; }

        public static int MappingComparison(Tuple<int, int> lineCol1, Tuple<int, int> lineCol2)
        {
            if (lineCol1.Item1 != lineCol2.Item1)
            {
                return lineCol1.Item1.CompareTo(lineCol2.Item1);
            }
            else
            {
                return lineCol1.Item2.CompareTo(lineCol2.Item2);
            }
        }

        public void AddMapping(int sLine, int sCol, int tLine, int tCol, string file, string name = null)
        {
            int sFileIndex = this.files.IndexOf(file);

            if (sFileIndex == -1)
            {
                this.files.Add(file);
                sFileIndex = this.files.Count - 1;
            }

            int sNameIndex = -1;

            if (name != null)
            {
                sNameIndex = this.names.IndexOf(name);
            }

            SourceMapping mapping = new SourceMapping(
                sLine,
                sCol,
                tLine,
                tCol,
                sFileIndex,
                sNameIndex);

            Tuple<int, int> key = Tuple.Create<int, int>(sLine, sCol);

            if (mappings.ContainsKey(key))
            {
                mappings[key] = mapping;
            }
            else
            {
                mappings.Add(key, mapping);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{" +
                "version: \"" + this.version + "\",");

            if (this.File != null)
            {
                sb.Append("file: \"" + this.File + "\",");
            }

            if (this.SourceRoot != null)
            {
                sb.Append("sourceroot: \"" + this.SourceRoot + "\",");
            }
            
            if (this.files.Count > 0)
            {
                sb.Append("sources: [\"" + string.Join("\",\"", this.files) + "\"],");
            }

            if (this.names.Count > 0)
            {
                sb.Append("names: [\"" + string.Join("\",\"", this.names) + "\"],");
            }
            
            sb.Append("mappings: \"");
            
            SourceMapping previousMapping = SourceMapping.DefaultMapping;

            int currentSourceLine = 0;
            bool firstSegment = true;

            foreach (var mappingKeyPair in mappings)
            {
                var mapping = mappingKeyPair.Value;

                // Fill missed semicolons
                if (currentSourceLine != mapping.SourceLine)
                {
                    sb.Append(new string(';', mapping.SourceLine - currentSourceLine));

                    previousMapping = SourceMapping.DefaultMapping;
                }
                else
                {
                    if (!firstSegment)
                    {
                        sb.Append(',');
                    }

                    firstSegment = false;
                }

                sb.Append(mapping.ToStringRelative(previousMapping));

                currentSourceLine = mapping.SourceLine;

                previousMapping = mapping;
            }

            sb.Append("\"}");

            return sb.ToString();
        }
    }
}
