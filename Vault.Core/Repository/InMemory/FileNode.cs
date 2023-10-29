using Vault.Core;

namespace Vault.Repository.InMemory
{
    public class FileNode : Node, IFileNode
    {
        public IContent Content { get; }
        
        public FileNode(DirectoryNode parent, string name, IContent content) 
            : base(parent, name)
        {
            Content = content;
        }
    }
}