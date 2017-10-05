using System.IO;
using System.Threading.Tasks;
using FastRsync.Core;
using FastRsync.Hash;

namespace FastRsync.Signature
{
    public interface ISignatureWriter
    {
        void WriteMetadata(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm);
        Task WriteMetadataAsync(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm);
        void WriteChunk(ChunkSignature signature);
        Task WriteChunkAsync(ChunkSignature signature);
    }

    public class SignatureWriter : ISignatureWriter
    {
        private readonly BinaryWriter signaturebw;

        public SignatureWriter(Stream signatureStream)
        {
            this.signaturebw = new BinaryWriter(signatureStream);
        }

        public void WriteMetadata(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm)
        {
            signaturebw.Write(BinaryFormat.SignatureHeader);
            signaturebw.Write(BinaryFormat.Version);
            signaturebw.Write(hashAlgorithm.Name);
            signaturebw.Write(rollingChecksumAlgorithm.Name);
            signaturebw.Write(BinaryFormat.EndOfMetadata);
        }

        public async Task WriteMetadataAsync(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm)
        {
            signaturebw.Write(BinaryFormat.SignatureHeader);
            signaturebw.Write(BinaryFormat.Version);
            signaturebw.Write(hashAlgorithm.Name);
            signaturebw.Write(rollingChecksumAlgorithm.Name);
            signaturebw.Write(BinaryFormat.EndOfMetadata);
        }

        public void WriteChunk(ChunkSignature signature)
        {
            signaturebw.Write(signature.Length);
            signaturebw.Write(signature.RollingChecksum);
            signaturebw.Write(signature.Hash);
        }

        public async Task WriteChunkAsync(ChunkSignature signature)
        {
            signaturebw.Write(signature.Length);
            signaturebw.Write(signature.RollingChecksum);
            await signaturebw.BaseStream.WriteAsync(signature.Hash, 0, signature.Hash.Length).ConfigureAwait(false);
        }
    }
}