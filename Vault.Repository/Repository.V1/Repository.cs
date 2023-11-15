using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal interface IRepositoryCtl : IRepository
    {
        IStorage Storage { get; }
        ICredentialsProvider CredentialsProvider { get; }
        DirectoryNode? FindDirectory(NodeId id);
        FileNode? FindFile(NodeId id);
        Node? FindNode(NodeId id) => (Node?)FindDirectory(id) ?? FindFile(id);
        IEnumerable<NodeId> FindChildren(NodeId parentId);

        IDirectoryNode AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent);
        
        IFileNode AddFile(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<IContent> encryptedContent);
    }
    
    public class RepositoryV1 : IRepositoryCtl
    {
        private readonly IStorage _storage;

        private readonly Dictionary<NodeId, DirectoryNode> _directories = new Dictionary<NodeId, DirectoryNode>();
        private readonly Dictionary<NodeId, FileNode> _files = new Dictionary<NodeId, FileNode>();

        public IStorage Storage => _storage;
        
        public ICredentialsProvider CredentialsProvider { get; }

        public RepositoryV1(IStorage storage, ICredentialsProvider credentialProvider)
        {
            _storage = storage;
            CredentialsProvider = credentialProvider;
        }

        public IDirectoryNode GetRoot()
        {
            var res = ((IRepositoryCtl)this).FindDirectory(_storage.Root.Id);
            if (res == null)
            {
                throw new InvalidOperationException();
            }

            return res;
        }

        DirectoryNode? IRepositoryCtl.FindDirectory(NodeId id)
        {
            if (_directories.TryGetValue(id, out var dir))
            {
                if (dir.IsValid)
                {
                    return dir;
                }
                _directories.Remove(id);
            }

            var data = _storage.GetNode(id) as IDirectoryData;
            if (data == null)
            {
                return null;
            }

            dir = new DirectoryNode(data, this);
            _directories.Add(id, dir);
            
            return dir;
        }
        
        FileNode? IRepositoryCtl.FindFile(NodeId id)
        {
            if (_files.TryGetValue(id, out var file))
            {
                if (file.IsValid)
                {
                    return file;
                }
                _files.Remove(id);
            }

            var data = _storage.GetNode(id) as IFileData;
            if (data == null)
            {
                return null;
            }

            file = new FileNode(data, this);
            _files.Add(id, file);
            
            return file;
        }
        
        public IEnumerable<NodeId> FindChildren(NodeId parentId)
        {
            foreach (var child in _storage.GetChildren(parentId))
            {
                yield return child.Id;
            }
        }

        public IDirectoryNode AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent)
        {
            IDirectoryData? data = _storage.AddDirectory(parentId, encryptedName, encryptedContent);
            if (data == null)
            {
                throw new InvalidOperationException();
            }

            DirectoryNode node = new DirectoryNode(data, this);
            _directories.Add(node.Id, node);
            return node;
        }

        public IFileNode AddFile(NodeId parentId, Box<StringContent> encryptedName, Box<IContent> encryptedContent)
        {
            IFileData? data = _storage.AddFile(parentId, encryptedName, encryptedContent);
            if (data == null)
            {
                throw new InvalidOperationException();
            }

            FileNode node = new FileNode(data, this);
            _files.Add(node.Id, node);
            return node;
        }
    }
}