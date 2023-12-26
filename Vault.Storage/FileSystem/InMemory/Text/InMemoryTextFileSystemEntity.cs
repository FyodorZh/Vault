using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    [Guid("23388A65-377D-456E-A2C6-F865B6E0815B")]
    public class InMemoryTextFileSystemEntity : ITextEntity, IDataStruct
    {
        private EntityName _name;
        private string? _data;

        public bool IsValid => _data != null;

        public EntityName Name => _name;

        public InMemoryTextFileSystemEntity()
        {
            _name = null!;
        }

        public void Setup(EntityName name, string data)
        {
            _name = name;
            _data = data;
        }

        public void Invalidate()
        {
            _data = null;
        }

        public Task<string> Read()
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            return Task.FromResult(_data);
        }

        public Task Write(string data)
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            _data = data;
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