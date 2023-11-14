using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository
{
    public interface IDirectoryEncryptionAspect : ILockableAspect<IContent>
    {
        IEncryptionSource? SelfChildrenNamesEncryption();
        IEncryptionSource? SelfChildrenContentEncryption();
        
        IEnumerable<IEncryptionSource> ContentEncryptionChain { get; }
        IEnumerable<IEncryptionSource> ChildrenNameEncryptionChain { get; }
    }
    
    public interface IDirectoryChildrenNamesAspect : ILockableAspect
    {
        IEnumerable<(string, INode)> All { get; }
    }
    
    public interface IDirectoryChildrenAspect : ILockableAspect
    {
        IEnumerable<(string, INode)> All { get; }
    }
    
    public interface IDirectoryNode : INode
    {
        IDirectoryEncryptionAspect Encryption { get; }

        IDirectoryChildrenNamesAspect ChildrenNames { get; }
        
        IDirectoryChildrenAspect Children2 { get; }
        
        
        IEnumerable<INode> Children { get; }
        INode? FindChild(string name);
        
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name);
    }
}