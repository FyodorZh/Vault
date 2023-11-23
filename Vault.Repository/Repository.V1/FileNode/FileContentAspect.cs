using Vault.Content;

namespace Vault.Repository.V1
{
    public interface IFileContentAspect : ILockableAspect<IFileContent>
    {
    }
    
    internal class FileContentAspect : LockableAspect<IFileContent, IFileContent>, IFileContentAspect
    {
        private readonly FileNode _owner;
        
        public FileContentAspect(FileNode node) 
            : base(true)
        {
            _owner = node;
        }
        
        protected sealed override IFileContent? UnlockState()
        {
            return _owner.Data.FileContent.Deserialize(_owner.Parent?.ChildrenContent.ContentEncryptionChain);
        }
    }
}