using System;
using System.Data.HashFunction.xxHash;
using System.IO;
using System.Threading.Tasks;

namespace FastRsync.Hash
{
    public class XxHashAlgorithm : IHashAlgorithm
    {
        public string Name => "XXH64";
        public int HashLength => 64/8;

        private readonly IxxHash algorithm;

        public XxHashAlgorithm()
        {
            algorithm = xxHashFactory.Instance.Create(new xxHashConfig
            {
                HashSizeInBits = 64
            });
        }

        public byte[] ComputeHash(Stream stream)
        {
            return algorithm.ComputeHash(stream).Hash;
        }

        public async Task<byte[]> ComputeHashAsync(Stream stream)
        {
            return (await algorithm.ComputeHashAsync(stream).ConfigureAwait(false)).Hash;
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(buffer, offset, data, 0, length);
            return algorithm.ComputeHash(data).Hash;
        }
    }
}
