using Vault.Content;

namespace Vault.Repository.V1
{
    internal class FileContentState : ContentState<IContent>
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