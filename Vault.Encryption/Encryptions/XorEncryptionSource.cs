using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    [Guid("32DE45A8-3BA0-4F69-BD49-0A98D989C0EF")]
    public class XorEncryptionSource : EncryptionSource
    {
        private bool _initializedCredentials;
        private byte _xor;

        private void InitCredentials()
        {
            if (!_initializedCredentials)
            {
                string credentials = CredentialsProvider!.GetCredentials();
                _xor = 0;
                foreach (var ch in credentials)
                {
                    _xor ^= (byte)ch;
                }
                _initializedCredentials = true;
            }
        }
        
        public override IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData)
        {
            InitCredentials();
            
            byte[] res = new byte[plainData.Count];
            for (int i = plainData.Count - 1; i >= 0; --i)
            {
                res[i] = (byte)(plainData[i] ^ _xor);
            }
            return res;
        }
        
        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> plainData)
        {
            InitCredentials();
            
            byte[] res = new byte[plainData.Count];
            for (int i = plainData.Count - 1; i >= 0; --i)
            {
                res[i] = (byte)(plainData[i] ^ _xor);
            }
            return res;
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            // DO NOTHING
        }
    }
}