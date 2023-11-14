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
            Name = new NameAspect(this);
        }

        public bool IsValid => Data.IsValid;

        public NodeId Id => Data.Id;

        public ILockableAspect<string> Name { get; }
        
        public abstract ILockableAspect<IContent> Content { get; }

        public virtual void LockAll()
        {
            Content.Lock();
            Name.Lock();
        }

        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent =>
            Data.ParentId.IsValid ? Repository.FindDirectory(Data.ParentId) : null;
    }
}