using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository
{
    public interface IDirectoryNode : INode
    {
        EncryptionDesc? ChildrenNamesEncryption { get; }
        EncryptionDesc? ContentEncryption { get; }
        
        IEnumerable<INode> Children { get; }
        INode? FindChild(string name);
        
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name, EncryptionSource encryptionSource);
    }
}