using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal abstract class Node<TNodeData> : INode
        where TNodeData : class, INodeData
    {
        private string? _name;
        private IContent? _content;
        
        protected TNodeData Data { get; }
        
        protected IRepositoryCtl Repository { get; }

        protected abstract bool ProcessContent(IContent? newContent);

        protected Node(TNodeData data, IRepositoryCtl repository)
        {
            Data = data;
            Repository = repository;
        }

        public bool IsValid => Data.IsValid;

        public NodeId Id => Data.Id;

        public LockState State { get; protected set; } = LockState.Closed;

        private bool SetContent(IContent? content)
        {
            if (content != _content)
            {
                var oldContent = _content;
                _content = content;
                if (!ProcessContent(_content))
                {
                    _content = oldContent;
                    return false;
                }
            }

            return true;
        }

        public virtual void LockAll()
        {
            LockContent();
            LockName();
        }
        
        public bool UnlockName()
        {
            if ((State & LockState.SelfName) != 0)
            {
                _name = Data.EncryptedName.Deserialize(Parent?.ChildrenNameEncryptionChain)?.Content;
                if (_name == null)
                {
                    return false;
                }
                State &= ~LockState.SelfName;
            }
            return true;
        }

        public void LockName()
        {
            if ((State & LockState.SelfName) == 0)
            {
                if (Data.ParentId.IsValid)
                {
                    _name = null;
                    State |= LockState.SelfName;
                }
            }
        }

        public bool UnlockContent()
        {
            if ((State & LockState.Content) != 0)
            {
                SetContent(Data.EncryptedContent.Deserialize(Parent?.EncryptionChain));
                if (_content == null)
                {
                    return false;
                }
                State &= ~LockState.Content;
            }

            return true;
        }

        public void LockContent()
        {
            if ((State & LockState.Content) == 0)
            {
                SetContent(null);
                State |= LockState.Content;
            }
        }

        public string? Name
        {
            get
            {
                UnlockName();
                return _name;
            }
        }
        
        public IContent? Content
        {
            get
            {
                UnlockContent();
                return _content;
            }
        }

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId.IsValid ? Repository.FindDirectory(Data.ParentId) : null;
    }
}