using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node, IDirectoryNode
    {
        public new IDirectoryData Data { get; }
        
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
            Data = data;
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

        public bool SetEncryption(EncryptionSource nameEncryption, EncryptionSource contentEncryption)
        {
            EncryptionSource? nameAndContentEncryption = null;
            if (nameEncryption == contentEncryption)
            {
                nameAndContentEncryption = nameEncryption;
            }

            if (contentEncryption is { NeedCredentials: true } ||
                nameEncryption is { NeedCredentials: true })
            {
                return false;
            }
            
            // TODO: не должно быть необходимо декодировать всё в любом случае
            if (ChildrenNames.Unlock() == LockUnlockResult.Fail ||
                ChildrenContent.Unlock() == LockUnlockResult.Fail)
            {
                return false;
            }

            IEncryptionSource curNameEncryption = Encryption.SelfChildrenNamesEncryption();
            IEncryptionSource curContentEncryption = Encryption.SelfChildrenContentEncryption();

            var parentContentEncryptionChain = Parent?.ChildrenContent.ContentEncryptionChain;
            var parentNamesEncryptionChain = Parent?.ChildrenNames.ChildrenNameEncryptionChain;
            foreach (var ch in Repository.Storage.GetAllSubChildren(Id))
            {
                IReadOnlyList<byte> nameData = ReEncrypt(
                    ch.Name.Data, 
                    ch.ParentId == Id ? parentNamesEncryptionChain : parentContentEncryptionChain, 
                    ch.ParentId == Id ? curNameEncryption : curContentEncryption, 
                    ch.ParentId == Id ? nameEncryption : contentEncryption);
                Repository.Storage.SetNodeName(ch.Id, new Box<StringContent>(nameData));

                if (ch is IDirectoryData dir)
                {
                    IReadOnlyList<byte> contentData = ReEncrypt(
                        dir.DirContent.Data, 
                        parentContentEncryptionChain, 
                        curContentEncryption, 
                        contentEncryption);
                    Repository.Storage.SetDirectoryContent(ch.Id, new Box<IDirectoryContent>(contentData));
                }
                else if (ch is IFileData file)
                {
                    IReadOnlyList<byte> contentData = ReEncrypt(
                        file.FileContent.Data, 
                        parentContentEncryptionChain, 
                        curContentEncryption, 
                        contentEncryption);
                    Repository.Storage.SetFileContent(ch.Id, new Box<IFileContent>(contentData));
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            var newDirContent = nameAndContentEncryption != null ?
                new DirectoryContent(nameAndContentEncryption) : 
                new DirectoryContent(nameEncryption, contentEncryption);
            
            var contentBox = new Box<IDirectoryContent>(newDirContent, Parent?.ChildrenContent.ContentEncryptionChain);
            Repository.Storage.SetDirectoryContent(Id, contentBox);
            Content.Lock();

            return true;
        }
    }
}