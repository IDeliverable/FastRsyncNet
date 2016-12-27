namespace FastRsync.Diagnostics
{
    public enum ProgressOperationType
    {
        BuildingSignatures,
        HashingFile,
        ReadingSignature,
        CreatingChunkMap,
        BuildingDelta,
        ApplyingDelta
    }

    public sealed class ProgressReport
    {
        public ProgressOperationType Operation { get; internal set; }

        public long CurrentPosition { get; internal set; }

        public long Total { get; internal set; }
    }
}