using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository
{
    public interface IDirectoryEncryptionState : ILockedState<IContent>
    {
        IEnumerable<IEncryptionSource> ContentEncryptionChain { get; }
        IEnumerable<IEncryptionSource> ChildrenNameEncryptionChain { get; }
    }
    
    public interface IDirectoryNode : INode
    {
        IDirectoryEncryptionState Encryption { get; }
        
        IEnumerable<INode> Children { get; }
        INode? FindChild(string name);
        
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name);
    }
}