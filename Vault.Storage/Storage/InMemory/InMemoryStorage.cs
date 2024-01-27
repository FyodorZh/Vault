using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Content;

namespace Vault.Storage.InMemory
{
    [Guid("341B9F6F-B76B-4D75-8991-FBF7BCADEDA5")]
    public class InMemoryStorage : IStorage, IVersionedDataStruct
    {
        private INodeIdSource _nodeIdSource;
        
        private readonly Dictionary<NodeId, NodeData> _nodes = new Dictionary<NodeId, NodeData>();
        private DirectoryData _root;

        private InMemoryStorage()
        {
            _nodeIdSource = null!;
            _root = null!;
        }

        public InMemoryStorage(
            INodeIdSource nodeIdSource,
            Box<StringContent> encryptedRootName,
            Box<DirectoryContent> encryptedRootContent)
        {
            _nodeIdSource = nodeIdSource;
            _root = new DirectoryData(_nodeIdSource.GenNew(), NodeId.Invalid, encryptedRootName, encryptedRootContent);
            _nodes.Add(_root.Id, _root);
        }

        public void Add(NodeData data)
        {
            if (!data.ParentId.IsValid || !_nodes.ContainsKey(data.ParentId))
            {
                throw new Exception();
            }
            
            _nodes.Add(data.Id, data);
        }

        Task<IDirectoryData> IStorage.GetRoot() => Task.FromResult<IDirectoryData>(_root);

        Task<INodeData?> IStorage.GetNode(NodeId id)
        {
            _nodes.TryGetValue(id, out var node);
            return Task.FromResult<INodeData?>(node);
        }

        Task<IEnumerable<INodeData>> IStorage.GetChildren(NodeId id)
        {
            List<INodeData> list = new List<INodeData>();
            foreach (var node in _nodes.Values)
            {
                if (node.ParentId == id)
                {
                    list.Add(node);
                }
            }
            return Task.FromResult<IEnumerable<INodeData>>(list);
        }
        
        Task<IDirectoryData> IStorage.AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(parentId, out var parent) || !(parent is DirectoryData))
            {
                throw new InvalidOperationException();
            }
            
            var node = new DirectoryData(_nodeIdSource.GenNew(), parentId, encryptedName, encryptedContent);
            _nodes.Add(node.Id, node);
            return Task.FromResult<IDirectoryData>(node);
        }

        Task<IFileData> IStorage.AddFile(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<FileContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(parentId, out var parent) || !(parent is DirectoryData))
            {
                throw new InvalidOperationException();
            }
            
            var node = new FileData(_nodeIdSource.GenNew(), parentId, encryptedName, encryptedContent);
            _nodes.Add(node.Id, node);
            return Task.FromResult<IFileData>(node);
        }

        public Task<bool> SetNodeName(NodeId id, Box<StringContent> encryptedName)
        {
            if (!_nodes.TryGetValue(id, out var node))
            {
                return Task.FromResult(false);
            }

            node.Name = encryptedName;
            return Task.FromResult(true);
        }

        Task<bool> IStorage.SetDirectoryContent(NodeId id, Box<DirectoryContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(id, out var node) || node is not DirectoryData dirData)
            {
                return Task.FromResult(false);
            }

            dirData.DirContent = encryptedContent;
            return Task.FromResult(true);
        }

        Task<bool> IStorage.SetFileContent(NodeId id, Box<FileContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(id, out var node) || node is not FileData fileData)
            {
                return Task.FromResult(false);
            }

            fileData.FileContent = encryptedContent;
            return Task.FromResult(true);
        }

        public void Serialize(ISerializer serializer)
        {
            serializer.AddClass(ref _nodeIdSource, () => throw new Exception());
            if (serializer.IsWriter)
            {
                serializer.Writer.WriteInt(_nodes.Count);
                foreach (var kv in _nodes)
                {
                    var key = kv.Key;
                    var value = kv.Value;
                    serializer.AddStruct(ref key);
                    serializer.AddClass(ref value);
                }

                var rootId = _root.Id;
                serializer.AddStruct(ref rootId);
            }
            else
            {
                int count = 0;
                serializer.Add(ref count);
                _nodes.Clear();
                for (int i = 0; i < count; ++i)
                {
                    NodeId key = new NodeId();
                    NodeData value = null!;
                    serializer.AddStruct(ref key);
                    serializer.AddClass(ref value, () => throw new Exception());
                    _nodes.Add(key, value);
                }

                NodeId rootId = default;
                serializer.AddStruct(ref rootId);
                _root = (DirectoryData)_nodes[rootId];
            }
        }

        public byte Version => 0;
    }
}