using System.IO;
using System.Threading.Tasks;
using FastRsync.Hash;

namespace FastRsync.Delta
{
    public interface IDeltaWriter
    {
        void WriteMetadata(IHashAlgorithm hashAlgorithm, byte[] expectedNewFileHash);
        void WriteCopyCommand(DataRange segment);
        void WriteDataCommand(Stream source, long offset, long length);
        Task WriteDataCommandAsync(Stream source, long offset, long length);
        void Finish();
    }
}