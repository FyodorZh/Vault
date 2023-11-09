using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class FileNode : Node<IFileData>, IFileNode
    {
        private IContent? _content;

        public IContent Content => _content ?? throw new Exception();
        
        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
            State &= ~LockState.ChildrenName;
        }

        public override void Unlock(LockState stateChange)
        {
            base.Unlock(stateChange);
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) != 0)
                {
                    _content = Data.EncryptedContent.Deserialize(Parent?.EncryptionChain);
                    
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
                    _content = null;                    
                    State |= LockState.Content;
                }
            }
        }
    }
}