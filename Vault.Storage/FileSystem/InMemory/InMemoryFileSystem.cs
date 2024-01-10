using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    public abstract class InMemoryFileSystem<TData, TEntity> : IFileSystem<TData>, IVersionedDataStruct
        where TData : class
        where TEntity : class, IEntity<TData>, IDataStruct, new()
    {
        private Dictionary<EntityName, TEntity> _entities = new Dictionary<EntityName, TEntity>();
        
        public Task<IEntity<TData>?> GetEntity(EntityName name)
        {
            if (_entities.TryGetValue(name, out var entity))
            {
                return Task.FromResult<IEntity<TData>?>(entity);
            }

            return Task.FromResult<IEntity<TData>?>(null);
        }

        public Task<IEnumerable<IEntity<TData>>> GetChildren(EntityName name)
        {
            List<IEntity<TData>> list = new List<IEntity<TData>>();

            foreach (var kv in _entities)
            {
                if (name.IsSubEntity(kv.Key))
                {
                    list.Add(kv.Value);
                }
            }

            return Task.FromResult((IEnumerable<IEntity<TData>>)list);
        }

        public Task<IEntity<TData>?> Add(EntityName name, TData data)
        {
            if (_entities.ContainsKey(name))
            {
                return Task.FromResult<IEntity<TData>?>(null);
            }

            TEntity entity = new TEntity();
            entity.Setup(name, data);
            _entities.Add(name, entity);
            
            return Task.FromResult<IEntity<TData>?>(entity);
        }

        public Task<bool> Delete(EntityName name)
        {
            if (_entities.TryGetValue(name, out var entity))
            {
                entity.Invalidate();
            }
            return Task.FromResult(_entities.Remove(name));
        }

        public virtual void Serialize(ISerializer serializer)
        {
            serializer.AddDictionary(ref _entities!, 
                //() => throw new Exception(),
                (ISerializer s, ref EntityName key) =>
                {
                    s.AddClass(ref key, () => throw new Exception());
                },
                (ISerializer s, ref TEntity value) =>
                {
                    s.AddClass(ref value, () => throw new Exception());
                });
        }

        public virtual byte Version => 0;
    }
}