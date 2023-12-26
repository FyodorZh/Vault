using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    public abstract class InMemoryFileSystem<TData, TEntity, ITEntity> : IGenericFileSystem<TData, ITEntity>, IVersionedDataStruct
        where TData : class
        where TEntity : class, ITEntity, IGenericEntity<TData>, IDataStruct, new()
        where ITEntity : class, IGenericEntity<TData>
    {
        private Dictionary<EntityName, TEntity> _entities = new Dictionary<EntityName, TEntity>();
        
        public Task<ITEntity?> GetEntity(EntityName name)
        {
            if (_entities.TryGetValue(name, out var entity))
            {
                return Task.FromResult<ITEntity?>(entity);
            }

            return Task.FromResult<ITEntity?>(null);
        }

        public Task<IEnumerable<ITEntity>> GetChildren(EntityName name)
        {
            List<ITEntity> list = new List<ITEntity>();

            foreach (var kv in _entities)
            {
                if (name.IsSubEntity(kv.Key))
                {
                    list.Add(kv.Value);
                }
            }

            return Task.FromResult((IEnumerable<ITEntity>)list);
        }

        public Task<ITEntity?> Add(EntityName name, TData data)
        {
            if (_entities.ContainsKey(name))
            {
                return Task.FromResult<ITEntity?>(null);
            }

            TEntity entity = new TEntity();
            entity.Setup(name, data);
            _entities.Add(name, entity);
            
            return Task.FromResult<ITEntity?>(entity);
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