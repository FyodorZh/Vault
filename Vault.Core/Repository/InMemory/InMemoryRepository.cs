namespace Vault.Repository.InMemory
{
    public class InMemoryRepository : IRepository
    {
        private DirectoryNode _root;
        
        public InMemoryRepository()
        {
            _root = new DirectoryNode(null, "root");
        }

        #region IRepository

        IDirectoryNode IRepository.InitNew()
        {
            return _root = new DirectoryNode(null, "root");
        }
        
        IDirectoryNode IRepository.GetRoot()
        {
            return _root;
        }

        #endregion
    }
}