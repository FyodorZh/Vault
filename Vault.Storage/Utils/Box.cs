using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Encryption;
using Vault.Serialization;

namespace Vault.Storage
{
    public interface IBox<out T> : IVersionedDataStruct
        where T : class, IDataStruct
    {
        IReadOnlyList<byte> Data { get; }
        T? Deserialize(IEncryptionChain? decryptorsChain = null);
    }
    
    [Guid("55100502-311C-4147-B8B8-619B8D790DFA")]
    public class Box<T> : IBox<T>
        where T : class, IDataStruct
    {
        private IReadOnlyList<byte> _data;
        public IReadOnlyList<byte> Data => _data;

        public Box()
        {
            _data = Array.Empty<byte>();
        }

        public Box(IReadOnlyList<byte> data)
        {
            _data = data;
        }

        public Box(T data, IEncryptionChain? encryptorsChain = null)
        {
            var bytes = Serializer.Serialize(data);
            _data = encryptorsChain?.Encrypt(bytes) ?? bytes;
        }

        public T? Deserialize(IEncryptionChain? decryptorsChain = null)
        {
            var data = decryptorsChain?.Decrypt(Data) ?? Data;
            IDataStruct? dataStruct = Serializer.Deserialize(data);
            return dataStruct as T;
        }

        public void Serialize(IOrderedSerializer serializer)
        {
            IReadOnlyList<byte>? data = _data; 
            serializer.Add(ref data);
            _data = data ?? Array.Empty<byte>();
        }

        public byte Version => 0;
    }
}