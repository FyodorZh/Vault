using System;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository.V1
{
    /// <summary>
    /// Encryption combinations (name, content)
    /// (null, null) => no encryption
    /// (null, content) => file names and file content encrypted same way
    /// (name, null) => only file names are encrypted
    /// (PLAIN, content) => only content encrypted (PLAIN is plane data encryption)
    /// </summary>
    
    internal class DirectoryEncryptionAspect : LockableAspect<IDirectoryContent, IDirectoryContent>, IDirectoryEncryptionAspect
    {
        private readonly DirectoryNode _owner;

        private IEncryptionSource? _selfChildrenContentEncryption;
        private IEncryptionSource? _selfChildrenNamesEncryption;

        private IEncryptionChain? _contentEncryptionChain;
        private IEncryptionChain? _childNameEncryptionChain;

        public IEncryptionSource SelfChildrenNamesEncryption()
        {
            Unlock();
            if (IsLocked)
            {
                throw new Exception();
            }

            return _selfChildrenNamesEncryption ?? throw new Exception("Impossible");
        }

        public IEncryptionSource SelfChildrenContentEncryption()
        {
            Unlock();
            if (IsLocked)
            {
                throw new Exception();
            }

            return _selfChildrenContentEncryption ?? throw new Exception("Impossible");
        }

        public IEncryptionChain ContentEncryptionChain
        {
            get
            {
                Unlock();
                return _contentEncryptionChain ?? throw new Exception();
            }
        }

        public IEncryptionChain ChildrenNameEncryptionChain
        {
            get
            {
                Unlock();
                return _childNameEncryptionChain ?? throw new Exception();
            }
        }

        public DirectoryEncryptionAspect(DirectoryNode node)
            : base(true)
        {
            _owner = node;
        }

        protected override IDirectoryContent? UnlockState()
        {
            IDirectoryContent? dirContent = _owner.Data.DirContent.Deserialize(_owner.Parent?.ChildrenContent.ContentEncryptionChain);
            if (dirContent == null)
            {
                return null;
            }

            _selfChildrenNamesEncryption = dirContent.GetForNames();
            _selfChildrenContentEncryption = dirContent.GetForContent();

            _childNameEncryptionChain = _owner.Parent?.ChildrenContent.ContentEncryptionChain ?? VoidEncryptionChain.Instance;
            _contentEncryptionChain = _owner.Parent?.ChildrenContent.ContentEncryptionChain ?? VoidEncryptionChain.Instance;
            
            _childNameEncryptionChain = new EncryptionChain(_childNameEncryptionChain, _selfChildrenNamesEncryption);
            _contentEncryptionChain = new EncryptionChain(_contentEncryptionChain, _selfChildrenContentEncryption);
            
            return dirContent;
        }

        protected override void LockState()
        {
            foreach (var child in _owner.Repository.Children(_owner.Id))
            {
                child.LockAll();
            }
            
            _selfChildrenContentEncryption = null;
            _selfChildrenNamesEncryption = null;
            
            _contentEncryptionChain?.Destroy();
            _childNameEncryptionChain?.Destroy();

            _contentEncryptionChain = null;
            _childNameEncryptionChain = null;
            
            _owner.ChildrenContent.Lock();
            _owner.ChildrenNames.Lock();
        }
    }
}