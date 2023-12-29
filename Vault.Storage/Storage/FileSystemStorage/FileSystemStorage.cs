using System.Collections.Generic;
using Vault.Content;
using Vault.FileSystem;

namespace Vault.Storage.FileSystem
{
    public class FileSystemStorage : IStorage
    {
        private readonly IBinaryFileSystem _fs;
        
        private IDirectoryData _root = null!;

        public FileSystemStorage(IBinaryFileSystem fileSystem)
        {
            _fs = fileSystem;
        }

        IDirectoryData IStorage.Root => _root;

        INodeData? IStorage.GetNode(NodeId id)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<INodeData> IStorage.GetChildren(NodeId parentId)
        {
            throw new System.NotImplementedException();
        }

        IDirectoryData IStorage.AddDirectory(NodeId parentId, Box<StringContent> encryptedName, Box<DirectoryContent> encryptedContent)
        {
            throw new System.NotImplementedException();
        }

        IFileData IStorage.AddFile(NodeId parentId, Box<StringContent> encryptedName, Box<FileContent> encryptedContent)
        {
            throw new System.NotImplementedException();
        }

        bool IStorage.SetNodeName(NodeId id, Box<StringContent> encryptedName)
        {
            throw new System.NotImplementedException();
        }

        bool IStorage.SetDirectoryContent(NodeId id, Box<IDirectoryContent> encryptedContent)
        {
            throw new System.NotImplementedException();
        }

        bool IStorage.SetFileContent(NodeId id, Box<IFileContent> encryptedContent)
        {
            throw new System.NotImplementedException();
        }
    }
}