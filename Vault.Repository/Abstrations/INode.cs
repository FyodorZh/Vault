using Vault.Content;

namespace Vault.Repository
{
    public interface INode
    {
        bool IsValid { get; }
        NodeId Id { get; }
        
        ILockableAspect<string> Name { get; }
        
        ILockableAspect<IContent> Content { get; }

        void LockAll();

        IDirectoryNode? Parent { get; }
    }
}