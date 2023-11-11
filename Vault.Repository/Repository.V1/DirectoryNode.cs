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

        protected override void OnContentChanged(IContent? newContent)
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
            }
            else
            {
                _encryption = (DirectoryContent)newContent;

                _contentEncryptionChain = new List<IEncryptionSource>();
                _childNameEncryptionChain = new List<IEncryptionSource>();
                if (Parent != null)
                {
                    _contentEncryptionChain.AddRange(Parent.EncryptionChain);
                    _childNameEncryptionChain.AddRange(Parent.EncryptionChain);
                }

                var forContent = _encryption.GetForContent();
                if (forContent != null)
                {
                    _contentEncryptionChain.Add(forContent);
                }
                
                var forNames = _encryption.GetForNames();
                if (forNames != null)
                {
                    _childNameEncryptionChain.Add(forNames);
                }
            }
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