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
        T? Deserialize(IEncryptionChain? decryptorsChain = null);
    }
    
    public class Box<T> : IBox<T>
        where T : class, IDataStruct
    {
        public IReadOnlyList<byte> Data { get; }

        public Box(IReadOnlyList<byte> data)
        {
            Data = data;
        }

        public Box(T data, IEncryptionChain? encryptorsChain = null)
        {
            var bytes = Serializer.Serialize(data);
            Data = encryptorsChain?.Encrypt(bytes) ?? bytes;
        }

        public T? Deserialize(IEncryptionChain? decryptorsChain = null)
        {
            var data = decryptorsChain?.Decrypt(Data) ?? Data;
            IDataStruct? dataStruct = Serializer.Deserialize(data);
            return dataStruct as T;
        }
    }
}