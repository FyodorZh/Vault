using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage
{
    public interface IStorage
    {
        INodeData Root { get; }
        
        INodeData? GetNode(NodeId id);
        
        IEnumerable<INodeData> GetChildren(NodeId parentId);
        
        IDirectoryData AddDirectory(NodeId parentId, Box<StringContent> encryptedName, 
            Box<EncryptionSource> contentEncryption, Box<EncryptionSource>? nameEncryption = null);
        
        IFileData AddFile(NodeId parentId, Box<StringContent> encryptedName, 
            Box<IContent> encryptedContent);

        bool SetNodeName(NodeId id, Box<StringContent> encryptedName);
        bool SetDirectoryContentEncryption(NodeId id, Box<EncryptionSource>? contentEncryption = null);
        bool SetDirectoryChildrenNameEncryption(NodeId id, Box<EncryptionSource>? childrenNameEncryption = null);
        bool SetFileContent(NodeId id, Box<IContent> content);
    }
}