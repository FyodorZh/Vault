using System.Collections.Generic;
using Archivarius;

namespace Vault.Encryption
{
    public interface IEncryptionSource
    {
        bool NeedCredentials { get; }
        bool AddCredentials(string credentials);
        void ClearCredentials();
        
        EncryptionDesc GetDescription();
        IReadOnlyList<byte>? Encrypt(IReadOnlyList<byte> plainData);
        IReadOnlyList<byte>? Decrypt(IReadOnlyList<byte> encryptedData);
    }
    
    public abstract class EncryptionSource : IEncryptionSource, IVersionedDataStruct
    {
        public abstract bool NeedCredentials { get; }
        public abstract bool AddCredentials(string credentials);
        public abstract void ClearCredentials();
        
        public abstract EncryptionDesc GetDescription();
        public abstract IReadOnlyList<byte>? Encrypt(IReadOnlyList<byte> plainData);
        public abstract IReadOnlyList<byte>? Decrypt(IReadOnlyList<byte> encryptedData);

        public abstract void Serialize(ISerializer serializer);
        public virtual byte Version => 0;
    }
}