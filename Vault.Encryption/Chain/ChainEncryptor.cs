using System;
using System.Collections.Generic;

namespace Vault.Encryption
{
    public class ChainEncryptor : Encryptor
    {
        public Encryptor First { get; }
        public Encryptor Second { get; }
        
        public override bool RequireCredentials => false;

        public ChainEncryptor(Encryptor first, Encryptor second)
        {
            First = first;
            Second = second;
        }
        
        public override void SetCredentials(string seed)
        {
            throw new InvalidOperationException();
        }

        public override IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData)
        {
            IReadOnlyList<byte> res = First.Encrypt(plainData);
            res = Second.Encrypt(res);
            return res;
        }
    }
}