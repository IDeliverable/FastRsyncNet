namespace FastRsync.Delta
{
    public class DeltaMetadata
    {
        public string HashAlgorithm { get; set; }
        public string ExpectedFileHashAlgorithm { get; set; }
        public string ExpectedFileHash { get; set; }
    }
}
