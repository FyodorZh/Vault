using Vault.Content;

namespace Vault.Storage
{
    public interface INodeData
    {
        bool IsValid { get; }
        NodeId Id { get; }
        NodeId ParentId { get; }
        IBox<StringContent> EncryptedName { get; }
        IBox<IContent> EncryptedContent { get; }
    }

    public interface IDirectoryData : INodeData
    {
    }

    public interface IFileData : INodeData
    {
    }
}