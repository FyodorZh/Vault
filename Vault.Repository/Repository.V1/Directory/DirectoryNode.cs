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
        }

        public bool IsLocked => _decryptor == null || _encryptor == null;

        public bool Unlock()
        {
            if (IsLocked)
            {
                _decryptor = _encryption.ConstructDecryptor();
                if (_decryptor == null)
                {
                    throw new InvalidOperationException();
                }
                
                _encryptor = _encryption.ConstructEncryptor();
                if (_encryptor == null)
                {
                    throw new InvalidOperationException();
                }

                if (_decryptor.RequireCredentials || _encryptor.RequireCredentials)
                {
                    var credentials = _repository.CredentialsProvider();
                    if (_decryptor.RequireCredentials)
                        _decryptor.SetCredentials(credentials);
                    if (_encryptor.RequireCredentials)
                        _encryptor.SetCredentials(credentials);
                }

                _decryptorsChain.Add(_decryptor);
                _encryptorsChain.Add(_encryptor);

                foreach (var child in Children)
                {
                    child.DecryptName(_decryptorsChain);
                    // todo log error
                }
            }
            return true;
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
            if (IsLocked)
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
            return _repository.AddFile(Id,
                new Box<StringContent>(new StringContent(name), _encryptorsChain),
                new Box<IContent>(content, _encryptorsChain));
        }

        public IDirectoryNode AddChildDirectory(string name, EncryptionSource encryptionSource)
        {
            return _repository.AddDirectory(Id,
                new Box<StringContent>(new StringContent(name), _encryptorsChain),
                new Box<EncryptionSource>(encryptionSource, _encryptorsChain));
        }
        
        private void CollectDecryptors(List<Decryptor> decryptors)
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
        
        private void CollectEncryptors(List<Encryptor> encryptors)
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