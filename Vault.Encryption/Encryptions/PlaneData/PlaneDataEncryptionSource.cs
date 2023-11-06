using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    [Guid("9D259A66-5F29-4F99-B141-DF71DA50F2BC")]
    public class PlaneDataEncryptionSource : EncryptionSource
    {
        public override Encryptor ConstructEncryptor()
        {
            return new PlaneDataEncryptor();
        }
        
        public override Decryptor ConstructDecryptor()
        {
            return new PlaneDataDecryptor();
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            // DO NOTHING
        }
    }
}