using System;
using System.Collections.Generic;

namespace Vault.Encryption
{
    public class ChainDecryptor : Decryptor
    {
        public Decryptor First { get; }
        public Decryptor Second { get; }
        
        public override bool RequireCredentials => false;

        public ChainDecryptor(Decryptor first, Decryptor second)
        {
            First = first;
            Second = second;
        }
        
        public override void SetCredentials(string seed)
        {
            throw new InvalidOperationException();
        }

        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData)
        {
            IReadOnlyList<byte> res = First.Decrypt(encryptedData);
            res = Second.Decrypt(res);
            
            return res;
        }
    }
}