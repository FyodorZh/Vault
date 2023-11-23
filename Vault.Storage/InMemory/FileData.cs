using Vault.Content;

namespace Vault.Storage.InMemory
{
    public class FileData : NodeData, IFileData
    {
        public IBox<IFileContent> FileContent { get; set; }
        
        public FileData(NodeId id, NodeId parentId, 
            IBox<StringContent> encryptedName,
            IBox<FileContent> fileContent) 
            : base(id, parentId, encryptedName)
        {
            FileContent = fileContent;
        }
    }
}