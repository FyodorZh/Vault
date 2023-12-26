using System.Threading.Tasks;
using Archivarius;
using Vault.Serialization;

namespace Vault.FileSystem
{
    public interface IGenericEntity<T>
        where T: class
    {
        internal void Setup(EntityName name, T data);
        internal void Invalidate();
        
        bool IsValid { get; }
        EntityName Name { get; }
        Task<T> Read();
        Task Write(T data);
    }
    
    public interface IBinaryEntity : IGenericEntity<byte[]>
    {
    }
    
    public interface ITextEntity : IGenericEntity<string>
    {
    }

    public static class IGenericEntity_Ext
    {
        public static async Task<TModel?> ReadModel<TModel>(this IGenericEntity<byte[]> entity)
            where TModel : class, IDataStruct
        {
            var bytes = await entity.Read();
            return Serializer.Deserialize(bytes) as TModel;
        }
        
        public static async Task<TModel?> ReadModel<TModel>(this IGenericEntity<string> entity)
            where TModel : class, IDataStruct
        {
            var json = await entity.Read();
            return SerializerJson.Deserialize(json) as TModel;
        }
        
        public static async Task WriteModel<TModel>(this IGenericEntity<byte[]> entity, TModel model)
            where TModel : class, IDataStruct
        {
            var bytes = Serializer.Serialize(model);
            await entity.Write(bytes);
        }
        
        public static async Task WriteModel<TModel>(this IGenericEntity<string> entity, TModel model)
            where TModel : class, IDataStruct
        {
            var json = SerializerJson.Serialize(model);
            await entity.Write(json);
        }
    }
}