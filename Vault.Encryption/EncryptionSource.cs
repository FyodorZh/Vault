using System.Collections.Generic;
using OrderedSerializer;

namespace Vault.Encryption
{
    public interface IEncryptionSource
    {
        bool NeedCredentials { get; }
        bool AddCredentials(string credentials);
        
        EncryptionDesc GetDescription();
        IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData);
        IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData);
    }
    
    public abstract class EncryptionSource : IEncryptionSource, IVersionedDataStruct
    {
        public abstract bool NeedCredentials { get; }
        public abstract bool AddCredentials(string credentials);
        
        public abstract EncryptionDesc GetDescription();
        public abstract IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData);
        public abstract IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData);

        public abstract void Serialize(IOrderedSerializer serializer);
        public virtual byte Version => 0;
    }
}