using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage
{
    public interface IStorage
    {
        INodeData Root { get; }
        
        INodeData? GetNode(Guid id);
        
        IEnumerable<INodeData> GetChildren(Guid parentId);
        
        IDirectoryData AddDirectory(Guid parentId, Box<StringContent> encryptedName, 
            Box<EncryptionSource> contentEncryption, bool encryptChildrenNames);
        
        IFileData AddFile(Guid parentId, Box<StringContent> encryptedName, 
            Box<IContent> encryptedContent);
    }
}