using System.Collections.Generic;

namespace Vault.Encryption
{
    public abstract class Encryptor
    {
        public abstract IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData);
    }
}