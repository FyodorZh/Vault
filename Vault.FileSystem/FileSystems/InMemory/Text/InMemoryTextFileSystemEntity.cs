using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Serialization;

namespace Vault.FileSystem
{
    [Guid("23388A65-377D-456E-A2C6-F865B6E0815B")]
    public class InMemoryTextFileSystemEntity : InMemoryFileSystemEntity<string>
    {
        protected override string Empty => "";

        protected override string SafeCopy(string src)
        {
            return src;
        }

        protected override void Serialize(ISerializer serializer, ref string? data)
        {
            serializer.Add(ref data);
        }

        protected override TModel? GetModel<TModel>() where TModel : class
        {
            var json = ((IEntityData<string>)this).GetData();
            return SerializerJson.Deserialize(json) as TModel;
        }

        public override Task WriteModel<TModel>(TModel model)
        {
            var json = SerializerJson.Serialize(model);
            return Write(json);
        }
    }
}