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
        protected readonly IRepositoryCtl _repository;

        private string? _name;

        protected TNodeData Data { get; }

        protected Node(TNodeData data, IRepositoryCtl repository)
        {
            Data = data;
            _repository = repository;
        }

        public bool IsValid => Data.IsValid;

        public Guid Id => Data.Id;

        public LockState State { get; protected set; } = LockState.Closed;

        public virtual void Unlock(LockState stateChange)
        {
            if ((stateChange & LockState.Name) != 0)
            {
                if ((State & LockState.Name) != 0)
                {
                    var decryptorsChain = new List<Decryptor>();
                    Parent!.CollectDecryptors(decryptorsChain);
                    _name = Data.EncryptedName.Deserialize(decryptorsChain)?.Content;
                    State &= ~LockState.Name;
                }
            }
        }

        public virtual void Lock(LockState stateChange)
        {
            if ((stateChange & LockState.Name) != 0)
            {
                if ((State & LockState.Name) == 0)
                {
                    _name = null;
                    State |= LockState.Name;
                }
            }
        }

        string? INode.Name => _name;

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId != null ? _repository.FindDirectory(Data.ParentId.Value) : null;
    }
}