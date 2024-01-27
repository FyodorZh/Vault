using System.Collections.Generic;
using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Encryption
{
    [Guid("32DE45A8-3BA0-4F69-BD49-0A98D989C0EF")]
    public class XorEncryptionSource : EncryptionSource
    {
        private bool _initializedCredentials;
        private byte _xor;
        private string? _dbgCredentials;
        
        public override bool NeedCredentials => !_initializedCredentials;

        public override bool AddCredentials(string credentials)
        {
            _xor = 0;
            foreach (var ch in credentials)
            {
                _xor ^= (byte)ch;
            }
            _initializedCredentials = true;
            _dbgCredentials = credentials;
            return true;
        }

        public override void ClearCredentials()
        {
            _initializedCredentials = false;
            _xor = 0;
            _dbgCredentials = null;
        }

        public override EncryptionDesc GetDescription()
        {
            return new EncryptionDesc("Xor", true, _initializedCredentials);
        }

        public override IReadOnlyList<byte>? Encrypt(IReadOnlyList<byte> plainData)
        {
            if (!_initializedCredentials)
            {
                return null;
            }
            
            byte[] res = new byte[plainData.Count + 1];
            for (int i = plainData.Count - 1; i >= 0; --i)
            {
                res[i] = (byte)(plainData[i] ^ _xor);
            }
            res[^1] = _xor;
            return res;
        }
        
        public override IReadOnlyList<byte>? Decrypt(IReadOnlyList<byte> plainData)
        {
            if (plainData[^1] != _xor)
            {
                return null;
            }
            
            byte[] res = new byte[plainData.Count - 1];
            for (int i = res.Length - 1; i >= 0; --i)
            {
                res[i] = (byte)(plainData[i] ^ _xor);
            }
            return res;
        }

        public override void Serialize(ISerializer serializer)
        {
            // DO NOTHING
        }
    }
}