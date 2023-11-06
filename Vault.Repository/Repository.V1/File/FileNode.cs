using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class FileNode : Node<IFileData>, IFileNode
    {
        private IContent _content;

        public IContent Content => _content;
        
        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
            //_content = content;
        }
    }
}