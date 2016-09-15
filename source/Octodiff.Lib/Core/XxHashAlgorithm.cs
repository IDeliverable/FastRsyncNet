using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octodiff.Core;
using System.Data.HashFunction;

namespace Octodiff.Lib.Core
{
    public class XxHashAlgorithm : IHashAlgorithm
    {
        public string Name => "XXH64";
        public int HashLength => 64/8;

        private readonly System.Data.HashFunction.xxHash algorithm;

        public XxHashAlgorithm()
        {
            algorithm = new System.Data.HashFunction.xxHash(64);
        }

        public byte[] ComputeHash(Stream stream)
        {
            return algorithm.ComputeHash(stream);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int length)
        {
            byte[] data = new byte[length];
            Buffer.BlockCopy(buffer, offset, data, 0, length);
            return algorithm.ComputeHash(data);
        }
    }
}
