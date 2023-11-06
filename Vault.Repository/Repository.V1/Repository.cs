using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;
using Vault.Storage.InMemory;

namespace Vault.Repository.V1
{
    internal interface IRepositoryCtl : IRepository
    {
        Func<string> CredentialsProvider { get; }
        DirectoryNode? FindDirectory(Guid id);
        FileNode? FindFile(Guid id);
        IEnumerable<Guid> FindChildren(Guid parentId);

        IDirectoryNode AddDirectory(Guid parentId, Box<StringContent> encryptedName, Box<EncryptionSource> encryption);
        IFileNode AddFile(Guid parentId, Box<StringContent> encryptedName, Box<IContent> encryptedContent);
    }
    
    public class RepositoryV1 : IRepository, IRepositoryCtl
    {
        private readonly IStorage _storage;

        private readonly Dictionary<Guid, DirectoryNode> _directories = new Dictionary<Guid, DirectoryNode>();
        private readonly Dictionary<Guid, FileNode> _files = new Dictionary<Guid, FileNode>();

        public Func<string> CredentialsProvider { get; }

        public RepositoryV1(IStorage storage, Func<string> credentialProvider)
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

        DirectoryNode? IRepositoryCtl.FindDirectory(Guid id)
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
        
        FileNode? IRepositoryCtl.FindFile(Guid id)
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
        
        public IEnumerable<Guid> FindChildren(Guid parentId)
        {
            throw new NotImplementedException();
        }

        public IDirectoryNode AddDirectory(Guid parentId, Box<StringContent> encryptedName, Box<EncryptionSource> encryption)
        {
            IDirectoryData? data = _storage.AddDirectory(parentId, encryptedName, encryption, true);
            if (data == null)
            {
                throw new InvalidOperationException();
            }

            DirectoryNode node = new DirectoryNode(data, this);
            _directories.Add(node.Id, node);
            return node;
        }

        public IFileNode AddFile(Guid parentId, Box<StringContent> encryptedName, Box<IContent> encryptedContent)
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