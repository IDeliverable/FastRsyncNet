using System;
using System.IO;
using System.Threading.Tasks;
using FastRsync.Core;
using FastRsync.Diagnostics;
using FastRsync.Hash;

namespace FastRsync.Signature
{
    public class SignatureBuilder
    {
        public const short MinimumChunkSize = 128;
        public const short DefaultChunkSize = 2048;
        public const short MaximumChunkSize = 31 * 1024;

        private short chunkSize;

        public SignatureBuilder() : this(SupportedAlgorithms.Hashing.Default(), SupportedAlgorithms.Checksum.Default())
        {
        }

        public SignatureBuilder(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm)
        {
            HashAlgorithm = hashAlgorithm;
            RollingChecksumAlgorithm = rollingChecksumAlgorithm;
            ChunkSize = DefaultChunkSize;
            ProgressReport = null;
        }

        public IProgress<ProgressReport> ProgressReport { get; set; }

        public IHashAlgorithm HashAlgorithm { get; set; }

        public IRollingChecksum RollingChecksumAlgorithm { get; set; }

        public short ChunkSize
        {
            get => chunkSize;
            set
            {
                if (value < MinimumChunkSize)
                    throw new ArgumentException($"Chunk size cannot be less than {MinimumChunkSize}");
                if (value > MaximumChunkSize)
                    throw new ArgumentException($"Chunk size cannot be exceed {MaximumChunkSize}");
                chunkSize = value;
            }
        }

        public void Build(Stream stream, ISignatureWriter signatureWriter)
        {
            WriteMetadata(stream, signatureWriter);
            WriteChunkSignatures(stream, signatureWriter);
        }

        public async Task BuildAsync(Stream stream, ISignatureWriter signatureWriter)
        {
            await WriteMetadataAsync(stream, signatureWriter).ConfigureAwait(false);
            await WriteChunkSignaturesAsync(stream, signatureWriter).ConfigureAwait(false);
        }

        private void WriteMetadata(Stream stream, ISignatureWriter signatureWriter)
        {
            ProgressReport?.Report(new ProgressReport
            {
                Operation = ProgressOperationType.HashingFile,
                CurrentPosition = 0,
                Total = stream.Length
            });

            stream.Seek(0, SeekOrigin.Begin);
            signatureWriter.WriteMetadata(HashAlgorithm, RollingChecksumAlgorithm);

            ProgressReport?.Report(new ProgressReport
            {
                Operation = ProgressOperationType.HashingFile,
                CurrentPosition = stream.Length,
                Total = stream.Length
            });
        }

        private async Task WriteMetadataAsync(Stream stream, ISignatureWriter signatureWriter)
        {
            ProgressReport?.Report(new ProgressReport
            {
                Operation = ProgressOperationType.HashingFile,
                CurrentPosition = 0,
                Total = stream.Length
            });

            stream.Seek(0, SeekOrigin.Begin);
            await signatureWriter.WriteMetadataAsync(HashAlgorithm, RollingChecksumAlgorithm).ConfigureAwait(false);

            ProgressReport?.Report(new ProgressReport
            {
                Operation = ProgressOperationType.HashingFile,
                CurrentPosition = stream.Length,
                Total = stream.Length
            });
        }

        private void WriteChunkSignatures(Stream stream, ISignatureWriter signatureWriter)
        {
            var checksumAlgorithm = RollingChecksumAlgorithm;
            var hashAlgorithm = HashAlgorithm;

            ProgressReport?.Report(new ProgressReport
            {
                Operation = ProgressOperationType.BuildingSignatures,
                CurrentPosition = 0,
                Total = stream.Length
            });
            stream.Seek(0, SeekOrigin.Begin);

            long start = 0;
            int read;
            var block = new byte[ChunkSize];
            while ((read = stream.Read(block, 0, block.Length)) > 0)
            {
                signatureWriter.WriteChunk(new ChunkSignature
                {
                    StartOffset = start,
                    Length = (short)read,
                    Hash = hashAlgorithm.ComputeHash(block, 0, read),
                    RollingChecksum = checksumAlgorithm.Calculate(block, 0, read)
                });

                start += read;
                ProgressReport?.Report(new ProgressReport
                {
                    Operation = ProgressOperationType.BuildingSignatures,
                    CurrentPosition = start,
                    Total = stream.Length
                });
            }
        }

        private async Task WriteChunkSignaturesAsync(Stream stream, ISignatureWriter signatureWriter)
        {
            var checksumAlgorithm = RollingChecksumAlgorithm;
            var hashAlgorithm = HashAlgorithm;

            ProgressReport?.Report(new ProgressReport
            {
                Operation = ProgressOperationType.BuildingSignatures,
                CurrentPosition = 0,
                Total = stream.Length
            });
            stream.Seek(0, SeekOrigin.Begin);

            long start = 0;
            int read;
            var block = new byte[ChunkSize];
            while ((read = await stream.ReadAsync(block, 0, block.Length).ConfigureAwait(false)) > 0)
            {
                await signatureWriter.WriteChunkAsync(new ChunkSignature
                {
                    StartOffset = start,
                    Length = (short)read,
                    Hash = hashAlgorithm.ComputeHash(block, 0, read),
                    RollingChecksum = checksumAlgorithm.Calculate(block, 0, read)
                }).ConfigureAwait(false);

                start += read;
                ProgressReport?.Report(new ProgressReport
                {
                    Operation = ProgressOperationType.BuildingSignatures,
                    CurrentPosition = start,
                    Total = stream.Length
                });
            }
        }
    }
}