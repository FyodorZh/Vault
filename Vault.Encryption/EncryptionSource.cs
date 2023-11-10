using System.Collections.Generic;
using OrderedSerializer;

namespace Vault.Encryption
{
    public interface IEncryptionSource
    {
        EncryptionDesc GetDescription();
        IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData);
        IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData);
    }
    
    public abstract class EncryptionSource : IEncryptionSource, IVersionedDataStruct
    {
        protected ICredentialsProvider? CredentialsProvider { get; private set; }

        public abstract EncryptionDesc GetDescription();

        public void SetCredentials(ICredentialsProvider credentialsProvider)
        {
            CredentialsProvider = credentialsProvider;
        }
        
        public abstract IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData);
        public abstract IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData);

        public abstract void Serialize(IOrderedSerializer serializer);
        public virtual byte Version => 0;
    }
}