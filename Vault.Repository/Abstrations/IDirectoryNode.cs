using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository
{
    public interface IDirectoryEncryptionAspect : ILockableAspect<IContent>
    {
        IEncryptionSource? SelfChildrenNamesEncryption();
        IEncryptionSource? SelfChildrenContentEncryption();
    }
    
    public interface IDirectoryChildrenNamesAspect : ILockableAspect
    {
        IEncryptionChain ChildrenNameEncryptionChain { get; }
        IEnumerable<INode> All { get; }
        INode? FindChild(string name);
    }
    
    public interface IDirectoryChildrenContentAspect : ILockableAspect
    {
        IEncryptionChain ContentEncryptionChain { get; }
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name);
    }
    
    public interface IDirectoryNode : INode
    {
        IDirectoryEncryptionAspect Encryption { get; }

        IDirectoryChildrenNamesAspect ChildrenNames { get; }
        
        IDirectoryChildrenContentAspect ChildrenContent { get; }

        bool SetEncryption(EncryptionSource? nameEncryption, EncryptionSource? contentEncryption);
    }
}