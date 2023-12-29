using Vault.Content;

namespace Vault.Storage.FileSystem
{
    public class DirectoryData : NodeData, IDirectoryData
    {
        private IBox<IDirectoryContent> _dirContent;

        public IBox<IDirectoryContent> DirContent
        {
            get => _dirContent;
            set => _dirContent = value;
        }

        private DirectoryData()
        {
            _dirContent = new Box<IDirectoryContent>();
        }
        
        public DirectoryData(NodeId id, NodeId parentId, 
            Box<StringContent> encryptedName,
            IBox<DirectoryContent> dirContent) 
            : base(id, parentId, encryptedName)
        {
            _dirContent = dirContent;
        }
    }
}