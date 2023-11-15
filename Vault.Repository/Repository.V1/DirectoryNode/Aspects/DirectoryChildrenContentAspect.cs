using System;
using Vault.Content;
using Vault.Repository.V1;
using Vault.Storage;

namespace Vault.Repository
{
    internal class DirectoryChildrenContentAspect : LockableAspect, IDirectoryChildrenContentAspect
    {
        private readonly DirectoryNode _owner;
        
        public DirectoryChildrenContentAspect(DirectoryNode dir)
        {
            _owner = dir;
        }
        
        public override LockUnlockResult Unlock()
        {
            if (!IsLocked)
            {
                return LockUnlockResult.NothingToDo;
            }
            
            if (_owner.Encryption.Unlock() == LockUnlockResult.Fail)
            {
                return LockUnlockResult.Fail;
            }
            
            var encryption = _owner.Encryption.SelfChildrenContentEncryption();
            if (encryption is { NeedCredentials: true })
            {
                string? credentials = _owner.Repository.CredentialsProvider.GetCredentials(_owner, encryption.GetDescription());
                if (credentials == null)
                {
                    return LockUnlockResult.Fail;
                }
                
                if (!encryption.AddCredentials(credentials))
                {
                    return LockUnlockResult.Fail;
                }
            }

            return base.Unlock();
        }

        public override LockUnlockResult Lock()
        {
            if (IsLocked)
            {
                return LockUnlockResult.NothingToDo;
            }
            
            if (!_owner.Encryption.IsLocked)
            {
                var nameEncryption = _owner.Encryption.SelfChildrenNamesEncryption();
                var contentEncryption = _owner.Encryption.SelfChildrenContentEncryption();

                if (contentEncryption != null &&
                    (nameEncryption != contentEncryption || _owner.ChildrenNames.IsLocked))
                {
                    contentEncryption.ClearCredentials();
                }
            }
            base.Lock();
            return LockUnlockResult.Success;
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