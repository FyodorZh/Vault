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

        async Task CollectAllChildrenOf(EntityName parent, List<EntityName> collector)
        {
            var children = await GetChildren(parent);
            foreach (var child in children)
            {
                collector.Add(child.Name);
                await CollectAllChildrenOf(child.Name, collector);
            }
        }
    }
    
    public interface IFileSystem<TData> : IFileSystem
        where TData : class
    {
        new Task<IEntity<TData>?> GetEntity(EntityName name);
        new Task<IEnumerable<IEntity<TData>>> GetChildren(EntityName name);
        
        Task<IEntity<TData>?> Add(EntityName name, TData? data);
    }
}