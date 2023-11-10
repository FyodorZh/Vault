using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node<IDirectoryData>, IDirectoryNode
    {
        private readonly EncryptionSource? _childrenNamesEncryption;
        private readonly EncryptionSource? _contentEncryption;

        private readonly List<IEncryptionSource> _contentEncryptionChain = new List<IEncryptionSource>();
        private readonly List<IEncryptionSource> _childNameEncryptionChain = new List<IEncryptionSource>();

        public EncryptionDesc? ChildrenNamesEncryption => _childrenNamesEncryption?.GetDescription();
        public EncryptionDesc? ContentEncryption => _contentEncryption?.GetDescription();

        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
            _contentEncryption = Data.ContentEncryption?.Deserialize(Parent?.EncryptionChain);
            _childrenNamesEncryption = Data.ChildrenNameEncryption?.Deserialize(Parent?.EncryptionChain);
            
            _contentEncryption?.SetCredentials(repository.CredentialsProvider);
            _childrenNamesEncryption?.SetCredentials(repository.CredentialsProvider);
        }
        
        public override void Unlock(LockState stateChange)
        {
            base.Unlock(stateChange);
            if ((stateChange & LockState.ChildrenName) != 0)
            {
                if ((State & LockState.ChildrenName) != 0)
                {
                    if (Parent != null)
                    {
                        _childNameEncryptionChain.AddRange(Parent.EncryptionChain);
                    }

                    var childNameEncryption = _childrenNamesEncryption ?? _contentEncryption;
                    if (childNameEncryption != null)
                    {
                        _childNameEncryptionChain.Add(childNameEncryption);
                    }
                    
                    State &= ~LockState.ChildrenName;
                    
                    foreach (var child in Children)
                    {
                        child.Unlock(LockState.SelfName);
                    }
                }
            }
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) != 0)
                {
                    if (Parent != null)
                    {
                        _contentEncryptionChain.AddRange(Parent.EncryptionChain);
                    }

                    if (_contentEncryption != null)
                    {
                        _contentEncryptionChain.Add(_contentEncryption);
                    }
                    
                    State &= ~LockState.Content;
                    
                    foreach (var child in Children)
                    {
                        child.Unlock(LockState.Content);
                    }
                }
            }
        }

        public override void Lock(LockState stateChange)
        {
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) == 0)
                {
                    foreach (var child in Children)
                    {
                        child.Lock(LockState.Content);
                    }
                    _contentEncryptionChain.RemoveAt(_contentEncryptionChain.Count - 1);
                    State |= LockState.Content;
                }
            }
            if ((stateChange & LockState.ChildrenName) != 0)
            {
                if ((State & LockState.ChildrenName) == 0)
                {
                    foreach (var child in Children)
                    {
                        child.Lock(LockState.ChildrenName | LockState.SelfName);
                    }
                    _childNameEncryptionChain.RemoveAt(_childNameEncryptionChain.Count - 1);
                    State |= LockState.ChildrenName;
                }
            }
            base.Lock(stateChange);
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
            if ((State & LockState.ChildrenName) != 0)
            {
                throw new InvalidOperationException();
            }

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
            if (State != LockState.Open)
            {
                throw new InvalidOperationException();
            }
            
            var fileNode = Repository.AddFile(Id,
                new Box<StringContent>(new StringContent(name), ChildrenNameEncryptionChain),
                new Box<IContent>(content, EncryptionChain));
            fileNode.Unlock(LockState.All);
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name, EncryptionSource encryptionSource)
        {            
            if (State != LockState.Open)
            {
                throw new InvalidOperationException();
            }
            
            var dirNode = Repository.AddDirectory(Id,
                new Box<StringContent>(new StringContent(name), ChildrenNameEncryptionChain),
                new Box<EncryptionSource>(encryptionSource, EncryptionChain));
            dirNode.Unlock(LockState.All);
            return dirNode;
        }

        public IEnumerable<IEncryptionSource> EncryptionChain
        {
            get
            {
                if ((State & LockState.Content) != 0)
                {
                    throw new InvalidOperationException();
                }

                return _contentEncryptionChain;
            }
        }
        
        public IEnumerable<IEncryptionSource> ChildrenNameEncryptionChain
        {
            get
            {
                if ((State & LockState.ChildrenName) != 0)
                {
                    throw new InvalidOperationException();
                }

                return _childNameEncryptionChain;
            }
        }
    }
}