using System;
using System.Collections.Generic;
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
        
        public override bool Unlock()
        {
            if (!IsLocked)
            {
                return true;
            }
            if (_owner.Encryption.Unlock())
            {
                var encryption = _owner.Encryption.SelfChildrenNamesEncryption();
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
                var encryption = _owner.Encryption.SelfChildrenNamesEncryption();
                encryption?.ClearCredentials();
            }
            base.Lock();
            if (!wasLocked)
            {
                _owner.Children2.Lock();
            }
        }

        public IEnumerable<(string, INode)> All
        {
            get
            {
                if (!Unlock())
                {
                    throw new Exception();
                }
                
                foreach (var chId in _owner.Repository.FindChildren(_owner.Id))
                {
                    INode? node = (INode?)_owner.Repository.FindDirectory(chId) ?? _owner.Repository.FindFile(chId);
                    yield return (node?.Name.Value ?? throw new Exception(), node);
                }
            }
        }

        public INode? FindChild(string name)
        {
            foreach (var pair in All)
            {
                if (pair.Item1 == name)
                {
                    return pair.Item2;
                }
            }

            return null;
        }
    }
}