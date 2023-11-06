using System.Collections.Generic;
using OrderedSerializer;
using Vault.Encryption;
using Vault.Serialization;

namespace Vault.Storage
{
    public class Box<T>
        where T : class, IDataStruct
    {
        public IReadOnlyList<byte> Data { get; }

        public Box(IReadOnlyList<byte> data)
        {
            Data = data;
        }

        public Box(T data, IEnumerable<Encryptor>? encryptorsChain = null)
        {
            var bytes = Serializer.Serialize(data);
            if (encryptorsChain != null)
            {
                foreach (var encryptor in encryptorsChain)
                {
                    bytes = encryptor.Encrypt(bytes);
                }
            }

            Data = bytes;
        }

        public T? Deserialize(IEnumerable<Decryptor>? decryptorsChain = null)
        {
            var data = Data;
            if (decryptorsChain != null)
            {
                foreach (var decryptor in decryptorsChain)
                {
                    data = decryptor.Decrypt(data);
                }
            }

            IDataStruct? dataStruct = Serializer.Deserialize(data);
            return dataStruct as T;
        }
    }
}