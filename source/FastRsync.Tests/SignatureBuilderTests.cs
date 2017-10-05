using System;
using System.IO;
using System.Security.Cryptography;
using FastRsync.Core;
using FastRsync.Hash;
using FastRsync.Signature;
using NUnit.Framework;

namespace FastRsync.Tests
{
    [TestFixture]
    public class SignatureBuilderTests
    {
        [Test]
        [TestCase(2, SignatureBuilder.MinimumChunkSize)]
        [TestCase(10, SignatureBuilder.MinimumChunkSize)]
        [TestCase(16974, SignatureBuilder.MinimumChunkSize)]
        [TestCase(2, SignatureBuilder.DefaultChunkSize)]
        [TestCase(10, SignatureBuilder.DefaultChunkSize)]
        [TestCase(16974, SignatureBuilder.DefaultChunkSize)]
        [TestCase(2, SignatureBuilder.MaximumChunkSize)]
        [TestCase(10, SignatureBuilder.MaximumChunkSize)]
        [TestCase(16974, SignatureBuilder.MaximumChunkSize)]
        public void SignatureBuilderXXHash_ForRandomData_BuildsSignature(int numberOfBytes, short chunkSize)
        {
            // Arrange
            var data = new byte[numberOfBytes];
            new Random().NextBytes(data);
            var dataStream = new MemoryStream(data);
            var signatureStream = new MemoryStream();

            // Act
            var target = new SignatureBuilder
            {
                ChunkSize = chunkSize
            };
            target.Build(dataStream, new SignatureWriter(signatureStream));

            // Assert
            Assert.Greater(signatureStream.Length, 8+5+7+3);

            signatureStream.Seek(0, SeekOrigin.Begin);
            var sig = new SignatureReader(signatureStream, null).ReadSignature();
            Assert.AreEqual(new XxHashAlgorithm().Name, sig.HashAlgorithm.Name);
            Assert.AreEqual(new XxHashAlgorithm().HashLength, sig.HashAlgorithm.HashLength);
            Assert.AreEqual(new Adler32RollingChecksum().Name, sig.RollingChecksumAlgorithm.Name);
        }

        [Test]
        [TestCase(2, SignatureBuilder.MinimumChunkSize)]
        [TestCase(10, SignatureBuilder.MinimumChunkSize)]
        [TestCase(16974, SignatureBuilder.MinimumChunkSize)]
        [TestCase(2, SignatureBuilder.DefaultChunkSize)]
        [TestCase(10, SignatureBuilder.DefaultChunkSize)]
        [TestCase(16974, SignatureBuilder.DefaultChunkSize)]
        [TestCase(2, SignatureBuilder.MaximumChunkSize)]
        [TestCase(10, SignatureBuilder.MaximumChunkSize)]
        [TestCase(16974, SignatureBuilder.MaximumChunkSize)]
        public void SignatureBuilderSha1_ForRandomData_BuildsSignature(int numberOfBytes, short chunkSize)
        {
            // Arrange
            var data = new byte[numberOfBytes];
            new Random().NextBytes(data);
            var dataStream = new MemoryStream(data);
            var signatureStream = new MemoryStream();

            // Act
            var target = new SignatureBuilder(SupportedAlgorithms.Hashing.Sha1(), SupportedAlgorithms.Checksum.Adler32Rolling())
            {
                ChunkSize = chunkSize
            };
            target.Build(dataStream, new SignatureWriter(signatureStream));

            // Assert
            Assert.Greater(signatureStream.Length, 8 + 4 + 7 + 3);

            signatureStream.Seek(0, SeekOrigin.Begin);
            var sig = new SignatureReader(signatureStream, null).ReadSignature();
            Assert.AreEqual(new HashAlgorithmWrapper("SHA1", SHA1.Create()).Name, sig.HashAlgorithm.Name);
            Assert.AreEqual(new HashAlgorithmWrapper("SHA1", SHA1.Create()).HashLength, sig.HashAlgorithm.HashLength);
            Assert.AreEqual(new Adler32RollingChecksum().Name, sig.RollingChecksumAlgorithm.Name);
        }
    }
}
