using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.FileSystem
{
    public interface IGenericFileSystem<in TData, TEntity>
        where TData : class
        where TEntity : IGenericEntity<TData>
    {
        Task<TEntity?> GetEntity(EntityName name);
        Task<IEnumerable<TEntity>> GetChildren(EntityName name);
        
        Task<TEntity?> Add(EntityName name, TData data);
        Task<bool> Delete(EntityName name);
    }
    
    public interface IBinaryFileSystem : IGenericFileSystem<byte[], IBinaryEntity>
    {
    }
    
    public interface ITextFileSystem : IGenericFileSystem<string, ITextEntity>
    {
    }
}