using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.FileSystem
{
    public interface IFileSystem<TData>
        where TData : class
    {
        Task<IEntity<TData>?> GetEntity(EntityName name);
        Task<IEnumerable<IEntity<TData>>> GetChildren(EntityName name);
        
        Task<IEntity<TData>?> Add(EntityName name, TData data);
        Task<bool> Delete(EntityName name);
    }
}