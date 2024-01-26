using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal interface IRepositoryCtl : IRepository
    {
        IStorage Storage { get; }
        ICredentialsProvider CredentialsProvider { get; }
        
        Task<DirectoryNode?> FindDirectory(NodeId id);
        Task<FileNode?> FindFile(NodeId id);
        async Task<Node?> FindNode(NodeId id) => (Node?)await FindDirectory(id) ?? await FindFile(id);
        
        Task<IEnumerable<NodeId>> FindChildren(NodeId parentId);

        async Task<IEnumerable<INode>> Children(NodeId parentId)
        {
            List<INode> list = new List<INode>();
            foreach (var id in await FindChildren(parentId))
            {
                list.Add(await FindNode(id) ?? throw new Exception());
            }
            return list;
        }

        Task<IDirectoryNode> AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent);
        
        Task<IFileNode> AddFile(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<FileContent> encryptedContent);
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

        public async Task<IDirectoryNode> GetRoot()
        {
            var res = await ((IRepositoryCtl)this).FindDirectory((await _storage.GetRoot()).Id);
            if (res == null)
            {
                throw new InvalidOperationException();
            }

            return res;
        }

        async Task<DirectoryNode?> IRepositoryCtl.FindDirectory(NodeId id)
        {
            if (_directories.TryGetValue(id, out var dir))
            {
                if (dir.IsValid)
                {
                    return dir;
                }
                _directories.Remove(id);
            }

            var data = await _storage.GetNode(id) as IDirectoryData;
            if (data == null)
            {
                return null;
            }
            
            var parent = await ((IRepositoryCtl)this).FindDirectory(data.ParentId);
            
            dir = new DirectoryNode(data, parent, this);
            _directories.Add(id, dir);
            
            return dir;
        }
        
        async Task<FileNode?> IRepositoryCtl.FindFile(NodeId id)
        {
            if (_files.TryGetValue(id, out var file))
            {
                if (file.IsValid)
                {
                    return file;
                }
                _files.Remove(id);
            }

            var data = await _storage.GetNode(id) as IFileData;
            if (data == null)
            {
                return null;
            }
            
            var parent = await ((IRepositoryCtl)this).FindDirectory(data.ParentId);

            file = new FileNode(data, parent, this);
            _files.Add(id, file);
            
            return file;
        }
        
        public async Task<IEnumerable<NodeId>> FindChildren(NodeId parentId)
        {
            List<NodeId> list = new List<NodeId>();
            foreach (var child in await _storage.GetChildren(parentId))
            {
                list.Add(child.Id);
            }
            return list;
        }

        public async Task<IDirectoryNode> AddDirectory(
            NodeId parentId, 
            Box<StringContent> encryptedName, 
            Box<DirectoryContent> encryptedContent)
        {
            IDirectoryData? data = await _storage.AddDirectory(parentId, encryptedName, encryptedContent);
            if (data == null)
            {
                throw new InvalidOperationException();
            }

            var parent = await ((IRepositoryCtl)this).FindDirectory(parentId);

            DirectoryNode node = new DirectoryNode(data, parent, this);
            _directories.Add(node.Id, node);
            return node;
        }

        public async Task<IFileNode> AddFile(NodeId parentId, Box<StringContent> encryptedName, Box<FileContent> encryptedContent)
        {
            IFileData? data = await _storage.AddFile(parentId, encryptedName, encryptedContent);
            if (data == null)
            {
                throw new InvalidOperationException();
            }
            
            var parent = await ((IRepositoryCtl)this).FindDirectory(parentId);

            FileNode node = new FileNode(data, parent, this);
            _files.Add(node.Id, node);
            return node;
        }
    }
}