using System.Threading.Tasks;
using Archivarius;
using Vault.Serialization;

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
    
    public interface IBinaryEntity : IEntity<byte[]>
    {
        async Task<TModel?> IEntity<byte[]>.ReadModel<TModel>() where TModel : class
        {
            var bytes = await Read();
            return Serializer.Deserialize(bytes) as TModel;
        }

        Task IEntity<byte[]>.WriteModel<TModel>(TModel model)
        {
            var bytes = Serializer.Serialize(model);
            return Write(bytes);
        }
    }
    
    public interface ITextEntity : IEntity<string>
    {
        async Task<TModel?> IEntity<string>.ReadModel<TModel>() where TModel : class
        {
            var json = await Read();
            return SerializerJson.Deserialize(json) as TModel;
        }

        Task IEntity<string>.WriteModel<TModel>(TModel model)
        {
            var json = SerializerJson.Serialize(model);
            return Write(json);
        }
    }
}