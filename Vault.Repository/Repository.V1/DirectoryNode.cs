using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class DirectoryNode : Node, IDirectoryNode
    {
        private readonly DirectoryContentState _content;
        public override ILockedState<IContent> Content => _content;

        public DirectoryNode(IDirectoryData data, IRepositoryCtl repository)
            : base(data, repository)
        {
            _content = new DirectoryContentState(this);
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
                new Box<StringContent>(new StringContent(name), ChildrenNameEncryptionChain),
                new Box<IContent>(content, EncryptionChain));
            return fileNode;
        }

        public IDirectoryNode AddChildDirectory(string name)
        {
            var nameBox = new Box<StringContent>(new StringContent(name), ChildrenNameEncryptionChain);
            var contentBox = new Box<DirectoryContent>(new DirectoryContent(), EncryptionChain);
            var dirNode = Repository.AddDirectory(Id, nameBox, contentBox);
            return dirNode;
        }

        public IEnumerable<IEncryptionSource> EncryptionChain
        {
            get
            {
                _content.Unlock();
                return _content.ContentEncryptionChain ?? throw new Exception();
            }
        }

        public IEnumerable<IEncryptionSource> ChildrenNameEncryptionChain
        {
            get
            {
                _content.Unlock();
                return _content.ChildNameEncryptionChain ?? throw new Exception();
            }
        }

        private class DirectoryContentState : ContentState<DirectoryContent>
        {
            private readonly DirectoryNode _owner;

            private List<IEncryptionSource>? _contentEncryptionChain;
            private List<IEncryptionSource>? _childNameEncryptionChain;

            public IEnumerable<IEncryptionSource>? ContentEncryptionChain => _contentEncryptionChain;
            public IEnumerable<IEncryptionSource>? ChildNameEncryptionChain => _childNameEncryptionChain;

            public DirectoryContentState(DirectoryNode node)
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
                    contentEncryptionChain.AddRange(_owner.Parent.EncryptionChain);
                    childNameEncryptionChain.AddRange(_owner.Parent.EncryptionChain);
                }

                var contentEncryption = encryption.GetForContent();
                var namesEncryption = encryption.GetForNames();

                if (contentEncryption != null)
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

                    contentEncryptionChain.Add(contentEncryption);
                }

                if (namesEncryption != null)
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

                    childNameEncryptionChain.Add(namesEncryption);
                }

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

                _contentEncryptionChain = null;
                _childNameEncryptionChain = null;
            }
        }
    }
}