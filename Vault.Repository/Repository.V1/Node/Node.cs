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

        string? INode.Name => _name;

        public bool DecryptName(IEnumerable<Decryptor> decryptorsChain)
        {
            _name = Data.EncryptedName.Deserialize(decryptorsChain)?.Content;
            return _name != null;
        }

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId != null ? _repository.FindDirectory(Data.ParentId.Value) : null;
    }
}