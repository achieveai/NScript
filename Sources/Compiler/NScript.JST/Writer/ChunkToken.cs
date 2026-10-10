//-----------------------------------------------------------------------
// <copyright file="ChunkToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST.Writer
{
    using NScript.Utils;

    /// <summary>
    /// A rendered chunk in a writer's token list.
    /// </summary>
    internal sealed class ChunkToken : TokenBase
    {
        public ChunkToken(RenderedChunk chunk)
            : base(TokenType.Chunk, chunk.FirstToken.Location)
        {
            this.Chunk = chunk;
        }

        public RenderedChunk Chunk { get; }

        public string Name => this.Chunk.Name;

        public TokenBase FirstToken => this.Chunk.FirstToken;

        public TokenBase LastToken => this.Chunk.LastToken;

        public string Text => this.Chunk.Text;

        public System.Collections.Generic.List<ChunkSegment> Segments => this.Chunk.Segments;

        public int EndLine => this.Chunk.EndLine;

        public int EndColumn => this.Chunk.EndColumn;

        public Location EndLocation => this.Chunk.EndLocation;
    }

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

        public bool IsSelf { get; }

        public bool IsLineStart { get; }

        public int SourceLine { get; }

        public int SourceColumn { get; }

        public string File { get; }

        public string Name { get; }
    }
}

namespace NScript.JST
{
    using System.Collections.Generic;
    using NScript.JST.Writer;
    using NScript.Utils;

    /// <summary>
    /// One function rendered on its own: its text and its source-map segments relative to
    /// its start. A writer splices it in at any depth, so a later build can write it again
    /// without converting or rendering the function.
    /// </summary>
    public sealed class RenderedChunk
    {
        internal RenderedChunk(
            string name,
            TokenBase firstToken,
            TokenBase lastToken,
            string text,
            List<ChunkSegment> segments,
            int endLine,
            int endColumn,
            Location endLocation)
        {
            this.Name = name;
            this.FirstToken = firstToken;
            this.LastToken = lastToken;
            this.Text = text;
            this.Segments = segments;
            this.EndLine = endLine;
            this.EndColumn = endColumn;
            this.EndLocation = endLocation;
        }

        /// <summary>The function's name as written, or "(anonymous)".</summary>
        public string Name { get; }

        /// <summary>The location current when the chunk starts.</summary>
        public Location FirstLocation => this.FirstToken.Location;

        /// <summary>The location current when the chunk ends.</summary>
        public Location EndLocation { get; }

        /// <summary>The text length, for size probes.</summary>
        public int Length => this.Text.Length;

        // Only the edge tokens are kept: spacing and splicing look at nothing else.
        internal TokenBase FirstToken { get; }

        internal TokenBase LastToken { get; }

        internal string Text { get; }

        internal List<ChunkSegment> Segments { get; }

        internal int EndLine { get; }

        internal int EndColumn { get; }
    }
}
