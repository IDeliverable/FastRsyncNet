using System.Collections.Generic;
using FastRsync.Core;
using FastRsync.Hash;

namespace FastRsync.Signature
{
    public class SignatureMetadata
    {
        public string ChunkHashAlgorithm { get; set; }
        public string RollingChecksumAlgorithm { get; set; }
        public string BaseFileHashAlgorithm { get; set; }
        public string BaseFileHash { get; set; }
    }

    public enum RsyncFormatType
    {
        Octodiff,
        FastRsync
    }

    public class Signature
    {
        public Signature(SignatureMetadata metadata, RsyncFormatType type)
        {
            HashAlgorithm = SupportedAlgorithms.Hashing.Create(metadata.ChunkHashAlgorithm);
            RollingChecksumAlgorithm = SupportedAlgorithms.Checksum.Create(metadata.RollingChecksumAlgorithm);
            Chunks = new List<ChunkSignature>();
            Metadata = metadata;
            Type = type;
        }

        public IHashAlgorithm HashAlgorithm { get; private set; }
        public IRollingChecksum RollingChecksumAlgorithm { get; private set; }
        public List<ChunkSignature> Chunks { get; private set; } 
        public SignatureMetadata Metadata { get; private set; }
        public RsyncFormatType Type { get; private set; }
    }
}