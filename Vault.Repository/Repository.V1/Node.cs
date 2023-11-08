using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;
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
        }

        public bool IsValid => Data.IsValid;

        public Guid Id => Data.Id;

        public LockState State { get; protected set; } = LockState.Closed;

        public virtual void Unlock(LockState stateChange)
        {
            if ((stateChange & LockState.SelfName) != 0)
            {
                if ((State & LockState.SelfName) != 0)
                {
                    var decryptorsChain = new List<EncryptionSource>();
                    Parent!.CollectDecryptors(decryptorsChain);
                    _name = Data.EncryptedName.Deserialize(decryptorsChain)?.Content;
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
                    _name = null;
                    State |= LockState.SelfName;
                }
            }
        }

        string? INode.Name => _name;

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId != null ? Repository.FindDirectory(Data.ParentId.Value) : null;
    }
}