using System;
using System.Collections.Generic;
using System.Linq;
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

        void IDataStruct.Serialize(IOrderedSerializer serializer)
        {
            if (serializer.IsWriter)
            {
                byte[]? bytes = null;
                if (_data is byte[] typedData)
                {
                    bytes = typedData;
                }
                else
                {
                    bytes = _data.ToArray();
                }

                serializer.Add(ref bytes);
            }
            else
            {
                byte[] bytes = null!;
                serializer.Add(ref bytes, Array.Empty<byte>);
                _data = bytes;
            }
        }

        public byte Version => 0;
    }
}