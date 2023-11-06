using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    [Guid("32DE45A8-3BA0-4F69-BD49-0A98D989C0EF")]
    public class XorEncryptionSource : EncryptionSource
    {
        public override Encryptor ConstructEncryptor()
        {
            return new XorEncryptor();
        }
        
        public override Decryptor ConstructDecryptor()
        {
            return new XorDecryptor();
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            // DO NOTHING
        }
    }
}