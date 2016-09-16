using System.Collections.Generic;
using FastRsync.Core;
using FastRsync.Hash;

namespace FastRsync.Signature
{
    public class Signature
    {
        public Signature(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm)
        {
            HashAlgorithm = hashAlgorithm;
            RollingChecksumAlgorithm = rollingChecksumAlgorithm;
            Chunks = new List<ChunkSignature>();
        }

        public IHashAlgorithm HashAlgorithm { get; private set; }
        public IRollingChecksum RollingChecksumAlgorithm { get; private set; }
        public List<ChunkSignature> Chunks { get; private set; } 
    }
}