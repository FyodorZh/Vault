using System;
using Archivarius;
using Vault.Content;

namespace Vault.Storage.FileSystem
{
    public abstract class NodeData : INodeData
    {
        private NodeId _id;
        private NodeId _parentId;
        private IBox<StringContent> _name;

        public bool IsValid { get; set; }
        public NodeId Id => _id;
        public NodeId ParentId => _parentId;

        public IBox<StringContent> Name
        {
            get => _name;
            set => _name = value;
        }

        protected NodeData()
        {
            _name = new Box<StringContent>();
        }

        protected NodeData(NodeId id, NodeId parentId,
            IBox<StringContent> encryptedName)
        {
            IsValid = true;
            _id = id;
            _parentId = parentId;
            _name = encryptedName;
        }
    }
}