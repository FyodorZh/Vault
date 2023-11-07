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

        public bool IsLocked => _content == null;

        public void Unlock()
        {
            if (IsLocked)
            {
                var decryptors = new List<Decryptor>();
                Parent!.CollectDecryptors(decryptors);
                _content = Data.EncryptedContent.Deserialize(decryptors);
            }
        }

        public IContent Content => _content ?? throw new Exception();
        
        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
        }
    }
}