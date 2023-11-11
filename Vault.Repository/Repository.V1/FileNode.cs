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

        protected override bool ProcessContent(IContent? newContent)
        {
            return true;
        }
    }
}