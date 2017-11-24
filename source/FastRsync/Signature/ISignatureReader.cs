namespace FastRsync.Signature
{
    public interface ISignatureReader
    {
        Signature ReadSignature();
        Signature ReadSignatureMetadata();
    }
}