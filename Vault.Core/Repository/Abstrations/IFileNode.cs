using Vault.Core;

namespace Vault.Repository
{
    public interface IFileNode : INode
    {        
        IContent Content { get; }
    }
}