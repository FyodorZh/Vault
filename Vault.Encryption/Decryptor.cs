using System.Collections.Generic;

namespace Vault.Encryption
{
    public abstract class Decryptor
    {
        public abstract IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData);
    }
}