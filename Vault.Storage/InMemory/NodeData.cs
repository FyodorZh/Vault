using Vault.Content;

namespace Vault.Storage.InMemory
{
    public class NodeData : INodeData
    {
        public bool IsValid { get; set; }
        public NodeId Id { get; }
        public NodeId ParentId { get; }
        public IBox<StringContent> EncryptedName { get; set; }
        public IBox<IContent> EncryptedContent { get; set; }

        protected NodeData(NodeId id, NodeId parentId, 
            IBox<StringContent> encryptedName,
            IBox<IContent> encryptedContent)
        {
            IsValid = true;
            Id = id;
            ParentId = parentId;
            EncryptedName = encryptedName;
            EncryptedContent = encryptedContent;
        }
    }

    public class DirectoryData : NodeData, IDirectoryData
    {
        public DirectoryData(NodeId id, NodeId parentId, 
            Box<StringContent> encryptedName,
            IBox<DirectoryContent> encryptedContent) 
            : base(id, parentId, encryptedName, encryptedContent)
        {
        }
    }

    public class FileData : NodeData, IFileData
    {
        public FileData(NodeId id, NodeId parentId, 
            IBox<StringContent> encryptedName,
            IBox<IContent> encryptedContent) 
            : base(id, parentId, encryptedName, encryptedContent)
        {
            EncryptedContent = encryptedContent;
        }
    }
}