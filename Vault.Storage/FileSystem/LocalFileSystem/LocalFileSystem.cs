using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Vault.FileSystem
{
    public class LocalFileSystem : IBinaryFileSystem
    {
        private readonly DirectoryInfo _root;

        public LocalFileSystem(DirectoryInfo root)
        {
            _root = root;
        }

        public Task<IBinaryEntity?> GetEntity(EntityName name)
        {
            var fi = new FileInfo(_root.FullName + "/" + name.FullName);
            if (fi.Exists)
            {
                return Task.FromResult<IBinaryEntity?>(new Entity(name, fi));
            }

            return Task.FromResult<IBinaryEntity?>(null);
        }

        public Task<IEnumerable<IBinaryEntity>> GetChildren(EntityName name)
        {
            throw new System.NotImplementedException();
        }

        public Task<IBinaryEntity?> Add(EntityName name, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public  Task<bool> Delete(EntityName name)
        {
            throw new System.NotImplementedException();
        }

        private class Entity : IBinaryEntity
        {
            private readonly FileInfo _info;

            public void Setup(EntityName name, byte[] data)
            {
                throw new System.NotImplementedException();
            }

            public void Invalidate()
            {
                throw new System.NotImplementedException();
            }

            public bool IsValid { get; }
            public EntityName Name { get; }

            public Entity(EntityName name, FileInfo info)
            {
                Name = name;
                _info = info;
            }
            
            public async Task<byte[]> Read()
            {
                return await File.ReadAllBytesAsync(_info.FullName);
            }

            public async Task Write(byte[] data)
            {
                await File.WriteAllBytesAsync(_info.FullName, data);
            }
        }
    }
}