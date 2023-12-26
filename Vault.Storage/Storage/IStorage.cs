using System.Collections.Generic;
using Vault.Content;

namespace Vault.Storage
{
    public interface IStorage
    {
        IDirectoryData Root { get; }
        
        INodeData? GetNode(NodeId id);
        
        IEnumerable<INodeData> GetChildren(NodeId parentId);

        IEnumerable<INodeData> GetAllSubChildren(NodeId parentId)
        {
            foreach (var ch in GetChildren(parentId))
            {
                yield return ch;
                foreach (var subCh in GetChildren(ch.Id))
                {
                    yield return subCh;
                }
            }
        }

        IDirectoryData AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent);
        
        IFileData AddFile(NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<FileContent> encryptedContent);

        bool SetNodeName(NodeId id, Box<StringContent> encryptedName);
        bool SetDirectoryContent(NodeId id, Box<IDirectoryContent> encryptedContent);
        bool SetFileContent(NodeId id, Box<IFileContent> encryptedContent);
    }
}