using System.IO;
using System.Threading.Tasks;

namespace FastRsync.Hash
{
    public interface IHashAlgorithm
    {
        string Name { get; }
        int HashLength { get; }
        byte[] ComputeHash(Stream stream);
        Task<byte[]> ComputeHashAsync(Stream stream);
        byte[] ComputeHash(byte[] buffer, int offset, int length);
    }
}