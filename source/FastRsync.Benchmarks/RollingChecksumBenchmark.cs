using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using FastRsync.Core;
using FastRsync.Hash;
using FastRsync.Signature;

namespace FastRsync.Benchmarks
{
    public class RollingCheckSumBenchmark
    {
        private readonly byte[] data;

        private readonly IRollingChecksum adler32RollingAlgorithm = SupportedAlgorithms.Checksum.Adler32Rolling();

        public RollingCheckSumBenchmark()
        {
            data = new byte[16974];
            new Random().NextBytes(data);
        }

        [Benchmark]
        public uint Adler32RollingCalculateChecksum()
        {
            return adler32RollingAlgorithm.Calculate(data, 0, data.Length);
        }
    }
}