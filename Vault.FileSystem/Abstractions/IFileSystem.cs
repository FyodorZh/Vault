using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.FileSystem
{
    public interface IFileSystem
    {
        Task<IEntity?> GetEntity(EntityName name);
        Task<IEnumerable<IEntity>> GetChildren(EntityName name);
        
        Task<IEntity?> Add(EntityName name);
        Task<bool> Delete(EntityName name);
    }
    
    public interface IFileSystem<TData>
        where TData : class
    {
        Task<IEntity<TData>?> GetEntity(EntityName name);
        Task<IEnumerable<IEntity<TData>>> GetChildren(EntityName name);
        
        Task<IEntity<TData>?> Add(EntityName name, TData? data);
        Task<bool> Delete(EntityName name);
    }
}