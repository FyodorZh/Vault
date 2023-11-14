using Vault.Content;

namespace Vault.Repository.V1
{
    internal class FileContentAspect : ContentAspect<IContent>
    {
        public FileContentAspect(FileNode node) 
            : base(node)
        {
        }

        protected override bool UnlockContent(IContent content)
        {
            return true;
        }
    }
}