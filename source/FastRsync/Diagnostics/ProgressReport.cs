namespace FastRsync.Diagnostics
{
    public sealed class ProgressReport
    {
        public string Operation { get; internal set; }

        public long CurrentPosition { get; internal set; }

        public long Total { get; internal set; }
    }
}