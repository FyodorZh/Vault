using System;
using Archivarius;
using Vault.Content;

namespace Vault.Storage.InMemory
{
    public abstract class NodeData : INodeData, IVersionedDataStruct
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

        public virtual void Serialize(ISerializer serializer)
        {
            if (serializer.IsWriter && !IsValid)
            {
                throw new InvalidOperationException();
            }

            serializer.AddStruct(ref _id);
            serializer.AddStruct(ref _parentId);
            serializer.AddClass(ref _name, () => throw new Exception());
        }

        public byte Version => 0;
    }


}