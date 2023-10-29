using Vault.Core;

namespace Vault.Repository.InMemory
{
    public class DirectoryNode : Node, IDirectoryNode
    {
        private readonly List<INode> _children = new List<INode>();
        
        public DirectoryNode(DirectoryNode? parent, string name) 
            : base(parent, name)
        {
        }

        #region IDirectoryNode
        
        IReadOnlyList<INode> IDirectoryNode.Children => _children;
        
        INode? IDirectoryNode.RequestChild(string name)
        {
            return _children.Find(ch => ch.Name == name);
        }

        IFileNode IDirectoryNode.AddChildFile(string name, IContent content)
        {
            var file = new FileNode(this, name, content);
            _children.Add(file);
            return file;
        }
        
        IDirectoryNode IDirectoryNode.AddChildDirectory(string name)
        {
            var dir = new DirectoryNode(this, name);
            _children.Add(dir);
            return dir;
        }
        
        #endregion
    }
}