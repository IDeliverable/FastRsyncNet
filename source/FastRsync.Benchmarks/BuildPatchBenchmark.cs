using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using FastRsync.Core;
using FastRsync.Delta;
using FastRsync.Signature;

namespace FastRsync.Benchmarks
{
    public class BuildPatchBenchmark
    {
        [Params(128, 16974)]
        public int BaseFileSize { get; set; }

        [Params(16974, 128)]
        public int NewFileSize { get; set; }

        private byte[] baseFileSignaturexxHash;
        private byte[] baseFileSignatureSha1;
        private byte[] baseFileSignatureMd5;
        private byte[] newFileData;

        private readonly DeltaBuilder deltaBuilder = new DeltaBuilder();

        [GlobalSetup]
        public void GlobalSetup()
        {
            var baseFileBytes = new byte[BaseFileSize];
            var rnd = new Random();
            rnd.NextBytes(baseFileBytes);

            newFileData = new byte[NewFileSize];
            rnd.NextBytes(newFileData);

            var baseDataStream = new MemoryStream(baseFileBytes);

            {
                var xxHashSignatureBuilder = new SignatureBuilder(SupportedAlgorithms.Hashing.XxHash(),
                    SupportedAlgorithms.Checksum.Adler32Rolling());
                var baseSignaturexxHashStream = new MemoryStream();
                xxHashSignatureBuilder.Build(baseDataStream, new SignatureWriter(baseSignaturexxHashStream));
                baseFileSignaturexxHash = baseSignaturexxHashStream.ToArray();
            }

            {
                var sha1SignatureBuilder = new SignatureBuilder(SupportedAlgorithms.Hashing.Sha1(),
                    SupportedAlgorithms.Checksum.Adler32Rolling());
                baseDataStream.Seek(0, SeekOrigin.Begin);
                var baseSignatureSha1Stream = new MemoryStream();
                sha1SignatureBuilder.Build(baseDataStream, new SignatureWriter(baseSignatureSha1Stream));
                baseFileSignatureSha1 = baseSignatureSha1Stream.ToArray();
            }

            {
                var md5SignatureBuilder = new SignatureBuilder(SupportedAlgorithms.Hashing.Md5(),
                    SupportedAlgorithms.Checksum.Adler32Rolling());
                baseDataStream.Seek(0, SeekOrigin.Begin);
                var baseSignatureMd5Stream = new MemoryStream();
                md5SignatureBuilder.Build(baseDataStream, new SignatureWriter(baseSignatureMd5Stream));
                baseFileSignatureMd5 = baseSignatureMd5Stream.ToArray();
            }
        }

        [Benchmark]
        public byte[] BuildPatchxxHash()
        {
            var deltaStream = new MemoryStream();
            var baseSignatureStream = new MemoryStream(baseFileSignaturexxHash);
            var newDataStream = new MemoryStream(newFileData);
            deltaBuilder.BuildDelta(newDataStream, new SignatureReader(baseSignatureStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));

            return deltaStream.ToArray();
        }

        [Benchmark]
        public byte[] BuildPatchSha1()
        {
            var deltaStream = new MemoryStream();
            var baseSignatureStream = new MemoryStream(baseFileSignatureSha1);
            var newDataStream = new MemoryStream(newFileData);
            deltaBuilder.BuildDelta(newDataStream, new SignatureReader(baseSignatureStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));

            return deltaStream.ToArray();
        }

        [Benchmark]
        public byte[] BuildPatchMd5()
        {
            var deltaStream = new MemoryStream();
            var baseSignatureStream = new MemoryStream(baseFileSignatureMd5);
            var newDataStream = new MemoryStream(newFileData);
            deltaBuilder.BuildDelta(newDataStream, new SignatureReader(baseSignatureStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));

            return deltaStream.ToArray();
        }
    }
}