using System;

namespace FastRsync.Core
{
    public class ChunkSignature
    {
        public long StartOffset;            // 8 (but not included in the file on disk)
        public short Length;                // 2
        public byte[] Hash;                 // depending on hash (20 for SHA1, 8 for xxHash64)
        public UInt32 RollingChecksum;      // 4

        public override string ToString()
        {
            return string.Format("{0,6}:{1,6} |{2,20}| {3}", StartOffset, Length, RollingChecksum, BitConverter.ToString(Hash).ToLowerInvariant().Replace("-", ""));
        }
    }
}