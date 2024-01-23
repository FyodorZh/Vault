using Archivarius;

namespace Vault.FileSystem
{
    public interface IEntityData
    { 
        IEntity Owner { get; }
        
        TModel? ReadModel<TModel>() where TModel : class, IDataStruct;
    }

    public interface IEntityData<TData> : IEntityData
        where TData : notnull
    {
        new IEntity<TData> Owner { get; }

        TData GetData();
    }
}