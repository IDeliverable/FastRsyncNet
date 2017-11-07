using System;
using System.IO;
using System.Threading.Tasks;
using FastRsync.Core;
using FastRsync.Hash;

namespace FastRsync.Delta
{
    public class BinaryDeltaWriter : IDeltaWriter
    {
        private readonly BinaryWriter writer;
        private readonly int readWriteBufferSize;

        public BinaryDeltaWriter(Stream stream, int readWriteBufferSize = 1024 * 1024)
        {
            writer = new BinaryWriter(stream);
            this.readWriteBufferSize = readWriteBufferSize;
        }

        public void WriteMetadata(IHashAlgorithm hashAlgorithm, byte[] expectedNewFileHash)
        {
            writer.Write(BinaryFormat.DeltaHeader);
            writer.Write(BinaryFormat.Version);
            writer.Write(hashAlgorithm.Name);
            writer.Write(expectedNewFileHash.Length);
            writer.Write(expectedNewFileHash);
            writer.Write(BinaryFormat.EndOfMetadata);
        }

        public void WriteCopyCommand(DataRange segment)
        {
            writer.Write(BinaryFormat.CopyCommand);
            writer.Write(segment.StartOffset);
            writer.Write(segment.Length);
        }

        public void WriteDataCommand(Stream source, long offset, long length)
        {
            writer.Write(BinaryFormat.DataCommand);
            writer.Write(length);

            var originalPosition = source.Position;
            try
            {
                source.Seek(offset, SeekOrigin.Begin);

                var buffer = new byte[Math.Min((int)length, readWriteBufferSize)];

                int read;
                long soFar = 0;
                while ((read = source.Read(buffer, 0, (int)Math.Min(length - soFar, buffer.Length))) > 0)
                {
                    soFar += read;
                    writer.Write(buffer, 0, read);
                }
            }
            finally
            {
                source.Seek(originalPosition, SeekOrigin.Begin);
            }
        }

        public async Task WriteDataCommandAsync(Stream source, long offset, long length)
        {
            writer.Write(BinaryFormat.DataCommand);
            writer.Write(length);

            var originalPosition = source.Position;
            try
            {
                source.Seek(offset, SeekOrigin.Begin);

                var buffer = new byte[Math.Min((int)length, readWriteBufferSize)];

                int read;
                long soFar = 0;
                while ((read = await source.ReadAsync(buffer, 0, (int)Math.Min(length - soFar, buffer.Length)).ConfigureAwait(false)) > 0)
                {
                    soFar += read;
                    await writer.BaseStream.WriteAsync(buffer, 0, read).ConfigureAwait(false);
                }
            }
            finally
            {
                source.Seek(originalPosition, SeekOrigin.Begin);
            }
        }

        public void Finish()
        {
        }
    }
}