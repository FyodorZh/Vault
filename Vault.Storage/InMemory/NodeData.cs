using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage.InMemory
{
    public class NodeData : INodeData
    {
        public bool IsValid { get; set; }
        public NodeId Id { get; }
        public NodeId ParentId { get; }
        public Box<StringContent> EncryptedName { get; set; }

        protected NodeData(NodeId id, NodeId parentId, Box<StringContent> encryptedName)
        {
            IsValid = true;
            Id = id;
            ParentId = parentId;
            EncryptedName = encryptedName;
        }
    }

    public class DirectoryData : NodeData, IDirectoryData
    {
        public Box<EncryptionSource>? ContentEncryption { get; set; }
        
        public Box<EncryptionSource>? ChildrenNameEncryption { get; set; }
        
        public DirectoryData(NodeId id, NodeId parentId, Box<StringContent> encryptedName,
            Box<EncryptionSource>? contentEncryption = null, 
            Box<EncryptionSource>? childrenNameEncryption = null) 
            : base(id, parentId, encryptedName)
        {
            ContentEncryption = contentEncryption;
            ChildrenNameEncryption = childrenNameEncryption;
        }
    }

    public class FileData : NodeData, IFileData
    {
        public Box<IContent> EncryptedContent { get; set; }
        
        public FileData(NodeId id, NodeId parentId, Box<StringContent> encryptedName,
            Box<IContent> encryptedContent) 
            : base(id, parentId, encryptedName)
        {
            EncryptedContent = encryptedContent;
        }
    }
}