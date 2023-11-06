using Vault.Content;

namespace Vault.Repository
{
    public interface IFileNode : INode
    {        
        IContent Content { get; }
    }
}