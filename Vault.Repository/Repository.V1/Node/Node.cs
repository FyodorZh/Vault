using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal abstract class Node : INode
    {
        public INodeData Data { get; }
        
        public IRepositoryCtl Repository { get; }


        protected Node(INodeData data, IRepositoryCtl repository)
        {
            Data = data;
            Repository = repository;
        }

        public bool IsValid => Data.IsValid;

        public NodeId Id => Data.Id;

        public string Name
        {
            get
            {
                var chain = Parent?.ChildrenNames.ChildrenNameEncryptionChain;
                string? name = Data.EncryptedName.Deserialize(chain)?.Content;
                return name ?? Id.ToString();
            }
        }
        
        public abstract ILockableAspect<IContent> Content { get; }

        public virtual void LockAll()
        {
            Content.Lock();
        }

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId.IsValid ? Repository.FindDirectory(Data.ParentId) : null;
    }
}