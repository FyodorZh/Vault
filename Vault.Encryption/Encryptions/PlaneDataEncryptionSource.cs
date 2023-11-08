using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    [Guid("9D259A66-5F29-4F99-B141-DF71DA50F2BC")]
    public class PlaneDataEncryptionSource : EncryptionSource
    {
        private IReadOnlyList<byte> Clone(IReadOnlyList<byte> encryptedData)
        {
            byte[] res = new byte[encryptedData.Count];
            for (int i = encryptedData.Count - 1; i >= 0; --i)
            {
                res[i] = encryptedData[i];
            }
            return res;
        }
        
        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData)
        {
            return Clone(encryptedData);
        }
        
        public override IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData)
        {
            return Clone(plainData);
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            // DO NOTHING
        }
    }
}