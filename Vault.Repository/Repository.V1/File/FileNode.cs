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

        public override void Unlock(LockState stateChange)
        {
            base.Unlock(stateChange);
            if ((stateChange & LockState.Content) != 0)
            {
                if ((State & LockState.Content) != 0)
                {
                    var decryptors = new List<Decryptor>();
                    Parent!.CollectDecryptors(decryptors);
                    _content = Data.EncryptedContent.Deserialize(decryptors);
                    
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

        public IContent Content => _content ?? throw new Exception();
        
        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
        }
    }
}