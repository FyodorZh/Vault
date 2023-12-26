using System;
using System.Runtime.InteropServices;
using Archivarius;
using Vault.Content;

namespace Vault.Storage.InMemory
{
    [Guid("093A9E7D-8636-4DD6-A5BF-BBA807397719")]
    public class FileData : NodeData, IFileData
    {
        private IBox<IFileContent> _fileContent;

        public IBox<IFileContent> FileContent
        {
            get => _fileContent;
            set => _fileContent = value;
        }

        private FileData()
        {
            _fileContent = new Box<IFileContent>();
        }
        
        public FileData(NodeId id, NodeId parentId, 
            IBox<StringContent> encryptedName,
            IBox<FileContent> fileContent) 
            : base(id, parentId, encryptedName)
        {
            _fileContent = fileContent;
        }

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.AddClass(ref _fileContent, () => throw new Exception());
        }
    }
}