using System;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage
{
    public interface INodeData
    {
        bool IsValid { get; }
        Guid Id { get; }
        Guid? ParentId { get; }
        Box<StringContent> EncryptedName { get; }
    }

    public interface IDirectoryData : INodeData
    {
        bool EncryptChildrenNames { get; }
        Box<EncryptionSource> ContentEncryption { get; }
    }

    public interface IFileData : INodeData
    {
        Box<IContent> EncryptedContent { get; }
    }
}