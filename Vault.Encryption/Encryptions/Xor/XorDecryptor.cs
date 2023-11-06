using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    public class XorDecryptor : Decryptor
    {
        private bool _initializedSeed;
        private byte _xor;
        
        public override bool RequireCredentials => true;
        
        public override void SetCredentials(string seed)
        {
            _initializedSeed = true;
            _xor = 0;
            foreach (var ch in seed)
            {
                _xor ^= (byte)ch;
            }
        }

        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> plainData)
        {
            if (!_initializedSeed)
            {
                throw new InvalidOperationException();
            }
            
            byte[] res = new byte[plainData.Count];
            for (int i = plainData.Count - 1; i >= 0; --i)
            {
                res[i] = (byte)(plainData[i] ^ _xor);
            }
            return res;
        }
    }
}