using System.Collections.Generic;
using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Encryption
{
    [Guid("9D259A66-5F29-4F99-B141-DF71DA50F2BC")]
    public class PlaneDataEncryptionSource : EncryptionSource
    {
        public override bool NeedCredentials => false;

        public override bool AddCredentials(string credentials)
        {
            throw new System.InvalidOperationException();
        }

        public override void ClearCredentials()
        {
            // DO NOTHING
        }

        public override EncryptionDesc GetDescription()
        {
            return new EncryptionDesc("PlaneData", false, false);
        }

        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> encryptedData)
        {
            return Clone(encryptedData);
        }
        
        public override IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData)
        {
            return Clone(plainData);
        }
        
        private static IReadOnlyList<byte> Clone(IReadOnlyList<byte> encryptedData)
        {
            byte[] res = new byte[encryptedData.Count];
            for (int i = encryptedData.Count - 1; i >= 0; --i)
            {
                res[i] = encryptedData[i];
            }
            return res;
        }

        public override void Serialize(ISerializer serializer)
        {
            // DO NOTHING
        }
    }
}