using System;
using System.Threading.Tasks;
using Archivarius;
using Vault.Content;
using Vault.FileSystem;

namespace Vault.Storage.FileSystem
{
    public abstract class NodeData : INodeData
    {
        protected readonly IEntity _fsEntity;
        private readonly NodeDataModel _entityDataModel;

        NodeId INodeData.Id => _entityDataModel.Id;

        NodeId INodeData.ParentId => _entityDataModel.ParentId;

        bool INodeData.IsValid => _fsEntity.IsValid;

        IBox<StringContent> INodeData.Name => _entityDataModel.Name;

        protected NodeData(IEntity fsEntity, NodeDataModel dataModel)
        {
            _fsEntity = fsEntity;
            _entityDataModel = dataModel;
        }
        
        public IEntity FsEntity => _fsEntity;

        public async Task SetName(Box<StringContent> encryptedName)
        {
            _entityDataModel.Name = encryptedName;
            await _fsEntity.WriteModel(_entityDataModel);
        }

        public abstract class NodeDataModel : IVersionedDataStruct
        {
            public NodeId Id;
            public NodeId ParentId;
            public IBox<StringContent> Name = null!;
            
            public virtual void Serialize(ISerializer serializer)
            {
                serializer.AddStruct(ref Id);
                serializer.AddStruct(ref ParentId);
                serializer.AddClass(ref Name, () => throw new Exception());
            }

            public virtual byte Version => 0;
        }
    }
}