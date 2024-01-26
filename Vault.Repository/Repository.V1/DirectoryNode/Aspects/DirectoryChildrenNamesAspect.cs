using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vault.Encryption;
using Vault.Repository.V1;

namespace Vault.Repository
{
    internal class DirectoryChildrenNamesAspect : LockableAspect, IDirectoryChildrenNamesAspect
    {
        private readonly DirectoryNode _owner;
        
        public DirectoryChildrenNamesAspect(DirectoryNode dir)
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
            
            var encryption = _owner.Encryption.SelfChildrenNamesEncryption();
            if (encryption is { NeedCredentials: true })
            {
                string? credentials = _owner.Repository.CredentialsProvider.GetCredentials(_owner, CredentialsType.Names, encryption.GetDescription());
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

        public override async Task<LockUnlockResult> Lock()
        {
            if (IsLocked)
            {
                return LockUnlockResult.NothingToDo;
            }
            
            if (!_owner.Encryption.IsLocked)
            {
                var nameEncryption = _owner.Encryption.SelfChildrenNamesEncryption();
                var contentEncryption = _owner.Encryption.SelfChildrenContentEncryption();

                if (nameEncryption != null &&
                    (nameEncryption != contentEncryption || _owner.ChildrenContent.IsLocked))
                {
                    nameEncryption.ClearCredentials();
                }
            }
            
            foreach (var ch in await _owner.Repository.Children(_owner.Id))
            {
                await ch.LockAll();
            }
            
            await base.Lock();
            return LockUnlockResult.Success;
        }
        
        public IEncryptionChain ChildrenNameEncryptionChain
        {
            get
            {
                if (Unlock() == LockUnlockResult.Fail)
                {
                    throw new Exception();
                }

                return _owner.Encryption.ChildrenNameEncryptionChain;
            }
        }

        public Task<IEnumerable<INode>> All
        {
            get
            {
                if (Unlock() == LockUnlockResult.Fail)
                {
                    throw new Exception();
                }

                return _owner.Repository.Children(_owner.Id);
            }
        }
        
        public async Task<INode?> FindChild(string name)
        {
            foreach (var node in await All)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }

            return null;
        }
    }
}