using OrderedSerializer;

namespace Vault.Encryption
{
    public interface IEncryptionSource
    {
        Encryptor ConstructEncryptor();
        Decryptor ConstructDecryptor();
    }
    
    public abstract class EncryptionSource : IEncryptionSource, IVersionedDataStruct
    {
        public abstract Encryptor ConstructEncryptor();
        public abstract Decryptor ConstructDecryptor();
        public abstract void Serialize(IOrderedSerializer serializer);
        public virtual byte Version => 0;
    }
}