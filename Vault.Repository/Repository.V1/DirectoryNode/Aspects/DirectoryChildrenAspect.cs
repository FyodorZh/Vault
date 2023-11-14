using System;
using Vault.Content;
using Vault.Repository.V1;
using Vault.Storage;

namespace Vault.Repository
{
    internal class DirectoryChildrenAspect : LockableAspect, IDirectoryChildrenAspect
    {
        private readonly DirectoryNode _owner;
        
        public DirectoryChildrenAspect(DirectoryNode dir)
        {
            _owner = dir;
        }
        
        public override bool Unlock()
        {
            if (!IsLocked)
            {
                return true;
            }
            if (_owner.Encryption.Unlock() && _owner.ChildrenNames.Unlock())
            {
                var encryption = _owner.Encryption.SelfChildrenContentEncryption();
                if (encryption is { NeedCredentials: true })
                {
                    string? credentials = _owner.Repository.CredentialsProvider.GetCredentials(_owner, encryption.GetDescription());
                    if (credentials == null)
                    {
                        return false;
                    }
                
                    if (!encryption.AddCredentials(credentials))
                    {
                        return false;
                    }
                }

                return base.Unlock();
            }
            base.Lock();
            return false;
        }

        public override void Lock()
        {
            bool wasLocked = IsLocked;
            if (!_owner.Encryption.IsLocked)
            {
                var encryption = _owner.Encryption.SelfChildrenContentEncryption();
                encryption?.ClearCredentials();
            }
            base.Lock();
            if (!wasLocked)
            {
                _owner.ChildrenNames.Lock();
            }
        }


        public IFileNode AddChildFile(string name, IContent content)
        {
            Unlock();
            if (IsLocked)
            {
                throw new Exception();
            }
            
            var fileNode = _owner.Repository.AddFile(_owner.Id,
                new Box<StringContent>(new StringContent(name), _owner.Encryption.ChildrenNameEncryptionChain),
                new Box<IContent>(content, _owner.Encryption.ContentEncryptionChain));
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name)
        {
            Unlock();
            if (IsLocked)
            {
                throw new Exception();
            }
            
            var nameBox = new Box<StringContent>(new StringContent(name), _owner.Encryption.ChildrenNameEncryptionChain);
            var contentBox = new Box<DirectoryContent>(new DirectoryContent(), _owner.Encryption.ContentEncryptionChain);
            var dirNode = _owner.Repository.AddDirectory(_owner.Id, nameBox, contentBox);
            return dirNode;
        }
    }
}