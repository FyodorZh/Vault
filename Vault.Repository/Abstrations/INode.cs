using Vault.Content;

namespace Vault.Repository
{
    public interface INode
    {
        bool IsValid { get; }
        NodeId Id { get; }
        
        ILockedState<string> Name { get; }
        
        ILockedState<IContent> Content { get; }

        void LockAll();

        IDirectoryNode? Parent { get; }
    }
}