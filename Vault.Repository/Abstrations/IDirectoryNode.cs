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
        IEnumerable<INode> All { get; }
        INode? FindChild(string name);
    }
    
    public interface IDirectoryChildrenContentAspect : ILockableAspect
    {
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name);
    }
    
    public interface IDirectoryNode : INode
    {
        IDirectoryEncryptionAspect Encryption { get; }

        IDirectoryChildrenNamesAspect ChildrenNames { get; }
        
        IDirectoryChildrenContentAspect ChildrenContent { get; }
    }
}