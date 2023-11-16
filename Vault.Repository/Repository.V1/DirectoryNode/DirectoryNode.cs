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
            IEncryptionChain? parentEncryptionChain,
            IEncryptionSource? curEncryption,
            IEncryptionSource? newEncryption)
        {
            data = parentEncryptionChain?.Decrypt(data) ?? data;
            data = curEncryption?.Decrypt(data) ?? data;

            data = newEncryption?.Encrypt(data) ?? data;
            data = parentEncryptionChain?.Encrypt(data) ?? data;

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
            
            foreach (var ch in Repository.Storage.GetAllSubChildren(Id))
            {
                IReadOnlyList<byte> nameData = ReEncrypt(
                    ch.EncryptedName.Data, 
                    ch.ParentId == Id ? parentNamesEncryptionChain : parentContentEncryptionChain, 
                    ch.ParentId == Id ? curNameEncryption : curContentEncryption, 
                    ch.ParentId == Id ? nameEncryption : contentEncryption);
                
                IReadOnlyList<byte> contentData = ReEncrypt(
                    ch.EncryptedContent.Data, 
                    parentContentEncryptionChain, 
                    curContentEncryption, 
                    contentEncryption);

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