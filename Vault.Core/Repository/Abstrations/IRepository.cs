namespace Vault.Repository
{
    public interface IRepository
    {
        IDirectoryNode InitNew();
        IDirectoryNode GetRoot();
    }
}