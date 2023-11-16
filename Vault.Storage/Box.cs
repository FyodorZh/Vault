using System.Collections.Generic;
using OrderedSerializer;
using Vault.Encryption;
using Vault.Serialization;

namespace Vault.Storage
{
    public interface IBox<out T>
        where T : class, IDataStruct
    {
        IReadOnlyList<byte> Data { get; }
        T? Deserialize(IEnumerable<IEncryptionSource>? decryptorsChain = null);
    }
    
    public class Box<T> : IBox<T>
        where T : class, IDataStruct
    {
        public IReadOnlyList<byte> Data { get; }

        public Box(IReadOnlyList<byte> data)
        {
            Data = data;
        }

        public Box(T data, IEnumerable<IEncryptionSource>? encryptorsChain = null)
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

        public T? Deserialize(IEnumerable<IEncryptionSource>? decryptorsChain = null)
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