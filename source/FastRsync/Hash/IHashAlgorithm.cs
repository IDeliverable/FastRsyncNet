using System.IO;

namespace FastRsync.Hash
{
    public interface IHashAlgorithm
    {
        string Name { get; }
        int HashLength { get; }
        byte[] ComputeHash(Stream stream);
        byte[] ComputeHash(byte[] buffer, int offset, int length);
    }
}