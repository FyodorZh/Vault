using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Content;

namespace Vault.Storage.InMemory
{
    [Guid("341B9F6F-B76B-4D75-8991-FBF7BCADEDA5")]
    public class InMemoryStorage : IStorage, IVersionedDataStruct
    {
        private readonly Dictionary<NodeId, NodeData> _nodes = new Dictionary<NodeId, NodeData>();
        private DirectoryData _root;

        private InMemoryStorage()
        {
            _root = null!;
        }

        public InMemoryStorage(
            Box<StringContent> encryptedRootName,
            Box<DirectoryContent> encryptedRootContent)
        {
            _root = new DirectoryData(NodeId.NewId(), NodeId.Invalid, encryptedRootName, encryptedRootContent);
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

        INodeData IStorage.Root => _root;

        INodeData? IStorage.GetNode(NodeId id)
        {
            _nodes.TryGetValue(id, out var node);
            return node;
        }

        IEnumerable<INodeData> IStorage.GetChildren(NodeId id)
        {
            foreach (var node in _nodes.Values)
            {
                if (node.ParentId == id)
                {
                    yield return node;
                }
            }
        }
        
        IDirectoryData IStorage.AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(parentId, out var parent) || !(parent is DirectoryData))
            {
                throw new InvalidOperationException();
            }
            
            var node = new DirectoryData(NodeId.NewId(), parentId, encryptedName, encryptedContent);
            _nodes.Add(node.Id, node);
            return node;
        }

        IFileData IStorage.AddFile(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<FileContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(parentId, out var parent) || !(parent is DirectoryData))
            {
                throw new InvalidOperationException();
            }
            
            var node = new FileData(NodeId.NewId(), parentId, encryptedName, encryptedContent);
            _nodes.Add(node.Id, node);
            return node;
        }

        public bool SetNodeName(NodeId id, Box<StringContent> encryptedName)
        {
            if (!_nodes.TryGetValue(id, out var node))
            {
                return false;
            }

            node.Name = encryptedName;
            return true;
        }

        bool IStorage.SetDirectoryContent(NodeId id, Box<IDirectoryContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(id, out var node) || node is not DirectoryData dirData)
            {
                return false;
            }

            dirData.DirContent = encryptedContent;
            return true;
        }

        bool IStorage.SetFileContent(NodeId id, Box<IFileContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(id, out var node) || node is not FileData fileData)
            {
                return false;
            }

            fileData.FileContent = encryptedContent;
            return true;
        }

        public void Serialize(IOrderedSerializer serializer)
        {
            if (serializer.IsWriter)
            {
                serializer.Writer.WriteInt(_nodes.Count);
                foreach (var kv in _nodes)
                {
                    var key = kv.Key;
                    var value = kv.Value;
                    serializer.AddVersionedStruct(ref key);
                    serializer.AddClass(ref value);
                }

                var rootId = _root.Id;
                serializer.AddVersionedStruct(ref rootId);
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
                    serializer.AddVersionedStruct(ref key);
                    serializer.AddClass(ref value, () => throw new Exception());
                    _nodes.Add(key, value);
                }

                NodeId rootId = default;
                serializer.AddVersionedStruct(ref rootId);
                _root = (DirectoryData)_nodes[rootId];
            }
        }

        public byte Version => 0;
    }
}