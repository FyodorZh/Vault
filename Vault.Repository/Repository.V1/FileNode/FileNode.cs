using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class FileNode : Node, IFileNode
    {
        public new IFileData Data { get; }
        
        public override ILockableAspect<IContent> Content { get; }

        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
            Data = data;
            Content = new FileContentAspect(this);
        }
    }
}