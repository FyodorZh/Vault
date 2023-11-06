using System;
using System.Collections.Generic;

namespace Vault.Encryption
{
    public class PlaneDataEncryptor : Encryptor
    {
        public override bool RequireCredentials => false;
        
        public override void SetCredentials(string seed)
        {
            throw new InvalidOperationException();
        }

        public override IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData)
        {
            byte[] res = new byte[plainData.Count];
            for (int i = plainData.Count - 1; i >= 0; --i)
            {
                res[i] = plainData[i];
            }
            return res;
        }
    }
}