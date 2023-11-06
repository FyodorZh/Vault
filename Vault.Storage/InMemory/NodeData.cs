using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage.InMemory
{
    public class NodeData : INodeData
    {
        public bool IsValid { get; set; }
        public Guid Id { get; }
        public Guid? ParentId { get; }
        public Box<StringContent> EncryptedName { get; set; }

        protected NodeData(Guid id, Guid? parentId, Box<StringContent> encryptedName)
        {
            IsValid = true;
            Id = id;
            ParentId = parentId;
            EncryptedName = encryptedName;
        }
    }

    public class DirectoryData : NodeData, IDirectoryData
    {
        public bool EncryptChildrenNames { get; }
        
        public Box<EncryptionSource> ContentEncryption { get; }
        
        public DirectoryData(Guid id, Guid? parentId, Box<StringContent> encryptedName,
            Box<EncryptionSource> contentEncryption, bool encryptChildrenNames) 
            : base(id, parentId, encryptedName)
        {
            EncryptChildrenNames = encryptChildrenNames;
            ContentEncryption = contentEncryption;
        }
    }

    public class FileData : NodeData, IFileData
    {
        public Box<IContent> EncryptedContent { get; }
        
        public FileData(Guid id, Guid? parentId, Box<StringContent> encryptedName,
            Box<IContent> encryptedContent) 
            : base(id, parentId, encryptedName)
        {
            EncryptedContent = encryptedContent;
        }
    }
}