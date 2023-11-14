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

        
        public IDirectoryChildrenAspect Children2 => throw new NotImplementedException();

        
        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
            _encryption = new DirectoryEncryptionAspect(this);
            _childrenNames = new DirectoryChildrenNamesAspect(this);
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

        public IEnumerable<INode> Children
        {
            get
            {
                foreach (var childId in Repository.FindChildren(Data.Id))
                {
                    INode? child = Repository.FindDirectory(childId);
                    if (child == null)
                    {
                        child = Repository.FindFile(childId);
                        if (child == null)
                        {
                            throw new Exception();
                        }
                    }

                    yield return child;
                }
            }
        }

        public INode? FindChild(string name)
        {
            foreach (var child in Children)
            {
                var childName = child.Name.Value;
                if (childName == null)
                {
                    throw new InvalidOperationException();
                }

                if (childName == name)
                {
                    return child;
                }
            }

            return null;
        }

        public IFileNode AddChildFile(string name, IContent content)
        {
            var fileNode = Repository.AddFile(Id,
                new Box<StringContent>(new StringContent(name), Encryption.ChildrenNameEncryptionChain),
                new Box<IContent>(content, Encryption.ContentEncryptionChain));
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name)
        {
            var nameBox = new Box<StringContent>(new StringContent(name), Encryption.ChildrenNameEncryptionChain);
            var contentBox = new Box<DirectoryContent>(new DirectoryContent(), Encryption.ContentEncryptionChain);
            var dirNode = Repository.AddDirectory(Id, nameBox, contentBox);
            return dirNode;
        }
    }
}