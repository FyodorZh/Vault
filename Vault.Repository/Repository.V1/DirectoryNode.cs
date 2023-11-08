using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node<IDirectoryData>, IDirectoryNode
    {
        private readonly List<EncryptionSource> _decryptorsChain = new List<EncryptionSource>();
        private readonly List<EncryptionSource> _encryptorsChain = new List<EncryptionSource>();

        private readonly EncryptionSource? _childrenNamesEncryption;
        private readonly EncryptionSource _contentEncryption;
        
        
        private EncryptionSource? _decryptor;
        private EncryptionSource? _encryptor;
        
        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
            Parent?.CollectDecryptors(_decryptorsChain);
            Parent?.CollectEncryptors(_encryptorsChain);
            var encryption = Data.ContentEncryption.Deserialize(_decryptorsChain);
            _contentEncryption = encryption ?? throw new InvalidOperationException();
            _contentEncryption.SetCredentials(repository.CredentialsProvider);
        }
        
        public override void Unlock(LockState stateChange)
        {
            base.Unlock(stateChange);
            if ((stateChange & LockState.ChildrenName) != 0)
            {
                if ((State & LockState.ChildrenName) != 0)
                {
                    
                    
                    
                    State &= ~LockState.ChildrenName;
                }
            }
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) != 0)
                {
                    _decryptor = _contentEncryption;
                    _encryptor = _contentEncryption;

                    _decryptorsChain.Add(_decryptor);
                    _encryptorsChain.Add(_encryptor);

                    foreach (var child in Children)
                    {
                        child.Unlock(LockState.SelfName);
                    }
                    
                    State &= ~LockState.Content;
                }
            }
        }

        public override void Lock(LockState stateChange)
        {
            base.Lock(stateChange);
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) == 0)
                {
                    foreach (var child in Children)
                    {
                        child.Lock(LockState.All);
                    }

                    _decryptorsChain.RemoveAt(_decryptorsChain.Count - 1);
                    _encryptorsChain.RemoveAt(_encryptorsChain.Count - 1);

                    _encryptor = null;
                    _decryptor = null;
                    
                    State |= LockState.Content;
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
            if ((State & LockState.Content) != 0)
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
            var fileNode = Repository.AddFile(Id,
                new Box<StringContent>(new StringContent(name), _encryptorsChain),
                new Box<IContent>(content, _encryptorsChain));
            fileNode.Unlock(LockState.All);
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name, EncryptionSource encryptionSource)
        {
            var dirNode = Repository.AddDirectory(Id,
                new Box<StringContent>(new StringContent(name), _encryptorsChain),
                new Box<EncryptionSource>(encryptionSource, _encryptorsChain));
            dirNode.Unlock(LockState.All);
            return dirNode;
        }

        // public IEnumerable<Decryptor> DecryptorsChain
        // {
        //     get
        //     {
        //         if ((State & LockState.Content))
        //     }
        // }

        public void CollectDecryptors(List<EncryptionSource> decryptors)
        {
            if (_decryptor == null)
            {
                throw new InvalidOperationException();
            }
            
            var parent = Parent;
            if (parent != null)
            {
                parent.CollectDecryptors(decryptors);
            }

            decryptors.Add(_decryptor);
        }
        
        public void CollectEncryptors(List<EncryptionSource> encryptors)
        {
            if (_encryptor == null)
            {
                throw new InvalidOperationException();
            }
            
            encryptors.Add(_encryptor);
            
            var parent = Parent;
            if (parent != null)
            {
                parent.CollectEncryptors(encryptors);
            }
        }
    }
}