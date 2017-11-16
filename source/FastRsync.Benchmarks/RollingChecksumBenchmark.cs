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
        [Params(128, 16974, 356879)]
        public int N { get; set; }

        private byte[] data;

        private readonly IRollingChecksum adler32RollingAlgorithm = SupportedAlgorithms.Checksum.Adler32Rolling();

        [GlobalSetup]
        public void GlobalSetup()
        {
            data = new byte[N];
            new Random().NextBytes(data);
        }

        [Benchmark]
        public uint Adler32RollingCalculateChecksum()
        {
            return adler32RollingAlgorithm.Calculate(data, 0, data.Length);
        }
    }
}