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
        
        public override bool NeedCredentials => !_initializedCredentials;

        public override bool AddCredentials(string credentials)
        {
            _xor = 0;
            foreach (var ch in credentials)
            {
                _xor ^= (byte)ch;
            }
            _initializedCredentials = true;
            return true;
        }

        public override EncryptionDesc GetDescription()
        {
            return new EncryptionDesc("Xor", true, _initializedCredentials);
        }

        public override IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> plainData)
        {
            byte[] res = new byte[plainData.Count];
            for (int i = plainData.Count - 1; i >= 0; --i)
            {
                res[i] = (byte)(plainData[i] ^ _xor);
            }
            return res;
        }
        
        public override IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> plainData)
        {
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