using OrderedSerializer;

namespace Vault.Encryption
{
    public interface IEncryptionSource
    {
        void SetCredentials(ICredentialsProvider credentialsProvider);
        Encryptor ConstructEncryptor();
        Decryptor ConstructDecryptor();
    }
    
    public abstract class EncryptionSource : IEncryptionSource, IVersionedDataStruct
    {
        protected ICredentialsProvider? CredentialsProvider { get; private set; }

        public void SetCredentials(ICredentialsProvider credentialsProvider)
        {
            CredentialsProvider = credentialsProvider;
        }
        
        public abstract Encryptor ConstructEncryptor();
        public abstract Decryptor ConstructDecryptor();
        public abstract void Serialize(IOrderedSerializer serializer);
        public virtual byte Version => 0;
    }
}