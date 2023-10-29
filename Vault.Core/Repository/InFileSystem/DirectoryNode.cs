using Vault.Core;

namespace Vault.Repository.InFileSystem
{
    public class DirectoryNode : Node, IDirectoryNode
    {
        private readonly DirectoryInfo _root;
        
        public DirectoryNode(DirectoryNode? parent, string name, DirectoryInfo root)
            : base(parent, name)
        {
            if (!root.Exists)
            {
                throw new InvalidOperationException("Invalid directory");
            }
            _root = root;
        }

        #region IDirectoryNode

        IReadOnlyList<INode> IDirectoryNode.Children { get; }
        INode? IDirectoryNode.RequestChild(string name){
            throw new NotImplementedException();
        }
        IFileNode IDirectoryNode.AddChildFile(string name, IContent content){
            throw new NotImplementedException();
        }
        IDirectoryNode IDirectoryNode.AddChildDirectory(string name){
            throw new NotImplementedException();
        }

        #endregion

    }
}