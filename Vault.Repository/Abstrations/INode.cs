using System.Threading.Tasks;
using Vault.Content;
using Vault.Storage;

namespace Vault.Repository
{
    public interface INode
    {
        bool IsValid { get; }
        NodeId Id { get; }

        string Name { get; }
        
        ILockableAspect<IContent> Content { get; }

        Task LockAll();

        IDirectoryNode? Parent { get; }
    }
}