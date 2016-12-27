using System;
using System.IO;
using FastRsync.Core;
using FastRsync.Diagnostics;
using FastRsync.Exceptions;
using FastRsync.Hash;

namespace FastRsync.Signature
{
    public class SignatureBuilder
    {
        public const short MinimumChunkSize = 128;
        public const short DefaultChunkSize = 2048;
        public const short MaximumChunkSize = 31 * 1024;

        private short chunkSize;

        public SignatureBuilder()
        {
            ChunkSize = DefaultChunkSize;
            HashAlgorithm = SupportedAlgorithms.Hashing.Default();
            RollingChecksumAlgorithm = SupportedAlgorithms.Checksum.Default();
            ProgressReport = null;
        }

        public IProgress<ProgressReport> ProgressReport { get; set; }

        public IHashAlgorithm HashAlgorithm { get; set; }

        public IRollingChecksum RollingChecksumAlgorithm { get; set; }

        public short ChunkSize
        {
            get { return chunkSize; }
            set
            {
                if (value < MinimumChunkSize)
                    throw new UsageException($"Chunk size cannot be less than {MinimumChunkSize}");
                if (value > MaximumChunkSize)
                    throw new UsageException($"Chunk size cannot be exceed {MaximumChunkSize}");
                chunkSize = value;
            }
        }

        public void Build(Stream stream, ISignatureWriter signatureWriter)
        {
            WriteMetadata(stream, signatureWriter);
            WriteChunkSignatures(stream, signatureWriter);
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

            var hash = HashAlgorithm.ComputeHash(stream);

            signatureWriter.WriteMetadata(HashAlgorithm, RollingChecksumAlgorithm, hash);

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
    }
}