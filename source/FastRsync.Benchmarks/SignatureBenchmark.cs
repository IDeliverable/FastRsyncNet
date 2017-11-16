using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using FastRsync.Core;
using FastRsync.Signature;

namespace FastRsync.Benchmarks
{
    public class SignatureBenchmark
    {
        private readonly byte[] data;

        private readonly SignatureBuilder xxHashSignatureBuilder =
            new SignatureBuilder(SupportedAlgorithms.Hashing.XxHash(), SupportedAlgorithms.Checksum.Adler32Rolling());
        private readonly SignatureBuilder sha1SignatureBuilder =
            new SignatureBuilder(SupportedAlgorithms.Hashing.Sha1(), SupportedAlgorithms.Checksum.Adler32Rolling());
        private readonly SignatureBuilder md5SignatureBuilder =
            new SignatureBuilder(SupportedAlgorithms.Hashing.Md5(), SupportedAlgorithms.Checksum.Adler32Rolling());

        public SignatureBenchmark()
        {
            data = new byte[16974];
            new Random().NextBytes(data);
        }

        [Benchmark]
        public byte[] SignaturexxHash()
        {
            var dataStream = new MemoryStream(data);
            var signatureStream = new MemoryStream();
            xxHashSignatureBuilder.Build(dataStream, new SignatureWriter(signatureStream));
            return signatureStream.ToArray();
        } 

        [Benchmark]
        public byte[] SignatureSha1()
        {
            var dataStream = new MemoryStream(data);
            var signatureStream = new MemoryStream();
            sha1SignatureBuilder.Build(dataStream, new SignatureWriter(signatureStream));
            return signatureStream.ToArray();
        }

        [Benchmark]
        public byte[] SignatureMd5()
        {
            var dataStream = new MemoryStream(data);
            var signatureStream = new MemoryStream();
            md5SignatureBuilder.Build(dataStream, new SignatureWriter(signatureStream));
            return signatureStream.ToArray();
        }
    }
}