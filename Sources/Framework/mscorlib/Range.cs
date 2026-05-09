namespace System
{
    /// <summary>
    /// Represents a range that has a start and end. Mirrors the C# 8
    /// <c>System.Range</c> shape so that Roslyn binds <c>x..y</c>,
    /// <c>x..</c>, <c>..y</c>, <c>..</c>, and <c>arr[x..y]</c> against
    /// this facade.
    /// </summary>
    public readonly struct Range
    {
        public Index Start { get; }

        public Index End { get; }

        public Range(Index start, Index end)
        {
            this.Start = start;
            this.End = end;
        }

        public static Range All => new Range(Index.Start, Index.End);

        public static Range StartAt(Index start) => new Range(start, Index.End);

        public static Range EndAt(Index end) => new Range(Index.Start, end);
    }
}
