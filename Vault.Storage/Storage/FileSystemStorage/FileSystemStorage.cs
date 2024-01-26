using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Content;
using Vault.FileSystem;

namespace Vault.Storage.FileSystem
{
    public class FileSystemStorage : IStorage
    {
        private readonly IFileSystem _fs;
        private readonly INodeIdSource _nodeIdSource;

        private readonly Dictionary<NodeId, NodeData> _nodes = new Dictionary<NodeId, NodeData>(); 
        
        public FileSystemStorage(IFileSystem fileSystem, INodeIdSource nodeIdSource)
        {
            _fs = fileSystem;
            _nodeIdSource = nodeIdSource;
        }

        public async Task<IDirectoryData> GetRoot()
        {
            return (IDirectoryData)(await GetNodeInternal(PathToId(EntityName.Root)) ?? throw new Exception());
        }

        public async Task<INodeData?> GetNode(NodeId id)
        {
            return await GetNodeInternal(id);
        }

        private async Task<NodeData?> GetNodeInternal(NodeId id)
        {
            if (_nodes.TryGetValue(id, out var nodeData))
            {
                return nodeData;
            }

            IEntity? nodeEntity = await _fs.GetEntity(IdToPath(id));
            if (nodeEntity == null)
            {
                return null;
            }

            var nodeDataModel = await nodeEntity.ReadModel<NodeData.NodeDataModel>();
            if (nodeDataModel == null)
            {
                throw new Exception("Can't deserialize node data");
            }
            if (nodeDataModel.Id != id)
            {
                throw new Exception("FATAL ERROR. NodeData Id and FileName Id mismatch");
            }
            
            switch (nodeDataModel)
            {
                case FileData.FileDataModel fileDataModel:
                    nodeData = new FileData(nodeEntity, fileDataModel);
                    break;
                case DirectoryData.DirectoryDataModel dirDataModel:
                    nodeData = new DirectoryData(nodeEntity, dirDataModel);
                    break;
                default:
                    throw new Exception();
            }
            
            _nodes.Add(id, nodeData);
            return nodeData;
        }

        public async Task<IEnumerable<INodeData>> GetChildren(NodeId parentId)
        {
            EntityName path = IdToPath(parentId);
            var parent = await _fs.GetEntity(path);
            if (parent == null)
            {
                return Array.Empty<INodeData>();
            }

            List<INodeData> list = new List<INodeData>();
            foreach (var child in await parent.FS.GetChildren(path))
            {
                var ch = await GetNode(PathToId(child.Name)) ?? throw new Exception();
                list.Add(ch);
            }

            return list;
        }

        public async Task<IDirectoryData> AddDirectory(NodeId parentId, Box<StringContent> encryptedName, Box<DirectoryContent> encryptedContent)
        {
            var parent = await GetNodeInternal(parentId);
            if (parent is not DirectoryData dir)
            {
                throw new Exception();
            }

            var id = _nodeIdSource.GenNew();
            var path = new EntityName(dir.FsEntity.Name, id.ToString());
            id = PathToId(path);

            return await dir.AddDirectory(path, id, encryptedName, encryptedContent);
        }

        public async Task<IFileData> AddFile(NodeId parentId, Box<StringContent> encryptedName, Box<FileContent> encryptedContent)
        {
            var parent = await GetNodeInternal(parentId);
            if (parent is not DirectoryData dir)
            {
                throw new Exception();
            }

            var id = _nodeIdSource.GenNew();
            var path = new EntityName(dir.FsEntity.Name, id.ToString());
            id = PathToId(path);

            return await dir.AddFile(path, id, encryptedName, encryptedContent);
        }

        public async Task<bool> SetNodeName(NodeId id, Box<StringContent> encryptedName)
        {
            var node = await GetNodeInternal(id);
            if (node != null)
            {
                await node.SetName(encryptedName);
                return true;
            }

            return false;
        }

        public async Task<bool> SetDirectoryContent(NodeId id, Box<IDirectoryContent> encryptedContent)
        {
            var node = await GetNodeInternal(id);
            if (node is not DirectoryData dir)
            {
                return false;
            }

            await dir.SetContent(encryptedContent);
            return true;
        }

        public async Task<bool> SetFileContent(NodeId id, Box<IFileContent> encryptedContent)
        {
            var node = await GetNodeInternal(id);
            if (node is not FileData file)
            {
                return false;
            }

            await file.SetContent(encryptedContent);
            return true;
        }

        private static NodeId PathToId(EntityName name)
        {
            return new NodeId(name.FullName);
        }

        private static EntityName IdToPath(NodeId id)
        {
            return new EntityName(id.ToString().Split('/'));
        }
    }
}