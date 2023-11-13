using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal abstract class Node : INode
    {
        public INodeData Data { get; }
        
        protected IRepositoryCtl Repository { get; }


        protected Node(INodeData data, IRepositoryCtl repository)
        {
            Data = data;
            Repository = repository;
            Name = new NameState(this);
        }

        public bool IsValid => Data.IsValid;

        public NodeId Id => Data.Id;

        public ILockedState<string> Name { get; }
        
        public abstract ILockedState<IContent> Content { get; }

        public virtual void LockAll()
        {
            Content.Lock();
            Name.Lock();
        }

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId.IsValid ? Repository.FindDirectory(Data.ParentId) : null;
        
        private class NameState : LockedState<string, string>
        {
            private readonly Node _owner;
        
            internal NameState(Node owner)
                : base(true)
            {
                _owner = owner;
            }
        
            protected override string? UnlockState()
            {
                return _owner.Data.EncryptedName.Deserialize(_owner.Parent?.ChildrenNameEncryptionChain)?.Content;
            }
        }

        protected abstract class ContentState<TContent> : LockedState<IContent, TContent>
            where TContent : class, IContent
        {
            private readonly Node _owner;

            protected abstract bool UnlockContent(TContent content);
            
            protected ContentState(Node node) 
                : base(true)
            {
                _owner = node;
            }

            protected sealed override TContent? UnlockState()
            {
                IContent? c = _owner.Data.EncryptedContent.Deserialize(_owner.Parent?.EncryptionChain);
                if (c == null)
                {
                    return null;
                }
                TContent content = (TContent)c;
                if (UnlockContent(content))
                {
                    return content;
                }

                return null;
            }
        }
    }
}