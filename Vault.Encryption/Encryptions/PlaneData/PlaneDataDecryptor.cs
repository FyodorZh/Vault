using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    public class PlaneDataDecryptor : Decryptor
    {
        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData)
        {
            byte[] res = new byte[encryptedData.Count];
            for (int i = encryptedData.Count - 1; i >= 0; --i)
            {
                res[i] = encryptedData[i];
            }
            return res;
        }
    }
}