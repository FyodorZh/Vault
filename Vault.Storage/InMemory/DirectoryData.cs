using Vault.Content;

namespace Vault.Storage.InMemory
{
    public class DirectoryData : NodeData, IDirectoryData
    {
        public IBox<IDirectoryContent> DirContent { get; set; }
        
        public DirectoryData(NodeId id, NodeId parentId, 
            Box<StringContent> encryptedName,
            IBox<DirectoryContent> dirContent) 
            : base(id, parentId, encryptedName)
        {
            DirContent = dirContent;
        }
    }
}