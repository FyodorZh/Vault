using Archivarius;

namespace Vault.Storage
{
    public interface INodeIdSource : IVersionedDataStruct
    {
        NodeId GenNew();
    }
}