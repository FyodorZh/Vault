using System;
using System.Collections.Generic;
using System.Linq;
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

                if (nameEncryption != null &&
                    (nameEncryption != contentEncryption || _owner.ChildrenContent.IsLocked))
                {
                    nameEncryption.ClearCredentials();
                }
            }
            base.Lock();
            return LockUnlockResult.Success;
        }

        public IEnumerable<INode> All
        {
            get
            {
                if (Unlock() == LockUnlockResult.Fail)
                {
                    throw new Exception();
                }

                foreach (var chId in _owner.Repository.FindChildren(_owner.Id))
                {
                    INode? node = _owner.Repository.FindNode(chId);
                    if (node == null)
                    {
                        throw new Exception();
                    }

                    yield return node;
                }
            }
        }
        
        public INode? FindChild(string name)
        {
            foreach (var node in All)
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