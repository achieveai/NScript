//-----------------------------------------------------------------------
// <copyright file="ChunkToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST.Writer
{
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// One method function, rendered on its own at depth 0 (dev mode). The outer writer
    /// splices <see cref="Text"/> in with re-indentation and re-adds <see cref="Segments"/>
    /// at the splice position, so the output equals rendering the tokens in place.
    /// </summary>
    internal sealed class ChunkToken : TokenBase
    {
        public ChunkToken(
            string name,
            LinkedList<TokenBase> tokens,
            string text,
            List<ChunkSegment> segments,
            int endLine,
            int endColumn,
            Location endLocation)
            : base(TokenType.Chunk, tokens.First.Value.Location)
        {
            this.Name = name;
            this.Tokens = tokens;
            this.Text = text;
            this.Segments = segments;
            this.EndLine = endLine;
            this.EndColumn = endColumn;
            this.EndLocation = endLocation;
        }

        public string Name { get; }

        /// <summary>The chunk's own tokens, spaces already arranged.</summary>
        public LinkedList<TokenBase> Tokens { get; }

        /// <summary>The first token; the outer writer spaces the chunk's left edge by it.</summary>
        public TokenBase FirstToken => this.Tokens.First.Value;

        /// <summary>The last token; the outer writer spaces the chunk's right edge by it.</summary>
        public TokenBase LastToken => this.Tokens.Last.Value;

        /// <summary>Rendered text at depth 0.</summary>
        public string Text { get; }

        /// <summary>Mappings emitted while rendering, relative to the chunk start.</summary>
        public List<ChunkSegment> Segments { get; }

        /// <summary>Generated line of the chunk end, relative to the chunk start.</summary>
        public int EndLine { get; }

        /// <summary>Generated column of the chunk end, at depth 0.</summary>
        public int EndColumn { get; }

        /// <summary>The writer's last location after the chunk's last token.</summary>
        public Location EndLocation { get; }
    }

    /// <summary>
    /// One source-map segment of a chunk. <see cref="Line"/> is relative to the chunk start;
    /// <see cref="Column"/> is at depth 0.
    /// </summary>
    internal readonly struct ChunkSegment
    {
        public ChunkSegment(int line, int column, bool isSelf, bool isLineStart, int sourceLine, int sourceColumn, string file, string name)
        {
            this.Line = line;
            this.Column = column;
            this.IsSelf = isSelf;
            this.IsLineStart = isLineStart;
            this.SourceLine = sourceLine;
            this.SourceColumn = sourceColumn;
            this.File = file;
            this.Name = name;
        }

        public int Line { get; }

        public int Column { get; }

        /// <summary>Maps the generated position to itself in the generated file.</summary>
        public bool IsSelf { get; }

        /// <summary>The column-0 mapping a writer newline adds; it is not re-indented.</summary>
        public bool IsLineStart { get; }

        public int SourceLine { get; }

        public int SourceColumn { get; }

        public string File { get; }

        public string Name { get; }
    }
}
