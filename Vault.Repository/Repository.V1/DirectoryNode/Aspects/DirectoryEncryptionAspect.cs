using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository.V1
{
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

        public IEnumerable<IEncryptionSource> ContentEncryptionChain
        {
            get
            {
                Unlock();
                return _contentEncryptionChain ?? throw new Exception();
            }
        }

        public IEnumerable<IEncryptionSource> ChildrenNameEncryptionChain
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
                contentEncryptionChain.AddRange(_owner.Parent.Encryption.ContentEncryptionChain);
                childNameEncryptionChain.AddRange(_owner.Parent.Encryption.ContentEncryptionChain);
            }

            var selfChildrenContentEncryption = encryption.GetForContent();
            var selfChildrenNamesEncryption = encryption.GetForNames();

            if (selfChildrenContentEncryption != null)
            {
                // if (contentEncryption.NeedCredentials)
                // {
                //     string? credentials = Repository.CredentialsProvider.GetCredentials(this, contentEncryption.GetDescription());
                //     if (credentials == null)
                //     {
                //         return false;
                //     }
                //
                //     if (!contentEncryption.AddCredentials(credentials))
                //     {
                //         return false;
                //     }
                // }

                contentEncryptionChain.Add(selfChildrenContentEncryption);
            }

            if (selfChildrenNamesEncryption != null)
            {
                // if (namesEncryption.NeedCredentials)
                // {
                //     string? credentials = Repository.CredentialsProvider.GetCredentials(this, namesEncryption.GetDescription());
                //     if (credentials == null)
                //     {
                //         return false;
                //     }
                //
                //     if (!namesEncryption.AddCredentials(credentials))
                //     {
                //         return false;
                //     }
                // }

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
            foreach (var child in _owner.Children)
            {
                child.LockAll();
            }
            
            _selfChildrenContentEncryption = null;
            _selfChildrenNamesEncryption = null;

            _contentEncryptionChain = null;
            _childNameEncryptionChain = null;
        }
    }
}