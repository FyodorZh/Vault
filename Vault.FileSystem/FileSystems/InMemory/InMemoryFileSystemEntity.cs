using System;
using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    public abstract class InMemoryFileSystemEntity<TData> : IEntityCtl<TData>, IDataStruct
        where TData : class
    {
        private EntityName _name;
        private TData? _data;

        public bool IsValid => _data != null;

        public EntityName Name => _name;

        protected InMemoryFileSystemEntity()
        {
            _name = null!;
        }

        void IEntityCtl<TData>.Setup(EntityName name, TData data)
        {
            _name = name;
            _data = data;
        }

        void IEntityCtl<TData>.Invalidate()
        {
            _data = null;
        }

        public Task<TData> Read()
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            return Task.FromResult(SafeCopy(_data));
        }

        public Task Write(TData data)
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            _data = SafeCopy(data);
            return Task.CompletedTask;
        }
        
        public void Serialize(ISerializer serializer)
        {
            if (serializer.IsWriter && _data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            serializer.AddClass(ref _name, () => throw new Exception());
            Serialize(serializer, ref _data);
        }
        
        public abstract Task<TModel?> ReadModel<TModel>() where TModel : class, IDataStruct;
        
        public abstract Task WriteModel<TModel>(TModel model) where TModel : class, IDataStruct;

        protected abstract TData SafeCopy(TData src);

        protected abstract void Serialize(ISerializer serializer, ref TData? data);
    }
}