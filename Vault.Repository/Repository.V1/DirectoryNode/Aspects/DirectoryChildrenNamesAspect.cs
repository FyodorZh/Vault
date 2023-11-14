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

        public IEnumerable<(string, INode)> All
        {
            get
            {
                if (!Unlock())
                {
                    throw new Exception();
                }
                
                //var chain = _owner.Encryption.ChildrenNameEncryptionChain;
                
                foreach (var ch in _owner.Children)
                {
                    yield return (ch.Name.Value ?? throw new Exception(), ch);
                }
            }
        }

        public override bool Unlock()
        {
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
            if (!_owner.Encryption.IsLocked)
            {
                var encryption = _owner.Encryption.SelfChildrenNamesEncryption();
                encryption?.ClearCredentials();
            }
            base.Lock();
        }
    }
}