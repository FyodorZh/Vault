using System.Collections.Generic;
using Vault.Content;

namespace Vault.Repository
{
    public interface IDirectoryNode : INode
    {
        IEnumerable<INode> Children { get; }
        INode? FindChild(string name);
        
        IFileNode AddChildFile(string name, IContent content);
        IDirectoryNode AddChildDirectory(string name);
    }
}