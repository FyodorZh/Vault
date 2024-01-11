using System.Threading.Tasks;
using Archivarius;

namespace Vault.FileSystem
{
    public interface IEntity<T>
        where T: class
    {
        internal void Setup(EntityName name, T data);
        internal void Invalidate();
        
        bool IsValid { get; }
        EntityName Name { get; }
        Task<T> Read();
        Task Write(T data);

        Task<TModel?> ReadModel<TModel>() where TModel : class, IDataStruct;
        Task WriteModel<TModel>(TModel model) where TModel : class, IDataStruct;
    }
}