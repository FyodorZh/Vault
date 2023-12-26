using System;
using System.Runtime.InteropServices;
using Archivarius;
using Vault.Content;

namespace Vault.Storage.InMemory
{
    [Guid("AECF55C1-8CC2-4E3A-8D83-0F3F241272E0")]
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

        public override void Serialize(ISerializer serializer)
        {
            base.Serialize(serializer);
            serializer.AddClass(ref _dirContent, () => throw new Exception());
        }
    }
}