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
        IDirectoryEncryptionAspect IDirectoryNode.Encryption => _encryption;
        public DirectoryEncryptionAspect Encryption => _encryption;
        
        
        private readonly DirectoryChildrenNamesAspect _childrenNames;
        IDirectoryChildrenNamesAspect IDirectoryNode.ChildrenNames => _childrenNames;
        public DirectoryChildrenNamesAspect ChildrenNames => _childrenNames;


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

        private static IReadOnlyList<byte> ReEncrypt(
            IReadOnlyList<byte> data, 
            IReadOnlyList<IEncryptionSource>? parentEncryptionChain,
            IEncryptionSource? curEncryption,
            IEncryptionSource? newEncryption)
        {
            if (parentEncryptionChain != null)
            {
                foreach (var d in parentEncryptionChain)
                {
                    data = d.Decrypt(data);
                }
            }

            if (curEncryption != null)
            {
                data = curEncryption.Decrypt(data);
            }

            if (newEncryption != null)
            {
                data = newEncryption.Encrypt(data);
            }

            if (parentEncryptionChain != null)
            {
                for (int i = parentEncryptionChain.Count - 1; i >= 0; --i)
                {
                    data = parentEncryptionChain[i].Encrypt(data);
                }
            }

            return data;
        }

        public bool SetEncryption(EncryptionSource? nameEncryption, EncryptionSource? contentEncryption)
        {
            if (contentEncryption is { NeedCredentials: true } ||
                nameEncryption is { NeedCredentials: true })
            {
                return false;
            }
            
            if (ChildrenNames.Unlock() == LockUnlockResult.Fail ||
                ChildrenContent.Unlock() == LockUnlockResult.Fail)
            {
                return false;
            }

            IEncryptionSource? curNameEncryption = Encryption.SelfChildrenNamesEncryption();
            IEncryptionSource? curContentEncryption = Encryption.SelfChildrenContentEncryption();

            var parentContentEncryptionChain = Parent?.ChildrenContent.ContentEncryptionChain;
            var parentNamesEncryptionChain = Parent?.ChildrenNames.ChildrenNameEncryptionChain;
            
            foreach (var ch in Repository.Storage.GetChildren(Id))
            {
                var nameData = ReEncrypt(ch.EncryptedName.Data, parentNamesEncryptionChain, curNameEncryption, nameEncryption);
                var contentData = ReEncrypt(ch.EncryptedContent.Data, parentContentEncryptionChain, curContentEncryption, contentEncryption);

                Repository.Storage.SetNodeName(ch.Id, new Box<StringContent>(nameData));
                Repository.Storage.SetNodeContent(ch.Id, new Box<IContent>(contentData));
            }

            var newDirContent = new DirectoryContent(nameEncryption, contentEncryption);
            var contentBox = new Box<IContent>(newDirContent, Parent?.ChildrenContent.ContentEncryptionChain);
            Repository.Storage.SetNodeContent(Id, contentBox);
            Content.Lock();

            return true;
        }
    }
}