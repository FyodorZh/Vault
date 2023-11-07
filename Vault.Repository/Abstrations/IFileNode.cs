using Vault.Content;

namespace Vault.Repository
{
    public interface IFileNode : INode
    {        
        bool IsLocked { get; }
        void Unlock();
        
        IContent Content { get; }
    }
}