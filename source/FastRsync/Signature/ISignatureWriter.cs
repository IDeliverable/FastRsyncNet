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
            var ms = new MemoryStream();
            var msbw = new BinaryWriter(ms);
            msbw.Write(BinaryFormat.SignatureHeader);
            msbw.Write(BinaryFormat.Version);
            msbw.Write(hashAlgorithm.Name);
            msbw.Write(rollingChecksumAlgorithm.Name);
            msbw.Write(BinaryFormat.EndOfMetadata);
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(signaturebw.BaseStream).ConfigureAwait(false);
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