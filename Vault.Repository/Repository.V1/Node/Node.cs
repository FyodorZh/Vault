using System.Threading.Tasks;
using Vault.Content;
using Vault.Encryption;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal abstract class Node : INode
    {
        public INodeData Data { get; }
        
        public IRepositoryCtl Repository { get; }
        
        IDirectoryNode? INode.Parent => Parent;
        public DirectoryNode? Parent { get; }
        
        protected Node(INodeData data, DirectoryNode? parent, IRepositoryCtl repository)
        {
            Data = data;
            Repository = repository;
            Parent = parent;
        }

        public bool IsValid => Data.IsValid;

        public NodeId Id => Data.Id;

        public string GetName()
        {
            var chain = Parent?.ChildrenNames.ChildrenNameEncryptionChain ?? VoidEncryptionChain.Instance;
            string? name = Data.Name.Deserialize(chain)?.Content;
            return name ?? Id.ToString();
        }
        
        public abstract ILockableAspect<IContent> Content { get; }

        public virtual Task LockAll()
        {
            Content.Lock();
            return Task.CompletedTask;
        }
    }
}