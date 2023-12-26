using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    [Guid("910991F8-60D3-414A-8852-E7C0AE2E10A7")]
    public class InMemoryBinaryFileSystemEntity : IBinaryEntity, IDataStruct
    {
        private EntityName _name;
        private byte[]? _data;

        public bool IsValid => _data != null;

        public EntityName Name => _name;

        public InMemoryBinaryFileSystemEntity()
        {
            _name = null!;
        }

        public void Setup(EntityName name, byte[] data)
        {
            _name = name;
            _data = data;
        }

        public void Invalidate()
        {
            _data = null;
        }

        public Task<byte[]> Read()
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            return Task.FromResult(_data.ToArray());
        }

        public Task Write(byte[] data)
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            _data = data.ToArray();
            return Task.CompletedTask;
        }

        public void Serialize(ISerializer serializer)
        {
            if (serializer.IsWriter && _data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            serializer.AddClass(ref _name, () => throw new Exception());
            serializer.Add(ref _data);
        }
    }
}