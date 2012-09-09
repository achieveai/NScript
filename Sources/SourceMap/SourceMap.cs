using OwaSourceMapper.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace OwaSourceMapper
{
    public struct SourceMapping
    {
        /// <summary>
        /// Source column.
        /// </summary>
        public int SourceColumn;

        /// <summary>
        /// Source line.
        /// </summary>
        public int SourceLine;

        /// <summary>
        /// Target column.
        /// </summary>
        public int TargetColumn;

        /// <summary>
        /// Target line.
        /// </summary>
        public int TargetLine;

        /// <summary>
        /// Zero-based index of the source file.
        /// </summary>
        public int SourceFileIndex;

        /// <summary>
        /// Zero-based index of the source name.
        /// </summary>
        public int SourceNameIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sLine">      The line. </param>
        /// <param name="sCol">       The col. </param>
        /// <param name="tLine">      The line. </param>
        /// <param name="tCol">       The col. </param>
        /// <param name="sFileIndex"> Zero-based index of the s file. </param>
        /// <param name="sNameIndex"> (optional) zero-based index of the s name. </param>
        public SourceMapping(int sLine, int sCol, int tLine, int tCol, int sFileIndex, int sNameIndex = -1)
        {
            this.SourceLine = sLine;
            this.SourceColumn = sCol;
            this.TargetLine = tLine;
            this.TargetColumn = tCol;
            this.SourceFileIndex = sFileIndex;
            this.SourceNameIndex = sNameIndex;
        }

        /// <summary>
        /// Gets the default mapping.
        /// </summary>
        /// <value>
        /// The default mapping.
        /// </value>
        public static SourceMapping DefaultMapping
        {
            get { return new SourceMapping(0, 0, 0, 0, 0, 0); }
        }

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", this.SourceLine, this.SourceColumn, this.TargetLine, this.TargetColumn);
        }

        /// <summary>
        /// Converts a previousMapping to a string relative.
        /// </summary>
        /// <param name="previousMapping"> The previous mapping. </param>
        /// <returns>
        /// previousMapping as a string.
        /// </returns>
        public string ToStringRelative(SourceMapping previousMapping)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceColumn - previousMapping.SourceColumn));

            if (this.SourceFileIndex >= 0)
            {
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceFileIndex - previousMapping.SourceFileIndex));
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.TargetLine - previousMapping.TargetLine));
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.TargetColumn - previousMapping.TargetColumn));

                if (this.SourceNameIndex != -1)
                {
                    sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceNameIndex));
                }
            }

            return sb.ToString();
        }
    }

    public class SourceMap
    {
        /// <summary>
        /// The mappings.
        /// </summary>
        private SortedList<Tuple<int, int>, SourceMapping> mappings;

        /// <summary>
        /// The files.
        /// </summary>
        private List<string> files = new List<string>();

        /// <summary>
        /// The names.
        /// </summary>
        private List<string> names = new List<string>();

        /// <summary>
        /// The version.
        /// </summary>
        private int version = 3;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SourceMap()
        {
            mappings = new SortedList<Tuple<int, int>, SourceMapping>();
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets source root.
        /// </summary>
        /// <value>
        /// The source root.
        /// </value>
        public string SourceRoot { get; set; }

        /// <summary>
        /// Mapping comparison.
        /// </summary>
        /// <param name="lineCol1"> The first line col. </param>
        /// <param name="lineCol2"> The second line col. </param>
        /// <returns>
        /// .
        /// </returns>
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

        /// <summary>
        /// Adds a mapping.
        /// </summary>
        /// <param name="sLine"> The line. </param>
        /// <param name="sCol">  The col. </param>
        /// <param name="tLine"> The line. </param>
        /// <param name="tCol">  The col. </param>
        /// <param name="file">  The file. </param>
        /// <param name="name">  (optional) the name. </param>
        public void AddMapping(int sLine, int sCol, int tLine, int tCol, string file, string name = null)
        {
            if (file != null)
            {
                file = file.Replace("\\", "\\\\");
            }

            int sFileIndex = this.files.IndexOf(file);

            if (sFileIndex == -1
                && file != null)
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

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{" +
                "\t\"version\": \"" + this.version + "\",\n");

            if (this.File != null)
            {
                sb.Append("\t\"file\": \"" + this.File + "\",\n");
            }

            if (this.SourceRoot != null)
            {
                sb.Append("\t\"sourceRoot\": \"" + this.SourceRoot + "\",\n");
            }
            
            if (this.files.Count > 0)
            {
                sb.Append("\t\"sources\": [\"" + string.Join("\",\n\t\t\"", this.files) + "\"],\n");
            }

            if (this.names.Count > 0)
            {
                sb.Append("\t\"names\": [\"" + string.Join("\",\"", this.names) + "\"],\n");
            }
            
            sb.Append("\t\"mappings\": \"");
            
            SourceMapping previousMapping = SourceMapping.DefaultMapping;

            int currentSourceLine = 0;
            bool firstSegment = true;

            StringBuilder mappingSb = new StringBuilder();
            foreach (var mappingKeyPair in mappings)
            {
                var mapping = mappingKeyPair.Value;

                // Fill missed semicolons
                if (currentSourceLine != mapping.SourceLine)
                {
                    mappingSb.Append(new string(';', mapping.SourceLine - currentSourceLine));
                    // previousMapping = SourceMapping.DefaultMapping;
                }
                else
                {
                    mappingSb.Append(',');
                }

                mappingSb.Append(mapping.ToStringRelative(previousMapping));

                currentSourceLine = mapping.SourceLine;
                if (mapping.SourceFileIndex >= 0)
                {
                    previousMapping = mapping;
                }
            }

            sb.Append(mappingSb);
            sb.Append("\"\n}");

            return sb.ToString();
        }
    }
}
