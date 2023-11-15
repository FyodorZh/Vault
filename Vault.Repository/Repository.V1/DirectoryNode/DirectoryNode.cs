using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node, IDirectoryNode
    {
        private readonly DirectoryEncryptionAspect _encryption;
        public override ILockableAspect<IContent> Content => _encryption;
        public IDirectoryEncryptionAspect Encryption => _encryption;

        
        private readonly DirectoryChildrenNamesAspect _childrenNames;
        public IDirectoryChildrenNamesAspect ChildrenNames => _childrenNames;


        private readonly DirectoryChildrenContentAspect _childrenContent;
        public IDirectoryChildrenContentAspect ChildrenContent => _childrenContent;

        
        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
            _encryption = new DirectoryEncryptionAspect(this);
            _childrenNames = new DirectoryChildrenNamesAspect(this);
            _childrenContent = new DirectoryChildrenContentAspect(this);
        }

        public override void LockAll()
        {
            ChildrenContent.Lock();
            ChildrenNames.Lock();
            Encryption.Lock();
            base.LockAll();
        }

        public bool SetEncryption(EncryptionSource? contentEncryption, EncryptionSource? namesEncryption)
        {
            // if (!UnlockContent())
            // {
            //     return false;
            // }
            //
            // if (contentEncryption != null && contentEncryption.NeedCredentials)
            // {
            //     string? credentials = Repository.CredentialsProvider.GetCredentials(
            //         this, contentEncryption.GetDescription());
            //     if (credentials == null || !contentEncryption.AddCredentials(credentials))
            //     {
            //         return false;
            //     }
            // }
            //
            // if (namesEncryption != null && namesEncryption.NeedCredentials)
            // {
            //     string? credentials = Repository.CredentialsProvider.GetCredentials(
            //         this, namesEncryption.GetDescription());
            //     if (credentials == null || !namesEncryption.AddCredentials(credentials))
            //     {
            //         return false;
            //     }
            // }
            //
            // DirectoryContent content = new DirectoryContent(namesEncryption, contentEncryption);
            //
            // if (!SetContent(content))
            // {
            //     return false;
            // }
            //
            // if (!Repository.Storage.SetDirectoryContent(Id,
            //         new Box<DirectoryContent>(content, Parent?.EncryptionChain)))
            // {
            //     LockContent();
            //     return false;
            // }
            //
            // return true;
            throw new NotImplementedException();
        }
    }
}