using System;
using BenchmarkDotNet.Running;

namespace FastRsync.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SignatureBenchmark>();
            BenchmarkRunner.Run<RollingCheckSumBenchmark>();
        }
    }
}
