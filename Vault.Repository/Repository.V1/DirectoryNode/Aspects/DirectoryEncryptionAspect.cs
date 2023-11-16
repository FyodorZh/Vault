using System;
using System.Collections.Generic;
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
    
    internal class DirectoryEncryptionAspect : ContentAspect<DirectoryContent>, IDirectoryEncryptionAspect
    {
        private readonly DirectoryNode _owner;

        private IEncryptionSource? _selfChildrenContentEncryption;
        private IEncryptionSource? _selfChildrenNamesEncryption;

        private List<IEncryptionSource>? _contentEncryptionChain;
        private List<IEncryptionSource>? _childNameEncryptionChain;

        public IEncryptionSource? SelfChildrenNamesEncryption()
        {
            Unlock();
            if (IsLocked)
            {
                throw new Exception();
            }

            return _selfChildrenNamesEncryption;
        }

        public IEncryptionSource? SelfChildrenContentEncryption()
        {
            Unlock();
            if (IsLocked)
            {
                throw new Exception();
            }

            return _selfChildrenContentEncryption;
        }

        public IReadOnlyList<IEncryptionSource> ContentEncryptionChain
        {
            get
            {
                Unlock();
                return _contentEncryptionChain ?? throw new Exception();
            }
        }

        public IReadOnlyList<IEncryptionSource> ChildrenNameEncryptionChain
        {
            get
            {
                Unlock();
                return _childNameEncryptionChain ?? throw new Exception();
            }
        }

        public DirectoryEncryptionAspect(DirectoryNode node)
            : base(node)
        {
            _owner = node;
        }

        protected override bool UnlockContent(DirectoryContent encryption)
        {
            var contentEncryptionChain = new List<IEncryptionSource>();
            var childNameEncryptionChain = new List<IEncryptionSource>();
            if (_owner.Parent != null)
            {
                contentEncryptionChain.AddRange(_owner.Parent.ChildrenContent.ContentEncryptionChain);
                childNameEncryptionChain.AddRange(_owner.Parent.ChildrenContent.ContentEncryptionChain);
            }

            var selfChildrenContentEncryption = encryption.GetForContent();
            var selfChildrenNamesEncryption = encryption.GetForNames();

            if (selfChildrenContentEncryption != null)
            {
                contentEncryptionChain.Add(selfChildrenContentEncryption);
            }

            if (selfChildrenNamesEncryption != null)
            {
                childNameEncryptionChain.Add(selfChildrenNamesEncryption);
            }

            _selfChildrenNamesEncryption = selfChildrenNamesEncryption;
            _selfChildrenContentEncryption = selfChildrenContentEncryption;

            _childNameEncryptionChain = childNameEncryptionChain;
            _contentEncryptionChain = contentEncryptionChain;

            return true;
        }

        protected override void LockState()
        {
            foreach (var child in _owner.Repository.Children(_owner.Id))
            {
                child.LockAll();
            }
            
            _selfChildrenContentEncryption = null;
            _selfChildrenNamesEncryption = null;

            _contentEncryptionChain = null;
            _childNameEncryptionChain = null;
            
            _owner.ChildrenContent.Lock();
            _owner.ChildrenNames.Lock();
        }
    }
}