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
        private readonly IRollingChecksum adler32RollingV2Algorithm = SupportedAlgorithms.Checksum.Adler32RollingV2();

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

        [Benchmark]
        public uint Adler32RollingV2CalculateChecksum()
        {
            return adler32RollingV2Algorithm.Calculate(data, 0, data.Length);
        }
    }
}