using System;
using System.IO;
using System.Threading.Tasks;
using FastRsync.Core;
using FastRsync.Delta;
using FastRsync.Diagnostics;
using FastRsync.Signature;
using NSubstitute;
using NUnit.Framework;

namespace FastRsync.Tests
{
    [TestFixture]
    public class PatchingTests
    {
        [Test]
        [TestCase(1378, 129, SignatureBuilder.MinimumChunkSize)]
        [TestCase(1378, 129, SignatureBuilder.DefaultChunkSize)]
        [TestCase(1378, 129, SignatureBuilder.MaximumChunkSize)]
        [TestCase(16974, 8452, SignatureBuilder.MinimumChunkSize)]
        [TestCase(16974, 8452, SignatureBuilder.DefaultChunkSize)]
        [TestCase(16974, 8452, SignatureBuilder.MaximumChunkSize)]
        [TestCase(6666, 6666, SignatureBuilder.MinimumChunkSize)]
        [TestCase(6666, 6666, SignatureBuilder.DefaultChunkSize)]
        [TestCase(6666, 6666, SignatureBuilder.MaximumChunkSize)]
        public async Task PatchingAsyncXXHash_ForNewData_PatchesFile(int baseNumberOfBytes, int newDataNumberOfBytes, short chunkSize)
        {
            // Arrange
            var baseData = new byte[baseNumberOfBytes];
            new Random().NextBytes(baseData);
            var baseDataStream = new MemoryStream(baseData);
            var baseSignatureStream = new MemoryStream();

            var signatureBuilder = new SignatureBuilder
            {
                ChunkSize = chunkSize
            };
            await signatureBuilder.BuildAsync(baseDataStream, new SignatureWriter(baseSignatureStream)).ConfigureAwait(false);
            baseSignatureStream.Seek(0, SeekOrigin.Begin);

            var newData = new byte[newDataNumberOfBytes];
            new Random().NextBytes(newData);
            var newDataStream = new MemoryStream(newData);

            var progressReporter = Substitute.For<IProgress<ProgressReport>>();

            // Act
            var deltaStream = new MemoryStream();
            var deltaBuilder = new DeltaBuilder();
            await deltaBuilder.BuildDeltaAsync(newDataStream, new SignatureReader(baseSignatureStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream))).ConfigureAwait(false);
            deltaStream.Seek(0, SeekOrigin.Begin);

            var patchedDataStream = new MemoryStream();
            var deltaApplier = new DeltaApplier();
            await deltaApplier.ApplyAsync(baseDataStream, new BinaryDeltaReader(deltaStream, progressReporter), patchedDataStream).ConfigureAwait(false);

            // Assert
            CollectionAssert.AreEqual(newData, patchedDataStream.ToArray());
            progressReporter.Received().Report(Arg.Any<ProgressReport>());
        }

        [Test]
        [TestCase(1378, 129, SignatureBuilder.MinimumChunkSize)]
        [TestCase(1378, 129, SignatureBuilder.DefaultChunkSize)]
        [TestCase(1378, 129, SignatureBuilder.MaximumChunkSize)]
        [TestCase(16974, 8452, SignatureBuilder.MinimumChunkSize)]
        [TestCase(16974, 8452, SignatureBuilder.DefaultChunkSize)]
        [TestCase(16974, 8452, SignatureBuilder.MaximumChunkSize)]
        [TestCase(6666, 6666, SignatureBuilder.MinimumChunkSize)]
        [TestCase(6666, 6666, SignatureBuilder.DefaultChunkSize)]
        [TestCase(6666, 6666, SignatureBuilder.MaximumChunkSize)]
        public void PatchingSyncXXHash_ForNewData_PatchesFile(int baseNumberOfBytes, int newDataNumberOfBytes, short chunkSize)
        {
            // Arrange
            var baseData = new byte[baseNumberOfBytes];
            new Random().NextBytes(baseData);
            var baseDataStream = new MemoryStream(baseData);
            var baseSignatureStream = new MemoryStream();

            var signatureBuilder = new SignatureBuilder
            {
                ChunkSize = chunkSize
            };
            signatureBuilder.Build(baseDataStream, new SignatureWriter(baseSignatureStream));
            baseSignatureStream.Seek(0, SeekOrigin.Begin);

            var newData = new byte[newDataNumberOfBytes];
            new Random().NextBytes(newData);
            var newDataStream = new MemoryStream(newData);

            var progressReporter = Substitute.For<IProgress<ProgressReport>>();

            // Act
            var deltaStream = new MemoryStream();
            var deltaBuilder = new DeltaBuilder();
            deltaBuilder.BuildDelta(newDataStream, new SignatureReader(baseSignatureStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            deltaStream.Seek(0, SeekOrigin.Begin);

            var patchedDataStream = new MemoryStream();
            var deltaApplier = new DeltaApplier();
            deltaApplier.Apply(baseDataStream, new BinaryDeltaReader(deltaStream, progressReporter), patchedDataStream);

            // Assert
            CollectionAssert.AreEqual(newData, patchedDataStream.ToArray());
            progressReporter.Received().Report(Arg.Any<ProgressReport>());
        }
    }
}
