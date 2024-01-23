using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Serialization;

namespace Vault.FileSystem
{
    [Guid("910991F8-60D3-414A-8852-E7C0AE2E10A7")]
    public class InMemoryBinaryFileSystemEntity : InMemoryFileSystemEntity<byte[]>
    {
        protected override byte[] Empty => Array.Empty<byte>();
        
        protected override byte[] SafeCopy(byte[] src)
        {
            byte[] res = new byte[src.Length];
            Buffer.BlockCopy(src, 0, res, 0, src.Length);
            return res;
        }

        protected override void Serialize(ISerializer serializer, ref byte[]? data)
        {
            serializer.Add(ref data);
        }
        
        protected override TModel? GetModel<TModel>() where TModel : class
        {
            var bytes = ((IEntityData<byte[]>)this).GetData();
            return Serializer.Deserialize(bytes) as TModel;
        }

        public override Task WriteModel<TModel>(TModel model)
        {
            var bytes = Serializer.Serialize(model);
            return Write(bytes);
        }
    }
}