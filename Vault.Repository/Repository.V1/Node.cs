using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal abstract class Node<TNodeData> : INode
        where TNodeData : class, INodeData
    {
        private string? _name;
        
        protected TNodeData Data { get; }
        
        protected IRepositoryCtl Repository { get; }

        protected Node(TNodeData data, IRepositoryCtl repository)
        {
            Data = data;
            Repository = repository;
            if (!data.ParentId.IsValid)
            {
                _name = Data.EncryptedName.Deserialize()?.Content;
                State &= ~LockState.SelfName;
            }
        }

        public bool IsValid => Data.IsValid;

        public NodeId Id => Data.Id;

        public LockState State { get; protected set; } = LockState.Closed;

        public virtual void Unlock(LockState stateChange)
        {
            if ((stateChange & LockState.SelfName) != 0)
            {
                if ((State & LockState.SelfName) != 0)
                {
                    _name = Data.EncryptedName.Deserialize(Parent?.ChildrenNameEncryptionChain)?.Content;
                    State &= ~LockState.SelfName;
                }
            }
        }

        public virtual void Lock(LockState stateChange)
        {
            if ((stateChange & LockState.SelfName) != 0)
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
        }

        string? INode.Name => _name;

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId.IsValid ? Repository.FindDirectory(Data.ParentId) : null;
    }
}