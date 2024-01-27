using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Content;

namespace Vault.Storage
{
    public interface IStorage
    {
        Task<IDirectoryData> GetRoot();
        
        Task<INodeData?> GetNode(NodeId id);
        
        Task<IEnumerable<INodeData>> GetChildren(NodeId parentId);

        async Task<IEnumerable<INodeData>> GetAllSubChildren(NodeId parentId)
        {
            List<INodeData> list = new List<INodeData>();
            
            foreach (var ch in await GetChildren(parentId))
            {
                list.Add(ch);
                list.AddRange(await GetAllSubChildren(ch.Id));
            }

            return list;
        }

        Task<IDirectoryData> AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent);
        
        Task<IFileData> AddFile(NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<FileContent> encryptedContent);

        Task<bool> SetNodeName(NodeId id, Box<StringContent> encryptedName);
        Task<bool> SetDirectoryContent(NodeId id, Box<DirectoryContent> encryptedContent);
        Task<bool> SetFileContent(NodeId id, Box<FileContent> encryptedContent);
    }
}