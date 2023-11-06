using System.Collections.Generic;

namespace Vault.Encryption
{
    public abstract class Encryptor
    {
        public abstract bool RequireCredentials { get; }
        public abstract void SetCredentials(string seed);
        public abstract IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData);
    }
}