using Vault.Content;

namespace Vault.Storage
{
    public interface INodeData
    {
        bool IsValid { get; }
        NodeId Id { get; }
        NodeId ParentId { get; }
        IBox<StringContent> Name { get; }
    }

    public interface IDirectoryData : INodeData
    {
        IBox<IDirectoryContent> DirContent { get; }
    }

    public interface IFileData : INodeData
    {
        IBox<IFileContent> FileContent { get; }
    }
}