using System;
using System.Threading.Tasks;
using FastRsync.Hash;

namespace FastRsync.Delta
{
    public interface IDeltaReader
    {
        byte[] ExpectedHash { get; }
        IHashAlgorithm HashAlgorithm { get; }
        void Apply(
            Action<byte[]> writeData,
            Action<long, long> copy
            );

        Task ApplyAsync(
            Func<byte[], Task> writeData,
            Func<long, long, Task> copy
        );
    }
}