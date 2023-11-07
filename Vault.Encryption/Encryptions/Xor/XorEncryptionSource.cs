using System;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    [Guid("32DE45A8-3BA0-4F69-BD49-0A98D989C0EF")]
    public class XorEncryptionSource : EncryptionSource
    {
        private string? _credentials;
        
        public XorEncryptionSource()
        {}

        public XorEncryptionSource(string credentials)
        {
            _credentials = credentials;
        }

        private string InitCredentials()
        {
            _credentials ??= CredentialsProvider?.GetCredentials();
            if (_credentials == null)
            {
                throw new Exception();
            }

            return _credentials;
        }
        
        public override Encryptor ConstructEncryptor()
        {
            return new XorEncryptor(InitCredentials());
        }
        
        public override Decryptor ConstructDecryptor()
        {
            return new XorDecryptor(InitCredentials());
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            // DO NOTHING
        }
    }
}