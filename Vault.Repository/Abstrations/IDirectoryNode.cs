using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository
{
    public interface IDirectoryEncryptionAspect : ILockableAspect<IDirectoryContent>
    {
        IEncryptionSource SelfChildrenNamesEncryption();
        IEncryptionSource SelfChildrenContentEncryption();
    }
    
    public interface IDirectoryChildrenNamesAspect : ILockableAspect
    {
        IEncryptionChain ChildrenNameEncryptionChain { get; }
        Task<IEnumerable<INode>> All { get; }
        Task<INode?> FindChild(string name);
    }
    
    public interface IDirectoryChildrenContentAspect : ILockableAspect
    {
        IEncryptionChain ContentEncryptionChain { get; }
        Task<IFileNode> AddChildFile(string name, string content);
        Task<IDirectoryNode> AddChildDirectory(string name);
    }
    
    public interface IDirectoryNode : INode
    {
        IDirectoryEncryptionAspect Encryption { get; }

        IDirectoryChildrenNamesAspect ChildrenNames { get; }
        
        IDirectoryChildrenContentAspect ChildrenContent { get; }

        Task<bool> SetEncryption(EncryptionSource nameEncryption, EncryptionSource contentEncryption);
    }
}