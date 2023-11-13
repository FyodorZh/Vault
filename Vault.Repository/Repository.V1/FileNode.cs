using Vault.Content;
using Vault.Storage;

namespace Vault.Repository.V1
{
    internal class FileNode : Node, IFileNode
    {
        public override ILockedState<IContent> Content { get; }

        public FileNode(IFileData data, IRepositoryCtl repository) 
            : base(data, repository)
        {
            Content = new FileContentState(this);
        }

        private class FileContentState : ContentState<IContent>
        {
            public FileContentState(FileNode node) 
                : base(node)
            {
            }

            protected override bool UnlockContent(IContent content)
            {
                return true;
            }
        }
    }
}