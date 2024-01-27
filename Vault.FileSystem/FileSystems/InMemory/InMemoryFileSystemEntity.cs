using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    [DebuggerDisplay("Path='{Name.FullName}'; Data={_data}")]
    public abstract class InMemoryFileSystemEntity<TData> : 
        IEntityCtl<TData>, IEntityData<TData>, IDataStruct
        where TData : class
    {
        private EntityName _name;
        private TData? _data;

        private readonly ValidityImpl _isValid = new ValidityImpl();
        public Validity IsValid => _isValid;

        public EntityName Name => _name;
        
        IFileSystem IEntity.FS => FS;
        public IFileSystem<TData> FS { get; private set; } = null!;

        protected InMemoryFileSystemEntity()
        {
            _name = null!;
        }

        void IEntityCtl<TData>.Setup(IFileSystem<TData> fs, EntityName name, TData? data)
        {
            FS = fs;
            _name = name;
            _data = data ?? Empty;
        }

        void IEntityCtl<TData>.Invalidate()
        {
            _data = null;
            _isValid.Invalidate();
        }

        Task<IEntityData<TData>> IEntity<TData>.ReadAllData()
        {
            return Task.FromResult<IEntityData<TData>>(this);
        }
        Task<IEntityData> IEntity.ReadAllData()
        {
            return Task.FromResult<IEntityData>(this);
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

        public Task<TModel?> ReadModel<TModel>() where TModel : class, IDataStruct
        {
            return Task.FromResult(GetModel<TModel>());
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

        public abstract Task WriteModel<TModel>(TModel model) where TModel : class, IDataStruct;

        protected abstract TData Empty { get; }
        
        protected abstract TData SafeCopy(TData src);
        
        protected abstract TModel? GetModel<TModel>() where TModel : class, IDataStruct;

        protected abstract void Serialize(ISerializer serializer, ref TData? data);

        IEntity IEntityData.Owner => this;
        IEntity<TData> IEntityData<TData>.Owner => this;

        TData IEntityData<TData>.GetData()
        {
            if (_data == null)
            {
                throw new InvalidOperationException("Entity is not valid");
            }
            return SafeCopy(_data);
        }

        TModel? IEntityData.ReadModel<TModel>() where TModel : class
        {
            return GetModel<TModel>();
        }

        private IDataStruct? DBG_As_Model
        {
            get
            {
                try
                {
                    return GetModel<IDataStruct>();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}