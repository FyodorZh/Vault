using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    public interface IEntity
    { 
        Validity IsValid { get; }
        EntityName Name { get; }

        Task<IEntityData> ReadAllData();

        Task<TModel?> ReadModel<TModel>() where TModel : class, IDataStruct;
        Task WriteModel<TModel>(TModel model) where TModel : class, IDataStruct;
    }
    
    public interface IEntity<T> : IEntity
        where T: notnull
    {
        new Task<IEntityData<T>> ReadAllData();
        
        Task<T> Read();
        Task Write(T data);
    }

    public interface IEntityCtl<T> : IEntity<T>
        where T: class
    {
        void Setup(EntityName name, T? data);
        void Invalidate();
    }
}