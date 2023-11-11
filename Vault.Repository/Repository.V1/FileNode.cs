using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class FileNode : Node<IFileData>, IFileNode
    {
        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
        }

        protected override void OnContentChanged(IContent? newContent)
        {
            // DO NOTHING
        }
    }
}