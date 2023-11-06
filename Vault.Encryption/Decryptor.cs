using System.Collections.Generic;

namespace Vault.Encryption
{
    public abstract class Decryptor
    {
        public abstract bool RequireCredentials { get; }
        public abstract void SetCredentials(string seed);
        public abstract IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData);
    }
}