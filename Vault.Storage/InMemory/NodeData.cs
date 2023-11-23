using Vault.Content;

namespace Vault.Storage.InMemory
{
    public class NodeData : INodeData
    {
        public bool IsValid { get; set; }
        public NodeId Id { get; }
        public NodeId ParentId { get; }
        public IBox<StringContent> Name { get; set; }

        protected NodeData(NodeId id, NodeId parentId, 
            IBox<StringContent> encryptedName)
        {
            IsValid = true;
            Id = id;
            ParentId = parentId;
            Name = encryptedName;
        }
    }


}