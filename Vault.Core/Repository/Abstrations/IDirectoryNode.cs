using Vault.Core;

namespace Vault.Repository
{
    public interface IDirectoryNode : INode
    {        
        IReadOnlyList<INode> Children { get; }
        INode? RequestChild(string name);
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name);
    }
}