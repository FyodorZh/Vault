using System;
using System.Collections.Generic;
using Vault.Content;

namespace Vault.Storage.InMemory
{
    public class InMemoryStorage : IStorage
    {
        private readonly Dictionary<NodeId, NodeData> _nodes = new Dictionary<NodeId, NodeData>();
        private readonly DirectoryData _root;

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
            Box<IContent> encryptedContent)
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

            node.EncryptedName = encryptedName;
            return true;
        }

        public bool SetNodeContent(NodeId id, Box<IContent> encryptedContent)
        {
            if (!_nodes.TryGetValue(id, out var node))
            {
                return false;
            }

            node.EncryptedContent = encryptedContent;
            return true;
        }
    }
}