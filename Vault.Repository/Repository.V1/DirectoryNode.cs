using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node<IDirectoryData>, IDirectoryNode
    {
        private DirectoryContent? _encryption;

        private List<IEncryptionSource>? _contentEncryptionChain;
        private List<IEncryptionSource>? _childNameEncryptionChain;

        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
        }

        protected override bool ProcessContent(IContent? newContent)
        {
            if (newContent == null)
            {
                foreach (var child in Children)
                {
                    child.LockAll();
                }

                _encryption = null;
                _contentEncryptionChain = null;
                _childNameEncryptionChain = null;
                return true;
            }
            
            var encryption = newContent as DirectoryContent;
            if (encryption == null)
            {
                return false;
            }
            
            var contentEncryptionChain = new List<IEncryptionSource>();
            var childNameEncryptionChain = new List<IEncryptionSource>();
            if (Parent != null)
            {
                contentEncryptionChain.AddRange(Parent.EncryptionChain);
                childNameEncryptionChain.AddRange(Parent.EncryptionChain);
            }
            
            var contentEncryption = encryption.GetForContent();
            var namesEncryption = encryption.GetForNames();

            if (contentEncryption != null)
            {
                if (contentEncryption.NeedCredentials)
                {
                    string? credentials = Repository.CredentialsProvider.GetCredentials(this, contentEncryption.GetDescription());
                    if (credentials == null)
                    {
                        return false;
                    }
                    contentEncryption.AddCredentials(credentials);
                }
                contentEncryptionChain.Add(contentEncryption);
            }
                
            if (namesEncryption != null)
            {
                if (namesEncryption.NeedCredentials)
                {
                    string? credentials = Repository.CredentialsProvider.GetCredentials(this, namesEncryption.GetDescription());
                    if (credentials == null)
                    {
                        return false;
                    }
                    namesEncryption.AddCredentials(credentials);
                }
                childNameEncryptionChain.Add(namesEncryption);
            }

            _encryption = encryption;
            _childNameEncryptionChain = childNameEncryptionChain;
            _contentEncryptionChain = contentEncryptionChain;

            return true;
        }

        public IEnumerable<INode> Children
        {
            get
            {
                foreach (var childId in Repository.FindChildren(Data.Id))
                {
                    INode? child = Repository.FindDirectory(childId);
                    if (child == null)
                    {
                        child = Repository.FindFile(childId);
                        if (child == null)
                        {
                            throw new Exception();
                        }
                    }

                    yield return child;
                }
            }
        }
        
        public INode? FindChild(string name)
        {
            foreach (var child in Children)
            {
                var childName = child.Name;
                if (childName == null)
                {
                    throw new InvalidOperationException();
                }

                if (childName == name)
                {
                    return child;
                }
            }

            return null;
        }

        public IFileNode AddChildFile(string name, IContent content)
        {
            var fileNode = Repository.AddFile(Id,
                new Box<StringContent>(new StringContent(name), ChildrenNameEncryptionChain),
                new Box<IContent>(content, EncryptionChain));
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name)
        {
            var nameBox = new Box<StringContent>(new StringContent(name), ChildrenNameEncryptionChain);
            var contentBox = new Box<DirectoryContent>(new DirectoryContent(), EncryptionChain);
            var dirNode = Repository.AddDirectory(Id, nameBox, contentBox);
            return dirNode;
        }

        public IEnumerable<IEncryptionSource> EncryptionChain
        {
            get
            {
                UnlockContent();
                return _contentEncryptionChain ?? throw new Exception();
            }
        }
        
        public IEnumerable<IEncryptionSource> ChildrenNameEncryptionChain
        {
            get
            {
                UnlockContent();
                return _childNameEncryptionChain ?? throw new Exception();
            }
        }
    }
}