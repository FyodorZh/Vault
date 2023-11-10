using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage
{
    public interface INodeData
    {
        bool IsValid { get; }
        NodeId Id { get; }
        NodeId ParentId { get; }
        Box<StringContent> EncryptedName { get; }
    }

    public interface IDirectoryData : INodeData
    {
        Box<EncryptionSource> ContentEncryption { get; }
        Box<EncryptionSource>? ChildrenNameEncryption { get; }
    }

    public interface IFileData : INodeData
    {
        Box<IContent> EncryptedContent { get; }
    }
}