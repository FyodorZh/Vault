using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Storage.InMemory
{
    public class InMemoryStorage : IStorage
    {
        private readonly Dictionary<Guid, NodeData> _nodes = new Dictionary<Guid, NodeData>();
        private readonly DirectoryData _root;

        public InMemoryStorage(Box<StringContent> encryptedName,
            Box<EncryptionSource> contentEncryption, bool encryptChildrenNames)
        {
            _root = new DirectoryData(Guid.NewGuid(), null, encryptedName, contentEncryption, encryptChildrenNames);
            _nodes.Add(_root.Id, _root);
        }

        public void Add(NodeData data)
        {
            if (data.ParentId == null || !_nodes.ContainsKey(data.ParentId.Value))
            {
                throw new Exception();
            }
            
            _nodes.Add(data.Id, data);
        }

        INodeData IStorage.Root => _root;

        INodeData? IStorage.GetNode(Guid id)
        {
            _nodes.TryGetValue(id, out var node);
            return node;
        }

        IEnumerable<INodeData> IStorage.GetChildren(Guid id)
        {
            foreach (var node in _nodes.Values)
            {
                if (node.ParentId == id)
                {
                    yield return node;
                }
            }
        }
        
        IDirectoryData IStorage.AddDirectory(Guid parentId, Box<StringContent> encryptedName, Box<EncryptionSource> contentEncryption, bool encryptChildrenNames)
        {
            if (_nodes.TryGetValue(parentId, out var parent) || !(parent is DirectoryData))
            {
                throw new InvalidOperationException();
            }
            
            var node = new DirectoryData(Guid.NewGuid(), parentId, encryptedName, contentEncryption, encryptChildrenNames);
            _nodes.Add(node.Id, node);
            return node;
        }

        IFileData IStorage.AddFile(Guid parentId, Box<StringContent> encryptedName, Box<IContent> encryptedContent)
        {
            if (_nodes.TryGetValue(parentId, out var parent) || !(parent is DirectoryData))
            {
                throw new InvalidOperationException();
            }
            
            var node = new FileData(Guid.NewGuid(), parentId, encryptedName, encryptedContent);
            _nodes.Add(node.Id, node);
            return node;
        }
    }
}