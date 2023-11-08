using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node<IDirectoryData>, IDirectoryNode
    {
        private readonly List<Decryptor> _decryptorsChain = new List<Decryptor>();
        private readonly List<Encryptor> _encryptorsChain = new List<Encryptor>();
        
        private readonly EncryptionSource _encryption;
        
        private Decryptor? _decryptor;
        private Encryptor? _encryptor;
        
        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
            Parent?.CollectDecryptors(_decryptorsChain);
            Parent?.CollectEncryptors(_encryptorsChain);
            var encryption = Data.ContentEncryption.Deserialize(_decryptorsChain);
            _encryption = encryption ?? throw new InvalidOperationException();
            _encryption.SetCredentials(repository.CredentialsProvider);
        }
        
        public override void Unlock(LockState stateChange)
        {
            base.Unlock(stateChange);
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) != 0)
                {
                    _decryptor = _encryption.ConstructDecryptor();
                    _encryptor = _encryption.ConstructEncryptor();

                    _decryptorsChain.Add(_decryptor);
                    _encryptorsChain.Add(_encryptor);

                    foreach (var child in Children)
                    {
                        child.Unlock(LockState.Name);
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
                foreach (var childId in _repository.FindChildren(Data.Id))
                {
                    INode? child = _repository.FindDirectory(childId);
                    if (child == null)
                    {
                        child = _repository.FindFile(childId);
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
            var fileNode = _repository.AddFile(Id,
                new Box<StringContent>(new StringContent(name), _encryptorsChain),
                new Box<IContent>(content, _encryptorsChain));
            fileNode.Unlock(LockState.All);
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name, EncryptionSource encryptionSource)
        {
            var dirNode = _repository.AddDirectory(Id,
                new Box<StringContent>(new StringContent(name), _encryptorsChain),
                new Box<EncryptionSource>(encryptionSource, _encryptorsChain));
            dirNode.Unlock(LockState.All);
            return dirNode;
        }
        
        public void CollectDecryptors(List<Decryptor> decryptors)
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
        
        public void CollectEncryptors(List<Encryptor> encryptors)
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